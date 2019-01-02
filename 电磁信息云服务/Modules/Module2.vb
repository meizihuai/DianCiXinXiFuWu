Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Collections.Generic
Imports System
Imports System.Collections.Specialized
Imports System.Threading
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Microsoft.Win32
Imports System.ComponentModel
Imports 电磁信息云服务.BusFreqGis

Module Module2
    Public upload As Double
    Public dowload As Double

    Public Function postH(ByVal uri As String, ByVal msg As String) As String
        ' While True
        Try
            ' ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf CheckValidationResult)
            Dim req As HttpWebRequest = WebRequest.Create(uri)
            req.Accept = "*/*"
            req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"

            req.KeepAlive = True
            req.ContentType = "application/x-www-form-urlencoded"
            req.Method = "POST"
            Dim data As String = msg
            Dim bytes() As Byte = Encoding.UTF8.GetBytes(data)
            Dim st As Stream = req.GetRequestStream
            st.Write(bytes, 0, bytes.Length)
            st.Flush()
            st.Close()
            Dim b() As Byte = Encoding.Default.GetBytes(msg)
            upload = upload + b.Length
            Dim rp As HttpWebResponse = req.GetResponse
            Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd
            b = Encoding.Default.GetBytes(str)
            dowload = dowload + b.Length
            Return str
        Catch ex As Exception

        End Try
        'End While
    End Function
    Public Function GetH(ByVal uri As String, ByVal msg As String) As String
        Dim num As Integer = 0
        While True
            Try
                Dim kkk As String = uri & msg
                Dim req As HttpWebRequest = WebRequest.Create(kkk)
                req.Accept = "*/*"
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
                req.CookieContainer = New CookieContainer
                req.KeepAlive = True
                req.ContentType = "application/x-www-form-urlencoded"
                req.Method = "GET"
                Dim b() As Byte = Encoding.Default.GetBytes(msg)
                upload = upload + b.Length
                Dim rp As HttpWebResponse = req.GetResponse
                Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd
                b = Encoding.Default.GetBytes(str)
                dowload = dowload + b.Length
                Return str
            Catch ex As Exception

            End Try
            num = num + 1
            If num = 4 Then Return ""
        End While
    End Function
    Public Function GetH(ByVal uri As String, ByVal msg As String, ByVal para() As String) As String
        Dim num As Integer = 0
        While True
            Try
                msg = String.Format(msg, para)
                Dim req As HttpWebRequest = WebRequest.Create(uri & msg)
                req.Accept = "*/*"
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13"
                req.CookieContainer = New CookieContainer
                req.KeepAlive = True
                req.ContentType = "application/x-www-form-urlencoded"
                req.Method = "GET"
                Dim b() As Byte = Encoding.Default.GetBytes(msg)
                upload = upload + b.Length
                Dim rp As HttpWebResponse = req.GetResponse
                Dim str As String = New StreamReader(rp.GetResponseStream(), Encoding.UTF8).ReadToEnd
                b = Encoding.Default.GetBytes(str)
                dowload = dowload + b.Length
                Return str
            Catch ex As Exception

            End Try
            num = num + 1
            If num = 4 Then Return ""
        End While
    End Function
    Structure GPSTranslatInfo
        Dim status As Integer
        Dim result As CoordInfo()
    End Structure
    Structure CoordInfo
        Dim x As Double
        Dim y As Double
    End Structure
    Structure TekBusDeviceInfo
        Dim lineId As Integer
        Dim gpsTime As String
        Dim freqUpdateTime As String
        Dim TekDeviceId As String
        Dim GwDeviceId As String
        Dim HDeviceId As String
        Dim TekCoordinfo As CoordInfo
        Dim GwCoordinfo As CoordInfo
        Dim HkCoordinfo As CoordInfo
        Dim freqJson As json_PPSJ
        Dim oldPointInfo As String
        Dim lng As Double
        Dim lat As Double
    End Structure
    Public Function GPS2BDS(ByVal coords As String) As CoordInfo()
        ' http://api.map.baidu.com/geoconv/v1/?coords=114.21892734521,29.575429778924;114.21892734521,29.575429778924&from=1&to=5&ak=你的密钥
        Dim ak As String = "5cW7OlxZXThtbkq1Y0u5yNO6"
        Dim url As String = "http://api.map.baidu.com/geoconv/v1/?coords=" & coords & "&from=1&to=5&ak=" & ak
        ' Console.WriteLine(coords)
        Dim result As String = GetH(url, "")
        Try
            Dim info As GPSTranslatInfo = JsonConvert.DeserializeObject(result, GetType(GPSTranslatInfo))
            If IsNothing(info) = False Then
                If info.status = 0 Then
                    If IsNothing(info.result) = False Then
                        If info.result.Count <> 0 Then
                            ' MsgBox(url & vbCrLf & result)
                            Return info.result
                        End If
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
        Return Nothing
    End Function

    Public Sub SetWebBrowserFeatures(ByVal ieVersion As Integer)
        If LicenseManager.UsageMode = LicenseUsageMode.Runtime Then
            Dim appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
            Dim ieMode As UInt32 = GeoEmulationModee(ieVersion)
            Dim featureControlRegKey As String = "HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\"
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION", appName, ieMode, RegistryValueKind.DWord)
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION", appName, 1, RegistryValueKind.DWord)

        End If
    End Sub
    Private Function GetBrowserVersion() As Integer
        Dim browserVersion As Integer = 0
        Dim ieKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Internet Explorer", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.QueryValues)
        Dim version = ieKey.GetValue("svcVersion")
        If IsNothing(version) Then
            Throw New ApplicationException("Microsoft Internet Explorer is required!")
        End If
        Integer.TryParse(version.ToString().Split(".")(0), browserVersion)
        If browserVersion < 7 Then
            Throw New ApplicationException("不支持的浏览器版本!")
        End If
        Return browserVersion
    End Function
    Private Function GeoEmulationModee(ByVal browserVersion As Integer) As UInt32
        Dim mode As UInt32 = 11000
        Select Case browserVersion
            Case 7
                mode = 7000
                Exit Select
            Case 8
                mode = 8000
                Exit Select
            Case 9
                mode = 9000
                Exit Select
            Case 10
                mode = 10000
                Exit Select
            Case 11
                mode = 11000
                Exit Select
        End Select
        Return mode
    End Function
End Module

