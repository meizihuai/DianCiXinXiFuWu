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
Public Class OnlineTask


    Private Sub OnlineTask_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        ini()
        If isLoadGis Then
            WebGis.Navigate(gisurl)
        Else
            refrushGis()
            GetTaskList()

        End If
    End Sub
    Private Sub ini()
        LVTask.View = View.Details
        LVTask.GridLines = True
        LVTask.FullRowSelect = True
        LVTask.Columns.Add("序号")
        LVTask.Columns.Add("任务类别")
        LVTask.Columns.Add("任务名称", 200)
        LVTask.Columns.Add("设备ID", 150)
        LVTask.Columns.Add("设备名称", 150)
        LVTask.Columns.Add("开始时间", 150)
        LVTask.Columns.Add("结束时间", 150)
        LVTask.Columns.Add("完成状态", 150)
        LVTask.Columns.Add("下载报告", 500)
    End Sub
    Public Sub GetTaskList()
        Label2.Visible = True
        Dim result As String = GetH(ServerUrl, "func=GetMyTask&token=" & token)
        '   Console.WriteLine(result)
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetTaskList()
            Exit Sub
        End If
        LVTask.Items.Clear()
        Label2.Visible = False
        If result = "[]" Then
            Exit Sub
        End If

        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) = False Then
            Dim dv As DataView = dt.DefaultView
            dv.Sort = "StartTime Asc"
            Dim dt2 As DataTable = dv.ToTable
            For Each row As DataRow In dt2.Rows
                If row("OverPercent") = "100%" Then Continue For
                Dim itm As New ListViewItem(LVTask.Items.Count + 1)
                itm.SubItems.Add(row("TaskName"))
                itm.SubItems.Add(row("TaskNickName"))
                itm.SubItems.Add(row("DeviceID"))
                itm.SubItems.Add(row("DeviceName").ToString)
                itm.SubItems.Add(row("StartTime"))
                itm.SubItems.Add(row("EndTime"))
                itm.SubItems.Add(row("OverPercent"))
                itm.SubItems.Add(row("ResultReportUrl"))
                LVTask.Items.Add(itm)
            Next
            If LVTask.Items.Count > 0 Then
                LVTaskBeSelect(0)
            End If
        End If
        Label2.Visible = False
    End Sub

    Private Sub WebGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        setGisCenter3(104.345, 30.678, 5, WebGis)
        refrushGis()
        GetTaskList()

    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetTaskList()
    End Sub
    Private Sub refrushGis()

        Application.DoEvents()
        Me.Invoke(Sub() Form1.GetOnlineDevice())
        Me.Invoke(Sub() If isLoadGis Then CleanGis(WebGis))
        If IsNothing(alldevlist) Then Return
        Dim TZBQTotalNum As Integer = 0
        Dim TSSTotalNum As Integer = 0
        Dim TZBQOnlineNum As Integer = 0
        Dim TSSOnlineNum As Integer = 0
        If isLoadGis Then
            For Each d In alldevlist
                If d.Kind = "TSS" Then
                    AddNewIco(d.Lng, d.Lat, d.Name, TssIco, WebGis)
                End If
                If d.Kind = "TZBQ" Then
                    AddNewIco(d.Lng, d.Lat, d.Name, TZBQIco, WebGis)
                End If
            Next
        End If
        'Dim resutl As String = GetH(ServerUrl, "func=GetAllDBDevlist&token=" & token)
        'If GetResultPara("result", resutl) = "fail" Then
        '    If GetResultPara("msg", resutl) = "Please login" Then
        '        Login()
        '        resutl = GetH(ServerUrl, "func=GetAllDBDevlist&token=" & token)
        '    End If
        'End If
        'If resutl = "" Then
        '    MsgBox("网络错误，请检查您的网络")
        '    Exit Sub
        'End If
        'Dim dt As DataTable = JsonConvert.DeserializeObject(resutl, GetType(DataTable))
        'If IsNothing(dt) = False Then
        '    For Each row As DataRow In dt.Rows
        '        Dim kind As String = row("Kind")
        '        If kind = "TZBQ" Then
        '            TZBQTotalNum = TZBQTotalNum + 1
        '        End If
        '        If kind = "TSS" Then
        '            TSSTotalNum = TSSTotalNum + 1
        '        End If
        '    Next
        'End If
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

    Private Sub LVTask_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVTask.SelectedIndexChanged
        If LVTask.SelectedIndices.Count = 0 Then Return
        LVTaskBeSelect(LVTask.SelectedIndices(0))
    End Sub
    Private Sub LVTaskBeSelect(ByVal index As Integer)
        Dim itm As ListViewItem = LVTask.Items(index)
        Dim taskName As String = itm.SubItems(1).Text
        Dim taskNickName As String = itm.SubItems(2).Text
        Dim DeviceID As String = itm.SubItems(3).Text
        Dim DeviceName As String = itm.SubItems(4).Text
        Dim StartTime As String = itm.SubItems(5).Text
        Dim EndTime As String = itm.SubItems(6).Text
        Dim StartDate As Date = Date.Parse(StartTime)
        Dim EndDate As Date = Date.Parse(EndTime)
        Dim nowDate As Date = Now
        Dim t1 As TimeSpan = EndDate - StartDate
        Dim t2 As TimeSpan = Now - StartDate
        Dim d As Double = t2.TotalSeconds / t1.TotalSeconds
        If d > 1 Then d = 1
        If StartTime >= Now Then
            d = 0
        End If
        Dim percent As String = (d * 100).ToString(0.0) & " %"
        Dim lng As String = ""
        Dim lat As String = ""
        For Each dev In alldevlist
            If dev.DeviceID = DeviceID Then
                lng = dev.Lng
                lat = dev.Lat
                Exit For
            End If
        Next
        Dim sb As New StringBuilder
        sb.Append("任务类型:" & taskName & "<br>")
        sb.Append("任务备注:" & taskNickName & "<br>")
        sb.Append("执行设备:" & DeviceName & "<br>")
        sb.Append("起始时间:" & StartTime & "<br>")
        sb.Append("结束时间:" & EndTime & "<br>")
        sb.Append("任务进度:" & percent)
        script("showWindowMsg", New String() {lng, lat, sb.ToString, True}, WebGis)
        script("setcenter", New String() {lng, lat}, WebGis)
    End Sub
End Class
