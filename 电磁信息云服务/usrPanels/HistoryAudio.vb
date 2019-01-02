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
Imports System.Security.AccessControl
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Media
Public Class HistoryAudio
    Dim selectAudioBuffer() As Byte
    Private Sub HistoryAudio_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
        Label2.Visible = False
        Label3.Visible = False
        Control.CheckForIllegalCrossThreadCalls = False
        ini()
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 50)
        LVDetail.Columns.Add("文件名", 200)
        inichart2()
        GetHistoryAudioList()
    End Sub
    Private Sub inichart2()
        Chart2.Series.Clear()
        Chart2.ChartAreas(0).AxisY.Maximum = 255
        Chart2.ChartAreas(0).AxisY.Minimum = 0
        Chart2.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart2.ChartAreas(0).AxisY.Interval = 51
        Chart2.ChartAreas(0).AxisY.IntervalOffset = 51
    End Sub
    Private Sub GetHistoryAudioList()
        Label2.Visible = True
        Dim result As String = GetServerResult("func=GetHistoryAudioList")
        Label2.Visible = False
        If result = "" Or result = "[]" Then Return
        Dim list As New List(Of String)
        list = JsonConvert.DeserializeObject(result, GetType(List(Of String)))
        LVDetail.Items.Clear()
        For i = 0 To list.Count - 1
            Dim n As String = list(i)
            Dim itm As New ListViewItem(i + 1)
            itm.SubItems.Add(n)
            LVDetail.Items.Add(itm)
        Next
        If LVDetail.Items.Count > 0 Then lvdetailBeSelect(0)
    End Sub
    Private Sub lvdetailBeSelect(ByVal index As Integer)
        Dim th As New Thread(AddressOf th_showAudio)
        th.Start(index)
    End Sub
    Private Sub th_showAudio(ByVal index As Integer)
        Label3.Visible = True
        Dim audiofilename As String = LVDetail.Items(index).SubItems(1).Text
        Dim result As String = GetServerResult("func=GetAudioBase64ByAudioFileName&audiofilename=" & audiofilename)
        Label3.Visible = False
        If result = "null" Then
            MsgBox("没有该文件")
            Exit Sub
        End If
        Try
            selectAudioBuffer = Convert.FromBase64String(result)
        Catch ex As Exception
            Exit Sub
        End Try

        Label6.Text = audiofilename
        Label11.Text = getWenJianDaXiao(selectAudioBuffer)
        Label12.Text = getShiChang(selectAudioBuffer)
        Dim Series As New Series '频谱   0
        Series.Label = "音频数据"
        Series.XValueType = ChartValueType.Auto
        Series.ChartType = SeriesChartType.FastLine
        Series.IsVisibleInLegend = False
        For i = 0 To selectAudioBuffer.Count - 1
            Series.Points.Add(selectAudioBuffer(i))
        Next
        Chart2.Series.Clear()
        Chart2.Series.Add(Series)
    End Sub
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetHistoryAudioList()
    End Sub
    Private Function getWenJianDaXiao(ByVal by() As Byte) As String
        Dim num As Double = 0
        num = by.Count
        If num < 1024 Then Return num & "b"
        If 1024 <= num And num < 1024 * 1024 Then
            Return Format(num / 1024, "0.0") & "Kb"
        End If
        If 1024 * 1024 <= num Then
            Return Format(num / (1024 * 1024), "0.0") & "Mb"
        End If
    End Function
    Private Function getShiChang(ByVal by() As Byte) As String
        Dim num As Integer = by.Count
        If num < 45 Then Return "00:00:00"
        Dim chang As Double = (num - 45) / 8000
        Dim HH As Integer
        Dim mm As Integer
        Dim ss As Integer
        If chang < 60 Then
            HH = 0
            mm = 0
            ss = chang
        End If
        If 60 <= chang And chang < 3600 Then
            HH = 0
            ss = chang Mod 60
            mm = (chang - ss) / 60
        End If
        If 3600 <= chang Then
            Dim m As Double = chang Mod 3600
            HH = (chang - m) / 3600
            If m < 60 Then
                mm = 0
                ss = m
            End If
            If 60 <= m Then
                ss = m Mod 60
                mm = (m - ss) / 60
            End If
        End If
        Return HH.ToString("00") & ":" & mm.ToString("00") & ":" & ss.ToString("00")
    End Function
    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedIndices.Count = 0 Then Return
        lvdetailBeSelect(LVDetail.SelectedIndices(0))
    End Sub

    Private Sub Label7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label7.Click
        playAudio()
    End Sub

    Private Sub Panel16_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel16.Click
        playAudio()
    End Sub
    Private Sub playAudio()
        sp = New SoundPlayer
        sp.Stop()
        If IsNothing(selectAudioBuffer) Then
            MsgBox("请选择一个文件")
            Exit Sub
        End If
        play(selectAudioBuffer)
    End Sub
    Dim sp As SoundPlayer
    Private Sub play(ByVal buf() As Byte)
        Dim th As New Thread(AddressOf sub_play)
        th.Start(buf)
    End Sub
    Dim isend As Boolean = False
    Private Sub sub_play(ByVal buf() As Byte)
        Try
            Dim ms As MemoryStream = New MemoryStream(buf)
            sp = New SoundPlayer(ms)
            isend = False
            sp.Play()
            isend = True
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Label8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label8.Click
        Try
            sp.Stop()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Panel17_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel17.Click
        Try
            sp.Stop()
        Catch ex As Exception

        End Try
    End Sub

End Class
