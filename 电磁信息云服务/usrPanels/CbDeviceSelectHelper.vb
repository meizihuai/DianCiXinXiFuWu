Public Class CbDeviceSelectHelper
    Private _cbDevice As CheckedListBox
    Private isSetedCheckBox As Boolean = False
    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub CbDeviceSelectHelper_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Dock = DockStyle.Fill
    End Sub

    Public Property CbDevice As CheckedListBox
        Get
            Return _cbDevice
        End Get
        Set(value As CheckedListBox)
            isSetedCheckBox = True
            _cbDevice = value
        End Set
    End Property

    Private Sub LkSelectAll_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LkSelectAll.LinkClicked
        If Not isSetedCheckBox Then Return
        For i = 0 To CbDevice.Items.Count - 1
            CbDevice.SetItemChecked(i, True)
        Next
    End Sub

    Private Sub LkReverse_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LkReverse.LinkClicked
        If Not isSetedCheckBox Then Return
        For i = 0 To CbDevice.Items.Count - 1
            CbDevice.SetItemChecked(i, Not CbDevice.GetItemChecked(i))
        Next
    End Sub

    Private Sub LkUnSelectAll_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LkUnSelectAll.LinkClicked
        If Not isSetedCheckBox Then Return
        For i = 0 To CbDevice.Items.Count - 1
            CbDevice.SetItemChecked(i, False)
        Next
    End Sub


End Class
