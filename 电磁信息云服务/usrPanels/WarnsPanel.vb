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
Public Class WarnsPanel
    Private WarnListDT As DataTable
    Private Sub WarnsPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        ini()

        If isLoadGis Then
            WebGis.Navigate(gisurl)
        Else
            GetWarnList()
        End If

    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("项目", 60)
        LVDetail.Columns.Add("内容", 150)
        Dim itm As New ListViewItem("预警类别")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("预警地点")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("设备ID")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("上报时间")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("预警频率")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("预警场强")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("超标百分比")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("持续时间")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem("干扰次数")
        itm.SubItems.Add("")
        LVDetail.Items.Add(itm)

        LVWarn.View = View.Details
        LVWarn.GridLines = True
        LVWarn.FullRowSelect = True
        LVWarn.Columns.Add("序号", 50)
        LVWarn.Columns.Add("预警类别")
        LVWarn.Columns.Add("预警地点")
        LVWarn.Columns.Add("设备ID")      
        LVWarn.Columns.Add("预警时间", 150)
        LVWarn.Columns.Add("预警字段", 150)
        LVWarn.Columns.Add("经度")
        LVWarn.Columns.Add("纬度")
        LVWarn.Columns.Add("消息字段")
    End Sub
    Private Sub GetWarnList()
        Label1.Text = "获取中……"
        Dim startTime As String = Now.AddDays(-90).ToString("yyyy-MM-dd HH:mm:ss")
        Dim endTime As String = Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss")
        Dim result As String = GetH(ServerUrl, "func=GetWarn&startTime=" & startTime & "&endTime=" & endTime & "&startIndex=0&count=100&token=" & token)
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetWarnList()
            Exit Sub
        End If
        Label1.Text = "预警列表"
        If result = "[]" Then
            Exit Sub
        End If
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) = False Then
            Dim dv As DataView = dt.DefaultView
            dv.Sort = "MsgTime Asc"
            Dim dt2 As DataTable = dv.ToTable
            WarnListDT = New DataTable
            For Each col As DataColumn In dt2.Columns
                WarnListDT.Columns.Add(col.ColumnName)
            Next
            LVWarn.Items.Clear()
            LVWarn.Visible = False
            For i = 0 To dt2.Rows.Count - 1
                Dim row As DataRow = dt2.Rows(i)
                Dim r As DataRow = WarnListDT.NewRow
                For j = 0 To dt2.Columns.Count - 1
                    r(j) = row(j)
                Next
                WarnListDT.Rows.Add(r)
                Dim index As Integer = WarnListDT.Rows.Count - 1
                Dim itm As New ListViewItem(LVWarn.Items.Count + 1)
                itm.SubItems.Add(GetWarnKindName(row("WarnKind")))
                itm.SubItems.Add(row("Address"))
                'itm.SubItems.Add("九江市开发区")
                itm.SubItems.Add(row("DeviceID"))
                itm.SubItems.Add(row("MsgTime"))
                itm.SubItems.Add(WARN2Msg(row("DeviceMsg"), row("Address")))
                Dim DeviceMsg As String = row("DeviceMsg")
                Dim lng As String = ""
                Dim lat As String = ""
                If InStr(DeviceMsg, "WARN") Then
                    Dim strtmp As String = DeviceMsg
                    strtmp = strtmp.Replace("<", "")
                    strtmp = strtmp.Replace(">", "")
                    strtmp = strtmp.Replace(Chr(13), "")
                    strtmp = strtmp.Replace(Chr(10), "")
                    Dim st() As String = strtmp.Split(",")
                    lng = st(3)
                    lat = st(4)                
                End If
                itm.SubItems.Add(lng)
                itm.SubItems.Add(lat)
                itm.SubItems.Add(DeviceMsg)
                LVWarn.Items.Add(itm)
            Next
            LVWarn.Visible = True
            If LVWarn.Items.Count > 0 Then
                LVWarnBeSelect(0)
            End If
        End If
    End Sub
    Private Function GetWarnKindName(ByVal kind As String) As String
        If kind = "GR" Then Return "干扰"
        If kind = "ZC" Then Return "违章"
        If kind = "WZ" Then Return "违章"
        If kind = "GZ" Then Return "故障"
        If kind = "HGB" Then Return "黑广播"
        If kind = "KX" Then Return "空闲"
    End Function
    Private Function WARN2Msg(ByVal strtmp As String, ByVal address As String) As String
        If InStr(strtmp, "WARN") Then
             strTmp = strTmp.Substring(InStr(strTmp, "<"), InStr(strTmp, ">") - InStr(strTmp, "<") - 1)
            Dim st() As String = strtmp.Split(",")
            Dim str As String
            Dim lx As String = ""
            If st(2) = "GR" Then lx = "干扰"
            If st(2) = "ZC" Then lx = "正常"
            If st(2) = "WZ" Then lx = "违章"
            If st(2) = "GZ" Then lx = "故障"
            If st(2) = "KX" Then lx = "空闲"
            If st(2) = "HGB" Then lx = "黑广播"
            Dim didian As String = address
            ' address = "九江市开发区"
            If st(2) = "GR" Then
                str = address & "上报" & lx & "预警" & ",频率" & st(7) & "MHz场强" & st(8) & "dBm" & ",超标" & st(9) & "%" & ",干扰" & st(10) & "次"
            Else
                str = address & "上报" & lx & "预警" & ",频率" & st(7) & "MHz场强" & st(8) & "dBm" & ",超标" & st(9) & "%"
            End If
            Return str
        End If
    End Function
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        GetWarnList()
    End Sub

   
    Private Sub LVWarnBeSelect(ByVal index As Integer)
        Dim itm As ListViewItem = LVWarn.Items(index)
        LVDetail.Items(0).SubItems(1).Text = itm.SubItems(1).Text
        LVDetail.Items(1).SubItems(1).Text = itm.SubItems(2).Text
        LVDetail.Items(2).SubItems(1).Text = itm.SubItems(3).Text
        LVDetail.Items(3).SubItems(1).Text = itm.SubItems(4).Text
        Dim strTmp As String = itm.SubItems(8).Text
        Dim lng As String = itm.SubItems(6).Text
        Dim lat As String = itm.SubItems(7).Text
        If InStr(strTmp, "WARN") Then
            'strTmp = strTmp.Replace("<", "")
            'strTmp = strTmp.Replace(">", "")
            'strTmp = strTmp.Replace("/0", "")
            'strTmp = strTmp.Replace(Chr(13), "")
            'strTmp = strTmp.Replace(Chr(10), "")
            strTmp = strTmp.Substring(InStr(strTmp, "<"), InStr(strTmp, ">") - InStr(strTmp, "<") - 1)
            'MsgBox(strTmp)
            Dim st() As String = strTmp.Split(",")
            Dim str As String
            Dim lx As String = ""
            If st(2) = "GR" Then lx = "干扰"
            If st(2) = "ZC" Then lx = "正常"
            If st(2) = "WZ" Then lx = "违章"
            If st(2) = "GZ" Then lx = "故障"
            If st(2) = "KX" Then lx = "空闲"
            If st(2) = "HGB" Then lx = "黑广播"
            LVDetail.Items(4).SubItems(1).Text = st(7) & "MHz"
            LVDetail.Items(5).SubItems(1).Text = st(8)
            LVDetail.Items(6).SubItems(1).Text = st(9) & " %"
            LVDetail.Items(7).SubItems(1).Text = "60秒"
            LVDetail.Items(8).SubItems(1).Text = "2次"
            Dim address As String = itm.SubItems(2).Text
            ' address = "九江市开发区"
            If st(2) = "GR" Then
                str = address & "<br>上报" & lx & "预警" & "<br>频率" & st(7) & "MHz<br>场强" & st(8) & "dBm" & "<br>超标" & st(9) & "%" & "<br>干扰" & st(10) & "次"
            Else
                str = address & "<br>上报" & lx & "预警" & "<br>频率" & st(7) & "MHz<br>场强" & st(8) & "dBm" & "<br>超标" & st(9) & "%"
            End If
            script("cleanall", New String() {}, WebGis)
            script("addpoint", New String() {lng, lat, "", True}, WebGis)
            script("showWindowMsg", New String() {lng, lat, str, True}, WebGis)
            script("setcenter", New String() {lng, lat}, WebGis)
        End If


       
        'LVDetail.Items(8).SubItems(1).Text = itm.SubItems(2).Text
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

    Private Sub WebGis_DocumentCompleted(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebGis.DocumentCompleted
        GetWarnList()
    End Sub

    Private Sub LVWarn_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVWarn.SelectedIndexChanged
        If LVWarn.SelectedItems.Count <= 0 Then Return
        Dim index As Integer = LVWarn.SelectedIndices(0)
        LVWarnBeSelect(index)
    End Sub

    Private Sub PictureBox3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetWarnList()
    End Sub
End Class
