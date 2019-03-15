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
Public Class HisFreqGis
    Dim lineId As String
    Dim lng As Double
    Dim lat As Double
    Dim busInfo As BusLine
    Dim msgId As String
    Dim BusFreqGisInfoList As List(Of BusFreqGisInfo)
    Structure BusFreqGisInfo
        Dim msgid As String
        Dim time As String
        Dim freqJsonLen As String

        Sub New(ByVal _msgid As String, ByVal _time As String, ByVal _freqJsonLen As String)
            msgid = _msgid
            time = _time
            freqJsonLen = _freqJsonLen
        End Sub
    End Structure
    Sub New(ByVal _lng As String, ByVal _lat As String, _msgid As String, ByVal _busInfoJson As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        msgId = _msgid
        lng = Val(_lng)
        lat = Val(_lat)
        busInfo = JsonConvert.DeserializeObject(_busInfoJson, GetType(BusLine))
        lineId = busInfo.lineId
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub HisFreqGis_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.TopMost = True
        Button2.Enabled = False
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.Text = "历史频谱地图"
        ini()
        iniDTP()
        iniChart1()
        If isLoadGis Then
            webGis.Navigate(gisurl)
        Else
            ' GetBusDeviceList()
        End If
        ' Me.Text = msgId
    End Sub
    Private Sub ini()
       LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("项目", 60)
        LVDetail.Columns.Add("内容", 220)
        Dim itm As New ListViewItem("线路名称")
        itm.SubItems.Add(busInfo.lineName)
        LVDetail.Items.Add(itm)

        itm = New ListViewItem("公交线路")
        itm.SubItems.Add(busInfo.busNo)
        LVDetail.Items.Add(itm)

        itm = New ListViewItem("车牌号")
        itm.SubItems.Add(busInfo.plateNumber)
        LVDetail.Items.Add(itm)

        itm = New ListViewItem("设备名称")
        itm.SubItems.Add(busInfo.deviceName)
        LVDetail.Items.Add(itm)

        itm = New ListViewItem("设备ID")
        itm.SubItems.Add(busInfo.deviceId)
        LVDetail.Items.Add(itm)

        itm = New ListViewItem("经度")
        itm.SubItems.Add(lng)
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("纬度")
        itm.SubItems.Add(lat)
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("地点")
        itm.SubItems.Add(busInfo.location)
        LVDetail.Items.Add(itm)
    End Sub
    Private Sub iniDTP()
        DTPStart.MaxDate = Now.AddDays(+1)
        DTPStart.CustomFormat = "yyyy-MM-dd HH:mm:ss"
        DTPStart.Format = DateTimePickerFormat.Custom
        DTPStart.ShowUpDown = True
        DTPStart.Value = Now.AddDays(-1).AddMinutes(10)
        DTPStart.Value = "2018-11-10 00:00:00"
        DTPStop.MaxDate = Now.AddDays(+2)
        DTPStop.CustomFormat = "yyyy-MM-dd HH:mm:ss"
        DTPStop.Format = DateTimePickerFormat.Custom
        DTPStop.ShowUpDown = True
        DTPStop.Value = Now.AddDays(+1).AddMinutes(10)
    End Sub
    Private Sub iniChart1()
        Chart1.Series.Clear()
        Chart1.ChartAreas(0).AxisY.Maximum = -20
        Chart1.ChartAreas(0).AxisY.Minimum = -120
        Chart1.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart1.ChartAreas(0).AxisY.Interval = 20
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
        Dim doc As HtmlDocument = web.Document
        Dim O(str.Count - 1) As Object
        For i = 0 To str.Length - 1
            O(i) = CObj(str(i))
        Next
        doc.InvokeScript(scriptName, O)
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
        b(6) = CObj("")
        b(7) = CObj("")
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

    Private Sub webGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles webGis.DocumentCompleted
        AddJumpPoint(lng, lat, busInfo.lineName, webGis)
        setGisCenter(lng, lat, webGis)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim th As New Thread(AddressOf TH_GetTimeList)
        th.Start()
    End Sub
    Private Sub TH_GetTimeList()
        Dim oldText As String = Button1.Text
        Me.Invoke(Sub()
                      Button1.Enabled = False
                      Button1.Text = "获取中……"
                      Application.DoEvents()
                  End Sub)
        Dim startTime As String = DTPStart.Value.ToString("yyyy-MM-dd HH:mm:ss")
        Dim endTime As String = DTPStop.Value.ToString("yyyy-MM-dd HH:mm:ss")
        Dim param As String = "func=GetHisFreqGisTimeList&lng={0}&lat={1}&lineId={2}&startTime={3}&endTime={4}&msgId=" & msgId
        param = String.Format(param, New String() {lng, lat, lineId, startTime, endTime})
        ' MsgBox(param)
        Dim np As normalResponse = GetServerNp(param)
        If np.result = False Then
            Me.Invoke(Sub()
                          Button1.Enabled = False
                          Button1.Text = "获取区间频谱时间"
                          Application.DoEvents()
                      End Sub)
            Me.Invoke(Sub() MsgBox(np.msg))
            Return
        End If
        Dim dt As DataTable = JsonConvert.DeserializeObject(np.data, GetType(DataTable))
        CBTimeList.Items.Clear()
        BusFreqGisInfoList = New List(Of BusFreqGisInfo)
        For Each row As DataRow In dt.Rows
            Dim msgid As String = row("msgid")
            Dim time As String = row("time")
            Dim freqJsonLen As String = row("freqJsonLen")
            Dim grid As String = row("grid")
            CBTimeList.Items.Add(time)
            BusFreqGisInfoList.Add(New BusFreqGisInfo(msgid, time, freqJsonLen))
        Next

        If CBTimeList.Items.Count > 0 Then
            CBTimeList.SelectedIndex = 0
            Button2.Enabled = True
            Button2.PerformClick()
        End If
        Me.Invoke(Sub() Button1.Enabled = True)
        Me.Invoke(Sub() Button1.Text = oldText)
    End Sub

    Structure BusFreqGisPPSJJsonInfo
        Dim freqStart As Double
        Dim freqEnd As Double
        Dim freqStep As Double
        Dim realPPSJ As json_PPSJ
    End Structure

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim th As New Thread(AddressOf TH_GetHisFreqGis)
        th.Start()
    End Sub
    Private Sub TH_GetHisFreqGis()
        Dim oldText As String = Button2.Text
        Me.Invoke(Sub()
                      Button2.Enabled = False
                      Button2.Text = "获取中……"
                      Application.DoEvents()
                  End Sub)
        Dim index As Integer = CBTimeList.SelectedIndex
        If index < 0 Then
            Me.Invoke(Sub()
                          Button2.Enabled = False
                          Button2.Text = "获取历史频谱"
                          Application.DoEvents()
                          MsgBox("请选择时间点")
                      End Sub)

            Return
        End If
        Dim msgid As String = BusFreqGisInfoList(index).msgid
        Dim freqStart As String = Val(TxtFreqStart.Text)
        Dim freqEnd As String = Val(TxtFreqEnd.Text)
        Dim param As String = "func=GetHisFreqGis&msgid={0}&freqStart={1}&freqEnd={2}"
        param = String.Format(param, New String() {msgid, freqStart, freqEnd})
        Dim np As normalResponse = GetServerNp(param)
        If np.result = False Then
            Me.Invoke(Sub() MsgBox(np.msg))
            Return
        End If
        Me.Invoke(Sub()
                      HandleBusFreqGisPPSJJsonInfo(np.data)
                      Button2.Enabled = True
                      Button2.Text = "获取历史频谱"
                  End Sub)
    End Sub
    Private Sub HandleBusFreqGisPPSJJsonInfo(ByVal json As String)
        Dim bfgp As BusFreqGisPPSJJsonInfo
        Try
            bfgp = JsonConvert.DeserializeObject(json, GetType(BusFreqGisPPSJJsonInfo))
        Catch ex As Exception
            Return
        End Try
        Dim chatFreqStart As Double = bfgp.freqStart
        Dim chatFreqEnd As Double = bfgp.freqEnd
        Dim p As json_PPSJ = bfgp.realPPSJ
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
        Dim dataCount As Integer = yy.Count
        Dim xx(dataCount - 1) As Double
        For i = 0 To dataCount - 1
            xx(i) = freqStart + i * freqStep
        Next
        xx(0) = chatFreqStart
        xx(dataCount - 1) = chatFreqEnd
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
        Chart1.ChartAreas(0).AxisX.Minimum = chatFreqStart
        Chart1.ChartAreas(0).AxisX.Maximum = chatFreqEnd
        Chart1.ChartAreas(0).AxisX.Interval = (chatFreqEnd - chatFreqStart) / 5
        Series.Points.Clear()
        For i = 0 To xx.Length - 1
            Series.Points.AddXY(xx(i), yy(i))
        Next
        If Chart1.Series.Count = 0 Then
            Chart1.Series.Add(Series)
        Else
            Chart1.Series(0) = Series
        End If
    End Sub

    Private Sub CBTimeList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CBTimeList.SelectedIndexChanged
        Dim th As New Thread(AddressOf TH_GetHisFreqGis)
        th.Start()
    End Sub
End Class