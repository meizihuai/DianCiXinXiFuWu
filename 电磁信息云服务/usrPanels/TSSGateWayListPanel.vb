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
Public Class TSSGateWayListPanel

    Private Sub TSSGateWayListPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        ini()
        Dim th As New Thread(AddressOf GetTSSGateWay)
        th.Start()
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 45)
        LVDetail.Columns.Add("设备ID", 150)
        LVDetail.Columns.Add("设备名称", 150)
        LVDetail.Columns.Add("设备类型", 80)
        LVDetail.Columns.Add("关联设备ID", 150)
        LVDetail.Columns.Add("关联设备名称", 150)
        LVDetail.Columns.Add("当前网络路线", 150)
        LVDetail.Columns.Add("地点", 100)
        LVDetail.Columns.Add("是否在线", 80)
        LVDetail.Columns.Add("上线时间", 80)
        LVDetail.Columns.Add("经度", 80)
        LVDetail.Columns.Add("纬度", 80)
        LVDetail.Columns.Add("IP", 80)
        LVDetail.Columns.Add("Port", 80)
    End Sub
    Private Sub GetTSSGateWay()
        Label2.Visible = True
        Label2.Text = "查询中……"
        Form1.GetOnlineDevice()
        Label2.Visible = False
        LVDetail.Items.Clear()
        Dim isSelected As Boolean = False
        If IsNothing(alldevlist) Then Return
        For Each d In alldevlist
            If d.Kind = "TSSGateWay" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.DeviceID)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.NetDeviceID)
                itm.SubItems.Add("未知(可刷新多次查看)")
                If d.NetDeviceID <> "" Then
                    For Each sh In alldevlist
                        If sh.DeviceID = d.NetDeviceID Then
                            itm.SubItems(5).Text = sh.Name
                            Exit For
                        End If
                    Next
                End If
                If d.NetSwitch = 0 Then itm.SubItems.Add("未知")
                If d.NetSwitch = 1 Then itm.SubItems.Add("内网(云平台)")
                If d.NetSwitch = 2 Then itm.SubItems.Add("外网(局域网)")
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If isSelected = False Then
                    isSelected = True
                    lbl_SelectedTZBQID.Text = d.DeviceID
                    lbl_SelectedTZBQName.Text = d.Name
                   
                End If
                If d.Kind = "TSSGateWay" Then
                    itm.SubItems(3).Text = "TSS监测网关"
                End If

                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                LVDetail.Items.Add(itm)
            End If
        Next
        If LVDetail.Items.Count > 0 Then
            BeSelect(0)
        End If
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        Dim th As New Thread(AddressOf GetTSSGateWay)
        th.Start()
    End Sub
    Private Sub BeSelect(ByVal index As Integer)
        Dim itm As ListViewItem = LVDetail.Items(index)
        lbl_SelectedTZBQID.Text = itm.SubItems(1).Text
        lbl_SelectedTZBQName.Text = itm.SubItems(2).Text
        Label7.Text = itm.SubItems(4).Text
        Label8.Text = itm.SubItems(5).Text
        Label10.Text = itm.SubItems(6).Text
    End Sub
    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim index As Integer = LVDetail.SelectedIndices(0)
        BeSelect(index)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim id As String = lbl_SelectedTZBQID.Text
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=netswitchin")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim id As String = lbl_SelectedTZBQID.Text
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=netswitchout")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        '加电
         Dim id As String = lbl_SelectedTZBQID.Text
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=poweron")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        Else
            MsgBox(result)
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        '断电
         Dim id As String = lbl_SelectedTZBQID.Text
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=poweroff")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        Else
            MsgBox(result)
        End If
    End Sub
End Class
