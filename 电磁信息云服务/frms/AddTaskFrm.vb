Imports System
Imports System.IO
Imports System.Text
Imports System.Math
Imports Newtonsoft
Imports Newtonsoft.Json
Public Class AddTaskFrm

    Private Sub AddTaskFrm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "新增任务"
        Me.TopMost = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        'Me.Text = alldevlist.Count
        '  Panel3.Enabled = False

        ' Panel5.Enabled = False
        ini()
       
        getSigNalCount()
    End Sub
    Private Sub getSigNalCount()
        Dim pdList As New List(Of Double)
        For Each itm In RTB.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    pdList.Add(Val(itm))
                End If
            End If
        Next
        Label6.Text = pdList.Count
    End Sub
    Private Sub ini()


        txtNicName.Text = Label2.Text & Now.ToString("yyyyMMddHHmmss")
        txtNicName2.Text = Label24.Text & Now.ToString("yyyyMMddHHmmss")
        txtNicName3.Text = Label47.Text & Now.ToString("yyyyMMddHHmmss")
        txtNicName5.Text = Label80.Text & Now.ToString("yyyyMMddHHmmss")
        txtTaskNickName6.Text = "频谱取样" & Now.ToString("yyyyMMddHHmmss")
        txtTaskNickName7.Text = "占用统计" & Now.ToString("yyyyMMddHHmmss")
        txtTaskNickName8.Text = "状态预警" & Now.ToString("yyyyMMddHHmmss")
        txtTaskNickName9.Text = "POA定位" & Now.ToString("yyyyMMddHHmmss")
        txtTaskNickName10.Text = "黑广播捕获" & Now.ToString("yyyyMMddHHmmss")
        txtTaskNickName11.Text = "违章捕获" & Now.ToString("yyyyMMddHHmmss")
        cbDeviceID.Items.Clear()
        cbDeviceID2.Items.Clear()
        cbDeviceID3.Items.Clear()
        cbDeviceID5.Items.Clear()
        cbDeviceID6.Items.Clear()
        cbDeviceID7.Items.Clear()
        CBDeviceID8.Items.Clear()
        CLBDeviceID9.Items.Clear()
        CBDeviceID10.Items.Clear()
        CBDeviceID11.Items.Clear()
        If IsNothing(alldevlist) Then
            Form1.GetOnlineDevice()
        End If
        For Each d In alldevlist
            If d.Kind = "TSS" Then
                cbDeviceID.Items.Add(d.Name)
                cbDeviceID3.Items.Add(d.Name)
                cbDeviceID6.Items.Add(d.Name)
                cbDeviceID7.Items.Add(d.Name)
                CBDeviceID10.Items.Add(d.Name)
                CBDeviceID11.Items.Add(d.Name)
            End If
            If d.Kind = "TZBQ" Then
                cbDeviceID2.Items.Add(d.Name)
                '  cbDeviceID3.Items.Add(d.Name)
                cbDeviceID5.Items.Add(d.Name)
                CBDeviceID8.Items.Add(d.Name)
                CLBDeviceID9.Items.Add(d.Name)
            End If
        Next
        If cbDeviceID.Items.Count > 0 Then cbDeviceID.SelectedIndex = 0
        If cbDeviceID2.Items.Count > 0 Then cbDeviceID2.SelectedIndex = 0
        If cbDeviceID3.Items.Count > 0 Then cbDeviceID3.SelectedIndex = 0
        If cbDeviceID5.Items.Count > 0 Then cbDeviceID5.SelectedIndex = 0
        If cbDeviceID6.Items.Count > 0 Then cbDeviceID6.SelectedIndex = 0
        If cbDeviceID7.Items.Count > 0 Then cbDeviceID7.SelectedIndex = 0
        If CBDeviceID8.Items.Count > 0 Then CBDeviceID8.SelectedIndex = 0
        If CLBDeviceID9.Items.Count > 0 Then CLBDeviceID9.SetItemChecked(0, True)
        If CBDeviceID10.Items.Count > 0 Then CBDeviceID10.SelectedIndex = 0
        If CBDeviceID11.Items.Count > 0 Then CBDeviceID11.SelectedIndex = 0
        If Form1.selectDeviceKind = "TSS" Then
            cbDeviceID.SelectedItem = Form1.selectDeviceID
            cbDeviceID6.SelectedItem = Form1.selectDeviceID
            cbDeviceID7.SelectedItem = Form1.selectDeviceID
            CBDeviceID10.SelectedItem = Form1.selectDeviceID
            CBDeviceID11.SelectedItem = Form1.selectDeviceID
        End If
        If Form1.selectDeviceKind = "TZBQ" Then
            cbDeviceID2.SelectedItem = Form1.selectDeviceID
            cbDeviceID3.SelectedItem = Form1.selectDeviceID
            cbDeviceID5.SelectedItem = Form1.selectDeviceID
            CBDeviceID8.SelectedItem = Form1.selectDeviceID
            For i = 0 To CLBDeviceID9.Items.Count - 1
                If CLBDeviceID9.Items(i) = Form1.selectDeviceID Then
                    CLBDeviceID9.SetItemChecked(i, True)
                End If
            Next
        End If
        txtStartTime.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime2.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime3.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime5.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime6.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime7.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime8.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime9.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime10.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtStartTime11.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTIme2.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime3.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime5.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime6.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime7.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime8.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime9.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime10.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtEndTime11.Text = Now.AddMinutes(3).ToString("yyyy-MM-dd HH:mm:ss")
        txtWechatName.Text = "张三"
        txtWechatName2.Text = "张三"
        txtWechatName3.Text = "张三"
        txtWechatName5.Text = "张三"
        txtWechatName6.Text = "张三"
        txtWechatName7.Text = "张三"
        txtWechatName8.Text = "张三"
        txtWechatName9.Text = "张三"
        txtWechatName10.Text = "张三"
        txtWechatName11.Text = "张三"
        txtEmailName.Text = "619498477@qq.com"
        txtEmailName2.Text = "619498477@qq.com"
        txtEmailName3.Text = "619498477@qq.com"
        txtEmailName5.Text = "619498477@qq.com"
        txtEmailName6.Text = "619498477@qq.com"
        txtEmailName7.Text = "619498477@qq.com"
        txtEmailName8.Text = "619498477@qq.com"
        txtEmailName9.Text = "619498477@qq.com"
        txtEmailName10.Text = "619498477@qq.com"
        txtEmailName11.Text = "619498477@qq.com"
        txtFreqStart.Text = "88.000"
        txtFreqEnd.Text = "108.000"
        txtFreqStep.Text = "25.00"
        txtFreqStart6.Text = "88.000"
        txtFreqEnd6.Text = "108.000"
        txtFreqStep6.Text = "25.00"
        txtSaveTimeStep6.Text = 10

        txtFreqStart7.Text = "88.000"
        txtFreqEnd7.Text = "108.000"
        txtFreqStep7.Text = "25.00"

        txtFreqStart3.Text = "88.000"
        txtFreqEnd3.Text = "108.000"
        txtFreqStep3.Text = "25.00"
        '   txtThreshol7.Text = -60
        RTB.Text = "300.345" & vbCrLf &
                   "400.412" & vbCrLf &
                   "500.315" & vbCrLf &
                   "660.186"
        Dim sb As New StringBuilder
        sb.AppendLine("400.000")
        sb.AppendLine("435.000")
        sb.AppendLine("446.000")
        sb.AppendLine("463.000")
        sb.AppendLine("495.000")
        RTB3.Text = sb.ToString
        RTB3.Text = "434.550" & vbCrLf & "439.550"
        Label36.Text = 5
        RTB5.Text = sb.ToString
        RTB8.Text = sb.ToString
        txtModuleTime5.Text = 30
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Me.Close()
    End Sub
    
    Structure tssOrder_stu
        Dim deviceID As String
        Dim task As String
        Dim freqStart As Double
        Dim freqEnd As Double
        Dim freqStep As Double
        Dim saveFreqStep As Integer
        Dim Threshol As Double
        Dim Fucha As Double
        Dim Daikuan As Double
        Dim MinDValue As Double
        Dim Legal As List(Of Double)
        Dim watchPoint As List(Of Double)
        Dim WarnNum As Integer
    End Structure
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = Label24.Text
        task.TaskNickName = txtNicName2.Text
        task.DeviceID = cbDeviceID2.SelectedItem
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = task.DeviceID Then
                    task.DeviceID = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        task.StartTime = txtStartTime2.Text
        task.EndTime = txtEndTIme2.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName2.Text
        task.PushEmailToUserName = txtEmailName2.Text
        Dim TaskCode As String = ""
        Dim pdList As New List(Of Double)
        For Each itm In RTB.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    pdList.Add(Val(itm))
                End If
            End If
        Next
        TaskCode = JsonConvert.SerializeObject(pdList)

        If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg2.Text
        Dim msg As String = JsonConvert.SerializeObject(task)

        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = Label2.Text
        task.TaskNickName = txtNicName.Text
        task.DeviceID = cbDeviceID.SelectedItem
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = task.DeviceID Then
                    task.DeviceID = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        task.StartTime = txtStartTime.Text
        task.EndTime = txtEndTime.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName.Text
        task.PushEmailToUserName = txtEmailName.Text
        Dim TaskCode As String = ""
        Dim freqbegin As String = Val(txtFreqStart.Text)
        Dim freqend As String = Val(txtFreqEnd.Text)
        Dim freqstep As String = Val(txtFreqStep.Text)
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = freqend
        p.freqStep = freqstep
        p.task = "bscan"
        p.deviceID = task.DeviceID

        TaskCode = JsonConvert.SerializeObject(p)
        If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg.Text
        Dim msg As String = JsonConvert.SerializeObject(task)
        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub

    Private Sub RTB_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RTB.TextChanged
        getSigNalCount()
    End Sub

    Private Sub RTB2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RTB5.TextChanged
        Dim pdList As New List(Of Double)
        For Each itm In RTB5.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    pdList.Add(Val(itm))
                End If
            End If
        Next
        Label53.Text = pdList.Count
    End Sub
    Structure YuJingStu
        Dim plist As List(Of Double)
        Dim moduleTime As Integer
        Sub New(ByVal _plist As List(Of Double), ByVal _moduleTime As Integer)
            plist = _plist
            moduleTime = _moduleTime
        End Sub
    End Structure
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = Label80.Text
        task.TaskNickName = txtNicName5.Text
        task.DeviceID = cbDeviceID5.SelectedItem
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = task.DeviceID Then
                    task.DeviceID = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        task.StartTime = txtStartTime5.Text
        task.EndTime = txtEndTime5.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName5.Text
        task.PushEmailToUserName = txtEmailName5.Text
        Dim TaskCode As String = ""
        Dim pdList As New List(Of Double)
        For Each itm In RTB5.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    pdList.Add(Val(itm))
                End If
            End If
        Next
        If txtModuleTime5.Text = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        Dim moduleTime As Integer = Val(txtModuleTime5.Text)
        If moduleTime < 10 Then
            MsgBox("建模时间必须大于或等于10秒")
            Exit Sub
        End If
        Dim yjs As New YuJingStu(pdList, Val(txtModuleTime5.Text))
        TaskCode = JsonConvert.SerializeObject(yjs)
        If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg5.Text
        Dim msg As String = JsonConvert.SerializeObject(task)
        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Me.Close()
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = "频谱取样"
        task.TaskNickName = txtTaskNickName6.Text
        task.DeviceID = cbDeviceID6.SelectedItem
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = task.DeviceID Then
                    task.DeviceID = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        task.StartTime = txtStartTime6.Text
        task.EndTime = txtEndTime6.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName6.Text
        task.PushEmailToUserName = txtEmailName6.Text
        Dim TaskCode As String = ""
        Dim freqbegin As String = Val(txtFreqStart6.Text)
        Dim freqend As String = Val(txtFreqEnd6.Text)
        Dim freqstep As String = Val(txtFreqStep6.Text)
        Dim saveFreqStep As String = Val(txtSaveTimeStep6.Text)
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = freqend
        p.freqStep = freqstep
        p.task = "bscan"
        p.deviceID = task.DeviceID
        p.saveFreqStep = saveFreqStep
        TaskCode = JsonConvert.SerializeObject(p)
        If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg.Text
        Dim msg As String = JsonConvert.SerializeObject(task)

        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        Me.Close()
    End Sub

    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        Me.Close()
    End Sub

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = "占用统计"
        task.TaskNickName = txtTaskNickName7.Text
        task.DeviceID = cbDeviceID7.SelectedItem
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = task.DeviceID Then
                    task.DeviceID = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        task.StartTime = txtStartTime7.Text
        task.EndTime = txtEndTime7.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName7.Text
        task.PushEmailToUserName = txtEmailName7.Text
        Dim TaskCode As String = ""
        Dim freqbegin As String = Val(txtFreqStart7.Text)
        Dim freqend As String = Val(txtFreqEnd7.Text)
        Dim freqstep As String = Val(txtFreqStep7.Text)
        Dim Threshol As String = Val(txtThreshol7.Text)
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = freqend
        p.freqStep = freqstep
        p.task = "bscan"
        p.deviceID = task.DeviceID
        p.Threshol = Threshol
        'If RTB0.Text <> "" Then
        '    Dim pdList As New List(Of Double)
        '    For Each itm In RTB0.Text.Split(Chr(10))
        '        If itm <> "" Then
        '            If IsNumeric(itm) Then
        '                pdList.Add(Val(itm))
        '            End If
        '        End If
        '    Next
        '    If pdList.Count > 0 Then
        '        p.watchPoint = pdList
        '    End If
        'End If
        TaskCode = JsonConvert.SerializeObject(p)
        If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg.Text
        Dim msg As String = JsonConvert.SerializeObject(task)

        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub

    Private Sub RichTextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RTB8.TextChanged
        Dim pdList As New List(Of Double)
        For Each itm In RTB8.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    pdList.Add(Val(itm))
                End If
            End If
        Next
        Label111.Text = pdList.Count
    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        Me.Close()
    End Sub

    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = "状态预警"
        task.TaskNickName = txtTaskNickName8.Text
        task.DeviceID = CBDeviceID8.SelectedItem
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = task.DeviceID Then
                    task.DeviceID = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        task.StartTime = txtStartTime8.Text
        task.EndTime = txtEndTime8.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName8.Text
        task.PushEmailToUserName = txtEmailName8.Text
        Dim TaskCode As String = ""
        Dim pdList As New List(Of Double)
        For Each itm In RTB.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    pdList.Add(Val(itm))
                End If
            End If
        Next
        Dim MoudleTime As Double = Val(txtModuleTime8.Text)
        Dim MaxPercent As Double = Val(txtMaxPercent8.Text)
        Dim MinPercent As Double = Val(txtMinPercent8.Text)
        Dim HoldSecond As Integer = Val(txtHoldSecond8.Text)
        Dim MinValue As Double = Val(txtMinValue8.Text)
        Dim MinValueSecond As Double = Val(txtMinValueSecond8.Text)
        If task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置基础参数")
            Exit Sub
        End If
        If MoudleTime = 0 Or MaxPercent = 0 Or MinPercent = 0 Or HoldSecond = 0 Or MinValue = 0 Or MinValueSecond = 0 Then
            MsgBox("请完整配置技术参数")
            Exit Sub
        End If
        Dim z As ZTYJ_stu
        z.pdList = pdList
        z.ModuleTime = MoudleTime
        z.MaxPercent = MaxPercent
        z.MinPercent = MinPercent
        z.HoldSecond = HoldSecond
        z.MinValue = MinValue
        z.MinValueSecond = MinValueSecond
        TaskCode = JsonConvert.SerializeObject(z)
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg8.Text
        Dim msg As String = JsonConvert.SerializeObject(task)
        '  MsgBox(msg)
        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub
    Structure ZTYJ_stu
        Dim pdList As List(Of Double)
        Dim ModuleTime As Double
        Dim MaxPercent As Double
        Dim MinPercent As Double
        Dim HoldSecond As Integer
        Dim MinValue As Double
        Dim MinValueSecond As Double
    End Structure

    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click
        Me.Close()
    End Sub

    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        For i = 0 To CLBDeviceID9.Items.Count - 1
            CLBDeviceID9.SetItemChecked(i, True)
        Next
    End Sub

    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        For i = 0 To CLBDeviceID9.Items.Count - 1
            If CLBDeviceID9.GetItemChecked(i) Then
                CLBDeviceID9.SetItemChecked(i, False)
            Else
                CLBDeviceID9.SetItemChecked(i, True)
            End If
        Next
    End Sub

    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = "POA定位"
        task.isMoreDevice = True
        task.TaskNickName = txtTaskNickName9.Text
        Dim list As New List(Of String)
        For i = 0 To CLBDeviceID9.CheckedItems.Count - 1
            Dim str As String = CLBDeviceID9.CheckedItems(i)
            For Each d In alldevlist
                If d.Name = str Then
                    list.Add(d.DeviceID)
                End If
            Next
        Next
        If list.Count < 2 Then
            MsgBox("请至少选择两个设备")
            Exit Sub
        End If
        task.DeviceID = JsonConvert.SerializeObject(list)
        task.StartTime = txtStartTime9.Text
        task.EndTime = txtEndTime9.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName9.Text
        task.PushEmailToUserName = txtEmailName9.Text
        Dim TaskCode As String = Val(txtSigNal9.Text)
        If TaskCode = "" Or TaskCode = 0 Then
            MsgBox("定位频点设置有误")
            Exit Sub
        End If
        If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg9.Text
        Dim msg As String = JsonConvert.SerializeObject(task)
        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub
   
    Private Sub RTB3_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RTB3.TextChanged
        Dim pdList As New List(Of Double)
        For Each itm In RTB3.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    pdList.Add(Val(itm))
                End If
            End If
        Next
        Label36.Text = pdList.Count
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Me.Close()
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim list As New List(Of String)
        For Each itm In cbDeviceID3.CheckedItems
            list.Add(itm)
        Next
        Dim successCount As Integer = 0
        Dim failCount As Integer = 0
        Dim failsb As New StringBuilder
        For Each deviceId In list
            Dim task As NormalTaskStu
            task.UserName = usr
            task.TaskName = "台站监督"
            task.TaskNickName = txtNicName3.Text & "_" & deviceId
            task.DeviceID = deviceId
            Dim kind As String = ""
            SyncLock alldevlist
                For Each itm In alldevlist
                    If itm.Name = task.DeviceID Then
                        task.DeviceID = itm.DeviceID
                        kind = itm.Kind
                        Exit For
                    End If
                Next
            End SyncLock
            task.StartTime = txtStartTime3.Text
            task.EndTime = txtEndTime3.Text
            task.TimeStep = 5
            task.PushWeChartToUserName = txtWechatName3.Text
            task.PushEmailToUserName = txtEmailName3.Text
            Dim TaskCode As String = ""
            If kind = "TZBQ" Then
                Dim pdList As New List(Of Double)
                For Each itm In RTB3.Text.Split(Chr(10))
                    If itm <> "" Then
                        If IsNumeric(itm) Then
                            pdList.Add(Val(itm))
                        End If
                    End If
                Next
                TaskCode = JsonConvert.SerializeObject(pdList)
            End If
            If kind = "TSS" Then
                Dim freqbegin As String = Val(txtFreqStart3.Text)
                Dim freqend As String = Val(txtFreqEnd3.Text)
                Dim freqstep As String = Val(txtFreqStep3.Text)
                Dim Threshol As String = Val(txtThreshol3.Text)
                Dim p As tssOrder_stu
                p.freqStart = freqbegin
                p.freqEnd = freqend
                p.freqStep = freqstep
                p.task = "bscan"
                p.deviceID = task.DeviceID
                p.Threshol = Threshol
                Dim pdList As New List(Of Double)
                For Each itm In RTB3.Text.Split(Chr(10))
                    If itm <> "" Then
                        If IsNumeric(itm) Then
                            pdList.Add(Val(itm))
                        End If
                    End If
                Next
                If pdList.Count > 0 Then
                    Dim isDo As Boolean = True
                    For Each itm In pdList
                        If itm < freqbegin Or itm > freqend Then
                            isDo = False
                            Exit For
                        End If
                    Next
                    If isDo = False Then
                        MsgBox("监测频率列表有些频点不在[" & freqbegin & "," & freqend & "]范围内！")
                        Return
                    End If

                    p.watchPoint = pdList
                End If
                TaskCode = JsonConvert.SerializeObject(p)
            End If
            If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
                MsgBox("请完整配置参数")
                Exit Sub
            End If
            task.TaskCode = TaskCode
            task.TaskBg = txtTaskBg3.Text
            Dim msg As String = JsonConvert.SerializeObject(task)

            Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
            Dim res As String = GetResultPara("result", result)
            If res = "success" Then
                successCount = successCount + 1
                'Dim w As New WarnBox("命令下发成功！")
                'w.Show()
                'Me.Close()
            Else
                failCount = failCount + 1
                failsb.AppendLine(result)
                '    MsgBox(result)
            End If
        Next
        Dim sb As New StringBuilder
        sb.AppendLine("任务下发完成")
        sb.AppendLine("成功数量:" & successCount)
        If failCount > 0 Then sb.AppendLine("失败数量:" & failCount)
        If failCount > 0 Then sb.AppendLine("失败提示:")
        If failCount > 0 Then sb.AppendLine(sb.ToString)
        MsgBox(sb.ToString)
        Me.Close()
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Me.Close()
    End Sub

    Private Sub Button21_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button21.Click
        Me.Close()
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = "违章捕获"
        task.TaskNickName = txtTaskNickName11.Text
        task.DeviceID = CBDeviceID11.SelectedItem
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = task.DeviceID Then
                    task.DeviceID = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        task.StartTime = txtStartTime11.Text
        task.EndTime = txtEndTime11.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName11.Text
        task.PushEmailToUserName = txtEmailName11.Text
        Dim TaskCode As String = ""
        Dim freqbegin As String = Val(txtFreqStart11.Text)
        Dim freqend As String = Val(txtFreqEnd11.Text)
        Dim freqstep As String = Val(txtFreqStep11.Text)
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = freqend
        p.freqStep = freqstep
        p.task = "bscan"
        p.deviceID = task.DeviceID
        p.Fucha = Val(txtFucha11.Text)
        p.Daikuan = Val(txtDaikuan11.Text)
        p.MinDValue = Val(txtMinDValue11.Text)
        p.Legal = New List(Of Double)
        p.WarnNum = Val(txtWarnNum11.Text)
        For Each itm In txtLegal11.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    p.Legal.Add(Val(itm))
                End If
            End If
        Next
        TaskCode = JsonConvert.SerializeObject(p)
        If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg.Text
        Dim msg As String = JsonConvert.SerializeObject(task)
        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub

    Private Sub Button22_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button22.Click
        Dim task As NormalTaskStu
        task.UserName = usr
        task.TaskName = "黑广播捕获"
        task.TaskNickName = txtTaskNickName10.Text
        task.DeviceID = CBDeviceID10.SelectedItem
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = task.DeviceID Then
                    task.DeviceID = itm.DeviceID
                    Exit For
                End If
            Next
        End SyncLock
        task.StartTime = txtStartTime10.Text
        task.EndTime = txtEndTime10.Text
        task.TimeStep = 5
        task.PushWeChartToUserName = txtWechatName10.Text
        task.PushEmailToUserName = txtEmailName10.Text
        Dim TaskCode As String = ""
        Dim freqbegin As String = Val(txtFreqStart10.Text)
        Dim freqend As String = Val(txtFreqEnd10.Text)
        Dim freqstep As String = Val(txtFreqStep10.Text)
        Dim p As tssOrder_stu
        p.freqStart = freqbegin
        p.freqEnd = freqend
        p.freqStep = freqstep
        p.task = "bscan"
        p.deviceID = task.DeviceID
        p.Fucha = Val(txtFucha10.Text)
        p.Daikuan = Val(txtDaikuan10.Text)
        p.MinDValue = Val(txtMinDValue10.Text)
        p.Legal = New List(Of Double)
        p.WarnNum = Val(txtWarnNum10.Text)
        For Each itm In txtLegal10.Text.Split(Chr(10))
            If itm <> "" Then
                If IsNumeric(itm) Then
                    p.Legal.Add(Val(itm))
                End If
            End If
        Next
        TaskCode = JsonConvert.SerializeObject(p)
        If TaskCode = "" Or task.TaskNickName = "" Or task.DeviceID = "" Or task.StartTime = "" Or task.EndTime = "" Then
            MsgBox("请完整配置参数")
            Exit Sub
        End If
        task.TaskCode = TaskCode
        task.TaskBg = txtTaskBg.Text
        Dim msg As String = JsonConvert.SerializeObject(task)
        Dim result As String = GetH(ServerUrl, "func=AddTask&token=" & token & "&TaskJson=" & msg)
        Dim res As String = GetResultPara("result", result)
        If res = "success" Then
            Dim w As New WarnBox("命令下发成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If
    End Sub

    Private Sub cbDeviceID3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If cbDeviceID3.Items.Count = 0 Then Return
        If cbDeviceID3.SelectedIndex < 0 Then Return
        Dim deviceID As String = cbDeviceID3.SelectedItem
        Dim kind As String = ""
        SyncLock alldevlist
            For Each itm In alldevlist
                If itm.Name = deviceID Then
                    deviceID = itm.DeviceID
                    kind = itm.Kind
                    Exit For
                End If
            Next
        End SyncLock

        If kind.ToUpper = "TZBQ" Then
            Dim sb As New StringBuilder
            sb.AppendLine("400.000")
            sb.AppendLine("435.000")
            sb.AppendLine("446.000")
            sb.AppendLine("463.000")
            sb.AppendLine("495.000")
            RTB3.Text = sb.ToString
            RTB3.Text = "434.550" & vbCrLf & "439.550"
            'Panel11.Visible = True
            'Panel12.Visible = False
            'Panel11.Height = 260
            'RTB3.Height = 250
        End If

        If kind.ToUpper = "TSS" Then
            RTB3.Text = "434.550" & vbCrLf & "439.550"
            'Panel11.Visible = True
            'Panel12.Visible = True
            'Panel12.Top = Panel11.Top
        End If
    End Sub

    Private Sub RTB0_TextChanged(sender As Object, e As EventArgs)
        'Dim pdList As New List(Of Double)
        'For Each itm In RTB0.Text.Split(Chr(10))
        '    If itm <> "" Then
        '        If IsNumeric(itm) Then
        '            pdList.Add(Val(itm))
        '        End If
        '    End If
        'Next
        'Label167.Text = pdList.Count
    End Sub

    Private Sub cbDeviceID3_SelectedIndexChanged_1(sender As Object, e As EventArgs) Handles cbDeviceID3.SelectedIndexChanged

    End Sub

    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        For i = 0 To cbDeviceID3.Items.Count - 1
            Dim itm As String = cbDeviceID3.Items(i).ToString
            Dim str As String = itm

            If str.Contains("长安") Then
                cbDeviceID3.SetItemChecked(i, True)
            End If
        Next
    End Sub
End Class