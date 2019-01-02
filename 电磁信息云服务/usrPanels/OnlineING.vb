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
Public Class OnlineING
    Public selectDeviceID As String
    Public realDeviceID As String
    Dim th_ReciveHttpMsg As Thread
    Private DeviceAddress As String
    Dim HttpMsgUrl As String
    Dim itmList As List(Of ListViewItem)
    Dim isTongJi As Boolean = True
    Dim TongJiCiShu As Integer = 0
    Dim freqStartTimeStr As String = ""
    Dim showTabIndex As Integer = 1
    Dim TimeFreqPoint As Double = -88
    Private selectSignalBiaoPanel As List(Of SignalBiaoPanel)
    Dim LV26Lock As New Object
    Dim LV20Lock As New Object
    Structure MainBroderCastMsgStu
        Dim deviceID As String
        Dim msgTime As String
        Dim deviceMsg As String
        Dim deviceKind As String
        Sub New(ByVal _deviceID As String, ByVal _deviceMsg As String, ByVal _deviceKind As String)
            deviceID = _deviceID
            msgTime = Now.ToString("yyyy-MM-dd HH:mm:ss")
            deviceMsg = _deviceMsg
            deviceKind = _deviceKind
        End Sub
    End Structure
    Structure json_PPSJ
        Dim freqStart As Double
        Dim freqStep As Double
        Dim deviceID As String
        Dim dataCount As Integer
        Dim value() As Double
    End Structure
    Structure JSON_Msg
        Dim func As String
        Dim msg As String
        Sub New(ByVal _func As String, ByVal _Msg As String)
            func = _func
            msg = _Msg
        End Sub
    End Structure
    Structure json_Audio
        Dim freq As Double
        Dim audioBase64 As String
    End Structure
    Sub New(ByVal _DeviceID As String, ByVal isShowHead As Boolean, ByVal index As Integer)
        ' 此调用是设计器所必需的。
        InitializeComponent()
        selectDeviceID = _DeviceID
        If index >= 0 Then
            showTabIndex = index
        End If

        For Each itm In alldevlist
            If itm.Name = _DeviceID Then
                DeviceAddress = itm.Address
                Exit For
            End If
        Next

    End Sub
    Private Sub OnlineING_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        Control.CheckForIllegalCrossThreadCalls = False
        Me.DoubleBuffered = True
        'TabControl1.Region = New Region(New RectangleF(TabPage1.Left, TabPage1.Top, TabPage1.Width, TabPage1.Height))
        ini()
        Panel51.Dock = DockStyle.Fill
        Panel43.Dock = DockStyle.Fill
        Panel51.Visible = False
        Panel43.Visible = True
        TabControl1.SelectedIndex = showTabIndex
        TextBox4.Text = selectDeviceID
        th_ReciveHttpMsg = New Thread(AddressOf ReciveHttpMsg)
        th_ReciveHttpMsg.Start()
    End Sub
    Private Sub ini()
        TextBox1.Text = 88
        TextBox2.Text = 108
        TextBox3.Text = 25
        'iniOpacity()
        'iniCenterSetting()
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = selectDeviceID Then
                    realDeviceID = itm.DeviceID
                    TextBox4.Text = itm.Name
                    TextBox6.Text = itm.Lng
                    TextBox7.Text = itm.Lat
                    Exit For
                End If
            Next
        End SyncLock
        PictureBox9.Cursor = Cursors.Hand
        PictureBox1.Cursor = Cursors.Hand
        PictureBox3.Cursor = Cursors.Hand
        iniLV()
        iniChart()
        iniRadio()
        ComboBox10.Items.Add("2018广州马拉松")
        ComboBox10.Items.Add("上海民航通讯频率")
        ComboBox10.Items.Add("对讲机频率(开放)")
        ComboBox10.Items.Add("国际遇险求救频率")
        ComboBox10.Items.Add("营救器信标频率")
        ComboBox10.Items.Add("应急通信频率(ITU)")
        ComboBox10.Items.Add("铁路通信频率")
        ComboBox10.Items.Add("国际遇险求救频率")
        ComboBox10.SelectedIndex = 0
        ComboBox1.Items.Add("2018广州马拉松")
        ComboBox1.Items.Add("自定义频率")
        ComboBox1.Items.Add("上海民航通讯频率")
        ComboBox1.Items.Add("对讲机频率(开放)")
        ComboBox1.Items.Add("国际遇险求救频率")
        ComboBox1.Items.Add("营救器信标频率")
        ComboBox1.Items.Add("应急通信频率(ITU)")
        ComboBox1.Items.Add("铁路通信频率")
        ComboBox1.Items.Add("国际遇险求救频率")
        ComboBox1.SelectedIndex = 0
        ComboBox2.Items.Add("2018广州马拉松")
        ComboBox2.Items.Add("自定义频率")
        ComboBox2.Items.Add("上海民航通讯频率")
        ComboBox2.Items.Add("对讲机频率(开放)")
        ComboBox2.Items.Add("国际遇险求救频率")
        ComboBox2.Items.Add("营救器信标频率")
        ComboBox2.Items.Add("应急通信频率(ITU)")
        ComboBox2.Items.Add("铁路通信频率")
        ComboBox2.Items.Add("国际遇险求救频率")
        ComboBox2.SelectedIndex = 0
        'iniComoBox()
        RDSignal.Checked = True
    End Sub
    Private Sub iniLV()
        LV20.Clear()
        LV20.View = View.Details
        LV20.GridLines = False
        LV20.FullRowSelect = True
        LV20.Columns.Add("序号", 50)
        LV20.Columns.Add("时间", 130)
        LV20.Columns.Add("频率(MHz)", 70)
        LV20.Columns.Add("地点", 80)
        LV20.Columns.Add("信号电平", 100)
        LV20.Columns.Add("最小值")
        LV20.Columns.Add("最大值")
        LV20.Columns.Add("出现次数")
        LV20.Columns.Add("平均值")
        LV20.Columns.Add("统计时长")
        LV20.Columns.Add("占用度")
        LV20.Columns.Add("监测次数")
        LV20.Columns.Add("超标次数")
        LV20.Columns.Add("占用度直方图", 600)
        LV27.Clear()
        LV27.View = View.Details
        LV27.GridLines = False
        LV27.FullRowSelect = True
        LV27.Columns.Add("序号", 50)
        LV27.Columns.Add("时间", 150)
        LV27.Columns.Add("频率(MHz)", 70)
        LV27.Columns.Add("地点", 100)
        LV27.Columns.Add("信号电平", 100)
        LV27.Columns.Add("最小值")
        LV27.Columns.Add("最大值")
        LV27.Columns.Add("出现次数")
        LV27.Columns.Add("平均值")
        LV27.Columns.Add("统计时长")
        LV27.Columns.Add("占用度")
        LV27.Columns.Add("监测次数")
        LV27.Columns.Add("超标次数")

        LV22.View = View.Details
        LV22.GridLines = False
        LV22.FullRowSelect = True
        LV22.Columns.Add("序号", 50)
        LV22.Columns.Add("时间", 150)
        LV22.Columns.Add("频率(MHz)", 100)
        LV22.Columns.Add("地点", 100)
        LV22.Columns.Add("信号电平", 100)
        LV22.Columns.Add("最小值")
        LV22.Columns.Add("最大值")

        LV26.View = View.Details
        LV26.GridLines = False
        LV26.FullRowSelect = True
        LV26.Columns.Add("信号频率(MHz)", 100)
        LV26.Columns.Add("实时电平(dBm)", 100)
        LV26.Columns.Add("属性识别", 70)
        LV26.Columns.Add("状态评估", 70)
        LV26.Columns.Add("可用评估", 70)
        LV26.Columns.Add("占用度", 60)
        LV26.Columns.Add("起始时间", 100)
        LV26.Columns.Add("更新时间", 100)
        LV26.Columns.Add("监测时长", 100)
        LV26.Columns.Add("最大电平(dBm)", 80)
        LV26.Columns.Add("平均电平(dBm)", 80)
        LV26.Columns.Add("最小电平(dBm)", 80)
        LV26.Columns.Add("监测次数", 80)
        LV26.Columns.Add("超标次数", 80)

        LV21.Clear()
        LV21.View = View.Details
        LV21.GridLines = False
        LV21.FullRowSelect = True
        LV21.Columns.Add("信号频率(MHz)", 100)
        LV21.Columns.Add("实时电平(dBm)", 100)
        LV21.Columns.Add("属性识别", 70)
        LV21.Columns.Add("状态评估", 70)
        LV21.Columns.Add("可用评估", 70)
        LV21.Columns.Add("占用度", 60)
        LV21.Columns.Add("起始时间", 100)
        LV21.Columns.Add("更新时间", 100)
        LV21.Columns.Add("监测时长", 100)
        LV21.Columns.Add("最大电平(dBm)", 80)
        LV21.Columns.Add("平均电平(dBm)", 80)
        LV21.Columns.Add("最小电平(dBm)", 80)
        LV21.Columns.Add("监测次数", 80)
        LV21.Columns.Add("超标次数", 80)
    End Sub
    Private Sub iniChart()
        iniChart1()
        iniChart2()
        iniChart3()
        inichart4()
        iniChart5()
        iniChart6()
        inichart7()
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
        Chart1.Series.Add(Series) '2
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
        Chart1.Series.Add(Series) '3

        Series = New Series("MoudleFreq")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Gray
        Series.Name = "MoudleFreq"
        Chart1.Series.Add(Series) '1
    End Sub
    Private Sub iniChart2()
        Chart2.Series.Clear()
        Chart2.ChartAreas(0).AxisY.Maximum = -20
        Chart2.ChartAreas(0).AxisY.Minimum = -120
        Chart2.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart2.ChartAreas(0).AxisY.Interval = 20
        Dim Series As New Series
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        Series.Name = ""
        Chart2.Series.Add(Series)
        Series = New Series("illegalsignal")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        '  Series.BorderWidth = "0.5"
        Series.ChartType = SeriesChartType.Column
        Series.IsVisibleInLegend = False
        Series.Color = Color.Red
        Series.Name = ""
        Chart2.Series.Add(Series)
    End Sub
    Private Sub iniChart3()
        Chart3.Series.Clear()
        Chart3.ChartAreas(0).AxisY.Maximum = 40
        Chart3.ChartAreas(0).AxisY.Minimum = -120
        Chart3.ChartAreas(0).AxisY.Interval = 20
        Chart3.ChartAreas(0).AxisY.IntervalOffset = 20
        ' Chart3.ChartAreas(0).BorderColor = Color.Yellow
        'Chart3.ChartAreas(0).BackColor = Color.Black
        Chart3.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        'Chart3.ChartAreas(0).AxisY.IsReversed = True
        'Chart3.ChartAreas(0).AxisX.Enabled = AxisEnabled.False
        'Chart3.ChartAreas(0).AxisX2.Enabled = AxisEnabled.True
        Dim Series As New Series '频谱   0
        Series.Label = "频谱数据"
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        'Series.ToolTip = "频率：#VALX\\n场强：#VAL"
        'Series.LabelToolTip = "频率：#VALX\\n场强：#VAL"
        Chart3.Series.Add(Series)
        Dim ser As New Series("illegalsignal") '信号   1
        ser.ChartType = SeriesChartType.Column
        ser.Color = Color.Red
        ser("PointWidth") = 0.25
        ser.IsVisibleInLegend = False
        'ser.ToolTip = "频率：#VALX\\n场强：#VAL"
        'ser.LabelToolTip = "频率：#VALX\\n场强：#VAL"
        Chart3.Series.Add(ser)
        Dim Series2 As New Series '最大值   2
        Series2.Label = "最大值"
        Series2.XValueType = ChartValueType.Auto
        Series2.ChartType = SeriesChartType.FastLine
        Series2.IsVisibleInLegend = False
        Chart3.Series.Add(Series2)
        Dim Series3 As New Series '状态跟踪   3
        Series3.Label = "状态跟踪"
        Series3.XValueType = ChartValueType.Auto
        Series3.ChartType = SeriesChartType.StepLine
        Series3.IsVisibleInLegend = False
        Chart3.Series.Add(Series3)
        Dim series4 As New Series '状态跟踪   3
        series4.Label = "状态跟踪"
        series4.XValueType = ChartValueType.Auto
        series4.ChartType = SeriesChartType.StepLine
        series4.IsVisibleInLegend = False
        Chart3.Series.Add(series4)
    End Sub
    Private Sub inichart4()
        Chart4.Series.Clear()
        Chart4.ChartAreas(0).AxisY.Maximum = 255
        Chart4.ChartAreas(0).AxisY.Minimum = 0
        Chart4.ChartAreas(0).AxisY.Interval = 51
        Chart4.ChartAreas(0).AxisY.IntervalOffset = 51

        Chart4.ChartAreas(0).AxisX.Maximum = 800
        Chart4.ChartAreas(0).AxisX.Minimum = 0
        Chart4.ChartAreas(0).AxisX.Interval = 100
        Chart4.ChartAreas(0).AxisX.IntervalOffset = 100
        Chart4.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        'chart6.ChartAreas(0).AxisY.IsReversed = True
        'chart6.ChartAreas(0).AxisX.Enabled = AxisEnabled.False
        'chart6.ChartAreas(0).AxisX2.Enabled = AxisEnabled.True
        Chart4.Series.Add("频率")
        Chart4.Series(0).IsVisibleInLegend = False
        Chart4.Series(0).ChartType = DataVisualization.Charting.SeriesChartType.FastLine
        Chart4.Series(0).Points.AddXY(88, 0)
    End Sub
    Private Sub iniChart5()
        Chart5.Series.Clear()
        Chart5.ChartAreas(0).AxisY.Maximum = -20
        Chart5.ChartAreas(0).AxisY.Minimum = -120
        Chart5.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart5.ChartAreas(0).AxisY.Interval = 20
        Dim Series As New Series
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        Series.Name = ""
        Chart5.Series.Add(Series)
        Series = New Series("illegalsignal")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.Column
        Series.IsVisibleInLegend = False
        Series.Color = Color.Red
        Series.Name = ""
        Chart5.Series.Add(Series)
    End Sub
    Private Sub iniChart6()
        Chart6.Series.Clear()
        Chart6.ChartAreas(0).AxisY.Maximum = -20
        Chart6.ChartAreas(0).AxisY.Minimum = -120
        Chart6.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart6.ChartAreas(0).AxisY.Interval = 20
        Dim Series2 As New Series("")
        Series2.Color = Color.Blue
        Series2.IsValueShownAsLabel = True
        Series2.IsVisibleInLegend = False
        Series2.ToolTip = "#VAL"
        Series2.Color = Color.Blue
        Series2.ChartType = SeriesChartType.FastLine

        Chart6.Series.Add(Series2)
        Dim Series As New Series("1")
        Series.Color = Color.Blue
        Series.IsValueShownAsLabel = True
        Series.IsVisibleInLegend = False
        Series.ToolTip = "#VAL"
        Series.Color = Color.Blue
        Series.ChartType = SeriesChartType.FastLine
        For i = 0 To 1999
            Series.Points.Add(-120)
        Next
        Chart6.Series.Add(Series)
    End Sub
    Private Sub inichart7()
        Chart7.Series.Clear()
        Chart7.ChartAreas(0).AxisY.Maximum = -20
        Chart7.ChartAreas(0).AxisY.Minimum = -120
        Chart7.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart7.ChartAreas(0).AxisY.Interval = 20
        Dim Series As New Series
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        Series.Name = ""
        Chart7.Series.Add(Series) '0  频谱层
        Series = New Series("illegalsignal")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.Column
        Series.IsVisibleInLegend = False
        Series.Color = Color.Red
        Series.Name = ""
        Chart7.Series.Add(Series) '1  信号层
        Series = New Series("zft")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.Column
        Series.IsVisibleInLegend = False
        Series.LabelToolTip = "#VAL" & " %"
        Series.ToolTip = "#VAL" & " %"
        Series.Label = "#VAL" & " %"
        Series.Color = Color.Blue
        Series.Name = ""
        Chart7.Series.Add(Series) '2  直方图层

    End Sub

    Private Sub iniRadio()
        Dim w As Integer = Panel34.Width
        Dim h As Integer = Panel34.Height
        Dim bitmap As New Bitmap(w, h)
        Dim g As Graphics = Graphics.FromImage(bitmap)
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.CompositingQuality = CompositingQuality.AssumeLinear
        For i = 0 To w Step 5
            Dim p1 As New Point(i, 30)
            Dim p2 As New Point(i, h - 1)
            'If i Mod CInt(w / 10) = 0 Then
            '    p1 = New Point(i, 30)
            'End If
            If i = w Then
                Dim ti As Integer = w - 1
                p1 = New Point(ti, 30)
                p2 = New Point(ti, h - 1)
                g.DrawLine(New Pen(Brushes.DimGray, 1), p1, p2)
                Exit For
            End If
            g.DrawLine(New Pen(Brushes.DimGray, 1), p1, p2)
        Next
        'For i = 1 To 10
        '    Dim x As Integer = (w / 10) * i
        '    Dim p1 As New Point(x, 30)
        '    Dim p2 As New Point(x, h - 1)
        '    g.DrawLine(New Pen(Brushes.DimGray, 1), p1, p2)
        'Next
        g.Save()
        Panel34.BackgroundImage = bitmap
    End Sub
    Private Sub ReciveHttpMsg()
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & realDeviceID & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & realDeviceID & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Console.WriteLine("HttpMsgUrl=" & HttpMsgUrl)
        While True
            Dim result As String = GetHttpMsg()
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
            Return str
        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try
    End Function
    Dim DrawThread2 As Thread
    Dim Thread_PPSJ As Thread
    Private Sub HandleHttpMsg(ByVal HttpMsg As String)
        Console.WriteLine("收到新消息ING  " & Now.ToString("HH:mm:ss"))
        Dim PPSJList As New List(Of json_PPSJ)
        Dim AudioList As New List(Of json_Audio)
        Dim TZBQList As New List(Of String)
        Try
            Dim list As List(Of String) = JsonConvert.DeserializeObject(HttpMsg, GetType(List(Of String)))
            Console.WriteLine(list.Count)
            For Each sh In list
                Try
                    Dim mb As MainBroderCastMsgStu = JsonConvert.DeserializeObject(sh, GetType(MainBroderCastMsgStu))
                    Dim deviceID As String = mb.deviceID
                    Dim deviceMsg As String = mb.deviceMsg
                    Dim msgTime As String = mb.msgTime
                    Dim deviceKind As String = mb.deviceKind
                    If deviceKind = "TSS" Then
                        Dim JObj As Object = JObject.Parse(deviceMsg)
                        Dim func As String = JObj("func").ToString
                        If func = "bscan" Then
                            Dim msg As String = JObj("msg").ToString
                            Dim ppsj As json_PPSJ = JsonConvert.DeserializeObject(msg, GetType(json_PPSJ))
                            PPSJList.Add(ppsj)
                        End If
                        If func = "ifscan_wav" Then
                            Dim msg As String = JObj("msg").ToString
                            Dim audio As json_Audio = JsonConvert.DeserializeObject(msg, GetType(json_Audio))
                            AudioList.Add(audio)
                        End If
                    Else

                    End If
                    If deviceKind = "TZBQ" Then
                        TZBQList.Add(deviceMsg)
                    End If
                Catch ex As Exception

                End Try
            Next
            'Dim p As JArray = JArray.Parse(HttpMsg)
            'Console.WriteLine(p.Count)
            'For Each itm As JValue In p
            '    Dim jMsg As String = itm.Value
            '    Dim JObj As Object = JObject.Parse(jMsg)
            '    Dim func As String = JObj("func").ToString
            '    If func = "bscan" Then
            '        Dim msg As String = JObj("msg").ToString
            '        Dim ppsj As json_PPSJ = JsonConvert.DeserializeObject(msg, GetType(json_PPSJ))
            '        PPSJList.Add(ppsj)
            '    End If
            '    If func = "ifscan_wav" Then
            '        Dim msg As String = JObj("msg").ToString
            '        Dim audio As json_Audio = JsonConvert.DeserializeObject(msg, GetType(json_Audio))
            '        AudioList.Add(audio)
            '    End If
            'Next
        Catch ex As Exception
            Exit Sub
        End Try
        If IsNothing(Thread_PPSJ) = False Then
            Try
                Thread_PPSJ.Abort()
            Catch ex As Exception

            End Try
        End If
        Thread_PPSJ = New Thread(AddressOf HandlePPSJList)
        Thread_PPSJ.Start(PPSJList)
        Dim th1 As New Thread(AddressOf HandleAudioList)
        th1.Start(AudioList)
        If IsNothing(DrawThread2) = False Then
            Try
                DrawThread2.Abort()
            Catch ex As Exception

            End Try
        End If
        DrawThread2 = New Thread(AddressOf handleTZBQList)
        DrawThread2.Start(TZBQList)

    End Sub
    Dim AudioPlayLock As Object
    Private Sub HandleAudioList(ByVal list As List(Of json_Audio))
        If IsNothing(list) Then Exit Sub
        If list.Count = 0 Then Exit Sub
        If IsNothing(AudioPlayLock) Then AudioPlayLock = New Object
        SyncLock AudioPlayLock
            For Each itm In list
                Dim freq As String = itm.freq
                Label19.Text = freq & " MHz"
                'Label19.Left = PictureBox10.Width / 2 - Label19.Width / 2
                Dim base64 As String = itm.audioBase64
                Try
                    Dim buffer() As Byte = Convert.FromBase64String(base64)
                    If IsNothing(buffer) = False Then
                        If buffer.Length > 44 Then
                            Dim realBy(buffer.Length - 45) As Byte
                            Array.Copy(buffer, 44, realBy, 0, realBy.Length)
                            Me.Invoke(Sub() gxChart(realBy))
                            play(buffer)
                        End If
                    End If
                Catch ex As Exception

                End Try
            Next
        End SyncLock
    End Sub
    Private Sub gxChart(ByVal by() As Byte)
        Dim Series As New Series
        Series.Name = "音频"
        Series.IsVisibleInLegend = False
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        For i = 0 To by.Count - 1
            Series.Points.AddXY(i + 1, by(i))
        Next
        Chart4.Series(0).Points.Clear()
        Chart4.Series(0) = Series
    End Sub
    Dim wavlistObject As Object
    Dim wavlist As List(Of Byte())
    Private Sub play(ByVal buf() As Byte)
        Try
            '  MsgBox(Now.ToString)
            Dim ms As MemoryStream = New MemoryStream(buf)
            Dim sp As SoundPlayer = New SoundPlayer(ms)
            sp.PlaySync()
            ' MsgBox(Now.ToString)
            'If IsNothing(wavlistObject) Then
            '    wavlistObject = New Object
            'End If
            'SyncLock wavlistObject
            '    If IsNothing(wavlist) Then
            '        wavlist = New List(Of Byte())
            '    End If
            '    If IsNothing(buf) Then Exit Sub
            '    wavlist.Add(buf)
            'End SyncLock
        Catch ex As Exception
            'MsgBox(ex.ToString)
        End Try
    End Sub
    Dim DisPlayLock2 As Object
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
                    If itm.dataCount < 5000 Then
                        Me.Invoke(Sub() handlePinPuFenXi(itm))
                    End If

                Catch ex As Exception

                End Try
                If i <> count - 1 Then
                    Sleep(sleepCount)
                End If
            Next
        End SyncLock
    End Sub
    Private Sub handleTZBQList(ByVal bqlist As List(Of String))
        If IsNothing(DisPlayLock2) Then DisPlayLock2 = New Object
        Dim count As Integer = bqlist.Count
        Dim maxCount As Integer = bqlist.Count
        If count > maxCount Then count = maxCount
        Dim sleepCount As Double = (GetHttpMsgTimeSpan * 1000 - 100) / (count + 1)
        SyncLock DisPlayLock2
            For i = 0 To count - 1
                Try
                    If i < maxCount Then
                        Dim bq As String = bqlist(i)
                        Me.Invoke(Sub() handleBQ(bq))
                    End If

                Catch ex As Exception

                End Try
                If i <> count - 1 Then
                    Sleep(sleepCount)
                End If
            Next
        End SyncLock
    End Sub
    Private Function getIDbyBQ(ByVal BQ As String)
        Try

            Dim id As String = BQ.Split(":")(1).Split(",")(1)
            Return id
        Catch ex As Exception

        End Try

    End Function
    Private Function getFuncByBQ(ByVal BQ As String)
        Try
            Dim func As String = BQ.Split(":")(1).Split(",")(0)
            If func = "ECHO" Then
                Dim huiying As String = BQ.Split(":")(1).Split(",")(2)
                func = func & "_" & huiying
            End If
            Return func
        Catch ex As Exception
            '‘MsgBox(ex.ToString)
        End Try
    End Function
    Private Function BCD2Int(ByVal BCD As String) As Integer
        Try
            Dim t As Integer = Convert.ToInt32(BCD, 16)
            Dim s As Integer = 0
            If t <= 127 Then
                s = -t
            Else
                s = t - 256
            End If
            Return s
        Catch ex As Exception
            Return -120
        End Try
    End Function
    Private Sub handleBQ(ByVal bq As String)
        If InStr(bq, "<TZBQ:") Then  Else Exit Sub

        Dim a As Integer = InStr(bq, "<")
        Dim b As Integer = InStr(bq, Chr(13))
        bq = Mid(bq, a, b - 1 + 1)
        Dim func As String = getFuncByBQ(bq)
        Dim id As String = getIDbyBQ(bq)
        'If id <> selectDeviceID Then Exit Sub
        If InStr(func, "ECHO_") Then
            Dim k As String = func.Split("_")(1)
            If k = "TIME" Then
                Me.Invoke(Sub() MsgBox("时间同步成功！"))
            End If
            If k = "JCPD" Then
                Console.WriteLine(bq)
                'Me.Invoke(Sub() MsgBox(BQ))
                'Me.Invoke(Sub() MsgBox("成功下发频点检测命令！"))
            End If
            If k = "JCMB" Then
                Me.Invoke(Sub() MsgBox("成功下发模板命令"))
            End If
            If k = "SGPS" Then
                Me.Invoke(Sub() MsgBox("GPS设置成功"))
            End If
            If k = "SZID" Then
                Me.Invoke(Sub() MsgBox("ID设置成功"))
            End If
        End If
        If func = "CXNEW" Then
            Dim str As String = "<TZBQ:SBZT,0>"
            ' SendMsgToDev(id, "<TZBQ:SBZT,0>")
        End If
        If func = "JCMB" Then
            'Label16.Text = "模板建立完成!"
            'Me.Invoke(Sub() MsgBox("模板建立完成！"))
            Dim str As String = bq.Substring(InStr(bq, "<"), InStr(bq, ">") - InStr(bq, "<") - 1)
            Dim st() As String = str.Split(",")
            Dim numofSinle As Integer = st(2)
            Dim resultMsg As New StringBuilder
            For i = 3 To st.Length - 1 Step 2
                Dim pl As String = st(i)
                Dim cq As String = st(i + 1)
                resultMsg.AppendLine(pl & "MHz," & cq & "dBm")
            Next
            MsgBox(resultMsg.ToString, MsgBoxStyle.OkOnly, "设备模板")
        End If
        If func = "JCPD" Then
            'MsgBox(BQ)
            '<TZBQ:JCPD,3,300,3,-79,-79,-79>
        End If
        If func = "PPSJ" Then
            Dim st() As String = bq.Split(",")
            Dim startPQ As Double = st(2)
            Dim endPD As Double = st(3)
            Dim bujin As Double = st(4)
            Dim leixing As String = st(5)
            If leixing = "R" Then leixing = "实时"
            If leixing = "M" Then leixing = "模板"
            If leixing = "W" Then leixing = "报警"
            Dim geshu As Integer = st(6).Split(">")(0)
            Dim x(geshu - 1) As Double
            Dim y(geshu - 1) As Double
            For i = 0 To geshu - 1
                x(i) = startPQ + i * bujin
            Next
            Dim BCDstr As String = bq.Split("]")(1)
            For i = 0 To geshu - 1
                Dim bcd As String = BCDstr.Substring(i * 2, 2)
                y(i) = BCD2Int(bcd)
            Next
            Dim js As json_PPSJ
            js.dataCount = x.Count
            js.deviceID = selectDeviceID
            js.freqStart = x(0)
            js.freqStep = bujin
            js.value = y
            '  Me.Invoke(Sub() HandlePPSJList(js))
        End If
        If func = "SSSJ" Then
            Me.Invoke(Sub() HandleSSSJ(bq))
        End If
    End Sub
    Structure zydStu
        Dim pl As Double
        Dim zyd As Double
        Sub New(ByVal _pl As Double, ByVal _zyd As Double)
            pl = _pl
            zyd = _zyd
        End Sub
    End Structure
    Dim sssjStartTime As String = ""
    Private Sub HandleSSSJ(ByVal bq As String)

        If sssjStartTime = "" Then
            sssjStartTime = Now.ToString("yyyy-MM-dd HH:mm:ss")
        End If
        Dim strtmp As String = Now.ToString("HH:mm:ss")
        Dim str As String = bq.Substring(InStr(bq, "<"), InStr(bq, ">") - InStr(bq, "<") - 1)
        Dim st() As String = str.Split(",")
        Dim numOfValue As Integer = st(2)
        Dim SigNalList As New List(Of Double)
        Dim CqList As New List(Of Double)
        If st.Length < 2 + numOfValue + 2 Then Return
        If st.Length > numOfValue * 2 Then
            For i = 3 To 2 + numOfValue
                Dim pd As Double = st(i + numOfValue)
                Dim cq As Double = st(i)
                SigNalList.Add(pd)
                CqList.Add(cq)
            Next
        End If
        If IsNothing(selectSignalBiaoPanel) Then
            Panel64.Controls.Clear()
            selectSignalBiaoPanel = New List(Of SignalBiaoPanel)
            Dim tmpList As New List(Of SignalBiaoPanel)
            For j = SigNalList.Count - 1 To 0 Step -1
                Dim itm As Double = SigNalList(j)
                Dim p As New SignalBiaoPanel(itm)
                Panel64.Controls.Add(p)
                tmpList.Add(p)
            Next
            For j = tmpList.Count - 1 To 0 Step -1
                selectSignalBiaoPanel.Add(tmpList(j))
            Next
        Else
            If selectSignalBiaoPanel.Count <> SigNalList.Count Then
                Panel64.Controls.Clear()
                selectSignalBiaoPanel = New List(Of SignalBiaoPanel)
                Dim tmpList As New List(Of SignalBiaoPanel)
                For j = SigNalList.Count - 1 To 0 Step -1
                    Dim itm As Double = SigNalList(j)
                    Dim p As New SignalBiaoPanel(itm)
                    Panel64.Controls.Add(p)
                    tmpList.Add(p)
                Next
                For j = tmpList.Count - 1 To 0 Step -1
                    selectSignalBiaoPanel.Add(tmpList(j))
                Next
            End If
        End If
        Dim count As Integer = SigNalList.Count
        Me.Invoke(Sub()
                      Try
                          For i = 0 To count - 1
                              Dim pd As Double = SigNalList(i)
                              Dim cq As Double = CqList(i)
                              Dim p As SignalBiaoPanel = selectSignalBiaoPanel(i)
                              p.SetSignalValue(Now.ToString("HH:mm:ss"), cq)
                              SyncLock LV26Lock
                                  handleLv26(pd, cq, SigNalList)
                              End SyncLock
                              If RDSignal.Checked Then
                                  SyncLock LV20Lock
                                      handlelv21(pd, cq, SigNalList)
                                  End SyncLock
                              End If
                          Next

                      Catch ex As Exception

                      End Try
                  End Sub)
        If RDSignal.Checked Then
            Me.Invoke(Sub()
                          Try
                              Dim Series As New Series("zft")
                              Series.Label = ""
                              Series.XValueType = ChartValueType.String
                              Series.ChartType = SeriesChartType.Column
                              Series.IsVisibleInLegend = False
                              Series.LabelToolTip = "#VAL" & " %"
                              Series.ToolTip = "#VAL" & " %"
                              Series.Label = "#VAL" & " %"
                              Series.Color = Color.Blue
                              Series("PointWidth") = 0.2
                              Series.Name = ""
                              Dim list As New List(Of zydStu)
                              For Each itm As ListViewItem In LV26.Items
                                  Dim pl As Double = Val(itm.SubItems(0).Text)
                                  Dim plString As String = itm.SubItems(0).Text.ToString
                                  Dim zyd As Double = Val(itm.SubItems(5).Text.Replace("%", ""))
                                  'Series.Points.AddXY(plString, zyd)

                                  list.Add(New zydStu(pl, zyd))
                              Next
                              For i = 0 To list.Count - 1
                                  For j = i + 1 To list.Count - 1
                                      Dim aItm As zydStu = list(i)
                                      Dim bItm As zydStu = list(j)
                                      If aItm.pl > bItm.pl Then
                                          list(j) = aItm
                                          list(i) = bItm
                                      End If
                                  Next
                              Next
                              For Each itm In list
                                  Series.Points.AddXY(itm.pl.ToString("0.00"), itm.zyd)
                              Next
                              Chart7.Series(2) = Series
                          Catch ex As Exception
                              MsgBox(ex.ToString)
                          End Try
                      End Sub)
        End If
        My.Application.DoEvents()
    End Sub
    Private Sub handleLv26(ByVal pd As Double, ByVal cq As Double, ByVal SigNalList As List(Of Double))
        Try
            If IsNothing(SigNalList) Then Exit Sub
            If LV26.Items.Count <> SigNalList.Count Then
                LV26.Items.Clear()
                For Each it In SigNalList
                    Dim itm As New ListViewItem(it)
                    For i = 1 To LV26.Columns.Count - 1
                        itm.SubItems.Add("--")
                    Next
                    itm.SubItems(itm.SubItems.Count - 1).Text = 0
                    itm.SubItems(itm.SubItems.Count - 2).Text = 0
                    LV26.Items.Add(itm)
                Next
            End If
            For Each itm As ListViewItem In LV26.Items
                If Val(itm.Text) = pd Then
                    itm.SubItems(1).Text = cq.ToString(".0")
                    If itm.SubItems(9).Text = "--" Then itm.SubItems(9).Text = 0
                    Dim k As Double = Val(itm.SubItems(9).Text)
                    If k = 0 Then k = -110
                    If k < cq Then
                        itm.SubItems(9).Text = cq.ToString(".0")
                    End If
                    If itm.SubItems(11).Text = "--" Then itm.SubItems(11).Text = 0
                    Dim min As Double = Val(itm.SubItems(11).Text)
                    If min = 0 Then min = -1
                    If min > cq Then
                        itm.SubItems(11).Text = cq.ToString(".0")
                    End If

                    If itm.SubItems(10).Text = "--" Then itm.SubItems(10).Text = 0
                    Dim avg As Double = 0
                    If itm.SubItems(10).Text = 0 Then
                        avg = cq.ToString(".0")
                    Else
                        If avg = 0 Then avg = cq
                        avg = ((avg + cq) / 2)
                    End If
                    itm.SubItems(10).Text = avg.ToString(".0")
                    Dim statu As String = "正常"
                    itm.ForeColor = Color.Black
                    If cq >= -60 Then
                        statu = "超标"
                        itm.ForeColor = Color.Red
                    End If
                    itm.SubItems(2).Text = "不明信号"
                    itm.SubItems(3).Text = statu
                    itm.SubItems(4).Text = "可用"
                    itm.SubItems(5).Text = "10%"
                    itm.SubItems(6).Text = sssjStartTime
                    itm.SubItems(7).Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
                    itm.SubItems(12).Text = Val(itm.SubItems(12).Text) + 1
                    If cq >= -86 Then
                        itm.SubItems(13).Text = Val(itm.SubItems(13).Text) + 1
                    End If
                    itm.SubItems(5).Text = ((Val(itm.SubItems(13).Text) / Val(itm.SubItems(12).Text)) * 100).ToString("0.00") & "%"
                    Dim timespan As TimeSpan = Now.Subtract(sssjStartTime)
                    itm.SubItems(8).Text = timespan.Hours.ToString("00") & ":" & timespan.Minutes.ToString("00") & ":" & timespan.Seconds.ToString("00")
                End If
            Next
        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try

    End Sub
    Private Sub handlelv21(ByVal pd As Double, ByVal cq As Double, ByVal SigNalList As List(Of Double))
        Try
            If IsNothing(SigNalList) Then Exit Sub
            If LV21.Items.Count <> SigNalList.Count Then
                LV21.Items.Clear()
                For Each it In SigNalList
                    Dim itm As New ListViewItem(it)
                    For i = 1 To LV21.Columns.Count - 1
                        itm.SubItems.Add("--")
                    Next
                    itm.SubItems(itm.SubItems.Count - 1).Text = 0
                    itm.SubItems(itm.SubItems.Count - 2).Text = 0
                    LV21.Items.Add(itm)
                Next
            End If
            For Each itm As ListViewItem In LV21.Items
                If Val(itm.Text) = pd Then
                    itm.SubItems(1).Text = cq.ToString(".0")
                    If itm.SubItems(9).Text = "--" Then itm.SubItems(9).Text = 0
                    Dim k As Double = Val(itm.SubItems(9).Text)
                    If k = 0 Then k = -110
                    If k < cq Then
                        itm.SubItems(9).Text = cq.ToString(".0")
                    End If
                    If itm.SubItems(11).Text = "--" Then itm.SubItems(11).Text = 0
                    Dim min As Double = Val(itm.SubItems(11).Text)
                    If min = 0 Then min = -1
                    If min > cq Then
                        itm.SubItems(11).Text = cq.ToString(".0")
                    End If

                    If itm.SubItems(10).Text = "--" Then itm.SubItems(10).Text = 0
                    Dim avg As Double = 0
                    If itm.SubItems(10).Text = 0 Then
                        avg = cq.ToString(".0")
                    Else
                        If avg = 0 Then avg = cq
                        avg = ((avg + cq) / 2)
                    End If
                    itm.SubItems(10).Text = avg.ToString(".0")
                    Dim statu As String = "正常"
                    itm.ForeColor = Color.Black
                    If cq >= -60 Then
                        statu = "超标"
                        itm.ForeColor = Color.Red
                    End If
                    itm.SubItems(2).Text = "不明信号"
                    itm.SubItems(3).Text = statu
                    itm.SubItems(4).Text = "可用"
                    itm.SubItems(5).Text = "10%"
                    itm.SubItems(6).Text = sssjStartTime
                    itm.SubItems(7).Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
                    itm.SubItems(12).Text = Val(itm.SubItems(12).Text) + 1
                    If cq >= -86 Then
                        itm.SubItems(13).Text = Val(itm.SubItems(13).Text) + 1
                    End If
                    itm.SubItems(5).Text = ((Val(itm.SubItems(13).Text) / Val(itm.SubItems(12).Text)) * 100).ToString("0.00") & "%"
                    Dim timespan As TimeSpan = Now.Subtract(sssjStartTime)
                    itm.SubItems(8).Text = timespan.Hours.ToString("00") & ":" & timespan.Minutes.ToString("00") & ":" & timespan.Seconds.ToString("00")
                End If
            Next
        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try

    End Sub
    Private Sub handlePinPuFenXi(ByVal p As json_PPSJ)
        'If Label25.Text = "00:00:00" Then
        '    Label25.Text = Now.ToString("HH:mm:ss")
        'End If
        'Label26.Text = Now.ToString("HH:mm:ss")
        Dim freqStart As Double = p.freqStart
        Dim freqStep As Double = p.freqStep
        Dim yy() As Double = p.value

        If IsNothing(yy) Then

            Exit Sub
        End If
        Dim dataCount As Integer = yy.Count
        Dim deviceID As String = p.deviceID
        Dim xx(dataCount - 1) As Double
        For i = 0 To yy.Count - 1
            xx(i) = freqStart + i * freqStep
        Next
        'If isSerchLocalDianTai Then
        '    handleSerchLoaclDiantai(xx, yy)
        '    isSerchLocalDianTai = False
        '    Exit Sub
        'End If
        Dim jieshu As Double = freqStart + (dataCount - 1) * freqStep
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
        Chart2.ChartAreas(0).AxisX.Minimum = freqStart
        Chart2.ChartAreas(0).AxisX.Maximum = jieshu
        Chart2.ChartAreas(0).AxisX.Interval = (jieshu - freqStart) / 5
        Chart3.ChartAreas(0).AxisX.Minimum = freqStart
        Chart3.ChartAreas(0).AxisX.Maximum = jieshu
        Chart3.ChartAreas(0).AxisX.Interval = (jieshu - freqStart) / 5
        If freqStart = 88 And jieshu = 108 Then
            Chart5.ChartAreas(0).AxisX.Minimum = freqStart
            Chart5.ChartAreas(0).AxisX.Maximum = jieshu
            Chart5.ChartAreas(0).AxisX.Interval = (jieshu - freqStart) / 5
        End If
        ' Series.BorderWidth = "0.5"
        For i = 0 To dataCount - 1
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
                        Exit For
                    End If
                Next
            End If
        End If
        Chart2.Series(0) = Series
        If freqStart = 88 And jieshu = 108 Then
            Chart5.Series(0) = Series

        End If
        GXPuBuTu(xx, yy)
        If freqStartTimeStr = "" Then
            freqStartTimeStr = Now.ToString("HH:mm:ss")
        End If
        If TimeFreqPoint < freqStart Or TimeFreqPoint > jieshu Then
            TimeFreqPoint = freqStart
        End If
        Dim TimeMarkPointValue As Double = 0
        For i = 0 To xx.Count - 1
            If xx(i) = TimeFreqPoint Then
                TimeMarkPointValue = yy(i)
                If Chart6.Series(0).Points.Count > 2000 Then
                    Chart6.Series(0).Points.RemoveAt(0)
                End If
                Chart6.Series(0).Points.AddXY(Now.ToString("HH:mm:ss"), yy(i))
                Exit For
            End If
        Next
        Dim str As String = "时间: " & freqStartTimeStr & " To " & Now.ToString("HH:mm:ss") & "  频率范围:[" & freqStart & "," & jieshu & "]"
        str = "Mark " & TimeFreqPoint & "MHz," & TimeMarkPointValue & "dBm" & "   " & str
        Label77.Text = str
        Try
            If True Then

                Dim du As Integer = AutoFenXiDu
                Dim result(,) As Double = XinHaoFenLi(xx, yy, du, AutoFenXiFuCha)
                Dim jieti As Integer = (du - 1) / 2
                'ChaFen(xx, yy)
                'ZuiDaZhiBaoChi(xx, yy)
                HandleMuBan(xx, yy)
                'Dim ser As New Series("illegalsignal")
                'ser.ChartType = SeriesChartType.Column
                'ser("PointWidth") = 0.1
                'ser.Color = Color.Gold
                'ser.IsVisibleInLegend = False
                'ser.Name = "illegalsignal"

                If True Then

                    If TongJiCiShu < 60 Then
                        Label23.Visible = True
                        TongJiCiShu = TongJiCiShu + 1
                        XinHao2LV(result)

                        If Chart2.Series.Count >= 2 Then
                            Chart2.Series(1).Points.Clear()
                        End If
                        If freqStart = 88 And jieshu = 108 Then
                            If Chart5.Series.Count >= 2 Then
                                Chart5.Series(1).Points.Clear()
                            End If
                        End If
                        For Each itm As ListViewItem In LV20.Items
                            Dim pl As String = itm.SubItems(2).Text
                            Dim count As String = itm.SubItems(7).Text
                            Dim cq As String = itm.SubItems(6).Text
                            If IsNumeric(count) Then
                                If Val(count) >= 10 Then
                                    If Chart2.Series.Count >= 2 Then
                                        'Chart2.Series(1).Points.AddXY(pl - 0.05, cq)
                                        'Chart2.Series(1).Points.AddXY(pl, cq)
                                        'Chart2.Series(1).Points.AddXY(pl + 0.05, cq)
                                        For j = 0 To xx.Count - 1 - jieti
                                            If xx(j) = pl Then
                                                If j >= jieti Then
                                                    For m = j - jieti To j + jieti
                                                        Dim value As Double = yy(m)
                                                        Me.Invoke(Sub() Chart2.Series(1).Points.AddXY(xx(m), value))
                                                    Next
                                                End If
                                                Exit For
                                            End If
                                        Next
                                        'MsgBox(sh)
                                        'MsgBox(pl & "," & cq)
                                        'If IsNothing(result) = False Then
                                        '    For i = 0 To result.Length / 2 - 1
                                        '        Dim rx As Double = result(i, 0)
                                        '        For j = 0 To xx.Count - 1 - jieti
                                        '            If xx(j) = rx Then
                                        '                If j >= jieti Then
                                        '                    For m = j - jieti To j + jieti
                                        '                        Chart2.Series(1).Points.AddXY(xx(m), yy(m))
                                        '                    Next
                                        '                End If
                                        '                Exit For
                                        '            End If
                                        '        Next
                                        '    Next
                                        'End If
                                    End If
                                    If freqStart = 88 And jieshu = 108 Then
                                        If Chart5.Series.Count >= 2 Then
                                            For j = 0 To xx.Count - 1 - jieti
                                                If xx(j) = pl Then
                                                    If j >= jieti Then
                                                        For m = j - jieti To j + jieti
                                                            Dim value As Double = yy(m)
                                                            Me.Invoke(Sub() Chart5.Series(1).Points.AddXY(xx(m), value))
                                                        Next
                                                    End If
                                                    Exit For
                                                End If
                                            Next

                                        End If
                                    End If
                                End If
                            End If
                        Next
                    Else
                        Label23.Visible = False
                    End If
                Else

                End If

                If True Then
                    'Me.Invoke(Sub() If Chart9.Series.Count >= 2 Then Chart9.Series(1) = ser)
                    ' Me.Invoke(Sub() If Chart2.Series.Count >= 2 Then Chart2.Series(1) = ser)
                    ' Me.Invoke(Sub() If Chart12.Series.Count >= 2 Then Chart12.Series(1) = ser)
                Else
                    'Me.Invoke(Sub() If Chart9.Series.Count >= 2 Then Chart9.Series(1).Points.Clear())
                    '  Me.Invoke(Sub() If Chart2.Series.Count >= 2 Then Chart2.Series(1).Points.Clear())
                    'Me.Invoke(Sub() If Chart12.Series.Count >= 2 Then Chart12.Series(1).Points.Clear())
                End If
            Else
                'Me.Invoke(Sub() If Chart8.Series.Count >= 2 Then Chart8.Series(1).Points.Clear())
                'Me.Invoke(Sub() If Chart9.Series.Count >= 2 Then Chart9.Series(1).Points.Clear())
                'Me.Invoke(Sub() If Chart2.Series.Count >= 2 Then Chart2.Series(1).Points.Clear())
                'Me.Invoke(Sub() If Chart12.Series.Count >= 2 Then Chart12.Series(1).Points.Clear())
            End If
            'SavePinPu(PinPuShuJu)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Structure MuBanPinPu
        Dim time As String
        Dim XX() As Double
        Dim yy() As Double
    End Structure
    Dim isShengChengMuBan As Boolean = False
    Dim muban As MuBanPinPu
    Dim ShengChengMuBanEndTime As Date = "1999-01-01 00:00:00"
    Dim signalStartTime As Date = Nothing
    Dim sigNalCount As Integer = 0
    Private Sub HandleMuBan(ByVal xx() As Double, ByVal yy() As Double)
        If isShengChengMuBan = False Then
            If IsNothing(muban) = False Then
                If IsNothing(muban.XX) = False And IsNothing(muban.yy) = False Then
                    Dim Series2 As New Series
                    Series2.Label = "频谱数据"
                    Series2.XValueType = ChartValueType.Auto
                    Series2.ChartType = SeriesChartType.FastLine
                    Series2.IsVisibleInLegend = False
                    Series2.Color = Color.Black
                    Series2.Name = "series2"
                    Dim series3 As New Series
                    series3.Label = "频谱数据"
                    series3.XValueType = ChartValueType.Auto
                    series3.ChartType = SeriesChartType.FastLine
                    series3.IsVisibleInLegend = False
                    series3.Color = Color.YellowGreen
                    series3.Name = "series3"
                    Dim chayy(yy.Count - 1) As Double
                    For i = 0 To xx.Count - 1
                        Dim cx As Double = xx(i)
                        Dim cy As Double = yy(i) - muban.yy(i)
                        If cy >= 0 Then
                            Series2.Points.AddXY(cx, 0)
                            Series2.Points.AddXY(cx, cy)
                            Series2.Points.AddXY(cx, 0)
                            series3.Points.AddXY(cx, 0)
                        Else
                            Series2.Points.AddXY(cx, 0)
                            series3.Points.AddXY(cx, 0)
                            series3.Points.AddXY(cx, cy)
                            series3.Points.AddXY(cx, 0)
                        End If
                        chayy(i) = cy
                    Next
                    Chart3.Series(3) = Series2
                    Chart3.Series(4) = series3
                    Dim result(,) As Double = XinHaoFenLi2(xx, chayy, 5, 3, 3)
                    Dim ser As New Series("illegalsignal")
                    ser.ChartType = SeriesChartType.Column
                    ser("PointWidth") = 0.1
                    ser.Color = Color.Red
                    ser.IsVisibleInLegend = False
                    Chart3.Series(1).Points.Clear()
                    If IsNothing(result) = False Then
                        ' MsgBox(result.Length / 2 - 1)
                        For i = 0 To result.Length / 2 - 1
                            Dim rx As Double = result(i, 0)
                            For j = 0 To xx.Count - 1
                                If xx(j) = rx Then
                                    Dim ry As Double = yy(j)
                                    If RadioButton1.Checked Then
                                        If CheckBox2.Checked Then
                                            Try
                                                Console.WriteLine("捕获到新信号" & rx & "MHz")
                                                speek("捕获到新信号" & rx & "MHz")
                                            Catch ex As Exception
                                                'Console.WriteLine(ex.ToString)
                                            End Try
                                        End If
                                        If True Then
                                            Dim msgTxt As String = "捕获到新信号" & rx & "MHz"
                                            'For Each itms In CLB.CheckedItems
                                            '    sendWeChatMsg(itms, msgTxt, False, "", "")
                                            'Next
                                        End If
                                        Dim itm As New ListViewItem(LV22.Items.Count + 1)
                                        itm.SubItems.Add(Now.ToString("yyyy-MM-dd HH:mm:ss"))
                                        itm.SubItems.Add(rx)
                                        itm.SubItems.Add(DeviceAddress)
                                        itm.SubItems.Add(ry)
                                        itm.SubItems.Add(ry)
                                        itm.SubItems.Add(ry)
                                        Dim isin As Boolean = False
                                        For Each it As ListViewItem In LV22.Items
                                            If it.SubItems(2).Text = rx Then
                                                it.SubItems(4).Text = ry
                                                If Val(it.SubItems(5).Text) > ry Then
                                                    it.SubItems(5).Text = ry
                                                End If
                                                If Val(it.SubItems(6).Text) < ry Then
                                                    it.SubItems(6).Text = ry
                                                End If
                                                isin = True
                                                Exit For
                                            End If
                                        Next

                                        If isin = False Then
                                            Dim isfind As Boolean = False
                                            For m = 0 To LV22.Items.Count - 1
                                                If Val(LV22.Items(m).SubItems(2).Text) > rx Then
                                                    isfind = True
                                                    LV22.Items.Insert(m, itm)
                                                    Exit For
                                                End If
                                            Next
                                            If isfind = False Then
                                                LV22.Items.Add(itm)
                                            End If
                                        End If
                                    End If
                                    For m = 0 To LV22.Items.Count - 1
                                        LV22.Items(m).Text = m + 1
                                    Next
                                    Dim jieti As Integer = (AutoFenXiDu - 1) / 2
                                    For n = 0 To xx.Count - 1
                                        If xx(n) = rx Then
                                            For m = n - jieti To n + jieti
                                                Chart3.Series(1).Points.AddXY(xx(m), yy(m))
                                            Next
                                            Exit For
                                        End If
                                    Next
                                End If
                            Next
                        Next
                    End If
                Else
                    Chart3.Series(3).Points.Clear()
                    Chart3.Series(1).Points.Clear()
                End If
            Else
                Chart3.Series(3).Points.Clear()
                Chart3.Series(1).Points.Clear()
            End If
        End If
        Dim Series As New Series
        Series.Label = "频谱数据"
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        For i = 0 To xx.Count - 1
            Series.Points.AddXY(xx(i), yy(i))
        Next
        Chart3.Series(0) = Series
        If True Then  '实时生成模板
            If isShengChengMuBan = False Then
                If Now < ShengChengMuBanEndTime Then
                    isShengChengMuBan = True
                End If
            End If
            ' MsgBox(isShengChengMuBan)
            If isShengChengMuBan = True Then
                If IsNothing(muban) Then
                    muban = New MuBanPinPu
                End If
                muban.time = Now.ToString("yyyy-MM-dd HH:mm:ss")
                If xx.Count = yy.Count Then
                    If IsNothing(muban.XX) Or IsNothing(muban.yy) Then
                        ReDim muban.XX(xx.Count - 1)
                        ReDim muban.yy(yy.Count - 1)
                        For i = 0 To xx.Count - 1
                            muban.XX(i) = xx(i)
                            muban.yy(i) = yy(i)
                        Next
                    Else
                        If xx.Count = muban.XX.Count Then
                            Dim iseque As Boolean = True
                            For i = 0 To xx.Count - 1
                                If xx(i) <> muban.XX(i) Then
                                    iseque = False
                                    Exit For
                                End If
                            Next
                            If iseque Then
                                For i = 0 To xx.Count - 1
                                    If yy(i) > muban.yy(i) Then
                                        muban.yy(i) = yy(i)
                                    End If
                                Next
                            End If
                        End If
                    End If
                    If IsNothing(muban.XX) = False And IsNothing(muban.yy) = False Then
                        Dim ser As New Series
                        ser.Label = "频谱数据"
                        ser.XValueType = ChartValueType.Auto
                        ser.ChartType = SeriesChartType.FastLine
                        ser.Color = Color.FromArgb(236, 170, 0)
                        ser.IsVisibleInLegend = False
                        For i = 0 To xx.Count - 1
                            ser.Points.AddXY(xx(i), muban.yy(i))
                        Next
                        Chart3.Series(2) = ser
                    End If
                End If
                If Now >= ShengChengMuBanEndTime Then
                    Label152.Text = "模板生成完毕"
                    isShengChengMuBan = False
                    'Label12.Text = ShengChengMuBanEndTime.ToString("yyyy-MM-dd HH:mm:ss")
                End If
            End If
        Else

        End If
    End Sub
    Public Sub speek(ByVal str As String)
        Dim th As New Thread(AddressOf th_speek)
        th.Start(str)
    End Sub
    Private Sub th_speek(ByVal str As String)
        Try
            Dim voice As New SpVoice
            voice.Rate = voiceRate
            voice.Volume = 100
            voice.Voice = voice.GetVoices().Item(0)
            voice.Speak(str)
        Catch ex As Exception
            '‘MsgBox(ex.ToString)
        End Try

    End Sub
    Private Sub XinHao2LV(ByVal result(,) As Double)
        If IsNothing(result) Then Exit Sub
        If IsNothing(itmList) Then
            itmList = New List(Of ListViewItem)
        End If
        If IsNothing(signalStartTime) Then
            signalStartTime = New Date
            signalStartTime = Now
            sigNalCount = 0
        End If
        If signalStartTime.Year <> Now.Year Then
            signalStartTime = New Date
            signalStartTime = Now
            sigNalCount = 0
        End If
        sigNalCount = sigNalCount + 1
        For i = 0 To result.Length / 2 - 1
            Dim pinlv As Double = result(i, 0)
            Dim changqiang As Double = result(i, 1)
            Dim isinlv4 As Boolean = False
            For k = 0 To itmList.Count - 1
                If itmList(k).SubItems(2).Text = pinlv Then
                    isinlv4 = True
                    Dim min As Double = Val(itmList(k).SubItems(5).Text)
                    Dim max As Double = Val(itmList(k).SubItems(6).Text)
                    If min > changqiang Then min = changqiang
                    If max < changqiang Then max = changqiang
                    itmList(k).SubItems(5).Text = min
                    itmList(k).SubItems(6).Text = max
                    itmList(k).SubItems(7).Text = Val(itmList(k).SubItems(7).Text) + 1
                    itmList(k).SubItems(8).Text = (max + min) * 0.5
                    Dim t As TimeSpan = Now - signalStartTime
                    Dim str As String = t.Hours.ToString("00") & ":" & t.Minutes.ToString("00") & ":" & t.Seconds.ToString("00")
                    itmList(k).SubItems(9).Text = str  '统计时长
                    'itmList(k).SubItems(10).Text = "" '占用度
                    itmList(k).SubItems(11).Text = sigNalCount '监测次数
                    Dim cbcount As Integer = Val(itmList(k).SubItems(12).Text)
                    itmList(k).SubItems(12).Text = cbcount + 1 '超标次数
                    Dim bfb As String = 100 * (cbcount / sigNalCount).ToString("0.00") & "%"
                    itmList(k).SubItems(10).Text = bfb
                    Exit For
                End If
            Next
            If isinlv4 = False Then
                Dim itm As New ListViewItem(itmList.Count + 1) '0
                itm.SubItems.Add(Now.ToString("yyyy-MM-dd HH:mm:ss"))  '1
                itm.SubItems.Add(pinlv) '2
                itm.SubItems.Add(DeviceAddress) '3
                itm.SubItems.Add(changqiang) '4
                itm.SubItems.Add(changqiang) '5
                itm.SubItems.Add(changqiang) '6
                itm.SubItems.Add(1) '6
                itm.SubItems.Add(changqiang) '7
                itm.SubItems.Add("00:00:00") '7
                itm.SubItems.Add(1) '7
                itm.SubItems.Add(sigNalCount) '7
                itm.SubItems.Add(1) '7
                itmList.Add(itm)
            End If
        Next
        For i = 0 To itmList.Count - 1
            itmList(i).Text = i + 1
        Next
        Dim plist As New List(Of ListViewItem)
        For Each itm In itmList
            Dim isadd As Boolean = False
            Dim count As Integer = Val(itm.SubItems(7).Text)
            If count < 10 Then
                Continue For
            End If
            For i = 0 To plist.Count - 1
                Dim itm2 As ListViewItem = plist(i)
                Dim count2 As Integer = Val(itm2.SubItems(7).Text)
                If count > count2 Then
                    plist.Insert(i, itm)
                    isadd = True
                    Exit For
                End If
            Next
            If isadd = False Then
                plist.Add(itm)
            End If
        Next
        LV20.Items.Clear()

        Dim cnt As Integer = plist.Count
        For i = 0 To cnt - 1
            Dim itm As ListViewItem = plist(i)
            itm.Text = i + 1
            Dim CBInt As Integer = Val(itm.SubItems(12).Text)
            Dim SumInt As Integer = Val(itm.SubItems(11).Text)
            Dim str As String = GetPerPic(CBInt / SumInt)
            itm.SubItems.Add(str)
            LV20.Items.Add(itm)
        Next
        LV27.Items.Clear()
        For i = 0 To cnt - 1
            Dim itm As ListViewItem = plist(i).Clone
            itm.Text = i + 1
            LV27.Items.Add(itm)
        Next

        Label145.Text = "信号数量: " & plist.Count
    End Sub
    Private Function GetPerPic(ByVal v As Double) As String
        If v >= 1 Then v = 1
        If v <= 0 Then v = 0
        If v = 0 Then Return ""
        Dim fk As String = "■"
        Dim value As Integer = v * 100
        Dim result As String = ""
        For i = 1 To value
            result = result + fk
        Next
        Return result
    End Function
    Private recentY As Integer = 0
    Private Sub GXPuBuTu(ByVal xx() As Double, ByVal yy() As Double)
        Try
            'Dim leftWidth As Integer = Chart1.Width * 0.04
            'Dim rightWidth As Integer = Chart1.Width * 0.02
            If IsNothing(yy) Then Exit Sub
            Dim max As Integer = 255
            Dim min As Integer = 0
            Dim width As Integer = yy.Count
            Dim heigth As Integer = PBX.Height
            'Dim num As Integer = width
            'If yy.Count < num Then
            '    num = yy.Count
            'End If
            If IsNothing(PBX.Image) Then
                Dim bmp As New Bitmap(width, heigth)
                recentY = 0
                For m = 0 To width - 1
                    Dim value As Double = yy(m)
                    Dim vR As Integer = Abs(value + 90)
                    Dim vG As Integer = Abs(value + 10)
                    Dim vB As Integer = Abs(value + 30)
                    If value > -70 Then
                        vR = 255
                        vG = 255
                        vB = 0
                    End If
                    If vR > 255 Then vR = 255 : If vR < 0 Then vR = 0
                    If vB > 255 Then vB = 255 : If vB < 0 Then vB = 0
                    If vG > 255 Then vG = 255 : If vG < 0 Then vG = 0
                    'If m = width - 1 Then
                    '    Console.WriteLine(vR & "," & vG & "," & vB)
                    'End If
                    bmp.SetPixel(m, recentY, Color.FromArgb(vR, vG, vB))
                Next
                recentY = recentY + 1
                PBX.Image = bmp
            Else
                Dim bmp As Bitmap = PBX.Image
                If recentY <= heigth - 1 Then
                    For m = 0 To width - 1
                        Dim value As Double = yy(m)
                        Dim vR As Integer = Abs(value + 90)
                        Dim vG As Integer = Abs(value + 10)
                        Dim vB As Integer = Abs(value + 30)
                        If value > -70 Then
                            vR = 255
                            vG = 255
                            vB = 0
                        End If
                        If vR > 255 Then vR = 255 : If vR < 0 Then vR = 0
                        If vB > 255 Then vB = 255 : If vB < 0 Then vB = 0
                        If vG > 255 Then vG = 255 : If vG < 0 Then vG = 0
                        'If m = width - 1 Then
                        '    Console.WriteLine(vR & "," & vG & "," & vB)
                        'End If
                        bmp.SetPixel(m, recentY, Color.FromArgb(vR, vG, vB))
                    Next
                    recentY = recentY + 1
                Else
                    Dim nb As Bitmap = bmp.Clone(New Rectangle(0, 1, bmp.Width, bmp.Height - 1), System.Drawing.Imaging.PixelFormat.DontCare)
                    bmp = New Bitmap(bmp.Width, bmp.Height)
                    Dim gk As Graphics = Graphics.FromImage(bmp)
                    gk.DrawImage(nb, 0, 0)
                    gk.Save()
                    For m = 0 To width - 1
                        Dim value As Double = yy(m)
                        Dim vR As Integer = Abs(value + 90)
                        Dim vG As Integer = Abs(value + 10)
                        Dim vB As Integer = Abs(value + 30)
                        If value > -70 Then
                            vR = 255
                            vG = 255
                            vB = 0
                        End If
                        If vR > 255 Then vR = 255 : If vR < 0 Then vR = 0
                        If vB > 255 Then vB = 255 : If vB < 0 Then vB = 0
                        If vG > 255 Then vG = 255 : If vG < 0 Then vG = 0
                        If recentY >= bmp.Height Then
                            bmp.SetPixel(m, bmp.Height - 1, Color.FromArgb(vR, vG, vB))
                        Else
                            bmp.SetPixel(m, recentY, Color.FromArgb(vR, vG, vB))
                        End If

                    Next
                End If
                'bmp.Save(Application.StartupPath & "\a.img")
                'Dim realBitmap As New Bitmap(bmp.Width + leftWidth + rightWidth, bmp.Height)
                'Dim rg As Graphics = Graphics.FromImage(realBitmap)
                'rg.DrawImage(bmp, leftWidth, 0)
                'rg.Save()
                PBX.Image = bmp
                ' Dim leftColor As Color = bmp.GetPixel(0, 0)
                Dim leftColor As Color = Color.FromArgb(0, 80, 60)
                Dim leftBmp As New Bitmap(Panel44.Width, bmp.Height)
                Dim gleft As Graphics = Graphics.FromImage(leftBmp)
                Dim leftBrush As Brush = New SolidBrush(leftColor)
                gleft.FillRectangle(leftBrush, 0, 0, leftBmp.Width, recentY)
                gleft.Save()
                PBXLeft.Image = leftBmp
                'Dim rightColor As Color = bmp.GetPixel(bmp.Width - 1, 0)
                'MsgBox(rightColor.R & "," & rightColor.G & "," & rightColor.B)
                Dim rightBmp As New Bitmap(Panel45.Width, bmp.Height)
                Dim gright As Graphics = Graphics.FromImage(rightBmp)
                Dim rightBrush As Brush = New SolidBrush(leftColor)
                gright.FillRectangle(rightBrush, 0, 0, leftBmp.Width, recentY)
                gright.Save()
                PBXRight.Image = rightBmp
                'Dim gp As Graphics = Panel43.CreateGraphics
                'gp.DrawString(Now.ToString("yyyy-MM-dd HH:mm:ss"), New Font("微软雅黑", 15), Brushes.Black, New Point(10, 10))
                'gp.Save()
            End If
        Catch ex As Exception
            ' MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        Dim id As String = selectDeviceID      
        Dim p As tssOrder_stu
        p.task = "bscan"
        p.freqStart = 88
        p.freqEnd = 108
        p.freqStep = 25
        p.deviceID = selectDeviceID
        Dim orderMsg As String = JsonConvert.SerializeObject(p)
        SendMsgToDev("", "bscan", orderMsg)
    End Sub
    Private Function SendMsgToDev(ByVal deviceID As String, ByVal func As String, ByVal orderMsg As String) As Boolean

        Dim ido As New INGDeviceOrder(deviceID, orderMsg)
        Dim mmm As String = JsonConvert.SerializeObject(ido)
        Dim INGMsg As INGMsgStu = New INGMsgStu(func, mmm)
        Dim json As String = JsonConvert.SerializeObject(INGMsg)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=CommandING&INGMsgText=" & json)
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        If r = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Return True
        Else
            If msg = "Please login" Then
                Login()
                SendMsgToDev(deviceID, func, orderMsg)
            Else
                Dim sb As New StringBuilder
                sb.AppendLine("命令下发失败")
                sb.AppendLine(msg)
                sb.AppendLine(errmsg)
                MsgBox(sb.ToString)
                Return False
            End If
        End If
    End Function

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        Dim id As String = selectDeviceID
        'Dim p As tssOrder_stu
        'p.task = "stop"
        'p.freqStart = 88
        'p.freqEnd = 108
        'p.freqStep = 25
        'p.deviceID = selectDeviceID
        'Dim orderMsg As String = JsonConvert.SerializeObject(p)
        SendMsgToDev("", "stop", "")
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        muban = Nothing
        Dim msg As String = InputBox("请输入模板建立时长", "电磁信息服务")
        If msg = "" Then
            MsgBox("请设置正确的时长！")
            Exit Sub
        End If
        Dim value As Integer = Val(msg)
        If value > 0 Then
            ShengChengMuBanEndTime = Now.AddSeconds(value)
            Label152.Text = ShengChengMuBanEndTime.ToString("HH:mm:ss") & "结束"
            Label11.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
            Label13.Text = value & " 秒"
        Else
            ShengChengMuBanEndTime = "1999-01-01 00:00:00"
            Label152.Text = "请设置正确的时长！"
            MsgBox("请设置正确的时长！")
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim nickid As String = TextBox4.Text
        If nickid = "" Then
            MsgBox("备注不能为空")
            Return
        End If
        Dim str As String = "?func=SetDeviceNickID&nickid=" & nickid & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        If result = "" Then
            result = "设置备注成功！"
        End If
        MsgBox(result)
    End Sub

    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        Dim lng As String = TextBox6.Text
        Dim lat As String = TextBox7.Text
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

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedItem = "自定义频率" Then
            TextBox5.Text = "400,410,420,430,440,450,460,470,480,490,500"
        End If
        If ComboBox1.SelectedItem = "国际遇险求救频率" Then
            TextBox5.Text = "156.4500,156.8000,121.5000,490,518,2.1875,4.2075,6.312,8.4145,16.8045,156.525,156.8"
        End If
        If ComboBox1.SelectedItem = "2018广州马拉松" Then
            TextBox5.Text = "580.0000,596.0000,660.0000,806.0000,816.000,404.2000,403.5375,403.6125,413.5375,413.6125,414.2000"
        End If
        If ComboBox1.SelectedItem = "上海民航通讯频率" Then
            TextBox5.Text = "126.8500,127.6500,124.5500,133.25,123.70,125.95,132.75"
        End If
        If ComboBox1.SelectedItem = "对讲机频率(开放)" Then
            TextBox5.Text = "409.75,409.625,409.775,409.7875,409.8,409.8125,409.8250,409.8375,409.8500,409.8625,409.8750,409.8875,409.9000,409.9125,409.9250,409.9375,409.9500,409.9625,409.9750,409.9875"
        End If
        If ComboBox1.SelectedItem = "营救器信标频率" Then
            TextBox5.Text = "121.5,123.1"
        End If
        If ComboBox1.SelectedItem = "应急通信频率(ITU)" Then
            TextBox5.Text = "144.1,145,433,433.5"
        End If
        If ComboBox1.SelectedItem = "铁路通信频率" Then
            TextBox5.Text = "467.45,467.5,467.55,457.55,467.6,467.65,467.7,467.75,457.7,467.775,467.825,467.875,457.825"
        End If
        If ComboBox1.SelectedItem = "民航通信信标频率" Then
            TextBox5.Text = "108.1,121.6,121.775,121.95,121.975,122,122.025,122.05,122.075,122.7,122.725,122.75,122.8,122.825"
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedItem = "自定义频率" Then
            TextBox5.Text = "400,410,420,430,440,450,460,470,480,490,500"
        End If
        If ComboBox2.SelectedItem = "国际遇险求救频率" Then
            TextBox10.Text = "156.4500,156.8000,121.5000,490,518,2.1875,4.2075,6.312,8.4145,16.8045,156.525,156.8"
        End If
        If ComboBox2.SelectedItem = "2018广州马拉松" Then
            TextBox10.Text = "580.0000,596.0000,660.0000,806.0000,816.000,404.2000,403.5375,403.6125,413.5375,413.6125,414.2000"
        End If
        If ComboBox2.SelectedItem = "上海民航通讯频率" Then
            TextBox10.Text = "126.8500,127.6500,124.5500,133.25,123.70,125.95,132.75"
        End If
        If ComboBox2.SelectedItem = "对讲机频率(开放)" Then
            TextBox10.Text = "409.75,409.625,409.775,409.7875,409.8,409.8125,409.8250,409.8375,409.8500,409.8625,409.8750,409.8875,409.9000,409.9125,409.9250,409.9375,409.9500,409.9625,409.9750,409.9875"
        End If
        If ComboBox2.SelectedItem = "营救器信标频率" Then
            TextBox10.Text = "121.5,123.1"
        End If
        If ComboBox2.SelectedItem = "应急通信频率(ITU)" Then
            TextBox10.Text = "144.1,145,433,433.5"
        End If
        If ComboBox2.SelectedItem = "铁路通信频率" Then
            TextBox10.Text = "467.45,467.5,467.55,457.55,467.6,467.65,467.7,467.75,457.7,467.775,467.825,467.875,457.825"
        End If
        If ComboBox2.SelectedItem = "民航通信信标频率" Then
            TextBox10.Text = "108.1,121.6,121.775,121.95,121.975,122,122.025,122.05,122.075,122.7,122.725,122.75,122.8,122.825"
        End If
    End Sub

    Private Sub PictureBox16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox16.Click
        Pic_Be_Click()
    End Sub
    Private Sub Pic_Be_Click()
        Dim id As String = 4
        Dim code As String = TextBox5.Text
        If code = "" Then
            MsgBox("请输入频点值，以逗号,隔开")
            Return
        End If
        Dim pds() As String = code.Split(",")
        Dim pdList As New List(Of Double)
        For Each itm In pds
            If itm = "" Then Continue For
            If IsNumeric(itm) Then
                pdList.Add(Val(itm))
            End If
        Next
        Dim jcpd As String = "<TZBQ:JCPD," & id & "," & 10 & "," & "Y" & "," & 15 & "," & 15 & "," & 10 & "," & 1 & "," & 1 & "," & pdList.Count
        For i = 0 To pdList.Count - 1
            jcpd = jcpd & "," & pdList(i)
        Next
        jcpd = jcpd & ">"
        If SendMsgToDev("", "tzbqOrder", jcpd) Then
            Sleep(800)
            Dim sssj As String = "<TZBQ:SSSJ," & 0 & ",1>"
            SendMsgToDev("", "tzbqOrder", sssj)
        End If
       
    End Sub

    Private Sub PictureBox15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox15.Click
        Dim id As String = selectDeviceID
        SendMsgToDev("", "stop", "")
    End Sub

    Private Sub RDSignal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RDSignal.CheckedChanged
        isTongJi = False
        'Label32.Text = "直方图"
        'Panel45.Visible = True
        'Panel17.Visible = True
        'Panel50.Visible = True
        For Each s In Chart7.Series
            s.Points.Clear()
        Next
        LV21.Clear()
        LV21.Columns.Add("信号频率(MHz)", 100)
        LV21.Columns.Add("实时电平(dBm)", 100)
        LV21.Columns.Add("属性识别", 70)
        LV21.Columns.Add("状态评估", 70)
        LV21.Columns.Add("可用评估", 70)
        LV21.Columns.Add("占用度", 60)
        LV21.Columns.Add("起始时间", 100)
        LV21.Columns.Add("更新时间", 100)
        LV21.Columns.Add("监测时长", 100)
        LV21.Columns.Add("最大电平(dBm)", 80)
        LV21.Columns.Add("平均电平(dBm)", 80)
        LV21.Columns.Add("最小电平(dBm)", 80)
        LV21.Columns.Add("监测次数", 80)
        LV21.Columns.Add("超标次数", 80)

   
        Chart7.ChartAreas(0).AxisY.Maximum = 100
        Chart7.ChartAreas(0).AxisY.Minimum = 0
        Chart7.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart7.ChartAreas(0).AxisY.Interval = 20
        Chart7.ChartAreas(0).AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount
        Chart7.ChartAreas(0).AxisX.Minimum = Double.NaN
        Chart7.ChartAreas(0).AxisX.Maximum = Double.NaN
        'Chart7.ChartAreas(0).AxisX.IsStartedFromZero = True
        Chart7.ChartAreas(0).AxisX.Interval = 1
    End Sub

   
    Private Sub PictureBox10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox10.Click

    End Sub
End Class
