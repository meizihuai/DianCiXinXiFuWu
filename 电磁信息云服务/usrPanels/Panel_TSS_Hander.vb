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
Imports System.Net
Imports System.Math
Imports System.Windows.Forms.DataVisualization.Charting
Public Class Panel_TSS_Hander
    Public selectDeviceID As String
    Private HttpMsgUrl As String
    Private th_ReciveHttpMsg As Thread

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
    Sub New(ByVal DeviceID As String)
        InitializeComponent()
        selectDeviceID = DeviceID
    End Sub

    Private Sub Panel_TSS_Hander_ControlRemoved(ByVal sender As Object, ByVal e As System.Windows.Forms.ControlEventArgs) Handles Me.ControlRemoved
        Try
            If IsNothing(th_ReciveHttpMsg) = False Then
                th_ReciveHttpMsg.Abort()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Panel_TSS_Hander_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        Me.Dock = DockStyle.Fill
        Me.Invoke(Sub() iniChart())
        Me.DoubleBuffered = True
        th_ReciveHttpMsg = New Thread(AddressOf ReciveHttpMsg)
        th_ReciveHttpMsg.Start()
    End Sub
   
    Private Sub Panel_TSS_Hander_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Chart1.Width = Me.Width

    End Sub
    Private Sub iniChart()
        Chart1.BackColor = Color.White
        Chart1.ChartAreas(0).BackColor = Color.White
        Chart1.ChartAreas(0).AxisY.Maximum = -20
        Chart1.ChartAreas(0).AxisY.Minimum = -120
        Chart1.ChartAreas(0).AxisY.Interval = 20
        Chart1.ChartAreas(0).BorderColor = Color.Black
        Chart1.ChartAreas(0).AxisX.Maximum = 108
        Chart1.ChartAreas(0).AxisX.Minimum = 88
        Chart1.ChartAreas(0).AxisX.Interval = 5
        Chart1.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart1.ChartAreas(0).AxisX.IsStartedFromZero = True
        Chart1.ChartAreas(0).AxisX.IsMarginVisible = False
        Dim Series As New Series("频谱")
        Series.Color = Color.Blue
        Series.XValueType = ChartValueType.Auto
        Series.IsValueShownAsLabel = True
        Series.IsVisibleInLegend = False
        Series.ToolTip = "#VAL"
        Series.ChartType = SeriesChartType.Line
        Chart1.Series.Clear()
        Chart1.Series.Add(Series)
    End Sub
    Private Sub ReciveHttpMsg()
        HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & selectDeviceID & "&token=" & token)
        If GetResultPara("result", HttpMsgUrl) = "fail" Then
            If GetResultPara("msg", HttpMsgUrl) = "Please login" Then
                Login()
                HttpMsgUrl = GetH(ServerUrl, "func=GetHttpMsgUrlById&deviceID=" & selectDeviceID & "&token=" & token)
            End If
        End If
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
            req.Accept = "*/*"
            req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
            req.Timeout = 5000
            req.ReadWriteTimeout = 5000
            req.ContentType = "application/x-www-form-urlencoded"
            req.Method = "GET"
            Dim rp As HttpWebResponse = req.GetResponse
            Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd
            Return str
        Catch ex As Exception

        End Try
    End Function
    Private Sub HandleHttpMsg(ByVal HttpMsg As String)
        Console.WriteLine("收到新消息TSS  " & Now.ToString("HH:mm:ss"))
        Dim PPSJList As New List(Of json_PPSJ)
        Dim AudioList As New List(Of json_Audio)
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
                If func = "ifscan_wav" Then
                    Dim msg As String = JObj("msg").ToString
                    Dim audio As json_Audio = JsonConvert.DeserializeObject(msg, GetType(json_Audio))
                    AudioList.Add(audio)

                End If
            Next
        Catch ex As Exception
            Exit Sub
        End Try
        Dim th As New Thread(AddressOf HandlePPSJList)
        th.Start(PPSJList)

        'Dim th1 As New Thread(AddressOf HandleAudioList)
        'th1.Start(AudioList)

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
                    Console.WriteLine(itm.freqStart & "," & itm.freqStep & "," & itm.dataCount)
                    If itm.dataCount < 2000 Then
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
        Dim Series As New Series("")
        Series.Color = Color.Blue
        'Series.XValueType = ChartValueType.Auto
        Series.IsValueShownAsLabel = True
        Series.IsVisibleInLegend = False
        Series.ToolTip = "#VAL"
        Series.Color = Color.Blue
        Series.ChartType = SeriesChartType.FastLine
        For i = 0 To xx.Count - 1
            Series.Points.AddXY(xx(i), yy(i))
        Next
        Chart1.Series(0) = Series
    End Sub
   
End Class
