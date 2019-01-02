Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class DutyLog
    Dim jobDt As DataTable
    Private Sub DutyLog_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        DTP1.Value = Now
        DTP2.Value = Now.AddDays(1)
        'txtTitle.Text = "标题"
        'txtContent.Text = "正文"
        'txtJob.Text = "专项任务"
        'txtAlarmJob.Text = "任务提醒"
        'txtFilePath.Text = "C:\Users\meizi\Desktop\附件测试.txt"
        Control.CheckForIllegalCrossThreadCalls = False
        Dim tha As New Thread(AddressOf ini)
        tha.Start()
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 45)  '0
        LVDetail.Columns.Add("录入时间", 100) '1
        LVDetail.Columns.Add("录入人员", 100) '2
        LVDetail.Columns.Add("工作来源", 100) '3
        LVDetail.Columns.Add("部门") '4
        LVDetail.Columns.Add("值班员", 100) '5
        LVDetail.Columns.Add("事件标题", 300) '6
        LVDetail.Columns.Add("事件内容", 200) '7
        LVDetail.Columns.Add("工作状态", 100) '8
        LVDetail.Columns.Add("开始时间", 150) '9
        LVDetail.Columns.Add("结束时间", 150) '10
        LVDetail.Columns.Add("专项任务", 200) '11
        LVDetail.Columns.Add("工作提醒", 100) '12
        LVDetail.Columns.Add("附件地址", 100) '13
        LVDetail.Columns.Add("执行设备", 100) '14
        LVDetail.Columns.Add("设备任务ID", 100) '15
        LVDetail.Columns.Add("设备任务进度", 100) '16
        LVDetail.Columns.Add("JobID", 50) '17

        ComboBox2.Items.Add("工作安排")
        ComboBox2.Items.Add("上级下达")
        ComboBox2.Items.Add("用户申报")
        ComboBox2.SelectedIndex = 0
        If myPower = 2 Then
            删除工作ToolStripMenuItem.Enabled = False
            Panel1.Visible = False
            GetJobList()
        Else
            GxZhiBanYuan()
            GetJobList()
        End If
    End Sub

    Private Sub GxZhiBanYuan()
        ComboBox1.Items.Clear()
        Dim result As String = GetServerResult("func=GetAllUser")
        If result = "[]" Then Return
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) Then Return
        For Each row As DataRow In dt.Rows
            Dim susr As String = row("usr")
            Dim power As String = row("power")
            Dim status As String = row("status")
            If status = "-1" Then status = "审核未通过"
            If status = "0" Then status = "待审核"
            If status = "1" Then status = "已审核"
            If power = "1" Then power = "领导"
            If power = "2" Then power = "值班员"
            If power = "9" Then power = "管理员"
            If status = "已审核" And power = "值班员" Then
                ComboBox1.Items.Add(susr)
            End If
        Next
        If ComboBox1.Items.Count > 0 Then ComboBox1.SelectedIndex = 0
    End Sub
    Private Sub GetJobList()
        Label11.Visible = True
        RadioButton4.Checked = True
        Dim result As String = GetServerResult("func=GetJobList")
        Label11.Visible = False
        LVDetail.Items.Clear()
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) Then Return
        If dt.Rows.Count = 0 Then Return
        jobDt = dt


        For Each row As DataRow In dt.Rows
            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            itm.SubItems.Add(row("Time"))
            itm.SubItems.Add(row("Submiter"))
            itm.SubItems.Add(row("JobFrom"))
            itm.SubItems.Add(row("Department"))
            itm.SubItems.Add(row("Worker"))
            itm.SubItems.Add(row("Title"))
            itm.SubItems.Add(row("Content"))
            itm.SubItems.Add(row("Status"))
            itm.SubItems.Add(row("StartTime"))
            itm.SubItems.Add(row("EndTime"))
            itm.SubItems.Add(row("Job"))
            itm.SubItems.Add(row("AlarmJob"))
            itm.SubItems.Add(row("FileUrl"))
            itm.SubItems.Add(row("DeviceID"))
            itm.SubItems.Add(row("TaskNickName"))

            itm.SubItems.Add("")
            itm.SubItems.Add(row("JobID"))
            itm.UseItemStyleForSubItems = True
            If row("Worker") = usr And row("Status") = "待处理" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Gold
                Next
            End If          
            LVDetail.Items.Add(itm)
        Next
    End Sub
    Private Sub ShowJobList(ByVal ShowStatus As String)
        If IsNothing(jobDt) Then Return
        LVDetail.Items.Clear()
        For Each row As DataRow In jobDt.Rows
            If ShowStatus <> "All" Then
                Dim stu As String = row("Status")
                If stu = ShowStatus Then
                    Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                    itm.SubItems.Add(row("Time"))
                    itm.SubItems.Add(row("Submiter"))
                    itm.SubItems.Add(row("JobFrom"))
                    itm.SubItems.Add(row("Department"))
                    itm.SubItems.Add(row("Worker"))
                    itm.SubItems.Add(row("Title"))
                    itm.SubItems.Add(row("Content"))
                    itm.SubItems.Add(row("Status"))
                    itm.SubItems.Add(row("StartTime"))
                    itm.SubItems.Add(row("EndTime"))
                    itm.SubItems.Add(row("Job"))
                    itm.SubItems.Add(row("AlarmJob"))
                    itm.SubItems.Add(row("FileUrl"))
                    itm.SubItems.Add(row("TaskNickName"))
                    itm.SubItems.Add(row("DeviceID"))
                    itm.SubItems.Add("")
                    itm.SubItems.Add(row("JobID"))
                    LVDetail.Items.Add(itm)
                End If
            Else
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(row("Time"))
                itm.SubItems.Add(row("Submiter"))
                itm.SubItems.Add(row("JobFrom"))
                itm.SubItems.Add(row("Department"))
                itm.SubItems.Add(row("Worker"))
                itm.SubItems.Add(row("Title"))
                itm.SubItems.Add(row("Content"))
                itm.SubItems.Add(row("Status"))
                itm.SubItems.Add(row("StartTime"))
                itm.SubItems.Add(row("EndTime"))
                itm.SubItems.Add(row("Job"))
                itm.SubItems.Add(row("AlarmJob"))
                itm.SubItems.Add(row("FileUrl"))
                itm.SubItems.Add(row("TaskNickName"))
                itm.SubItems.Add(row("DeviceID"))
                itm.SubItems.Add("")
                itm.SubItems.Add(row("JobID"))
                LVDetail.Items.Add(itm)
            End If
        Next
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Label2.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
    End Sub

    Private Sub Panel31_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel31.Click
        btn_submit_Click
    End Sub

    Private Sub Label9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label9.Click
       btn_submit_Click
    End Sub
    
    Private Sub btn_submit_Click()
        Dim Time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim JobFrom As String = ComboBox2.SelectedItem
        Dim Department As String = TextBox7.Text
        Dim Worker As String=ComboBox1.SelectedItem
        Dim Title As String = txtTitle.Text
        Dim Content As String = txtContent.Text
        Dim Status As String = "待处理"
        Dim StartTime As String = DTP1.Value.ToString("yyyy-MM-dd") & " " & txtHH1.Text & ":" & txtMM1.Text & ":" & txtSS1.Text
        Dim EndTime As String = DTP2.Value.ToString("yyyy-MM-dd") & " " & txtHH2.Text & ":" & txtMM2.Text & ":" & txtSS2.Text
        Dim Job As String = txtJob.Text
        Dim AlarmJob As String = txtAlarmJob.Text
        Dim DeviceID As String = ""
        Dim TaskNickName As String = ""
        Dim FileUrl As String = txtFilePath.Text
        If JobFrom = "" Then
            MsgBox("工作来源不可为空")
            Return
        End If
        If Department = "" Then
            MsgBox("部门不可为空")
            Return
        End If
        If Worker = "" Then
            MsgBox("值班员不可为空")
            Return
        End If
        If Title = "" Then
            MsgBox("标题不可为空")
            Return
        End If
        If Content = "" Then
            MsgBox("正文不可为空")
            Return
        End If
        Dim startDate As Date
        Dim endDate As Date
        Try
            startDate = Date.Parse(StartTime)
            endDate = Date.Parse(EndTime)
        Catch ex As Exception
            MsgBox("开始时间或结束时间格式有误")
            Return
        End Try
        If endDate <= startDate Then
            MsgBox("结束时间需大于开始时间")
            Return
        End If
        Dim jobs As jobStu
        jobs.Submiter = usr
        jobs.Time = Time
        jobs.JobFrom = JobFrom
        jobs.Department = Department
        jobs.Worker = Worker
        jobs.Title = Title
        jobs.Content = Content
        jobs.Status = Status
        jobs.StartTime = startDate.ToString("yyyy-MM-dd HH:mm:ss")
        jobs.EndTime = endDate.ToString("yyyy-MM-dd HH:mm:ss")
        jobs.Job = Job
        jobs.AlarmJob = AlarmJob
        jobs.DeviceID = DeviceID
        jobs.TaskNickName = TaskNickName
        jobs.fileName = ""
        jobs.fileBase64 = ""
        If FileUrl <> "" Then
            If File.Exists(FileUrl) Then              
                Dim f As New FileInfo(FileUrl)
                Dim fLen As Double = f.Length
                If fLen > 5 * 1024 * 1024 Then
                    MsgBox("附件不能超过5M")
                    Return
                End If
                jobs.fileName = f.Name
                jobs.fileBase64 = File2Base64(FileUrl)
            End If
        End If
        Dim json As String = JsonConvert.SerializeObject(jobs)
        Dim ps As PostStu
        ps.func = "AddJob"
        ps.token = token
        ps.msg = json
        json = JsonConvert.SerializeObject(ps)
        Dim result As String = PostServerResult(json)
        If GetResultPara("result", result) = "success" Then
            Dim w As New WarnBox("录入成功！")
            w.Show()
            GetJobList()
        Else
            Dim msg As String = GetResultPara("msg", result)
            Dim errMsg As String = GetResultPara("errMsg", result)
            Dim sb As New StringBuilder
            sb.AppendLine("录入失败")
            sb.AppendLine(msg)
            sb.AppendLine(errMsg)
            MsgBox(sb.ToString)
        End If
    End Sub
    Public Function File2Base64(ByVal filePath As String) As String
        If File.Exists(filePath) = False Then Return ""
        Dim f As New FileInfo(filePath)
        Dim fileLen As Double = f.Length
        Dim stream As New FileStream(filePath, FileMode.Open)
        Dim br As New BinaryReader(stream)
        Dim arr() As Byte = br.ReadBytes(fileLen)
        br.Close()
        stream.Close()
        Return Convert.ToBase64String(arr)
    End Function
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GxZhiBanYuan()
    End Sub

    Private Sub txtHH1_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHH1.Leave
        Dim str As String = txtHH1.Text
        If IsNumeric(str) = False Then str = "00"
        Dim int As Integer = Val(str)
        If int < 0 Then int = 0
        If int > 23 Then int = 23
        str = int.ToString("00")
        txtHH1.Text = str
    End Sub
    Private Sub txtHH2_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHH2.Leave
        Dim str As String = txtHH2.Text
        If IsNumeric(str) = False Then str = "00"
        Dim int As Integer = Val(str)
        If int < 0 Then int = 0
        If int > 23 Then int = 23
        str = int.ToString("00")
        txtHH2.Text = str
    End Sub
    Private Sub txtMM1_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtMM1.Leave
        Dim str As String = txtMM1.Text
        If IsNumeric(str) = False Then str = "00"
        Dim int As Integer = Val(str)
        If int < 0 Then int = 0
        If int > 59 Then int = 59
        str = int.ToString("00")
        txtMM1.Text = str
    End Sub
    Private Sub txtMM2_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtMM2.Leave
        Dim str As String = txtMM2.Text
        If IsNumeric(str) = False Then str = "00"
        Dim int As Integer = Val(str)
        If int < 0 Then int = 0
        If int > 59 Then int = 59
        str = int.ToString("00")
        txtMM2.Text = str
    End Sub
    Private Sub txtSS1_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSS1.Leave
        Dim str As String = txtSS1.Text
        If IsNumeric(str) = False Then str = "00"
        Dim int As Integer = Val(str)
        If int < 0 Then int = 0
        If int > 59 Then int = 59
        str = int.ToString("00")
        txtSS1.Text = str
    End Sub
    Private Sub txtSS2_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSS2.Leave
        Dim str As String = txtSS2.Text
        If IsNumeric(str) = False Then str = "00"
        Dim int As Integer = Val(str)
        If int < 0 Then int = 0
        If int > 59 Then int = 59
        str = int.ToString("00")
        txtSS2.Text = str
    End Sub

    Private Sub Panel31_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel31.Paint

    End Sub

    Private Sub Label20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label20.Click
        OFD_Click()
    End Sub
    Private Sub Panel43_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel43.Click
        OFD_Click()
    End Sub

    Private Sub OFD_Click()
        Dim OFD As New OpenFileDialog
        OFD.ShowDialog()
        Dim path As String = OFD.FileName
        If File.Exists(path) = False Then Return
        Dim f As New FileInfo(path)
        Dim fLen As Double = f.Length
        If fLen > 5 * 1024 * 1024 Then
            MsgBox("附件不能超过5M")
            Exit Sub
        End If
        txtFilePath.Text = path
    End Sub

  
    Private Sub Label10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label10.Click
        GetJobList()
    End Sub

    Private Sub Panel34_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel34.Click
        GetJobList()
    End Sub

   
    Private Sub Panel34_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel34.Paint

    End Sub

    Private Sub RadioButton4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton4.CheckedChanged
        If RadioButton4.Checked Then ShowJobList("All")
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        If RadioButton3.Checked Then ShowJobList("已处理")
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked Then ShowJobList("处理中")
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then ShowJobList("待处理")
    End Sub

  
    Private Sub 处理任务ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 处理任务ToolStripMenuItem.Click
        If LVDetail.SelectedIndices.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim Worker As String = itm.SubItems(5).Text
        Dim Submiter As String = itm.SubItems(2).Text
        If Worker <> usr And Submiter <> usr Then
            MsgBox("您不能编辑此任务")
            Exit Sub
        End If
        Dim JobID As String = itm.SubItems(LVDetail.Columns.Count - 1).Text
        Dim h As New HandleJob(JobID)
        h.Show()
    End Sub

    Private Sub 下载附件ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 下载附件ToolStripMenuItem.Click
        If LVDetail.SelectedIndices.Count = 0 Then Return
        Dim value As String = LVDetail.SelectedItems(0).SubItems(13).Text
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

    Private Sub 删除工作ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 删除工作ToolStripMenuItem.Click
        If LVDetail.SelectedIndices.Count = 0 Then Return
        Dim JobID As String = LVDetail.SelectedItems(0).SubItems(LVDetail.Columns.Count - 1).Text
        If MsgBox("是否确认删除该条记录?", MsgBoxStyle.YesNoCancel, "提示") <> MsgBoxResult.Yes Then
            Return
        End If
        Dim result As String = GetServerResult("func=DeleteJobByID&jobid=" & JobID)
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("result", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        If r = "success" Then
            Dim w As New WarnBox("删除成功!")
            w.Show()
            GetJobList()
        Else
            Dim sb As New StringBuilder
            sb.AppendLine("删除失败")
            sb.AppendLine(msg)
            sb.AppendLine(errmsg)
            MsgBox(sb.ToString)
        End If
    End Sub
End Class
