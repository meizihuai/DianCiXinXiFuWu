Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class HandleJob
    Dim JobID As String
    Sub New(ByVal _JobID As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        JobID = _JobID
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub HandleJob_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MaximizeBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Control.CheckForIllegalCrossThreadCalls = False
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.TopMost = True
        Me.Text = "处理任务"
        If myPower = 2 Then
            Panel31.Enabled = False
            txtTitle.Enabled = False
            txtContent.Enabled = False
            txtStartTime.Enabled = False
            txtEndTime.Enabled = False
            txtJob.Enabled = False
            txtAlarmJob.Enabled = False
            txtJobFrom.Enabled = False
            txtFilePath.Enabled = False
            CheckBox1.Checked = False
            CheckBox2.Visible = False
        Else
            CheckBox2.Visible = True
        End If
        Dim th As New Thread(AddressOf ini)
        th.Start()
    End Sub
    Private Sub ini()
        Me.Text = "获取工作详情……"
        Dim result As String = GetServerResult("func=GetJobByID&jobid=" & JobID)
        Me.Text = "处理任务"
        If result = "[]" Then
            MsgBox("获取任务详情失败")
            Me.Close()
            Return
        End If
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) Then
            MsgBox("获取任务详情失败")
            Me.Close()
            Return
        End If
        If dt.Rows.Count = 0 Then
            MsgBox("获取任务详情失败")
            Me.Close()
            Return
        End If
        Dim row As DataRow = dt.Rows(0)
        txtTitle.Text = row("Title")
        txtContent.Text = row("Content")
        txtStartTime.Text = row("StartTime")
        txtEndTime.Text = row("EndTime")
        txtJob.Text = row("Job")
        txtAlarmJob.Text = row("AlarmJob")
        txtJobFrom.Text = row("JobFrom")
        txtFilePath.Text = row("FileUrl")
        txtDeviceID.Text = row("DeviceID")
        txtTaskNickName.Text = row("TaskNickName")
        Label7.Text = "当前状态: " & row("Status")
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Panel31.Enabled = True
        Else
            Panel31.Enabled = False
        End If
    End Sub

    Private Sub Panel1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel1.Click
        Me.Close()
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click
        Me.Close()
    End Sub

    Private Sub txtFilePath_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtFilePath.TextChanged
        If txtFilePath.Text = "" Then
            Panel43.Enabled = False
        Else
            Panel43.Enabled = True
        End If
    End Sub

    Private Sub Label20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label20.Click
        Dim value As String = txtFilePath.Text
        If value = "" Then
            MsgBox("该任务没有附件")
            Return
        End If
        If InStr(value, "http://") Then
            If ServerIP <> "123.207.31.37" Then
                value = value.Replace("123.207.31.37", ServerIP)
            End If
            Process.Start(value)
        Else
            MsgBox("下载路径有误，请检查")
        End If
    End Sub

    Private Sub Panel43_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel43.Click
        Dim value As String = txtFilePath.Text
        If value = "" Then
            MsgBox("该任务没有附件")
            Return
        End If
        If InStr(value, "http://") Then
            If ServerIP <> "123.207.31.37" Then
                value = value.Replace("123.207.31.37", ServerIP)
            End If
            Process.Start(value)
        Else
            MsgBox("下载路径有误，请检查")
        End If
    End Sub

    Private Sub Label9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label9.Click
        btn_submit_click()
    End Sub

    Private Sub Panel31_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel31.Click
        btn_submit_click()
    End Sub
    Private Sub btn_submit_click()
        Dim jobs As jobStu
        jobs.JobID = JobID
        jobs.JobFrom = txtJobFrom.Text
        jobs.Title = txtTitle.Text
        jobs.Content = txtContent.Text
        If CheckBox1.Checked Then
            jobs.Status = "已处理"
        End If
        If CheckBox2.Checked Then
            jobs.Status = "已处理"
        End If
        jobs.StartTime = txtStartTime.Text
        jobs.EndTime = txtEndTime.Text
        jobs.Job = txtJob.Text
        jobs.AlarmJob = txtAlarmJob.Text
        jobs.DeviceID = txtDeviceID.Text
        jobs.TaskNickName = txtTaskNickName.Text
        Dim json As String = JsonConvert.SerializeObject(jobs)
        Dim ps As PostStu
        ps.func = "UpdateJobInfo"
        ps.token = token
        ps.msg = json
        json = JsonConvert.SerializeObject(ps)
        Dim result As String = PostServerResult(json)
        If GetResultPara("result", result) = "success" Then
            Dim w As New WarnBox("提交成功!")
            w.Show()
            Me.Close()
        Else
            Dim msg As String = GetResultPara("msg", result)
            Dim errMsg As String = GetResultPara("errMsg", result)
            Dim sb As New StringBuilder
            sb.AppendLine("提交失败")
            sb.AppendLine(msg)
            sb.AppendLine(errMsg)
            MsgBox(sb.ToString)
        End If
    End Sub
    Private Sub Panel31_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel31.Paint

    End Sub

    Private Sub Label6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label6.Click
        Dim s As New SelectAllDeviceID(Me)
        s.Show()
    End Sub

    Private Sub Panel35_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel35.Click
        Dim s As New SelectAllDeviceID(Me)
        s.Show()
    End Sub
    Private Sub Panel34_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel34.Click
        Dim s As New SelectTaskNickName(Me)
        s.Show()
    End Sub

   
    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label5.Click
        Dim s As New SelectTaskNickName(Me)
        s.Show()
    End Sub
End Class