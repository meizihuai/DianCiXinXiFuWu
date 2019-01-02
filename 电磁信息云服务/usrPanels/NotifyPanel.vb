Public Class NotifyPanel
    Public Event RaiseClose(ByVal p As NotifyPanel)
    Sub New(ByVal time As String, ByVal kind As String, ByVal content As String, ByVal linkid As String)
        ' 此调用是设计器所必需的。
        InitializeComponent()
        lblTime.Text = time
        lblKind.Text = kind
        lblContent.Text = content
        lblLink.Text = linkid
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub

    Private Sub NotifyPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Top
    End Sub

    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click
        RaiseEvent RaiseClose(Me)
    End Sub
End Class
