Imports System
Imports System.IO
Imports System.Text
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Threading
Imports System.Threading.Thread
Imports System.Net
Imports System.Net.NetworkInformation
Public Class loginfrm
    Dim usrlist As List(Of loginUsrStu)
    Dim title As String = "电磁信息云服务系统 登录 "
    Dim logLable As Label
    Dim usrPath As String = Application.StartupPath & "\usrconfig.ini"
    Structure loginUsrStu
        Dim usr As String
        Dim pwd As String
        Dim selected As Boolean
        Sub New(ByVal _usr As String, ByVal _pwd As String, ByVal _selected As Boolean)
            usr = _usr
            pwd = _pwd
            selected = _selected
        End Sub
    End Structure

    Private Sub loginfrm_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        End
    End Sub
    Public Sub ini()
        Dim path As String = Application.StartupPath & "\config.ini"
        If File.Exists(path) Then
            Dim sr As New StreamReader(path, Encoding.Default)
            Dim str As String = sr.ReadToEnd
            sr.Close()
            myConnectConfig = New myConfig("123.207.31.37", 8080)
            Try
                myConnectConfig = JsonConvert.DeserializeObject(str, GetType(myConfig))
            Catch ex As Exception
                myConnectConfig = New myConfig("123.207.31.37", 8080)
                Dim js As String = JsonConvert.SerializeObject(myConnectConfig)
                Dim sw As New StreamWriter(path, False, Encoding.Default)
                sw.Write(js)
                sw.Close()
            End Try
            Dim ip As String = myConnectConfig.serverIP
            ServerIP = ip
            ' remoteServerIP = ServerIP
            ServerUrl = "http://" & myConnectConfig.serverIP & ":" & myConnectConfig.serverPort & "/?"
            '  Label4.Text = "当前服务器地址:" & ServerIP
        Else
            myConnectConfig = New myConfig("123.207.31.37", 8080)
            Dim js As String = JsonConvert.SerializeObject(myConnectConfig)
            Dim sw As New StreamWriter(path, False, Encoding.Default)
            sw.Write(js)
            sw.Close()
        End If
        If ServerIP = "123.207.31.37" Then
            Me.Text = title & "  [ " & "云服务" & " ]"
        ElseIf ServerIP = "61.145.180.149" Then
            Me.Text = title & "  [ " & "东莞网" & " ]"
        Else
            Me.Text = title & "  [ " & ServerIP & " ]"
        End If
        Dim th As New Thread(AddressOf JianCha)
        th.Start()
    End Sub
    Private Sub loginfrm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        PictureBox1.BackColor = Color.Transparent
        PictureBox6.BackColor = Color.Transparent
        Control.CheckForIllegalCrossThreadCalls = False
        PictureBox1.Left = Me.Width / 2 - PictureBox1.Width / 2
        ComboBox1.Left = Me.Width / 2 - ComboBox1.Width / 2
        TextBox1.Left = Me.Width / 2 - TextBox1.Width / 2
        Panel34.Left = Me.Width / 2 - Panel34.Width / 2
        Label10.Left = Panel34.Width / 2 - Label10.Width / 2
        LinkLabel1.BackColor = Color.Transparent
        PictureBox2.BackColor = Color.Transparent
        Me.Text = title

        ini()
    End Sub
    Private Sub JianCha()

        Me.Invoke(Sub() Panel34.Enabled = False)
        logLable = New Label
        logLable.AutoSize = True
        logLable.BackColor = Color.Transparent
        logLable.Location = New Point(Panel34.Left, Panel34.Top + 50)
        logLable.ForeColor = Color.White
        Me.Invoke(Sub() Me.Controls.Add(logLable))
        logLable.Text = "正在检查公网连接……"
        Dim p As New Ping
        Dim ps As PingReply = p.Send("123.207.31.37")

        If ps.Status = IPStatus.Success Then

            logLable.Text = "正在检查必要文件……"

            CheckLocationDllFile("EPPlus.dll")
            CheckLocationDllFile("DocX.dll")
            CheckLocationDllFile("Microsoft.VisualBasic.PowerPacks.Vs.dll")
            CheckLocationDllFile("Newtonsoft.Json.dll")
            CheckLocationDllFile("update.exe")
            logLable.Text = "正在检查更新……"
            HandleUpdate()

        End If


        logLable.Text = "正在启动登录组件……"
        logLable.Visible = False
        TextBox1.PasswordChar = "*"
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        If File.Exists(usrPath) Then
            Dim sr As New StreamReader(usrPath, Encoding.Default)
            Dim str As String = sr.ReadToEnd
            sr.Close()
            Try
                usrlist = JsonConvert.DeserializeObject(str, GetType(List(Of loginUsrStu)))
                For Each itm In usrlist
                    ComboBox1.Items.Add(itm.usr)
                    If itm.selected Then
                        ComboBox1.SelectedItem = itm.usr
                    End If
                Next
                If ComboBox1.Items.Count > 0 Then
                    If ComboBox1.SelectedIndex < 0 Then
                        ComboBox1.SelectedIndex = 0
                    End If
                End If
            Catch ex As Exception
                File.Delete(usrPath)
                usrlist = Nothing
            End Try
        End If
        Me.Invoke(Sub() Panel34.Enabled = True)

    End Sub
    Private Sub HandleUpdate()
        Dim sw As New StreamWriter("updateinfo.ini", False, Encoding.Default)
        sw.Write(exeName)
        sw.Close()
        Dim updateurl As String = "http://123.207.31.37:8080/?func=autoUpdate&updateFunc=getupdate&exename=电磁信息云服务.exe&version=" & Version

        Dim tmp As String = GetH(updateurl, "")

        Dim result As String = GetNorResult("result", tmp)
        Dim msg As String = GetNorResult("msg", tmp)
        If result = "success" Then
            If File.Exists("update.exe") Then
                
                Shell("update.exe", AppWinStyle.NormalFocus)
                End
            End If

        End If
    End Sub

    Private Sub CheckLocationDllFile(ByVal fileName As String)
        Dim path As String = Application.StartupPath & "\" & fileName
        If File.Exists(path) = False Then
            logLable.Text = "正在下载必要组件……"
            Dim url As String = "http://123.207.31.37:8082/update/电磁信息服务/" & fileName
            Download(url, path)
        End If
    End Sub
    Private Sub Download(ByVal url As String, ByVal path As String)
        If File.Exists(path) Then File.Delete(path)
        While True
            Try
                Dim req As HttpWebRequest = WebRequest.Create(url)
                req.Accept = "*/*"
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
                req.KeepAlive = True
                req.ContentType = "application/x-www-form-urlencoded"
                req.Method = "GET"
                Dim rp As HttpWebResponse = req.GetResponse
                Dim sum As Integer = 0
                Dim buffer() As Byte
                While True
                    Dim by(102400000) As Byte
                    Dim num As Integer = rp.GetResponseStream.Read(by, 0, by.Count)
                    If num = 0 Then
                        Exit While
                    Else
                        sum = sum + num
                        If IsNothing(buffer) Then
                            ReDim buffer(num - 1)
                            Array.Copy(by, 0, buffer, 0, num)
                        Else
                            Dim bu(num - 1) As Byte
                            Array.Copy(by, 0, bu, 0, num)
                            buffer = buffer.Concat(bu).ToArray
                        End If
                    End If
                End While

                Dim stream As New FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)
                Dim bw As New BinaryWriter(stream)
                bw.Write(buffer)
                bw.Close()

                Exit While
            Catch ex As Exception

            End Try
        End While
    End Sub
    Private Sub loginfrm_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Dim g As Graphics = e.Graphics
        Dim FColor As Color = ColorTranslator.FromHtml("#5C5278")
        Dim TColor As Color = ColorTranslator.FromHtml("#4FA9B5")
        Dim b As Brush = New LinearGradientBrush(Me.ClientRectangle, FColor, TColor, LinearGradientMode.Vertical)
        g.FillRectangle(b, Me.ClientRectangle)
    End Sub

    Private Sub Panel34_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel34.Click
        sub_login()
    End Sub

    Private Sub Label10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label10.Click
        sub_login()
    End Sub

    Private Sub sub_login()
        Label10.Text = "正在登录……"
        Dim usr As String = ""
        Dim pwd As String = ""
        If ComboBox1.Items.Count = 0 Then
            usr = ComboBox1.Text
            pwd = TextBox1.Text
            usrlist = New List(Of loginUsrStu)
            usrlist.Add(New loginUsrStu(usr, pwd, True))
        Else
            If ComboBox1.SelectedIndex >= 0 Then
                usr = ComboBox1.SelectedItem
                pwd = TextBox1.Text
                For i = 0 To usrlist.Count - 1
                    Dim itm As loginUsrStu = usrlist(i)
                    If itm.usr = usr Then
                        itm.selected = True
                        itm.pwd = pwd
                    Else
                        itm.selected = False
                    End If
                    usrlist(i) = itm
                Next
            Else
                usr = ComboBox1.Text
                pwd = TextBox1.Text
                Dim isFind As Boolean = False
                For i = 0 To usrlist.Count - 1
                    Dim itm As loginUsrStu = usrlist(i)
                    If itm.usr = usr Then
                        itm.selected = True
                        itm.pwd = pwd
                        isFind = True
                    Else
                        itm.selected = False
                    End If
                    usrlist(i) = itm
                Next
                If isFind = False Then
                    usrlist.Add(New loginUsrStu(usr, pwd, True))
                End If
            End If
        End If

        If usr = "" Then
            Label10.Text = "登录"
            MsgBox("请输入用户名")
            Return
        End If
        If pwd = "" Then
            Label10.Text = "登录"
            MsgBox("请输入密码")
            Return
        End If
        Dim path As String = Application.StartupPath & "\config.ini"
        If File.Exists(path) Then
            Dim sr As New StreamReader(path, Encoding.Default)
            Dim str As String = sr.ReadToEnd
            sr.Close()
            myConnectConfig = New myConfig("123.207.31.37", 8080)
            Try
                myConnectConfig = JsonConvert.DeserializeObject(str, GetType(myConfig))
            Catch ex As Exception
                myConnectConfig = New myConfig("123.207.31.37", 8080)
                Dim js As String = JsonConvert.SerializeObject(myConnectConfig)
                Dim sw As New StreamWriter(path, False, Encoding.Default)
                sw.Write(js)
                sw.Close()
            End Try
            Dim ip As String = myConnectConfig.serverIP
            ServerIP = ip
            ' remoteServerIP = ServerIP
            ServerUrl = "http://" & myConnectConfig.serverIP & ":" & myConnectConfig.serverPort & "/?"
        Else
            myConnectConfig = New myConfig("123.207.31.37", 8080)
            Dim js As String = JsonConvert.SerializeObject(myConnectConfig)
            Dim sw As New StreamWriter(path, False, Encoding.Default)
            sw.Write(js)
            sw.Close()
        End If
        If ServerIP = "123.207.31.37" Then
            Me.Text = title & "  [ " & "云服务" & " ]"
        ElseIf ServerIP = "61.145.180.149" Then
            Me.Text = title & "  [ " & "东莞网" & " ]"
        Else
            Me.Text = title & "  [ " & ServerIP & " ]"
        End If

        Dim result As String = GetH(ServerUrl, "func=login&usr={0}&pwd={1}", New String() {usr, pwd})
        If GetResultPara("result", result) = "success" Then
            token = GetResultPara("token", result)
            Module1.usr = usr
            Module1.pwd = pwd
            SaveUsrList()
            Label10.Text = "登录"
            Me.Hide()
            Form1.Show()
        Else
            Dim msgt As String = GetNorResult("msg", result)
            Dim errmsg As String = GetNorResult("errmsg", result)
            Dim sb As New StringBuilder
            sb.AppendLine("登录失败")
            sb.AppendLine(msgt)
            sb.AppendLine(errmsg)
            MsgBox(sb.ToString)
            Label10.Text = "登录"
        End If

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If IsNothing(usrlist) Then Return
        If ComboBox1.SelectedIndex < 0 Then Return
        Dim str As String = ComboBox1.SelectedItem
        For i = 0 To usrlist.Count - 1
            Dim itm As loginUsrStu = usrlist(i)
            If itm.usr = str Then
                TextBox1.Text = itm.pwd
                Return
            End If
        Next
    End Sub
    Private Sub SaveUsrList()
        Dim sw As New StreamWriter(usrPath, False, Encoding.Default)
        Dim json As String = JsonConvert.SerializeObject(usrlist)
        sw.Write(json)
        sw.Close()
    End Sub

  
    Private Sub PictureBox2_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox2.MouseHover
        TextBox1.PasswordChar = ""
    End Sub

    Private Sub PictureBox2_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox2.MouseLeave
        TextBox1.PasswordChar = "*"
    End Sub

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            sub_login()
        End If
    End Sub



    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        registerfrm.Show()
    End Sub

    Private Sub Panel34_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel34.Paint

    End Sub

    Private Sub PictureBox6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox6.Click
        Dim k As New IpSettingFrm(True, Me)
        k.Show()
    End Sub
End Class