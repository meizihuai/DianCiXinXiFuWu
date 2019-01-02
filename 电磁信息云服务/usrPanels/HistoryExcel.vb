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
Public Class HistoryExcel

    Private Sub HistoryExcel_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Label2.Visible = False
        Me.Dock = DockStyle.Fill
        Control.CheckForIllegalCrossThreadCalls = False
        ini()
    End Sub
    Private Sub ini()
        LVTask.View = View.Details
        LVTask.GridLines = True
        LVTask.FullRowSelect = True
        LVTask.Columns.Add("序号")
        LVTask.Columns.Add("文档名称", 450)
        LVTask.Columns.Add("文档对应任务类型", 150)
        LVTask.Columns.Add("文档对应任务备注", 200)
        LVTask.Columns.Add("任务执行设备ID", 150)
        LVTask.Columns.Add("任务执行设备名称", 150)
        LVTask.Columns.Add("任务开始时间", 150)
        LVTask.Columns.Add("任务结束时间", 150)
        LVTask.Columns.Add("任务完成状态", 150)
        LVTask.Columns.Add("报告地址", 500)
        GetTaskList()
    End Sub
    Private Sub GetTaskList()
        Label2.Visible = True
        Dim result As String = GetH(ServerUrl, "func=GetMyTask&token=" & token)
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetTaskList()
            Exit Sub
        End If

        Label2.Visible = False
        LVTask.Items.Clear()
        If result = "[]" Then
            Exit Sub
        End If
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) = False Then
            Dim dv As DataView = dt.DefaultView
            dv.Sort = "StartTime Asc"
            Dim dt2 As DataTable = dv.ToTable
            For Each row As DataRow In dt2.Rows
                If row("TaskName") = "频率监测" Then
                    Dim per As String = row("OverPercent")
                    If per = "100%" Then
                        Dim rru As String = row("ResultReportUrl")
                        Dim st() As String = rru.Split("/")
                        Dim fileName As String = st(st.Length - 1)
                        Dim itm As New ListViewItem(LVTask.Items.Count + 1)
                        itm.SubItems.Add(fileName)
                        itm.SubItems.Add(row("TaskName"))
                        itm.SubItems.Add(row("TaskNickName"))
                        itm.SubItems.Add(row("DeviceID"))
                        itm.SubItems.Add(row("DeviceName").ToString)
                        itm.SubItems.Add(row("StartTime"))
                        itm.SubItems.Add(row("EndTime"))
                        itm.SubItems.Add(row("OverPercent"))
                        itm.SubItems.Add(rru)
                        LVTask.Items.Add(itm)
                    End If
                End If
            Next
        End If
        Label2.Visible = False
    End Sub
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetTaskList()
    End Sub
    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        If LVTask.SelectedItems.Count <= 0 Then Exit Sub
        Dim value As String = LVTask.SelectedItems(0).SubItems(9).Text
        If value = "" Then Exit Sub
        If InStr(value, "http://") Then
            If ServerIP <> "123.207.31.37" Then
                value = value.Replace("123.207.31.37", ServerIP)
            End If
            Process.Start(value)
        Else
            MsgBox("下载路径有误，请检查")
        End If
    End Sub
End Class
