Public Class freqTaskDetail
    Public index As Integer
    Public Event beClick(ByVal _index As Integer)
    Sub New(ByVal _index As Integer)
        ' 此调用是设计器所必需的。
        InitializeComponent()
        index = _index
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub
    Private Sub freqTaskDetail_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Top
        Me.Height = 120

    End Sub

    Private Sub lickDetail_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lickDetail.LinkClicked
        RaiseEvent beClick(index)
    End Sub
End Class
