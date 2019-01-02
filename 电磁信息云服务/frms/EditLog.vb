Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class EditLog
    Dim LogID As String
    Dim isEdit As Boolean
    Sub New(ByVal _LogID As String, ByVal _isEdit As Boolean)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        LogID = _LogID
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub EditLog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MaximizeBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Control.CheckForIllegalCrossThreadCalls = False
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.TopMost = True
        Me.Text = "编辑日志"
        If LogID = "" Then
            MsgBox("LogID不能为空")
            Me.Close()
        End If
        If isEdit = False Then
            Panel7.Visible = False
            Me.Text = "日志查看"
        End If
        Dim th As New Thread(AddressOf ini)
        th.Start()
    End Sub
    Private Sub ini()
        CBCata.Items.Add("领导批示")
        CBCata.Items.Add("值班员记事")
        ' CBCata.Items.Add("系统自动上报")
        If myPower = 2 Then
            CBCata.SelectedIndex = 1
            CBCata.Enabled = False
        Else
            CBCata.SelectedIndex = 0
            CBCata.Enabled = True
        End If
        GetLogInfo()
    End Sub
    Private Sub GetLogInfo()
        Me.Text = "获取日志详情……"
        Dim result As String = GetServerResult("func=GetLogByID&logid=" & LogID)
        If isEdit = False Then
            Me.Text = "日志查看"
        End If
        If result = "[]" Then
            MsgBox("获取详情失败")
            Me.Close()
        End If
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) Then
            MsgBox("获取详情失败")
            Me.Close()
        End If
        If dt.Rows.Count = 0 Then
            MsgBox("获取详情失败")
            Me.Close()
        End If
        Dim row As DataRow = dt.Rows(0)
        CBCata.SelectedItem = row("Cata")
        CBKind.SelectedItem = row("Kind")
        CBCata.Items.Clear()
        CBCata.Text = row("Cata")
        CBKind.Items.Clear()
        CBKind.Text = row("Kind")
        RTB.Text = row("Content")
        txtDeviceNickName.Text = row("DeviceNickName")
        txtTaskNickName.Text = row("TaskNickName")
        lblUsr.Text = row("Usr")
        Label1.Text = row("Time")
    End Sub
    Private Sub Label11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label11.Click
        Me.Close()
    End Sub

    Private Sub Panel2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel2.Click
        Me.Close()
    End Sub


    Private Sub CBCata_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBCata.SelectedIndexChanged
        CBKind.Items.Clear()
        If CBCata.SelectedIndex = 0 Then
            CBKind.Items.Add("工作安排")
            CBKind.Items.Add("交办事宜")
            CBKind.Items.Add("其他")
            CBKind.SelectedIndex = 0
        End If
        If CBCata.SelectedIndex = 1 Then
            CBKind.Items.Add("承办工作")
            CBKind.Items.Add("电话记录")
            CBKind.Items.Add("工作计划")
            CBKind.Items.Add("预订计划")
            CBKind.Items.Add("专项任务")
            CBKind.Items.Add("用户投诉")
            CBKind.Items.Add("微信转录")
            CBKind.Items.Add("预警处理")
            CBKind.Items.Add("其他")
            CBKind.SelectedIndex = 0
        End If
    End Sub

    Private Sub Label9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label9.Click
        Btn_submit_click()
    End Sub

    Private Sub Panel31_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel31.Click
        Btn_submit_click()
    End Sub

    Private Sub Btn_submit_click()
        Dim s As logstu
        s.Cata = CBCata.SelectedItem
        s.Kind = CBKind.SelectedItem
        s.Content = RTB.Text
        s.DeviceNickName = txtDeviceNickName.Text
        s.DeviceID = GetDeviceIDByName(s.DeviceNickName)
        s.TaskNickName = txtTaskNickName.Text
        s.LogID = LogID
        Dim json As String = JsonConvert.SerializeObject(s)
        Dim p As PostStu
        p.func = "UpdateLogInfo"
        p.msg = json
        p.token = token
        Dim result As String = PostServerResult(JsonConvert.SerializeObject(p))
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errMsg", result)
        If r = "success" Then
            Dim w As New WarnBox("修改成功")
            w.Show()
            Me.Close()
        Else
            Dim sb As New StringBuilder
            sb.AppendLine("录入失败")
            sb.AppendLine(msg)
            sb.AppendLine(errmsg)
            MsgBox(sb.ToString)
        End If
    End Sub

    Private Sub Label10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label10.Click
        Dim s As New SelectAllDeviceID(Me)
        s.Show()
    End Sub

    Private Sub Panel35_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel35.Click
        Dim s As New SelectAllDeviceID(Me)
        s.Show()
    End Sub

    Private Sub Label12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label12.Click
        Dim s As New SelectTaskNickName(Me)
        s.Show()
    End Sub
    Private Sub Panel27_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel27.Click
        Dim s As New SelectTaskNickName(Me)
        s.Show()
    End Sub
End Class