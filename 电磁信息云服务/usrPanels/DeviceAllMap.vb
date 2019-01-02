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
Public Class DeviceAllMap

    Private Sub DeviceAllMap_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        Label2.Visible = False
        If isLoadGis Then
            WebGis.Navigate(gisurl)
        Else
            RefrushGis()
        End If
        ini()

    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 45)
        LVDetail.Columns.Add("设备名称", 80)
        LVDetail.Columns.Add("设备类型", 80)
        LVDetail.Columns.Add("地点", 80)
        LVDetail.Columns.Add("上线时间", 80)
        LVDetail.Columns.Add("经度", 80)
        LVDetail.Columns.Add("纬度", 80)
        LVDetail.Columns.Add("IP", 80)
        LVDetail.Columns.Add("Port", 80)
      
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
        Try
            Dim doc As HtmlDocument = web.Document
            Dim O(str.Count - 1) As Object
            For i = 0 To str.Length - 1
                O(i) = CObj(str(i))
            Next
            doc.InvokeScript(scriptName, O)
        Catch ex As Exception

        End Try
        
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

    Delegate Sub wt_setGisCenter3(ByVal lng As String, ByVal lat As String, ByVal size As Integer, ByVal web As WebBrowser)
    Private Sub setGisCenter3(ByVal lng As String, ByVal lat As String, ByVal size As Integer, ByVal web As WebBrowser)
        Dim d As New wt_setGisCenter3(AddressOf th_setGisCenter3)
        Dim b(3) As Object
        b(0) = lng
        b(1) = lat
        b(2) = size
        b(3) = web
        Me.Invoke(d, b)
    End Sub
    Private Sub th_setGisCenter3(ByVal lng As String, ByVal lat As String, ByVal size As Integer, ByVal web As WebBrowser)
        '70410045
        '421127199303212592
        '梅子怀
        Dim doc As HtmlDocument = web.Document
        If IsNothing(doc) Then Exit Sub
        Dim ObjArr(2) As Object
        ObjArr(0) = CObj(lng)
        ObjArr(1) = CObj(lat)
        ObjArr(2) = CObj(size)
        doc.InvokeScript("setcenter3", ObjArr)
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

    Private Sub WebGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        'setGisCenter3(104.345, 30.678, 5, WebGis)
        '113.75
        '113.739898,22.804636
        '113.75, 23.065
        setGisCenter3(113.739898, 22.804636, 13, WebGis)
        RefrushGis()
    End Sub
    Private Sub RefrushGis()
      th_refrushGis
    End Sub
    Private Sub th_refrushGis()
        Label2.Visible = True
        LVDetail.Items.Clear()
        Application.DoEvents()
        Me.Invoke(Sub() Form1.GetOnlineDevice())
        Label2.Visible = False
        Me.Invoke(Sub() If isLoadGis Then CleanGis(WebGis))
        Dim TZBQTotalNum As Integer = 0
        Dim TSSTotalNum As Integer = 0
        Dim INGTotalNum As Integer = 0
        Dim INGOnlineNum As Integer = 0
        Dim TSSGateWayOnlineNum As Integer = 0
        Dim TSSGateWayNum As Integer = 0
        Dim TZBQOnlineNum As Integer = 0
        Dim TSSOnlineNum As Integer = 0
        If IsNothing(alldevlist) Then Return
        For Each d In alldevlist
            If d.Kind = "TZBQ" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                    TSSOnlineNum = TSSOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TSSGateWay" Then
                    itm.SubItems(2).Text = "TSS网关"
                    TSSGateWayOnlineNum = TSSGateWayOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                    TZBQOnlineNum = TZBQOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "ING" Then
                    itm.SubItems(2).Text = "监测网关"
                    INGOnlineNum = INGOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                LVDetail.Items.Add(itm)
            End If          
        Next
        For Each d In alldevlist
            If d.Kind = "TSS" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                    TSSOnlineNum = TSSOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TSSGateWay" Then
                    itm.SubItems(2).Text = "TSS网关"
                    TSSGateWayOnlineNum = TSSGateWayOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                    TZBQOnlineNum = TZBQOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "ING" Then
                    itm.SubItems(2).Text = "监测网关"
                    INGOnlineNum = INGOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                LVDetail.Items.Add(itm)
            End If
        Next
        For Each d In alldevlist
            If d.Kind = "TSSGateWay" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                    TSSOnlineNum = TSSOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TSSGateWay" Then
                    itm.SubItems(2).Text = "TSS网关"
                    TSSGateWayOnlineNum = TSSGateWayOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                    TZBQOnlineNum = TZBQOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "ING" Then
                    itm.SubItems(2).Text = "监测网关"
                    INGOnlineNum = INGOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                LVDetail.Items.Add(itm)
            End If
        Next
        For Each d In alldevlist
            If d.Kind = "ING" Then
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                    TSSOnlineNum = TSSOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TSSGateWay" Then
                    itm.SubItems(2).Text = "TSS网关"
                    TSSGateWayOnlineNum = TSSGateWayOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                    TZBQOnlineNum = TZBQOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                If d.Kind = "ING" Then
                    itm.SubItems(2).Text = "监测网关"
                    INGOnlineNum = INGOnlineNum + 1
                    If isLoadGis Then AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                    'AddPoint(d.Lng, d.Lat, d.Name, WebGis)
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                LVDetail.Items.Add(itm)
            End If
        Next
        For Each itm As ListViewItem In LVDetail.Items
            itm.UseItemStyleForSubItems = True
            Dim kind As String = itm.SubItems(2).Text
            Dim color As Color
            If kind = "微型传感器" Then
                color = color.Blue
            End If
            If kind = "频谱传感器" Then
                color = color.Green
            End If
            If kind = "TSS网关" Then
                color = color.FromArgb(218, 165, 32)
            End If
            If kind = "监测网关" Then
                color = color.FromArgb(139, 101, 8)
            End If
            For i = 0 To itm.SubItems.Count - 1
                itm.SubItems(i).ForeColor = color
            Next
        Next
        Dim resutl As String = GetH(ServerUrl, "func=GetAllDBDevlist&token=" & token)
        If GetResultPara("result", resutl) = "fail" Then
            If GetResultPara("msg", resutl) = "Please login" Then
                Login()
                resutl = GetH(ServerUrl, "func=GetAllDBDevlist&token=" & token)
            End If
        End If
        If resutl = "" Then
            MsgBox("网络错误，请检查您的网络")
            Exit Sub
        End If
        Dim dt As DataTable = JsonConvert.DeserializeObject(resutl, GetType(DataTable))
        If IsNothing(dt) = False Then
            For Each row As DataRow In dt.Rows
                Dim kind As String = row("Kind")
                If kind = "TZBQ" Then
                    TZBQTotalNum = TZBQTotalNum + 1
                End If
                If kind = "TSS" Then
                    TSSTotalNum = TSSTotalNum + 1
                End If
                If kind = "ING" Then
                    INGTotalNum = INGTotalNum + 1
                End If
                If kind = "TSSGateWay" Then
                    TSSGateWayNum = TSSGateWayNum + 1
                End If
            Next
        End If
        lblTZBQTotalNum.Text = TZBQTotalNum
        lblTZBQOnlineNum.Text = TZBQOnlineNum
        Dim TZBQdper As Single = Format(100 * TZBQOnlineNum / TZBQTotalNum, "0.0")
        lblTZBQOnlinePer.Text = TZBQdper & " %"
        lblTZBQDisOnlineNum.Text = TZBQTotalNum - TZBQOnlineNum
        lblTZBQDisOnlinePer.Text = (100 - TZBQdper) & " %"

        lblTSSTotalNum.Text = TSSTotalNum
        lblTSSOnlineNum.Text = TSSOnlineNum
        Dim TSSdper As Single = Format(100 * TSSOnlineNum / TSSTotalNum, "0.0")
        lblTSSOnlinePer.Text = TSSdper & " %"
        lblTSSDisOnlineNum.Text = TSSTotalNum - TSSOnlineNum
        lblTssDisOnlinePer.Text = (100 - TSSdper) & " %"
        Label2.Visible = False
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        RefrushGis()
    End Sub

    Private Sub 选中该设备并进入设备功能地图ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 选中该设备并进入设备功能地图ToolStripMenuItem.Click
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim deviceName As String = itm.SubItems(1).Text
        Dim kind As String = itm.SubItems(2).Text
        If kind = "频谱传感器" Then
            Form1.p_beClick("频谱传感器," & deviceName)
        End If
        If kind = "微型传感器" Then
            Form1.p_beClick("微型传感器," & deviceName)
        End If

    End Sub

    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim lng As String = itm.SubItems(5).Text
        Dim lat As String = itm.SubItems(6).Text
        Dim selectDeviceID As String = itm.SubItems(1).Text
        Dim selectDeviceKind As String = itm.SubItems(2).Text

        If lng <> "" And lat <> "" Then
            setGisCenter3(lng, lat, 15, WebGis)
            'Form1.selectDeviceID = selectDeviceID
            'Form1.Label2.Text = "选中  " & "微型传感器" & "  " & selectDeviceID
            'If selectDeviceKind = "微型传感器" Then
            '    Form1.p_beClick("微型传感器," & selectDeviceID)
            'End If
            'If selectDeviceKind = "频谱传感器" Then
            '    Form1.p_beClick("频谱传感器," & selectDeviceID)
            'End If
        End If

    End Sub
End Class
