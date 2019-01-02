Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class SelectTaskNickName
    Dim fatherFrm As HandleJob
    Dim flogPanel As LogPanel
    Dim fEditLog As EditLog
    Sub New(ByVal _fatherFrm As HandleJob)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        fatherFrm = _fatherFrm
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Sub New(ByVal _logpanel As LogPanel)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        flogPanel = _logpanel
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Sub New(ByVal _fEditLog As EditLog)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        fEditLog = _fEditLog
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub SelectTaskNickName_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "单击记录或右键菜单选择任务记录"
        ' Me.MaximizeBox = False
        Me.TopMost = True
        Control.CheckForIllegalCrossThreadCalls = False
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Dim th As New Thread(AddressOf ini)
        th.Start()
    End Sub
    Private Sub ini()
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
        Me.Text = "获取任务列表……"
        Dim result As String = GetH(ServerUrl, "func=GetMyTask&token=" & token)
        '   Console.WriteLine(result)
        Me.Text = "单击记录或右键菜单选择任务记录"
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetTaskList()
            Exit Sub
        End If
        LVTask.Items.Clear()
        If result = "[]" Then
            Exit Sub
        End If
        LVTask.Visible = False
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
        LVTask.Visible = True
    End Sub

    Private Sub LVTask_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVTask.SelectedIndexChanged
        If LVTask.SelectedIndices.Count = 0 Then Return
        Dim itm As ListViewItem = LVTask.SelectedItems(0)
        Dim TaskNickName As String = itm.SubItems(2).Text
        Dim DeviceName As String = itm.SubItems(4).Text
        If IsNothing(fatherFrm) = False Then
            fatherFrm.txtTaskNickName.Text = TaskNickName
            fatherFrm.txtDeviceID.Text = DeviceName
        End If
        If IsNothing(flogPanel) = False Then
            flogPanel.txtTaskNickName.Text = TaskNickName
            flogPanel.txtDeviceNickName.Text = DeviceName
        End If
        If IsNothing(fEditLog) = False Then
            fEditLog.txtTaskNickName.Text = TaskNickName
            fEditLog.txtDeviceNickName.Text = DeviceName
        End If
        Me.Close()
    End Sub
End Class