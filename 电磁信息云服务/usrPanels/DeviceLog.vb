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
Public Class DeviceLog

    Private Sub DeviceLog_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        ini()
        'DTP.MaxDate = Now.AddDays(-1).ToString("yyyy-MM-dd")
        'DTP2.MaxDate = Now.ToString("yyyy-MM-dd")
        DTP.Value = Now.AddDays(-1)
        DTP2.Value = Now
        'DTP.Value = Now
        'DTP.MinDate = "2018-03-17"
        GetAllDBDeviceList()

    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 45)
        LVDetail.Columns.Add("设备名称", 150)
        LVDetail.Columns.Add("设备类型", 90)
        LVDetail.Columns.Add("设备ID", 50)

        LVDeviceLog.View = View.Details
        LVDeviceLog.GridLines = True
        LVDeviceLog.FullRowSelect = True
        LVDeviceLog.Columns.Add("序号", 45)
        LVDeviceLog.Columns.Add("设备ID", 50)
        LVDeviceLog.Columns.Add("台站名称", 150)
        LVDeviceLog.Columns.Add("时间", 150)
        LVDeviceLog.Columns.Add("地点", 150)
        LVDeviceLog.Columns.Add("发生事件", 200)
        LVDeviceLog.Columns.Add("执行结果", 150)
        LVDeviceLog.Columns.Add("设备状态", 150)
    End Sub
    Private Sub GetAllDBDeviceList()
        Label2.Visible = True
        Dim resutl As String = GetH(ServerUrl, "func=GetAllDBDevlist&token=" & token)
        If GetResultPara("result", resutl) = "fail" Then
            If GetResultPara("msg", resutl) = "Please login" Then
                Login()
                resutl = GetH(ServerUrl, "func=GetAllDBDevlist&token=" & token)
            End If
        End If
        Label2.Visible = False
        If resutl = "" Then
            MsgBox("网络错误，请检查您的网络")
            Exit Sub
        End If
        Dim dt As DataTable = JsonConvert.DeserializeObject(resutl, GetType(DataTable))
        If IsNothing(dt) = False Then
            LVDetail.Items.Clear()
            Dim index As Integer = 0
            For Each row As DataRow In dt.Rows
                Dim kind As String = row("Kind")
                index = index + 1
                Dim itm As New ListViewItem(index)

                itm.SubItems.Add(row("DeviceNickName"))
                If kind = "TZBQ" Then
                    itm.SubItems.Add("微型传感器")
                End If
                If kind = "TSS" Then
                    itm.SubItems.Add("频谱传感器")
                End If
                itm.SubItems.Add(row("DeviceID"))
                LVDetail.Items.Add(itm)
                If row("DeviceNickName") = "上海静安花园0001" Then
                    lv_BClick(LVDetail.Items.Count - 1)
                End If
            Next
            If dt.Rows.Count = 0 Then Return
            Label6.Text = dt.Rows(0)("DeviceNickName")
            'GetDeviceLogByNickName()
        End If
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetAllDBDeviceList()
    End Sub

  
    Private Sub Label10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label10.Click
        GetDeviceLogByNickName()
    End Sub

    Private Sub Panel34_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel34.Click
        GetDeviceLogByNickName()
    End Sub

    Private Sub GetAllDeviceLog()
        ' Try\

        LVDeviceLog.Items.Clear()
        Dim result As String = GetServerResult("func=GetAllDeviceLog")
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) Then Return
        If dt.Rows.Count = 0 Then Return
        LVDeviceLog.Items.Clear()
        For i = 0 To dt.Rows.Count - 1
            Dim row As DataRow = dt.Rows(i)
            Dim itm As New ListViewItem(i + 1)
            itm.SubItems.Add(row("DeviceID"))
            itm.SubItems.Add(row("DeviceNickName"))
            itm.SubItems.Add(row("Time"))
            itm.SubItems.Add(row("Address"))
            itm.SubItems.Add(row("Log"))
            itm.SubItems.Add(row("Result"))
            itm.SubItems.Add(row("Status"))
            LVDeviceLog.Items.Add(itm)
          
        Next

        ' Catch ex As Exception

        ' End Try
    End Sub
    Private Sub GetDeviceLogByNickName()
        ' Try
        If Label6.Text = "未选择" Or Label6.Text = "" Then
            MsgBox("请选择设备")
            Return
        End If
        LVDeviceLog.Items.Clear()
        Dim nickName As String = Label6.Text
        Dim sdate As String = DTP.Value.ToString("yyyy-MM-dd")
        Dim edate As String = DTP2.Value.ToString("yyyy-MM-dd")
        Dim str As String = "func=GetDeviceLogByNickNameWithTimeRegion&nickname=" & nickName & "&startTime=" & sdate & "&endTime=" & edate
        Dim result As String = GetServerResult(str)
        Try
            Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
            If IsNothing(dt) Then Return
            If dt.Rows.Count = 0 Then Return
            LVDeviceLog.Items.Clear()
            For i = 0 To dt.Rows.Count - 1
                Dim row As DataRow = dt.Rows(i)
                Dim itm As New ListViewItem(i + 1)
                itm.SubItems.Add(row("DeviceID"))
                itm.SubItems.Add(row("DeviceNickName"))
                itm.SubItems.Add(row("Time"))
                itm.SubItems.Add(row("Address"))
                itm.SubItems.Add(row("Log"))
                itm.SubItems.Add(row("Result"))
                itm.SubItems.Add(row("Status"))
                LVDeviceLog.Items.Add(itm)
            Next
        Catch ex As Exception

        End Try
        
        ' Catch ex As Exception

        ' End Try
    End Sub
  
    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedItems.Count <= 0 Then Return
        lv_BClick(LVDetail.SelectedIndices(0))
    End Sub
    Private Sub lv_BClick(ByVal index As Integer)
        If index < 0 Then Return
        If index >= LVDetail.Items.Count Then Return
        If LVDetail.Items.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.Items(index)
        Label6.Text = itm.SubItems(1).Text
        GetDeviceLogByNickName()
    End Sub

    Private Sub Panel3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel3.Paint

    End Sub
End Class
