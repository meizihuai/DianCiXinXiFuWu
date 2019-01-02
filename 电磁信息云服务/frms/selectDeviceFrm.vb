Public Class selectDeviceFrm

    Private Sub selectDeviceFrm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "选择传感器"
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.TopMost = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        ini()
        iniDeviceList()
    End Sub
    Private Sub ini()
        LvDetail.View = View.Details
        LvDetail.GridLines = True
        LvDetail.FullRowSelect = True
        LvDetail.Columns.Add("序号", 45)
        LvDetail.Columns.Add("设备名称", 150)
        LvDetail.Columns.Add("设备类型", 80)
        LvDetail.Columns.Add("地点", 100)
        LvDetail.Columns.Add("是否在线", 80)
        LvDetail.Columns.Add("上线时间", 80)
        LvDetail.Columns.Add("经度", 80)
        LvDetail.Columns.Add("纬度", 80)
        LvDetail.Columns.Add("IP", 80)
        LvDetail.Columns.Add("Port", 80)
    End Sub
    Private Sub iniDeviceList()
        LvDetail.Visible = False
        LvDetail.Items.Clear()
        Form1.GetOnlineDevice()
        If IsNothing(alldevlist) Then Return
        For Each d In alldevlist
            If d.Kind = "TZBQ" Then
                Dim itm As New ListViewItem(LvDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSS" Then itm.ForeColor = Color.Green
                    If d.Kind = "TZBQ" Then itm.ForeColor = Color.Blue
                Next
                LvDetail.Items.Add(itm)
            End If    
        Next
        For Each d In alldevlist
            If d.Kind = "TSS" Then
                Dim itm As New ListViewItem(LvDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "TSS" Then
                    itm.SubItems(2).Text = "频谱传感器"
                End If
                If d.Kind = "TZBQ" Then
                    itm.SubItems(2).Text = "微型传感器"
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "TSS" Then itm.ForeColor = Color.Green
                    If d.Kind = "TZBQ" Then itm.ForeColor = Color.Blue
                Next
                LvDetail.Items.Add(itm)
            End If
        Next
        For Each d In alldevlist
            If d.Kind = "ING" Then
                Dim itm As New ListViewItem(LvDetail.Items.Count + 1)
                itm.SubItems.Add(d.Name)
                itm.SubItems.Add(d.Kind)
                itm.SubItems.Add(d.Address)
                itm.SubItems.Add("在线")
                itm.SubItems.Add(d.OnlineTime)
                itm.SubItems.Add(d.Lng)
                itm.SubItems.Add(d.Lat)
                If d.Kind = "ING" Then
                    itm.SubItems(2).Text = "监测网关"
                End If
                itm.SubItems.Add(d.IP)
                itm.SubItems.Add(d.Port)
                itm.UseItemStyleForSubItems = True
                For i = 0 To itm.SubItems.Count - 1
                    If d.Kind = "ING" Then itm.ForeColor = Color.FromArgb(139, 101, 8)
                Next
                LvDetail.Items.Add(itm)
            End If
        Next
        LvDetail.Visible = True
    End Sub

    Private Sub LvDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LvDetail.SelectedIndexChanged
        If LvDetail.SelectedItems.Count = 0 Then Return
        Dim itm As ListViewItem = LvDetail.SelectedItems(0)
        Dim lng As String = itm.SubItems(6).Text
        Dim lat As String = itm.SubItems(7).Text
        Dim selectDeviceID As String = itm.SubItems(1).Text
        Dim selectDeviceKind As String = itm.SubItems(2).Text
        Dim Kind As String = itm.SubItems(2).Text
        'Form1.isSelected = True
        'Form1.selectDeviceID = selectDeviceID
        'If Kind = "微型传感器" Then Form1.selectDeviceKind = "TZBQ"
        'If Kind = "频谱传感器" Then Form1.selectDeviceKind = "TSS"
        'Form1.Label2.Text = "选中  " & "微型传感器" & "  " & selectDeviceID
        Form1.SelectDevice(selectDeviceID)
        Me.Close()
    End Sub
End Class