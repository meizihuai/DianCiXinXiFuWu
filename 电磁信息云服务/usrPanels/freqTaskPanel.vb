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
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Net
Imports System.Math
Public Class freqTaskPanel
    Private FreqTaskListDT As DataTable
    Private selectStartTime As String
    Private selectEndTime As String
    Private selectDeviceID As String
    Private selectFreqStart As Double
    Private selectFreqEnd As Double
    Private isPlay As Boolean = False
    Private playThread As Thread
    Private Sub freqTaskPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        ini()

        PictureBox2.Cursor = Cursors.Hand
        PictureBox3.Cursor = Cursors.Hand
        PictureBox4.Cursor = Cursors.Hand
        PictureBox5.Cursor = Cursors.Hand
        Label5.Cursor = Cursors.Hand
        Panel16.Cursor = Cursors.Hand
        Label6.Cursor = Cursors.Hand
        Panel17.Cursor = Cursors.Hand

        iniChart1()
        iniChart2()

        GetFreqTaskList()
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("项目", 60)
        LVDetail.Columns.Add("内容", 150)
        Dim itm As New ListViewItem("任务名称")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("任务类别")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("任务地点")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("执行设备")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("起始时间")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("结束时间")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("起始频率")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("终止频率")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("完成状态")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("信号数量")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("报告地址")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("下载报告")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)

        LVTask.View = View.Details
        LVTask.GridLines = True
        LVTask.FullRowSelect = True
        LVTask.Columns.Add("序号", 50)
        LVTask.Columns.Add("任务类别")
        LVTask.Columns.Add("任务名称", 200)
        LVTask.Columns.Add("设备ID", 150)
        LVTask.Columns.Add("开始时间", 150)
        LVTask.Columns.Add("结束时间", 150)
        LVTask.Columns.Add("完成状态", 150)
        LVTask.Columns.Add("下载报告", 500)

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
    End Sub
    Private Sub iniChart1()
      

        Chart1.Series.Clear()
        Chart1.ChartAreas(0).AxisY.Maximum = -20
        Chart1.ChartAreas(0).AxisY.Minimum = -120
        Chart1.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart1.ChartAreas(0).AxisY.Interval = 20
        Dim Series As New Series
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        Series.Color = Color.Blue
        Series.Name = ""
        Chart1.Series.Add(Series)
        Series = New Series("illegalsignal")
        Series.Label = ""
        Series.XValueType = ChartValueType.Auto
        '  Series.BorderWidth = "0.5"
        Series.ChartType = SeriesChartType.Column
        Series.IsVisibleInLegend = False
        Series.Color = Color.Red
        Series.Name = ""
        Chart1.Series.Add(Series)
        'Dim Series As New DataVisualization.Charting.Series
        'Series.Label = "频谱数据"
        'Series.XValueType = ChartValueType.Auto
        'Series.ChartType = SeriesChartType.FastLine
        'Series.IsVisibleInLegend = False
        'Chart1.Series.Add(Series)     
    End Sub
    Private Sub iniChart2()
        Chart2.Series.Clear()
        Chart2.ChartAreas(0).AxisY.Maximum = -20
        Chart2.ChartAreas(0).AxisY.Minimum = -120
        Chart2.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart2.ChartAreas(0).AxisY.Interval = 20
        Chart2.ChartAreas(0).AxisY.IntervalOffset = 20
        'Dim Series As New DataVisualization.Charting.Series
        'Series.Label = "频谱数据"
        'Series.XValueType = ChartValueType.Auto
        'Series.ChartType = SeriesChartType.FastLine
        'Series.IsVisibleInLegend = False
        'Chart2.Series.Add(Series)     
    End Sub
    Private Sub GetFreqTaskList()
        Label1.Text = "获取中……"
        Dim result As String = GetH(ServerUrl, "func=GetMyTask&token=" & token)
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetFreqTaskList()
            Exit Sub
        End If
        ' Panel_FreqTaskList.Controls.Clear()
        Label1.Text = "任务列表"
        If result = "[]" Then
            Exit Sub
        End If
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) = False Then
            Dim dv As DataView = dt.DefaultView
            dv.Sort = "StartTime Asc"
            Dim dt2 As DataTable = dv.ToTable
            FreqTaskListDT = New DataTable
            For Each col As DataColumn In dt2.Columns
                FreqTaskListDT.Columns.Add(col.ColumnName)
            Next
            'For Each row As DataRow In dt2.Rows
            '    Dim itm As New ListViewItem(LVTask.Items.Count + 1)
            '    itm.SubItems.Add(row("TaskName"))
            '    itm.SubItems.Add(row("TaskNickName"))
            '    itm.SubItems.Add(row("DeviceID"))
            '    itm.SubItems.Add(row("StartTime"))
            '    itm.SubItems.Add(row("EndTime"))
            '    itm.SubItems.Add(row("OverPercent"))
            '    itm.SubItems.Add(row("ResultReportUrl"))
            '    LVTask.Items.Add(itm)
            'Next
            For i = 0 To dt2.Rows.Count - 1
                Dim row As DataRow = dt2.Rows(i)
                If row("TaskName") = "频谱监测" Or row("TaskName") = "频谱取样" Or row("TaskName") = "占用统计" Or row("TaskName") = "黑广播捕获" Or row("TaskName") = "违章捕获" Then
                    Dim r As DataRow = FreqTaskListDT.NewRow
                    For j = 0 To dt2.Columns.Count - 1
                        r(j) = row(j)
                    Next
                    FreqTaskListDT.Rows.Add(r)
                    Dim index As Integer = FreqTaskListDT.Rows.Count - 1
                    Dim itm As New ListViewItem(LVTask.Items.Count + 1)
                    itm.SubItems.Add(row("TaskName"))
                    itm.SubItems.Add(row("TaskNickName"))
                    itm.SubItems.Add(row("DeviceID"))
                    itm.SubItems.Add(row("StartTime"))
                    itm.SubItems.Add(row("EndTime"))
                    itm.SubItems.Add(row("OverPercent"))
                    itm.SubItems.Add(row("ResultReportUrl"))
                    LVTask.Items.Add(itm)
                End If
            Next
            'For i = 0 To dt2.Rows.Count - 1
            '    Dim row As DataRow = dt2.Rows(i)
            '    If row("TaskName") = "频谱监测" Then
            '        Dim r As DataRow = FreqTaskListDT.NewRow
            '        For j = 0 To dt2.Columns.Count - 1
            '            r(j) = row(j)
            '        Next
            '        FreqTaskListDT.Rows.Add(r)
            '        Dim index As Integer = FreqTaskListDT.Rows.Count - 1
            '        Dim p As New freqTaskDetail(index)
            '        p.lbl_TaskTime.Text = row("StartTime")
            '        p.lbl_TaskName.Text = row("TaskName")
            '        p.lbl_TaskNickName.Text = row("TaskNickName")
            '        Dim TaskCode As String = row("TaskCode")
            '        Dim jb As JObject = JsonConvert.DeserializeObject(TaskCode, GetType(JObject))
            '        Dim freqStart As String = jb("freqStart").ToString
            '        Dim freqEnd As String = jb("freqEnd").ToString
            '        p.lbl_TaskCode.Text = "[" & freqStart & "MHz," & freqEnd & "MHz]"
            '        Panel_FreqTaskList.Controls.Add(p)
            '        AddHandler p.beClick, AddressOf freqTaskDetailBeClick
            '    End If
            'Next
            freqTaskDetailBeClick(FreqTaskListDT.Rows.Count - 1)
        End If

        Label1.Text = "任务列表"
    End Sub
    Private Sub freqTaskDetailBeClick(ByVal index As Integer)
        If index < 0 Then Return
        isPlay = False
        LVDetail.Items(0).SubItems(1).Text = FreqTaskListDT.Rows(index)("TaskNickName")
        LVDetail.Items(1).SubItems(1).Text = FreqTaskListDT.Rows(index)("TaskName")
        LVDetail.Items(2).SubItems(1).Text = "广州市开发区"
        LVDetail.Items(3).SubItems(1).Text = FreqTaskListDT.Rows(index)("DeviceID")
        LVDetail.Items(4).SubItems(1).Text = FreqTaskListDT.Rows(index)("StartTime")
        LVDetail.Items(5).SubItems(1).Text = FreqTaskListDT.Rows(index)("EndTime")
        Dim TaskCode As String = FreqTaskListDT.Rows(index)("TaskCode")
        Dim jb As JObject = JsonConvert.DeserializeObject(TaskCode, GetType(JObject))
        Dim freqStart As String = jb("freqStart").ToString
        Dim freqEnd As String = jb("freqEnd").ToString
        selectFreqStart = Val(freqStart)
        selectFreqEnd = Val(freqEnd)
        Chart1.ChartAreas(0).AxisX.Minimum = selectFreqStart
        Chart1.ChartAreas(0).AxisX.Maximum = selectFreqEnd
        Chart1.ChartAreas(0).AxisX.Interval = (selectFreqEnd - selectFreqStart) / 5
        selectStartTime = FreqTaskListDT.Rows(index)("StartTime")
        selectEndTime = FreqTaskListDT.Rows(index)("EndTime")
        selectDeviceID = FreqTaskListDT.Rows(index)("DeviceID")
        LVDetail.Items(6).SubItems(1).Text = freqStart & "MHz"
        LVDetail.Items(7).SubItems(1).Text = freqEnd & "MHz"
        LVDetail.Items(8).SubItems(1).Text = FreqTaskListDT.Rows(index)("OverPercent")
        LVDetail.Items(9).SubItems(1).Text = 0
        LVDetail.Items(10).SubItems(1).Text = FreqTaskListDT.Rows(index)("ResultReportUrl")
        LVDetail.Items(11).SubItems(1).Text = "右键下载"
        Label2.Text = "[" & freqStart & "MHz," & freqEnd & "MHz]"
        Label10.Text = "Mark " & freqStart & "MHz时序"
        'For Each p As freqTaskDetail In Panel_FreqTaskList.Controls
        '    If p.index = index Then
        '        p.Panel1.BackColor = Color.FromArgb(20, 68, 106)
        '        p.ForeColor = Color.White
        '        p.lickDetail.Visible = False
        '    Else
        '        p.Panel1.BackColor = Color.White
        '        p.ForeColor = Color.Black
        '        p.lickDetail.Visible = True
        '    End If
        'Next
        For Each c In Chart1.Series
            c.Points.Clear()
        Next
        ' Chart1.Series.Clear()
        Chart2.Series.Clear()
    End Sub
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        isPlay = False
        GetFreqTaskList()
    End Sub
    Private Sub 下载报告ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 下载报告ToolStripMenuItem.Click
        Dim value As String = LVDetail.Items(10).SubItems(1).Text
        If value = "" Then Exit Sub
        If InStr(value, "http://") Then
            If ServerIP <> "123.207.31.37" Then
                value = value.Replace("123.207.31.37", ServerIP)
            End If
            Process.Start(value)
        Else
            MsgBox("下载路径有误，请检查")
        End If
    End Sub
    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label5.Click
        play()
    End Sub

    Private Sub Panel16_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel16.Click

        play()
    End Sub
    Dim itmlist As List(Of ListViewItem)
    Private Sub Label6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label6.Click
        stopPlay()
    End Sub
    Private Sub Panel17_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel17.MouseClick
        stopPlay()
    End Sub
    Private Sub play()
        isPlay = True
        TongJiCiShu = 0
        sigNalCount = 0
        signalStartTime = Nothing
        itmlist = New List(Of ListViewItem)
        playThread = New Thread(AddressOf sub_play)
        playThread.Start()
    End Sub
    Private Sub stopPlay()
        isPlay = False
        If IsNothing(playThread) Then Return
        Try
            playThread.Abort()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub sub_play()
        If isPlay = False Then Exit Sub
        Label4.Text = "正在获取数据……"
        Dim dik As New Dictionary(Of String, String)
        dik.Add("func", "GetDeviceMsgTimeList")
        dik.Add("deviceID", selectDeviceID)
        dik.Add("token", token)
        dik.Add("startTime", selectStartTime)
        dik.Add("endTime", selectEndTime)

        Dim msg As String = TransforPara2Query(dik)

        Dim url As String = ServerUrl
        Dim result As String = GetH(url, msg)

        If result = "" Then
            MsgBox("没有任何数据")
            Exit Sub
        End If
        If result = "[]" Then
            MsgBox("没有任何数据")
            Exit Sub
        End If
        Dim TimeList As List(Of String) = result.Split(",").ToList
        If IsNothing(TimeList) Then
            MsgBox("没有任何数据")
            Exit Sub
        End If
        Label4.Text = TimeList(0)
        PlayLoop(TimeList)
    End Sub
    Structure JSONMsg
        Dim MsgTime As String
        Dim DeviceID As String
        Dim DeviceKind As String
        Dim Func As String
        Dim DeviceMsg As String
    End Structure
    Structure JSONfreq
        Dim func As String
        Dim msg As String
    End Structure
    Structure freqStu
        Dim freqStart As Double
        Dim freqStep As Double
        Dim deviceID As String
        Dim dataCount As Integer
        Dim value() As Double
        Dim isDSGFreq As Boolean
        Dim DSGFreqBase64 As String
    End Structure
    Private Sub PlayLoop(ByVal TimeList As List(Of String))
        If isPlay = False Then Exit Sub
        Dim sumCount As Integer = TimeList.Count
        For i = 0 To TimeList.Count - 1
            Label8.Text = (100 * (i + 1) / sumCount).ToString("0.00") & " %"
            Dim MsgTime As String = TimeList(i)
            Label4.Text = MsgTime
            Dim result As String = GetDeviceMsgByMsgTime(MsgTime)
            Me.Invoke(Sub() DrawMsgItem(result))
        Next
    End Sub
    Dim TongJiCiShu As Integer = 0
    Dim signalStartTime As Date
    Dim sigNalCount As Integer = 0
    Private Sub DrawMsgItem(ByVal result As String)
        If result = "" Then Return
        If InStr(result, "result=fail") Then
            Return
        End If
        Dim jsonMsgs As List(Of JSONMsg) = JsonConvert.DeserializeObject(result, GetType(List(Of JSONMsg)))
        For i = 0 To jsonMsgs.Count - 1
            Dim itm As JSONMsg = jsonMsgs(i)
            Dim MsgTime As String = itm.MsgTime
            Dim jsonFreq As JSONfreq = JsonConvert.DeserializeObject(itm.DeviceMsg, GetType(JSONfreq))
            Dim freqStu As freqStu = JsonConvert.DeserializeObject(jsonFreq.msg, GetType(freqStu))
            Dim freqStart As Double = freqStu.freqStart
            Dim freqStep As Double = freqStu.freqStep
            Dim DeviceID As String = freqStu.DeviceID
            Dim dataCount As Integer = freqStu.dataCount
            Dim value() As Double = freqStu.value
            If freqStu.isDSGFreq Then
                value = DSGBase2PPSJValues(freqStu.DSGFreqBase64)
            End If

            If IsNothing(value) Then Continue For
            dataCount = value.Count
            If dataCount = 0 Then Continue For
            Dim xx(dataCount - 1) As Double
            Dim yy() As Double = value
            For j = 0 To dataCount - 1
                xx(j) = freqStart + j * freqStep
            Next
            RefrushChart(xx, yy)
            Dim du As Integer = AutoFenXiDu
            Dim sigNal(,) As Double = XinHaoFenLi(xx, yy, du, AutoFenXiFuCha)
            Dim jieti As Integer = (du - 1) / 2
            If TongJiCiShu < 30 Then
                TongJiCiShu = TongJiCiShu + 1
                XinHao2LV(sigNal)
                If Chart2.Series.Count >= 2 Then
                    Chart2.Series(1).Points.Clear()
                End If
                For Each lvitm As ListViewItem In LV20.Items
                    Dim pl As String = lvitm.SubItems(2).Text
                    Dim count As String = lvitm.SubItems(7).Text
                    Dim cq As String = lvitm.SubItems(6).Text
                    If IsNumeric(count) Then
                        If Val(count) >= 10 Then
                            ' MsgBox(Chart1.Series.Count)
                            If Chart1.Series.Count >= 2 Then
                                For j = 0 To xx.Count - 1 - jieti
                                    If xx(j) = pl Then
                                        If j >= jieti Then
                                            For m = j - jieti To j + jieti
                                                Dim values As Double = yy(m)
                                                Me.Invoke(Sub() Chart1.Series(1).Points.AddXY(xx(m), values))
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
            If Chart2.Series.Count <= 0 Then
                Dim Series2 As New Series("")
                Series2.Color = Color.Blue
                Series2.XValueType = ChartValueType.DateTime
                Series2.IsValueShownAsLabel = True
                Series2.IsVisibleInLegend = False
                Series2.ToolTip = "#VAL"
                Series2.Color = Color.Blue
                Series2.ChartType = SeriesChartType.FastLine
                Series2.Points.AddXY(MsgTime, yy(0))
                Chart2.Series.Add(Series2)
            Else
                Chart2.Series(0).Points.AddXY(MsgTime, yy(0))
            End If
            ' GXPuBuTu(xx, yy)

        Next     
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
                itm.SubItems.Add("") '3
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
        LV20.Visible = False
        LV20.Items.Clear()
        Dim cnt As Integer = plist.Count
        For i = 0 To cnt - 1
            Dim itm As ListViewItem = plist(i)
            itm.Text = i + 1
            LV20.Items.Add(itm)
        Next
        LV20.Visible = True
    End Sub
    Private Sub RefrushChart(ByVal xx() As Double, ByVal yy() As Double)
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
        If Chart1.Series.Count <= 0 Then
            Chart1.Series.Add(Series)
        Else
            Chart1.Series(0) = Series
        End If
       
    End Sub
    Private recentY As Integer = 0
    Private Sub GXPuBuTu(ByVal xx() As Double, ByVal yy() As Double)
        'Try
        '    If IsNothing(yy) Then Exit Sub
        '    Dim max As Integer = 255
        '    Dim min As Integer = 0
        '    Dim width As Integer = yy.Count - 1
        '    Dim heigth As Integer = PBX.Height
        '    Dim px As Integer = 1
        '    Dim num As Integer = width
        '    If yy.Count < num Then
        '        num = yy.Count
        '    End If
        '    If IsNothing(PBX.Image) Then
        '        Dim bmp As New Bitmap(width, heigth)
        '        recentY = 0
        '        For m = 0 To num - 1
        '            Dim value As Double = yy(m)
        '            Dim vR As Integer = Abs(value + 90)
        '            Dim vG As Integer = Abs(value + 10)
        '            Dim vB As Integer = Abs(value + 30)
        '            If value > -70 Then
        '                vR = 255
        '                vG = 255
        '                vB = 0
        '            End If
        '            bmp.SetPixel(m, recentY, Color.FromArgb(vR, vG, vB))
        '        Next
        '        recentY = recentY + 1
        '        PBX.Image = bmp
        '    Else
        '        Dim bmp As Bitmap = PBX.Image
        '        If recentY < heigth - 1 Then
        '            For m = 0 To num - 1
        '                Dim value As Double = yy(m)
        '                Dim vR As Integer = Abs(value + 90)
        '                Dim vG As Integer = Abs(value + 10)
        '                Dim vB As Integer = Abs(value + 30)
        '                If value > -70 Then
        '                    vR = 255
        '                    vG = 255
        '                    vB = 0
        '                End If
        '                bmp.SetPixel(m, recentY, Color.FromArgb(vR, vG, vB))
        '            Next
        '            recentY = recentY + 1
        '        Else
        '            PBX.Image = Nothing
        '            Return
        '            For i = 0 To heigth - 2
        '                For j = 0 To num - 1
        '                    bmp.SetPixel(j, i, bmp.GetPixel(j, i + 1))
        '                Next
        '            Next
        '            For m = 0 To num - 1
        '                Dim value As Double = yy(m)
        '                Dim vR As Integer = Abs(value + 90)
        '                Dim vG As Integer = Abs(value + 10)
        '                Dim vB As Integer = Abs(value + 30)
        '                If value > -70 Then
        '                    vR = 255
        '                    vG = 255
        '                    vB = 0
        '                End If
        '                bmp.SetPixel(m, recentY, Color.FromArgb(vR, vG, vB))
        '            Next
        '        End If
        '        'bmp.Save(Application.StartupPath & "\a.img")
        '        PBX.Image = bmp
        '    End If
        'Catch ex As Exception
        '    'MsgBox(ex.ToString)
        '    PBX.Image = Nothing
        'End Try
    End Sub
    Private Function GetDeviceMsgByMsgTime(ByVal MsgTime As String) As String
        Dim dik As New Dictionary(Of String, String)
        dik.Add("func", "GetDeviceMsg")
        dik.Add("deviceID", selectDeviceID)
        dik.Add("token", token)
        dik.Add("msgTime", MsgTime)
        Dim msg As String = TransforPara2Query(dik)
        Dim url As String = ServerUrl
        Dim result As String = GetH(url, msg)
        Return result
    End Function

    Private Sub Panel_FreqTaskList_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel_FreqTaskList.Paint

    End Sub

    Private Sub LVTask_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVTask.SelectedIndexChanged
        If LVTask.SelectedIndices.Count = 0 Then Return
        Dim index As Integer = LVTask.SelectedIndices(0)
        freqTaskDetailBeClick(index)
    End Sub

  
End Class
