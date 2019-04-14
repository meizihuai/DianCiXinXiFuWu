Public Class LisanInfo
    Public freqStart As Double
    Public freqEnd As Double
    Public pointlist As List(Of LisanPointInfo)
    Public startTime As Date
    Public watchTime As Integer
End Class
Public Class LisanPointInfo
    Public freq As Double
    Public sigNalHalfWidth As Integer
    Public sigNalInfo As String  '信号属性
    Public sigNalStatus As String '状态属性
    Public isFree As Boolean '是否可用
    Public dbm As Double '信号电平
    Public dbm_max As Double
    Public dbm_min As Double
    Public dbm_avg As Double
    Public watchTime As Integer
    Public overCount As Integer  '信号出现次数
    Public overPercent As Integer '占用度
    Sub New()

    End Sub
    Sub New(freq As Double, dbm As Double, Optional sigNalHalfWidth As Integer = 1)
        Me.freq = freq
        Me.dbm = dbm
        Me.dbm_max = dbm
        Me.dbm_min = dbm
        Me.dbm_avg = dbm
        Me.isFree = True
        Me.sigNalInfo = "未知"
        Me.sigNalStatus = "正常"
        Me.watchTime = 1
        Me.sigNalHalfWidth = sigNalHalfWidth
    End Sub
End Class
Public Class SignalHelper
    Public Shared Function GetSigNals(xx() As Double, yy() As Double, freqwidth As Integer, fucha As Double, Optional overLever As Double = -80) As List(Of LisanPointInfo)
        Dim result As Double(,) = XinHaoFenLi(xx, yy, freqwidth, fucha)
        If IsNothing(result) Then Return Nothing
        Dim sigNalHalfWidth As Integer = (AutoFenXiDu - 1) / 2
        Dim sigNalCount As Integer = result.GetLength(0)
        If sigNalCount = 0 Then Return Nothing
        Dim list As New List(Of LisanPointInfo)
        For i = 0 To sigNalCount - 1
            Dim x As Double = result(i, 0)
            Dim y As Double = result(i, 1)
            list.Add(New LisanPointInfo(Math.Round(x, 2), y, sigNalHalfWidth))
        Next
        For i = 0 To xx.Count - 1
            Dim x As Double = xx(i)
            Dim y As Double = yy(i)
            If y > -70 Then
                list.Add(New LisanPointInfo(Math.Round(x, 2), y, sigNalHalfWidth))
            End If
        Next
        Return list
    End Function
    Public Shared Function JionSigNalList(oldList As List(Of LisanPointInfo), newList As List(Of LisanPointInfo), Optional overLever As Double = -80) As List(Of LisanPointInfo)
        If IsNothing(oldList) Then Return newList
        If IsNothing(newList) Then Return oldList
        For Each itm In oldList
            Dim oldValue As Double = itm.dbm
            For Each nItm In newList
                If itm.freq = nItm.freq Then
                    itm.dbm = nItm.dbm
                    Exit For
                End If
            Next
            If itm.dbm_max < itm.dbm Or itm.dbm_max = 0 Then itm.dbm_max = itm.dbm
            If itm.dbm_min > itm.dbm Or itm.dbm_min = 0 Then itm.dbm_min = itm.dbm
            itm.dbm_avg = (itm.dbm + oldValue) / 2
            itm.watchTime = itm.watchTime + 1
            itm.sigNalStatus = "正常"
            If itm.dbm > overLever Then
                itm.overCount = itm.overCount + 1
                itm.sigNalStatus = "超标"
            End If
            itm.overPercent = Math.Round(（100 * itm.overCount / itm.watchTime), 2)
            itm.isFree = itm.overPercent < 50
        Next
        Return oldList
    End Function
    Public Shared Function TolistViewItems(list As List(Of LisanPointInfo), time As String, watchTimeSpan As String, location As String) As ListViewItem()
        If IsNothing(list) Then Return Nothing
        Dim itmlist As New List(Of ListViewItem)
        For i = 0 To list.Count - 1
            Dim p As LisanPointInfo = list(i)
            Dim itm As New ListViewItem(i + 1)
            itm.SubItems.Add(time)
            itm.SubItems.Add(p.freq)
            itm.SubItems.Add(location)
            itm.SubItems.Add(p.sigNalInfo)  '信号属性  '4  信号属性
            itm.SubItems.Add(p.sigNalStatus)  '状态属性  '5  状态属性
            itm.SubItems.Add(IIf(p.isFree, "可用", "不可用"))  '可用评估  '6  可用评估
            itm.SubItems.Add(p.dbm) '7  信号电平
            itm.SubItems.Add(p.dbm_min) '8  最小值
            itm.SubItems.Add(p.dbm_max) '9  最大值
            itm.SubItems.Add(p.overCount) '10  出现次数
            itm.SubItems.Add(p.dbm_avg) '11 平均值
            itm.SubItems.Add(watchTimeSpan) '12 统计时长
            itm.SubItems.Add(p.overPercent) '13  占用度
            itm.SubItems.Add(p.watchTime) '14  监测次数
            itm.SubItems.Add(p.overCount) '15 超标次数
            Dim CBInt As Integer = Val(itm.SubItems(12 + 3).Text)
            Dim SumInt As Integer = Val(itm.SubItems(11 + 3).Text)
            Dim str As String = GetPerPic(CBInt / SumInt)
            itm.SubItems.Add(str) '占用度直方图
            If p.sigNalStatus <> "正常" Then
                itm.ForeColor = Color.Red
            End If
            itmlist.Add(itm)
        Next
        Return itmlist.ToArray()
    End Function
    Private Shared Function GetPerPic(ByVal v As Double) As String
        If v >= 1 Then v = 1
        If v <= 0 Then v = 0
        If v = 0 Then Return ""
        Dim fk As String = "■"
        Dim value As Integer = v * 10
        Dim result As String = ""
        For i = 1 To value
            result = result + fk
        Next
        Return result
    End Function
End Class
