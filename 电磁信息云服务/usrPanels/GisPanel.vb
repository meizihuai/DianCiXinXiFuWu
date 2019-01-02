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
Public Class GisPanel
    'Structure deviceStu
    '    Dim DeviceID As String
    '    Dim Name As String
    '    Dim Address As String
    '    Dim Kind As String
    '    Dim OnlineTime As String
    '    Dim Statu As String
    '    Dim Lng As String
    '    Dim Lat As String
    '    Dim IP As String
    '    Dim Port As Integer
    '    Dim sbzt As String
    '    Dim Func As String
    '    Dim HTTPMsgUrl As String
    'End Structure
    Public selectDeviceID As String
    Public isFunctionView As Boolean = False
    Sub New(ByVal _selectDeviceID As String, ByVal _isFunctionView As Boolean)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        selectDeviceID = _selectDeviceID
        isFunctionView = _isFunctionView
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Sub New(ByVal _selectDeviceID As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        selectDeviceID = _selectDeviceID
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub GisPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        If isFunctionView Then
            If WebGis.Controls.Count = 0 Then
                Dim kind As String = GetDevKindByID(selectDeviceID)
                If kind = "TSS" Then
                    Dim p As New OnlineFreq(selectDeviceID, True, -1)
                    WebGis.Controls.Add(p)
                End If
                If kind = "TZBQ" Then
                    Dim p As New OnlineSignal(selectDeviceID, True, 3)
                    WebGis.Controls.Add(p)
                End If
                If kind = "ING" Then
                    Dim p As New OnlineING(selectDeviceID, True, -1)
                    WebGis.Controls.Add(p)
                End If
            Else
                CloseFunctionsUserPanel()
            End If
            If isLoadGis Then WebGis.Navigate(gisurl)
        Else
            If isLoadGis Then WebGis.Navigate(gisurl)
        End If
        ini()
        RefrushDeviceStatus()
    End Sub
  
    Private Sub ini()
        ToolTip1.SetToolTip(PictureBox2, "打开地理视图")
        ToolTip1.SetToolTip(PictureBox1, "打开功能视图")
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("项目", 60)
        LVDetail.Columns.Add("内容", 150)
        Dim itm As New ListViewItem("设备ID")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("设备备注")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("设备地点")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("设备类型")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("上线时间")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("设备状态")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("经度")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("纬度")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("任务状态")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("功能状态")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
    End Sub
    Private Sub RefrushDeviceStatus()
        Dim ds As deviceStu
        For Each d In alldevlist          
            If d.Name = selectDeviceID Then
                ds = d
                Exit For
            End If
        Next
        If ds.DeviceID = "" Then
            MsgBox("找不到该设备详细信息，请刷新设备列表！")
            Exit Sub
        End If
        LVDetail.Items(0).SubItems(1).Text = ds.DeviceID
        LVDetail.Items(1).SubItems(1).Text = ds.Name
        LVDetail.Items(2).SubItems(1).Text = ds.Address
        LVDetail.Items(3).SubItems(1).Text = ds.Kind
        LVDetail.Items(4).SubItems(1).Text = ds.OnlineTime
        LVDetail.Items(5).SubItems(1).Text = ds.Statu
        LVDetail.Items(6).SubItems(1).Text = ds.Lng
        LVDetail.Items(7).SubItems(1).Text = ds.Lat
        If IsNothing(ds.sbzt) = False Then LVDetail.Items(8).SubItems(1).Text = ds.sbzt
        If IsNothing(ds.Func) = False Then LVDetail.Items(9).SubItems(1).Text = ds.Func
        If ds.Statu = "working" Then
            LVDetail.Items(8).SubItems(1).Text = "正在执行任务……"
        End If
        If ds.Statu = "free" Then
            LVDetail.Items(8).SubItems(1).Text = "没有任务，空闲状态"
        End If
    End Sub
    Private Sub RefrushGis()
        If isLoadGis = False Then Return
        CleanGis(WebGis)
        Dim ds As deviceStu
        For Each d In alldevlist
            If d.Kind = "TSS" Then
                AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
            End If
            If d.Kind = "TZBQ" Then
                AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
            End If
            If d.Name = selectDeviceID Then
                ds = d
                th_setGisCenter(d.Lng, d.Lat, WebGis)

            End If
        Next

    End Sub
    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        CloseFunctionsUserPanel()
        RefrushGis()
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

    Private Sub WebGis_ControlRemoved(ByVal sender As Object, ByVal e As System.Windows.Forms.ControlEventArgs) Handles WebGis.ControlRemoved
        'MsgBox(WebGis.Controls.Count)
        'For Each p As Control In WebGis.Controls
        '    MsgBox(p.GetType.ToString)
        '    If p Is GetType(OnlineFreq) Then
        '        MsgBox("o ?")
        '        Dim op As OnlineFreq = CType(p, OnlineFreq)
        '        op.stopALL()
        '    End If
        'Next
    End Sub

   
  
    Private Sub WebGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        RefrushGis()
    End Sub

  
    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        If WebGis.Controls.Count = 0 Then
            Dim kind As String = GetDevKindByID(selectDeviceID)
            If kind = "TSS" Then
                Dim p As New OnlineFreq(selectDeviceID, True, -1)
                WebGis.Controls.Add(p)
            End If
            If kind = "TZBQ" Then
                Dim p As New OnlineSignal(selectDeviceID, True, 3)
                WebGis.Controls.Add(p)
            End If
            If kind = "ING" Then
                Dim p As New OnlineING(selectDeviceID, True, -1)
                WebGis.Controls.Add(p)
            End If
        Else
            CloseFunctionsUserPanel()
        End If
    End Sub
    Public Sub CloseFunctionsUserPanel()
        For Each p As Control In WebGis.Controls
            If p.GetType.ToString = GetType(OnlineFreq).ToString Then
                Dim op As OnlineFreq = CType(p, OnlineFreq)
                op.stopALL()
            End If
            If p.GetType.ToString = GetType(OnlineSignal).ToString Then
                Dim op As OnlineSignal = CType(p, OnlineSignal)
                op.stopALL()
            End If
        Next
        WebGis.Controls.Clear()
    End Sub

    Private Sub Panel20_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel20.Paint

    End Sub

    Private Sub Panel3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel3.Paint

    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click

    End Sub

   
End Class
