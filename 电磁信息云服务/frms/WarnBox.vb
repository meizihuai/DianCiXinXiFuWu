Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Public Class WarnBox
    Dim cTh As Thread
    Sub New(ByVal Text As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        Label1.Text = Text
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub WarnBox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        Me.TopMost = True
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Control.CheckForIllegalCrossThreadCalls = False
        run()
    End Sub
    Private Sub run()
        cTh = New Thread(AddressOf th_close)
        cTh.Start()
    End Sub
    Private Sub th_close()
        'Me.Text = "提示  " & 2 & "  秒后自动关闭提示窗……"
        'Sleep(1000)
        Me.Text = "提示  " & 1 & "  秒后自动关闭提示窗……"
        Sleep(1000)
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            cTh.Abort()
        Catch ex As Exception

        End Try
        Me.Close()
    End Sub
End Class