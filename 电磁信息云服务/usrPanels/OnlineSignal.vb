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


Public Class OnlineSignal
    Public selectDeviceID As String
    Public realDeviceID As String
    Dim th_ReciveHttpMsg As Thread
    Private DeviceAddress As String
    Dim HttpMsgUrl As String
    Dim itmList As List(Of ListViewItem)
    Dim isTongJi As Boolean = True
    Private selectSignalBiaoPanel As List(Of SignalBiaoPanel)
    Dim TongJiCiShu As Integer = 0
    Dim freqStartTimeStr As String = ""
    Dim showTabIndex As Integer = 0
    Dim TimeFreqPoint As Double = 300
    Dim LV26Lock As New Object
    Dim LV20Lock As New Object
    Structure json_PPSJ
        Dim freqStart As Double
        Dim freqStep As Double
        Dim deviceID As String
        Dim dataCount As Integer
        Dim value() As Double
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
        'TabPage1.Text = ""
        'TabPage2.Text = ""
        'TabPage3.Text = ""
        'TabPage5.Text = ""

    End Sub
    Private Sub OnlineSignal_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        Control.CheckForIllegalCrossThreadCalls = False
        TextBox4.Text = selectDeviceID
        ini()
        Label33.Text = selectDeviceID
        DTP.Value = Now.AddDays(-1)
        DTP2.Value = Now
        TabControl1.SelectedIndex = showTabIndex
        th_ReciveHttpMsg = New Thread(AddressOf ReciveHttpMsg)
        th_ReciveHttpMsg.Start()
        GetDeviceLog()
    End Sub
    Public Sub stopALL()
        Try
            th_ReciveHttpMsg.Abort()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ini()
        TextBox1.Text = 300
        TextBox2.Text = 400
        TextBox3.Text = 200
        'iniOpacity()
        'iniCenterSetting()
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = selectDeviceID Then
                    realDeviceID = itm.DeviceID
                    TextBox4.Text = itm.Name
                    TextBox6.Text = itm.Lng
                    TextBox7.Text = itm.Lat
                    TextBox8.Text = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        PictureBox1.Cursor = Cursors.Hand
        PictureBox3.Cursor = Cursors.Hand
        iniLV()
        iniChart()
        RDSignal.Checked = True
        ComboBox1.Items.Add("自定义频率")
        ComboBox1.Items.Add("2018广州马拉松")

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
    End Sub
    Private Sub iniLV()
        LV20.Clear()
        LV20.View = View.Details
        LV20.GridLines = False
        LV20.FullRowSelect = True
        LV20.Columns.Add("序号", 50)
        LV20.Columns.Add("时间", 150)
        LV20.Columns.Add("频率(MHz)", 70)
        LV20.Columns.Add("地点", 100)
        LV20.Columns.Add("信号电平", 100)
        LV20.Columns.Add("最小值")
        LV20.Columns.Add("最大值")
        LV20.Columns.Add("出现次数")
        LV20.Columns.Add("平均值")
        LV20.Columns.Add("统计时长")
        LV20.Columns.Add("占用度")
        LV20.Columns.Add("监测次数")
        LV20.Columns.Add("超标次数")

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
        LV26.Columns.Add("模板值", 80)  '14

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
        inichart4()
        inichart5()
        inichart6()
    End Sub
    Private Sub inichart4()
        Chart4.Series.Clear()
        'Chart4.ChartAreas(0).CursorX.IsUserEnabled = True
        'Chart4.ChartAreas(0).CursorY.IsUserEnabled = True
        Chart4.ChartAreas(0).AxisY.Maximum = -20
        Chart4.ChartAreas(0).AxisY.Minimum = -120
        Chart4.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart4.ChartAreas(0).AxisY.Interval = 20
        'Chart4.ChartAreas(0).AxisX.Maximum = 0
        'Chart4.ChartAreas(0).AxisX.Minimum = 800
        'Chart4.ChartAreas(0).AxisX.Interval = 100
        Dim Series As New Series("频谱")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        Series.Name = "频谱"
        Series.LabelToolTip = "频率：#VALX 场强：#VAL"
        Series.ToolTip = "频率：#VALX 场强：#VAL"
        Chart4.Series.Add(Series) '0

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
        Chart4.Series.Add(Series) '2
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
        Chart4.Series.Add(Series) '3

        Series = New Series("MoudleFreq")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Gray
        Series.Name = "MoudleFreq"
        Chart4.Series.Add(Series) '1
    End Sub
    Private Sub inichart5()
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
        Chart5.Series.Add(Series) '0  频谱层
        Series = New Series("illegalsignal")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.Column
        Series.IsVisibleInLegend = False
        Series.Color = Color.Red
        Series.Name = ""
        Chart5.Series.Add(Series) '1  信号层
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
        Chart5.Series.Add(Series) '2  直方图层

    End Sub
   
    Private Sub inichart6()
        Chart6.Series.Clear()
        Chart6.ChartAreas(0).AxisY.Maximum = 40
        Chart6.ChartAreas(0).AxisY.Minimum = -120
        Chart6.ChartAreas(0).AxisY.Interval = 20
        Chart6.ChartAreas(0).AxisY.IntervalOffset = 20
        ' chart6.ChartAreas(0).BorderColor = Color.Yellow
        'chart6.ChartAreas(0).BackColor = Color.Black
        Chart6.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        'chart6.ChartAreas(0).AxisY.IsReversed = True
        'chart6.ChartAreas(0).AxisX.Enabled = AxisEnabled.False
        'chart6.ChartAreas(0).AxisX2.Enabled = AxisEnabled.True
        Dim Series As New Series '频谱   0
        Series.Label = "频谱数据"
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        'Series.ToolTip = "频率：#VALX\\n场强：#VAL"
        'Series.LabelToolTip = "频率：#VALX\\n场强：#VAL"
        Chart6.Series.Add(Series)
        Dim ser As New Series("illegalsignal") '信号   1
        ser.ChartType = SeriesChartType.Column
        ser.Color = Color.Red
        ser("PointWidth") = 0.25
        ser.IsVisibleInLegend = False
        'ser.ToolTip = "频率：#VALX\\n场强：#VAL"
        'ser.LabelToolTip = "频率：#VALX\\n场强：#VAL"
        Chart6.Series.Add(ser)
        Dim Series2 As New Series '最大值   2
        Series2.Label = "最大值"
        Series2.XValueType = ChartValueType.Auto
        Series2.ChartType = SeriesChartType.FastLine
        Series2.IsVisibleInLegend = False
        Chart6.Series.Add(Series2)
        Dim Series3 As New Series '状态跟踪   3
        Series3.Label = "状态跟踪"
        Series3.XValueType = ChartValueType.Auto
        Series3.ChartType = SeriesChartType.StepLine
        Series3.IsVisibleInLegend = False
        Chart6.Series.Add(Series3)
        Dim series4 As New Series '状态跟踪   3
        series4.Label = "状态跟踪"
        series4.XValueType = ChartValueType.Auto
        series4.ChartType = SeriesChartType.StepLine
        series4.IsVisibleInLegend = False
        Chart6.Series.Add(series4)
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
        'Console.WriteLine("HttpMsgUrl=" & HttpMsgUrl)
        While True
            Dim result As String = GetHttpMsg()
            If result = "" Then Continue While
            HandleHttpMsg(result)
        End While
    End Sub
    Private Function GetHttpMsg() As String
        Try
            Dim req As HttpWebRequest = WebRequest.Create(HttpMsgUrl & "?func=GetDevMsg")
            'Me.Invoke(Sub() MsgBox(HttpMsgUrl & "?func=GetDevMsg"))
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
    Dim DrawThread As Thread
    Private Sub HandleHttpMsg(ByVal HttpMsg As String)
        'Console.WriteLine("收到新消息TZBQ  " & Now.ToString("HH:mm:ss") & " ")

        Dim TZBQList As New List(Of String)
        Try
            Dim p As JArray = JArray.Parse(HttpMsg)
            'Console.WriteLine(p.Count)
            For Each itm As JValue In p
                Dim jMsg As String = itm.Value
                If jMsg <> "" Then
                    TZBQList.Add(jMsg)
                End If
            Next
        Catch ex As Exception
            'Console.WriteLine(ex.ToString)
            Exit Sub
        End Try

        If TZBQList.Count <= 0 Then Exit Sub
        'handleTZBQList(TZBQList)
        If IsNothing(DrawThread) = False Then
            Try
                DrawThread.Abort()
            Catch ex As Exception

            End Try
        End If
        DrawThread = New Thread(AddressOf handleTZBQList)
        DrawThread.Start(TZBQList)
    End Sub
    Dim DisPlayLock As Object
    Private Sub handleTZBQList(ByVal bqlist As List(Of String))
        If IsNothing(DisPlayLock) Then DisPlayLock = New Object
        Dim maxCount As Integer = bqlist.Count
        Dim count As Integer = bqlist.Count
        If count > maxCount Then count = maxCount
        Dim sleepCount As Double = (GetHttpMsgTimeSpan * 1000 - 100) / (count + 1)

        SyncLock DisPlayLock
            For i = 0 To count - 1
                Try
                    If i < maxCount Then
                        Dim bq As String = bqlist(i)
                        Me.Invoke(Sub() handleBQ(bq))
                    End If
                Catch ex As Exception

                End Try
                If i < count - 1 Then
                    Sleep(sleepCount)
                End If
            Next
        End SyncLock
    End Sub

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
    Dim maxYY() As Double
    Dim isJianMu As Boolean = False
    Dim HH As Integer
    Dim MM As Integer
    Dim SS As Integer
    Dim SSSJObject As Object
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
    Private Sub sendMsgToDev(ByVal msg As String)

        Dim th As New Thread(AddressOf th_SendMsgToDev)
        th.Start(msg)
    End Sub
    Private Sub th_SendMsgToDev(ByVal msg As String)
        Dim str As String = "?func=tzbqOrder&datamsg=" & msg & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim r As String = GetNorResult("result", result)
        Dim msgt As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        Me.Invoke(Sub()
                      If r = "success" Then
                          Dim w As New WarnBox("命令下发成功！")
                          w.Show()
                      Else
                          If msgt = "Please login" Then
                              Login()
                              sendMsgToDev(msg)
                          Else
                              Dim sb As New StringBuilder
                              sb.AppendLine("命令下发失败")
                              sb.AppendLine(msgt)
                              sb.AppendLine(errmsg)
                              MsgBox(sb.ToString)
                          End If

                      End If
                  End Sub)

        'Console.WriteLine(result)
    End Sub

    Private Sub handleBQ(ByVal BQ As String)
        If InStr(BQ, "<TZBQ:") Then Else Exit Sub
        'Console.WriteLine(BQ)
        Dim a As Integer = InStr(BQ, "<")
        Dim b As Integer = InStr(BQ, Chr(13))
        BQ = Mid(BQ, a, b - 1 + 1)
        Dim func As String = getFuncByBQ(BQ)
        Dim id As String = getIDbyBQ(BQ)
        'If id <> selectDeviceID Then Exit Sub
        If InStr(func, "ECHO_") Then
            Dim k As String = func.Split("_")(1)
            If k = "TIME" Then
                Me.Invoke(Sub() MsgBox("时间同步成功！"))
            End If
            If k = "JCPD" Then
                'Console.WriteLine(BQ)
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
            sendMsgToDev("<TZBQ:SBZT,0>")
        End If
        If func = "JCMB" Then
            'Label16.Text = "模板建立完成!"
            'Me.Invoke(Sub() MsgBox("模板建立完成！"))
            Dim str As String = BQ.Substring(InStr(BQ, "<"), InStr(BQ, ">") - InStr(BQ, "<") - 1)
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
            Dim st() As String = BQ.Split(",")
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
            Dim BCDstr As String = BQ.Split("]")(1)
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
            Me.Invoke(Sub() HandlePPSJList(js))
        End If
        If func = "SSSJ" Then
            Me.Invoke(Sub() HandleSSSJ(BQ))
        End If
    End Sub

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
            Panel1.Controls.Clear()
            selectSignalBiaoPanel = New List(Of SignalBiaoPanel)
            Dim tmpList As New List(Of SignalBiaoPanel)
            For j = SigNalList.Count - 1 To 0 Step -1
                Dim itm As Double = SigNalList(j)
                Dim p As New SignalBiaoPanel(itm)
                Panel1.Controls.Add(p)
                tmpList.Add(p)
            Next
            For j = tmpList.Count - 1 To 0 Step -1
                selectSignalBiaoPanel.Add(tmpList(j))
            Next
        Else
            If selectSignalBiaoPanel.Count <> SigNalList.Count Then
                Panel1.Controls.Clear()
                selectSignalBiaoPanel = New List(Of SignalBiaoPanel)
                Dim tmpList As New List(Of SignalBiaoPanel)
                For j = SigNalList.Count - 1 To 0 Step -1
                    Dim itm As Double = SigNalList(j)
                    Dim p As New SignalBiaoPanel(itm)
                    Panel1.Controls.Add(p)
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
                                      handleLV20(pd, cq, SigNalList)
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
                              Chart5.Series(2) = Series
                          Catch ex As Exception
                              ' 'Console.WriteLine(ex.ToString)
                          End Try
                      End Sub)
        End If
        My.Application.DoEvents()
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
    Private flagIsCreateMoudle As Boolean = False
    Private flagIsWatchSigNal As Boolean = False
    Private flagCreatedMoudle As Boolean = False
    Private illegalSigNanListLock As Object
    Private illegalSigNalList As List(Of illegalSigNalInfo)
    Structure illegalSigNalInfo
        Dim freq As Double
        Dim type As String
        Dim illegalCount As Integer
        Dim isHandled As Boolean
        Sub New(freq As Double, type As String, isHandled As Boolean)
            Me.type = type
            Me.freq = freq
            Me.isHandled = isHandled
            illegalCount = 1
        End Sub
    End Structure
    Private Sub LinkLabel3_LinkClicked_2(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        If flagIsCreateMoudle Then
            ''按下 关闭建模按钮
            flagCreatedMoudle = True
            LinkLabel3.Text = "开始建模"
            flagIsCreateMoudle = False
        Else
            ''按下 开始建模按钮
            flagCreatedMoudle = False
            LinkLabel3.Text = "关闭建模"
            flagIsCreateMoudle = True
        End If
    End Sub

    Private Sub LinkLabel4_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        If flagIsWatchSigNal Then
            ''按下 关闭监督状态
            LinkLabel4.Text = "开始监督状态"
            flagIsWatchSigNal = False
        Else
            ''按下 开始监督状态
            If IsNothing(illegalSigNanListLock) Then illegalSigNanListLock = New Object
            SyncLock illegalSigNanListLock
                illegalSigNalList = New List(Of illegalSigNalInfo)
            End SyncLock
            flagCreatedMoudle = True
            LinkLabel4.Text = "关闭监督状态"
            flagIsWatchSigNal = True
        End If
    End Sub
    Private Sub handleLv26(ByVal pd As Double, ByVal cq As Double, ByVal SigNalList As List(Of Double))
        Try
            Dim isCreateMoudle As Boolean = flagIsCreateMoudle
            Dim isWatchSigNal As Boolean = flagIsWatchSigNal
            Dim isCreatedMoudle As Boolean = flagCreatedMoudle
            '   Console.WriteLine("isWatchSigNal=" & isWatchSigNal)
            If IsNothing(SigNalList) Then Exit Sub
            Dim colCount As Integer = LV26.Columns.Count
            Dim defaultMoudle As Double = -60
            Dim moudleRegion As Integer = 10
            If LV26.Items.Count <> SigNalList.Count Then
                LV26.Items.Clear()
                For Each it In SigNalList
                    Dim itm As New ListViewItem(it)
                    For i = 1 To colCount - 1
                        itm.SubItems.Add("--")
                    Next
                    itm.SubItems(14).Text = defaultMoudle '模板值
                    itm.SubItems(13).Text = 0
                    itm.SubItems(12).Text = 0
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
                    'itm.ForeColor = Color.Black
                    Dim moudleValue As Double = Val(itm.SubItems(14).Text)
                    If moudleValue >= 0 Then moudleValue = defaultMoudle
                    If isCreateMoudle Then
                        If moudleValue < cq Then
                            moudleValue = cq
                            itm.SubItems(14).Text = moudleValue
                        End If
                    Else
                        If Not isWatchSigNal Then
                            If Not isCreatedMoudle Then
                                itm.SubItems(14).Text = cq
                            End If
                        End If
                    End If
                    itm.SubItems(4).Text = "可用"
                    If isWatchSigNal Then
                        '  Console.WriteLine("isWatchSigNal=" & isWatchSigNal & "moudleValue=" & moudleValue & ",cq=" & cq)
                        If cq >= moudleValue + moudleRegion Then
                            statu = "超标"
                            itm.ForeColor = Color.Red
                            itm.SubItems(4).Text = "不可用"
                        End If
                        If cq <= moudleValue - moudleRegion Then
                            statu = "故障"
                            itm.ForeColor = Color.Goldenrod
                            itm.SubItems(4).Text = "可用"
                        End If
                        If cq > moudleValue - moudleRegion And cq <= moudleValue Then
                            statu = "闲置"
                            itm.ForeColor = Color.Gray
                            itm.SubItems(4).Text = "可用"
                        End If

                        itm.SubItems(2).Text = "不明信号"
                        itm.SubItems(3).Text = statu

                        itm.SubItems(5).Text = "0%"
                        itm.SubItems(6).Text = sssjStartTime
                        itm.SubItems(7).Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
                        itm.SubItems(12).Text = Val(itm.SubItems(12).Text) + 1

                        If cq >= defaultMoudle Then
                            itm.SubItems(13).Text = Val(itm.SubItems(13).Text) + 1
                        End If
                        Dim illegalCount As Integer = itm.SubItems(13).Text
                        Dim zyd As String = ((Val(itm.SubItems(13).Text) / Val(itm.SubItems(12).Text)) * 100).ToString("0.00") & "%"
                        itm.SubItems(5).Text = zyd
                        If True Then
                            SyncLock illegalSigNanListLock
                                Dim isFind As Boolean = False
                                For i = 0 To illegalSigNalList.Count - 1
                                    Dim signal As illegalSigNalInfo = illegalSigNalList(i)
                                    If signal.freq = pd Then
                                        If signal.type = statu Then
                                            isFind = True
                                            signal.illegalCount = signal.illegalCount + 1
                                        Else
                                            signal.type = statu
                                            signal.isHandled = False
                                            signal.illegalCount = 1
                                            isFind = True
                                        End If
                                        If signal.illegalCount > 10 Then
                                            If signal.isHandled = False Then
                                                signal.isHandled = True
                                                If pd = 456 Then
                                                    'Console.WriteLine(" signal.illegalCount=" & signal.illegalCount & ", signal.isHandled=" & signal.isHandled & ",statu=" & statu)
                                                End If
                                                If statu = "超标" Or statu = "故障" Then
                                                    Dim speekMsg As String = statu & "预警，频率" & pd & "MHz，占用度" & zyd
                                                    speek(speekMsg)
                                                    UploadAlarmToServer(statu & "预警", speekMsg)
                                                    '  MsgBox(speekMsg)
                                                End If
                                            End If
                                        End If

                                        illegalSigNalList(i) = signal
                                        Exit For
                                    End If
                                Next
                                If Not isFind Then
                                    illegalSigNalList.Add(New illegalSigNalInfo(pd, statu, False))
                                End If
                            End SyncLock
                        End If
                    End If

                    Dim timespan As TimeSpan = Now.Subtract(sssjStartTime)
                    itm.SubItems(8).Text = timespan.Hours.ToString("00") & ":" & timespan.Minutes.ToString("00") & ":" & timespan.Seconds.ToString("00")
                End If
            Next
        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try

    End Sub
    Private Sub UploadAlarmToServer(alarmKind As String, alarmContent As String)
        Dim Time As String = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim Cata As String = "预警上报"
        Dim Kind As String = alarmKind
        Dim Content As String = alarmContent
        Dim usr As String = Module1.usr
        Dim DeviceNickName As String = selectDeviceID
        Dim DeviceID As String = GetDeviceIDByName(selectDeviceID)
        Dim TaskNickName As String = "日常监测"
        Dim s As logstu
        s.Time = Time
        s.Cata = Cata
        s.Kind = Kind
        s.Content = Content
        s.Usr = usr
        s.DeviceNickName = DeviceNickName
        s.DeviceID = DeviceID
        s.TaskNickName = TaskNickName
        Dim json As String = JsonConvert.SerializeObject(s)

        Dim p As PostStu
        p.func = "AddLog"
        p.msg = json
        p.token = token

        Dim result As String = PostServerResult(JsonConvert.SerializeObject(p))
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        'If r = "success" Then
        '    Dim w As New WarnBox("录入成功")
        '    w.Show()
        '    GetLogList()
        'Else
        '    Dim sb As New StringBuilder
        '    sb.AppendLine("录入失败")
        '    sb.AppendLine(msg)
        '    sb.AppendLine(errmsg)
        '    MsgBox(sb.ToString)
        'End If
    End Sub
    Private Sub handleLV20(ByVal pd As Double, ByVal cq As Double, ByVal SigNalList As List(Of Double))
        Try
            If IsNothing(SigNalList) Then Exit Sub
            If LV20.Items.Count <> SigNalList.Count Then
                LV20.Items.Clear()
                For Each it In SigNalList
                    Dim itm As New ListViewItem(it)
                    For i = 1 To LV20.Columns.Count - 1
                        itm.SubItems.Add("--")
                    Next
                    itm.SubItems(itm.SubItems.Count - 1).Text = 0
                    itm.SubItems(itm.SubItems.Count - 2).Text = 0
                    LV20.Items.Add(itm)
                Next
            End If
            For Each itm As ListViewItem In LV20.Items
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
    Private Sub HandlePPSJList(ByVal itm As json_PPSJ)

        Try
            Me.Invoke(Sub() handlePinPuFenXi(itm))
        Catch ex As Exception
            'MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub handlePinPuFenXi(ByVal p As json_PPSJ)
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
        Dim jieshu As Double = freqStart + (dataCount - 1) * freqStep
        Dim Series As New Series("频谱")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        Series.Name = "频谱"
        Chart4.ChartAreas(0).AxisX.Minimum = freqStart
        Chart4.ChartAreas(0).AxisX.Maximum = jieshu
        Chart4.ChartAreas(0).AxisX.Interval = (jieshu - freqStart) / 5

        Chart6.ChartAreas(0).AxisX.Minimum = freqStart
        Chart6.ChartAreas(0).AxisX.Maximum = jieshu
        Chart6.ChartAreas(0).AxisX.Interval = (jieshu - freqStart) / 5
        For i = 0 To dataCount - 1
            Series.Points.AddXY(xx(i), yy(i))
        Next
        If Chart4.Series.Count = 0 Then
            Chart4.Series.Add(Series)
        Else
            Chart4.Series(0) = Series
        End If
        If Chart4.Series.Count >= 3 Then
            If Chart4.Series(1).Points.Count >= 1 Then
                Dim xValue As Double = Chart4.Series(1).Points(0).XValue
                For Each ppt In Series.Points
                    If ppt.XValue = xValue Then
                        Chart4.Series(2).Points.Clear()
                        Chart4.Series(2).Points.AddXY(xValue, ppt.YValues(0))
                        Exit For
                    End If
                Next
            End If
        End If
        If RDFreq.Checked Then
            Chart5.ChartAreas(0).AxisX.Minimum = freqStart
            Chart5.ChartAreas(0).AxisX.Maximum = jieshu
            Chart5.ChartAreas(0).AxisX.Interval = (jieshu - freqStart) / 5
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
                HandleMuBan(xx, yy)
                If isTongJi Then
                    If TongJiCiShu <= 30 Then
                        TongJiCiShu = TongJiCiShu + 1
                        XinHao2LV(result)
                        If Chart5.Series.Count >= 2 Then
                            Chart5.Series(1).Points.Clear()
                        End If
                        For Each itm As ListViewItem In LV20.Items
                            Dim pl As String = itm.SubItems(2).Text
                            Dim count As String = itm.SubItems(7).Text
                            Dim cq As String = itm.SubItems(6).Text
                            If IsNumeric(count) Then
                                If Val(count) >= 10 Then
                                    If Chart5.Series.Count >= 2 Then
                                        For j = 0 To xx.Count - 1 - jieti
                                            If xx(j) = pl Then
                                                If j >= jieti Then
                                                    For m = j - jieti To j + jieti
                                                        Dim value As Double = yy(m)
                                                        If value <= -120 Then
                                                            value = 100
                                                        End If
                                                        Me.Invoke(Sub() Chart5.Series(1).Points.AddXY(xx(m), value))
                                                    Next
                                                End If
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            'MsgBox(ex.ToString)
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
                    Chart6.Series(3) = Series2
                    Chart6.Series(4) = series3
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
                    ''Console.WriteLine("幅差：" & fucha)
                    ''Console.WriteLine("带宽：" & daikuan)
                    ''Console.WriteLine("小差：" & mincha)
                    Dim result(,) As Double = XinHaoFenLi2(xx, chayy, daikuan, fucha, mincha)
                    Dim ser As New Series("illegalsignal")
                    ser.ChartType = SeriesChartType.Column
                    ser("PointWidth") = 0.1
                    ser.Color = Color.Red
                    ser.IsVisibleInLegend = False
                    Chart6.Series(1).Points.Clear()
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
                                                'Console.WriteLine("捕获到新信号" & rx & "MHz")
                                                speek("捕获到新信号" & rx & "MHz")
                                            Catch ex As Exception
                                                ''Console.WriteLine(ex.ToString)
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
                                                Chart6.Series(1).Points.AddXY(xx(m), yy(m))
                                            Next
                                            Exit For
                                        End If
                                    Next
                                End If
                            Next
                        Next
                    End If
                Else
                    Chart6.Series(3).Points.Clear()
                    Chart6.Series(1).Points.Clear()
                End If
            Else
                Chart6.Series(3).Points.Clear()
                Chart6.Series(1).Points.Clear()
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
        Chart6.Series(0) = Series
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
                        Chart6.Series(2) = ser
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
                    ' itmList(k).SubItems(10).Text = "" '占用度
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
        SyncLock LV20Lock
            LV20.Items.Clear()
            Dim cnt As Integer = plist.Count
            For i = 0 To cnt - 1
                Dim itm As ListViewItem = plist(i)
                itm.Text = i + 1
                LV20.Items.Add(itm)
            Next
        End SyncLock


        Label145.Text = "信号数量: " & itmList.Count
    End Sub
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
                    '    'Console.WriteLine(vR & "," & vG & "," & vB)
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
                        '    'Console.WriteLine(vR & "," & vG & "," & vB)
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
                ' PBXLeft.Image = leftBmp
                'Dim rightColor As Color = bmp.GetPixel(bmp.Width - 1, 0)
                'MsgBox(rightColor.R & "," & rightColor.G & "," & rightColor.B)
                Dim rightBmp As New Bitmap(Panel45.Width, bmp.Height)
                Dim gright As Graphics = Graphics.FromImage(rightBmp)
                Dim rightBrush As Brush = New SolidBrush(leftColor)
                gright.FillRectangle(rightBrush, 0, 0, leftBmp.Width, recentY)
                gright.Save()
                'PBXRight.Image = rightBmp
                'Dim gp As Graphics = Panel43.CreateGraphics
                'gp.DrawString(Now.ToString("yyyy-MM-dd HH:mm:ss"), New Font("微软雅黑", 15), Brushes.Black, New Point(10, 10))
                'gp.Save()
            End If
        Catch ex As Exception
            ' MsgBox(ex.ToString)
        End Try
    End Sub
    Dim signalStartTime As Date = Nothing
    Dim sigNalCount As Integer = 0
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        Dim id As String = selectDeviceID
        sigNalCount = 0
        TongJiCiShu = 0
        sigNalCount = 0
        PBX.Image = Nothing
        signalStartTime = Nothing
        itmList = Nothing
        inichart4()
        inichart5()
        inichart6()
        Dim msg As String = "<TZBQ:SMBH," & id & "," & TextBox1.Text & "," & TextBox2.Text & "," & (TextBox3.Text) / 1000 & "," & 1 & "," & 0 & "," & 0 & "," & 1 & "," & 1 & ">"
        sendMsgToDev(msg)

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
    Private Sub stopDevice()
        Dim id As String = selectDeviceID
        sendMsgToDev("<TZBQ:STOP," & id & ">")
    End Sub
    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        stopDevice()
    End Sub

    Private Sub LinkLabel3_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
        AddTaskFrm.Show()
        AddTaskFrm.TabControl1.SelectedIndex = 1
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub chart6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

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

    Private Sub LinkLabel3_LinkClicked_1(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)

    End Sub

    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        Dim id As String = selectDeviceID
        Dim msg As String = "<TZBQ:TIME," & id & "," & TextBox9.Text & ">"
        sendMsgToDev(msg)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim id As String = selectDeviceID
        TextBox9.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        Dim msg As String = "<TZBQ:TIME," & id & "," & TextBox9.Text & ">"
        sendMsgToDev(msg)
    End Sub

    Private Sub Button24_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button24.Click
        Dim id As String = selectDeviceID
        Dim msg As String = "<TZBQ:SZID," & id & "," & TextBox8.Text & ">"
        sendMsgToDev(msg)
    End Sub

    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        Dim lng As String = TextBox6.Text
        Dim lat As String = TextBox7.Text
        Dim msg As String = "<TZBQ:SGPS," & selectDeviceID & "," & lng & "," & lat & ">"
        sendMsgToDev(msg)


        If Val(lng) < 100 Or Val(lat) < 5 Then MsgBox("请填写正确经纬度") : Return
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
        If nickid = "" Then
            MsgBox("备注不能为空")
            Return
        End If
        Dim str As String = "func=SetDeviceNickID&nickid=" & nickid
        Dim result As String = GethWithToken(HttpMsgUrl, str)
        If result = "" Then
            result = "设置备注成功！"
        End If
        MsgBox(result)
    End Sub

    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub RadioButton4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton4.CheckedChanged
        Label18.Text = "台站信息表"
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        Label18.Text = "信号评估表"
    End Sub

    Private Sub RadioButton6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton6.CheckedChanged
        Label1.Text = "起始频率"
        Label2.Visible = True
        TextBox3.Text = 25
        TextBox2.Visible = True
        Label5.Visible = True
        Label3.Text = "频率步进"
        Label6.Text = "KHz"
    End Sub

    Private Sub RadioButton5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton5.CheckedChanged
        Label1.Text = "中心频率"
        Label2.Visible = False
        TextBox3.Text = 5
        TextBox2.Visible = False
        Label5.Visible = False
        Label3.Text = "扫描宽度"
        Label6.Text = "MHz"
    End Sub

    Private Sub PictureBox12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox12.Click
        RadioButton3.Checked = True
        Dim q As New QuickFreq(Me)
        q.Show()
    End Sub

    Private Sub Panel1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)

    End Sub


    Private Sub PictureBox13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox13.Click
        Pic_Be_Click(0)
    End Sub
    Private Sub Pic_Be_Click(ByVal clickCount As Integer)
        Dim id As String = selectDeviceID
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
        Dim str As String = "?func=tzbqOrder&datamsg=" & jcpd & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim r As String = GetNorResult("result", result)
        Dim msgt As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        If r = "success" Then
            Sleep(800)
            sendMsgToDev("<TZBQ:SSSJ," & id & ",1>")
        Else
            If msgt = "Please login" Then
                Login()
                If clickCount = 0 Then
                    Pic_Be_Click(1)
                Else
                    MsgBox("登录信息失效，请关闭软件重新登录")
                    Return
                End If
            Else
                Dim sb As New StringBuilder
                sb.AppendLine("命令下发失败")
                sb.AppendLine(msgt)
                sb.AppendLine(errmsg)
                MsgBox(sb.ToString)
            End If

        End If
    End Sub

    Private Sub PictureBox11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox11.Click
        Dim id As String = selectDeviceID
        sendMsgToDev("<TZBQ:SSSJ," & id & ",0>")
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedItem = "自定义频率" Then
            TextBox5.Text = "310,432,456,462,530,610,630"
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

    Private Sub RDFreq_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RDFreq.CheckedChanged
        isTongJi = RDFreq.Checked
        Label7.Text = "信号表"
        For Each s In Chart5.Series
            s.Points.Clear()
        Next
        LV20.Columns.Clear()
        LV20.Items.Clear()
        LV20.Columns.Add("序号", 50)
        LV20.Columns.Add("时间", 150)
        LV20.Columns.Add("频率(MHz)", 70)
        LV20.Columns.Add("地点", 100)
        LV20.Columns.Add("信号电平", 100)
        LV20.Columns.Add("最小值")
        LV20.Columns.Add("最大值")
        LV20.Columns.Add("出现次数")
        LV20.Columns.Add("平均值")
        LV20.Columns.Add("统计时长")
        LV20.Columns.Add("占用度")
        LV20.Columns.Add("监测次数")
        LV20.Columns.Add("超标次数")

        Chart5.ChartAreas(0).AxisY.Maximum = -20
        Chart5.ChartAreas(0).AxisY.Minimum = -120
        Chart5.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart5.ChartAreas(0).AxisY.Interval = 20
        Chart5.ChartAreas(0).AxisX.Minimum = 80
        Chart5.ChartAreas(0).AxisX.Maximum = 108
        Chart5.ChartAreas(0).AxisX.Interval = 4
        Panel50.Visible = False
        Panel17.Visible = False
        Panel45.Visible = False
    End Sub

    Private Sub RDSignal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RDSignal.CheckedChanged
        isTongJi = False
        Label7.Text = "直方图"
        Panel45.Visible = True
        Panel17.Visible = True
        Panel50.Visible = True
        For Each s In Chart5.Series
            s.Points.Clear()
        Next
        LV20.Columns.Clear()
        LV20.Items.Clear()
        LV20.Columns.Add("信号频率(MHz)", 100)
        LV20.Columns.Add("实时电平(dBm)", 100)
        LV20.Columns.Add("属性识别", 70)
        LV20.Columns.Add("状态评估", 70)
        LV20.Columns.Add("可用评估", 70)
        LV20.Columns.Add("占用度", 60)
        LV20.Columns.Add("起始时间", 100)
        LV20.Columns.Add("更新时间", 100)
        LV20.Columns.Add("监测时长", 100)
        LV20.Columns.Add("最大电平(dBm)", 80)
        LV20.Columns.Add("平均电平(dBm)", 80)
        LV20.Columns.Add("最小电平(dBm)", 80)
        LV20.Columns.Add("监测次数", 80)
        LV20.Columns.Add("超标次数", 80)
        Chart5.ChartAreas(0).AxisY.Maximum = 100
        Chart5.ChartAreas(0).AxisY.Minimum = 0
        Chart5.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart5.ChartAreas(0).AxisY.Interval = 20
        Chart5.ChartAreas(0).AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount
        Chart5.ChartAreas(0).AxisX.Minimum = Double.NaN
        Chart5.ChartAreas(0).AxisX.Maximum = Double.NaN
        'Chart5.ChartAreas(0).AxisX.IsStartedFromZero = True
        Chart5.ChartAreas(0).AxisX.Interval = 1
    End Sub

    Private Sub PictureBox15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox15.Click
        Pic15_Be_Click()
    End Sub
    Private Sub Pic15_Be_Click()
        If RDSignal.Checked = False Then Return
        Dim id As String = selectDeviceID
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
        Dim str As String = "?func=tzbqOrder&datamsg=" & jcpd & "&token=" & token
        Dim result As String = GetH(HttpMsgUrl, str)
        Dim r As String = GetNorResult("result", result)
        Dim msgt As String = GetNorResult("msg", result)
        Dim errmsg As String = GetNorResult("errmsg", result)
        If r = "success" Then
            Sleep(800)
            sendMsgToDev("<TZBQ:SSSJ," & id & ",1>")
        Else
            If msgt = "Please login" Then
                Login()
                Pic15_Be_Click()
            Else
                Dim sb As New StringBuilder
                sb.AppendLine("命令下发失败")
                sb.AppendLine(msgt)
                sb.AppendLine(errmsg)
                MsgBox(sb.ToString)
            End If

        End If
    End Sub

    Private Sub PictureBox10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox10.Click
        Dim list As New List(Of ListViewItem)
        SyncLock LV26Lock
            For Each itm In LV26.Items
                list.Add(itm)
            Next
        End SyncLock
        Dim excel As New ExcelPackage
        Dim exSheet As ExcelWorksheet = excel.Workbook.Worksheets.Add("信号表")
        Dim colCount As Integer = LV26.Columns.Count
        For i = 0 To colCount - 1
            exSheet.Cells(1, i + 1).Value = LV26.Columns(i).Text
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

    Private Sub PictureBox9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox9.Click
        Dim list As New List(Of ListViewItem)
        SyncLock LV26Lock
            For Each itm In LV26.Items
                list.Add(itm)
            Next
        End SyncLock
        Dim colCount As Integer = LV26.Columns.Count
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
                Dim cName As String = LV26.Columns(i).Text
                tab.Rows(0).Cells(i).Paragraphs(0).Append(cName).Bold().FontSize(7)
            Next
            For i = 6 To colCount - 3
                Dim cName As String = LV26.Columns(i + 2).Text
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

    Private Sub PictureBox4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click
        Dim list As New List(Of ListViewItem)
        SyncLock LV20Lock
            For Each itm In LV20.Items
                list.Add(itm)
            Next
        End SyncLock
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

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        Dim list As New List(Of ListViewItem)
        SyncLock LV20Lock
            For Each itm In LV20.Items
                list.Add(itm)
            Next
        End SyncLock
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



    Private Sub Chart4_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Chart4.MouseMove
        Dim myTestResult As HitTestResult = Chart4.HitTest(e.X, e.Y)
        If myTestResult.ChartElementType = ChartElementType.DataPoint Then
            Me.Cursor = Cursors.Cross
            Dim i As Integer = myTestResult.PointIndex
            Dim dp As DataPoint = myTestResult.Series.Points(i)
            Dim xValue As Double = dp.XValue
            Dim yValue As Double = dp.YValues(0)
            Chart4.Series(1).Points.Clear()
            Chart4.Series(1).Points.AddXY(xValue, -120)
            Chart4.Series(1).Points.AddXY(xValue, -20)
            ' myTestResult.Series.Points(i).MarkerSize = 5
        Else
            Me.Cursor = Cursors.Default
        End If
    End Sub

    Private Sub 选中作为时序Mark点ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 选中作为时序Mark点ToolStripMenuItem.Click
        If Chart4.Series.Count >= 3 Then
            If Chart4.Series(1).Points.Count >= 1 Then
                Dim xValue As Double = Chart4.Series(1).Points(0).XValue
                TimeFreqPoint = xValue
                'For Each ppt In Series.Points
                '    If ppt.XValue = xValue Then
                '        Chart4.Series(2).Points.Clear()
                '        Chart4.Series(2).Points.AddXY(xValue, ppt.YValues(0))
                '        Exit For
                '    End If
                'Next
            End If
        End If
    End Sub

    Private Sub PictureBox6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox6.Click
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

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox2.TextChanged

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

End Class
