Public Class QuickFreq
    Dim selectOnlineFreq As OnlineFreq
    Dim selectOnlineSigNal As OnlineSignal
    Dim selectCarFreqGis As CarFreqGis
    Sub New(ByVal _selectOnlineFreq As OnlineFreq)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        selectOnlineFreq = _selectOnlineFreq
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Sub New(ByVal _selectOnlineSigNal As OnlineSignal)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        selectOnlineSigNal = _selectOnlineSigNal
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Sub New(ByVal _selectCarFreqGis As CarFreqGis)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        selectCarFreqGis = _selectCarFreqGis
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub QuickFreq_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '  Return
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Me.Left = Form1.Left + Form1.Width / 2 - Me.Width / 2
        Me.Top = Form1.Top + Form1.Height / 2 - Me.Height / 2
        Me.TopMost = True
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 50)
        LVDetail.Columns.Add("名称", 150)
        LVDetail.Columns.Add("起始频率(MHz)", 100)
        LVDetail.Columns.Add("终止频率(MHz)", 100)
        Dim itm As ListViewItem
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("短波")
        itm.SubItems.Add("1.5")
        itm.SubItems.Add("30")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("调频广播")
        itm.SubItems.Add("88")
        itm.SubItems.Add("108")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("民航通信")
        itm.SubItems.Add("108")
        itm.SubItems.Add("137")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("水上通信")
        itm.SubItems.Add("156")
        itm.SubItems.Add("172")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("自定义1")
        itm.SubItems.Add("400")
        itm.SubItems.Add("500")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("自定义2")
        itm.SubItems.Add("400")
        itm.SubItems.Add("2000")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("自定义2")
        itm.SubItems.Add("400")
        itm.SubItems.Add("2000")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("半段")
        itm.SubItems.Add("30")
        itm.SubItems.Add("3000")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("全段")
        itm.SubItems.Add("30")
        itm.SubItems.Add("6000")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("铁路GSM")
        itm.SubItems.Add("885")
        itm.SubItems.Add("935")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("铁路GSM")
        itm.SubItems.Add("885")
        itm.SubItems.Add("935")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国移动EGSM")
        itm.SubItems.Add("890")
        itm.SubItems.Add("954")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国移动GSM")
        itm.SubItems.Add("890")
        itm.SubItems.Add("954")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国移动DSC1800")
        itm.SubItems.Add("1710")
        itm.SubItems.Add("1820")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国移动TD-SCDMA")
        itm.SubItems.Add("1880")
        itm.SubItems.Add("2025")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国移动TD-LTE")
        itm.SubItems.Add("2300")
        itm.SubItems.Add("2400")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国联通GSM")
        itm.SubItems.Add("909")
        itm.SubItems.Add("960")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国联通DSC-1800")
        itm.SubItems.Add("1740")
        itm.SubItems.Add("1850")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国联通WCDMA")
        itm.SubItems.Add("1940")
        itm.SubItems.Add("2145")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国电信CDMA")
        itm.SubItems.Add("825")
        itm.SubItems.Add("880")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国电信CDMA2000")
        itm.SubItems.Add("1920")
        itm.SubItems.Add("2125")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国电信FDD-LTE")
        itm.SubItems.Add("1765")
        itm.SubItems.Add("1865")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("中国电信TD-LTE")
        itm.SubItems.Add("2370")
        itm.SubItems.Add("2390")
        LVDetail.Items.Add(itm)
        itm = New ListViewItem(LVDetail.Items.Count + 1)
        itm.SubItems.Add("WiFi")
        itm.SubItems.Add("2400")
        itm.SubItems.Add("2483")
        LVDetail.Items.Add(itm)

    End Sub

    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedItems.Count = 0 Then
            Return
        End If
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim freqStart As String = itm.SubItems(2).Text
        Dim freqEnd As String = itm.SubItems(3).Text
        If IsNothing(selectOnlineFreq) = False Then
            selectOnlineFreq.TextBox1.Text = freqStart
            selectOnlineFreq.TextBox2.Text = freqEnd
        End If
        If IsNothing(selectOnlineSigNal) = False Then

            selectOnlineSigNal.TextBox1.Text = freqStart
            selectOnlineSigNal.TextBox2.Text = freqEnd
        End If
        If IsNothing(selectCarFreqGis) = False Then

            selectCarFreqGis.TextBox1.Text = freqStart
            selectCarFreqGis.TextBox2.Text = freqEnd
        End If
        Me.Close()
    End Sub
End Class