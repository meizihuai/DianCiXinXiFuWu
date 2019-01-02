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
Public Class HistoryWord
    Dim itmList As List(Of ListViewItem)
    Private Sub HistoryWord_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            itmList = New List(Of ListViewItem)
            ComboBox1.Items.Clear()
            ComboBox1.Items.Add("全部")
            For Each row As DataRow In dt2.Rows
                If row("TaskName") <> "频率监测" Then
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
                        itmList.Add(itm)
                        Dim isfind As Boolean = False
                        For Each c In ComboBox1.Items
                            If c = row("TaskName") Then
                                isfind = True
                                Exit For
                            End If
                        Next
                        If isfind = False Then
                            ComboBox1.Items.Add(row("TaskName"))
                        End If
                    End If
                End If              
            Next
            If ComboBox1.Items.Count > 0 Then
                ComboBox1.SelectedIndex = 0
            End If
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

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex < 0 Then Return
        If ComboBox1.SelectedIndex = 0 Then
            LVTask.Items.Clear()
            For Each itm In itmList
                LVTask.Items.Add(itm)
            Next
        Else
            LVTask.Items.Clear()
            Dim TaskName As String = ComboBox1.SelectedItem
            For Each itm In itmList
                If itm.SubItems(2).Text = TaskName Then
                    LVTask.Items.Add(itm)
                End If
            Next
        End If
    End Sub
End Class
