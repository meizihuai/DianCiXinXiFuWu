Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class AllDeviceList

    Private Sub AllDeviceList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        Control.CheckForIllegalCrossThreadCalls = False
        ini()
        If isLoadGis Then
            WebGis.Navigate(gisurl)
        Else
            RBAll.Checked = True
        End If
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 45)
        LVDetail.Columns.Add("设备名称", 120)
        LVDetail.Columns.Add("设备类型", 80)
        LVDetail.Columns.Add("地点", 80)
        LVDetail.Columns.Add("是否在线", 80)
        LVDetail.Columns.Add("上线时间", 80)
        LVDetail.Columns.Add("经度", 80)
        LVDetail.Columns.Add("纬度", 80)
        LVDetail.Columns.Add("IP", 80)
        LVDetail.Columns.Add("Port", 80)
    End Sub
    Private Sub RefrushGis()
        Label1.Text = "正在刷新……"
        My.Application.DoEvents()
        LVDetail.Visible = False
        LVDetail.Items.Clear()
        CleanGis(WebGis)
        Dim TZBQTotalNum As Integer = 0
        Dim TSSTotalNum As Integer = 0
        Dim TZBQOnlineNum As Integer = 0
        Dim TSSOnlineNum As Integer = 0
        If IsNothing(alldevlist) Then Label1.Text = "云服务设备列表" : Return
        For Each d In alldevlist
            If d.Kind = "TZBQ" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                    TSSOnlineNum = TSSOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                    TZBQOnlineNum = TZBQOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSS" Then itm.ForeColor = Color.Green
                    If d.Kind = "TZBQ" Then itm.ForeColor = Color.Blue
                Next
                LVDetail.Items.Add(itm)
            End If
        Next
        For Each d In alldevlist
            If d.Kind = "TSS" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                    TSSOnlineNum = TSSOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                    TZBQOnlineNum = TZBQOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSS" Then itm.ForeColor = Color.Green
                    If d.Kind = "TZBQ" Then itm.ForeColor = Color.Blue
                Next
                LVDetail.Items.Add(itm)
            End If
        Next
        For Each d In alldevlist
            If d.Kind = "TSSGateWay" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSSGateWay" Then
                    itm.SubItems(2).Text = "TSS网关"
                    TSSOnlineNum = TSSOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSSGateWay" Then itm.ForeColor = Color.FromArgb(218, 165, 32)
                Next
                LVDetail.Items.Add(itm)
            End If
        Next
        For Each d In alldevlist
            If d.Kind = "ING" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "ING" Then
                    itm.SubItems(2).Text = "监测网关"
                    TSSOnlineNum = TSSOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "ING" Then itm.ForeColor = Color.FromArgb(139, 101, 8)
                Next
                LVDetail.Items.Add(itm)
            End If
        Next
        LVDetail.Visible = True
        RBOnline.Text = "在线:" & TZBQOnlineNum + TSSOnlineNum
        Dim resutl As String = GetServerResult("func=GetAllDBDevlist&token=" & token)
        Dim dt As DataTable = JsonConvert.DeserializeObject(resutl, GetType(DataTable))
        If IsNothing(dt) = False Then
            For Each row As DataRow In dt.Rows
                Dim kind As String = row("Kind")
                If kind = "TZBQ" Then
                    TZBQTotalNum = TZBQTotalNum + 1
                End If
                If kind = "TSS" Then
                    TSSTotalNum = TSSTotalNum + 1
                End If
                If kind = "TSSGateWay" Then
                    TSSTotalNum = TSSTotalNum + 1
                End If
                If kind = "ING" Then
                    TSSTotalNum = TSSTotalNum + 1
                End If
                Dim DeviceID As String = row("DeviceID").ToString
                Dim DeviceName As String = row("DeviceNickName").ToString
                Dim Address As String = row("Address").ToString
                Dim isOnline As Boolean = False
                For Each itm In alldevlist
                    If itm.DeviceID = DeviceID Then
                        isOnline = True
                        Exit For
                    End If
                Next
                If isOnline = False Then
                    Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                    itm.SubItems.Add(DeviceName)
                    itm.SubItems.Add("")
                    itm.SubItems.Add(Address)
                    itm.SubItems.Add("离线")
                    itm.SubItems.Add(row("OnlineTime").ToString)
                    itm.SubItems.Add(row("Lng").ToString)
                    itm.SubItems.Add(row("Lat").ToString)
                    If kind = "TSS" Then
                        itm.SubItems(2).Text = "频谱传感器"
                        If isLoadGis Then AddNewIco(row("Lng").ToString, row("Lat").ToString, DeviceName, TssIco, WebGis)
                    End If
                    If kind = "TZBQ" Then
                        itm.SubItems(2).Text = "微型传感器"
                        If isLoadGis Then AddNewIco(row("Lng").ToString, row("Lat").ToString, DeviceName, TZBQIco, WebGis)
                    End If
                    itm.SubItems.Add(row("IP").ToString)
                    itm.SubItems.Add(row("Port").ToString)
                    itm.UseItemStyleForSubItems = True
                    For i = 0 To itm.SubItems.Count - 1
                        itm.SubItems(i).ForeColor = Color.Gray
                    Next
                    LVDetail.Items.Add(itm)
                End If
            Next
        End If
        RBAll.Text = "全部:" & TSSTotalNum + TZBQTotalNum
        RBDisonline.Text = "离线:" & TSSTotalNum + TZBQTotalNum - TZBQOnlineNum - TSSOnlineNum
        Label1.Text = "云服务设备列表"
        My.Application.DoEvents()
    End Sub
#Region "更新GIS"
    Delegate Sub wt_cleanGis(ByVal web As WebBrowser)
    Private Sub CleanGis(ByVal web As WebBrowser)
        Dim d As New wt_cleanGis(AddressOf th_CleanGis)
        Dim b(0) As Object
        b(0) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub th_CleanGis(ByVal web As WebBrowser)

        Dim doc As HtmlDocument = web.Document
        If IsNothing(doc) Then Exit Sub
        Dim ObjArr(0) As Object
        doc.InvokeScript("cleanall", ObjArr)

    End Sub
    Delegate Sub wt_script(ByVal scriptName As String, ByVal str() As String, ByVal web As WebBrowser)
    Private Sub script(ByVal scriptName As String, ByVal str() As String, ByVal web As WebBrowser)
        Dim d As New wt_script(AddressOf th_script)
        Dim b(2) As Object
        b(0) = scriptName
        b(1) = str
        b(2) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub th_script(ByVal scriptName As String, ByVal str() As String, ByVal web As WebBrowser)
        Try
            Dim doc As HtmlDocument = web.Document
            Dim O(str.Count - 1) As Object
            For i = 0 To str.Length - 1
                O(i) = CObj(str(i))
            Next
            doc.InvokeScript(scriptName, O)
        Catch ex As Exception

        End Try

    End Sub
    Delegate Sub wt_setGisCenter(ByVal lng As String, ByVal lat As String, ByVal web As WebBrowser)
    Private Sub setGisCenter(ByVal lng As String, ByVal lat As String, ByVal web As WebBrowser)
        Dim d As New wt_setGisCenter(AddressOf th_setGisCenter)
        Dim b(2) As Object
        b(0) = lng
        b(1) = lat
        b(2) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub th_setGisCenter(ByVal lng As String, ByVal lat As String, ByVal web As WebBrowser)
        '70410045
        '421127199303212592
        '梅子怀
        Dim doc As HtmlDocument = web.Document
        If IsNothing(doc) Then Exit Sub
        Dim ObjArr(2) As Object
        ObjArr(0) = CObj(lng)
        ObjArr(1) = CObj(lat)
        doc.InvokeScript("setcenter", ObjArr)
    End Sub

    Delegate Sub wt_setGisCenter3(ByVal lng As String, ByVal lat As String, ByVal size As Integer, ByVal web As WebBrowser)
    Private Sub setGisCenter3(ByVal lng As String, ByVal lat As String, ByVal size As Integer, ByVal web As WebBrowser)
        Dim d As New wt_setGisCenter3(AddressOf th_setGisCenter3)
        Dim b(3) As Object
        b(0) = lng
        b(1) = lat
        b(2) = size
        b(3) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub th_setGisCenter3(ByVal lng As String, ByVal lat As String, ByVal size As Integer, ByVal web As WebBrowser)
        '70410045
        '421127199303212592
        '梅子怀
        Dim doc As HtmlDocument = web.Document
        If IsNothing(doc) Then Exit Sub
        Dim ObjArr(2) As Object
        ObjArr(0) = CObj(lng)
        ObjArr(1) = CObj(lat)
        ObjArr(2) = CObj(size)
        doc.InvokeScript("setcenter3", ObjArr)
    End Sub

    Private Sub AddJumpPoint(ByVal lng As String, ByVal lat As String, ByVal label As String, ByVal web As WebBrowser)
        Dim d As New wt_AddPoint(AddressOf th_AddJumpPoint)
        Dim b(3) As Object
        b(0) = lng
        b(1) = lat
        b(2) = label
        b(3) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub th_AddJumpPoint(ByVal lng As String, ByVal lat As String, ByVal label As String, ByVal web As WebBrowser)
        Dim doc As HtmlDocument = web.Document
        Dim ObjArr(2) As Object
        ObjArr(0) = CObj(lng)
        ObjArr(1) = CObj(lat)
        ObjArr(2) = CObj(label)
        doc.InvokeScript("addpoint", ObjArr)
    End Sub
    Delegate Sub wt_AddPoint(ByVal lng As String, ByVal lat As String, ByVal label As String, ByVal web As WebBrowser)
    Private Sub AddPoint(ByVal lng As String, ByVal lat As String, ByVal label As String, ByVal web As WebBrowser)
        Dim d As New wt_AddPoint(AddressOf th_AddPoint)
        Dim b(3) As Object
        b(0) = lng
        b(1) = lat
        b(2) = label
        b(3) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub AddNewIco(ByVal lng As String, ByVal lat As String, ByVal label As String, ByVal icoUrl As String, ByVal web As WebBrowser)
        script("addNewIcoPoint", New String() {lng, lat, label, icoUrl}, web)
    End Sub
    Private Sub th_AddPoint(ByVal lng As String, ByVal lat As String, ByVal label As String, ByVal web As WebBrowser)
        Dim doc As HtmlDocument = web.Document
        If IsNothing(doc) Then Exit Sub
        Dim ObjArr(2) As Object
        ObjArr(0) = CObj(lng)
        ObjArr(1) = CObj(lat)
        ObjArr(2) = CObj(label)
        doc.InvokeScript("addBz", ObjArr)
    End Sub
#End Region

    Private Sub WebGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        setGisCenter3(113.75, 23.04, 11, WebGis)
        RBAll.Checked = True
    End Sub

    Private Sub RBOnline_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RBOnline.CheckedChanged
        LVDetail.Visible = False
        LVDetail.Items.Clear()
        CleanGis(WebGis)
        For Each d In alldevlist
            If d.Kind = "TZBQ" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSS" Then itm.ForeColor = Color.Green
                    If d.Kind = "TZBQ" Then itm.ForeColor = Color.Blue
                Next
                LVDetail.Items.Add(itm)
            End If          
        Next
        For Each d In alldevlist
            If d.Kind = "TSS" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSS" Then itm.ForeColor = Color.Green
                    If d.Kind = "TZBQ" Then itm.ForeColor = Color.Blue
                Next
                LVDetail.Items.Add(itm)
            End If
        Next
        LVDetail.Visible = True
    End Sub

    Private Sub RBDisonline_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RBDisonline.CheckedChanged
        LVDetail.Visible = False
        LVDetail.Items.Clear()
        CleanGis(WebGis)
        Dim resutl As String = GetServerResult("func=GetAllDBDevlist&token=" & token)
        Dim dt As DataTable = JsonConvert.DeserializeObject(resutl, GetType(DataTable))
        If IsNothing(dt) = False Then
            For Each row As DataRow In dt.Rows
                Dim kind As String = row("Kind").ToString
                Dim DeviceID As String = row("DeviceID").ToString
                Dim DeviceName As String = row("DeviceNickName").ToString
                Dim Address As String = row("Address").ToString
                Dim isOnline As Boolean = False
                For Each itm In alldevlist
                    If itm.DeviceID = DeviceID Then
                        isOnline = True
                        Exit For
                    End If
                Next
                If isOnline = False Then
                    Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                    itm.SubItems.Add(DeviceName)
                    itm.SubItems.Add("")
                    itm.SubItems.Add(Address)
                    itm.SubItems.Add("离线")
                    itm.SubItems.Add(row("OnlineTime").ToString)
                    itm.SubItems.Add(row("Lng").ToString)
                    itm.SubItems.Add(row("Lat").ToString)
                    If kind = "TSS" Then
                        itm.SubItems(2).Text = "频谱传感器"
                        If isLoadGis Then AddNewIco(row("Lng").ToString(), row("Lat").ToString(), DeviceName, TssIco, WebGis)
                    End If
                    If kind = "TZBQ" Then
                        itm.SubItems(2).Text = "微型传感器"
                        If isLoadGis Then AddNewIco(row("Lng").ToString(), row("Lat").ToString(), DeviceName, TZBQIco, WebGis)
                    End If
                    itm.SubItems.Add(row("IP").ToString)
                    itm.SubItems.Add(row("Port").ToString)
                    itm.UseItemStyleForSubItems = True
                    For i = 0 To itm.SubItems.Count - 1
                        itm.SubItems(i).ForeColor = Color.Gray
                    Next
                    LVDetail.Items.Add(itm)
                End If
            Next
            LVDetail.Visible = True
        End If
       
    End Sub

    Private Sub RBAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RBAll.CheckedChanged
        RefrushGis()
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        RefrushGis()
    End Sub

    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim lng As String = itm.SubItems(6).Text
        Dim lat As String = itm.SubItems(7).Text
        Dim selectDeviceID As String = itm.SubItems(1).Text
        Dim selectDeviceKind As String = itm.SubItems(2).Text
        Dim isOnline As String = itm.SubItems(4).Text
        If isOnline <> "在线" Then Return
        Dim ds As deviceStu
        For Each d In alldevlist
            If d.Name = selectDeviceID Then
                ds = d
                Exit For
            End If
        Next
        If ds.DeviceID = "" Then
            MsgBox("找不到该设备详细信息，请刷新设备列表！")
            Exit Sub
        End If
        Dim str As String = "设备名称: " & ds.Name & "<br>"
        str = str & "设备地址: " & ds.Address & "<br>"
        If ds.Kind = "TZBQ" Then
            str = str & "设备类型: " & "微型传感器" & "<br>"
        End If
        If ds.Kind = "TSS" Then
            str = str & "设备类型: " & "频谱传感器" & "<br>"
        End If
        str = str & "上线时间: " & ds.OnlineTime & "<br>"
        str = str & "设备经度: " & ds.Lng & "<br>"
        str = str & "设备纬度: " & ds.Lat & "<br>"
        str = str & "设备状态: " & ds.Statu & "<br>"
        If ds.Statu = "working" Then
            str = str & "任务状态: " & "正在执行任务" & "<br>"
            LVDetail.Items(8).SubItems(1).Text = "正在执行任务……"
        End If
        If ds.Statu = "free" Then
            str = str & "任务状态: " & "没有任务，空闲状态" & "<br>"
        End If
        str = "<div style='font-family: Microsoft YaHei;padding-top:5px;font-size:13'>" & str & "</div>"
        'str = "<div style='font-family: 宋体'>" & str & "</div>"
        script("showWindowMsg", New String() {lng, lat, str, True}, WebGis)
        script("setcenter", New String() {lng, lat}, WebGis)
        Form1.SelectDevice(selectDeviceID)

        If lng <> "" And lat <> "" Then
            setGisCenter3(lng, lat, 15, WebGis)
            'Form1.selectDeviceID = selectDeviceID
            'Form1.Label2.Text = "选中  " & "微型传感器" & "  " & selectDeviceID
            'If selectDeviceKind = "微型传感器" Then
            '    Form1.p_beClick("微型传感器," & selectDeviceID)
            'End If
            'If selectDeviceKind = "频谱传感器" Then
            '    Form1.p_beClick("频谱传感器," & selectDeviceID)
            'End If
        End If

    End Sub

    Private Sub 选择此传感器ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 选择此传感器ToolStripMenuItem.Click
        '详细信息
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim DeviceName As String = itm.SubItems(1).Text
        Dim Kind As String = itm.SubItems(2).Text
        Dim isOnline As String = itm.SubItems(4).Text
        If isOnline <> "在线" Then Return
        Dim lng As String = itm.SubItems(6).Text
        Dim lat As String = itm.SubItems(7).Text
        Dim ds As deviceStu
        For Each d In alldevlist
            If d.Name = DeviceName Then
                ds = d
                Exit For
            End If
        Next
        If ds.DeviceID = "" Then
            MsgBox("找不到该设备详细信息，请刷新设备列表！")
            Exit Sub
        End If
        Dim str As String = "设备名称: " & ds.Name & "<br>"
        str = str & "设备地址: " & ds.Address & "<br>"
        If ds.Kind = "TZBQ" Then
            str = str & "设备类型: " & "微型传感器" & "<br>"
        End If
        If ds.Kind = "TSS" Then
            str = str & "设备类型: " & "频谱传感器" & "<br>"
        End If
        str = str & "上线时间: " & ds.OnlineTime & "<br>"
        str = str & "设备经度: " & ds.Lng & "<br>"
        str = str & "设备纬度: " & ds.Lat & "<br>"
        str = str & "设备状态: " & ds.Statu & "<br>"
        If ds.Statu = "working" Then
            str = str & "任务状态: " & "正在执行任务" & "<br>"
            LVDetail.Items(8).SubItems(1).Text = "正在执行任务……"
        End If
        If ds.Statu = "free" Then
            str = str & "任务状态: " & "没有任务，空闲状态" & "<br>"
        End If
        script("showWindowMsg", New String() {lng, lat, str, True}, WebGis)
        script("setcenter", New String() {lng, lat}, WebGis)
    End Sub

    Private Sub 选择该传感器ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 选择该传感器ToolStripMenuItem.Click
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim DeviceName As String = itm.SubItems(1).Text
        Dim Kind As String = itm.SubItems(2).Text
        Dim isOnline As String = itm.SubItems(4).Text
        If isOnline <> "在线" Then Return
        Form1.SelectDevice(DeviceName)
        'Form1.isSelected = True
        'Form1.selectDeviceID = DeviceName
        'If Kind = "微型传感器" Then Form1.selectDeviceKind = "TZBQ"
        'If Kind = "频谱传感器" Then Form1.selectDeviceKind = "TSS"
        'Form1.Label2.Text = "选中  " & "微型传感器" & "  " & DeviceName
    End Sub

    Private Sub Panel3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel3.Paint

    End Sub

    Private Sub PictureBox3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        RBAll.Checked = False
        RBAll.Checked = True
    End Sub
End Class
