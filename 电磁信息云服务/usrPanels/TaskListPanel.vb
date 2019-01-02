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
Public Class TaskListPanel
    Private Sub TaskListPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        Label3.Visible = False
        LVTask.View = View.Details
        LVTask.GridLines = True
        LVTask.FullRowSelect = True
        LVTask.Columns.Add("序号")
        LVTask.Columns.Add("任务类别")
        LVTask.Columns.Add("任务名称", 200)
        LVTask.Columns.Add("设备ID", 150)
        LVTask.Columns.Add("设备名称", 150)
        LVTask.Columns.Add("开始时间", 150)
        LVTask.Columns.Add("结束时间", 150)
        LVTask.Columns.Add("完成状态", 150)
        LVTask.Columns.Add("下载报告", 500)
        GetTaskList()
    End Sub
    Public Sub GetTaskList()
        Label3.Visible = True
        Label3.Text = "获取中……"
        Dim result As String = GetH(ServerUrl, "func=GetMyTask&token=" & token)
        '   Console.WriteLine(result)
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetTaskList()
            Exit Sub
        End If
        LVTask.Items.Clear()
        Label3.Visible = False
        Label3.Text = "获取中……"
        If result = "[]" Then
            Exit Sub
        End If

        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) = False Then
            Dim dv As DataView = dt.DefaultView
            dv.Sort = "StartTime Asc"
            Dim dt2 As DataTable = dv.ToTable
            For Each row As DataRow In dt2.Rows
                Dim itm As New ListViewItem(LVTask.Items.Count + 1)
                itm.SubItems.Add(row("TaskName"))
                itm.SubItems.Add(row("TaskNickName"))
                itm.SubItems.Add(row("DeviceID"))
                itm.SubItems.Add(row("DeviceName").ToString)
                itm.SubItems.Add(row("StartTime"))
                itm.SubItems.Add(row("EndTime"))
                itm.SubItems.Add(row("OverPercent"))
                itm.SubItems.Add(row("ResultReportUrl"))
                LVTask.Items.Add(itm)
            Next
        End If
        Label3.Visible = False
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        If LVTask.SelectedItems.Count <= 0 Then Exit Sub
        Dim value As String = LVTask.SelectedItems(0).SubItems(8).Text
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

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetTaskList()
    End Sub

   
    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        AddTaskFrm.Show()
    End Sub

    Private Sub 删除任务ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 删除任务ToolStripMenuItem.Click
        If LVTask.SelectedItems.Count <= 0 Then Exit Sub
        If MsgBox("是否确认删除该任务记录?", MsgBoxStyle.YesNoCancel, "提示") <> MsgBoxResult.Yes Then
            Exit Sub
        End If
        Dim value As String = LVTask.SelectedItems(0).SubItems(7).Text
        '  If value = "" Then Exit Sub
        Dim itm As ListViewItem = LVTask.SelectedItems(0)
        Dim TaskName As String = itm.SubItems(1).Text
        Dim TaskNickName As String = itm.SubItems(2).Text
        Dim DeviceID As String = itm.SubItems(3).Text
        Dim StartTime As String = itm.SubItems(5).Text
        Dim EndTime As String = itm.SubItems(6).Text
        Dim dik As New Dictionary(Of String, String)
        dik.Add("token", token)
        dik.Add("func", "DeleteMyTask")
        dik.Add("TaskName", TaskName)
        dik.Add("TaskNickName", TaskNickName)
        dik.Add("deviceID", DeviceID)
        dik.Add("StartTime", StartTime)
        dik.Add("EndTime", EndTime)
        Dim msg As String = TransforPara2Query(dik)
        Dim url As String = ServerUrl
        Dim result As String = GetH(url, msg)
        If GetResultPara("result", result) = "success" Then
            MsgBox("删除成功！")
        Else
            MsgBox(result)
        End If
        GetTaskList()
    End Sub

    Private Sub Panel3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel3.Paint

    End Sub

    Private Sub LVTask_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVTask.SelectedIndexChanged

    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        searchBtnClick()
    End Sub
    Private Sub searchBtnClick()
        Dim txt As String = TextBox1.Text
        For Each itm As ListViewItem In LVTask.Items
            itm.UseItemStyleForSubItems = True
            For Each subitm As ListViewItem.ListViewSubItem In itm.SubItems
                subitm.BackColor = Color.White
            Next
        Next
        If txt = "" Then

            Return
        End If
        Dim selectIndex As Integer = -1
        For Each itm As ListViewItem In LVTask.Items
            itm.UseItemStyleForSubItems = True
            Dim isGreen As Boolean = False
            For Each subitm As ListViewItem.ListViewSubItem In itm.SubItems
                If InStr(subitm.Text, txt) > 0 Then
                    If selectIndex = -1 Then selectIndex = itm.Index
                    isGreen = True
                    Exit For
                End If
            Next
            If isGreen Then
                For Each subitm As ListViewItem.ListViewSubItem In itm.SubItems
                    subitm.BackColor = Color.Green
                Next
            End If
        Next
        If selectIndex > -1 Then LVTask.EnsureVisible(selectIndex)
    End Sub

    

    Private Sub TextBox1_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyUp
        If e.KeyCode = Keys.Enter Then
            searchBtnClick()
        End If
    End Sub
    
End Class
