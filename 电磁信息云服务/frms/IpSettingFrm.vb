Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Collections.Generic
Imports System
Imports System.Collections.Specialized
Imports System.Threading
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class IpSettingFrm
    Dim isLoginFrm As Boolean = False
    Dim lfrm As loginfrm = Nothing
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Sub New(ByVal _isLoginFrm As Boolean, ByVal _lfrm As loginfrm)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        isLoginFrm = _isLoginFrm
        lfrm = _lfrm
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub IpSettingFrm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.Text = "更改服务器地址"
        Dim tmp As String = ServerUrl.Replace("http://", "").Replace(":8080/?", "")
        tmp = myConnectConfig.serverIP
        TextBox2.Text = myConnectConfig.serverPort
        TextBox1.Text = tmp
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim ip As String = TextBox1.Text
        Dim port As Integer = Val(TextBox2.Text)
        Dim url As String = "http://" & ip & ":" & port & "/?"
        Dim str As String = url & "func=webtest"
        Try
            Dim result As String = GetHTest(url, "")
            MsgBox("地址可用！")
        Catch ex As Exception
            MsgBox("地址不可用！")
        End Try
    End Sub
    Public Function GetHTest(ByVal uri As String, ByVal msg As String) As String
        Dim req As HttpWebRequest = WebRequest.Create(uri & msg)
        req.Accept = "*/*"
        req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
        req.CookieContainer = New CookieContainer
        req.KeepAlive = True
        req.ContentType = "application/x-www-form-urlencoded"
        req.Timeout = 1000
        req.Method = "GET"
        Dim rp As HttpWebResponse = req.GetResponse
        Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd
        Return str
    End Function

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim ip As String = TextBox1.Text
        Dim port As Integer = Val(TextBox2.Text)
        Dim url As String = "http://" & ip & ":" & port & "/?"
        ServerUrl = url
        ServerIP = ip
        Dim path As String = Application.StartupPath & "\config.ini"
        myConnectConfig = New myConfig(ip, port)
        Dim js As String = JsonConvert.SerializeObject(myConnectConfig)
        Dim sw As New StreamWriter(path, False, Encoding.Default)
        sw.Write(js)
        sw.Close()
        '  Form1.Label4.Text = "当前服务器地址:" & ip
        If isLoginFrm = False Then
            Form1.thini()
        Else
            If IsNothing(lfrm) = False Then
                lfrm.ini()
            End If
        End If

        Me.Close()

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        TextBox1.Text = "123.207.31.37"
        TextBox2.Text = 8080
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        TextBox1.Text = "61.145.180.149"
        TextBox2.Text = 8081
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        TextBox1.Text = "192.168.0.215"
        TextBox2.Text = 8081
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        TextBox1.Text = "172.18.25.31"
        TextBox2.Text = 8081
    End Sub
End Class