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
Public Class BusHisFreqGis
    Private HttpMsgUrl As String
    Private selectDeviceId As String
    Private selectLineId As String
    Private ReciveDevMsgThread As Thread
    Private Thread_PPSJ As Thread
    Private BusLineList As List(Of BusLine)
    Private selectBusLine As BusLine
    Private isShowFreq As Boolean
    Private sigNalPointValue As Double
    Private BusIco As String = "http://123.207.31.37:8082/bmapico/bus.png"
    Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub

    Private Sub BusHisFreqGis_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        Label2.Visible = False
        Label2.ForeColor = Color.White
        WebGis.ObjectForScripting = Me
        RDFreq.Checked = True
        PanelSignal.Visible = False
        ini()
        CheckBox1.Checked = True
        iniChart1()
        'TxtStartTime.Text = Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00")
        'TxtEndTime.Text = Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00")
        ' TxtStartTime.Text = "2018-11-19 00:00:00"
        DTP.MinDate = "2018-11-10"
        Dim dtmp As Date = Now
        DTP.MaxDate = dtmp.ToString("yyyy-MM-dd 23:59:59")
        DTP.Value = dtmp.AddDays(-1).ToString("yyyy-MM-dd 00:00:00")
        '  DTP.Value = dtmp.ToString("2018-11-19 00:00:00")
        If isLoadGis Then
            WebGis.Navigate(gisurl)
            'Dim path As String = "file:///C:/Users/meizi/Desktop/BmapHTML/Baidumap.html"
            'WebGis.Navigate(path)
        Else
            GetBusDeviceList()
        End If
        'AddHandler WebGis.Document.Window.Error, AddressOf Window_Error
    End Sub
    Private Sub Window_Error(ByVal sender As Object, ByVal e As HtmlElementErrorEventArgs)
        Try
            e.Handled = True
            Return
        Catch ex As Exception

        End Try
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
        Chart1.ChartAreas(0).AxisY.Maximum = -40
        Chart1.ChartAreas(0).AxisY.Minimum = -100
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


        Series = New Series("illegalsignal")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        '  Series.BorderWidth = "0.5"
        Series.ChartType = SeriesChartType.Column
        Series.IsVisibleInLegend = False
        Series.Color = Color.Red
        Series.Name = ""
        Chart1.Series.Add(Series)   '4
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
    Private Sub SelectLine(ByVal index As Integer)
        If IsNothing(BusLineList) Then Return
        If index >= BusLineList.Count Then Return
        Dim bus As BusLine = BusLineList(index)
        selectBusLine = bus
        selectDeviceId = bus.deviceId
        selectLineId = bus.lineId
        lbl_LineName.Text = bus.lineName
        lbl_DeviceName.Text = bus.deviceName
        lbl_Lng.Text = bus.lng
        lbl_Lat.Text = bus.lat
        lbl_Time.Text = bus.time
        If IsNothing(ReciveDevMsgThread) = False Then
            Try
                ReciveDevMsgThread.Abort()
            Catch ex As Exception

            End Try
        End If
        CleanGis(WebGis)
    End Sub
    Public Function ShowHisFreqGis(ByVal lngTmp As String, ByVal latTmp As String, ByVal lineId As String, ByVal msgId As String)
        'MsgBox(msgId)
        'Dim H As New HisFreqGis(lngTmp, latTmp, JsonConvert.SerializeObject(selectBusLine))
        'H.Show()
        Dim sb As New StringBuilder()
        sb.AppendLine("您选择的坐标是：" & lngTmp & "," & latTmp)
        sb.AppendLine("线路Id:" & lineId)
        sb.AppendLine("时间:" & lineId)
        Return ""
    End Function
    Public Function ShowHisFreqGisByJson(json As String)
        '  MsgBox(json)
        Dim hinfo As HaiLiangPointInfo = JsonConvert.DeserializeObject(json, GetType(HaiLiangPointInfo))
        Dim sb As New StringBuilder()
        sb.AppendLine("您选择的坐标是：" & hinfo.x & "," & hinfo.y)
        sb.AppendLine("线路Id:" & hinfo.lineId)
        sb.AppendLine("时间:" & hinfo.time)
        Label18.Text = hinfo.x
        Label17.Text = hinfo.y
        Label13.Text = hinfo.time
        Dim lindId As String = hinfo.lineId
        Dim msgId As String = hinfo.msgId
        selectedMsgId = msgId
    End Function
    Private selectedMsgId As String
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim p As New HisFreqGis(Label18.Text, Label17.Text, selectedMsgId, JsonConvert.SerializeObject(selectBusLine))
        p.Show()
    End Sub
    Private Sub GetBusHisFreqList()
        isShowFreq = RDFreq.Checked
        iniChart1()
        If isShowFreq = False Then
            Chart1.ChartAreas(0).AxisX.Enabled = AxisEnabled.False
            Dim txt As String = TxtSignal.Text
            If txt = "" Then
                MsgBox("您选择了信号模式回放，请设置好需要回放的信号频点值！")
                Return
            End If
            If IsNumeric(txt) = False Then
                MsgBox("您选择了信号模式回放，请设置好需要回放的信号频点值！")
                Return
            End If
            If Val(txt) <= 0 Then
                MsgBox("您选择了信号模式回放，请设置好需要回放的信号频点值！")
                Return
            End If
            sigNalPointValue = Val(TxtSignal.Text)
            If sigNalPointValue >= 30 And sigNalPointValue <= 6000 Then
            Else
                MsgBox("您选择了信号模式回放，但是您的信号频点值不在[30,6000]范围内！")
                Return
            End If
        Else
            Chart1.ChartAreas(0).AxisX.Enabled = AxisEnabled.True
        End If
        ReciveDevMsgThread = New Thread(AddressOf ThGetBusHisFreqList)
        ReciveDevMsgThread.Start()
    End Sub
    Structure HaiLiangPointInfo
        Dim x As Double
        Dim y As Double
        Dim lineId As String
        Dim msgId As String
        Dim time As String
        Sub New(ByVal x As Double, ByVal y As Double, lineId As String, msgId As String, time As String)
            Me.x = x
            Me.y = y
            Me.lineId = lineId
            Me.msgId = msgId
            Me.time = time
        End Sub
    End Structure
    Private Sub ThGetBusHisFreqList()
        Try
            Dim isLockGisView As Boolean = CheckBox1.Checked
            Label6.Text = "正在获取……"
            Dim dtmp As Date = DTP.Value
            Dim startTime As String = dtmp.ToString("yyyy-MM-dd 00:00:00")
            Dim endTime As String = dtmp.ToString("yyyy-MM-dd 23:59:59")
            Try
                Dim dStartTime As Date = Date.Parse(startTime)
                startTime = dStartTime.ToString("yyyy-MM-dd HH:mm:ss")
            Catch ex As Exception
                MsgBox("起始时间格式不正确")
            End Try
            Try
                Dim dEndTime As Date = Date.Parse(endTime)
                endTime = dEndTime.ToString("yyyy-MM-dd HH:mm:ss")
            Catch ex As Exception
                MsgBox("结束时间格式不正确")
            End Try
            Dim param As String = "func=GetBusHisFreqList&startTime=" & startTime & "&endTime=" & endTime & "&lineId=" & selectLineId
            Dim result As String = GetH(ServerUrl, param & "&token=" & token)
            Label6.Text = "历史频谱地图"
            Dim np As normalResponse = JsonConvert.DeserializeObject(result, GetType(normalResponse))
            If np.result = False Then
                MsgBox(np.msg)
                Return
            End If
            Dim tmpStr As String = np.data.ToString()
            CleanGis(WebGis)
            Dim dt As DataTable
            Try
                Dim haiLiangPointsList As New List(Of HaiLiangPointInfo)
                dt = JsonConvert.DeserializeObject(tmpStr, GetType(DataTable))
                If IsNothing(dt) Then Return
                If dt.Rows.Count = 0 Then Return
                'addFreqGisPoint(lng, lat, info, isShowPoint, isCenter, centerSize, oldlng, oldlat, isMakePolyLine, polyLineColor, lineId, msgId, strokeWeight, strokeOpacity)
                Dim nDt As New DataTable
                nDt.Columns.Add("lng")
                nDt.Columns.Add("lat")
                nDt.Columns.Add("info")
                nDt.Columns.Add("isCenter")
                nDt.Columns.Add("isShowPoint")
                nDt.Columns.Add("centerSize")
                nDt.Columns.Add("oldlng")
                nDt.Columns.Add("oldlat")
                nDt.Columns.Add("isMakePolyLine")
                nDt.Columns.Add("polyLineColor")
                nDt.Columns.Add("lineId")
                nDt.Columns.Add("msgId")
                nDt.Columns.Add("strokeWeight")
                nDt.Columns.Add("strokeOpacity")
                Dim oldLng As String = ""
                Dim oldLat As String = ""
                Dim isSetCenter As Boolean = True
                Dim sumCount As Integer = dt.Rows.Count
                ' sumCount = 100

                For i = 0 To sumCount - 1
                    Dim row As DataRow = dt.Rows(i)
                    Label12.Text = i + 1 & "/" & sumCount
                    'msgid,type,freqStart,freqEnd,freqStep,pointCount,lng,lat,freqJsonLen
                    Dim msgId As String = row("msgid")
                    Dim time As String = row("time")
                    Dim lng As String = row("lng")
                    Dim lat As String = row("lat")
                    If IsNothing(lng) = False And IsNothing(lat) = False Then
                        If IsNumeric(lng) And IsNumeric(lat) Then
                            If Val(lng) > 100 And Val(lat) > 10 Then
                                haiLiangPointsList.Add(New HaiLiangPointInfo(lng, lat, selectLineId, msgId, time))
                                Dim nRow As DataRow = nDt.NewRow
                                nRow("lng") = lng
                                nRow("lat") = lat
                                nRow("info") = ""
                                nRow("isCenter") = isSetCenter
                                nRow("isShowPoint") = False
                                nRow("centerSize") = 18
                                'If oldLng = "" Or oldLat = "" Then
                                '    oldLng = lng
                                '    oldLat = lat
                                '    nRow("oldlng") = oldLng
                                '    nRow("oldlat") = oldLat
                                '    nRow("isMakePolyLine") = False
                                'Else
                                '    nRow("oldlng") = oldLng
                                '    nRow("oldlat") = oldLat
                                '    nRow("isMakePolyLine") = False
                                '    oldLng = lng
                                '    oldLat = lat
                                'End If
                                nRow("polyLineColor") = "blue"
                                nRow("lineId") = selectLineId
                                nRow("msgId") = msgId
                                nRow("strokeWeight") = 10
                                nRow("strokeOpacity") = 0.5
                                nDt.Rows.Add(nRow)
                                Dim jsNameTmp As String = "addFreqGisPoint"
                                'addFreqGisPoint(lng, lat, info, isShowPoint, isCenter, centerSize, oldlng, oldlat, isMakePolyLine, polyLineColor, lineId, msgId, strokeWeight, strokeOpacity)
                                Dim obj() As Object = New Object() {lng, lat, "", False, isSetCenter, 13, oldLng, oldLat, False, "blue", selectLineId, msgId, 10, 0.5}
                                If oldLng = "" Or oldLat = "" Then
                                    oldLng = lng
                                    oldLat = lat
                                Else
                                    obj = New Object() {lng, lat, "", False, isSetCenter, 13, oldLng, oldLat, True, "blue", selectLineId, msgId, 10, 0.5}
                                    oldLng = lng
                                    oldLat = lat
                                End If
                                script(jsNameTmp, obj, WebGis)
                                If isLockGisView Then
                                    If isSetCenter Then isSetCenter = False
                                End If
                            End If
                        End If
                    End If
                Next
                If haiLiangPointsList.Count > 0 Then
                    Dim js As String = JsonConvert.SerializeObject(haiLiangPointsList)
                    script("addHaiLiangPoints", New Object() {js}, WebGis)
                End If
                Dim oldPointInfo As String = ""
                If Not isShowFreq Then
                    If Chart1.Series(0).Points.Count = 0 Then
                        Chart1.Series(0).Points.Add(-120)
                    End If
                    Dim series As New Series("illegalsignal")
                    series.XValueType = ChartValueType.Auto
                    series.ChartType = SeriesChartType.Column
                    series.IsVisibleInLegend = False
                    series.Color = Color.Blue
                    series.Name = ""
                    For i = 0 To sumCount - 1
                        series.Points.Add(-120)
                        ' Chart1.Series(4).Points.Add(-120)
                    Next
                    Chart1.Series(4) = series
                End If
                For i = 0 To sumCount - 1
                    Dim row As DataRow = dt.Rows(i)
                    Label12.Text = i + 1 & "/" & sumCount
                    Dim msgId As String = row("msgid")
                    Dim time As String = row("time")
                    Dim lng As String = row("lng")
                    Dim lat As String = row("lat")
                    If IsNothing(lng) = False And IsNothing(lat) = False Then
                        If IsNumeric(lng) And IsNumeric(lat) Then
                            If Val(lng) > 100 And Val(lat) > 10 Then
                                Dim jsNameTmp As String = "addFreqGisPoint"
                                Dim obj() As Object = New Object() {lng, lat, time, True, Not isLockGisView, 14, oldLng, oldLat, False, "blue", selectLineId, msgId, 10, 0.5}
                                script(jsNameTmp, obj, WebGis)
                                If oldPointInfo = "" Then
                                    oldPointInfo = time
                                Else
                                    jsNameTmp = "deletePoint"
                                    obj = New Object() {oldPointInfo}
                                    script(jsNameTmp, obj, WebGis)
                                    oldPointInfo = time
                                End If
                                GetBusHisFreqData(msgId, time, i, sumCount, lng, lat)
                            End If
                        End If
                    End If
                Next
                'Dim jsName As String = "addFreqGisPointList"
                'Dim jsonStr As String = JsonConvert.SerializeObject(nDt)
                'File.WriteAllText("jsonStr.json", jsonStr)
                'script(jsName, New Object() {jsonStr}, WebGis)
            Catch ex As Exception
                ' MsgBox(ex.ToString)
            End Try
        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try
    End Sub
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
    Private Sub GetBusHisFreqData(ByVal msgid As String, ByVal time As String, index As Integer, sumCount As Integer, lng As Double, lat As Double)
        If msgid = "" Then Return
        If IsNumeric(msgid) = False Then Return
        Dim np As normalResponse = GetServerNp("func=GetBusHisFreqData&msgid=" & msgid)
        If np.result Then
            Dim msg As String = np.data.ToString
            Dim ppsj As json_PPSJ = JsonConvert.DeserializeObject(msg, GetType(json_PPSJ))
            Me.Invoke(Sub() handlePinPuFenXi(ppsj, time, index, sumCount, lng, lat))
        End If
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
    Dim handledPointCount As Integer = 0
    Private Sub handlePinPuFenXi(ByVal p As json_PPSJ, ByVal msgTime As String, index As Integer, sumCount As Integer, lng As Double, lat As Double)
        'If handledPointCount >= 1000 Then
        '    handledPointCount = 0
        'End If
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
        Dim runLocationTime As String = ""
        Dim lngTmp As Double
        Dim latTmp As Double
        Dim time As String
        If IsNothing(p.runLocation) = False Then
            lngTmp = p.runLocation.lng
            latTmp = p.runLocation.lat
            time = p.runLocation.time
            runLocationTime = time
            lbl_Lng.Text = lngTmp
            lbl_Lat.Text = latTmp
            lbl_Time.Text = time
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
        If isShowFreq Then
            ShowFreq(freqStart, jieshu, xx, yy, isDSGFreq, jslenstr, runLocationTime)
        Else
            ShowSigNal(freqStart, jieshu, xx, yy, isDSGFreq, jslenstr, runLocationTime, index, sumCount, lng, lat, time)
        End If
    End Sub
    Dim sigNalMaxPointValue As Double = 0
    Dim sigNalMaxPointIndex As Integer = 0
    Dim sigNalMaxPointInfo As String = ""
    Private Sub ShowFreq(ByVal freqStart As Double, ByVal jieshu As Double, ByVal xx() As Double, ByVal yy() As Double, ByVal isDSGFreq As Boolean, ByVal jslenstr As String, ByVal runLocationTime As String)
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
        Dim str As String = "时间: " & runLocationTime & "  频率范围:[" & freqStart & "," & jieshu & "]"
        'str = "Mark " & TimeFreqPoint & "MHz," & TimeMarkPointValue & "dBm" & "   " & str
        If isDSGFreq Then
            str = str & "  <DSG频谱压缩 " & jslenstr & ">"
        Else
            str = str & "  <" & jslenstr & ">"
        End If
        Label4.Text = str
    End Sub
    Private Sub ShowSigNal(ByVal freqStart As Double, ByVal jieshu As Double, ByVal xx() As Double, ByVal yy() As Double, ByVal isDSGFreq As Boolean, ByVal jslenstr As String, ByVal runLocationTime As String, ponintIndex As Integer, sumCount As Integer, lng As Double, lat As Double, time As String)

        'Chart1.ChartAreas(0).AxisX.Minimum = freqStart
        'Chart1.ChartAreas(0).AxisX.Maximum = jieshu
        'Chart1.ChartAreas(0).AxisX.Interval = (jieshu - freqStart) / 5
        If Chart1.Series(0).Points.Count = 0 Then
            Chart1.Series(0).Points.Add(-120)
        End If
        Dim index As Integer = 0
        For i = 0 To xx.Length - 1
            If xx(i) = sigNalPointValue Then
                index = i
                Exit For
            End If
            If xx(i) > sigNalPointValue Then
                If i = 0 Then index = i : Exit For
                index = i
                Dim c1 As Double = xx(i) - sigNalPointValue
                Dim c2 As Double = sigNalPointValue - xx(i - 1)
                If c2 < c1 Then
                    index = i - 1
                End If
                Exit For
            End If
        Next
        ' Chart1.Series(4).Points.Add(yy(index))
        Dim series As Series = Chart1.Series(4)
        If series.Points.Count = sumCount Then
            ' MsgBox(ponintIndex & "," & Chart1.Series(4).Points(ponintIndex).YValues(0) & "," & yy(index))
            series.Points(ponintIndex).YValues(0) = yy(index)
            If sigNalMaxPointValue = 0 Then
                sigNalMaxPointValue = yy(index)
                sigNalMaxPointIndex = 0
                series.Points(ponintIndex).Color = Color.Red
                sigNalMaxPointInfo = "坐标:" & lng.ToString("0.00000") & "," & lat.ToString("0.00000") & ";场强:" & yy(index) & ";时间:" & time
                script("addpoint", New Object() {lng, lat, sigNalMaxPointInfo}, WebGis)
            Else
                If sigNalMaxPointValue < yy(index) Then
                    sigNalMaxPointValue = yy(index)
                    series.Points(ponintIndex).Color = Color.Red
                    series.Points(sigNalMaxPointIndex).Color = Color.Blue
                    sigNalMaxPointIndex = ponintIndex
                    script("deletePoint", New Object() {sigNalMaxPointInfo}, WebGis)
                    sigNalMaxPointInfo = "坐标:" & lng.ToString("0.00000") & "," & lat.ToString("0.00000") & ";场强:" & yy(index) & ";时间:" & time
                    script("addpoint", New Object() {lng, lat, sigNalMaxPointInfo}, WebGis)
                End If
            End If

            Chart1.Series(4) = series

            'MsgBox(Chart1.Series(4).Points(ponintIndex).YValues(0))
        End If

        'For i = 0 To 1
        '    Chart1.Series(4).Points.AddXY(runLocationTime, yy(index))
        'Next

        Dim str As String = "时间: " & runLocationTime & "  频率范围:[" & freqStart & "," & jieshu & "]"
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

    Private Sub WebGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        GetBusDeviceList()

    End Sub

    Private Sub 选择该条线路ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 选择该条线路ToolStripMenuItem.Click
        If LVDetail.Items.Count = 0 Then Return
        If LVDetail.SelectedIndices.Count = 0 Then Return
        Dim index As Integer = LVDetail.SelectedIndices(0)
        SelectLine(index)
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetBusDeviceList()
    End Sub

   

    Private Sub Label14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label14.Click
        GetBusHisFreqList()
    End Sub

    Private Sub Panel16_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel16.Click
        GetBusHisFreqList()
    End Sub

   
    Private Sub RDSignal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RDSignal.CheckedChanged
        PanelSignal.Visible = RDSignal.Checked
    End Sub

    Private Sub BusHisFreqGis_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Panel7.Height = Me.Height * 0.2
    End Sub

End Class
