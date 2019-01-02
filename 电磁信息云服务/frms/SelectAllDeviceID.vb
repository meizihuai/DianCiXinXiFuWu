Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class SelectAllDeviceID
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
    Private Sub SelectAllDeviceID_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "单击记录或右键菜单选择设备"
        ' Me.MaximizeBox = False
        Me.TopMost = True
        Control.CheckForIllegalCrossThreadCalls = False
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.TopMost = True
        Dim th As New Thread(AddressOf ini)
        th.Start()
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 45)
        LVDetail.Columns.Add("设备名称", 150)
        LVDetail.Columns.Add("设备类型", 80)
        LVDetail.Columns.Add("地点", 80)
        LVDetail.Columns.Add("上线时间", 80)
        LVDetail.Columns.Add("经度", 80)
        LVDetail.Columns.Add("纬度", 80)
        LVDetail.Columns.Add("IP", 80)
        LVDetail.Columns.Add("Port", 80)
        For Each d In alldevlist
            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            itm.SubItems.Add(d.Name)
            itm.SubItems.Add(d.Kind)
            itm.SubItems.Add(d.Address)
            itm.SubItems.Add(d.OnlineTime)
            itm.SubItems.Add(d.Lng)
            itm.SubItems.Add(d.Lat)
            If d.Kind = "TSS" Then
                itm.SubItems(2).Text = "频谱传感器"              
            End If
            If d.Kind = "TZBQ" Then
                itm.SubItems(2).Text = "微型传感器"
            End If
            itm.SubItems.Add(d.IP)
            itm.SubItems.Add(d.Port)
            LVDetail.Items.Add(itm)
        Next
    End Sub

    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedIndices.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim DeviceName As String = itm.SubItems(1).Text
        If IsNothing(fatherFrm) = False Then
            fatherFrm.txtDeviceID.Text = DeviceName
        End If
        If IsNothing(flogPanel) = False Then
            flogPanel.txtDeviceNickName.Text = DeviceName
        End If
        If IsNothing(fEditLog) = False Then
            fEditLog.txtDeviceNickName.Text = DeviceName
        End If
        Me.Close()
    End Sub
End Class