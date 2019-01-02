Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports System.Data
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net
Imports System.Math
Public Class DeviceSetting

    Private Sub DeviceSetting_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        ini()
        iniDeviceList()
    End Sub
    Private Sub ini()
        LVTZBQ.View = View.Details
        LVTZBQ.GridLines = True
        LVTZBQ.FullRowSelect = True
        LVTZBQ.Columns.Add("序号", 45)
        LVTZBQ.Columns.Add("设备ID", 150)
        LVTZBQ.Columns.Add("设备名称", 150)
        LVTZBQ.Columns.Add("设备类型", 80)
        LVTZBQ.Columns.Add("地点", 100)
        LVTZBQ.Columns.Add("是否在线", 80)
        LVTZBQ.Columns.Add("上线时间", 80)
        LVTZBQ.Columns.Add("经度", 80)
        LVTZBQ.Columns.Add("纬度", 80)
        LVTZBQ.Columns.Add("IP", 80)
        LVTZBQ.Columns.Add("Port", 80)
        LVTSS.View = View.Details
        LVTSS.GridLines = True
        LVTSS.FullRowSelect = True
        LVTSS.Columns.Add("序号", 45)
        LVTSS.Columns.Add("设备ID", 150)
        LVTSS.Columns.Add("设备名称", 150)
        LVTSS.Columns.Add("设备类型", 80)
        LVTSS.Columns.Add("地点", 100)
        LVTSS.Columns.Add("是否在线", 80)
        LVTSS.Columns.Add("上线时间", 80)
        LVTSS.Columns.Add("经度", 80)
        LVTSS.Columns.Add("纬度", 80)
        LVTSS.Columns.Add("IP", 80)
        LVTSS.Columns.Add("Port", 80)
    End Sub
    Private Sub iniDeviceList()
        Form1.GetOnlineDevice()
        LVTZBQ.Items.Clear()
        Dim isSelected As Boolean = False
        If IsNothing(alldevlist) Then Return
        For Each d In alldevlist
            If d.Kind = "TZBQ" Then
                Dim itm As New ListViewItem(LVTZBQ.Items.Count + 1)
                itm.SubItems.Add(d.DeviceID)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If isSelected = False Then
                    isSelected = True
                    lbl_SelectedTZBQID.Text = d.DeviceID
                    lbl_SelectedTZBQName.Text = d.Name
                    TextBox9.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
                    TextBox4.Text = d.Name
                    TextBox8.Text = d.DeviceID
                    TextBox6.Text = d.Lng
                    TextBox7.Text = d.Lat
                End If
                If d.Kind = "TSS" Then
                    itm.SubItems(3).Text = "频谱传感器"
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(3).Text = "微型传感器"
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSS" Then itm.ForeColor = Color.Green
                    If d.Kind = "TZBQ" Then itm.ForeColor = Color.Blue
                Next
                LVTZBQ.Items.Add(itm)
            End If
        Next

        LVTSS.Items.Clear()
        isSelected = False
        For Each d In alldevlist
            If d.Kind = "TSS" Then
                Dim itm As New ListViewItem(LVTSS.Items.Count + 1)
                itm.SubItems.Add(d.DeviceID)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If isSelected = False Then
                    isSelected = True
                    lbl_SelectedTSSID.Text = d.DeviceID
                    lbl_SelectedTSSName.Text = d.Name
                    TextBox1.Text = d.Name
                End If
                If d.Kind = "TSS" Then
                    itm.SubItems(3).Text = "频谱传感器"
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(3).Text = "微型传感器"
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSS" Then itm.ForeColor = Color.Green
                    If d.Kind = "TZBQ" Then itm.ForeColor = Color.Blue
                Next
                LVTSS.Items.Add(itm)
            End If
        Next
        LVTSS.Visible = True
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        iniDeviceList()
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        iniDeviceList()
    End Sub
    '0   LVTSS.Columns.Add("序号", 45)
    '1   LVTZBQ.Columns.Add("设备ID", 150)
    '2   LVTSS.Columns.Add("设备名称", 150)
    '3   LVTSS.Columns.Add("设备类型", 80)
    '4   LVTSS.Columns.Add("地点", 100)
    '5   LVTSS.Columns.Add("是否在线", 80)
    '6   LVTSS.Columns.Add("上线时间", 80)
    '7   LVTSS.Columns.Add("经度", 80)
    '8   LVTSS.Columns.Add("纬度", 80)

    Private Sub LVTZBQ_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVTZBQ.SelectedIndexChanged
        If LVTZBQ.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVTZBQ.SelectedItems(0)
        lbl_SelectedTZBQID.Text = itm.SubItems(1).Text
        lbl_SelectedTZBQName.Text = itm.SubItems(2).Text
        TextBox9.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        TextBox4.Text = itm.SubItems(2).Text
        TextBox8.Text = itm.SubItems(1).Text
        TextBox6.Text = itm.SubItems(7).Text
        TextBox7.Text = itm.SubItems(8).Text
    End Sub

    Private Sub LVTSS_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVTSS.SelectedIndexChanged
        If LVTSS.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVTSS.SelectedItems(0)
        lbl_SelectedTSSID.Text = itm.SubItems(1).Text
        lbl_SelectedTSSName.Text = itm.SubItems(2).Text
        TextBox3.Text = itm.SubItems(7).Text
        TextBox2.Text = itm.SubItems(8).Text
        TextBox1.Text = itm.SubItems(2).Text
    End Sub

    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        Dim id As String = lbl_SelectedTZBQID.Text
        Dim msg As String = "<TZBQ:TIME," & id & "," & TextBox9.Text & ">"
        sendMsgToDev(id, msg)
    End Sub
    Public Sub sendMsgToDev(ByVal id As String, ByVal msg As String)
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim str As String = "?func=tzbqOrder&datamsg=" & msg & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim r As String = GetNorResult("result", result)
        Dim msgt As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        If r = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        Else
            If msgt = "Please login" Then
                Login()
                sendMsgToDev(id, msg)
            Else
                Dim sb As New StringBuilder
                sb.AppendLine("命令下发失败")
                sb.AppendLine(msgt)
                sb.AppendLine(errmsg)
                MsgBox(sb.ToString)
            End If

        End If
        Console.WriteLine(result)
    End Sub

    Private Sub Button24_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button24.Click
        Dim id As String = lbl_SelectedTZBQID.Text
        Dim msg As String = "<TZBQ:SZID," & id & "," & TextBox8.Text & ">"
        sendMsgToDev(id, msg)
    End Sub

    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        Dim lng As String = TextBox6.Text
        Dim lat As String = TextBox7.Text
        Dim id As String = lbl_SelectedTZBQID.Text
        Dim msg As String = "<TZBQ:SGPS," & id & "," & lng & "," & lat & ">"
        sendMsgToDev(id, msg)
        If Val(lng) < 100 Or Val(lat) < 5 Then MsgBox("请填写正确经纬度") : Return
        Dim HttpMsgUrl As String = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim str As String = String.Format("?func=SetDeviceLngAndLat&lng={0}&lat={1}&token=" & token, lng, lat)
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim r As String = GetNorResult("result", result)
        msg = GetNorResult("msg", result)
        If r = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        Else
            MsgBox(msg)
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim nickid As String = TextBox4.Text
        Dim id As String = lbl_SelectedTZBQID.Text
        If nickid = "" Then
            MsgBox("备注不能为空")
            Return
        End If
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim str As String = "func=SetDeviceNickID&nickid=" & nickid
        Dim result As String = GethWithToken(HttpMsgUrl, str)
        If result = "" Then
            result = "设置备注成功！"
        End If
        MsgBox(result)
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim nickid As String = TextBox1.Text
        Dim id As String = lbl_SelectedTSSID.Text
        If nickid = "" Then
            MsgBox("备注不能为空")
            Return
        End If
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)

        Dim str As String = "?func=SetDeviceNickID&nickid=" & nickid & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        If result = "" Then
            result = "设置备注成功！"
        End If
        MsgBox(result)
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim id As String = lbl_SelectedTSSID.Text
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

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim id As String = lbl_SelectedTSSID.Text
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
        Dim id As String = lbl_SelectedTSSID.Text
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim dataType As String = "task"
        Dim funcType As String = "reboot"
        Dim paraMsg As String = ""
        Dim str As String = "?func=tssOrderByCode&dataType=" & dataType & "&funcType=" & funcType & "&paraMsg=" & paraMsg & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim w As New WarnBox("命令下发成功！")
        w.Show()
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Dim id As String = lbl_SelectedTSSID.Text
        Dim HttpMsgUrl As String
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & id & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Dim lng As String = TextBox3.Text
        Dim lat As String = TextBox2.Text
        If Val(lng) < 100 Or Val(lat) < 5 Then MsgBox("请填写正确经纬度") : Return
        Dim str As String = String.Format("?func=SetDeviceLngAndLat&lng={0}&lat={1}&token=" & token, lng, lat)
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("msg", result)
        If r = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        Else
            MsgBox(msg)
        End If
    End Sub

  
End Class
