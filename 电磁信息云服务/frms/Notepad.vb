Public Class Notepad

    Private Sub Notepad_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        '   Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.TopMost = True
        Me.Text = "我的记事(Admin)"
        '  Me.Text = Panel1.Width & "," & Panel1.Height
    End Sub

   
End Class