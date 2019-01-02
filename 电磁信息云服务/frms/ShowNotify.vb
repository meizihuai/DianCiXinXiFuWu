Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Public Class ShowNotify

    Private Sub ShowNotify_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Text = "系统通知"
        Me.TopMost = True
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Control.CheckForIllegalCrossThreadCalls = False
        Dim th As New Thread(AddressOf ShowNotifyList)
        th.Start()
    End Sub
    Private Sub ShowNotifyList()
        If IsNothing(NotifyList) Then Return
        If IsNothing(NotifyLock) Then Return
        If NotifyList.Count = 0 Then Return
        Dim mList As List(Of notifyStu)
        SyncLock NotifyLock
            mList = NotifyList
        End SyncLock
        Me.Invoke(Sub()
                      PanelMain.Controls.Clear()
                      Try
                          For Each itm In mList
                              Dim p As New NotifyPanel(itm.Time, itm.Kind, itm.Content, itm.linkID)
                              AddHandler p.RaiseClose, AddressOf CLosePanel
                              PanelMain.Controls.Add(p)
                          Next
                      Catch ex As Exception

                      End Try

                  End Sub)
    End Sub
    Private Sub CLosePanel(ByVal p As NotifyPanel)

        RemoveNotify(p.lblTime.Text, p.lblContent.Text)
        PanelMain.Controls.Remove(p)
    End Sub

    Private Sub PictureBox4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox4.Click
        If IsNothing(NotifyList) Then Return
        If IsNothing(NotifyLock) Then Return
        SyncLock NotifyLock
            NotifyList.Clear()
        End SyncLock
        PanelMain.Controls.Clear()
        Me.Close()
    End Sub
End Class