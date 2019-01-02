Imports System
Imports System.IO
Imports System.Text
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class RunImage
    Dim bmp As Bitmap
    Structure JTStu
        Dim imgName As String
        Dim imgText As String
        Dim userName As String
        Dim base64 As String
    End Structure
    Sub New(ByVal _bmp As Bitmap)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        bmp = _bmp
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub RunImage_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.TopMost = True
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        PictureBox1.Image = bmp
        TextBox1.Text = "系统运行截图" & Now.ToString("yyyyMMddHHmmss")
        RichTextBox1.Text = Now.ToString("yyyy-MM-dd HH:mm:ss") & "  " & "系统运行记录截图"
        TextBox2.Text = usr
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If TextBox1.Text = "" Then
            MsgBox("请输入截图名称")
            Exit Sub
        End If
        If RichTextBox1.Text = "" Then
            MsgBox("请输入截图备注")
            Exit Sub
        End If
        If IsNothing(bmp) Then
            MsgBox("截图格式有误，请重新截图")
            Exit Sub
        End If
        Dim j As JTStu
        j.imgName = TextBox1.Text
        j.imgText = RichTextBox1.Text
        j.userName = TextBox2.Text
        j.base64 = img2data(bmp)
        Dim json As String = JsonConvert.SerializeObject(j)
        Dim ps As PostStu
        ps.func = "UploadRunImage"
        ps.token = token
        ps.msg = json
        json = JsonConvert.SerializeObject(ps)
        Dim result As String = PostServerResult(json)
        If GetResultPara("result", result) = "success" Then
            Dim w As New WarnBox("上传成功！")
            w.Show()
            Me.Close()
        Else
            MsgBox(result)
        End If

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
End Class