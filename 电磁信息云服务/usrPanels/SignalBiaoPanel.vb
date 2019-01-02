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
Public Class SignalBiaoPanel
    Public SigNalName As Double
    Sub New(ByVal _SigNalName As Double)
        ' 此调用是设计器所必需的。
        InitializeComponent()
        SigNalName = _SigNalName
        drawValue(PBX1, 0, _SigNalName & "MHz")
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub
    Private Sub SignalBiaoPanel_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Top
        iniChart1()
    End Sub
    Private Sub iniChart1()
        Chart1.Series.Clear()
        Chart1.ChartAreas(0).AxisY.Maximum = -20
        Chart1.ChartAreas(0).AxisY.Minimum = -120
        Chart1.ChartAreas(0).AxisX.LabelStyle.IsStaggered = False
        Chart1.ChartAreas(0).AxisY.Interval = 20
        Chart1.ChartAreas(0).AxisY.IntervalOffset = 20
        'Dim Series As New DataVisualization.Charting.Series
        'Series.Label = "频谱数据"
        'Series.XValueType = ChartValueType.Auto
        'Series.ChartType = SeriesChartType.FastLine
        'Series.IsVisibleInLegend = False
        'Chart1.Series.Add(Series)     
    End Sub
    Public Sub SetSignalValue(ByVal msgTime As String, ByVal v As Double)
        drawValue(PBX1, v, SigNalName & "MHz," & v & "dBm")
        If Chart1.Series.Count <= 0 Then
            Dim Series2 As New Series("")
            Series2.Color = Color.Blue
            Series2.IsValueShownAsLabel = True
            Series2.IsVisibleInLegend = False
            Series2.ToolTip = "#VAL"
            Series2.Color = Color.Blue
            Series2.ChartType = SeriesChartType.FastLine
            Series2.Points.AddXY(msgTime, v)
            Chart1.Series.Add(Series2)
            Dim Series As New Series("1")
            Series.Color = Color.Blue
            Series.IsValueShownAsLabel = True
            Series.IsVisibleInLegend = False
            Series.ToolTip = "#VAL"
            Series.Color = Color.Blue
            Series.ChartType = SeriesChartType.FastLine
            For i = 0 To 1999
                Series.Points.Add(-120)
            Next
            Chart1.Series.Add(Series)
        Else
            If Chart1.Series(0).Points.Count > 2000 Then
                Chart1.Series(0).Points.RemoveAt(0)
            End If
            Chart1.Series(0).Points.AddXY(msgTime, v)
        End If
    End Sub
    Private Sub drawValue(ByVal pbx As PictureBox, ByVal db As Double, ByVal msg As String)
        Dim minDB As Double = -110
        Dim maxDB As Double = -20
        If db < minDB Then db = minDB
        If db > maxDB Then
            If db = 0 Then
                db = minDB
            Else
                db = maxDB
            End If
        End If
        Dim bitmap As New Bitmap(pbx.Width, pbx.Height)
        pbx.BackColor = Color.Black
        Dim g As Graphics = Graphics.FromImage(bitmap)
        g.SmoothingMode = SmoothingMode.AntiAlias
        'g.InterpolationMode = InterpolationMode.HighQualityBicubic
        'g.CompositingQuality = CompositingQuality.AssumeLinear
        Dim a As Integer = pbx.Width / 2
        Dim b As Integer = pbx.Height * 0.9
        g.FillEllipse(Brushes.Red, a - 5, b - 5, 10, 10)
        Dim maxR As Integer = pbx.Width / 2 - 15
        Dim midR As Integer = pbx.Width / 2 - 20
        Dim minR As Integer = pbx.Width / 2 - 25
        For i = 0 To 120 Step 4
            Dim x1 As Integer = a - maxR * Cos(PI * (30 + i) / 180)
            Dim y1 As Integer = b - maxR * Sin(PI * (30 + i) / 180)
            Dim x2 As Integer = a - midR * Cos(PI * (30 + i) / 180)
            Dim y2 As Integer = b - midR * Sin(PI * (30 + i) / 180)
            Dim x3 As Integer = a - minR * Cos(PI * (30 + i) / 180)
            Dim y3 As Integer = b - minR * Sin(PI * (30 + i) / 180)
            Dim pen As New Pen(New SolidBrush(Color.White), 2)
            g.DrawLine(pen, x1, y1, x2, y2)
        Next
        For i = 0 To 120 Step 20
            Dim x1 As Integer = a - maxR * Cos(PI * (30 + i) / 180)
            Dim y1 As Integer = b - maxR * Sin(PI * (30 + i) / 180)
            Dim x2 As Integer = a - midR * Cos(PI * (30 + i) / 180)
            Dim y2 As Integer = b - midR * Sin(PI * (30 + i) / 180)
            Dim x3 As Integer = a - minR * Cos(PI * (30 + i) / 180)
            Dim y3 As Integer = b - minR * Sin(PI * (30 + i) / 180)
            Dim markValue As Integer = minDB + (maxDB - minDB) * i / 120
            Select Case i
                Case 0
                    g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.White), x1 - 30, y1 - 10)
                    Dim pen As New Pen(New SolidBrush(Color.White), 3)
                    g.DrawLine(pen, x1, y1, x3, y3)
                Case 20
                    g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.White), x1 - 20, y1 - 8)
                    Dim pen As New Pen(New SolidBrush(Color.White), 3)
                    g.DrawLine(pen, x1, y1, x3, y3)
                Case 40
                    g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.White), x1 - 15, y1 - 10)
                    Dim pen As New Pen(New SolidBrush(Color.White), 3)
                    g.DrawLine(pen, x1, y1, x3, y3)
                Case 60
                    g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.White), x1 - 12, y1 - 10)
                    Dim pen As New Pen(New SolidBrush(Color.White), 3)
                    g.DrawLine(pen, x1, y1, x3, y3)
                Case 80
                    g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.White), x1 - 12, y1 - 10)
                    Dim pen As New Pen(New SolidBrush(Color.White), 3)
                    g.DrawLine(pen, x1, y1, x3, y3)
                Case 100
                    g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.White), x1 - 5, y1 - 10)
                    Dim pen As New Pen(New SolidBrush(Color.White), 3)
                    g.DrawLine(pen, x1, y1, x3, y3)
                Case 120
                    g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.White), x1, y1 - 10)
                    Dim pen As New Pen(New SolidBrush(Color.White), 3)
                    g.DrawLine(pen, x1, y1, x3, y3)
            End Select
        Next
        Dim sita As Double = 30 + 120 * (db - minDB) / (maxDB - minDB)
        Dim xValue As Integer = a - (maxR + 0) * Cos(PI * sita / 180)
        Dim yValue As Integer = b - (maxR + 0) * Sin(PI * sita / 180)
        Dim penValue As New Pen(Brushes.Red, 2)
        g.DrawLine(penValue, a, b, xValue, yValue)
        Dim sizef As SizeF = g.MeasureString(msg, New Font("宋体", 10))
        g.DrawString(msg, New Font("宋体", 10), Brushes.White, pbx.Width / 2 - sizef.Width / 2, pbx.Height / 2)
        g.Save()
        'bitmap.Save("./tmp.jpg")
        pbx.Image = bitmap
    End Sub
    'Private Sub drawValue(ByVal pbx As PictureBox, ByVal db As Double, ByVal msg As String)
    '    If db < -110 Then db = -110
    '    If db > -20 Then
    '        If db = 0 Then
    '            db = -110
    '        Else
    '            db = -20
    '        End If
    '    End If
    '    Dim bitmap As New Bitmap(pbx.Width, pbx.Height)
    '    Dim g As Graphics = Graphics.FromImage(bitmap)
    '    g.SmoothingMode = SmoothingMode.AntiAlias
    '    'g.InterpolationMode = InterpolationMode.HighQualityBicubic
    '    'g.CompositingQuality = CompositingQuality.AssumeLinear
    '    Dim a As Integer = pbx.Width / 2
    '    Dim b As Integer = pbx.Height * 0.9
    '    g.FillEllipse(Brushes.Red, a - 5, b - 5, 10, 10)
    '    Dim maxR As Integer = pbx.Width / 2 - 15
    '    Dim midR As Integer = pbx.Width / 2 - 20
    '    Dim minR As Integer = pbx.Width / 2 - 25
    '    For i = 0 To 120 Step 4
    '        Dim x1 As Integer = a - maxR * Cos(PI * (30 + i) / 180)
    '        Dim y1 As Integer = b - maxR * Sin(PI * (30 + i) / 180)
    '        Dim x2 As Integer = a - midR * Cos(PI * (30 + i) / 180)
    '        Dim y2 As Integer = b - midR * Sin(PI * (30 + i) / 180)
    '        Dim x3 As Integer = a - minR * Cos(PI * (30 + i) / 180)
    '        Dim y3 As Integer = b - minR * Sin(PI * (30 + i) / 180)
    '        Dim pen As New Pen(New SolidBrush(Color.Black), 2)
    '        g.DrawLine(pen, x1, y1, x2, y2)
    '    Next
    '    For i = 0 To 120 Step 20
    '        Dim x1 As Integer = a - maxR * Cos(PI * (30 + i) / 180)
    '        Dim y1 As Integer = b - maxR * Sin(PI * (30 + i) / 180)
    '        Dim x2 As Integer = a - midR * Cos(PI * (30 + i) / 180)
    '        Dim y2 As Integer = b - midR * Sin(PI * (30 + i) / 180)
    '        Dim x3 As Integer = a - minR * Cos(PI * (30 + i) / 180)
    '        Dim y3 As Integer = b - minR * Sin(PI * (30 + i) / 180)
    '        Dim markValue As Integer = -110 + 90 * i / 120
    '        Select Case i
    '            Case 0
    '                g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.Black), x1 - 30, y1 - 10)
    '                Dim pen As New Pen(New SolidBrush(Color.Black), 3)
    '                g.DrawLine(pen, x1, y1, x3, y3)
    '            Case 20
    '                g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.Black), x1 - 20, y1 - 8)
    '                Dim pen As New Pen(New SolidBrush(Color.Black), 3)
    '                g.DrawLine(pen, x1, y1, x3, y3)
    '            Case 40
    '                g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.Black), x1 - 15, y1 - 10)
    '                Dim pen As New Pen(New SolidBrush(Color.Black), 3)
    '                g.DrawLine(pen, x1, y1, x3, y3)
    '            Case 60
    '                g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.Black), x1 - 12, y1 - 10)
    '                Dim pen As New Pen(New SolidBrush(Color.Black), 3)
    '                g.DrawLine(pen, x1, y1, x3, y3)
    '            Case 80
    '                g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.Black), x1 - 12, y1 - 10)
    '                Dim pen As New Pen(New SolidBrush(Color.Black), 3)
    '                g.DrawLine(pen, x1, y1, x3, y3)
    '            Case 100
    '                g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.Black), x1 - 5, y1 - 10)
    '                Dim pen As New Pen(New SolidBrush(Color.Black), 3)
    '                g.DrawLine(pen, x1, y1, x3, y3)
    '            Case 120
    '                g.DrawString(markValue, New Font("宋体", 8), New SolidBrush(Color.Black), x1, y1 - 10)
    '                Dim pen As New Pen(New SolidBrush(Color.Black), 3)
    '                g.DrawLine(pen, x1, y1, x3, y3)
    '        End Select
    '    Next
    '    Dim sita As Double = 30 + 120 * (110 + db) / 90
    '    Dim xValue As Integer = a - (maxR + 0) * Cos(PI * sita / 180)
    '    Dim yValue As Integer = b - (maxR + 0) * Sin(PI * sita / 180)
    '    Dim penValue As New Pen(Brushes.Red, 2)
    '    g.DrawLine(penValue, a, b, xValue, yValue)
    '    Dim sizef As SizeF = g.MeasureString(msg, New Font("宋体", 10))
    '    g.DrawString(msg, New Font("宋体", 10), Brushes.Black, pbx.Width / 2 - sizef.Width / 2, pbx.Height / 2)
    '    g.Save()
    '    'bitmap.Save("./tmp.jpg")
    '    pbx.Image = bitmap
    'End Sub
  


  
    Private Sub Chart1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chart1.Click

    End Sub

    Private Sub PBX1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PBX1.Click

    End Sub
End Class
