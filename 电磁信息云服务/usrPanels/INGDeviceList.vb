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
Public Class INGDeviceList
    Private selectDeviceID As String
    Private Sub INGDeviceList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        Label2.Visible = False
        Label4.Visible = False
        ini()
        Dim th As New Thread(AddressOf GetINGDeviceList)
        th.Start()
    End Sub
    Private Sub ini()
        LVList.View = View.Details
        LVList.GridLines = True
        LVList.FullRowSelect = True
        LVList.Columns.Add("序号", 45)
        LVList.Columns.Add("设备名称", 80)
        LVList.Columns.Add("设备类型", 80)
        LVList.Columns.Add("地点", 80)
        LVList.Columns.Add("上线时间", 80)
        LVList.Columns.Add("经度", 80)
        LVList.Columns.Add("纬度", 80)
        LVList.Columns.Add("IP", 80)
        LVList.Columns.Add("Port", 80)

        LVinfo.View = View.Details
        LVinfo.GridLines = True
        LVinfo.FullRowSelect = True
        LVinfo.Columns.Add("项目", 100)
        LVinfo.Columns.Add("内容", 360)
        For i = 0 To LVList.Columns.Count - 1
            Dim text As String = LVList.Columns(i).Text
            Dim itm As New ListViewItem(text)
            itm.SubItems.Add("未选择设备")
            LVinfo.Items.Add(itm)
        Next

        LVDeviceList.View = View.Details
        LVDeviceList.GridLines = True
        LVDeviceList.FullRowSelect = True
        LVDeviceList.Columns.Add("序号", 45)
        LVDeviceList.Columns.Add("设备名称", 80)
        LVDeviceList.Columns.Add("设备类型", 80)
        LVDeviceList.Columns.Add("地点", 80)
        LVDeviceList.Columns.Add("上线时间", 80)
        LVDeviceList.Columns.Add("经度", 80)
        LVDeviceList.Columns.Add("纬度", 80)
        LVDeviceList.Columns.Add("IP", 80)
        LVDeviceList.Columns.Add("Port", 80)
    End Sub
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetINGDeviceList()
    End Sub
    Private Sub GetINGDeviceList()
        LVList.Items.Clear()
        LVDeviceList.Items.Clear()
        Label2.Visible = True
        Me.Invoke(Sub() Form1.GetOnlineDevice())
        Label2.Visible = False
        If IsNothing(alldevlist) Then Return
        For Each d In alldevlist
            If d.Kind = "ING" Then
                Dim itm As New ListViewItem(LVList.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                itm.SubItems(2).Text = "监测网关"
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                LVList.Items.Add(itm)
            End If
        Next
        If LVList.Items.Count > 0 Then BeSelect(0)
    End Sub
    Private Sub BeSelect(ByVal index As Integer)
        If index < 0 Then Return
        If index >= LVList.Items.Count Then Return
        Dim itm As ListViewItem = LVList.Items(index)
        For i = 0 To LVinfo.Items.Count - 1
            Dim infoItm As ListViewItem = LVinfo.Items(i)
            infoItm.SubItems(1).Text = itm.SubItems(i).Text
        Next
        selectDeviceID = itm.SubItems(1).Text
        GetINGDevice()
    End Sub

    Private Sub LVList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVList.SelectedIndexChanged
        If LVList.SelectedIndices.Count > 0 Then
            Dim index As Integer = LVList.SelectedIndices(0)
            BeSelect(index)
        End If
    End Sub
    Private Sub GetINGDevice()
        Dim id As String = selectDeviceID
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim INGMsg As INGMsgStu = New INGMsgStu("GetDevList", "")
        Dim json As String = JsonConvert.SerializeObject(INGMsg)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=CommandING&INGMsgText=" & json)
        Try
            Dim list As List(Of deviceStu) = JsonConvert.DeserializeObject(result, GetType(List(Of deviceStu)))
            LVDeviceList.Items.Clear()
            Label4.Visible = True
            Me.Invoke(Sub() Form1.GetOnlineDevice())
            Label4.Visible = False
            If IsNothing(alldevlist) Then Return
            For Each d In list
                Dim itm As New ListViewItem(LVDeviceList.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "ING" Then
                    itm.SubItems(2).Text = "监测网关"
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                End If
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                LVDeviceList.Items.Add(itm)
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        GetINGDevice()
        '  test()
    End Sub
  
    Private Sub test()
        Dim id As String = selectDeviceID
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim p As tssOrder_stu
        p.task = "stop"
        p.freqStart = 88
        p.freqEnd = 108
        p.freqStep = 25
        p.deviceID = selectDeviceID
        Dim orderMsg As String = JsonConvert.SerializeObject(p)
        Dim ido As New INGDeviceOrder("Tek180501", orderMsg)
        Dim mmm As String = JsonConvert.SerializeObject(ido)
        Dim INGMsg As INGMsgStu = New INGMsgStu("DeviceOrder", mmm)
        Dim json As String = JsonConvert.SerializeObject(INGMsg)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=CommandING&INGMsgText=" & json)
        MsgBox(result)
        'Dim id As String = selectDeviceID
        'sendMsgToDev(id, "<TZBQ:STOP," & id & ">")
    End Sub
    Private Sub test2()
        Dim id As String = selectDeviceID
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim orderMsg As String = "<TZBQ:STOP," & 4 & ">"
        Dim ido As New INGDeviceOrder("4", orderMsg)
        Dim mmm As String = JsonConvert.SerializeObject(ido)
        Dim INGMsg As INGMsgStu = New INGMsgStu("DeviceOrder", mmm)
        Dim json As String = JsonConvert.SerializeObject(INGMsg)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=CommandING&INGMsgText=" & json)
        MsgBox(result)
    End Sub
End Class
