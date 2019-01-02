Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms.DataVisualization.Charting
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net
Imports System.Math
Imports SpeechLib
Imports System.Media
Imports OfficeOpenXml
Public Class SetChatInfo
    Dim mychart As Chart
    Sub New(ByVal _mychart As chart)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        mychart = _mychart
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub SetChatInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.TopMost = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Dim max As Double = mychart.ChartAreas(0).AxisY.Maximum
        Dim min As Double = mychart.ChartAreas(0).AxisY.Minimum
        Dim perValue As Double = mychart.ChartAreas(0).AxisY.Interval
        Dim count As Double = (max - min) / perValue
        count = count.ToString("0.00")
        TxtMax.Text = max
        TxtMin.Text = min
        TxtCount.Text = count
        LblPerValue.Text = perValue
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim max As Double = Val(TxtMax.Text)
        Dim min As Double = Val(TxtMin.Text)
        Dim count As Double = Val(TxtCount.Text)
        Dim perValue As Double = (max - min) / count
        If max < min Then
            MsgBox("最大值刻度 不能小于 最小值刻度")
            Return
        End If
        mychart.ChartAreas(0).AxisY.Maximum = max
        mychart.ChartAreas(0).AxisY.Minimum = min
        mychart.ChartAreas(0).AxisY.Interval = perValue
        Me.Close()
    End Sub

    Private Sub TxtCount_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtCount.TextChanged
        Dim max As Double = Val(TxtMax.Text)
        Dim min As Double = Val(TxtMin.Text)
        Dim count As Double = Val(TxtCount.Text)
        Dim perValue As Double = (max - min) / count
        LblPerValue.Text = perValue
        If max < min Then
            Return
        End If
        mychart.ChartAreas(0).AxisY.Maximum = max
        mychart.ChartAreas(0).AxisY.Minimum = min
        mychart.ChartAreas(0).AxisY.Interval = perValue
    End Sub

    Private Sub TxtMax_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtMax.TextChanged
        Dim max As Double = Val(TxtMax.Text)
        Dim min As Double = Val(TxtMin.Text)
        Dim count As Double = Val(TxtCount.Text)
        Dim perValue As Double = (max - min) / count
        LblPerValue.Text = perValue
        If max < min Then         
            Return
        End If
        mychart.ChartAreas(0).AxisY.Maximum = max
        mychart.ChartAreas(0).AxisY.Minimum = min
        mychart.ChartAreas(0).AxisY.Interval = perValue
    End Sub

    Private Sub TxtMin_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TxtMin.TextChanged
        Dim max As Double = Val(TxtMax.Text)
        Dim min As Double = Val(TxtMin.Text)
        Dim count As Double = Val(TxtCount.Text)
        Dim perValue As Double = (max - min) / count
        LblPerValue.Text = perValue
        If max < min Then
            Return
        End If
        mychart.ChartAreas(0).AxisY.Maximum = max
        mychart.ChartAreas(0).AxisY.Minimum = min
        mychart.ChartAreas(0).AxisY.Interval = perValue
    End Sub
End Class