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
Public Class POAPanel
    Dim deviceList As List(Of deviceInfo)
    Dim poaIcoList As List(Of String)
    Dim cplist As List(Of ChartPointAndGisInfo)
    Dim msgThreadList As List(Of Thread)
    Dim DisPlayLock As New Object
    Dim orderFreqCenter As Double
    Dim orderFreqStart As Double
    Dim orderFreqStep As Double
    Dim orderFreqEnd As Double
    Dim dik As Dictionary(Of String, String)
    Dim dikLock As New Object
    Structure ChartPointAndGisInfo
        Dim device As deviceInfo
        Dim pointIndex As Integer
        Dim GisTxt As String
    End Structure
    Structure deviceInfo
        Dim deviceName As String
        Dim deviceId As String
        Dim httpMsgUrl As String
        Dim lng As String
        Dim lat As String
        Sub New(deviceName As String, deviceId As String, httpMsgUrl As String, lng As String, lat As String)
            Me.deviceName = deviceName
            Me.deviceId = deviceId
            Me.httpMsgUrl = httpMsgUrl
            Me.lng = lng
            Me.lat = lat
        End Sub
    End Structure
    Private Sub POAPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        WebGis.ObjectForScripting = Me
        ini()
        iniChart1()
        If isLoadGis Then
            WebGis.Navigate(gisurl)
        Else
            RefrushDeviceList()
        End If
    End Sub
    Private Sub ini()
        poaIcoList = New List(Of String)
        poaIcoList.Add("http://123.207.31.37:8082/bmapico/poa0.png")
        poaIcoList.Add("http://123.207.31.37:8082/bmapico/poa1.png")
        poaIcoList.Add("http://123.207.31.37:8082/bmapico/poa2.png")
        poaIcoList.Add("http://123.207.31.37:8082/bmapico/poa3.png")
        poaIcoList.Add("http://123.207.31.37:8082/bmapico/poa4.png")
        poaIcoList.Add("http://123.207.31.37:8082/bmapico/poa5.png")
    End Sub
    Private Sub iniChart1()
        Chart1.ChartAreas(0).AxisX.Interval = 1   '设置X轴坐标的间隔为1
        Chart1.ChartAreas(0).AxisX.IntervalOffset = 1   '设置X轴坐标偏移为1
        Chart1.ChartAreas(0).AxisX.LabelStyle.IsStaggered = True '   设置是否交错显示,比如数据多的时间分成两行来显示 

        Chart1.Series.Clear()
        'Chart1.ChartAreas(0).CursorX.IsUserEnabled = True
        'Chart1.ChartAreas(0).CursorY.IsUserEnabled = True
        Chart1.ChartAreas(0).AxisY.Maximum = -20
        Chart1.ChartAreas(0).AxisY.Minimum = -120

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
        Series.Color = Color.Blue
        Series.Name = ""
        Chart1.Series.Add(Series)   '4
    End Sub
    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        RefrushDeviceList()
    End Sub
    Private Sub RefrushDeviceList()
        Label2.Visible = True
        Form1.GetOnlineDevice()
        Label2.Visible = False
        CLB.Items.Clear()
        If IsNothing(alldevlist) Then Return
        If alldevlist.Count = 0 Then Return
        deviceList = New List(Of deviceInfo)
        For Each itm In alldevlist
            If itm.Kind.ToLower = "tss" Then
                Dim deviceName As String = itm.Name
                Dim deviceId As String = itm.DeviceID
                Dim httpMsgUrl As String = itm.HTTPMsgUrl
                httpMsgUrl = httpMsgUrl.Replace("123.207.31.37", ServerIP)
                httpMsgUrl = httpMsgUrl.Replace("+", ServerIP)
                Dim lng As String = itm.Lng
                Dim lat As String = itm.Lat
                deviceList.Add(New deviceInfo(deviceName, deviceId, httpMsgUrl, lng, lat))
                If deviceName.Contains("长安") Then
                    CLB.Items.Add(deviceName)
                End If
            End If
        Next
        For Each itm In alldevlist
            If itm.Kind.ToLower = "tss" Then
                Dim deviceName As String = itm.Name
                If deviceName.Contains("虎门") Then
                    CLB.Items.Add(deviceName)
                End If
            End If
        Next
        Button1.PerformClick()
    End Sub
    Private Sub DeletePoint(info As String)
        Dim jsName2 As String = "deletePoint"
        Dim obj2 As Object() = New Object() {info}
        script(jsName2, obj2, WebGis)
    End Sub
    Private Sub WebGis_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        setGisCenter3(113.739898, 22.804636, 13, WebGis)
        RefrushDeviceList()
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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        For i = 0 To CLB.Items.Count - 1
            CLB.SetItemChecked(i, True)
        Next
        ChangeWebGisDevices()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        For i = 0 To CLB.Items.Count - 1
            CLB.SetItemChecked(i, Not CLB.GetItemChecked(i))
        Next
        ChangeWebGisDevices()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        For i = 0 To CLB.Items.Count - 1
            Dim deviceName As String = CLB.Items(i)
            CLB.SetItemChecked(i, deviceName.Contains("长安"))
        Next
        ChangeWebGisDevices()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        For i = 0 To CLB.Items.Count - 1
            Dim deviceName As String = CLB.Items(i)
            CLB.SetItemChecked(i, deviceName.Contains("虎门"))
        Next
        ChangeWebGisDevices()
    End Sub
#End Region
    Private Function GetDeviceInfoByDeviceName(deviceName As String) As deviceInfo
        For Each itm In deviceList
            If itm.deviceName = deviceName Then
                Return itm
            End If
        Next
    End Function

    Private Sub ChangeWebGisDevices()
        CleanGis(WebGis)
        For i = 0 To CLB.Items.Count - 1
            Dim bool As Boolean = CLB.GetItemChecked(i)
            Dim d As deviceInfo = GetDeviceInfoByDeviceName(CLB.Items(i))
            Dim lng As String = d.lng
            Dim lat As String = d.lat
            Dim deviceName As String = d.deviceName
            If bool Then
                If lng <> "" And lat <> "" Then
                    AddNewIco(lng, lat, deviceName, poaIcoList(1), WebGis)
                End If
            End If
        Next
    End Sub
    Private Sub CLB_SelectedValueChanged(sender As Object, e As EventArgs) Handles CLB.SelectedValueChanged
        ChangeWebGisDevices()
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        For i = 0 To Chart1.Series.Count - 1
            Chart1.Series(i).Points.Clear()
        Next
        If Chart1.Series(0).Points.Count = 0 Then
            Chart1.Series(0).Points.Add(-120)
        End If
        CleanGis(WebGis)
        StopAll()
        msgThreadList = New List(Of Thread)
        Dim th As New Thread(AddressOf StartPOATest)
        th.Start()
    End Sub
    Public Sub StopAll()
        If IsNothing(msgThreadList) Then Return
        Try
            For Each itm In msgThreadList
                Try
                    itm.Abort()
                Catch ex As Exception

                End Try
            Next
        Catch ex As Exception

        End Try
    End Sub


    Private Sub StartPOATest()
        Dim freqCenter As Double = Val(TxtSignal.Text)
        Dim freqWidth As Double = Val(TxtSigWidth.Text)
        Dim width As Double = 100 * (freqWidth / 1000)
        Dim freqbegin As Double = freqCenter - width
        Dim freqend As Double = freqCenter + width
        Dim freqstep As Double = freqWidth
        Dim gcValue As Double = 8
        orderFreqCenter = freqCenter
        orderFreqStart = freqbegin
        orderFreqEnd = freqend
        orderFreqStep = freqstep / 1000
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = freqend
        p.freqStep = freqstep
        p.gcValue = gcValue
        p.task = "bscan"
        Label31.Visible = True
        Dim sumCount As Integer = CLB.CheckedItems.Count
        Dim readCount As Integer = 0
        cplist = New List(Of ChartPointAndGisInfo)
        SyncLock dikLock
            dik = New Dictionary(Of String, String)
        End SyncLock
        For i = 0 To CLB.Items.Count - 1
            Dim bool As Boolean = CLB.GetItemChecked(i)
            Dim d As deviceInfo = GetDeviceInfoByDeviceName(CLB.Items(i))
            Dim lng As String = d.lng
            Dim lat As String = d.lat
            Dim deviceName As String = d.deviceName
            Dim httpMsgUrl As String = d.httpMsgUrl
            If bool Then
                Dim DHDeviceStr As String = "2to1"
                p.DHDevice = DHDeviceStr
                p.deviceID = d.deviceId
                Dim orderMsg As String = JsonConvert.SerializeObject(p)
                'MsgBox(orderMsg)
                Dim th As New Thread(Sub()
                                         Dim str As String = "?func=tssOrder&datamsg=" & orderMsg & "&token=" & token
                                         Dim result As String = GetH(httpMsgUrl, str)
                                         Dim r As String = GetNorResult("result", result)
                                         Dim msg As String = GetNorResult("msg", result)
                                         Dim errmsg As String = GetNorResult("errmsg", result)
                                     End Sub)
                th.Start()
                readCount = readCount + 1
                Label12.Text = readCount & " / " & sumCount
                Dim cp As New ChartPointAndGisInfo
                cp.device = d
                Chart1.Series(4).Points.AddXY(d.deviceName, -120)
                cp.pointIndex = Chart1.Series(4).Points.Count - 1

                If lng <> "" And lat <> "" Then
                    AddNewIco(lng, lat, deviceName, poaIcoList(0), WebGis)
                    cp.GisTxt = deviceName
                End If
                dik.Add(deviceName, deviceName)
                cplist.Add(cp)
            End If
        Next
        Label31.Visible = False
        For Each cp In cplist
            Dim th As New Thread(Sub()
                                     GetDeviceFreqLoop(cp)
                                 End Sub)
            msgThreadList.Add(th)
            th.Start()
        Next
    End Sub
    Private Sub GetDeviceFreqLoop(cp As ChartPointAndGisInfo)
        Dim httpMsgUrl As String = cp.device.httpMsgUrl
        Dim gisTxt As String = cp.GisTxt
        Dim pointIndex As Integer = cp.pointIndex
        While True
            Try
                Dim result As String = GetHttpMsg(httpMsgUrl)
                If result = "" Then Continue While
                HandleHttpMsg(result, cp)
            Catch ex As Exception

            End Try
        End While
    End Sub
    Private Function GetHttpMsg(HttpMsgUrl As String) As String
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
    Private Sub HandleHttpMsg(ByVal HttpMsg As String, cp As ChartPointAndGisInfo)
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
            Dim Thread_PPSJ As New Thread(Sub()
                                              HandlePPSJList(PPSJList, cp)
                                          End Sub)
            Thread_PPSJ.Start()
        End If
    End Sub
    Private Sub HandlePPSJList(ByVal Plist As List(Of json_PPSJ), cp As ChartPointAndGisInfo)
        If IsNothing(Plist) Then Exit Sub
        If Plist.Count = 0 Then Exit Sub
        If IsNothing(DisPlayLock) Then DisPlayLock = New Object
        Dim count As Integer = Plist.Count
        Dim sleepCount As Double = (GetHttpMsgTimeSpan * 1000 - 100) / (count + 1)
        SyncLock DisPlayLock
            For i = 0 To Plist.Count - 1
                Try
                    Dim itm As json_PPSJ = Plist(i)
                    Me.Invoke(Sub() handlePinPuFenXi(itm, cp))
                Catch ex As Exception

                End Try
                'If i <> count - 1 Then
                '    Sleep(sleepCount)
                'End If
            Next
        End SyncLock
    End Sub
    Private Sub handlePinPuFenXi(ByVal p As json_PPSJ, cp As ChartPointAndGisInfo)
        Dim jstmp As String = JsonConvert.SerializeObject(p)
        Dim jsLen As Long = jstmp.Length
        Dim freqStart As Double = p.freqStart
        Dim freqStep As Double = p.freqStep
        Dim yy() As Double = p.value

        Dim isDSGFreq As Boolean = False
        If p.isDSGFreq Then
            isDSGFreq = True
            yy = DSGBase2PPSJValues(p.DSGFreqBase64)
        End If
        If IsNothing(yy) Then Return
        If yy.Count = 0 Then Return
        Dim freqEnd As Double = freqStart + yy.Length * freqStep
        If freqStart.ToString <> orderFreqStart.ToString Then
            Return
        End If
        If freqStep.ToString <> orderFreqStep.ToString Then
            Return
        End If
        If Abs(orderFreqEnd - freqEnd) > freqStep * 2 Then
            Return
        End If
        Console.WriteLine("-->" & cp.device.deviceName & "," & freqStart & "," & freqStep & "," & yy.Count)
        Dim signalStrength As Double = GetSignalStrength(orderFreqCenter, freqStart, freqStep, freqEnd, yy)
        Dim icoUrl As String = GetIcoBySignal(signalStrength)
        Dim lng As String = cp.device.lng
        Dim lat As String = cp.device.lat
        If lng <> "" And lat <> "" Then
            ResetGisTxtAndIco(cp, signalStrength, icoUrl)
        End If
        Me.Invoke(Sub()
                      Dim series As Series = Chart1.Series(4)
                      Dim pointIndex As Integer = cp.pointIndex
                      series.Points(pointIndex).YValues(0) = signalStrength
                      Dim maxValue As Double = signalStrength
                      Dim maxIndex As Integer = pointIndex
                      For i = 0 To series.Points.Count - 1
                          Dim yValue As Double = series.Points(i).YValues(0)
                          If yValue > maxValue Then
                              maxValue = yValue
                              maxIndex = i
                          End If
                      Next
                      For i = 0 To series.Points.Count - 1
                          If i = maxIndex Then
                              series.Points(i).Color = Color.Red
                          Else
                              series.Points(i).Color = Color.Blue
                          End If
                      Next
                      Chart1.Series(4) = series
                      For Each itm In cplist
                          If itm.pointIndex = maxIndex Then
                              ResetGisTxtAndIco(itm, maxValue, poaIcoList(5))
                              Exit For
                          End If
                      Next
                  End Sub)
    End Sub
    Private Function ResetGisTxtAndIco(cp As ChartPointAndGisInfo, signalStrength As String, icoUrl As String) As String
        Dim gisTxt As String = cp.GisTxt
        Dim deviceName As String = cp.device.deviceName
        SyncLock dikLock
            If IsNothing(dik) = False Then
                If dik.ContainsKey(deviceName) Then
                    gisTxt = dik(deviceName)
                End If
            End If
        End SyncLock

        Dim json As String = JsonConvert.SerializeObject(dik)
        '  MsgBox("删除前-->" & gisTxt & vbCrLf & json & vbCrLf & dik.Count)
        DeletePoint(gisTxt)
        '  MsgBox("删除后-->" & gisTxt)
        gisTxt = deviceName & " " & signalStrength
        Dim lng As String = cp.device.lng
        Dim lat As String = cp.device.lat
        If lng <> "" And lat <> "" Then
            AddNewIco(lng, lat, gisTxt, icoUrl, WebGis)
            SyncLock dikLock
                If IsNothing(dik) Then
                    dik = New Dictionary(Of String, String)
                End If
                dik(deviceName) = gisTxt
            End SyncLock
        End If

    End Function

    Private Function GetSignalStrength(freqCenter As Double, freqStart As Double, freqStep As Double, freqEnd As Double, yy() As Double) As Double
        If IsNothing(yy) Then Return -120
        If yy.Count = 0 Then Return -120
        Dim value As Double = yy(0)
        Dim minCha As Double = Abs(freqCenter - freqStart)
        Dim index As Integer = -1
        For i = freqStart To freqEnd Step freqStep
            index = index + 1
            Dim cha As Double = Abs(freqCenter - i)
            If cha < minCha Then
                minCha = cha
                value = yy(index)
            End If
        Next
        Return value
    End Function
    Private Function GetIcoBySignal(sigNal As Double) As String
        'If sigNal < -95 Then Return poaIcoList(1)
        'If sigNal < -90 Then Return poaIcoList(2)
        'If sigNal < -85 Then Return poaIcoList(3)
        If sigNal < -80 Then Return poaIcoList(1)
        Return poaIcoList(4)
    End Function

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        StopAll()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs)
        AddNewIco(113.739898, 22.804636, "长安咸西站", poaIcoList(5), WebGis)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs)
        DeletePoint("长安咸西站")
    End Sub
End Class
