Public Class FrmBlackRadio
    Private Sub FrmBlackRadio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        Me.Text = "黑广播预警列表"
        Init()
        IniLastBlackRadios()
    End Sub
    Private Sub Init()
        LV.View = View.Details
        LV.GridLines = True
        LV.FullRowSelect = True
        LV.Columns.Add("序号", 80)
        LV.Columns.Add("频谱(MHz)", 120)
        LV.Columns.Add("地区", 120)
        LV.Columns.Add("信号属性", 120)
        LV.Columns.Add("信号状态", 120)
        LV.Columns.Add("最大值(dBm)", 120)
        LV.Columns.Add("平均值(dBm)", 120)
    End Sub
    Private Sub IniLastBlackRadios()
        Dim itm As New ListViewItem(1)
        itm.SubItems.Add("103.3")
        itm.SubItems.Add("樟木头")
        itm.SubItems.Add("非法台站")
        itm.SubItems.Add("超标")
        itm.SubItems.Add("-60")
        itm.SubItems.Add("-78")
        LV.Items.Add(itm)

        itm = New ListViewItem(2)
        itm.SubItems.Add("104.5")
        itm.SubItems.Add("长安")
        itm.SubItems.Add("非法台站")
        itm.SubItems.Add("超标")
        itm.SubItems.Add("-45")
        itm.SubItems.Add("-63")
        LV.Items.Add(itm)

        itm = New ListViewItem(3)
        itm.SubItems.Add("105.5")
        itm.SubItems.Add("深圳")
        itm.SubItems.Add("非法台站")
        itm.SubItems.Add("超标")
        itm.SubItems.Add("-72")
        itm.SubItems.Add("-81")
        LV.Items.Add(itm)
    End Sub
End Class