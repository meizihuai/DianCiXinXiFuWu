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
Imports System.Net.NetworkInformation

Public Class Form1
    Dim gisurl As String = "http://123.207.31.37:8082/baidumap.html"
    Dim title As String = "电磁信息云服务" & Version
    Dim TSSIco As String = "http://123.207.31.37:8082/bmapico/TSS.png"
    Dim TZBQIco As String = "http://123.207.31.37:8082/bmapico/TZBQ.png"
    Dim BeijingIco As String = "http://123.207.31.37:8082/bmapico/beijing.png"
    Public selectDeviceID As String
    Public selectDeviceKind As String
    Dim selectOnlineSignalPanel As OnlineSignal
    Dim selectOnlineFreqPanel As OnlineFreq
    Dim selectAllwarmPanel As Allwarns
    Private myUIstring As String
    Public isSelected As Boolean = False
    Dim ut As uiTree
    Private myDeviceList As List(Of String)
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        End
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        SetWebBrowserFeatures(9)
        Me.DoubleBuffered = True
        Me.MaximizeBox = True
        Me.Text = title
        Me.WindowState = FormWindowState.Maximized
        NotifyList = New List(Of notifyStu)
        Dim int As Integer
        int = 13
        myDeviceList = New List(Of String)
        'Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        'WebBrowser1.Navigate(gisurl)
        'iniGuidePanel()
        'Dim g As Graphics = PictureBox1.CreateGraphics
        'g.SmoothingMode = SmoothingMode.AntiAlias
        'g.InterpolationMode = InterpolationMode.HighQualityBicubic
        'g.CompositingQuality = CompositingQuality.AssumeLinear
        'For i = 0 To 10
        '    AddNotify(New notifyStu("系统上线", "运行正常", ""))
        'Next
        Dim thspeed As New Thread(AddressOf speedControl)
        thspeed.Start()
        Dim th As New Thread(AddressOf ini)
        th.Start()
        Dim th2 As New Thread(AddressOf GetNewDeviceList)
        th2.Start()
    End Sub
    Private Sub speedControl()
        While True
            Sleep(1000)
            Try
                Dim u As String = GetLen(upload)
                Dim w As String = GetLen(dowload)
                upload = 0
                dowload = 0
                Dim str As String = "↑ {0} ↓ {1}"
                str = String.Format(str, u, w)
                lblSpeed.Text = str
            Catch ex As Exception

            End Try
        End While
    End Sub
    Private Function GetLen(ByVal k As Double) As String
        Dim str As String = ""
        If k < 1024 Then
            Return k.ToString("0.00") & " b/s"
        End If
        Dim d As Double = 1024 * 1024
        If 1024 <= k And k < d Then
            Return (k / 1024).ToString("0.00") & " kb/s"
        End If

        Return (k / d).ToString("0.00") & " Mb/s"
    End Function
    Private Sub GetNewDeviceList()
        While True
            Try
                'Dim hh As String = ServerUrl & "func=getalldevlist" & "&token=" & token
                'Me.Invoke(Sub() Clipboard.SetText(hh))
                Dim result As String = GetServerResult("func=getalldevlist")
                Dim newDevList As List(Of deviceStu)
                newDevList = JsonConvert.DeserializeObject(result, GetType(List(Of deviceStu)))
                If IsNothing(newDevList) = False Then
                    If IsNothing(myDeviceList) = False Then
                        For i = newDevList.Count - 1 To 0 Step -1
                            Dim itm As String = newDevList(i).DeviceID
                            If myDeviceList.Contains(itm) = False Then
                                newDevList.RemoveAt(i)
                            End If
                        Next
                    End If
                    If newDevList.Count < alldevlist.Count Then
                        Dim msg As String = "发现设备掉线"
                        ShowNewSsytemMsg(msg)
                        Dim chaji As List(Of deviceStu) = alldevlist.Except(newDevList).ToList
                        msg = "设备掉线:"
                        For Each itm In chaji
                            msg = msg + itm.Name & ","
                        Next
                        msg = msg.Substring(0, msg.Length - 1)
                        AddNotify(New notifyStu("设备掉线", msg, ""))
                        'For Each oldDeivce In alldevlist
                        '    Dim isFind As Boolean = False
                        '    For Each newDevice In newDevList
                        '        If oldDeivce.Name = newDevice.Name Then
                        '            isFind = True
                        '            Exit For
                        '        End If
                        '    Next
                        'Next
                        GetOnlineDevice()
                    End If
                    If newDevList.Count > alldevlist.Count Then
                        Dim msg As String = "发现新设备上线"
                        ShowNewSsytemMsg(msg)
                        Dim chaji As List(Of deviceStu) = newDevList.Except(alldevlist).ToList
                        msg = "设备上线:"
                        For Each itm In chaji
                            msg = msg + itm.Name & ","
                        Next
                        msg = msg.Substring(0, msg.Length - 1)
                        AddNotify(New notifyStu("设备上线", msg, ""))
                        GetOnlineDevice()
                    End If
                End If
            Catch ex As Exception

            End Try
            Sleep(5000)
        End While
    End Sub
    Private Sub ShowNewSsytemMsg(ByVal msg As String)
        Dim th As New Thread(AddressOf th_sub_ShowNewSsytemMsg)
        th.Start(msg)
    End Sub
    Private Sub th_sub_ShowNewSsytemMsg(ByVal msg As String)
        lblSystemMsg.ForeColor = Color.Red
        Me.Invoke(Sub() lblSystemMsg.Text = msg)
        Sleep(3000)
        lblSystemMsg.Text = "无系统消息"
        lblSystemMsg.ForeColor = Color.FromArgb(255, 224, 192)
    End Sub
    Private Sub defaultShow()
        'Dim defaultPanel As New TaskListPanel
        Dim defaultPanel As New DeviceAllMap
        ShowPanel(defaultPanel)
    End Sub
    Private Sub iniGuidePanelByFilter()
        Panel_Guide.Controls.Clear()
        myUIstring = myUIstring.Replace(vbCr, "").Replace(vbLf, "")
        Dim st() As String = myUIstring.Split("@")
        ut = New uiTree("main")
        For Each sh In st
            If sh = "" Then Continue For
            If InStr(sh, ":") = False Then Continue For
            Dim sk() As String = sh.Split(":")
            Dim sons() As String = sk(1).Split(",")
            If sk(0) <> "" Then
                ut.AddNode(sk(0))
                For Each s In sons
                    If s <> "" Then
                        If s = "系统账号管理" Then
                            If myPower = 1 Or myPower = 9 Then
                                ut.item(sk(0)).AddNode(s)
                            End If
                        Else
                            ut.item(sk(0)).AddNode(s)
                        End If
                    End If                
                Next
            End If            
        Next
        For j = ut.NodeList.Count - 1 To 0 Step -1
            Dim itm As uiTree = ut.NodeList(j)
            Dim p As fatherUiNode = itm.pfather
            Panel_Guide.Controls.Add(p)
            AddHandler p.beClick, AddressOf p_beClick
        Next
        For Each itm In ut.NodeList
            AddHandler itm.OnFatherUiNodeUiOpen, AddressOf On_FatherUiNodeUiOpen
        Next
    End Sub

    Private Sub iniGuidePanel()
        If myUIstring = "" Then Return
        If myUIstring <> "all" Then
            iniGuidePanelByFilter()
            Return
        End If
        Panel_Guide.Controls.Clear()
        ut = New uiTree("main")
        ut.AddNode("值班管理")
        'ut.item("值班管理").AddNode("刷新在线设备")
        ut.item("值班管理").AddNode("任务文档管理")
        ut.item("值班管理").AddNode("值班工作日志")

        ut.item("值班管理").AddNode("值班任务日志")

        ut.item("值班管理").AddNode("在线事件管理")
        ut.item("值班管理").AddNode("在线任务管理")


        ut.AddNode("设备管理")
        ut.item("设备管理").AddNode("全网设备分布")
        ut.item("设备管理").AddNode("服务设备分布")
        ut.item("设备管理").AddNode("设备列表")
        ut.item("设备管理").AddNode("设备网关")
        ut.item("设备管理").AddNode("监测网关")
        ut.item("设备管理").AddNode("设备信息")
        ut.item("设备管理").AddNode("设备设置")

        'ut.item("值班管理").AddNode("在线信息")
        'ut.AddNode("频谱传感器")
        'ut.AddNode("微型传感器")
        ut.AddNode("监测服务")
        ut.AddNode("信息服务")
        ut.item("信息服务").AddNode("设备工作日志")
        ut.item("信息服务").AddNode("报告文档")
        ' ut.item("信息服务").AddNode("报表文档")
        ut.item("信息服务").AddNode("音频取样")
        '  ut.item("信息服务").AddNode("频谱取样")
        ut.item("信息服务").AddNode("截图上报")
        ut.item("信息服务").AddNode("截图管理")

        ut.AddNode("智能监测")
        ut.item("智能监测").AddNode("POA定位")

        ut.AddNode("公交系统")
        ut.item("公交系统").AddNode("频谱地图")
        ut.item("公交系统").AddNode("历史频谱")
        ' ut.item("公交系统").AddNode("任务报告")
        ut.AddNode("监测车系统")
        ut.item("监测车系统").AddNode("频谱地图")
        ut.item("监测车系统").AddNode("历史频谱")

        ut.AddNode("数据服务")
        ut.item("数据服务").AddNode("频谱数据")
        ut.item("数据服务").AddNode("台站数据")
        ut.item("数据服务").AddNode("预警数据")

        ut.AddNode("系统管理")
        ut.item("系统管理").AddNode("刷新在线设备")
        If myPower = 1 Or myPower = 9 Then
            ut.item("系统管理").AddNode("系统账号管理")
        End If
        ut.item("系统管理").AddNode("更改服务器地址")

        For j = ut.NodeList.Count - 1 To 0 Step -1
            Dim itm As uiTree = ut.NodeList(j)
            Dim p As fatherUiNode = itm.pfather
            Panel_Guide.Controls.Add(p)
            AddHandler p.beClick, AddressOf p_beClick
        Next
        For Each itm In ut.NodeList
            AddHandler itm.OnFatherUiNodeUiOpen, AddressOf On_FatherUiNodeUiOpen
        Next
    End Sub
    Private Sub On_FatherUiNodeUiOpen(ByVal name As String)
        If IsNothing(ut) Then Return
        For Each itm In ut.NodeList
            If itm.name <> name Then
                itm.pfather.uiClose()
            End If
        Next
    End Sub
    Public Sub p_beClick(ByVal name As String)
        If InStr(name, ",") < 0 Then
            Exit Sub
        End If
        Dim st() As String = name.Split(",")
        If st.Count <> 2 Then Exit Sub
        Dim fatherName As String = st(0)
        Dim SonName As String = st(1)
        If IsNothing(selectGisPanel) = False Then
            selectGisPanel.CloseFunctionsUserPanel()
            selectGisPanel = Nothing
        End If
        If IsNothing(selectOnlineFreqPanel) = False Then
            selectOnlineFreqPanel.stopALL()
        End If
        If IsNothing(selectOnlineSignalPanel) = False Then
            selectOnlineSignalPanel.stopALL()
        End If
        If IsNothing(selectAllwarmPanel) = False Then
            selectAllwarmPanel.stopALL()
        End If
        If fatherName = "值班管理" Then
            If SonName = "刷新在线设备" Then
                GetOnlineDevice()
                Dim w As New WarnBox("已刷新在线设备！")
                w.Show()
            End If

            If SonName = "任务文档管理" Then
                Dim defaultPanel As New TaskListPanel
                ShowPanel(defaultPanel)
            End If
            If SonName = "值班任务日志" Then
                Dim defaultPanel As New DutyLog
                ShowPanel(defaultPanel)
            End If
            If SonName = "值班工作日志" Then
                Dim defaultPanel As New LogPanel
                ShowPanel(defaultPanel)
            End If

            If SonName = "在线事件管理" Then
                Dim defaultPanel As New Allwarns
                ShowPanel(defaultPanel)
                selectAllwarmPanel = defaultPanel
            End If
            If SonName = "在线任务管理" Then
                Dim defaultPanel As New OnlineTask
                ShowPanel(defaultPanel)
            End If

        End If
        If fatherName = "设备管理" Then
            If SonName = "全网设备分布" Then
                Dim defaultPanel As New OtherSystemDevice
                ShowPanel(defaultPanel)
            End If
            If SonName = "服务设备分布" Then
                Dim defaultPanel As New DeviceAllMap()
                ShowPanel(defaultPanel)
            End If
            If SonName = "设备列表" Then
                Dim p As New AllDeviceList
                ShowPanel(p)
            End If
            If SonName = "设备网关" Then
                Dim p As New TSSGateWayListPanel
                ShowPanel(p)
            End If
            If SonName = "监测网关" Then
                Dim p As New INGDeviceList
                ShowPanel(p)
            End If

            If SonName = "设备信息" Then
                SelectDeviceWithFunction(selectDeviceID, False)
            End If
            If SonName = "设备设置" Then
                Dim p As New DeviceSetting
                ShowPanel(p)
            End If
        End If
        If fatherName = "智能监测" Then
            If SonName = "POA定位" Then
                Dim p As New POAPanel
                ShowPanel(p)
            End If
        End If
        If fatherName = "监测服务" Then
            If SonName = "固定频率测量" Then
                'TSS
                Dim p As New OnlineFreq(selectDeviceID, False, 0)
                ShowPanel(p)
                selectOnlineFreqPanel = p
            End If
            'If SonName = "中频分析" Then
            '    If selectDeviceKind = "TZBQ" Then
            '        Dim p As New OnlineSignal(selectDeviceID, False, 0)
            '        ShowPanel(p)
            '        selectOnlineSignalPanel = p
            '    End If
            '    If selectDeviceKind = "TSS" Then
            '        Dim p As New OnlineFreq(selectDeviceID, False, 1)
            '        ShowPanel(p)
            '        selectOnlineFreqPanel = p
            '    End If
            'End If

            If SonName = "频谱监测" Or SonName = "中频分析" Or SonName = "频段扫描" Then
                'TZBQ+TSS
                If selectDeviceKind = "TZBQ" Then
                    Dim p As New OnlineSignal(selectDeviceID, False, 0)
                    ShowPanel(p)
                    selectOnlineSignalPanel = p
                End If
                If selectDeviceKind = "TSS" Then
                    Dim p As New OnlineFreq(selectDeviceID, False, 1)
                    ShowPanel(p)
                    selectOnlineFreqPanel = p
                End If
            End If
            If SonName = "离散扫描" Then
                'TZBQ+TSS
                If selectDeviceKind = "TZBQ" Then
                    Dim p As New OnlineSignal(selectDeviceID, False, 1)
                    ShowPanel(p)
                    selectOnlineSignalPanel = p
                End If
                If selectDeviceKind = "TSS" Then
                    Dim p As New OnlineFreq(selectDeviceID, False, 2)
                    ShowPanel(p)
                    selectOnlineFreqPanel = p
                End If
            End If
            If SonName = "条件捕获" Then
                'TZBQ+TSS
                If selectDeviceKind = "TZBQ" Then
                    Dim p As New OnlineSignal(selectDeviceID, False, 2)
                    ShowPanel(p)
                    selectOnlineSignalPanel = p
                End If
                If selectDeviceKind = "TSS" Then
                    Dim p As New OnlineFreq(selectDeviceID, False, 3)
                    ShowPanel(p)
                    selectOnlineFreqPanel = p
                End If
            End If

            If SonName = "监督评估" Then
                'TZBQ
                Dim p As New OnlineSignal(selectDeviceID, False, 3)
                ShowPanel(p)
                selectOnlineSignalPanel = p
            End If
        End If
        If fatherName = "公交系统" Then
            If SonName = "频谱地图" Then
                Dim defaultPanel As New BusFreqGis
                ShowPanel(defaultPanel)
            End If
            If SonName = "历史频谱" Then
                Dim defaultPanel As New BusHisFreqGis
                ShowPanel(defaultPanel)
            End If
            If SonName = "任务报告" Then
                'Dim defaultPanel As New BusFreqGis
                'ShowPanel(defaultPanel)
            End If
        End If
        If fatherName = "监测车系统" Then
            If SonName = "频谱地图" Then
                Dim defaultPanel As New CarFreqGis
                ShowPanel(defaultPanel)
            End If
            If SonName = "历史频谱" Then
                Dim defaultPanel As New CarHisFreqGis
                ShowPanel(defaultPanel)
            End If
        End If
        If fatherName = "数据服务" Then
            If SonName = "频谱数据" Then
                Dim defaultPanel As New freqTaskPanel
                ShowPanel(defaultPanel)
            End If
            If SonName = "台站数据" Then
                Dim defaultPanel As New SignalTaskPanel
                ShowPanel(defaultPanel)
            End If
            If SonName = "预警数据" Then
                'WarnsPanel
                Dim defaultPanel As New WarnsPanel
                ShowPanel(defaultPanel)
            End If
        End If

        If fatherName = "微型传感器" Then
            If IsNothing(selectGisPanel) = False Then
                selectGisPanel.CloseFunctionsUserPanel()
                selectGisPanel = Nothing
            End If
            Dim defaultPanel As New GisPanel(SonName)
            isSelected = True
            selectDeviceID = SonName
            selectDeviceKind = "TZBQ"
            Label2.Text = "选中  " & "微型传感器" & "  " & selectDeviceID
            ShowPanel(defaultPanel)
            selectGisPanel = defaultPanel
            ut.item("监测服务").Clear()
            'ut.item("监测服务").AddNode("频谱监测")
            'ut.item("监测服务").AddNode("离散扫描")
            'ut.item("监测服务").AddNode("条件捕获")
            'ut.item("监测服务").AddNode("台站监督")
            ut.item("监测服务").AddNode("频谱监测")
            ut.item("监测服务").AddNode("监督评估")
            ut.item("监测服务").AddNode("条件捕获")
            'ut.item("监测服务").AddNode("POA定位(开发中)")
            ut.item("监测服务").AddNode("频段扫描")
            ut.item("监测服务").AddNode("离散扫描")
        End If
        If fatherName = "频谱传感器" Then
            If IsNothing(selectGisPanel) = False Then
                selectGisPanel.CloseFunctionsUserPanel()
                selectGisPanel = Nothing
            End If
            isSelected = True
            Dim defaultPanel As New GisPanel(SonName)
            selectDeviceID = SonName
            selectDeviceKind = "TSS"
            Label2.Text = "选中  " & "频谱传感器" & "  " & selectDeviceID
            ShowPanel(defaultPanel)
            selectGisPanel = defaultPanel
            ut.item("监测服务").Clear()
            'ut.item("监测服务").AddNode("远端侦听")
            'ut.item("监测服务").AddNode("频谱监测")
            'ut.item("监测服务").AddNode("离散扫描")
            'ut.item("监测服务").AddNode("条件捕获")
            ut.item("监测服务").AddNode("固定频率测量")
            ut.item("监测服务").AddNode("中频分析")
            ut.item("监测服务").AddNode("频谱监测")
            ut.item("监测服务").AddNode("频段扫描")
            ut.item("监测服务").AddNode("离散扫描")
            'ut.item("监测服务").AddNode("测向定位(开发中)")
            ut.item("监测服务").AddNode("条件捕获")
        End If
        If fatherName = "信息服务" Then
            If SonName = "设备工作日志" Then
                Dim defaultPanel As New DeviceLog
                ShowPanel(defaultPanel)
            End If
            If SonName = "截图管理" Then
                Dim defaultPanel As New HistoryImg
                ShowPanel(defaultPanel)
            End If
            If SonName = "音频取样" Then
                Dim defaultPanel As New HistoryAudio
                ShowPanel(defaultPanel)
            End If
            If SonName = "报告文档" Then
                Dim defaultPanel As New HistoryWord
                ShowPanel(defaultPanel)
            End If
            If SonName = "报表文档" Then
                Dim defaultPanel As New HistoryExcel
                ShowPanel(defaultPanel)
            End If
            If SonName = "截图上报" Then
                Dim bitmap As Bitmap = jp()
                Dim ri As New RunImage(bitmap)
                ri.Show()
            End If
        End If

        If fatherName = "系统管理" Then
            If SonName = "刷新在线设备" Then
                GetOnlineDevice()
                Dim w As New WarnBox("已刷新在线设备！")
                w.Show()
            End If
            If SonName = "系统账号管理" Then
                Dim defaultPanel As New usrManager
                ShowPanel(defaultPanel)
            End If
            If SonName = "更改服务器地址" Then
                IpSettingFrm.Show()
            End If
        End If
    End Sub
    Public Sub SelectDevice(ByVal DeviceName As String)

        Dim kind As String
        Dim isFind As Boolean = False
        For Each d In alldevlist
            If d.Name = DeviceName Then
                kind = d.Kind
                isFind = True
                Exit For
            End If
        Next
        For Each d In alldevlist

            If d.DeviceID = DeviceName Then
                kind = d.Kind
                isFind = True
                Exit For
            End If
        Next

        If isFind = False Then Return
        'If IsNothing(selectGisPanel) = False Then
        '    selectGisPanel.CloseFunctionsUserPanel()
        '    selectGisPanel = Nothing
        'End If
        ' Dim defaultPanel As New GisPanel(DeviceName)
        isSelected = True
        selectDeviceID = DeviceName
        selectDeviceKind = kind

        'ShowPanel(defaultPanel)
        'selectGisPanel = defaultPanel
        Try
            ut.item("监测服务").Clear()
            'ut.item("监测服务").AddNode("频谱监测")
            'ut.item("监测服务").AddNode("离散扫描")
            'ut.item("监测服务").AddNode("条件捕获")
            'ut.item("监测服务").AddNode("台站监督")
            If kind = "TZBQ" Then
                Label2.Text = "选中  " & "微型传感器" & "  " & selectDeviceID
                ut.item("监测服务").AddNode("频谱监测")
                ut.item("监测服务").AddNode("监督评估")
                ut.item("监测服务").AddNode("条件捕获")
                'ut.item("监测服务").AddNode("POA定位(开发中)")
                ut.item("监测服务").AddNode("频段扫描")
                ut.item("监测服务").AddNode("离散扫描")

            End If
            If kind = "TSS" Then
                Label2.Text = "选中  " & "频谱传感器" & "  " & selectDeviceID
                ut.item("监测服务").AddNode("固定频率测量")
                ut.item("监测服务").AddNode("中频分析")
                ut.item("监测服务").AddNode("频谱监测")
                ut.item("监测服务").AddNode("频段扫描")
                ut.item("监测服务").AddNode("离散扫描")
                'ut.item("监测服务").AddNode("测向定位(开发中)")
                ut.item("监测服务").AddNode("条件捕获")
            End If
            If kind = "ING" Then
                Label2.Text = "选中  " & "监测网关" & "  " & selectDeviceID
                ut.item("监测服务").AddNode("固定频率测量")
                ut.item("监测服务").AddNode("中频分析")
                ut.item("监测服务").AddNode("频谱监测")
                ut.item("监测服务").AddNode("频段扫描")
                ut.item("监测服务").AddNode("离散扫描")
                ut.item("监测服务").AddNode("条件捕获")
                ut.item("监测服务").AddNode("监督评估")
            End If
        Catch ex As Exception

        End Try


    End Sub
    Public Sub SelectDeviceWithFunction(ByVal DeviceName As String, ByVal isFunctionView As Boolean)
        Dim kind As String
        Dim isFind As Boolean = False
        If IsNothing(alldevlist) Then Return
        For Each d In alldevlist
            If d.Name = DeviceName Then
                kind = d.Kind
                isFind = True
                Exit For
            End If
        Next
        If isFind = False Then Return
        If IsNothing(selectGisPanel) = False Then
            selectGisPanel.CloseFunctionsUserPanel()
            selectGisPanel = Nothing
        End If
        Dim defaultPanel As New GisPanel(DeviceName, isFunctionView)
        isSelected = True
        selectDeviceID = DeviceName
        selectDeviceKind = kind

        ShowPanel(defaultPanel)
        selectGisPanel = defaultPanel
        ut.item("监测服务").Clear()
        'ut.item("监测服务").AddNode("频谱监测")
        'ut.item("监测服务").AddNode("离散扫描")
        'ut.item("监测服务").AddNode("条件捕获")
        'ut.item("监测服务").AddNode("台站监督")
        If kind = "TZBQ" Then
            Label2.Text = "选中  " & "微型传感器" & "  " & selectDeviceID
            ut.item("监测服务").AddNode("频谱监测")
            ut.item("监测服务").AddNode("监督评估")
            ut.item("监测服务").AddNode("条件捕获")
            'ut.item("监测服务").AddNode("POA定位(开发中)")
            ut.item("监测服务").AddNode("频段扫描")
            ut.item("监测服务").AddNode("离散扫描")
        End If
        If kind = "TSS" Then
            Label2.Text = "选中  " & "频谱传感器" & "  " & selectDeviceID
            ut.item("监测服务").AddNode("固定频率测量")
            ut.item("监测服务").AddNode("中频分析")
            ut.item("监测服务").AddNode("频谱监测")
            ut.item("监测服务").AddNode("频段扫描")
            ut.item("监测服务").AddNode("离散扫描")
            'ut.item("监测服务").AddNode("测向定位(开发中)")
            ut.item("监测服务").AddNode("条件捕获")
        End If
        If kind = "ING" Then
            Label2.Text = "选中  " & "监测网关" & "  " & selectDeviceID
            ut.item("监测服务").AddNode("固定频率测量")
            ut.item("监测服务").AddNode("中频分析")
            ut.item("监测服务").AddNode("频谱监测")
            ut.item("监测服务").AddNode("频段扫描")
            ut.item("监测服务").AddNode("离散扫描")
            ut.item("监测服务").AddNode("条件捕获")
            ut.item("监测服务").AddNode("监督评估")
        End If
    End Sub
    Private Function jp() As Bitmap
        Try
            Dim p1 As New Point(Me.Left, Me.Top)
            Dim p2 As New Point(Me.Width, Me.Height)
            Dim pic As New Bitmap(p2.X, p2.Y)
            Dim p3 As New Point(0, 0)
            Using g As Graphics = Graphics.FromImage(pic)
                g.CopyFromScreen(p1, p3, p2)
                g.Save()
                Return pic
            End Using
        Catch ex As Exception
            '‘MsgBox(ex.ToString)
        End Try
    End Function
    Private Sub ShowPanel(ByVal p As Object)
        Me.Invoke(Sub() PanelMain.Controls.Clear())
        Me.Invoke(Sub() PanelMain.Controls.Add(p))
    End Sub
    Public Sub thini()
        iniGuidePanel()
        Dim th As New Thread(AddressOf ini)
        th.Start()
    End Sub
    Private Sub ini()
        Label2.Text = "未选中设备"
        isSelected = False
        selectDeviceID = ""
        lblSystemMsg.Text = "正在初始化……"
        Dim path As String = Application.StartupPath & "\config.ini"
        If File.Exists(path) Then
            Dim sr As New StreamReader(path, Encoding.Default)
            Dim str As String = sr.ReadToEnd
            sr.Close()
            myConnectConfig = New myConfig("123.207.31.37", 8080)
            Try
                myConnectConfig = JsonConvert.DeserializeObject(str, GetType(myConfig))
            Catch ex As Exception
                myConnectConfig = New myConfig("123.207.31.37", 8080)
                Dim js As String = JsonConvert.SerializeObject(myConnectConfig)
                Dim sw As New StreamWriter(path, False, Encoding.Default)
                sw.Write(js)
                sw.Close()
            End Try
            Dim ip As String = myConnectConfig.serverIP
            ServerIP = ip
            ' remoteServerIP = ServerIP
            ServerUrl = "http://" & myConnectConfig.serverIP & ":" & myConnectConfig.serverPort & "/?"

            '  Label4.Text = "当前服务器地址:" & ServerIP
        Else
            myConnectConfig = New myConfig("123.207.31.37", 8080)
            Dim js As String = JsonConvert.SerializeObject(myConnectConfig)
            Dim sw As New StreamWriter(path, False, Encoding.Default)
            sw.Write(js)
            sw.Close()
        End If

        Dim bk As String = Label2.Text
        Label2.Text = "正在检测公网连接……"
        Dim p As New Ping
        Dim ps As PingReply = p.Send("123.207.31.37")
        If ps.Status = IPStatus.Success Then
            isLoadGis = True
        Else
            isLoadGis = False
        End If
        Label2.Text = bk
        If Login() = True Then
            lblSystemMsg.Text = "正在获取权限……"
            Dim result As String = GetServerResult("func=GetMyPower")
            Dim r As String = GetNorResult("result", result)
            Dim msg As String = GetNorResult("msg", result)
            Dim errmsg As String = GetNorResult("errmsg", result)
            If r = "success" Then
                Dim power As String = GetNorResult("power", result)
                myPower = Val(power)
                Dim txtUsr As String = usr
                If myPower = 0 Then
                    MsgBox("您的账号尚未审核，请联系上级")
                    End
                End If
                If myPower = 1 Then
                    txtUsr = usr & "[领导]"
                End If
                If myPower = 2 Then
                    txtUsr = usr & "[值班员]"
                End If
                If myPower = 9 Then
                    txtUsr = usr & "[管理员]"
                End If
                lblUser.Text = txtUsr
                GetMyUI()
                GetMyDeviceList()
            Else
                Dim sb As New StringBuilder
                sb.AppendLine("获取权限失败")
                sb.AppendLine(msg)
                sb.AppendLine(errmsg)
                MsgBox(sb.ToString)
            End If
            lblSystemMsg.Text = "更新导航栏……"
            Me.Invoke(Sub() iniGuidePanel())
            lblSystemMsg.Text = "获取在线设备……"
            GetOnlineDevice()
            lblSystemMsg.Text = "显示主页……"
            Me.Invoke(Sub() defaultShow())
        End If
        lblSystemMsg.Text = "无系统消息"
        If ServerIP = "123.207.31.37" Then
            Me.Text = title & "  [ " & "云服务" & " ]"
        ElseIf ServerIP = "61.145.180.149" Then
            Me.Text = title & "  [ " & "东莞网" & " ]"
        Else
            Me.Text = title & "  [ " & ServerIP & " ]"
        End If

        'Dim P_TSS_H As New Panel_TSS_Hander("0320170001")
        'Me.Invoke(Sub() PanelMain.Controls.Clear())
        'Me.Invoke(Sub() PanelMain.Controls.Add(P_TSS_H))
        'P_TSS_H.Left = 0
        'P_TSS_H.Top = 0
        'P_TSS_H.Width = PanelMain.Width
        'P_TSS_H.Height = PanelMain.Height
    End Sub
    Private Sub GetMyDeviceList()
        Try
            lblSystemMsg.Text = "正在处理权限2……"
            Dim result As String = GetServerResult("func=GetMyDevice")
            Dim r As String = GetNorResult("result", result)
            Dim msg As String = GetNorResult("msg", result)
            Dim Device As String = GetNorResult("device", result)
            Dim errmsg As String = GetNorResult("errmsg", result)
            If r = "success" Then
                If Device = "all" Then
                    myDeviceList = Nothing
                Else
                    myDeviceList = New List(Of String)
                    For Each sh In Device.Split(",")
                        myDeviceList.Add(sh)
                    Next
                End If
            Else
                Dim sb As New StringBuilder
                sb.AppendLine("获取myDevice失败")
                sb.AppendLine(msg)
                sb.AppendLine(errmsg)
                MsgBox(sb.ToString)
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub GetMyUI()
        Try
            lblSystemMsg.Text = "正在处理权限1……"
            Dim result As String = GetServerResult("func=GetMyUI")
            Dim r As String = GetNorResult("result", result)
            Dim msg As String = GetNorResult("msg", result)
            Dim ui As String = GetNorResult("ui", result)
            Dim errmsg As String = GetNorResult("errmsg", result)
            If r = "success" Then
                myUIstring = ui
            Else
                Dim sb As New StringBuilder
                sb.AppendLine("获取UI失败")
                sb.AppendLine(msg)
                sb.AppendLine(errmsg)
                MsgBox(sb.ToString)
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        lbl_Time.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
    End Sub
    Public Sub GetOnlineDevice()
        Dim resutl As String = GetH(ServerUrl, "func=getalldevlist&token=" & token)
        If GetResultPara("result", resutl) = "fail" Then
            If GetResultPara("msg", resutl) = "Please login" Then
                Login()
                resutl = GetH(ServerUrl, "func=getalldevlist&token=" & token)
            End If
        End If
        If resutl = "" Then
            MsgBox("网络错误，请检查您的网络")
            Exit Sub
        End If
        If resutl = "" Or resutl = "[]" Then Return
        alldevlist = JsonConvert.DeserializeObject(resutl, GetType(List(Of deviceStu)))

        If IsNothing(alldevlist) = False Then
            'ShowDeviceOnWebGis("")
            If IsNothing(myDeviceList) = False Then
                For i = alldevlist.Count - 1 To 0 Step -1
                    Dim itm As String = alldevlist(i).DeviceID
                    If myDeviceList.Contains(itm) = False Then

                        alldevlist.RemoveAt(i)
                    End If
                Next
            End If
            Me.Invoke(Sub()
                          'ut.item("微型传感器").Clear()
                          'ut.item("频谱传感器").Clear()
                          Try
                              For Each dev In alldevlist
                                  Dim name As String = dev.Name
                                  Dim kind As String = dev.Kind
                                  If kind = "TZBQ" Then
                                      ' ut.item("微型传感器").AddNode(name)
                                      If isSelected = False Then
                                          isSelected = True
                                          selectDeviceID = name
                                          selectDeviceKind = "TZBQ"
                                          Label2.Text = "选中  " & "微型传感器" & "  " & selectDeviceID
                                          ut.item("监测服务").Clear()
                                          'ut.item("监测服务").AddNode("频谱监测")
                                          'ut.item("监测服务").AddNode("离散扫描")
                                          'ut.item("监测服务").AddNode("条件捕获")
                                          'ut.item("监测服务").AddNode("台站监督")
                                          ut.item("监测服务").AddNode("频谱监测")
                                          ut.item("监测服务").AddNode("监督评估")
                                          ut.item("监测服务").AddNode("条件捕获")
                                          'ut.item("监测服务").AddNode("POA定位(开发中)")
                                          ut.item("监测服务").AddNode("频段扫描")
                                          ut.item("监测服务").AddNode("离散扫描")
                                      End If
                                  End If
                                  If kind = "TSS" Then
                                      'ut.item("频谱传感器").AddNode(name)
                                      If isSelected = False Then
                                          isSelected = True
                                          selectDeviceID = name
                                          selectDeviceKind = "TSS"
                                          Label2.Text = "选中  " & "频谱传感器" & "  " & selectDeviceID
                                          ut.item("监测服务").Clear()
                                          'ut.item("监测服务").AddNode("远端侦听")
                                          'ut.item("监测服务").AddNode("频谱监测")
                                          'ut.item("监测服务").AddNode("离散扫描")
                                          'ut.item("监测服务").AddNode("条件捕获")
                                          ut.item("监测服务").AddNode("固定频率测量")
                                          ut.item("监测服务").AddNode("中频分析")
                                          ut.item("监测服务").AddNode("频谱监测")
                                          ut.item("监测服务").AddNode("频段扫描")
                                          ut.item("监测服务").AddNode("离散扫描")
                                          'ut.item("监测服务").AddNode("测向定位(开发中)")
                                          ut.item("监测服务").AddNode("条件捕获")
                                      End If
                                  End If

                                  'ut.item("服务设备分布图").AddNode(name)
                              Next
                              '  SelectDevice("ING_180501")
                          Catch ex As Exception

                          End Try

                          ' SelectDevice("上海淮海中路TS7030005")

                      End Sub)

        End If
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        '  Label2.Left = (Panel1.Width - Panel2.Left) * 0.5 - Label2.Width * 0.5
    End Sub

    Private Sub Label5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        IpSettingFrm.Show()
    End Sub
    Dim isNotepadShowed As Boolean = False
    Private Sub PictureBox12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If isNotepadShowed Then
            isNotepadShowed = False
            Notepad.Close()
        Else
            isNotepadShowed = True
            Notepad.Show()
        End If
    End Sub

    Private Sub Label2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label2.Click
        If selectDeviceID = "" Then Return
        If selectDeviceID <> "" Then
            '  MsgBox(selectDeviceID)
        End If
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        selectDeviceFrm.Show()
    End Sub


    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click
        SelectDeviceWithFunction(selectDeviceID, True)
    End Sub
    Dim isNotifyShowed As Boolean = False
    Private Sub PictureBox5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox5.Click
        If isNotifyShowed Then
            isNotifyShowed = False
            ShowNotify.Close()
        Else
            isNotifyShowed = True
            ShowNotify.Show()
        End If

    End Sub

    Private Sub 安全退出ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 安全退出ToolStripMenuItem.Click
        If File.Exists(Application.StartupPath & "\main.exe") Then
            Shell(Application.StartupPath & "\main.exe", AppWinStyle.NormalFocus)
            End
        End If
        If File.Exists(Application.StartupPath & "\电磁信息云服务.exe") Then
            Shell(Application.StartupPath & "\电磁信息云服务.exe", AppWinStyle.NormalFocus)
            End
        End If
        End
    End Sub

 
    Private Sub PictureBox6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox6.Click
        IpSettingFrm.Show()
    End Sub

    Private Sub PictureBox7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox7.Click
        Dim bitmap As Bitmap = jp()
        Dim ri As New RunImage(bitmap)
        ri.Show()
    End Sub
End Class
