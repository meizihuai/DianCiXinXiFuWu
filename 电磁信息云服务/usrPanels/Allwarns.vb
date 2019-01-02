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
Imports SpeechLib

Public Class Allwarns
    Dim WarnListDT As DataTable
    Dim thReviceWarn As Thread
    Private Sub Allwarns_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        ini()
        Label2.Visible = False
        If isLoadGis Then
            WebGis.Navigate(gisurl)
        Else
            GetWarnList()
        End If
        thReviceWarn = New Thread(AddressOf ReviceWarn)
        thReviceWarn.Start()
    End Sub
    Public Sub stopALL()
        If IsNothing(thReviceWarn) = False Then
            Try
                thReviceWarn.Abort()
            Catch ex As Exception

            End Try
        End If
    End Sub
    Private Sub ReviceWarn()
        While True
            Try
                Dim msg As String = GetHttpWarnMsg()
                If msg <> "" Then
                    If msg = "null" Then
                        Continue While
                    End If
                    Console.WriteLine(msg)
                    Me.Invoke(Sub() PictureBox1.Image = My.Resources.告警待处理)
                    Dim list As List(Of String) = JsonConvert.DeserializeObject(msg, GetType(List(Of String)))
                    If IsNothing(list) Then
                        Continue While
                    End If
                    For Each warn In list
                        If InStr(warn, "WARN") Then
                            Dim oldDeviceMsg As String = warn
                            warn = CutBQ(warn)
                            Dim st() As String = warn.Split(",")
                            Dim str As String
                            Dim lx As String = ""
                            If st(2) = "GR" Then lx = "干扰"
                            If st(2) = "ZC" Then lx = "正常"
                            If st(2) = "KX" Then lx = "空闲"
                            If st(2) = "WZ" Then lx = "违章"
                            If st(2) = "GZ" Then lx = "故障"
                            If st(2) = "HGB" Then lx = "黑广播"
                            Dim lng As String = st(3)
                            Dim lat As String = st(4)
                            Dim address As String = "东莞实验网"
                            Dim id As String = st(1)
                            Dim deviceName As String = ""
                            SyncLock alldevlist
                                For Each dev In alldevlist
                                    If dev.DeviceID = id Then
                                        lng = dev.Lng
                                        lat = dev.Lat
                                        address = dev.Address
                                        deviceName = dev.Name
                                        Exit For
                                    End If
                                Next
                            End SyncLock

                            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                            itm.SubItems.Add(lx)
                            'itm.SubItems.Add(row("Address"))
                            itm.SubItems.Add(address)
                            itm.SubItems.Add(deviceName)
                            itm.SubItems.Add(Now.ToString("yyyy-MM-dd HH:mm:ss"))
                            itm.SubItems.Add(WARN2Msg(warn, address))
                            Dim DeviceMsg As String = warn
                            itm.SubItems.Add(lng)
                            itm.SubItems.Add(lat)
                            itm.SubItems.Add(oldDeviceMsg)
                            LVDetail.Items.Add(itm)


                            If st(2) = "GR" Then
                                str = "即时告警<br>传感器:" & deviceName & "<br>在" & address & "<br>上报" & lx & "预警" & "<br>频率: " & st(7) & "MHz<br>场强: " & st(8) & "dBm" & "<br>超标: " & st(9) & " %" & "<br>干扰: " & st(10) & " 次" & "<br>预警时间:" & Now.ToString("yyyy-MM-dd HH:mm:ss")
                            Else
                                str = "即时告警<br>传感器:" & deviceName & "<br>在" & address & "<br>上报" & lx & "预警" & "<br>频率: " & st(7) & "MHz<br>场强: " & st(8) & "dBm" & "<br>超标: " & st(9) & " %" & "<br>预警时间:" & Now.ToString("yyyy-MM-dd HH:mm:ss")
                            End If
                            script("cleanall", New String() {}, WebGis)
                            script("addpoint", New String() {lng, lat, "", True}, WebGis)
                            script("showWindowMsg", New String() {lng, lat, str, True}, WebGis)
                            script("setcenter", New String() {lng, lat}, WebGis)
                            If CheckBox1.Checked Then
                                Dim warnSpeekMsg As String
                                warnSpeekMsg = "传感器" & deviceName & ",在" & address.Replace(",", "") & "上报" & lx & "预警," & "频率" & st(7) & "MHz"
                                speek(warnSpeekMsg)
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception

            End Try
            ' Me.Invoke(Sub() PictureBox1.Image = My.Resources.一般告警)
        End While
    End Sub
    Public Sub speek(ByVal str As String)
        Dim th As New Thread(AddressOf th_speek)
        th.Start(str)
    End Sub
    Private Sub th_speek(ByVal str As String)
        Try
            str = str.Replace("MHz", "兆赫兹")
            str = str.Replace("dBm", "")
            Dim voice As New SpVoice
            voice.Rate = 2
            voice.Volume = 100
            voice.Voice = voice.GetVoices().Item(0)
            voice.Speak(str)
        Catch ex As Exception
            '‘MsgBox(ex.ToString)
        End Try

    End Sub
    Private Function GetHttpWarnMsg() As String
        Try
            Dim req As HttpWebRequest = WebRequest.Create(ServerUrl & "func=GetNewWarn&token=" & token)
            ' Me.Invoke(Sub() MsgBox(HttpMsgUrl & "?func=GetDevMsg"))
            req.Accept = "*/*"
            req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
            req.KeepAlive = True
            req.Timeout = 5000
            req.ReadWriteTimeout = 5000
            req.ContentType = "application/x-www-form-urlencoded"
            req.Method = "GET"
            Dim rp As HttpWebResponse = req.GetResponse
            Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd
            Return str
        Catch ex As Exception
            '  MsgBox(ex.ToString)
        End Try
    End Function
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 50)
        LVDetail.Columns.Add("预警类别")
        LVDetail.Columns.Add("预警地点")
        LVDetail.Columns.Add("设备ID")
        LVDetail.Columns.Add("预警时间", 150)
        LVDetail.Columns.Add("预警字段", 150)
        LVDetail.Columns.Add("经度")
        LVDetail.Columns.Add("纬度")
        LVDetail.Columns.Add("消息字段")

    End Sub
    Private Sub GetWarnList()
        Return
        LVDetail.Items.Clear()
        Label2.Visible = True
        My.Application.DoEvents()
        Dim result As String = GetH(ServerUrl, "func=GetWarn&startTime=2018-01-01 00:00:00&endTime=2019-01-01 00:00:00&startIndex=0&count=1000&token=" & token)
        If GetResultPara("msg", result) = "Please login" Then
            Login()
            GetWarnList()
            Exit Sub
        End If
        Label2.Visible = False
        My.Application.DoEvents()
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
            For i = 0 To dt2.Rows.Count - 1
                Dim row As DataRow = dt2.Rows(i)
                Dim r As DataRow = WarnListDT.NewRow
                For j = 0 To dt2.Columns.Count - 1
                    r(j) = row(j)
                Next
                WarnListDT.Rows.Add(r)
                Dim index As Integer = WarnListDT.Rows.Count - 1
                Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
                itm.SubItems.Add(GetWarnKindName(row("WarnKind")))
                'itm.SubItems.Add(row("Address"))
                itm.SubItems.Add("九江市开发区")
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
                LVDetail.Items.Add(itm)
            Next
            If LVDetail.Items.Count > 0 Then
                lvdetailBeSelect(0)
            End If
        End If
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
    Private Function GetWarnKindName(ByVal kind As String) As String
        If kind = "GR" Then Return "干扰"
        If kind = "ZC" Then Return "违章"
        If kind = "WZ" Then Return "违章"
        If kind = "GZ" Then Return "故障"
    End Function
    Private Function WARN2Msg(ByVal strtmp As String, ByVal address As String) As String
        If InStr(strtmp, "WARN") Then
            strtmp = strtmp.Replace("<", "")
            strtmp = strtmp.Replace(">", "")
            strtmp = strtmp.Replace(Chr(13), "")
            strtmp = strtmp.Replace(Chr(10), "")
            Dim st() As String = strtmp.Split(",")
            Dim str As String
            Dim lx As String = ""
            If st(2) = "GR" Then lx = "干扰"
            If st(2) = "ZC" Then lx = "正常"
            If st(2) = "WZ" Then lx = "违章"
            If st(2) = "GZ" Then lx = "故障"
            If st(2) = "KX" Then lx = "空闲"
            If st(2) = "HGB" Then lx = "黑广播"
            Dim didian As String = "东莞火车站"
            address = "九江市开发区"
            If st(2) = "GR" Then
                str = address & "上报" & lx & "预警" & ",频率" & st(7) & "MHz场强" & st(8) & "dBm" & ",超标" & st(9) & "%" & ",干扰" & st(10) & "次"
            Else
                str = address & "上报" & lx & "预警" & ",频率" & st(7) & "MHz场强" & st(8) & "dBm" & ",超标" & st(9) & "%"
            End If
            Return str
        End If
    End Function

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetWarnList()
    End Sub

    Private Sub lvdetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedItems.Count <= 0 Then Return
        Dim index As Integer = LVDetail.SelectedIndices(0)
        lvdetailBeSelect(index)
    End Sub
    Private Function CutBQ(ByVal bq As String) As String
        If InStr(bq, "<") Then
            If InStr(bq, ">") Then
                bq = bq.Substring(InStr(bq, "<") - 1, InStr(bq, ">") - InStr(bq, "<"))
                Return bq
            End If
        End If
        Return ""
    End Function
    Private Sub lvdetailBeSelect(ByVal index As Integer)
        Dim itm As ListViewItem = LVDetail.Items(index)
        'LVDetail.Items(0).SubItems(1).Text = itm.SubItems(1).Text
        'LVDetail.Items(1).SubItems(1).Text = itm.SubItems(2).Text
        'LVDetail.Items(2).SubItems(1).Text = itm.SubItems(3).Text
        'LVDetail.Items(3).SubItems(1).Text = itm.SubItems(4).Text
        Dim strTmp As String = itm.SubItems(8).Text
        Dim lng As String = itm.SubItems(6).Text
        Dim lat As String = itm.SubItems(7).Text
        If InStr(strTmp, "WARN") Then
            'strTmp = strTmp.Replace("<", "")
            'strTmp = strTmp.Replace(">", "")
            'strTmp = strTmp.Replace("/0", "")
            'strTmp = strTmp.Replace(Chr(13), "")
            'strTmp = strTmp.Replace(Chr(10), "")
            strTmp = CutBQ(strTmp)
            Dim st() As String = strTmp.Split(",")
            Dim str As String
            Dim lx As String = ""
            If st(2) = "GR" Then lx = "干扰"
            If st(2) = "ZC" Then lx = "正常"
            If st(2) = "WZ" Then lx = "违章"
            If st(2) = "GZ" Then lx = "故障"
            If st(2) = "KX" Then lx = "空闲"
            If st(2) = "HGB" Then lx = "黑广播"
            'LVDetail.Items(4).SubItems(1).Text = st(7) & "MHz"
            'LVDetail.Items(5).SubItems(1).Text = st(8)
            'LVDetail.Items(6).SubItems(1).Text = st(9) & " %"
            'LVDetail.Items(7).SubItems(1).Text = "60秒"
            'LVDetail.Items(8).SubItems(1).Text = "2次"
            Dim address As String = itm.SubItems(2).Text
            address = "九江市开发区"

            Dim id As String = st(1)
            Dim deviceName As String = id

            For Each dev In alldevlist
                If dev.DeviceID = id Then
                    lng = dev.Lng
                    lat = dev.Lat
                    address = dev.Address
                    deviceName = dev.Name
                    Exit For
                End If
            Next


            If st(2) = "GR" Then
                str = "传感器:" & deviceName & "<br>在" & address & "<br>上报" & lx & "预警" & "<br>频率: " & st(7) & "MHz<br>场强: " & st(8) & "dBm" & "<br>超标: " & st(9) & " %" & "<br>干扰: " & st(10) & " 次"
            Else
                str = "传感器:" & deviceName & "<br>在" & address & "<br>上报" & lx & "预警" & "<br>频率: " & st(7) & "MHz<br>场强: " & st(8) & "dBm" & "<br>超标: " & st(9) & " %"
            End If
            script("cleanall", New String() {}, WebGis)
            script("addpoint", New String() {lng, lat, "", True}, WebGis)
            script("showWindowMsg", New String() {lng, lat, str, True}, WebGis)
            script("setcenter", New String() {lng, lat}, WebGis)
        End If



        'LVDetail.Items(8).SubItems(1).Text = itm.SubItems(2).Text
    End Sub


    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        PictureBox1.Image = My.Resources.一般告警
    End Sub
End Class
