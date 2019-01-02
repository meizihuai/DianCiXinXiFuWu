Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class WarnSelect
    Private WarnListDT As DataTable
    Dim flogPanel As LogPanel
    Sub New(ByVal _logpanel As LogPanel)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        flogPanel = _logpanel
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub WarnSelect_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
        LVWarn.View = View.Details
        LVWarn.GridLines = True
        LVWarn.FullRowSelect = True
        LVWarn.Columns.Add("序号", 50)
        LVWarn.Columns.Add("预警类别")
        LVWarn.Columns.Add("预警地点")
        LVWarn.Columns.Add("设备ID")
        LVWarn.Columns.Add("预警时间", 150)
        LVWarn.Columns.Add("预警字段", 150)
        LVWarn.Columns.Add("经度")
        LVWarn.Columns.Add("纬度")
        LVWarn.Columns.Add("消息字段")
        GetWarnList()
    End Sub
    Public Sub GetWarnList()
        Me.Text = "获取任务列表……"
        Dim result As String = GetH(ServerUrl, "func=GetWarn&startTime=2018-01-01 00:00:00&endTime=2019-01-01 00:00:00&startIndex=0&count=1000&token=" & token)
        Me.Text = "单击记录或右键菜单选择任务记录"
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetWarnList()
            Exit Sub
        End If

        LVWarn.Items.Clear()
        If result = "[]" Then
            Exit Sub
        End If
        LVWarn.Visible = False
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) = False Then
            Dim dv As DataView = dt.DefaultView
            dv.Sort = "MsgTime Asc"
            Dim dt2 As DataTable = dv.ToTable
            WarnListDT = New DataTable
            For Each col As DataColumn In dt2.Columns
                WarnListDT.Columns.Add(col.ColumnName)
            Next
            For i = 0 To dt2.Rows.Count - 1
                Dim row As DataRow = dt2.Rows(i)
                Dim r As DataRow = WarnListDT.NewRow
                For j = 0 To dt2.Columns.Count - 1
                    r(j) = row(j)
                Next
                WarnListDT.Rows.Add(r)
                Dim index As Integer = WarnListDT.Rows.Count - 1
                Dim itm As New ListViewItem(LVWarn.Items.Count + 1)
                itm.SubItems.Add(GetWarnKindName(row("WarnKind")))
                'itm.SubItems.Add(row("Address"))
                itm.SubItems.Add("九江市开发区")
                itm.SubItems.Add(row("DeviceID"))
                itm.SubItems.Add(row("MsgTime"))
                itm.SubItems.Add(WARN2Msg(row("DeviceMsg"), row("Address")))
                Dim DeviceMsg As String = row("DeviceMsg")
                Dim lng As String = ""
                Dim lat As String = ""
                If InStr(DeviceMsg, "WARN") Then
                    Dim strtmp As String = DeviceMsg
                      strTmp = strTmp.Substring(InStr(strTmp, "<"), InStr(strTmp, ">") - InStr(strTmp, "<") - 1)
                    Dim st() As String = strtmp.Split(",")
                    lng = st(3)
                    lat = st(4)
                End If
                itm.SubItems.Add(lng)
                itm.SubItems.Add(lat)
                itm.SubItems.Add(DeviceMsg)
                LVWarn.Items.Add(itm)
            Next
            LVWarn.Visible = True
        End If
    End Sub
    Private Function GetWarnKindName(ByVal kind As String) As String
        If kind = "GR" Then Return "干扰"
        If kind = "ZC" Then Return "违章"
        If kind = "WZ" Then Return "违章"
        If kind = "GZ" Then Return "故障"
        If kind = "KX" Then Return "空闲"
        If kind = "HGB" Then Return "黑广播"
    End Function
    Private Function WARN2Msg(ByVal strtmp As String, ByVal address As String) As String
        If InStr(strtmp, "WARN") Then
              strTmp = strTmp.Substring(InStr(strTmp, "<"), InStr(strTmp, ">") - InStr(strTmp, "<") - 1)
            Dim st() As String = strtmp.Split(",")
            Dim str As String
            Dim lx As String = ""
            If st(2) = "GR" Then lx = "干扰"
            If st(2) = "ZC" Then lx = "正常"
            If st(2) = "WZ" Then lx = "违章"
            If st(2) = "GZ" Then lx = "故障"
            If st(2) = "KX" Then lx = "空闲"
            If st(2) = "HGB" Then lx = "黑广播"
            Dim didian As String = "东莞火车站"
            address = "九江市开发区"
            If st(2) = "GR" Then
                str = address & "上报" & lx & "预警" & ",频率" & st(7) & "MHz场强" & st(8) & "dBm" & ",超标" & st(9) & "%" & ",干扰" & st(10) & "次"
            Else
                str = address & "上报" & lx & "预警" & ",频率" & st(7) & "MHz场强" & st(8) & "dBm" & ",超标" & st(9) & "%"
            End If
            Return str
        End If
    End Function

    Private Sub LVWarn_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVWarn.SelectedIndexChanged
        If LVWarn.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVWarn.SelectedItems(0)
        Dim warnMsg As String = itm.SubItems(5).Text
        If IsNothing(flogPanel) = False Then
            If flogPanel.RTB.Text = "" Then
                flogPanel.RTB.Text = warnMsg
            Else
                flogPanel.RTB.AppendText(vbLf & warnMsg)
            End If
        End If
        Me.Close()
    End Sub
End Class