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
Public Class SignalTaskPanel
    Private FreqTaskListDT As DataTable
    Private selectStartTime As String
    Private selectEndTime As String
    Private selectDeviceID As String
    Private selectSignalList As List(Of Double)
    Private selectTimeListSumCount As Integer
    Private selectTimeListReadCount As Integer
    Private isPlay As Boolean = False
    Private playThread As Thread
    Private selectSignalBiaoPanel As List(Of SignalBiaoPanel)
    Private Sub SignalTaskPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        ini()
        'iniChart2()
        'iniChart2()
        PictureBox1.Cursor = Cursors.Hand
        PictureBox2.Cursor = Cursors.Hand
        PictureBox3.Cursor = Cursors.Hand
        PictureBox4.Cursor = Cursors.Hand
        PictureBox5.Cursor = Cursors.Hand
        Label5.Cursor = Cursors.Hand
        Panel16.Cursor = Cursors.Hand
        Label6.Cursor = Cursors.Hand
        Panel17.Cursor = Cursors.Hand
        GetSignalTaskList()
        iniChart2()
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
        itm = New ListViewItem("频点信息")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("频点数量")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("完成状态")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("可用数量")
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
    End Sub
    Private Sub iniChart2()
        Chart2.Series.Clear()
        Chart2.ChartAreas(0).AxisY.Maximum = -20
        Chart2.ChartAreas(0).AxisY.Minimum = -120
        Chart2.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart2.ChartAreas(0).AxisY.Interval = 20
        Chart2.ChartAreas(0).AxisX.Minimum = 88
        Chart2.ChartAreas(0).AxisX.Maximum = 108
        Chart2.ChartAreas(0).AxisX.Interval = 5
        ' Chart2.ChartAreas(0).AxisX.IsStartedFromZero = True

        Dim Series As New Series '频谱   0

        Series = New Series '频谱   0
        Series.Label = "频谱数据"
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.Points.AddXY(88, -120)
        Series.IsVisibleInLegend = False
        Chart2.Series.Add(Series)


        Dim ser As New Series("illegalsignal")
        ser.ChartType = SeriesChartType.Column
        ser("PointWidth") = 0.1
        ser.Color = Color.Red
        ser.IsVisibleInLegend = False
        Dim r As New Random
        For i = 88 To 108 Step 1
            Dim v As Double = r.Next(-90, -70)
            ser.Points.AddXY(i, v)
        Next
        ser.IsVisibleInLegend = False
        Chart2.Series.Add(ser)
    End Sub
    Private Sub GetSignalTaskList()
        Label1.Text = "获取中……"
        Dim result As String = GetH(ServerUrl, "func=GetMyTask&token=" & token)
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetSignalTaskList()
            Exit Sub
        End If
        'Panel_FreqTaskList.Controls.Clear()
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
            For i = 0 To dt2.Rows.Count - 1
                Dim row As DataRow = dt2.Rows(i)
                If row("TaskName") = "可用评估" Then
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
            '    If row("TaskName") <> "频谱监测" Then
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
            '        p.lbl_TaskCode.Text = TaskCode
            '        Panel_FreqTaskList.Controls.Add(p)
            '        AddHandler p.beClick, AddressOf freqTaskDetailBeClick
            '    End If
            'Next
            freqTaskDetailBeClick(FreqTaskListDT.Rows.Count - 1)
        End If

        Label1.Text = "任务列表"
    End Sub
    Private Sub freqTaskDetailBeClick(ByVal index As Integer)
        isPlay = False
        If index = -1 Then Return
        LVDetail.Items(0).SubItems(1).Text = FreqTaskListDT.Rows(index)("TaskNickName")
        LVDetail.Items(1).SubItems(1).Text = FreqTaskListDT.Rows(index)("TaskName")
        LVDetail.Items(2).SubItems(1).Text = "广州市开发区"
        LVDetail.Items(3).SubItems(1).Text = FreqTaskListDT.Rows(index)("DeviceID")
        LVDetail.Items(4).SubItems(1).Text = FreqTaskListDT.Rows(index)("StartTime")
        LVDetail.Items(5).SubItems(1).Text = FreqTaskListDT.Rows(index)("EndTime")
        Dim TaskCode As String = FreqTaskListDT.Rows(index)("TaskCode")
        Dim tmp As String = TaskCode.Replace("[", "").Replace("]", "")
        If tmp = "" Then Return
        selectSignalList = New List(Of Double)
        For Each itm In tmp.Split(",")
            selectSignalList.Add(Val(itm))
        Next
        selectStartTime = FreqTaskListDT.Rows(index)("StartTime")
        selectEndTime = FreqTaskListDT.Rows(index)("EndTime")
        selectDeviceID = FreqTaskListDT.Rows(index)("DeviceID")
        LVDetail.Items(6).SubItems(1).Text = TaskCode
        LVDetail.Items(7).SubItems(1).Text = selectSignalList.Count
        LVDetail.Items(8).SubItems(1).Text = FreqTaskListDT.Rows(index)("OverPercent")
        LVDetail.Items(9).SubItems(1).Text = 0
        LVDetail.Items(10).SubItems(1).Text = FreqTaskListDT.Rows(index)("ResultReportUrl")
        LVDetail.Items(11).SubItems(1).Text = "右键下载"
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
        Panel14.Controls.Clear()
        selectSignalBiaoPanel = New List(Of SignalBiaoPanel)
        Dim tmpList As New List(Of SignalBiaoPanel)
        For j = selectSignalList.Count - 1 To 0 Step -1
            Dim itm As Double = selectSignalList(j)
            Dim p As New SignalBiaoPanel(itm)
            Panel14.Controls.Add(p)
            tmpList.Add(p)
        Next
        For j = tmpList.Count - 1 To 0 Step -1
            selectSignalBiaoPanel.Add(tmpList(j))
        Next
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

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetSignalTaskList()
    End Sub
    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label5.Click
        play()
    End Sub
    Private Sub Panel16_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel16.MouseClick
        play()
    End Sub
    Private Sub Label6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label6.Click
        stopPlay()
    End Sub
    Private Sub Panel17_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel17.MouseClick
        stopPlay()
    End Sub

    Private Sub play()
        selectTimeListSumCount = 0
        selectTimeListReadCount = 0
        isPlay = True
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
    'selectTimeListSumCount
    Private Sub sub_play()
        If isPlay = False Then Exit Sub
        Label4.Text = "正在获取数据……"
        Dim dik As New Dictionary(Of String, String)
        dik.Add("func", "GetDeviceMsgTimeListInfo")
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
        If Val(result) <= 0 Then
            MsgBox("没有任何数据")
            Exit Sub
        End If
        Dim count As Integer = 1000
        selectTimeListSumCount = Val(result)
        If count > selectTimeListSumCount Then
            count = selectTimeListSumCount
        End If
        ReviceMsgListLoop(0, count, selectTimeListSumCount)
        'PlayLoop(TimeList)
    End Sub
    Private Sub ReviceMsgListLoop(ByVal startIndex As Integer, ByVal count As Integer, ByVal maxCount As Integer)
        If isPlay = False Then Exit Sub
        Label4.Text = "正在获取数据……"
        Dim dik As New Dictionary(Of String, String)
        dik.Add("func", "GetDeviceMsgTimeListByIndex")
        dik.Add("deviceID", selectDeviceID)
        dik.Add("token", token)
        dik.Add("startTime", selectStartTime)
        dik.Add("endTime", selectEndTime)
        dik.Add("startIndex", startIndex)
        dik.Add("count", count)
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
        If TimeList.Count = 0 Then Console.WriteLine("TimeList.Count = 0 ") : Exit Sub
        Label4.Text = TimeList(0)
        PlayLoop(TimeList)
        startIndex = startIndex + count
        If startIndex >= maxCount Then
            Console.WriteLine(startIndex & "," & maxCount)
            Return
        End If
        If count > maxCount - startIndex Then
            count = maxCount - startIndex
        End If
        Console.WriteLine("Count =" & count)
        ReviceMsgListLoop(startIndex, count, maxCount)
    End Sub
    Structure JSONMsg
        Dim MsgTime As String
        Dim DeviceID As String
        Dim DeviceKind As String
        Dim Func As String
        Dim DeviceMsg As String
    End Structure
    Private Sub PlayLoop(ByVal TimeList As List(Of String))
        If isPlay = False Then Exit Sub
        Dim sumCount As Integer = TimeList.Count
        For i = 0 To TimeList.Count - 1
            selectTimeListReadCount = selectTimeListReadCount + 1
            Dim v As Double = selectTimeListReadCount / selectTimeListSumCount
            If v > 1 Then v = 1
            Label8.Text = (100 * v).ToString("0.00") & " %"
            Dim MsgTime As String = TimeList(i)
            Label4.Text = MsgTime
            Dim result As String = GetDeviceMsgByMsgTime(MsgTime)
            Me.Invoke(Sub() DrawMsgItem(result))
        Next
    End Sub
    Private Sub DrawMsgItem(ByVal result As String)
        If result = "" Then Return
        Dim jsonMsgs As List(Of JSONMsg) = JsonConvert.DeserializeObject(result, GetType(List(Of JSONMsg)))
        For i = 0 To jsonMsgs.Count - 1
            Dim itm As JSONMsg = jsonMsgs(i)
            Dim MsgTime As String = itm.MsgTime
            Dim DeviceMsg As String = itm.DeviceMsg
            Dim Func As String = itm.Func
            If Func <> "SSSJ" Then Continue For
            Dim BQ As String = DeviceMsg
            Dim str As String = BQ.Substring(InStr(BQ, "<"), InStr(BQ, ">") - InStr(BQ, "<") - 1)
            Dim st() As String = str.Split(",")
            Dim numOfValue As Integer = st(2)
            If st.Length > numOfValue * 2 Then
                For j = 3 To 2 + numOfValue
                    Dim pd As Double = st(j + numOfValue)
                    Dim cq As Double = st(j)
                    If j - 3 < selectSignalBiaoPanel.Count Then
                        Dim p As SignalBiaoPanel = selectSignalBiaoPanel(j - 3)
                        p.SetSignalValue(MsgTime, cq)
                    End If                 
                Next
                Exit Sub
            End If
        Next
    End Sub
    Private Function GetDeviceMsgByMsgTime(ByVal MsgTime As String) As String
        Dim dik As New Dictionary(Of String, String)
        dik.Add("func", "GetDeviceMsg")
        dik.Add("deviceID", selectDeviceID)
        dik.Add("token", token)
        dik.Add("msgTime", MsgTime)
        dik.Add("msgfunc", "SSSJ")
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

    Private Sub Panel5_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel5.Paint

    End Sub
End Class
