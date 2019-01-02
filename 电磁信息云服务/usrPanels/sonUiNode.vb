Public Class sonUiNode
    Public Event beClick(ByVal name As String)
    Sub New(ByVal title As String)
        ' 此调用是设计器所必需的。
        InitializeComponent()
        lblTitle.Text = title
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub
    Private Sub sonUiNode_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Height = 30
        lblTitle.Top = Me.Height / 2 - lblTitle.Height / 2
        Me.Dock = DockStyle.Top
        Me.Cursor = Cursors.Hand
        '31,89,137
        '53, 97, 155
    End Sub

    Private Sub lblTitle_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblTitle.MouseClick
        RaiseEvent beClick(lblTitle.Text)
    End Sub
    Private Sub Panel6_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Panel6.MouseClick
        RaiseEvent beClick(lblTitle.Text)
    End Sub

    Private Sub Panel6_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel6.MouseEnter
        Panel6.BackColor = Color.FromArgb(53, 97, 155)
    End Sub

    Private Sub Panel6_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel6.MouseLeave
        Panel6.BackColor = Color.FromArgb(31, 89, 137)
    End Sub
    Private Sub lblTitle_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblTitle.MouseEnter
        Panel6.BackColor = Color.FromArgb(53, 97, 155)
    End Sub

    Private Sub lblTitle_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblTitle.MouseLeave
        Panel6.BackColor = Color.FromArgb(31, 89, 137)
    End Sub
    Private Sub Panel6_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel6.Paint

    End Sub
End Class
