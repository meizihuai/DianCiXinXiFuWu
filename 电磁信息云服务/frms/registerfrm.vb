Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Net.NetworkInformation
Public Class registerfrm

    Private Sub registerfrm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MaximizeBox = False
        Me.TopMost = True
        Me.MinimizeBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Me.Left = loginfrm.Left + loginfrm.Width / 2 - Me.Width / 2
        Me.Top = loginfrm.Top + loginfrm.Height / 2 - Me.Height / 2
        ComboBox1.Items.Add("值班员")
        ComboBox1.Items.Add("领导")      
        ComboBox1.SelectedIndex = 0
        TextBox2.PasswordChar = "*"
        TextBox3.PasswordChar = "*"
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim rusr As String = TextBox1.Text
        Dim rpwd As String = TextBox2.Text
        Dim rcpwd As String = TextBox3.Text
        If ComboBox1.SelectedIndex < 0 Then
            MsgBox("请选择岗位")
            Return
        End If
        Dim rPower As String = ComboBox1.SelectedItem
        Dim rstatus As Integer = 0
        If rPower = "领导" Then rPower = 1
        If rPower = "值班员" Then rPower = 2
        If rusr = "" Then
            MsgBox("请输入用户名")
            Return
        End If
        If InStr(rusr, "&") Or InStr(rusr, "=") Or InStr(rusr, "#") Then
            MsgBox("用户名不能包含 &  =  # 等特出字符")
            Return
        End If
        If Len(rusr) > 10 Then
            MsgBox("用户名不能超过10个字符")
            Return
        End If
        If rpwd = "" Then
            MsgBox("密码不能为空")
            Return
        End If
        If InStr(rpwd, "&") Or InStr(rpwd, "=") Or InStr(rpwd, "#") Then
            MsgBox("密码不能包含 &  =  # 等特出字符")
            Return
        End If
        If rcpwd <> rpwd Then
            MsgBox("两次输入密码不一致")
            Return
        End If
        Dim dik As New Dictionary(Of String, String)
        dik.Add("func", "regist")
        dik.Add("rusr", rusr)
        dik.Add("rpwd", rpwd)
        dik.Add("rpower", rPower)
        Dim str As String = TransforPara2Query(dik)
        Dim result As String = GetH(ServerUrl, str)
        Dim r As String = GetNorResult("result", result)
        If r = "success" Then
            Dim msg As String = GetNorResult("msg", result)
            Dim errmsg As String = GetNorResult("errmsg", result)
            Dim sb As New StringBuilder
            sb.AppendLine("注册成功!")
            sb.AppendLine(msg)
            sb.AppendLine(errmsg)
            MsgBox(sb.ToString)
            Me.Close()
        Else
            Dim msg As String = GetNorResult("msg", result)
            Dim errmsg As String = GetNorResult("errmsg", result)
            Dim sb As New StringBuilder
            sb.AppendLine("注册失败")
            sb.AppendLine(msg)
            sb.AppendLine(errmsg)
            MsgBox(sb.ToString)
        End If
    End Sub
End Class