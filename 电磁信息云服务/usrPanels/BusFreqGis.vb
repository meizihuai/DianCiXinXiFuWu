Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms.DataVisualization.Charting
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net
Imports System.Math
Imports SpeechLib
Imports System.Media
Imports OfficeOpenXml
Public Class BusFreqGis
    Private HttpMsgUrl As String
    Private selectDeviceId As String
    Private selectLineId As String
    Private ReciveDevMsgThread As Thread
    Private Thread_PPSJ As Thread
    Private BusLineList As List(Of BusLine)
    Private selectBusLine As BusLine
    Private BusIco As String = "http://123.207.31.37:8082/bmapico/bus.png"
    Private dik As New Dictionary(Of String, String) 'Tek设备和网
    Private flagShowAllLine As Boolean = False
    Private showAllLineThread As Thread
    Private Sub BusFreqGis_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        If IsNothing(showAllLineThread) = False Then
            Try
                showAllLineThread.Abort()
            Catch ex As Exception

            End Try
        End If
    End Sub
    Private Sub BusFreqGis_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        Label2.Enabled = False
        WebGis.ObjectForScripting = Me

        dik.Add("Tek180508", "DSG-GW0101")
        dik.Add("Tek180509", "DSG-GW0102")
        dik.Add("Tek180510", "DSG-GW0103")
        dik.Add("Tek180511", "DSG-GW0100")
        ini()
        iniChart1()
        If isLoadGis Then
            flagShowAllLine = CheckBox2.Checked
            WebGis.Navigate(gisurl)
        Else
            CheckBox2.Enabled = False
            flagShowAllLine = False
            GetBusDeviceList()
        End If
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 50)
        LVDetail.Columns.Add("线路名称", 100)
        LVDetail.Columns.Add("设备名称", 80)
        LVDetail.Columns.Add("设备ID", 80)
        LVDetail.Columns.Add("公交号", 80)
        LVDetail.Columns.Add("车牌号", 80)
        LVDetail.Columns.Add("经度", 100)
        LVDetail.Columns.Add("纬度", 100)
        LVDetail.Columns.Add("最新上报时间", 100)
        LVDetail.Columns.Add("最新地址", 100)
    End Sub
    Private Sub iniChart1()
        Chart1.Series.Clear()
        'Chart1.ChartAreas(0).CursorX.IsUserEnabled = True
        'Chart1.ChartAreas(0).CursorY.IsUserEnabled = True
        Chart1.ChartAreas(0).AxisY.Maximum = -20
        Chart1.ChartAreas(0).AxisY.Minimum = -120
        Chart1.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart1.ChartAreas(0).AxisY.Interval = 20
        'Chart1.ChartAreas(0).AxisX.Maximum = 0
        'Chart1.ChartAreas(0).AxisX.Minimum = 800
        'Chart1.ChartAreas(0).AxisX.Interval = 100
        Dim Series As New Series("频谱")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        Series.Name = "频谱"
        Series.LabelToolTip = "频率：#VALX 场强：#VAL"
        Series.ToolTip = "频率：#VALX 场强：#VAL"
        Chart1.Series.Add(Series) '0

        Series = New Series("markLine")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.Line
        Series.IsVisibleInLegend = False
        Series.Color = Color.Red
        Series.Name = "markLine"
        Series.LabelToolTip = "频率：#VALX 场强：#VAL"
        Series.ToolTip = "频率：#VALX 场强：#VAL"
        'Series.Label = "频率：#VALX 场强：#VAL"
        'Series.IsValueShownAsLabel = True


        'Series.Label = "#VALX" & "," & "#VAL"
        'Series.Points.AddXY(90, -20)
        'Series.Points.AddXY(90, -120)
        Chart1.Series.Add(Series) '1
        Series = New Series("markPoint")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.Point
        Series.IsVisibleInLegend = False
        Series.Color = Color.Red
        Series.Name = "markPoint"
        Series.LabelToolTip = "频率：#VALX 场强：#VAL"
        Series.ToolTip = "频率：#VALX 场强：#VAL"
        Series.Label = "频率：#VALX MHz 场强：#VAL dBm"
        Series.IsValueShownAsLabel = True
        Chart1.Series.Add(Series) '2

        Series = New Series("MoudleFreq")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Gray
        Series.Name = "MoudleFreq"
        Chart1.Series.Add(Series) '3
    End Sub
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        flagShowAllLine = CheckBox2.Checked
        If flagShowAllLine = False Then
            CleanGis(WebGis)
            If IsNothing(showAllLineThread) = False Then
                Try
                    showAllLineThread.Abort()
                Catch ex As Exception

                End Try
            End If
        End If
    End Sub

    Private Sub ShowAllLine()
        If flagShowAllLine = False Then Return
        If IsNothing(showAllLineThread) = False Then
            Try
                showAllLineThread.Abort()
            Catch ex As Exception

            End Try
        End If
        showAllLineThread = New Thread(AddressOf GetAllLineInfoLoop)
        showAllLineThread.Start()
    End Sub
    Private Sub GetAllLineInfoLoop()
        Dim oldTekBusDeviceInfo As TekBusDeviceInfo() = Nothing
        While True
            Try
                Dim isLockGisView As Boolean = CheckBox1.Checked
                Dim np As normalResponse = GetServerNp("func=GetTekBusAllInfo")
                If np.result Then
                    Dim str As String = np.data.ToString
                    Dim TekBusDevices() As TekBusDeviceInfo = JsonConvert.DeserializeObject(str, GetType(TekBusDeviceInfo()))
                    If IsNothing(TekBusDevices) = False Then
                        If IsNothing(oldTekBusDeviceInfo) Then
                            For i = 0 To TekBusDevices.Count - 1
                                Dim itm As TekBusDeviceInfo = TekBusDevices(i)
                                Dim jsNameTmp As String = "addFreqGisPoint"
                                Dim oinfo As String = itm.TekDeviceId & "," & itm.gpsTime
                                itm.oldPointInfo = oinfo
                                Dim lng As Double = 0
                                Dim lat As Double = 0
                                If itm.HkCoordinfo.x > 0 And itm.HkCoordinfo.y > 0 Then
                                    lng = itm.HkCoordinfo.x
                                    lat = itm.HkCoordinfo.y
                                End If
                                If itm.GwCoordinfo.x > 0 And itm.GwCoordinfo.y > 0 Then
                                    lng = itm.GwCoordinfo.x
                                    lat = itm.GwCoordinfo.y
                                End If
                                If itm.TekCoordinfo.x > 0 And itm.TekCoordinfo.y > 0 Then
                                    lng = itm.TekCoordinfo.x
                                    lat = itm.TekCoordinfo.y
                                End If
                                itm.lng = lng
                                itm.lat = lat
                                TekBusDevices(i) = itm
                                If lng = 0 Or lat = 0 Then Continue For
                                Dim objTmp() As Object = New Object() {lng, lat, oinfo, True, Not isLockGisView, 14, lng, lat, False, "blue", itm.lineId, "", 10, 0.5}
                                script(jsNameTmp, objTmp, WebGis)
                            Next
                            oldTekBusDeviceInfo = TekBusDevices
                        Else
                            For i = 0 To TekBusDevices.Count - 1
                                Dim itm As TekBusDeviceInfo = TekBusDevices(i)
                                Dim jsNameTmp As String = "addFreqGisPoint"
                                Dim oinfo As String = itm.TekDeviceId & "," & itm.gpsTime
                                Dim tmpOldInfo As String = oldTekBusDeviceInfo(i).oldPointInfo
                                itm.oldPointInfo = oinfo
                                Dim lng As Double = 0
                                Dim lat As Double = 0
                                If itm.HkCoordinfo.x > 0 And itm.HkCoordinfo.y > 0 Then
                                    lng = itm.HkCoordinfo.x
                                    lat = itm.HkCoordinfo.y
                                End If
                                If itm.GwCoordinfo.x > 0 And itm.GwCoordinfo.y > 0 Then
                                    lng = itm.GwCoordinfo.x
                                    lat = itm.GwCoordinfo.y
                                End If
                                If itm.TekCoordinfo.x > 0 And itm.TekCoordinfo.y > 0 Then
                                    lng = itm.TekCoordinfo.x
                                    lat = itm.TekCoordinfo.y
                                End If
                                itm.lng = lng
                                itm.lat = lat

                                TekBusDevices(i) = itm
                                Dim oldTmpLng As Double = oldTekBusDeviceInfo(i).lng
                                Dim oldTmpLat As Double = oldTekBusDeviceInfo(i).lat
                                If oldTmpLng = 0 Or oldTmpLat = 0 Then
                                    oldTmpLng = lng
                                    oldTmpLat = lat
                                End If
                                If tmpOldInfo <> "" Then
                                    Dim jsName2 As String = "deletePoint"
                                    Dim obj2 As Object() = New Object() {tmpOldInfo}
                                    script(jsName2, obj2, WebGis)
                                End If
                                If lng > 0 And lat > 0 Then
                                    Dim objTmp() As Object = New Object() {lng, lat, oinfo, True, Not isLockGisView, 14, oldTmpLng, oldTmpLat, True, "blue", itm.lineId, "", 10, 0.5}
                                    script(jsNameTmp, objTmp, WebGis)
                                End If

                            Next
                            oldTekBusDeviceInfo = TekBusDevices
                        End If
                    End If
                End If
            Catch ex As Exception

            End Try
            Sleep(16 * 1000)
        End While
    End Sub
    Private Sub GetBusDeviceList()
        Dim th As New Thread(AddressOf th_GetBusDeviceList)
        th.Start()
    End Sub
    Private Sub th_GetBusDeviceList()
        Label2.Visible = True
        Dim result As String = GetH(ServerUrl, "func=GetBusLines&token=" & token)
        Label2.Visible = False
        If result = "" Or result = "[]" Then
            BusLineList = Nothing
            Return
        End If
        Try
            BusLineList = JsonConvert.DeserializeObject(result, GetType(List(Of BusLine)))
            BusLineList2LV()
            If selectDeviceId = "" Then
                SelectLine(0)
            Else
                Dim isfind As Boolean = False
                For Each itm In BusLineList
                    If itm.deviceId = selectDeviceId Then
                        isfind = True
                        Exit For
                    End If
                Next
                If isfind = False Then
                    SelectLine(0)
                End If
            End If
        Catch ex As Exception
            BusLineList = Nothing
        End Try
    End Sub
    Private Sub BusLineList2LV()
        LVDetail.Items.Clear()
        Dim int As Integer = 0
        For Each bus In BusLineList
            int = int + 1
            Dim itm As New ListViewItem(int)
            itm.SubItems.Add(bus.lineName)

            itm.SubItems.Add(bus.deviceName)
            itm.SubItems.Add(bus.deviceId)
            itm.SubItems.Add(bus.busNo)
            itm.SubItems.Add(bus.plateNumber)
            itm.SubItems.Add(bus.lng)
            itm.SubItems.Add(bus.lat)
            itm.SubItems.Add(bus.time)
            itm.SubItems.Add(bus.location)
            LVDetail.Items.Add(itm)
        Next
    End Sub
    Private selectMsgId As String
    Public Function ShowHisFreqGis(ByVal lngTmp As String, ByVal latTmp As String, ByVal lineId As String, ByVal msgid As String)
        selectMsgId = msgid
        Dim H As New HisFreqGis(lngTmp, latTmp, msgid, JsonConvert.SerializeObject(selectBusLine))
        H.Show()
        Return ""
    End Function
    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim p As New HisFreqGis(lbl_Lng.Text, lbl_Lat.Text, selectMsgId, JsonConvert.SerializeObject(selectBusLine))
        p.Show()
    End Sub
    Private Sub WebGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        GetBusDeviceList()
        ShowAllLine()
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetBusDeviceList()
    End Sub
    'Private Function GetlocationByGateWay(deviceId As String) As runLocation
    '    ' http://123.207.31.37:8080/?func=GetGateWayLocation&deviceId=DSG-GW0100&token=928453310
    '    If dik.ContainsKey(deviceId) = False Then Return Nothing
    '    Dim dsgGateWayDeviceId As String = dik(deviceId)
    '    Dim np As normalResponse = GetServerNp("func=GetGateWayLocation&deviceId=" & dsgGateWayDeviceId)
    '    If np.result Then
    '        If IsNothing(np.data) Then Return Nothing
    '        Dim dataStr As String = np.data.ToString
    '        Console.WriteLine("GateWay DeivceId=" & dsgGateWayDeviceId & ",经纬度:" & dataStr)
    '        If dataStr = "" Then Return Nothing
    '        If dataStr.Contains(",") = False Then Return Nothing
    '        Dim st() As String = dataStr.Split(",")
    '        If st.Length <> 2 Then Return Nothing
    '        Dim lng As String = st(0)
    '        Dim lat As String = st(1)
    '        If IsNumeric(lng) = False Then Return Nothing
    '        If IsNumeric(lat) = False Then Return Nothing
    '        If lng = 0 Or lat = 0 Then Return Nothing
    '        lbl_Lng.Text = lng
    '        lbl_Lat.Text = lat
    '        Dim dateNow As Date = Now
    '        lbl_Time.Text = dateNow.ToString("yyyy-MM-dd HH:mm:ss")
    '        '   Return Nothing
    '        HandleFreqGis(lng, lat, dateNow.ToString("yyyy-MM-dd HH:mm:ss"), True)
    '    End If
    'End Function
    Private Sub 选择该条线路ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 选择该条线路ToolStripMenuItem.Click
        If LVDetail.Items.Count = 0 Then Return
        If LVDetail.SelectedIndices.Count = 0 Then Return
        Dim index As Integer = LVDetail.SelectedIndices(0)
        SelectLine(index)
    End Sub
    Private Sub SelectLine(ByVal index As Integer)
        If IsNothing(BusLineList) Then Return
        If index >= BusLineList.Count Then Return
        Dim bus As BusLine = BusLineList(index)
        selectBusLine = bus
        selectDeviceId = bus.deviceId
        selectLineId = bus.lineId
        lbl_LineName.Text = bus.lineName
        lbl_DeviceName.Text = bus.deviceName
        If bus.lng <> "" And bus.lat <> "" Then
            If IsNumeric(bus.lng) And IsNumeric(bus.lat) Then
                If bus.lng > 0 And bus.lat > 0 Then
                    lbl_Lng.Text = bus.lng
                    lbl_Lat.Text = bus.lat
                    lbl_Time.Text = bus.time
                End If

            End If
        End If
        If IsNothing(ReciveDevMsgThread) = False Then
            Try
                ReciveDevMsgThread.Abort()
            Catch ex As Exception

            End Try
        End If
        CleanGis(WebGis)
        ReciveDevMsgThread = New Thread(AddressOf ReviceDevMsg)
        ReciveDevMsgThread.Start()
    End Sub
    Private Sub ReviceDevMsg()
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & selectDeviceId & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & selectDeviceId & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Console.WriteLine("HttpMsgUrl=" & HttpMsgUrl)
        While True
            Dim result As String = GetHttpMsg()
            '  GetlocationByGateWay(selectDeviceId)
            If result = "" Then Continue While
            HandleHttpMsg(result)

        End While

    End Sub
    Private Function GetHttpMsg() As String
        Try
            Dim req As HttpWebRequest = WebRequest.Create(HttpMsgUrl & "?func=GetDevMsg")
            ' Me.Invoke(Sub() MsgBox(HttpMsgUrl & "?func=GetDevMsg"))
            req.Accept = "*/*"
            req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
            req.KeepAlive = True
            req.Timeout = 5000
            req.ReadWriteTimeout = 5000
            req.ContentType = "application/x-www-form-urlencoded"
            req.Method = "GET"

            Dim rp As HttpWebResponse = req.GetResponse
            Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd
            Dim b() As Byte = Encoding.Default.GetBytes(str)
            dowload = dowload + b.Length
            Return str
        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try
    End Function
    Structure runLocation
        Dim lng As String
        Dim lat As String
        Dim time As String
    End Structure
    Structure json_PPSJ
        Dim freqStart As Double
        Dim freqStep As Double
        Dim freqEnd As Double
        Dim deviceID As String
        Dim dataCount As Integer
        Dim runLocation As runLocation
        Dim value() As Double
        Dim isDSGFreq As Boolean
        Dim DSGFreqBase64 As String
    End Structure
    Private Sub HandleHttpMsg(ByVal HttpMsg As String)
        Console.WriteLine("收到新消息TSS  " & Now.ToString("HH:mm:ss"))
        Dim PPSJList As New List(Of json_PPSJ)
        Try
            Dim p As JArray = JArray.Parse(HttpMsg)
            For Each itm As JValue In p
                Dim jMsg As String = itm.Value
                Dim JObj As Object = JObject.Parse(jMsg)
                Dim func As String = JObj("func").ToString
                If func = "bscan" Then
                    Dim msg As String = JObj("msg").ToString
                    Dim ppsj As json_PPSJ = JsonConvert.DeserializeObject(msg, GetType(json_PPSJ))
                    PPSJList.Add(ppsj)
                End If
            Next
        Catch ex As Exception
            Return
        End Try
        If PPSJList.Count > 0 Then
            If IsNothing(Thread_PPSJ) = False Then
                Try
                    Thread_PPSJ.Abort()
                Catch ex As Exception

                End Try
            End If
            Thread_PPSJ = New Thread(AddressOf HandlePPSJList)
            Thread_PPSJ.Start(PPSJList)
        End If
    End Sub
    Dim DisPlayLock As Object
    Private Sub HandlePPSJList(ByVal Plist As List(Of json_PPSJ))
        If IsNothing(Plist) Then Exit Sub
        If Plist.Count = 0 Then Exit Sub
        If IsNothing(DisPlayLock) Then DisPlayLock = New Object
        Dim count As Integer = Plist.Count
        Dim sleepCount As Double = (GetHttpMsgTimeSpan * 1000 - 100) / (count + 1)
        SyncLock DisPlayLock
            For i = 0 To Plist.Count - 1
                Try
                    Dim itm As json_PPSJ = Plist(i)
                    Console.WriteLine(itm.runLocation.lng & "," & itm.runLocation.lat)
                    Console.WriteLine(itm.freqStart & "," & itm.freqStep & "," & itm.dataCount)
                    Me.Invoke(Sub() handlePinPuFenXi(itm))
                Catch ex As Exception

                End Try
                If i <> count - 1 Then
                    Sleep(sleepCount)
                End If
            Next
        End SyncLock
    End Sub
    Private OldLng As Single
    Private OldLat As Single
    Dim oldPointInfo As String = ""
    Private Sub HandleFreqGis(ByVal lng As Single, ByVal lat As Single, ByVal time As String, isGateWayLocation As Boolean, lineId As String, TekDeviceId As String)
        'AddPoint(lng, lat, "", WebGis)
        'setGisCenter(lng, lat, WebGis)
        If isGateWayLocation = True Then Return
        If lng = 0 Or lat = 0 Then Return
        Dim isLockGisView As Boolean = CheckBox1.Checked
        Dim coordes As String = ""

        If OldLng = 0 Or OldLat = 0 Then
            OldLng = lng
            OldLat = lat
            'addPolyline(OldLng, OldLat, OldLat, OldLat, True, "blue", WebGis)
            'Dim jsOrder As String = "setcenter3"
            'script(jsOrder, New String() {OldLng, OldLat, 18}, WebGis)

            Dim jsNameTmp As String = "addFreqGisPoint"
            Dim objTmp() As Object = New Object() {lng, lat, time, True, Not isLockGisView, 14, OldLng, OldLat, False, "blue", lineId, "", 10, 0.5}
            script(jsNameTmp, objTmp, WebGis)
            oldPointInfo = time
            'coordes = lng & "," & lat
            'wdpoints = GPS2BDS(coordes)
            'If IsNothing(wdpoints) = False Then
            '    If wdpoints.Count = 1 Then
            '        Dim jsOrder As String = "setcenter3"
            '        script(jsOrder, New String() {wdpoints(0).x, wdpoints(0).y, 18}, WebGis)
            '        ' setGisCenter(wdpoints(0).x, wdpoints(0).y, WebGis)
            '    End If
            'End If
            Return
        End If

        Dim jsName As String = "addFreqGisPoint"
        Dim obj() As Object = New Object() {lng, lat, time, True, Not isLockGisView, 14, OldLng, OldLat, True, "blue", lineId, "", 10, 0.5}
        script(jsName, obj, WebGis)
        If oldPointInfo = "" Then
            oldPointInfo = time
        Else
            jsName = "deletePoint"
            obj = New Object() {oldPointInfo}
            script(jsName, obj, WebGis)
            oldPointInfo = time
        End If
        'addFreqGisPoint(lng, lat, info, isCenter, centerSize, oldlng, oldlat, isMakePolyLine, polyLineColor, lineId,strokeWeight,strokeOpacity) 
        '  addPolyline(OldLng, OldLat, lng, lat, True, "blue", WebGis)

        'coordes = OldLng & "," & OldLat & ";" & lng & "," & lat
        'wdpoints = GPS2BDS(coordes)
        'If IsNothing(wdpoints) = False Then
        '    If wdpoints.Count = 2 Then
        '        addPolyline(wdpoints(0).x, wdpoints(0).y, wdpoints(1).x, wdpoints(1).y, True, "blue", WebGis)
        '        Dim jsOrder As String = "setcenter3"
        '        '  script(jsOrder, New String() {wdpoints(1).x, wdpoints(1).y, 18}, WebGis)
        '        ' Console.WriteLine("addPolyline," & wdpoints(0).x & "," & wdpoints(0).y & ";" & wdpoints(1).x & "," & wdpoints(1).y)
        '    End If
        'End If

        OldLng = lng
        OldLat = lat
    End Sub

    Dim recentFreqStart As Double
    Dim recentFreqEnd As Double
    Private Function GetLen(ByVal k As Double) As String
        Dim str As String = ""
        If k < 1024 Then
            Return k.ToString("0.00") & " b"
        End If
        Dim d As Double = 1024 * 1024
        If 1024 <= k And k < d Then
            Return (k / 1024).ToString("0.00") & " kb"
        End If

        Return (k / d).ToString("0.00") & " Mb"
    End Function
    Private Sub handlePinPuFenXi(ByVal p As json_PPSJ)
        Dim jstmp As String = JsonConvert.SerializeObject(p)
        Dim jsLen As Long = jstmp.Length
        Dim jslenstr As String = GetLen(jsLen)
        Dim freqStart As Double = p.freqStart
        Dim freqStep As Double = p.freqStep
        Dim yy() As Double = p.value
        Dim isDSGFreq As Boolean = False
        If p.isDSGFreq Then
            isDSGFreq = True
            yy = DSGBase2PPSJValues(p.DSGFreqBase64)
        End If
        If IsNothing(yy) Then
            Exit Sub
        End If
        If IsNothing(p.runLocation) = False Then
            Dim lng As Double = p.runLocation.lng
            Dim lat As Double = p.runLocation.lat
            Dim time As String = p.runLocation.time
            lbl_Lng.Text = lng
            lbl_Lat.Text = lat
            lbl_Time.Text = time
            HandleFreqGis(lng, lat, "选中线路 " & p.deviceID & " " & time, False, selectLineId, selectDeviceId)
        End If
        Dim dataCount As Integer = yy.Count
        Dim deviceID As String = p.deviceID
        Dim maxCount As Integer = 5000
        Dim xx() As Double
        If dataCount < maxCount Then
            ReDim xx(dataCount - 1)
            For i = 0 To yy.Count - 1
                xx(i) = freqStart + i * freqStep
            Next
            'Console.WriteLine(freqStart)
            'Console.WriteLine(freqStep)
            'Console.WriteLine(xx(xx.Length - 1))
        Else
            Dim realValue() As Double = yy
            Dim xlist As New List(Of Double)
            Dim ylist As New List(Of Double)
            Dim st As Integer = Math.Ceiling(dataCount / maxCount)
            For i = 0 To dataCount - 1 Step st
                xlist.Add(freqStart + i * freqStep)
                ylist.Add(realValue(i))
            Next
            If xlist(xlist.Count - 1) <> freqStart + (dataCount - 1) * freqStep Then
                xlist.Add(freqStart + (dataCount - 1) * freqStep)
                ylist.Add(yy(yy.Length - 1))
            End If
            xx = xlist.ToArray
            yy = ylist.ToArray
        End If

        'If isSerchLocalDianTai Then
        '    handleSerchLoaclDiantai(xx, yy)
        '    isSerchLocalDianTai = False
        '    Exit Sub
        'End If
        Dim jieshu As Double = freqStart + (dataCount - 1) * freqStep
        jieshu = p.freqEnd
        If freqStart <> recentFreqStart Or jieshu <> recentFreqEnd Then
            recentFreqStart = freqStart
            recentFreqEnd = jieshu
        End If
        Dim Series As New Series("频谱")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        Series.Name = "频谱"
        Series.LabelToolTip = "频率：#VALX 场强：#VAL"
        Series.ToolTip = "频率：#VALX 场强：#VAL"
        'Series.Label = "频率：#VALX 场强：#VAL"
        'Series.IsValueShownAsLabel = True
        Chart1.ChartAreas(0).AxisX.Minimum = freqStart
        Chart1.ChartAreas(0).AxisX.Maximum = jieshu
        Chart1.ChartAreas(0).AxisX.Interval = (jieshu - freqStart) / 5
        For i = 0 To xx.Length - 1
            Series.Points.AddXY(xx(i), yy(i))
        Next
        If Chart1.Series.Count = 0 Then
            Chart1.Series.Add(Series)
        Else
            Chart1.Series(0) = Series
        End If
        If Chart1.Series.Count >= 3 Then
            If Chart1.Series(1).Points.Count >= 1 Then
                Dim xValue As Double = Chart1.Series(1).Points(0).XValue
                For Each ppt In Series.Points
                    If ppt.XValue = xValue Then
                        Chart1.Series(2).Points.Clear()
                        Chart1.Series(2).Points.AddXY(xValue, ppt.YValues(0))
                        ' Chart1.Series(2).Points.AddXY(xValue, -25)
                        Exit For
                    End If
                Next
            End If
        End If
        Dim str As String = "时间: " & Now.ToString("HH:mm:ss") & "  频率范围:[" & freqStart & "," & jieshu & "]"
        'str = "Mark " & TimeFreqPoint & "MHz," & TimeMarkPointValue & "dBm" & "   " & str
        If isDSGFreq Then
            str = str & "  <DSG频谱压缩 " & jslenstr & ">"
        Else
            str = str & "  <" & jslenstr & ">"
        End If
        Label4.Text = str
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
    Delegate Sub wt_script(ByVal scriptName As String, ByVal str() As Object, ByVal web As WebBrowser)
    Private Sub script(ByVal scriptName As String, ByVal str() As Object, ByVal web As WebBrowser)
        Dim d As New wt_script(AddressOf th_script)
        Dim b(2) As Object
        b(0) = scriptName
        b(1) = str
        b(2) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub th_script(ByVal scriptName As String, ByVal str() As Object, ByVal web As WebBrowser)
        Dim doc As HtmlDocument = web.Document
        'Dim O(str.Count - 1) As Object
        'For i = 0 To str.Length - 1
        '    O(i) = CObj(str(i))
        'Next
        doc.InvokeScript(scriptName, str)
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
    Delegate Sub wt_addPolyline(ByVal lng1 As String, ByVal lat1 As String, ByVal lng2 As String, ByVal lat2 As String, ByVal flagSetcenter As Boolean, ByVal color As String, ByVal web As WebBrowser)
    Private Sub addPolyline(ByVal lng1 As String, ByVal lat1 As String, ByVal lng2 As String, ByVal lat2 As String, ByVal flagSetcenter As Boolean, ByVal color As String, ByVal web As WebBrowser)
        Dim d As New wt_addPolyline(AddressOf th_addPolyline)
        Dim b(6) As Object
        b(0) = lng1
        b(1) = lat1
        b(2) = lng2
        b(3) = lat2
        b(4) = flagSetcenter
        b(5) = color
        b(6) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub th_addPolyline(ByVal lng1 As String, ByVal lat1 As String, ByVal lng2 As String, ByVal lat2 As String, ByVal flagSetcenter As Boolean, ByVal color As String, ByVal web As WebBrowser)
        Dim doc As HtmlDocument = web.Document
        If IsNothing(doc) Then Exit Sub
        Dim b(7) As Object
        b(0) = CObj(lng1)
        b(1) = CObj(lat1)
        b(2) = CObj(lng2)
        b(3) = CObj(lat2)
        b(4) = CObj(flagSetcenter)
        b(5) = CObj(color)
        b(6) = CObj(BusIco)
        b(7) = CObj(selectLineId)
        ' MsgBox(lng1 & "," & lat1 & "  " & lng2 & "," & lat2)
        doc.InvokeScript("addPolyline", b)
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

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub



    Private Sub PictureBox3_Resize(sender As Object, e As EventArgs) Handles PictureBox3.Resize

    End Sub

    Private Sub BusFreqGis_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Panel7.Height = Me.Height * 0.2
    End Sub

    Private Sub LVDetail_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LVDetail.SelectedIndexChanged

    End Sub
End Class
