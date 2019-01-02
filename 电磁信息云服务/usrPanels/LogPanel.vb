Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class LogPanel
   
    Private Sub LogPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        lblUsr.Text = usr
        Control.CheckForIllegalCrossThreadCalls = False
        Dim th As New Thread(AddressOf ini)
        th.Start()
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 50) '0
        LVDetail.Columns.Add("时间", 150) '1
        LVDetail.Columns.Add("用户名", 100) '2
        LVDetail.Columns.Add("日志类型", 150) '3
        LVDetail.Columns.Add("事件类型", 150) '4
        LVDetail.Columns.Add("事件内容", 500) '5
        LVDetail.Columns.Add("设备名称", 100) '6
        ''  LVDetail.Columns.Add("设备ID", 100)
        LVDetail.Columns.Add("设备任务名称", 100) '7
        LVDetail.Columns.Add("关联ID", 100) '8
        LVDetail.Columns.Add("事件流水号", 100) '9

        CBCata.Items.Add("领导批示")
        CBCata.Items.Add("值班员记事")
        '  CBCata.Items.Add("系统自动上报")
        If myPower = 2 Then
            CBCata.SelectedIndex = 1
            CBCata.Enabled = False
        Else
            CBCata.SelectedIndex = 0
            CBCata.Enabled = True
        End If
        CBCata.SelectedIndex = 1
        GetLogList()
    End Sub
    Private Sub GetLogList()
        Label11.Visible = True
        Dim startTime As String = DTP.Value.ToString("yyyy-MM-dd ") & "00:00:00"
        Dim endTime As String = DTP2.Value.ToString("yyyy-MM-dd ") & "23:59:59"
        Dim result As String = GetServerResult("func=GetLogListByDate&startTime=" & startTime & "&endTime=" & endTime)
        Label11.Visible = False
        LVDetail.Items.Clear()
        If result = "[]" Then Return
        LVDetail.Visible = False
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) Then Return
        If dt.Rows.Count = 0 Then Return
        For Each row As DataRow In dt.Rows
            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            itm.SubItems.Add(row("Time"))
            itm.SubItems.Add(row("Usr"))
            itm.SubItems.Add(row("Cata"))
            itm.SubItems.Add(row("Kind"))
            itm.SubItems.Add(row("Content"))
            itm.SubItems.Add(row("DeviceNickName"))
            'itm.SubItems.Add(row("DeviceID"))
            itm.SubItems.Add(row("TaskNickName"))
            itm.SubItems.Add(row("RelateID").ToString)
            itm.SubItems.Add(row("LogID"))
            LVDetail.Items.Add(itm)
        Next
        LVDetail.Visible = True
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Label1.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
    End Sub

    Private Sub Panel31_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel31.Click
        btn_submit_click()
    End Sub
    Private Sub Label9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label9.Click
        btn_submit_click()
    End Sub
   
    Private Sub btn_submit_click()
        Dim Time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim Cata As String = CBCata.SelectedItem
        Dim Kind As String = CBKind.SelectedItem
        Dim Content As String = RTB.Text
        Dim usr As String = lblUsr.Text
        Dim DeviceNickName As String = txtDeviceNickName.Text
        Dim DeviceID As String = GetDeviceIDByName(DeviceNickName)
        Dim TaskNickName As String = txtTaskNickName.Text
        If Cata = "" Or Kind = "" Or Content = "" Then
            MsgBox("来源类型，日志类型，事件内容不能为空")
            Return
        End If
        Dim s As logstu
        s.Time = Time
        s.Cata = Cata
        s.Kind = Kind
        s.Content = Content
        s.Usr = usr
        s.DeviceNickName = DeviceNickName
        s.DeviceID = DeviceID
        s.TaskNickName = TaskNickName
        Dim json As String = JsonConvert.SerializeObject(s)
        Dim p As PostStu
        p.func = "AddLog"
        p.msg = json
        p.token = token

        Dim result As String = PostServerResult(JsonConvert.SerializeObject(p))
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        If r = "success" Then
            Dim w As New WarnBox("录入成功")
            w.Show()
            GetLogList()
        Else
            Dim sb As New StringBuilder
            sb.AppendLine("录入失败")
            sb.AppendLine(msg)
            sb.AppendLine(errmsg)
            MsgBox(sb.ToString)
        End If
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


   
    Private Sub Label10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label10.Click
        Dim s As New SelectAllDeviceID(Me)
        s.Show()
    End Sub
    Private Sub Panel35_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Panel35.Click
        Dim s As New SelectAllDeviceID(Me)
        s.Show()
    End Sub
  
    Private Sub Label12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label12.Click
        Dim s As New SelectTaskNickName(Me)
        s.Show()
    End Sub

    Private Sub Panel27_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Panel27.Click
        Dim s As New SelectTaskNickName(Me)
        s.Show()
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetLogList()
    End Sub

    Private Sub 删除记录ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 删除记录ToolStripMenuItem.Click
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim LogID As String = itm.SubItems(LVDetail.Columns.Count - 1).Text
        Dim userName As String = itm.SubItems(2).Text
        If userName = "system" Then
            If usr = "admin" Then
                If MsgBox("重要提示！是否确认删除该条' 系统主动上报 '记录?", MsgBoxStyle.YesNoCancel, "重要提示") <> MsgBoxResult.Yes Then
                    Return
                End If
            Else
                If userName <> usr Then
                    MsgBox("您不能删除系统主动上报日志")
                    Return
                End If
            End If
        Else
            If userName <> usr Then
                MsgBox("您不能删除其他用户的记录")
                Return
            End If
        End If
        If MsgBox("是否确认删除该条记录?", MsgBoxStyle.YesNoCancel, "提示") <> MsgBoxResult.Yes Then
            Return
        End If
        Dim result As String = GetServerResult("func=DeleteLogByID&logid=" & LogID)
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        If r = "success" Then
            'Dim w As New WarnBox("删除成功")
            'w.Show()
            GetLogList()
        Else
            Dim sb As New StringBuilder
            sb.AppendLine("删除失败")
            sb.AppendLine(msg)
            sb.AppendLine(errmsg)
            MsgBox(sb.ToString)
        End If
    End Sub

    Private Sub 编辑记录ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 编辑记录ToolStripMenuItem.Click
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim LogID As String = itm.SubItems(LVDetail.Columns.Count - 1).Text
        Dim userName As String = itm.SubItems(2).Text
        If userName <> usr Then
            MsgBox("您不能编辑其他用户的记录")
            Return
        End If
        Dim w As New EditLog(LogID, True)
        w.Show()
    End Sub

    Private Sub DTP_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DTP.ValueChanged
        GetLogList()
    End Sub

    Private Sub Panel35_Paint(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Panel35.Paint

    End Sub

    Private Sub Panel35_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel35.Paint

    End Sub

    Private Sub Label13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label13.Click
        Dim w As New WarnSelect(Me)
        w.Show()
    End Sub

  
    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        Dim w As New WarnSelect(Me)
        w.Show()
    End Sub

    Private Sub Label17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label17.Click
        AddTaskFrm.Show()
    End Sub
    Private Sub Panel36_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Panel36.Click
        AddTaskFrm.Show()
    End Sub

    Private Sub 查看详情ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 查看详情ToolStripMenuItem.Click
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim LogID As String = itm.SubItems(LVDetail.Columns.Count - 1).Text
        Dim w As New EditLog(LogID, False)
        w.Show()
    End Sub

    Private Sub LVDetail_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles LVDetail.MouseDoubleClick
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim LogID As String = itm.SubItems(LVDetail.Columns.Count - 1).Text
        Dim w As New EditLog(LogID, False)
        w.Show()
    End Sub

  
    Private Sub 下载报告文件ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 下载报告文件ToolStripMenuItem.Click
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim Cata As String = itm.SubItems(3).Text
        Dim Kind As String = itm.SubItems(4).Text
        Dim RelateID As String = itm.SubItems(8).Text
        If RelateID = "" Then
            MsgBox("没有相应文件可下载")
        End If
        If Kind = "下发任务" Or Kind = "设备任务完成提示" Then
            Dim result As String = GetServerResult("func=GetTaskInfoByTaskNickName&TaskNickName=" & RelateID)
            If result = "[]" Then
                MsgBox("获取关联下载地址失败，请检查请求参数")
                Exit Sub
            End If
            Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
            If IsNothing(dt) Then
                MsgBox("获取关联下载地址失败，请检查请求参数")
                Exit Sub
            End If
            If dt.Rows.Count = 0 Then
                MsgBox("获取关联下载地址失败，请检查请求参数")
                Exit Sub
            End If
            Dim row As DataRow = dt.Rows(0)
            If IsNothing(row("OverPercent")) Then
                MsgBox("该任务未完成，请等待任务完成")
                Exit Sub
            End If
            If row("OverPercent") = "100%" Then
                If IsNothing(row("ResultReportUrl")) Then
                    MsgBox("该任务无报告")
                    Exit Sub
                End If
                Dim ResultReportUrl As String = row("ResultReportUrl").ToString
                If ResultReportUrl = "" Then
                    MsgBox("该任务无报告")
                    Exit Sub
                End If
                Dim value As String = ResultReportUrl
                If value = "" Then Exit Sub
                If InStr(value, "http://") Then
                    If ServerIP <> "123.207.31.37" Then
                        value = value.Replace("123.207.31.37", ServerIP)
                    End If
                    Process.Start(value)
                Else
                    MsgBox("下载路径有误，请检查")
                End If
            Else
                MsgBox("该任务未完成，请等待任务完成")
                Exit Sub
            End If
        End If
    End Sub
End Class
