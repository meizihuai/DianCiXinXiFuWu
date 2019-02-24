Imports System
Imports System.IO
Imports System.Text
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Threading
Imports System.Threading.Thread
Imports System.IO.Compression
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Module Module1
    Public exeName As String = "电磁信息云服务.exe"
    Public Version As String = "1.4.3.8"
    Public hasaki As String = "mac"
    Public hero As String = "zed"
    Public uiOpenImg As Image = Image.FromFile("./img/展开.png")
    Public uiCloseImg As Image = Image.FromFile("./img/收起.png")
    Public alldevlist As List(Of deviceStu)
    Public isHuanChong, isShouTing, isAutoFenli, isFenli, isMuban, isGenzong, isSave As Boolean
    Public AutoFenXiDu As Integer = 5
    Public AutoFenXiFuCha As Double = 5
    Public VoiceMuban As Integer = 0
    Public voiceRate As Integer = 2

    Public VoiceKind As Integer = 0
    Public selectGisPanel As GisPanel
    Public TZBQIco As String = "http://123.207.31.37:8082/bmapico/TZBQ.png" & "?t=" & Now.Ticks
    Public TssIco As String = "http://123.207.31.37:8082/bmapico/Tss.png" & "?t=" & Now.Ticks
    Public BeijingIco As String = "http://123.207.31.37:8082/bmapico/beijing.png" & "?t=" & Now.Ticks
    Public AllinoneIco As String = "http://123.207.31.37:8082/bmapico/Allinone.png" & "?t=" & Now.Ticks
    Public DirectionIco As String = "http://123.207.31.37:8082/bmapico/Direction.png" & "?t=" & Now.Ticks
    Public SH57Ico As String = "http://123.207.31.37:8082/bmapico/SH57_57.png" & "?t=" & Now.Ticks
    Public SH57_713Ico As String = "http://123.207.31.37:8082/bmapico/SH57_713.png" & "?t=" & Now.Ticks
    Public SH57_DisOnlineIco As String = "http://123.207.31.37:8082/bmapico/SH57_DisOnline.png" & "?t=" & Now.Ticks
    Public SH713Ico As String = "http://123.207.31.37:8082/bmapico/SH713.png" & "?t=" & Now.Ticks
    Public ServerUrl As String = "http://123.207.31.37:8080/?" & "?t=" & Now.Ticks
    ' Public remoteServerIP As String = "123.207.31.37"
    Public isLoadGis As Boolean = False
    Public ServerIP As String = "123.207.31.37"
    Public gisurl As String = "http://123.207.31.37:8082/baidumap.html" & "?t=" & Now.Ticks
    Public GetHttpMsgTimeSpan As Integer = 2
    Public token As String = ""

    Public usr As String = "电磁信息服务"
    Public pwd As String = "928453310"
    Public myPower As Integer
    Public NotifyList As List(Of notifyStu)
    Public NotifyLock As Object
    Public myConnectConfig As myConfig
    Structure BusLine
        Dim lineId As String
        Dim lineName As String
        Dim busNo As String
        Dim plateNumber As String
        Dim deviceId As String
        Dim deviceName As String
        Dim lng As String
        Dim lat As String
        Dim location As String
        Dim time As String
    End Structure
    Public Structure normalResponse 'json回复格式
        Public result As Boolean
        Public msg As String
        Public errmsg As String
        Public data As Object
    End Structure
    Structure myConfig
        Dim serverIP As String
        Dim serverPort As Integer
        Sub New(ByVal _serverIP As String, ByVal _serverPort As Integer)
            serverIP = _serverIP
            serverPort = _serverPort
        End Sub
    End Structure
    Structure INGMsgStu
        Dim MsgID As Integer
        Dim MsgTime As String
        Dim func As String
        Dim msg As String
        Sub New(ByVal _func As String, ByVal _msg As String)
            MsgID = 0
            MsgTime = Now.ToString("yyyy-MM-dd HH:mm:ss")
            func = _func
            msg = _msg
        End Sub
    End Structure
    Structure INGDeviceOrder
        Dim DeviceID As String
        Dim orderJson As String
        Sub New(ByVal _deviceID As String, ByVal _orderJson As String)
            DeviceID = _deviceID
            orderJson = _orderJson
        End Sub
    End Structure
    Structure tssOrder_stu
        Dim deviceID As String
        Dim task As String
        Dim freqStart As Double
        Dim freqEnd As Double
        Dim freqStep As Double
        Dim gcValue As Double
        Dim DHDevice As String
    End Structure
    Structure notifyStu
        Dim Time As String
        Dim Kind As String
        Dim Content As String
        Dim linkID As String
        Sub New(ByVal _Kind As String, ByVal _Content As String, ByVal _linkID As String)
            Time = Now.ToString("yyyy-MM-dd HH:mm:ss")
            Kind = _Kind
            Content = _Content
            linkID = _linkID
        End Sub
    End Structure
    Structure NormalTaskStu
        Dim UserName As String
        Dim TaskName As String
        Dim TaskNickName As String
        Dim DeviceID As String
        Dim DeviceName As String
        Dim StartTime As String
        Dim EndTime As String
        Dim TimeStep As String
        Dim TaskCode As String
        Dim PushWeChartToUserName As String
        Dim PushEmailToUserName As String
        Dim TaskBg As String
        Dim isMoreDevice As Boolean
    End Structure
    Structure deviceStu
        Dim DeviceID As String
        Dim Name As String
        Dim Address As String
        Dim Kind As String
        Dim OnlineTime As String
        Dim Statu As String
        Dim Lng As String
        Dim Lat As String
        Dim IP As String
        Dim Port As Integer
        Dim RunKind As String
        Dim sbzt As String
        Dim Func As String
        Dim isNetGateWay As Boolean
        Dim NetDeviceID As String
        Dim NetGateWayID As String
        Dim NetSwitch As Integer
        Dim HTTPMsgUrl As String
    End Structure
    Structure PostStu
        Dim func As String
        Dim msg As String
        Dim token As String
    End Structure
    Structure logstu
        Dim LogID As String
        Dim Time As String
        Dim Cata As String
        Dim Kind As String
        Dim Content As String
        Dim Usr As String
        Dim DeviceNickName As String
        Dim DeviceID As String
        Dim TaskNickName As String
        Dim RelateID As String
    End Structure
    Structure jobStu
        Dim Submiter As String
        Dim JobID As String
        Dim Time As String
        Dim JobFrom As String
        Dim Department As String
        Dim Worker As String
        Dim Title As String
        Dim Content As String
        Dim Status As String
        Dim StartTime As String
        Dim EndTime As String
        Dim Job As String
        Dim AlarmJob As String
        Dim DeviceID As String
        Dim TaskNickName As String
        Dim fileName As String
        Dim fileBase64 As String
    End Structure
    Public Function tobytes(ByVal by() As Byte, ByVal startindex As Integer, ByVal count As Integer) As Byte()
        If count <= 0 Then Return Nothing
        Dim bu(count - 1) As Byte
        If by.Count < count + startindex Then
            Return Nothing
        End If
        If by.Count - startindex >= count Then
            For i = 0 To count - 1
                bu(i) = by(startindex + i)
            Next
        End If
        Return bu
    End Function
    Private Function delchr(ByVal str As String) As String
        Dim St As String = ""
        St = str.Replace(ChrW(0), "")
        Return St
    End Function
    Public Function tostr(ByVal by() As Byte) As String
        If IsNothing(by) Then
            Return Nothing
        End If
        Dim str As String = Encoding.Default.GetString(by)
        Return str
    End Function
    Public Function tobyte(ByVal str As String) As Byte()
        If str = "" Then
            Return New Byte() {0}
        End If
        Dim by() As Byte = Encoding.Default.GetBytes(str)
        Return by
    End Function
    Public Function readfile(ByVal filename As String) As String
        If File.Exists(filename) = False Then Return Nothing
        Dim sr As New StreamReader(filename, Encoding.Default)
        Dim str As String = sr.ReadToEnd
        sr.Close()
        Return str
    End Function
    Public Function GetResultPara(ByVal Para As String, ByVal fromString As String) As String
        If InStr(fromString, ";") Then
            For Each sh In fromString.Split(";")
                If InStr(sh, "=") Then
                    Dim key As String = sh.Split("=")(0)
                    Dim value As String = sh.Split("=")(1)
                    If key = Para Then
                        Return value
                    End If
                End If
            Next
        End If
        Return ""
    End Function
    Public Function Login() As Boolean
        Dim result As String = GetH(ServerUrl, "func=login&usr={0}&pwd={1}", New String() {usr, pwd})
        If GetResultPara("result", result) = "success" Then
            token = GetResultPara("token", result)
            Return True
        Else
            MsgBox(result)
        End If
        Return False
    End Function
    Public Function GetMin(ByVal values() As Double) As Double
        If IsNothing(values) Then Return 0
        Dim min As Double = values(0)
        For Each itm In values
            If itm < min Then
                min = itm
            End If
        Next
        Return min
    End Function
    Public Function GetMax(ByVal values() As Double) As Double
        If IsNothing(values) Then Return 0
        Dim max As Double = values(0)
        For Each itm In values
            If itm > max Then
                max = itm
            End If
        Next
        Return max
    End Function
    Public Function GetDevKindByID(ByVal id As String) As String
        For Each itm In alldevlist
            If itm.Name = id Then
                Return itm.Kind
            End If
        Next
    End Function
    Public Function GetAvg(ByVal y() As Double) As Double
        Dim sum As Double
        For Each d In y
            sum = sum + d
        Next
        Return sum / (y.Count)
    End Function
    Public Function XinHaoFenLi(ByVal xx() As Double, ByVal yy() As Double, ByVal Du As Integer, ByVal fucha As Double) As Double(,)
        Dim rx As New List(Of Double)
        Dim ry As New List(Of Double)
        Try
            If xx.Count <> yy.Count Then Return Nothing
            If Du Mod 2 = 0 Then Return Nothing
            Dim avg As Double = -80
            'avg = GetAvg(yy)
            'For i = 0 To xx.Count - 1
            '    Dim isxinhao As Boolean = True
            '    If yy(i) < avg Then
            '        isxinhao = False
            '    End If
            '    If isxinhao Then
            '        rx.Add(xx(i))
            '        ry.Add(yy(i))
            '    End If
            'Next
            avg = GetAvg(yy)
            Dim jieti As Integer = (Du - 1) / 2
            For i = jieti To xx.Count - 1 - jieti
                Dim isxinhao As Boolean = True
                If (yy(i) < avg) Then
                    isxinhao = False
                    Continue For
                End If
                For j = i - jieti To i - 1
                    If yy(j) >= yy(j + 1) Then
                        isxinhao = False
                        Exit For
                    End If
                Next
                For j = i To i + jieti - 1
                    If yy(j) <= yy(j + 1) Then
                        isxinhao = False
                        Exit For
                    End If
                Next
                If yy(i) - fucha < yy(i + jieti) Then
                    isxinhao = False
                End If
                If yy(i) - fucha < yy(i - jieti) Then
                    isxinhao = False
                End If
                If isxinhao Then
                    rx.Add(xx(i))
                    ry.Add(yy(i))
                End If
            Next
            Dim result(rx.Count - 1, 1) As Double
            For i = 0 To rx.Count - 1
                result(i, 0) = rx(i)
                result(i, 1) = ry(i)
            Next
            Return result
        Catch ex As Exception
            '‘MsgBox(ex.ToString)
        End Try
    End Function
    Public Function XinHaoFenLi2(ByVal xx() As Double, ByVal yy() As Double, ByVal Du As Integer, ByVal fucha As Double, ByVal minCha As Integer) As Double(,)
        Dim rx As New List(Of Double)
        Dim ry As New List(Of Double)
        Try
            If xx.Count <> yy.Count Then Return Nothing

            If Du Mod 2 = 0 Then Return Nothing
            Dim jieti As Integer = (Du - 1) / 2
            For i = jieti To xx.Count - 1 - jieti
                Dim isxinhao As Boolean = True
                If yy(i) <= 0 Then
                    isxinhao = False
                End If
                'For j = i - jieti To i - 1
                '    If yy(j) >= yy(j + 1) Then
                '        isxinhao = False
                '        Exit For
                '    End If
                'Next
                'For j = i To i + jieti - 1
                '    If yy(j) <= yy(j + 1) Then
                '        isxinhao = False
                '        Exit For
                '    End If
                'Next
                For j = i - jieti To i - 1
                    If yy(j) <= 0 Then isxinhao = False
                    If yy(i) - yy(j) < fucha Then
                        isxinhao = False
                    End If
                Next
                For j = i + 1 To i + jieti - 1
                    If yy(j) <= 0 Then isxinhao = False
                    If yy(i) - yy(j) < fucha Then
                        isxinhao = False
                    End If
                Next
                If yy(i) - fucha < yy(i - 1) Then
                    isxinhao = False
                End If
                If yy(i) - fucha < yy(i + 1) Then
                    isxinhao = False
                End If
                If yy(i) < minCha Then
                    isxinhao = False
                End If
                If isxinhao Then
                    rx.Add(xx(i))
                    ry.Add(yy(i))
                    ' MsgBox(xx(i) & "," & yy(i))
                End If
            Next
            Dim result(rx.Count - 1, 1) As Double
            For i = 0 To rx.Count - 1
                result(i, 0) = rx(i)
                result(i, 1) = ry(i)
            Next
            Return result
        Catch ex As Exception
            '‘MsgBox(ex.ToString)
        End Try

    End Function
    Public Function GetNorResult(ByVal paraName As String, ByVal result As String) As String
        If InStr(result, ";") = False Then Return ""
        For Each itm In result.Split(";")
            If InStr(itm, "=") Then
                Dim k As String = itm.Split("=")(0)
                Dim v As String = itm.Split("=")(1)
                If k = paraName Then
                    Return v
                End If
            End If
        Next
        Return ""
    End Function
    Public Function GetServerResult(ByVal str As String) As String
        Dim resutl As String = GetH(ServerUrl, str & "&token=" & token)
        If GetResultPara("result", resutl) = "fail" Then
            If GetResultPara("msg", resutl) = "Please login" Then
                Login()
                resutl = GetH(ServerUrl, str & "&token=" & token)
            End If
        End If
        Return resutl
    End Function
    Public Function GetServerNp(ByVal str As String) As normalResponse
        Dim resutl As String = GetH(ServerUrl, str & "&token=" & token)
        If GetResultPara("result", resutl) = "fail" Then
            If GetResultPara("msg", resutl) = "Please login" Then
                Login()
                resutl = GetH(ServerUrl, str & "&token=" & token)
            End If
        End If
        Dim np As normalResponse
        Try
            np = JsonConvert.DeserializeObject(resutl, GetType(normalResponse))
        Catch ex As Exception

        End Try
        Return np
    End Function
    Public Function PostServerResult(ByVal str As String) As String
        Dim resutl As String = postH(ServerUrl, str)
        If GetResultPara("result", resutl) = "fail" Then
            If GetResultPara("msg", resutl) = "Please login" Then
                Login()
                resutl = postH(ServerUrl, str)
            End If
        End If
        Return resutl
    End Function
    Public Function img2data(ByVal bmp As Bitmap) As String
        Try
            Dim ms As New MemoryStream
            bmp.Save(ms, Imaging.ImageFormat.Jpeg)
            Dim arr(ms.Length) As Byte
            ms.Position = 0
            ms.Read(arr, 0, ms.Length)
            ms.Close()
            Return Convert.ToBase64String(arr)
        Catch ex As Exception
            Return ""
        End Try
    End Function
    Public Function data2img(ByVal base64 As String) As Bitmap
        Try
            Dim b() As Byte = Convert.FromBase64String(base64)
            Dim ms As New MemoryStream(b)
            Dim bitmap As New Bitmap(ms)
            Return bitmap
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function GetDeviceIDByName(ByVal dName As String) As String
        If IsNothing(alldevlist) Then Return "" '
        If dName = "" Then Return ""
        For Each itm In alldevlist
            If itm.Name = dName Then
                Return itm.DeviceID
            End If
        Next
        Return ""
    End Function
    Public Function GetDeviceNameById(ByVal id As String) As String
        If IsNothing(alldevlist) Then Return "" '
        If id = "" Then Return ""
        For Each itm In alldevlist
            If itm.DeviceID = id Then
                Return itm.Name
            End If
        Next
        Return ""
    End Function
    Public Function TransforPara2Query(ByVal dik As Dictionary(Of String, String)) As String
        If IsNothing(dik) Then Return ""
        Dim str As String = ""
        For Each itm In dik
            str = str & "&" & itm.Key & "=" & itm.Value
        Next
        If str <> "" Then
            str = str.Substring(1, str.Length - 1)
        End If
        Return str
    End Function
    Public Function GethWithToken(ByVal url As String, ByVal msg As String) As String
        Dim result As String = GetH(url, "?" & msg & "&token=" & token)
        If InStr(result, "Please login") Then
            Login()
            result = GetH(url, "?" & msg & "&token=" & token)
            Return result
        End If

        Return result
    End Function
    Public Sub AddNotify(ByVal n As notifyStu)
        If IsNothing(n) Then Return
        If IsNothing(NotifyList) Then NotifyList = New List(Of notifyStu)
        If IsNothing(NotifyLock) Then NotifyLock = New Object
        SyncLock NotifyLock
            NotifyList.Add(n)
        End SyncLock
    End Sub
    Public Sub RemoveNotify(ByVal n As notifyStu)
        If IsNothing(n) Then Return
        If IsNothing(NotifyList) Then NotifyList = New List(Of notifyStu)
        If IsNothing(NotifyLock) Then NotifyLock = New Object
        SyncLock NotifyLock
            NotifyList.Remove(n)
        End SyncLock
    End Sub
    Public Sub RemoveNotify(ByVal Time As String, ByVal Content As String)
        If IsNothing(NotifyList) Then NotifyList = New List(Of notifyStu)
        If IsNothing(NotifyLock) Then NotifyLock = New Object
        SyncLock NotifyLock

            For i = NotifyList.Count - 1 To 0 Step -1

                If NotifyList(i).Time = Time And NotifyList(i).Content = Content Then

                    NotifyList.RemoveAt(i)
                    Exit For
                End If
            Next
        End SyncLock
    End Sub
    Public Function Compress(ByVal data() As Byte) As Byte()
        Dim stream As MemoryStream = New MemoryStream
        Dim gZip As New GZipStream(stream, CompressionMode.Compress)
        gZip.Write(data, 0, data.Length)
        gZip.Close()
        Return stream.ToArray
    End Function
    Public Function Decompress(ByVal data() As Byte) As Byte()
        Dim stream As MemoryStream = New MemoryStream
        Dim gZip As New GZipStream(New MemoryStream(data), CompressionMode.Decompress)
        Dim n As Integer = 0
        While True
            Dim by(409600) As Byte
            n = gZip.Read(by, 0, by.Length)
            If n = 0 Then Exit While
            stream.Write(by, 0, n)
        End While
        gZip.Close()
        Return stream.ToArray
    End Function
    Public Function PPSJValues2DSGBase(ByVal value() As Double) As String
        If IsNothing(value) Then Return ""
        Dim by() As Byte
        For Each v In value
            Dim k As Single = v
            Dim b() As Byte = BitConverter.GetBytes(k)
            If IsNothing(by) Then
                by = b
            Else
                by = by.Concat(b).ToArray
            End If
        Next
        Dim nb() As Byte = Compress(by)
        Dim base64 As String = Convert.ToBase64String(nb)
        Return base64
    End Function
    Public Function DSGBase2PPSJValues(ByVal base64 As String) As Double()
        If base64 = "" Then Return Nothing
        Try
            Dim nb() As Byte = Convert.FromBase64String(base64)
            Dim jb() As Byte = Decompress(nb)
            Dim list As New List(Of Double)
            For i = 0 To jb.Length - 1 Step 4
                Dim k As Single = BitConverter.ToSingle(jb, i)
                Dim d As Double = k
                list.Add(d.ToString("0.0"))
            Next
            Return list.ToArray
        Catch ex As Exception

        End Try
        Return Nothing
    End Function
End Module
