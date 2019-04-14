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
Imports System.Web
Imports System.Web.HttpUtility
Imports WavePlayer
Public Class OnlineFreq
    Public isLiSanSaoMiao As Boolean = False
    Public lisanFreqStart As Double
    Public lisanFreqEnd As Double
    Public lisanFreqList As List(Of Double)
    Public lisanTime As Date
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
    Class runLocation
        Public lng As String
        Public lat As String
        Public time As String
    End Class

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
        Dim samplingRate As Long
        Dim audioBit As Integer
        Dim channelNum As Integer
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
    Public Sub stopALL()
        Try
            th_ReciveHttpMsg.Abort()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub OnlineFreq_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        Control.CheckForIllegalCrossThreadCalls = False
        Me.DoubleBuffered = True
        'TabControl1.Region = New Region(New RectangleF(TabPage1.Left, TabPage1.Top, TabPage1.Width, TabPage1.Height))
        ini()
        Label33.Text = selectDeviceID
        DTP.Value = Now.AddDays(-1)
        DTP2.Value = Now
        ComboBox3.Items.Clear()
        ComboBox3.Items.Add("2to1")
        ComboBox3.Items.Add("one")
        ComboBox3.SelectedIndex = 0
        Panel51.Dock = DockStyle.Bottom
        Panel43.Dock = DockStyle.Bottom
        Panel4.Dock = DockStyle.Fill
        Panel51.Visible = False
        Panel43.Visible = True
        TabControl1.SelectedIndex = showTabIndex
        TextBox4.Text = selectDeviceID
        th_ReciveHttpMsg = New Thread(AddressOf ReciveHttpMsg)
        th_ReciveHttpMsg.Start()
        GetDeviceLog()
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
        ComboBox1.Items.Add("2018广州马拉松")
        ComboBox1.Items.Add("上海民航通讯频率")
        ComboBox1.Items.Add("对讲机频率(开放)")
        ComboBox1.Items.Add("国际遇险求救频率")
        ComboBox1.Items.Add("营救器信标频率")
        ComboBox1.Items.Add("应急通信频率(ITU)")
        ComboBox1.Items.Add("铁路通信频率")
        ComboBox1.Items.Add("国际遇险求救频率")
        ComboBox1.SelectedIndex = 2

        ComboBox2.Items.Add("8")
        ComboBox2.Items.Add("16")
        ComboBox2.SelectedIndex = 0
        'iniComoBox()
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
        LV20.Columns.Add("信号属性", 80)
        LV20.Columns.Add("状态属性", 80)
        LV20.Columns.Add("可用评估", 80)
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
        LV27.Columns.Add("信号属性", 80)
        LV27.Columns.Add("状态属性", 80)
        LV27.Columns.Add("可用评估", 80)
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

        LVDeviceLog.View = View.Details
        LVDeviceLog.GridLines = True
        LVDeviceLog.FullRowSelect = True
        LVDeviceLog.Columns.Add("序号", 45)
        LVDeviceLog.Columns.Add("设备ID", 50)
        LVDeviceLog.Columns.Add("台站名称", 150)
        LVDeviceLog.Columns.Add("时间", 150)
        LVDeviceLog.Columns.Add("地点", 150)
        LVDeviceLog.Columns.Add("发生事件", 200)
        LVDeviceLog.Columns.Add("执行结果", 150)
        LVDeviceLog.Columns.Add("设备状态", 150)

    End Sub
    Private Sub iniChart()
        iniChart1()
        iniChart2()
        iniChart3()
        inichart4()
        iniChart5()
        iniChart6()
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
    Private Sub ReciveHttpMsg()
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & realDeviceID & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & realDeviceID & "&token=" & token)
            End If
        End If
        HttpMsgUrl = HttpMsgUrl.Replace("123.207.31.37", ServerIP)
        Console.WriteLine("HttpMsgUrl=" & HttpMsgUrl & "?func=GetDevMsg")
        Dim th As New Thread(AddressOf GetControlerVersion)
        th.Start()
        While True
            Dim result As String = GetHttpMsg()
            If result = "" Then Continue While
            HandleHttpMsg(result)
        End While
    End Sub
    Private Function GetHttpMsg() As String
        Try
            Dim req As HttpWebRequest = WebRequest.Create(HttpMsgUrl & "?func=GetDevMsg")


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
            '  Console.WriteLine(str)
            Return str
        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try
    End Function
    Dim Thread_PPSJ As Thread
    Dim Thread_Audio As Thread
    Private Function data2img(ByVal by() As Byte) As Bitmap
        Try

            Dim ms As New MemoryStream(by)
            Dim bitmap As New Bitmap(ms)
            Return bitmap
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Private Sub HandleHttpMsg(ByVal HttpMsg As String)
        Console.WriteLine("收到新消息TSS  " & Now.ToString("HH:mm:ss"))

        Dim PPSJList As New List(Of json_PPSJ)
        Dim AudioList As New List(Of json_Audio)
        Dim lsinfo As LisanInfo
        Try
            Dim p As JArray = JArray.Parse(HttpMsg)

            For Each itm As JValue In p
                Dim jMsg As String = itm.Value
                Dim JObj As Object = JObject.Parse(jMsg)
                Dim func As String = JObj("func").ToString
                Console.WriteLine("func= " & func)
                If func = "bscan" Then
                    Dim msg As String = JObj("msg").ToString
                    Dim ppsj As json_PPSJ = JsonConvert.DeserializeObject(msg, GetType(json_PPSJ))
                    PPSJList.Add(ppsj)
                End If
                If func = "lisan" Then
                    Dim msg As String = JObj("msg").ToString
                    Try
                        lsinfo = JsonConvert.DeserializeObject(Of LisanInfo)(msg)
                    Catch ex As Exception

                    End Try
                    ' File.WriteAllText("C:\Users\meizi\Desktop\lisan.txt", msg)
                End If
                If func = "ifscan_wav" Then
                    Dim msg As String = JObj("msg").ToString
                    Dim audio As json_Audio = JsonConvert.DeserializeObject(msg, GetType(json_Audio))
                    AudioList.Add(audio)
                End If
                If func = "ScreenImage" Then
                    Dim msg As String = JObj("msg").ToString
                    Try
                        Dim buffer() As Byte = Convert.FromBase64String(msg)
                        If IsNothing(buffer) = False Then
                            Dim decombuffer() As Byte = Decompress(buffer)
                            If IsNothing(decombuffer) = False Then
                                Dim bmp As Bitmap = data2img(decombuffer)
                                If IsNothing(bmp) = False Then
                                    Label41.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
                                    PBXControlImagr.Image = bmp
                                End If
                            End If
                        End If
                    Catch ex As Exception

                    End Try
                End If
            Next
        Catch ex As Exception
            ' MsgBox(ex.ToString)
            Exit Sub
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
        If AudioList.Count > 0 Then
            'If IsNothing(Thread_Audio) = False Then
            '    Try
            '        Thread_Audio.Abort()
            '    Catch ex As Exception

            '    End Try
            'End If

            'Thread_Audio = New Thread(AddressOf HandleAudioList)
            'Thread_Audio.Start(AudioList)
            Dim th2 As New Thread(AddressOf HandleAudioList)
            th2.Start(AudioList)
        End If
        If IsNothing(lsinfo) = False Then
            LisanData2Lv20(lsinfo)
        End If

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
                            ' Dim audioBufferInfo As New audioBufferInfo(realBy, 8000, 1, 8)
                            Dim th As New Thread(AddressOf play)
                            th.Start(buffer)
                            ' play(buffer)
                            'Me.Invoke(Sub() gxChart(realBy))
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
    Structure audioBufferInfo
        Dim buf() As Byte
        Dim nSamplesPerSec As Integer
        Dim nChannels As Integer
        Dim bit As Short
        Sub New(ByVal _buf() As Byte, ByVal _nSamplesPerSec As Integer, ByVal _nChannels As Integer, ByVal _bit As Short)
            buf = _buf
            nSamplesPerSec = _nSamplesPerSec
            nChannels = _nChannels
            bit = _bit
        End Sub
    End Structure
    Private Sub play(ByVal buf() As Byte)
        Try
            Console.WriteLine("标识：" & Encoding.Default.GetString(buf, 0, 4))
            Console.WriteLine("pcm：" & BitConverter.ToInt16(buf, 20))
            Console.WriteLine("采样率：" & BitConverter.ToInt32(buf, 24))
            Console.WriteLine("通道数：" & BitConverter.ToInt16(buf, 22))
            Console.WriteLine("音频位数：" & BitConverter.ToInt16(buf, 34))
            Dim ms As MemoryStream = New MemoryStream(buf)
            Dim sp As SoundPlayer = New SoundPlayer(ms)
            sp.Play()
            'Dim objWavePlayer As New WavePlayer.WavePlayer(audioBufferInfo.nSamplesPerSec, 174600, audioBufferInfo.nChannels, audioBufferInfo.bit)
            'objWavePlayer.Load(audioBufferInfo.buf, False)
            'objWavePlayer.Play()
        Catch ex As Exception
            'MsgBox(ex.ToString)
        End Try
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
                    'Console.WriteLine(itm.runLocation.lng & "," & itm.runLocation.lat)
                    Console.WriteLine(itm.freqStart & "," & itm.freqStep & "," & itm.freqEnd & "," & itm.dataCount)
                    Console.WriteLine("dataCount=" & itm.dataCount)
                    Me.Invoke(Sub() handlePinPuFenXi(itm))
                    'If itm.dataCount < 5000 Then

                    'End If
                Catch ex As Exception
                    ' MsgBox(ex.ToString)
                End Try
                If i <> count - 1 Then
                    Sleep(sleepCount)
                End If
            Next
        End SyncLock
    End Sub
    Private recentFreqStart As Double
    Private recentFreqEnd As Double
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
        'If Label25.Text = "00:00:00" Then
        '    Label25.Text = Now.ToString("HH:mm:ss")
        'End If
        'Label26.Text = Now.ToString("HH:mm:ss")
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

        Dim dataCount As Integer = yy.Count
        Dim deviceID As String = p.deviceID
        Dim maxCount As Integer = 5000
        Dim xx() As Double
        If dataCount <= maxCount Then
            ReDim xx(dataCount - 1)
            For i = 0 To yy.Count - 1
                xx(i) = freqStart + i * freqStep
            Next
            Console.WriteLine("<=5000")
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
        xx(xx.Length - 1) = p.freqEnd

        'If isSerchLocalDianTai Then
        '    handleSerchLoaclDiantai(xx, yy)
        '    isSerchLocalDianTai = False
        '    Exit Sub
        'End If
        Dim jieshu As Double = freqStart + (dataCount - 1) * freqStep
        jieshu = p.freqEnd
        If freqStart <> recentFreqStart Or jieshu <> recentFreqEnd Then
            iniPBX()
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
        If isDSGFreq Then
            str = str & "  <DSG频谱压缩 " & jslenstr & ">"
        Else
            str = str & "  <" & jslenstr & ">"
        End If
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
                If isTongJi Then
                    If TongJiCiShu < 60 Then
                        'Label23.Visible = True
                        TongJiCiShu = TongJiCiShu + 1
                        XinHao2LV(result, xx, yy)
                        ' Label24.Text = "正在搜索与统计"
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
                            Dim count As String = itm.SubItems(7 + 3).Text
                            Dim cq As String = itm.SubItems(6 + 3).Text
                            If IsNumeric(count) Then
                                If Val(count) >= 0 Then
                                    If Chart2.Series.Count >= 2 Then
                                        For j = 0 To xx.Count - 1
                                            If xx(j) = pl Then
                                                For m = j - jieti To j + jieti
                                                    If m >= 0 And m <= xx.Count - 1 Then
                                                        Dim value As Double = yy(m)
                                                        Me.Invoke(Sub() Chart2.Series(1).Points.AddXY(xx(m), value))
                                                    End If

                                                Next
                                                Exit For
                                            End If
                                        Next
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
                        'Label23.Visible = False
                        Label24.Text = "搜索统计完毕"
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
            ' MsgBox(ex.ToString)
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
                    Dim fucha As Double = 5
                    Dim daikuan As Double = 3
                    Dim mincha As Double = 3
                    fucha = Val(TextBox11.Text)
                    daikuan = Val(TextBox12.Text) * 2 + 1
                    mincha = Val(TextBox13.Text)
                    If fucha < 1 Then fucha = 1
                    If daikuan Mod 2 = 0 Then daikuan = daikuan + 1
                    If daikuan < 3 Then daikuan = 3
                    If mincha < 0 Then mincha = 0
                    Console.WriteLine("幅差：" & fucha)
                    Console.WriteLine("带宽：" & daikuan)
                    Console.WriteLine("小差：" & mincha)
                    Dim result(,) As Double = XinHaoFenLi2(xx, chayy, daikuan, fucha, mincha)
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
    Private Sub XinHao2LV(ByVal result(,) As Double, xx() As Double, yy() As Double)
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
                    Dim min As Double = Val(itmList(k).SubItems(5 + 3).Text)
                    Dim max As Double = Val(itmList(k).SubItems(6 + 3).Text)
                    If min > changqiang Then min = changqiang
                    If max < changqiang Then max = changqiang
                    itmList(k).SubItems(5 + 3).Text = min
                    itmList(k).SubItems(6 + 3).Text = max
                    itmList(k).SubItems(7 + 3).Text = Val(itmList(k).SubItems(7 + 3).Text) + 1
                    itmList(k).SubItems(8 + 3).Text = (max + min) * 0.5
                    Dim t As TimeSpan = Now - signalStartTime
                    Dim str As String = t.Hours.ToString("00") & ":" & t.Minutes.ToString("00") & ":" & t.Seconds.ToString("00")
                    itmList(k).SubItems(9 + 3).Text = str  '统计时长
                    'itmList(k).SubItems(10).Text = "" '占用度
                    itmList(k).SubItems(11 + 3).Text = sigNalCount '监测次数
                    Dim cbcount As Integer = Val(itmList(k).SubItems(12 + 3).Text)
                    itmList(k).SubItems(12 + 3).Text = cbcount + 1 '超标次数
                    Dim cbPercent As Double = 100 * (cbcount / sigNalCount)
                    Dim bfb As String = cbPercent.ToString("0.00") & "%"
                    itmList(k).SubItems(10 + 3).Text = bfb
                    itmList(k).SubItems(4).Text = "不明信号" '信号属性
                    Dim ztsx As String = "正常"
                    Dim kypg As String = "可用"
                    If cbPercent > 50 And cbcount >= (sigNalCount * 0.5) Then
                        ztsx = "超标"
                    End If
                    If cbPercent > 20 And cbcount >= (sigNalCount * 0.5) Then
                        kypg = "不可用"
                    End If
                    itmList(k).SubItems(5).Text = ztsx '状态属性
                    itmList(k).SubItems(6).Text = kypg '可用评估
                    Exit For
                End If
            Next
            If isinlv4 = False Then
                Dim itm As New ListViewItem(itmList.Count + 1) '0
                itm.SubItems.Add(Now.ToString("yyyy-MM-dd HH:mm:ss"))  '1
                itm.SubItems.Add(pinlv) '2
                itm.SubItems.Add(DeviceAddress) '3
                itm.SubItems.Add("")  '信号属性  '4
                itm.SubItems.Add("")  '状态属性  '5
                itm.SubItems.Add("")  '可用评估  '6
                itm.SubItems.Add(changqiang) '7
                itm.SubItems.Add(changqiang) '8
                itm.SubItems.Add(changqiang) '9
                itm.SubItems.Add(1) '10
                itm.SubItems.Add(changqiang) '11
                itm.SubItems.Add("00:00:00") '12
                itm.SubItems.Add(1) '13
                itm.SubItems.Add(sigNalCount) '14
                itm.SubItems.Add(1) '15
                itmList.Add(itm)
            End If
        Next
        For i = 0 To itmList.Count - 1
            itmList(i).Text = i + 1
        Next
        Dim plist As New List(Of ListViewItem)
        For Each itm In itmList
            Dim isadd As Boolean = False
            Dim count As Integer = Val(itm.SubItems(7 + 3).Text)
            If count < 10 Then
                Continue For
            End If
            For i = 0 To plist.Count - 1
                Dim itm2 As ListViewItem = plist(i)
                Dim count2 As Integer = Val(itm2.SubItems(7 + 3).Text)
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


        Dim cnt As Integer = plist.Count
        'If isLiSanSaoMiao Then
        '    Dim isNeedReload As Boolean = False
        '    If LV20.Items.Count = 0 Then
        '        isNeedReload = True
        '    Else
        '        Dim lvFreqlist As New List(Of String)
        '        For Each itm As ListViewItem In LV20.Items
        '            Dim freq As Double = itm.SubItems(2).Text
        '            lvFreqlist.Add(freq)
        '        Next
        '        For Each itm In lvFreqlist
        '            If lisanFreqList.Contains(itm) = False Then
        '                isNeedReload = True
        '                Exit For
        '            End If
        '        Next
        '        If isNeedReload Then
        '            For Each itm In lisanFreqList
        '                If lvFreqlist.Contains(itm) = False Then
        '                    isNeedReload = True
        '                    Exit For
        '                End If
        '            Next
        '        End If
        '    End If
        '    If isNeedReload Then
        '        LV20.Items.Clear()
        '    End If
        '    Dim nowstr As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        '    Dim newItmlist As New List(Of ListViewItem)
        '    Dim runIndex As Integer = 0
        '    Dim ts As TimeSpan = Now - lisanTime
        '    Dim watchTime As String = $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}"
        '    Dim chaobiaoMax As Double = -70
        '    If isNeedReload Then
        '        For Each freq In lisanFreqList
        '            Dim changqiang As Double = 0
        '            For i = runIndex To xx.Length - 2
        '                Dim leftd As Double = Math.Abs(xx(i) - freq)
        '                Dim rightd As Double = Math.Abs(xx(i + 1) - freq)
        '                If leftd < rightd Then
        '                    changqiang = yy(i)
        '                Else
        '                    changqiang = yy(i + 1)
        '                End If
        '            Next
        '            Dim itm As New ListViewItem(newItmlist.Count + 1) '0
        '            itm.SubItems.Add(nowstr)  '1 时间
        '            itm.SubItems.Add(freq) '2  频点
        '            itm.SubItems.Add(DeviceAddress) '3  地点
        '            itm.SubItems.Add("")  '信号属性  '4  信号属性
        '            itm.SubItems.Add("")  '状态属性  '5  状态属性
        '            itm.SubItems.Add("")  '可用评估  '6  可用评估
        '            itm.SubItems.Add(changqiang) '7  信号电平
        '            itm.SubItems.Add(changqiang) '8  最小值
        '            itm.SubItems.Add(changqiang) '9  最大值
        '            itm.SubItems.Add(0) '10  出现次数
        '            itm.SubItems.Add(changqiang) '11 平均值
        '            itm.SubItems.Add(watchTime) '12 统计时长
        '            itm.SubItems.Add(0) '13  占用度
        '            itm.SubItems.Add(1) '14  监测次数
        '            itm.SubItems.Add(0) '15 超标次数
        '            Dim CBInt As Integer = Val(itm.SubItems(12 + 3).Text)
        '            Dim SumInt As Integer = Val(itm.SubItems(11 + 3).Text)
        '            Dim str As String = GetPerPic(CBInt / SumInt)
        '            itm.SubItems.Add(str) '占用度直方图
        '            newItmlist.Add(itm)
        '        Next
        '        LV20.Items.AddRange(newItmlist.ToArray())
        '    Else
        '        For Each freq In lisanFreqList
        '            Dim changqiang As Double = 0
        '            For i = runIndex To xx.Length - 2
        '                Dim leftd As Double = Math.Abs(xx(i) - freq)
        '                Dim rightd As Double = Math.Abs(xx(i + 1) - freq)
        '                If leftd < rightd Then
        '                    changqiang = yy(i)
        '                Else
        '                    changqiang = yy(i + 1)
        '                End If
        '            Next
        '            Dim itm As ListViewItem
        '            If isNeedReload = False Then
        '                For i = 0 To LV20.Items.Count - 1
        '                    If LV20.Items(i).SubItems(2).Text = freq.ToString() Then
        '                        itm = LV20.Items(i)
        '                        Exit For
        '                    End If
        '                Next
        '            End If
        '            If IsNothing(itm) Then Continue For


        '            itm.SubItems(1).Text = nowstr
        '            itm.SubItems(4).Text = "" '4  信号属性
        '            itm.SubItems(5).Text = "" '5  状态属性
        '            itm.SubItems(6).Text = "" '6  可用评估
        '            itm.SubItems(7).Text = changqiang
        '            itm.SubItems(8).Text = IIf(itm.SubItems(8).Text > changqiang, changqiang, itm.SubItems(8).Text)
        '            itm.SubItems(9).Text = IIf(itm.SubItems(9).Text < changqiang, changqiang, itm.SubItems(9).Text)
        '            itm.SubItems(11).Text = (Double.Parse(itm.SubItems(11).Text) + changqiang) / 2
        '            Dim ischaobiao As Boolean = changqiang > chaobiaoMax
        '            If ischaobiao Then
        '                itm.SubItems(10).Text += 1
        '                itm.SubItems(15).Text += 1
        '            End If
        '            itm.SubItems(14).Text += 1
        '            Dim CBInt As Integer = Val(itm.SubItems(15).Text)
        '            Dim SumInt As Integer = Val(itm.SubItems(14).Text)
        '            Dim zyd As String = (100 * CBInt / SumInt).ToString("0.0")
        '            itm.SubItems(13).Text = zyd
        '            Dim str As String = GetPerPic(CBInt / SumInt)
        '            itm.SubItems(16).Text = str

        '        Next

        '    End If


        'Else
        '    LV20.Items.Clear()
        '    For i = 0 To cnt - 1
        '        Dim itm As ListViewItem = plist(i)
        '        itm.Text = i + 1
        '        Dim CBInt As Integer = Val(itm.SubItems(12 + 3).Text)
        '        Dim SumInt As Integer = Val(itm.SubItems(11 + 3).Text)
        '        Dim str As String = GetPerPic(CBInt / SumInt)
        '        itm.SubItems.Add(str)
        '        LV20.Items.Add(itm)
        '    Next
        'End If

        LV27.Items.Clear()
        For i = 0 To cnt - 1
            Dim itm As ListViewItem = plist(i).Clone
            itm.Text = i + 1
            LV27.Items.Add(itm)
        Next

        Label145.Text = "信号数量: " & plist.Count
    End Sub
    Private Sub LisanData2Lv20(lsinfo As LisanInfo)
        If IsNothing(lsinfo) Then Return
        If IsNothing(lsinfo.pointlist) Then Return
        Dim newItmlist As New List(Of ListViewItem)
        Dim nowstr As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim ts As TimeSpan = Now - lsinfo.startTime
        Dim ws As String = $"{ts.Hours.ToString("0.00")}:{ts.Minutes.ToString("0.00")}:{ts.Seconds.ToString("0.00")}"
        For Each freq In lsinfo.pointlist
            Dim itm As New ListViewItem(newItmlist.Count + 1) '0
            itm.SubItems.Add(nowstr)  '1 时间
            itm.SubItems.Add(freq.freq) '2  频点
            itm.SubItems.Add(DeviceAddress) '3  地点
            itm.SubItems.Add(freq.sigNalInfo)  '信号属性  '4  信号属性
            itm.SubItems.Add(freq.sigNalStatus)  '状态属性  '5  状态属性
            itm.SubItems.Add(IIf(freq.isFree, "可用", "不可用"))  '可用评估  '6  可用评估
            itm.SubItems.Add(freq.dbm) '7  信号电平
            itm.SubItems.Add(freq.dbm_min) '8  最小值
            itm.SubItems.Add(freq.dbm_max) '9  最大值
            itm.SubItems.Add(freq.overCount) '10  出现次数
            itm.SubItems.Add(freq.dbm_avg) '11 平均值
            itm.SubItems.Add(ws) '12 统计时长
            itm.SubItems.Add(freq.overPercent) '13  占用度
            itm.SubItems.Add(lsinfo.watchTime) '14  监测次数
            itm.SubItems.Add(freq.overCount) '15 超标次数
            Dim CBInt As Integer = Val(itm.SubItems(12 + 3).Text)
            Dim SumInt As Integer = Val(itm.SubItems(11 + 3).Text)
            Dim str As String = GetPerPic(CBInt / SumInt)
            itm.SubItems.Add(str) '占用度直方图
            newItmlist.Add(itm)
        Next
        Me.Invoke(Sub()
                      LV20.Items.Clear()
                      LV20.Items.AddRange(newItmlist.ToArray())
                  End Sub)
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

    Private Sub StartFreq(freqbegin As Double, freqend As Double, freqstep As Double)
        Dim msg As String = ""

        Dim gcValue As Double = 8
        If ComboBox2.SelectedIndex = 1 Then
            gcValue = 16
        End If
        If RadioButton4.Checked Then
            'freqstep = 25
            Dim centerFreq As Double = Val(TextBox1.Text)
            Dim span As Double = Val(TextBox2.Text) / 2
            freqbegin = centerFreq - span
            freqend = centerFreq + span
        End If
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = freqend
        p.freqStep = freqstep
        p.gcValue = gcValue
        p.task = "bscan"
        Dim DHDeviceStr As String = "2to1"
        If ComboBox3.SelectedIndex = 1 Then
            DHDeviceStr = "one"
        End If
        p.DHDevice = DHDeviceStr
        p.deviceID = selectDeviceID

        Dim orderMsg As String = JsonConvert.SerializeObject(p)
        SendMsgToDev(orderMsg)
    End Sub
    Private Sub Button26_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        StopDevice()
    End Sub
    Private Function SendMsgToDev(ByVal orderMsg As String)
        Label31.Visible = True
        Dim th As New Thread(AddressOf Th_SendMsgToDev)
        th.Start(orderMsg)
    End Function
    Private Sub Th_SendMsgToDev(ByVal orderMsg As String)
        Dim str As String = "?func=tssOrder&datamsg=" & orderMsg & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        Me.Invoke(Sub()
                      Label31.Visible = False
                      If r = "success" Then
                          Dim w As New WarnBox("命令下发成功！")
                          w.Show()
                      Else
                          If msg = "Please login" Then
                              Login()
                              SendMsgToDev(orderMsg)
                          Else
                              Dim sb As New StringBuilder
                              sb.AppendLine("命令下发失败")
                              sb.AppendLine(result)
                              MsgBox(sb.ToString)
                          End If
                      End If
                  End Sub)

    End Sub
    Private Sub StopDevice()
        Dim p As tssOrder_stu
        p.freqStart = 88
        p.freqEnd = 108
        p.freqStep = 25
        p.task = "stop"
        p.deviceID = selectDeviceID
        Dim orderMsg As String = JsonConvert.SerializeObject(p)
        SendMsgToDev(orderMsg)
    End Sub
    Private Sub iniPBX()
        PBX.Image = Nothing
        PBXLeft.Image = Nothing
        PBXRight.Image = Nothing
    End Sub
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        isLiSanSaoMiao = False
        freqStartTimeStr = ""
        TongJiCiShu = 0
        sigNalCount = 0
        iniPBX()
        signalStartTime = Nothing
        itmList = Nothing
        iniChart1()
        iniChart2()
        iniChart3()
        Dim freqbegin As Double = Val(TextBox1.Text)
        Dim freqend As Double = Val(TextBox2.Text)
        Dim freqstep As Double = Val(TextBox3.Text)
        StartFreq(freqbegin, freqend, freqstep)
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        StopDevice()
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        isTongJi = CheckBox1.Checked

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

    Private Sub Panel34_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel34.MouseDown
        Dim maxNum As Double = 108
        Dim minNum As Double = 88
        Dim width As Integer = Panel34.Width
        Dim x As Integer = e.X
        Dim v As Double = x / width
        Dim pl As Double = minNum + (maxNum - minNum) * v
        Label18.Text = pl.ToString("0.0000") & " MHz"
        LSP.X1 = x
        LSP.X2 = x
        If x > width - 120 Then
            Label18.Left = x - Label18.Width - 10
        Else
            Label18.Left = x + 10
        End If
    End Sub
    Private Sub Panel34_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel34.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Dim maxNum As Double = 108
            Dim minNum As Double = 88
            Dim width As Integer = Panel34.Width
            Dim x As Integer = e.X
            If x < 0 Then x = 0
            If x > width Then x = width
            Dim v As Double = x / width
            If x = width Then x = width - 1
            Dim pl As Double = minNum + (maxNum - minNum) * v
            Label18.Text = pl.ToString("0.0000") & " MHz"
            LSP.X1 = x
            LSP.X2 = x
            If x > width - 120 Then
                Label18.Left = x - Label18.Width - 10
            Else
                Label18.Left = x + 10
            End If

        End If
    End Sub

    Private Sub OnlineFreq_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Panel24.Left = Me.Width / 2 - Panel24.Width / 2
        Panel24.Top = Me.Height / 2 - Panel24.Height / 2
        Panel44.Width = Chart1.Width * 0.04
        Panel45.Width = Chart1.Width * 0.02
        'Label77.Text = Chart1.Width & "/" & Chart1.ChartAreas(0).Position.Width
    End Sub
    Private Sub PlayAudio(ByVal freq As Double)
        Dim text As String = freq
        Dim msg As String = ""
        Dim freqbegin As String = text
        Dim freqend As String = text
        Dim freqstep As String = text
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = 108
        p.freqStep = 10 * (108 - freqbegin) / 8
        p.task = "ifscan_wav"
        p.deviceID = selectDeviceID
        Dim orderMsg As String = JsonConvert.SerializeObject(p)
        SendMsgToDev(orderMsg)
    End Sub


    Private Sub PictureBox9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub



    Private Sub 快捷输入ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 快捷输入ToolStripMenuItem.Click
        Dim text As String = InputBox("请输入频率值(范围88 -- 108)", "电磁信息云服务")
        If text = "" Then Exit Sub
        If IsNumeric(text) = False Then Exit Sub
        Dim vt As Double = Val(text)
        If vt < 88 Or vt > 108 Then
            MsgBox("请输入介于88到108之间的值")
            Exit Sub
        End If
        Label18.Text = text & "MHz"
        Dim maxNum As Double = 108
        Dim minNum As Double = 88
        Dim width As Integer = Panel34.Width
        Dim x As Integer = width * ((vt - minNum) / (maxNum - minNum))
        If x >= width Then x = width - 1
        LSP.X1 = x
        LSP.X2 = x
        If x > 400 Then
            Label18.Left = x - Label18.Width - 10
        Else
            Label18.Left = x + 10
        End If
    End Sub

    Private Sub Label10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        If LV20.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LV20.SelectedItems(0)
        Dim pl As String = itm.SubItems(2).Text
        Dim text As String = pl
        If IsNumeric(text) = False Then Exit Sub
        Dim vt As Double = Val(text)
        If vt < 88 Or vt > 108 Then
            MsgBox("请输入介于88到108之间的值")
            Exit Sub
        End If
        TabControl1.SelectedIndex = 0
        Label18.Text = text & "MHz"
        Dim maxNum As Double = 108
        Dim minNum As Double = 88
        Dim width As Integer = Panel34.Width
        Dim x As Integer = width * ((vt - minNum) / (maxNum - minNum))
        If x >= width Then x = width - 1
        LSP.X1 = x
        LSP.X2 = x
        If x > 400 Then
            Label18.Left = x - Label18.Width - 10
        Else
            Label18.Left = x + 10
        End If


        PlayAudio(text)
    End Sub


    Private Sub PictureBox3_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox3.MouseHover
        PictureBox3.Image = My.Resources.播放_on
    End Sub

    Private Sub PictureBox3_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox3.MouseLeave
        PictureBox3.Image = My.Resources.播放
    End Sub
    Private Sub PictureBox1_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.MouseHover
        PictureBox1.Image = My.Resources.停止_on
    End Sub

    Private Sub PictureBox1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.MouseLeave
        PictureBox1.Image = My.Resources.停止
    End Sub

    Private Sub Panel34_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)

    End Sub

    Private Sub PictureBox10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox10.Click
        isLiSanSaoMiao = False
        Dim text As String = Label18.Text
        text = text.Replace("MHz", "")
        PlayAudio(text)
    End Sub
    Private Sub PictureBox10_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox10.MouseHover
        PictureBox10.Image = My.Resources.播放_on
    End Sub

    Private Sub PictureBox10_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox10.MouseLeave
        PictureBox10.Image = My.Resources.播放
    End Sub

    Private Sub PictureBox9_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox9.MouseHover
        PictureBox9.Image = My.Resources.停止_on
    End Sub

    Private Sub PictureBox9_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox9.MouseLeave
        PictureBox9.Image = My.Resources.停止
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        If LV27.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LV27.SelectedItems(0)
        Dim pl As String = itm.SubItems(2).Text
        Dim text As String = pl
        If IsNumeric(text) = False Then Exit Sub
        Dim vt As Double = Val(text)
        If vt < 88 Or vt > 108 Then
            MsgBox("请输入介于88到108之间的值")
            Exit Sub
        End If
        TabControl1.SelectedIndex = 0
        Label18.Text = text & "MHz"
        Dim maxNum As Double = 108
        Dim minNum As Double = 88
        Dim width As Integer = Panel34.Width
        Dim x As Integer = width * ((vt - minNum) / (maxNum - minNum))
        If x >= width Then x = width - 1
        LSP.X1 = x
        LSP.X2 = x
        If x > 400 Then
            Label18.Left = x - Label18.Width - 10
        Else
            Label18.Left = x + 10
        End If
        PlayAudio(text)
    End Sub

    Private Sub PictureBox11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox11.Click
        TongJiCiShu = 0
        sigNalCount = 0
        signalStartTime = Nothing
        itmList = Nothing
        iniChart1()
        iniChart2()
        iniChart3()

        Dim msg As String = ""
        Dim freqbegin As String = 88
        Dim freqend As String = 108
        Dim freqstep As String = 25
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = freqend
        p.freqStep = freqstep
        p.task = "bscan"
        p.deviceID = selectDeviceID
        Dim orderMsg As String = JsonConvert.SerializeObject(p)
        SendMsgToDev(orderMsg)
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

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim dataType As String = "task"
        Dim funcType As String = "reboot"
        Dim paraMsg As String = ""
        Dim str As String = "?func=tssOrderByCode&dataType=" & dataType & "&funcType=" & funcType & "&paraMsg=" & paraMsg & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim w As New WarnBox("命令下发成功！")
        w.Show()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Dim dataType As String = "task"
        'Dim funcType As String = "netswitch"
        'Dim paraMsg As String = "<netswitch:sw=in;>"
        'Dim str As String = "?func=tssOrderByCode&dataType=" & dataType & "&funcType=" & funcType & "&paraMsg=" & paraMsg
        'Dim result As String = GetH(HttpMsgUrl, Str)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=netswitchin")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        End If

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        'Dim dataType As String = "task"
        'Dim funcType As String = "netswitch"
        'Dim paraMsg As String = "<netswitch:sw=out;>"
        'Dim str As String = "?func=tssOrderByCode&dataType=" & dataType & "&funcType=" & funcType & "&paraMsg=" & paraMsg
        'Dim result As String = GetH(HttpMsgUrl, str)
        Dim result As String = GethWithToken(HttpMsgUrl, "func=netswitchout")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        End If
    End Sub

    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        Label1.Text = "起始频率"
        Label2.Text = "终止频率"
        Label2.Visible = True
        TextBox2.Text = Val(TextBox1.Text) + 20
        TextBox3.Text = 25
        TextBox2.Visible = True
        Label5.Visible = True
        Label3.Text = "频率步进"
        Label6.Text = "KHz"
    End Sub

    Private Sub RadioButton4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton4.CheckedChanged
        If RadioButton4.Checked = False Then
            Return
        End If
        Label1.Text = "中心频率"
        Label2.Text = "扫描带宽"
        Label3.Text = "频率步进"
        TextBox2.Text = 5
    End Sub

    Private Sub PictureBox12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox12.Click
        RadioButton3.Checked = True
        Dim q As New QuickFreq(Me)
        q.Show()
        'Dim img As Image = PictureBox12.Image
        'img.Save("d:\meizihuai.png", Imaging.ImageFormat.Png)
    End Sub

    Private Sub CheckBox3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked Then
            Label21.Text = "音频波形"
        Else
            Label21.Text = "Mark时序"
        End If

    End Sub

    Private Sub LV20_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub



    'Private Sub Chart1_GetToolTipText(ByVal sender As Object, ByVal e As System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs) Handles Chart1.GetToolTipText
    '    If e.HitTestResult.ChartElementType = ChartElementType.DataPoint Then
    '        Dim i As Integer = e.HitTestResult.PointIndex
    '        Dim dp As DataPoint = e.HitTestResult.Series.Points(i)
    '        e.Text = dp.XValue & "," & dp.YValues(0)
    '    End If

    'End Sub

    Private Sub Chart1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Chart1.MouseMove
        'If e.Button = Windows.Forms.MouseButtons.Left Then
        '    Dim myTestResult As HitTestResult = Chart1.HitTest(e.X, e.Y)
        '    If myTestResult.ChartElementType = ChartElementType.DataPoint Then
        '        Me.Cursor = Cursors.Cross
        '        Dim i As Integer = myTestResult.PointIndex
        '        Dim dp As DataPoint = myTestResult.Series.Points(i)
        '        Dim xValue As Double = dp.XValue
        '        Dim yValue As Double = dp.YValues(0)
        '        Chart1.Series(1).Points.Clear()
        '        Chart1.Series(1).Points.AddXY(xValue, -120)
        '        Chart1.Series(1).Points.AddXY(xValue, -20)


        '        ' myTestResult.Series.Points(i).MarkerSize = 5
        '    Else
        '        Me.Cursor = Cursors.Default
        '    End If
        'End If
        Dim myTestResult As HitTestResult = Chart1.HitTest(e.X, e.Y)
        If myTestResult.ChartElementType = ChartElementType.DataPoint Then
            Me.Cursor = Cursors.Cross
            Dim i As Integer = myTestResult.PointIndex
            Dim dp As DataPoint = myTestResult.Series.Points(i)
            Dim xValue As Double = dp.XValue
            Dim yValue As Double = dp.YValues(0)
            Chart1.Series(1).Points.Clear()
            Chart1.Series(1).Points.AddXY(xValue, -180)
            Chart1.Series(1).Points.AddXY(xValue, 0)
            ' myTestResult.Series.Points(i).MarkerSize = 5
        Else
            Me.Cursor = Cursors.Default
        End If

        'Dim g As Graphics = Chart1.CreateGraphics
        'Chart1.Invalidate()
        'g.DrawLine(Pens.Red, New Point(e.X, 0), New Point(e.X, Chart1.Height))
        'g.Save()
        'Chart1.ChartAreas(0).CursorX.SetCursorPixelPosition(New Point(e.X, e.Y), True)
        'Chart1.ChartAreas(0).CursorY.SetCursorPixelPosition(New Point(e.X, e.Y), True)


    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
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
            TextBox5.Text = "400,410.125,420.150,430.175"
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

    Private Sub LinkLabel4_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        If Chart1.Series.Count >= 4 Then Chart1.Series(3).Points.Clear()
    End Sub

    Private Sub LinkLabel3_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)

    End Sub

    Private Sub Chart1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub 选中作为时序Mark点ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 选中作为时序Mark点ToolStripMenuItem.Click
        If Chart1.Series.Count >= 3 Then
            If Chart1.Series(1).Points.Count >= 1 Then
                Dim xValue As Double = Chart1.Series(1).Points(0).XValue
                TimeFreqPoint = xValue
                'For Each ppt In Series.Points
                '    If ppt.XValue = xValue Then
                '        Chart1.Series(2).Points.Clear()
                '        Chart1.Series(2).Points.AddXY(xValue, ppt.YValues(0))
                '        Exit For
                '    End If
                'Next
            End If
        End If
    End Sub

    Private Sub RadioButton6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        'Dim c As New Chart
        'c.Width = 200
        'c.Height = 200
        'c.Left = 0
        'c.Top = 0
        'c.Dock = DockStyle.Fill
        '' c.BackColor = Color.Black
        'c.ChartAreas.Add("0")
        'c.ChartAreas(0).AxisY.Maximum = -20
        'c.ChartAreas(0).AxisY.Minimum = -120
        'c.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        'c.ChartAreas(0).AxisY.Interval = 20
        'Dim Series As New Series("频谱")
        'Series.Label = ""
        'Series.XValueType = ChartValueType.Auto
        'Series.ChartType = SeriesChartType.FastLine
        'Series.IsVisibleInLegend = False
        'Series.Color = Color.Blue
        'Series.Name = "频谱"
        'Series.LabelToolTip = "频率：#VALX 场强：#VAL"
        'Series.ToolTip = "频率：#VALX 场强：#VAL"
        'Series.Points.AddXY(88, -50)
        'Series.Points.AddXY(98, -50)
        'c.Series.Add(Series) '0
        '' c.BringToFront()
        'Panel43.Controls.Add(c)
    End Sub



    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim dataType As String = "task"
        Dim funcType As String = "getheartbeat"
        Dim paraMsg As String = ""
        Dim str As String = "?func=tssOrderByCode&dataType=" & dataType & "&funcType=" & funcType & "&paraMsg=" & paraMsg & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim w As New WarnBox("命令下发成功！")
        w.Show()
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

    Private Sub LinkLabel2_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked

    End Sub

    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox2.CheckedChanged

    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged

    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        If RadioButton4.Checked Then
            '中频分析      
            Dim span As Double = Val(TextBox2.Text)
            Dim count As Integer = 800
            Dim freqStep As Double = span * 1.25
            '  TextBox3.Text = freqStep
        End If
        If RadioButton3.Checked Then
            '频段分析
            Dim span As Double = Val(TextBox2.Text) - Val(TextBox1.Text)
            Dim count As Integer = 800
            Dim freqStep As Double = span * 1.25
            ' TextBox3.Text = freqStep
        End If
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged
        If RadioButton4.Checked Then
            '中频分析      
            Dim span As Double = Val(TextBox2.Text)
            Dim count As Integer = 800
            Dim freqStep As Double = span * 1.25
            freqStep = 25
            ' TextBox3.Text = freqStep
        End If
        If RadioButton3.Checked Then
            '频段分析
            Dim span As Double = Val(TextBox2.Text) - Val(TextBox1.Text)
            Dim count As Integer = 800
            Dim freqStep As Double = span * 1.25
            freqStep = 25
            ' TextBox3.Text = freqStep
        End If
    End Sub

    Private Sub Panel34_Paint_1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel34.Paint

    End Sub

    Private Sub PictureBox9_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox9.Click
        StopDevice()
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        Dim list As New List(Of ListViewItem)
        For Each itm In LV20.Items
            list.Add(itm)
        Next
        Dim colCount As Integer = LV20.Columns.Count
        Dim SFD As New SaveFileDialog
        SFD.Filter = "*.docx|*.docx"
        Dim result = SFD.ShowDialog
        If result = DialogResult.OK Or result = DialogResult.Yes Then
            Dim path As String = SFD.FileName
            If File.Exists(path) Then File.Delete(path)
            Dim doc As Novacode.DocX = Novacode.DocX.Create(path)

            Dim startTime As String = list(0).SubItems(6).Text
            Dim endTime As String = list(0).SubItems(7).Text
            doc.InsertParagraph.AppendLine(" ● 信号信息统计表[" & startTime & " To " & endTime & "]").Font(New FontFamily("宋体")).FontSize(15)
            Dim tab As Novacode.Table = doc.AddTable(list.Count + 1, colCount - 2)
            tab.Design = Novacode.TableDesign.TableGrid
            tab.Alignment = Novacode.Alignment.center
            For i = 0 To colCount - 3
                tab.Rows(0).Cells(i).FillColor = Color.FromArgb(226, 226, 226)
            Next
            For i = 0 To 5
                Dim cName As String = LV20.Columns(i).Text
                tab.Rows(0).Cells(i).Paragraphs(0).Append(cName).Bold().FontSize(7)
            Next
            For i = 6 To colCount - 3
                Dim cName As String = LV20.Columns(i + 2).Text
                tab.Rows(0).Cells(i).Paragraphs(0).Append(cName).Bold().FontSize(7)
            Next
            For i = 0 To list.Count - 1
                Dim itm As ListViewItem = list(i)
                For j = 0 To 5
                    Dim cValue As String = itm.SubItems(j).Text
                    tab.Rows(i + 1).Cells(j).Paragraphs(0).Append(cValue).FontSize(7)
                Next
                For j = 6 To colCount - 3
                    Dim cValue As String = itm.SubItems(j + 2).Text
                    tab.Rows(i + 1).Cells(j).Paragraphs(0).Append(cValue).FontSize(7)
                Next
            Next
            doc.InsertParagraph.InsertTableAfterSelf(tab)
            doc.Save()
            MsgBox("已导出为Word文件")
        End If
    End Sub

    Private Sub PictureBox5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox5.Click
        Dim list As New List(Of ListViewItem)
        For Each itm In LV20.Items
            list.Add(itm)
        Next
        Dim excel As New ExcelPackage
        Dim exSheet As ExcelWorksheet = excel.Workbook.Worksheets.Add("信号表")
        Dim colCount As Integer = LV20.Columns.Count
        For i = 0 To colCount - 1
            exSheet.Cells(1, i + 1).Value = LV20.Columns(i).Text
        Next
        For i = 0 To list.Count - 1
            Dim itm As ListViewItem = list(i)
            For j = 0 To colCount - 1
                exSheet.Cells(i + 2, j + 1).Value = itm.SubItems(j).Text
            Next
        Next
        Dim SFD As New SaveFileDialog
        SFD.Filter = "*.xlsx|*.xlsx"
        Dim result = SFD.ShowDialog
        If result = DialogResult.OK Or result = DialogResult.Yes Then
            Dim path As String = SFD.FileName
            If File.Exists(path) Then File.Delete(path)
            excel.SaveAs(New FileInfo(path))
            MsgBox("已导出为Excel文件")
        End If
    End Sub

    Private Sub PictureBox7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox7.Click
        Dim list As New List(Of ListViewItem)
        For Each itm In LV22.Items
            list.Add(itm)
        Next
        Dim colCount As Integer = LV22.Columns.Count
        Dim SFD As New SaveFileDialog
        SFD.Filter = "*.docx|*.docx"
        Dim result = SFD.ShowDialog
        If result = DialogResult.OK Or result = DialogResult.Yes Then
            Dim path As String = SFD.FileName
            If File.Exists(path) Then File.Delete(path)
            Dim doc As Novacode.DocX = Novacode.DocX.Create(path)

            Dim startTime As String = list(0).SubItems(6).Text
            Dim endTime As String = list(0).SubItems(7).Text
            doc.InsertParagraph.AppendLine(" ● 信号信息统计表[" & startTime & " To " & endTime & "]").Font(New FontFamily("宋体")).FontSize(15)
            Dim tab As Novacode.Table = doc.AddTable(list.Count + 1, colCount - 2)
            tab.Design = Novacode.TableDesign.TableGrid
            tab.Alignment = Novacode.Alignment.center
            For i = 0 To colCount - 3
                tab.Rows(0).Cells(i).FillColor = Color.FromArgb(226, 226, 226)
            Next
            For i = 0 To 5
                Dim cName As String = LV22.Columns(i).Text
                tab.Rows(0).Cells(i).Paragraphs(0).Append(cName).Bold().FontSize(7)
            Next
            For i = 6 To colCount - 3
                Dim cName As String = LV22.Columns(i + 2).Text
                tab.Rows(0).Cells(i).Paragraphs(0).Append(cName).Bold().FontSize(7)
            Next
            For i = 0 To list.Count - 1
                Dim itm As ListViewItem = list(i)
                For j = 0 To 5
                    Dim cValue As String = itm.SubItems(j).Text
                    tab.Rows(i + 1).Cells(j).Paragraphs(0).Append(cValue).FontSize(7)
                Next
                For j = 6 To colCount - 3
                    Dim cValue As String = itm.SubItems(j + 2).Text
                    tab.Rows(i + 1).Cells(j).Paragraphs(0).Append(cValue).FontSize(7)
                Next
            Next
            doc.InsertParagraph.InsertTableAfterSelf(tab)
            doc.Save()
            MsgBox("已导出为Word文件")
        End If
    End Sub

    Private Sub PictureBox6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox6.Click
        Dim list As New List(Of ListViewItem)
        For Each itm In LV22.Items
            list.Add(itm)
        Next
        Dim excel As New ExcelPackage
        Dim exSheet As ExcelWorksheet = excel.Workbook.Worksheets.Add("信号表")
        Dim colCount As Integer = LV22.Columns.Count
        For i = 0 To colCount - 1
            exSheet.Cells(1, i + 1).Value = LV22.Columns(i).Text
        Next
        For i = 0 To list.Count - 1
            Dim itm As ListViewItem = list(i)
            For j = 0 To colCount - 1
                exSheet.Cells(i + 2, j + 1).Value = itm.SubItems(j).Text
            Next
        Next
        Dim SFD As New SaveFileDialog
        SFD.Filter = "*.xlsx|*.xlsx"
        Dim result = SFD.ShowDialog
        If result = DialogResult.OK Or result = DialogResult.Yes Then
            Dim path As String = SFD.FileName
            If File.Exists(path) Then File.Delete(path)
            excel.SaveAs(New FileInfo(path))
            MsgBox("已导出为Excel文件")
        End If
    End Sub

    Private Sub 调整坐标轴范围ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 调整坐标轴范围ToolStripMenuItem.Click
        Dim f As New SetChatInfo(Chart1)
        f.Show()
    End Sub

    Private Sub Label34_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label34.Click
        GetDeviceLog()
    End Sub

    Private Sub Panel56_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel56.Click
        GetDeviceLog()
    End Sub
    Private Sub GetDeviceLog()
        Dim th As New Thread(AddressOf th_GetDevLog)
        th.Start()
    End Sub
    Private Sub th_GetDevLog()
        Me.Invoke(Sub()
                      Label38.Visible = True
                  End Sub)

        If Label33.Text = "未选择" Or Label33.Text = "" Then
            Me.Invoke(Sub()
                          Label38.Visible = False
                          MsgBox("请选择设备")
                      End Sub)
            Return
        End If
        LVDeviceLog.Items.Clear()
        Dim nickName As String = Label33.Text
        Dim sdate As String = DTP.Value.ToString("yyyy-MM-dd")
        Dim edate As String = DTP2.Value.ToString("yyyy-MM-dd")
        Dim str As String = "func=GetDeviceLogByNickNameWithTimeRegion&nickname=" & nickName & "&startTime=" & sdate & "&endTime=" & edate
        Dim result As String = GetServerResult(str)

        Try
            Me.Invoke(Sub()
                          Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
                          If IsNothing(dt) Then Return
                          If dt.Rows.Count = 0 Then Return
                          LVDeviceLog.Items.Clear()
                          For i = 0 To dt.Rows.Count - 1
                              Dim row As DataRow = dt.Rows(i)
                              Dim itm As New ListViewItem(i + 1)
                              itm.SubItems.Add(row("DeviceID"))
                              itm.SubItems.Add(row("DeviceNickName"))
                              itm.SubItems.Add(row("Time"))
                              itm.SubItems.Add(row("Address"))
                              itm.SubItems.Add(row("Log"))
                              itm.SubItems.Add(row("Result"))
                              itm.SubItems.Add(row("Status"))
                              LVDeviceLog.Items.Add(itm)
                          Next
                          Label38.Visible = False
                      End Sub)

        Catch ex As Exception

        End Try
    End Sub

    Private Sub RadioButton5_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton5.CheckedChanged
        Panel43.Visible = RadioButton5.Checked
    End Sub

    Private Sub RadioButton6_CheckedChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton6.CheckedChanged
        Panel51.Visible = RadioButton6.Checked
    End Sub


    Private Sub LinkLabel3_LinkClicked_1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        If Chart1.Series.Count < 4 Then Return
        Dim Series As New Series("MoudleFreq")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Gray
        Series.Name = "MoudleFreq"
        Dim se As Series = Chart1.Series(0)
        For Each p In se.Points
            Series.Points.AddXY(p.XValue, p.YValues(0))
        Next
        Chart1.Series(3) = Series
    End Sub

    Private Sub LinkLabel4_LinkClicked_1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        If Chart1.Series.Count >= 4 Then Chart1.Series(3).Points.Clear()
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim result As String = GethWithToken(HttpMsgUrl, "func=GetScreenImage")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        Else
            MsgBox("命令下发失败!" & GetNorResult("msg", result))
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked Then
            Timer1.Start()
        Else
            Timer1.Stop()
        End If
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            GethWithToken(HttpMsgUrl, "func=GetScreenImage")
        Catch ex As Exception

        End Try
    End Sub

    Structure MouseClickInfo
        Dim type As Integer
        Dim x As Integer
        Dim y As Integer
        Sub New(ByVal _type As Integer, ByVal _x As Integer, ByVal _y As Integer)
            type = _type
            x = _x
            y = _y
        End Sub
    End Structure

    Private Sub PBXControlImagr_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PBXControlImagr.MouseClick
        Dim mcInfo As MouseClickInfo
        mcInfo.x = e.X
        mcInfo.y = e.Y
        If e.Button = Windows.Forms.MouseButtons.Left Then
            mcInfo.type = 1
        End If
        If e.Button = Windows.Forms.MouseButtons.Right Then
            mcInfo.type = 2
        End If
        Dim json As String = JsonConvert.SerializeObject(mcInfo)
        GethWithToken(HttpMsgUrl, "func=mouseClick&datamsg=" & json)
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Dim r = MsgBox("是否需要重启工控机？", MsgBoxStyle.YesNo, "提示")
        If r = MsgBoxResult.No Then Return
        Dim result As String = GethWithToken(HttpMsgUrl, "func=ReStartWindows")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        Else
            MsgBox("命令下发失败!" & GetNorResult("msg", result))
        End If
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim r = MsgBox("是否需要关闭工控机？关闭可能无法远程启动！", MsgBoxStyle.YesNo, "提示")
        If r = MsgBoxResult.No Then Return
        Dim result As String = GethWithToken(HttpMsgUrl, "func=ShutdownWindows")
        If GetNorResult("result", result) = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
        Else
            MsgBox("命令下发失败!" & GetNorResult("msg", result))
        End If
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Dim th As New Thread(AddressOf GetControlerVersion)
        th.Start()
    End Sub
    Private Sub GetControlerVersion()
        Me.Invoke(Sub() Label43.Text = "正在查询……")
        Dim result As String = GethWithToken(HttpMsgUrl, "func=GetControlerVersion")
        Me.Invoke(Sub() Label43.Text = result)
    End Sub

    Private Sub TextBox6_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox6.TextChanged
        Dim str As String = TextBox6.Text
        If InStr(str, ",") Then
            Dim st() As String = str.Split(",")
            TextBox6.Text = st(0)
            TextBox7.Text = st(1)
        End If
    End Sub

    Private Sub Label23_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub PictureBox8_Click(sender As Object, e As EventArgs) Handles PictureBox8.Click

    End Sub

    Private Sub PictureBox4_Click_1(sender As Object, e As EventArgs) Handles PictureBox4.Click

    End Sub

    Private Sub PictureBox2_Click_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub PictureBox5_Click_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub PictureBox14_Click(sender As Object, e As EventArgs) Handles PictureBox14.Click
        isLiSanSaoMiao = True
        Label45.Visible = True
        LV20.Items.Clear()
        lisanTime = Now
        Dim str As String = TextBox5.Text
        If str = "" Then
            MsgBox("请输入频点列表")
            Return
        End If
        Dim list As List(Of String) = str.Split(",").ToList()
        Dim dlist As New List(Of Double)
        For Each itm In list
            If IsNumeric(itm) = False Then
                MsgBox(itm & " 值无效")
                Return
            Else
                dlist.Add(Double.Parse(itm))
            End If
        Next
        dlist.Sort()
        Dim freqstart As Double = dlist(0)
        Dim freqEnd As Double = dlist(dlist.Count - 1)
        'If freqstart < 30 Then
        '    MsgBox("频点值不能小于30")
        '    Return
        'End If
        'If freqEnd > 6000 Then
        '    MsgBox("频点值不能大于6000")
        '    Return
        'End If

        freqStartTimeStr = ""
        TongJiCiShu = 0
        sigNalCount = 0
        iniPBX()
        signalStartTime = Nothing
        itmList = Nothing
        iniChart1()
        iniChart2()
        iniChart3()
        'StartFreq(freqstart, freqEnd, 25)
        'lisanFreqList = New List(Of Double)
        'For Each itm In list
        '    Dim d As Double = Double.Parse(itm)
        '    lisanFreqList.Add(d)
        'Next
        Dim th As New Thread(AddressOf SendLisanToServer)
        th.Start(dlist)
    End Sub
    Private Sub SendLisanToServer(list As List(Of Double))

        Dim p As New tssOrder_stu
        p.task = "lisan"
        p.data = JsonConvert.SerializeObject(list)
        Dim orderMsg As String = JsonConvert.SerializeObject(p)
        Label31.Visible = True
        Dim str As String = "?func=tssOrder&datamsg=" & orderMsg & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)

        Try
            Dim np As normalResponse = JsonConvert.DeserializeObject(Of normalResponse)(result)
            Label45.Visible = False
            Me.Invoke(Sub()
                          If np.result Then
                              Dim w As New WarnBox("命令下发成功！")
                              w.Show()
                          Else
                              If np.msg.Contains("Please login") Then
                                  Login()
                                  SendMsgToDev(orderMsg)
                              Else
                                  Dim sb As New StringBuilder
                                  sb.AppendLine("命令下发失败")
                                  sb.AppendLine(np.msg)
                                  MsgBox(sb.ToString)
                              End If
                          End If
                      End Sub)

        Catch ex As Exception
        End Try
    End Sub

    Private Sub PictureBox13_Click(sender As Object, e As EventArgs) Handles PictureBox13.Click
        StopDevice()
    End Sub
End Class



