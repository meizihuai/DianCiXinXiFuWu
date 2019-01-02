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
Imports System.Math
Public Class OtherSystemDevice
    Dim otherDeviceDt As DataTable

    Private Sub OtherSystemDevice_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        WebGis.ObjectForScripting = Me
        ini()

        If isLoadGis Then
            WebGis.Navigate(gisurl)
        Else
            GetOtherDeviceDT()
        End If
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 50)
        LVDetail.Columns.Add("设备名称", 80)
        LVDetail.Columns.Add("设备地址", 100)
        LVDetail.Columns.Add("状态", 100)
        LVDetail.Columns.Add("设备类型", 100)
        LVDetail.Columns.Add("厂家", 100)
        LVDetail.Columns.Add("经度", 100)
        LVDetail.Columns.Add("纬度", 100)
        LVDetail.Columns.Add("网络切换", 100)
        LVDetail.Columns.Add("是否挂网关", 100)
    End Sub
  
    Private Sub WebGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        '  setGisCenter3(104.345, 30.678, 5, WebGis)
        setGisCenter3(113.75, 23.04, 11, WebGis)
        GetOtherDeviceDT()
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

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetOtherDeviceDT()
    End Sub
    Private Sub GetOtherDeviceDT()
        RB_All.Checked = True
        Label10.Text = 0
        Label11.Text = 0
        Label12.Text = 0
        Label13.Text = 0
        Label5.Text = 0
        Label15.Text = 0
        Label2.Visible = True
        If isLoadGis Then CleanGis(WebGis)
        Dim result As String = GetServerResult("func=GetOtherDeviceList")
        Label2.Visible = False
        If result = "[]" Then Return
        Try
            otherDeviceDt = New DataTable
            otherDeviceDt = JsonConvert.DeserializeObject(result, GetType(DataTable))
            If IsNothing(otherDeviceDt) Then Return
            LVDetail.Items.Clear()
            LVDetail.Visible = False
            For Each row As DataRow In otherDeviceDt.Rows
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(row("DeviceName"))
                itm.SubItems.Add(row("Address"))
                itm.SubItems.Add(row("Status"))
                Dim kind As String = row("DeviceKind")
                Dim Vender As String = row("Vender")
                If kind = "Allinone" Then '一体机
                    itm.SubItems.Add("一体机")
                    Label10.Text = Val(Label10.Text) + 1
                    AddNewIco(row("lng"), row("Lat"), row("DeviceName"), AllinoneIco, WebGis)
                End If
                If kind = "Direction" Then '测向
                    itm.SubItems.Add("测向站")
                    Label11.Text = Val(Label11.Text) + 1
                    AddNewIco(row("lng"), row("Lat"), row("DeviceName"), DirectionIco, WebGis)
                End If
                If kind = "SH57" Then
                    itm.SubItems.Add("上海57所传感器")
                    Label5.Text = Val(Label5.Text) + 1
                    Dim status As String = row("status")
                    If Vender = "713" Then
                        AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH713Ico, WebGis)
                    Else
                        If status = "正常" Then
                            If IsNothing(row("HaveGateWay")) = False Then
                                If row("HaveGateWay").ToString = "1" Then
                                    AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57_713Ico, WebGis)
                                Else
                                    AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57Ico, WebGis)
                                End If
                            Else
                                AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57Ico, WebGis)
                            End If
                        Else
                            AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57_DisOnlineIco, WebGis)
                        End If
                    End If
                End If
                itm.SubItems.Add(row("Vender"))
                itm.SubItems.Add(row("Lng"))
                itm.SubItems.Add(row("Lat"))
                itm.SubItems.Add(row("NetSwitch").ToString)
                itm.SubItems.Add(row("HaveGateWay"))
                LVDetail.Items.Add(itm)
            Next
            For Each dev In alldevlist
                Dim dId As String = dev.DeviceID
                Dim isSH57 As Boolean = False
                If InStr(dId, "-") Then
                    Dim st() As String = dId.Split("-")
                    If st(0) = "57S" Then
                        isSH57 = True
                    End If
                End If
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(dev.Name)
                itm.SubItems.Add(dev.Address)
                If dev.Statu = "free" Then
                    itm.SubItems.Add("正常")
                Else
                    itm.SubItems.Add("正常")
                End If
                If isSH57 Then
                    itm.SubItems.Add("上海57所传感器")
                    ' Label5.Text = Val(Label5.Text) + 1
                    AddNewIco(dev.Lng, dev.Lat, dev.Name, SH57_713Ico, WebGis)
                Else
                    If dev.Kind = "TSS" Then
                        itm.SubItems.Add("频谱传感器")
                        AddNewIco(dev.Lng, dev.Lat, dev.Name, TssIco, WebGis)
                    End If
                    If dev.Kind = "TZBQ" Then
                        itm.SubItems.Add("微型传感器")
                        AddNewIco(dev.Lng, dev.Lat, dev.Name, TZBQIco, WebGis)
                    End If
                    If dev.Kind = "TSSGateWay" Then
                        itm.SubItems.Add("TSS网关")
                        AddNewIco(dev.Lng, dev.Lat, dev.Name, TZBQIco, WebGis)
                    End If
                End If
               
                itm.SubItems.Add("大盛公")
                itm.SubItems.Add(dev.Lng)
                itm.SubItems.Add(dev.Lat)
                itm.SubItems.Add("")
                If isSH57 Then
                    itm.SubItems.Add("1")
                Else
                    itm.SubItems.Add("")
                End If

                LVDetail.Items.Add(itm)
            Next
            Label13.Text = alldevlist.Count
            Label15.Text = Val(Label10.Text) + Val(Label11.Text) + Val(Label12.Text) + Val(Label13.Text) + Val(Label5.Text)
        Catch ex As Exception

        End Try
        LVDetail.Visible = True
        RB_All.Checked = True
        TongJiNum()
    End Sub
    Public Sub SetGisCenterByLngLat(ByVal lngList As List(Of Double), ByVal latList As List(Of Double), ByVal web As WebBrowser)
        If IsNothing(lngList) Then Return
        If IsNothing(latList) Then Return
        If lngList.Count = 0 Then Return
        If latList.Count = 0 Then Return
        If lngList.Count <> latList.Count Then Return
        If IsNothing(web) Then Return
        Dim zoom As Integer = GetZoom(lngList, latList)
        Dim lng As Double = GetAvg(lngList.ToArray)
        Dim lat As Double = GetAvg(latList.ToArray)
        Try
            setGisCenter3(lng, lat, zoom, web)
        Catch ex As Exception

        End Try
    End Sub
    Public Function GetZoom(ByVal lngList As List(Of Double), ByVal latList As List(Of Double)) As Integer
        Dim d As Double = GetMaxDis(lngList, latList)
        If d <= 5 Then Return 15
        If d >= 960 Then Return 3
        If d <= 60 Then Return 13
        If d <= 120 Then Return 13
        If d <= 180 Then Return 12
        If d <= 240 Then Return 11
        If d <= 300 Then Return 11
        If d <= 360 Then Return 11
        If d <= 420 Then Return 11
        If d <= 480 Then Return 11
        If d <= 540 Then Return 10
        If d <= 600 Then Return 9
        If d <= 660 Then Return 8
        If d <= 720 Then Return 7
        If d <= 780 Then Return 6
        If d <= 840 Then Return 5
        If d <= 900 Then Return 4
    End Function
    Public Function GetMaxDis(ByVal lngList As List(Of Double), ByVal latList As List(Of Double)) As Double
        Dim max As Double = 0
        For i = 0 To lngList.Count - 1
            Dim lng1 As Double = lngList(i)
            Dim lat1 As Double = latList(i)
            For j = i + 1 To lngList.Count - 1
                Dim lng2 As Double = lngList(j)
                Dim lat2 As Double = latList(j)
                If i <> j Then
                    Dim dis As Double = GetDistance(lat1, lng1, lat2, lng2)
                    If dis > max Then
                        max = dis
                    End If
                End If
            Next
        Next
        Return max
    End Function
    Private Function rad(ByVal d As Double) As Double
        rad = d * 3.1415926535898 / 180
    End Function
    Private Function GetDistance(ByVal lat1 As Double, ByVal lng1 As Double, ByVal lat2 As Double, ByVal lng2 As Double) As Double
        Dim radlat1 As Double, radlat2 As Double
        Dim a As Double, b As Double, s As Double, Temp As Double
        radlat1 = rad(lat1)
        radlat2 = rad(lat2)
        a = radlat1 - radlat2
        b = rad(lng1) - rad(lng2)
        Temp = Sqrt(Sin(a / 2) ^ 2 + Cos(radlat1) * Cos(radlat2) * Sin(b / 2) ^ 2)
        s = 2 * Atan(Temp / Sqrt(-Temp * Temp + 1))
        s = s * 6378.137
        GetDistance = s

    End Function
    Public Function SwitchOut(ByVal DeviceName As String) As String
        '切换到云平台
        Dim isDo As Boolean = False
        For Each itm As ListViewItem In LVDetail.Items
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(2).Text
            Dim kind As String = itm.SubItems(4).Text
            If dName = DeviceName Then
                If kind = "上海57所传感器" Then
                    isDo = True
                Else
                    MsgBox("该设备不支持切换网络")
                    isDo = False
                End If
                Exit For
            End If
        Next
        If isDo = False Then
            Return ""
        End If
        Dim httpMsgUrl As String = GetServerResult("func=GetHttpMsgUrlByDeviceName&DeviceName=" & DeviceName)
        If httpMsgUrl <> "" Then
            Dim result As String = GethWithToken(httpMsgUrl, "func=netswitchin")

            If GetNorResult("result", result) = "success" Then
                Dim w As New WarnBox("命令下发成功！")
                w.Show()
            End If
        End If
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(2).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            If kind = "上海57所传感器" Then
                If dName = DeviceName Then
                    AddNewIco(lng, lat, dName, SH57_713Ico, WebGis)
                Else
                    AddNewIco(lng, lat, dName, SH57Ico, WebGis)
                End If
            End If
            If kind = "一体机" Then '一体机
                AddNewIco(lng, lat, dName, AllinoneIco, WebGis)
            End If
            If kind = "测向站" Then '一体机
                AddNewIco(lng, lat, dName, DirectionIco, WebGis)
            End If
            If kind = "测向站" Then '一体机
                AddNewIco(lng, lat, dName, DirectionIco, WebGis)
            End If
            If kind = "频谱传感器" Then '一体机
                AddNewIco(lng, lat, dName, TssIco, WebGis)
            End If
            If kind = "微型传感器" Then '测向
                AddNewIco(lng, lat, dName, TZBQIco, WebGis)
            End If
        Next
    End Function
    Public Function SwitchIn(ByVal DeviceName As String) As String
        '切换到监测
        Dim isDo As Boolean = False
        For Each itm As ListViewItem In LVDetail.Items
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(2).Text
            Dim kind As String = itm.SubItems(4).Text
            If dName = DeviceName Then
                If kind = "上海57所传感器" Then
                    isDo = True
                Else
                    MsgBox("该设备不支持切换网络")
                    isDo = False
                End If
                Exit For
            End If
        Next
        If isDo = False Then
            Return ""
        End If
        Dim httpMsgUrl As String = GetServerResult("func=GetHttpMsgUrlByDeviceName&DeviceName=" & DeviceName)
        If httpMsgUrl <> "" Then
            Dim result As String = GethWithToken(httpMsgUrl, "func=netswitchout")

            If GetNorResult("result", result) = "success" Then
                Dim w As New WarnBox("命令下发成功！")
                w.Show()
            End If
        End If
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(2).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            If kind = "上海57所传感器" Then
                AddNewIco(lng, lat, dName, SH57Ico, WebGis)
            End If
            If kind = "一体机" Then '一体机
                AddNewIco(lng, lat, dName, AllinoneIco, WebGis)
            End If
            If kind = "测向站" Then '一体机
                AddNewIco(lng, lat, dName, DirectionIco, WebGis)
            End If
            If kind = "测向站" Then '一体机
                AddNewIco(lng, lat, dName, DirectionIco, WebGis)
            End If
            If kind = "频谱传感器" Then '一体机
                AddNewIco(lng, lat, dName, TssIco, WebGis)
            End If
            If kind = "微型传感器" Then '测向
                AddNewIco(lng, lat, dName, TZBQIco, WebGis)
            End If
        Next
    End Function
    Private Sub TongJiNum()
        If LVDetail.Items.Count = 0 Then Return
        Dim NumNormal As Integer = 0
        Dim NumErr As Integer = 0
        Dim NumRepair As Integer = 0
        Dim NumRemove As Integer = 0
        For Each itm As ListViewItem In LVDetail.Items
            Dim status As String = itm.SubItems(3).Text
            If status = "正常" Then NumNormal = NumNormal + 1
            If status = "故障" Then NumErr = NumErr + 1
            If status = "送修" Then NumRepair = NumRepair + 1
            If status = "拆除" Then NumRemove = NumRemove + 1
        Next
        RB_All.Text = "全部 " & LVDetail.Items.Count
        RB_Normal.Text = "正常 " & NumNormal
        RB_Err.Text = "故障 " & NumErr
        RB_Repair.Text = "送修 " & NumRepair
        RB_Remove.Text = "拆除 " & NumRemove
    End Sub

    Private Sub ShowDeviceKind(ByVal k As String)

        If IsNothing(otherDeviceDt) Then Return
        If isLoadGis Then CleanGis(WebGis)
        LVDetail.Items.Clear()
        LVDetail.Visible = False
        Dim lngList As New List(Of Double)
        Dim latList As New List(Of Double)
        For Each row As DataRow In otherDeviceDt.Rows
            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            itm.SubItems.Add(row("DeviceName"))
            itm.SubItems.Add(row("Address"))
            itm.SubItems.Add(row("Status"))
            Dim kind As String = row("DeviceKind")
            Dim Vender As String = row("Vender")
            If kind <> k Then Continue For
            If kind = "Allinone" Then '一体机
                itm.SubItems.Add("一体机")
                AddNewIco(row("lng"), row("Lat"), row("DeviceName"), AllinoneIco, WebGis)
            End If
            If kind = "Direction" Then '测向
                itm.SubItems.Add("测向站")
                AddNewIco(row("lng"), row("Lat"), row("DeviceName"), DirectionIco, WebGis)
            End If
            If kind = "SH57" Then
                itm.SubItems.Add("上海57所传感器")
                Dim status As String = row("status")
                If Vender = "713" Then
                    AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH713Ico, WebGis)
                Else
                    If status = "正常" Then
                        If IsNothing(row("HaveGateWay")) = False Then
                            If row("HaveGateWay").ToString = "1" Then
                                AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57_713Ico, WebGis)
                            Else
                                AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57Ico, WebGis)
                            End If
                        Else
                            AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57Ico, WebGis)
                        End If
                    Else
                        AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57_DisOnlineIco, WebGis)
                    End If
                End If
            End If
            itm.SubItems.Add(row("Vender"))
            itm.SubItems.Add(row("Lng"))
            itm.SubItems.Add(row("Lat"))
            itm.SubItems.Add(row("NetSwitch").ToString)
            itm.SubItems.Add(row("HaveGateWay"))
            lngList.Add(row("Lng"))
            latList.Add(row("Lat"))
            LVDetail.Items.Add(itm)
        Next
        If k = "SH57" Then

            SetGisCenterByLngLat(lngList, latList, WebGis)
            lngList = Nothing
            latList = Nothing
        End If
        If k = "CloudDevice" Then
            'For Each dev In alldevlist
            '    Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            '    itm.SubItems.Add(dev.Name)
            '    itm.SubItems.Add(dev.Address)
            '    If dev.Statu = "free" Then
            '        itm.SubItems.Add("正常")
            '    Else
            '        itm.SubItems.Add("正常")
            '    End If
            '    If dev.Kind = "TSS" Then
            '        itm.SubItems.Add("频谱传感器")
            '        AddNewIco(dev.Lng, dev.Lat, dev.Name, TssIco, WebGis)
            '    End If
            '    If dev.Kind = "TZBQ" Then
            '        itm.SubItems.Add("微型传感器")
            '        AddNewIco(dev.Lng, dev.Lat, dev.Name, TZBQIco, WebGis)
            '    End If
            '    itm.SubItems.Add("大盛公")
            '    itm.SubItems.Add(dev.Lng)
            '    itm.SubItems.Add(dev.Lat)
            '    itm.SubItems.Add("")
            '    itm.SubItems.Add("")
            '    LVDetail.Items.Add(itm)
            'Next
            For Each dev In alldevlist
                Dim dId As String = dev.DeviceID
                Dim isSH57 As Boolean = False
                If InStr(dId, "-") Then
                    Dim st() As String = dId.Split("-")
                    If st(0) = "57S" Then
                        isSH57 = True
                    End If
                End If
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(dev.Name)
                itm.SubItems.Add(dev.Address)
                If dev.Statu = "free" Then
                    itm.SubItems.Add("正常")
                Else
                    itm.SubItems.Add("正常")
                End If
                If isSH57 Then
                    itm.SubItems.Add("上海57所传感器")
                    'Label5.Text = Val(Label5.Text) + 1
                    AddNewIco(dev.Lng, dev.Lat, dev.Name, SH57_713Ico, WebGis)
                Else
                    If dev.Kind = "TSS" Then
                        itm.SubItems.Add("频谱传感器")
                        AddNewIco(dev.Lng, dev.Lat, dev.Name, TssIco, WebGis)
                    End If
                    If dev.Kind = "TZBQ" Then
                        itm.SubItems.Add("微型传感器")
                        AddNewIco(dev.Lng, dev.Lat, dev.Name, TZBQIco, WebGis)
                    End If
                End If

                itm.SubItems.Add("大盛公")
                itm.SubItems.Add(dev.Lng)
                itm.SubItems.Add(dev.Lat)
                itm.SubItems.Add("")
                If isSH57 Then
                    itm.SubItems.Add("1")
                Else
                    itm.SubItems.Add("")
                End If

                LVDetail.Items.Add(itm)
            Next
        End If
        If k = "All" Then
            GetOtherDeviceDT()
            'setGisCenter3(113.75, 23.04, 11, WebGis)
            'For Each row As DataRow In otherDeviceDt.Rows
            '    Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            '    itm.SubItems.Add(row("DeviceName"))
            '    itm.SubItems.Add(row("Address"))
            '    itm.SubItems.Add(row("Status"))

            '    Dim kind As String = row("DeviceKind")
            '    If kind = "Allinone" Then '一体机
            '        itm.SubItems.Add("一体机")
            '        AddNewIco(row("lng"), row("Lat"), row("DeviceName"), AllinoneIco, WebGis)
            '    End If
            '    If kind = "Direction" Then '测向
            '        itm.SubItems.Add("测向站")
            '        AddNewIco(row("lng"), row("Lat"), row("DeviceName"), DirectionIco, WebGis)
            '    End If
            '    If kind = "SH57" Then
            '        itm.SubItems.Add("上海57所传感器")
            '        Dim status As String = row("status")

            '        If status = "正常" Then
            '            If IsNothing(row("HaveGateWay")) = False Then
            '                If row("HaveGateWay").ToString = "1" Then
            '                    AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57_713Ico, WebGis)
            '                Else
            '                    AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57Ico, WebGis)
            '                End If
            '            Else
            '                AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57Ico, WebGis)
            '            End If
            '        Else
            '            AddNewIco(row("lng"), row("Lat"), row("DeviceName"), SH57_DisOnlineIco, WebGis)
            '        End If
            '    End If
            '    itm.SubItems.Add(row("Vender"))
            '    itm.SubItems.Add(row("Lng"))
            '    itm.SubItems.Add(row("Lat"))
            '    itm.SubItems.Add(row("NetSwitch").ToString)
            '    itm.SubItems.Add(row("HaveGateWay"))
            '    LVDetail.Items.Add(itm)
            'Next
            'For Each dev In alldevlist
            '    Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            '    itm.SubItems.Add(dev.Name)
            '    itm.SubItems.Add(dev.Address)
            '    If dev.Statu = "free" Then
            '        itm.SubItems.Add("正常")
            '    Else
            '        itm.SubItems.Add("正常")
            '    End If
            '    If dev.Kind = "TSS" Then
            '        itm.SubItems.Add("频谱传感器")
            '        AddNewIco(dev.Lng, dev.Lat, dev.Name, TssIco, WebGis)
            '    End If
            '    If dev.Kind = "TZBQ" Then
            '        itm.SubItems.Add("微型传感器")
            '        AddNewIco(dev.Lng, dev.Lat, dev.Name, TZBQIco, WebGis)
            '    End If
            '    itm.SubItems.Add("大盛公")
            '    itm.SubItems.Add(dev.Lng)
            '    itm.SubItems.Add(dev.Lat)
            '    itm.SubItems.Add("")
            '    itm.SubItems.Add("")
            '    LVDetail.Items.Add(itm)
            'Next
        End If
        If LVDetail.Items.Count > 0 Then
            Dim itm As ListViewItem = LVDetail.Items(0)
            Dim DeviceName As String = itm.SubItems(1).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            If k <> "SH57" Then setGisCenter(lng, lat, WebGis)
        End If
        TongJiNum()
        LVDetail.Visible = True
        RB_All.Checked = True
    End Sub
    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click
        ShowDeviceKind("Allinone")
        Label4.Text = Label3.Text & " 设备分布"
    End Sub

    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ShowDeviceKind("Allinone")
        'Label4.Text = Label5.Text & " 设备分布"
    End Sub
    Private Sub Label6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label6.Click
        ShowDeviceKind("Direction")
        Label4.Text = Label6.Text & " 设备分布"
    End Sub

    Private Sub Label7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label7.Click
        ShowDeviceKind("")
        Label4.Text = Label7.Text & " 设备分布"
    End Sub

    Private Sub Label8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label8.Click
        ShowDeviceKind("CloudDevice")
        Label4.Text = Label8.Text & " 设备分布"
    End Sub

    Private Sub Label14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label14.Click
        ShowDeviceKind("All")
        Label4.Text = "全网" & " 设备分布"
    End Sub

    Private Sub PictureBox6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox6.Click
        ShowDeviceKind("CloudDevice")
    End Sub


    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click
        ShowDeviceKind("Direction")
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ShowDeviceKind("Allinone")
    End Sub

    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim DeviceName As String = itm.SubItems(1).Text
        Dim lng As String = itm.SubItems(6).Text
        Dim lat As String = itm.SubItems(7).Text
        setGisCenter(lng, lat, WebGis)
    End Sub


    Private Sub Label9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label9.Click
        ShowDeviceKind("SH57")
    End Sub

    Private Sub RB_Normal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RB_Normal.CheckedChanged
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            itm.UseItemStyleForSubItems = True
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(3).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            If status = "正常" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Green
                Next
                If kind = "上海57所传感器" Then
                    AddNewIco(lng, lat, dName, SH57Ico, WebGis)
                End If
                If kind = "一体机" Then '一体机
                    AddNewIco(lng, lat, dName, AllinoneIco, WebGis)
                End If
                If kind = "测向站" Then '一体机
                    AddNewIco(lng, lat, dName, DirectionIco, WebGis)
                End If
                If kind = "测向站" Then '一体机
                    AddNewIco(lng, lat, dName, DirectionIco, WebGis)
                End If
                If kind = "频谱传感器" Then '一体机
                    AddNewIco(lng, lat, dName, TssIco, WebGis)
                End If
                If kind = "微型传感器" Then '测向
                    AddNewIco(lng, lat, dName, TZBQIco, WebGis)
                End If
            Else
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.White
                Next
            End If
        Next
    End Sub

    Private Sub RB_Err_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RB_Err.CheckedChanged
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            itm.UseItemStyleForSubItems = True
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(3).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            If status = "故障" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Green
                Next
                If kind = "上海57所传感器" Then
                    AddNewIco(lng, lat, dName, SH57Ico, WebGis)
                End If
                If kind = "一体机" Then '一体机
                    AddNewIco(lng, lat, dName, AllinoneIco, WebGis)
                End If
                If kind = "测向站" Then '一体机
                    AddNewIco(lng, lat, dName, DirectionIco, WebGis)
                End If
                If kind = "测向站" Then '一体机
                    AddNewIco(lng, lat, dName, DirectionIco, WebGis)
                End If
                If kind = "频谱传感器" Then '一体机
                    AddNewIco(lng, lat, dName, TssIco, WebGis)
                End If
                If kind = "微型传感器" Then '测向
                    AddNewIco(lng, lat, dName, TZBQIco, WebGis)
                End If
            Else
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.White
                Next
            End If
        Next
    End Sub

    Private Sub RB_Repair_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RB_Repair.CheckedChanged
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            itm.UseItemStyleForSubItems = True
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(3).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            If status = "送修" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Green
                Next
                If kind = "上海57所传感器" Then
                    AddNewIco(lng, lat, dName, SH57Ico, WebGis)
                End If
                If kind = "一体机" Then '一体机
                    AddNewIco(lng, lat, dName, AllinoneIco, WebGis)
                End If
                If kind = "测向站" Then '一体机
                    AddNewIco(lng, lat, dName, DirectionIco, WebGis)
                End If
                If kind = "测向站" Then '一体机
                    AddNewIco(lng, lat, dName, DirectionIco, WebGis)
                End If
                If kind = "频谱传感器" Then '一体机
                    AddNewIco(lng, lat, dName, TssIco, WebGis)
                End If
                If kind = "微型传感器" Then '测向
                    AddNewIco(lng, lat, dName, TZBQIco, WebGis)
                End If
            Else
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.White
                Next
            End If
        Next
    End Sub

    Private Sub RB_Remove_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RB_Remove.CheckedChanged
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            itm.UseItemStyleForSubItems = True
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(3).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            If status = "拆除" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Green
                Next
                If kind = "上海57所传感器" Then
                    AddNewIco(lng, lat, dName, SH57Ico, WebGis)
                End If
                If kind = "一体机" Then '一体机
                    AddNewIco(lng, lat, dName, AllinoneIco, WebGis)
                End If
                If kind = "测向站" Then '一体机
                    AddNewIco(lng, lat, dName, DirectionIco, WebGis)
                End If
                If kind = "测向站" Then '一体机
                    AddNewIco(lng, lat, dName, DirectionIco, WebGis)
                End If
                If kind = "频谱传感器" Then '一体机
                    AddNewIco(lng, lat, dName, TssIco, WebGis)
                End If
                If kind = "微型传感器" Then '测向
                    AddNewIco(lng, lat, dName, TZBQIco, WebGis)
                End If
            Else
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.White
                Next
            End If
        Next
    End Sub

    Private Sub RB_All_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RB_All.CheckedChanged
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            itm.UseItemStyleForSubItems = True
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(3).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            For i = 0 To LVDetail.Columns.Count - 1
                itm.SubItems(i).BackColor = Color.White
            Next

            If kind = "上海57所传感器" Then
                AddNewIco(lng, lat, dName, SH57Ico, WebGis)
            End If
            If kind = "一体机" Then '一体机
                AddNewIco(lng, lat, dName, AllinoneIco, WebGis)
            End If
            If kind = "测向站" Then '一体机
                AddNewIco(lng, lat, dName, DirectionIco, WebGis)
            End If
            If kind = "测向站" Then '一体机
                AddNewIco(lng, lat, dName, DirectionIco, WebGis)
            End If
            If kind = "频谱传感器" Then '一体机
                AddNewIco(lng, lat, dName, TssIco, WebGis)
            End If
            If kind = "微型传感器" Then '测向
                AddNewIco(lng, lat, dName, TZBQIco, WebGis)
            End If
        Next
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            itm.UseItemStyleForSubItems = True
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(3).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            Dim Vender As String = itm.SubItems(5).Text
            If Vender = "713" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Green
                Next
                AddNewIco(lng, lat, dName, SH713Ico, WebGis)
            Else
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.White
                Next
            End If
        Next
    End Sub

    Private Sub RBGateWay_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RBGateWay.CheckedChanged
        CleanGis(WebGis)
        For Each itm As ListViewItem In LVDetail.Items
            itm.UseItemStyleForSubItems = True
            Dim dName As String = itm.SubItems(1).Text
            Dim status As String = itm.SubItems(3).Text
            Dim kind As String = itm.SubItems(4).Text
            Dim lng As String = itm.SubItems(6).Text
            Dim lat As String = itm.SubItems(7).Text
            Dim NetSwitch As String = itm.SubItems(8).Text
            Dim HaveGateWay As String = itm.SubItems(9).Text
            If HaveGateWay = "1" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Green
                Next
                AddNewIco(lng, lat, dName, SH57_713Ico, WebGis)
            Else
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.White
                Next
            End If
        Next
    End Sub

    Private Sub Panel6_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel6.Paint

    End Sub

    Private Sub 切换到云平台ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 切换到云平台ToolStripMenuItem.Click
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim dName As String = itm.SubItems(1).Text
        Dim status As String = itm.SubItems(3).Text
        Dim kind As String = itm.SubItems(4).Text
        Dim lng As String = itm.SubItems(6).Text
        Dim lat As String = itm.SubItems(7).Text
        Dim NetSwitch As String = itm.SubItems(8).Text
        Dim HaveGateWay As String = itm.SubItems(9).Text
        If HaveGateWay = "1" Then
            SwitchOut(dName)
        Else
            MsgBox("该设备不支持切换网络")
        End If
    End Sub

    Private Sub 切换到监测网ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 切换到监测网ToolStripMenuItem.Click
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim dName As String = itm.SubItems(1).Text
        Dim status As String = itm.SubItems(3).Text
        Dim kind As String = itm.SubItems(4).Text
        Dim lng As String = itm.SubItems(6).Text
        Dim lat As String = itm.SubItems(7).Text
        Dim NetSwitch As String = itm.SubItems(8).Text
        Dim HaveGateWay As String = itm.SubItems(9).Text
        If HaveGateWay = "1" Then
            SwitchIn(dName)
        Else
            MsgBox("该设备不支持切换网络")
        End If
    End Sub
End Class
