<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DeviceAllMap
    Inherits System.Windows.Forms.UserControl

    'UserControl 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DeviceAllMap))
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LVDetail = New System.Windows.Forms.ListView()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.选中该设备并进入设备功能地图ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.lblTSSTotalNum = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.lblTSSErrPer = New System.Windows.Forms.Label()
        Me.lblTssDisOnlinePer = New System.Windows.Forms.Label()
        Me.lblTSSOnlinePer = New System.Windows.Forms.Label()
        Me.lblTSSErrNum = New System.Windows.Forms.Label()
        Me.lblTSSDisOnlineNum = New System.Windows.Forms.Label()
        Me.lblTSSOnlineNum = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.lblTZBQTotalNum = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.lblTZBQErrPer = New System.Windows.Forms.Label()
        Me.lblTZBQDisOnlinePer = New System.Windows.Forms.Label()
        Me.lblTZBQOnlinePer = New System.Windows.Forms.Label()
        Me.lblTZBQErrNum = New System.Windows.Forms.Label()
        Me.lblTZBQDisOnlineNum = New System.Windows.Forms.Label()
        Me.lblTZBQOnlineNum = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ShapeContainer1 = New Microsoft.VisualBasic.PowerPacks.ShapeContainer()
        Me.LineShape1 = New Microsoft.VisualBasic.PowerPacks.LineShape()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.WebGis = New System.Windows.Forms.WebBrowser()
        Me.Panel20 = New System.Windows.Forms.Panel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Panel21 = New System.Windows.Forms.Panel()
        Me.Panel4.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel6.SuspendLayout()
        Me.Panel7.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel8.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel20.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.Panel1)
        Me.Panel4.Controls.Add(Me.Panel3)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel4.Location = New System.Drawing.Point(5, 5)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(301, 579)
        Me.Panel4.TabIndex = 9
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.LVDetail)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 51)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(301, 528)
        Me.Panel1.TabIndex = 8
        '
        'LVDetail
        '
        Me.LVDetail.ContextMenuStrip = Me.ContextMenuStrip1
        Me.LVDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVDetail.Location = New System.Drawing.Point(0, 0)
        Me.LVDetail.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.LVDetail.Name = "LVDetail"
        Me.LVDetail.Size = New System.Drawing.Size(301, 528)
        Me.LVDetail.TabIndex = 1
        Me.LVDetail.UseCompatibleStateImageBehavior = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.选中该设备并进入设备功能地图ToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(248, 26)
        '
        '选中该设备并进入设备功能地图ToolStripMenuItem
        '
        Me.选中该设备并进入设备功能地图ToolStripMenuItem.Name = "选中该设备并进入设备功能地图ToolStripMenuItem"
        Me.选中该设备并进入设备功能地图ToolStripMenuItem.Size = New System.Drawing.Size(247, 22)
        Me.选中该设备并进入设备功能地图ToolStripMenuItem.Text = "选中该设备,并进入设备功能地图"
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(57, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(73, Byte), Integer))
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.PictureBox3)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(301, 51)
        Me.Panel3.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(190, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(73, 20)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "获取中……"
        '
        'PictureBox3
        '
        Me.PictureBox3.Cursor = System.Windows.Forms.Cursors.Hand
        Me.PictureBox3.Image = CType(resources.GetObject("PictureBox3.Image"), System.Drawing.Image)
        Me.PictureBox3.Location = New System.Drawing.Point(270, 13)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Size = New System.Drawing.Size(25, 25)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox3.TabIndex = 12
        Me.PictureBox3.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(3, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(107, 20)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "云服务设备列表"
        '
        'Panel6
        '
        Me.Panel6.Controls.Add(Me.Panel7)
        Me.Panel6.Controls.Add(Me.Panel8)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel6.Location = New System.Drawing.Point(879, 5)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Padding = New System.Windows.Forms.Padding(5, 0, 0, 0)
        Me.Panel6.Size = New System.Drawing.Size(239, 579)
        Me.Panel6.TabIndex = 13
        '
        'Panel7
        '
        Me.Panel7.BackColor = System.Drawing.Color.White
        Me.Panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel7.Controls.Add(Me.lblTSSTotalNum)
        Me.Panel7.Controls.Add(Me.Label19)
        Me.Panel7.Controls.Add(Me.lblTSSErrPer)
        Me.Panel7.Controls.Add(Me.lblTssDisOnlinePer)
        Me.Panel7.Controls.Add(Me.lblTSSOnlinePer)
        Me.Panel7.Controls.Add(Me.lblTSSErrNum)
        Me.Panel7.Controls.Add(Me.lblTSSDisOnlineNum)
        Me.Panel7.Controls.Add(Me.lblTSSOnlineNum)
        Me.Panel7.Controls.Add(Me.Label26)
        Me.Panel7.Controls.Add(Me.Label27)
        Me.Panel7.Controls.Add(Me.Label28)
        Me.Panel7.Controls.Add(Me.lblTZBQTotalNum)
        Me.Panel7.Controls.Add(Me.Label17)
        Me.Panel7.Controls.Add(Me.lblTZBQErrPer)
        Me.Panel7.Controls.Add(Me.lblTZBQDisOnlinePer)
        Me.Panel7.Controls.Add(Me.lblTZBQOnlinePer)
        Me.Panel7.Controls.Add(Me.lblTZBQErrNum)
        Me.Panel7.Controls.Add(Me.lblTZBQDisOnlineNum)
        Me.Panel7.Controls.Add(Me.lblTZBQOnlineNum)
        Me.Panel7.Controls.Add(Me.Label9)
        Me.Panel7.Controls.Add(Me.Label8)
        Me.Panel7.Controls.Add(Me.Label6)
        Me.Panel7.Controls.Add(Me.Label5)
        Me.Panel7.Controls.Add(Me.Label3)
        Me.Panel7.Controls.Add(Me.PictureBox2)
        Me.Panel7.Controls.Add(Me.PictureBox1)
        Me.Panel7.Controls.Add(Me.ShapeContainer1)
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel7.Location = New System.Drawing.Point(5, 51)
        Me.Panel7.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(234, 528)
        Me.Panel7.TabIndex = 8
        '
        'lblTSSTotalNum
        '
        Me.lblTSSTotalNum.AutoSize = True
        Me.lblTSSTotalNum.Location = New System.Drawing.Point(125, 295)
        Me.lblTSSTotalNum.Name = "lblTSSTotalNum"
        Me.lblTSSTotalNum.Size = New System.Drawing.Size(15, 17)
        Me.lblTSSTotalNum.TabIndex = 38
        Me.lblTSSTotalNum.Text = "0"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label19.Location = New System.Drawing.Point(83, 293)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(37, 20)
        Me.Label19.TabIndex = 37
        Me.Label19.Text = "共计"
        '
        'lblTSSErrPer
        '
        Me.lblTSSErrPer.AutoSize = True
        Me.lblTSSErrPer.Location = New System.Drawing.Point(157, 268)
        Me.lblTSSErrPer.Name = "lblTSSErrPer"
        Me.lblTSSErrPer.Size = New System.Drawing.Size(26, 17)
        Me.lblTSSErrPer.TabIndex = 36
        Me.lblTSSErrPer.Text = "0%"
        '
        'lblTssDisOnlinePer
        '
        Me.lblTssDisOnlinePer.AutoSize = True
        Me.lblTssDisOnlinePer.Location = New System.Drawing.Point(157, 241)
        Me.lblTssDisOnlinePer.Name = "lblTssDisOnlinePer"
        Me.lblTssDisOnlinePer.Size = New System.Drawing.Size(26, 17)
        Me.lblTssDisOnlinePer.TabIndex = 35
        Me.lblTssDisOnlinePer.Text = "0%"
        '
        'lblTSSOnlinePer
        '
        Me.lblTSSOnlinePer.AutoSize = True
        Me.lblTSSOnlinePer.Location = New System.Drawing.Point(157, 214)
        Me.lblTSSOnlinePer.Name = "lblTSSOnlinePer"
        Me.lblTSSOnlinePer.Size = New System.Drawing.Size(26, 17)
        Me.lblTSSOnlinePer.TabIndex = 34
        Me.lblTSSOnlinePer.Text = "0%"
        '
        'lblTSSErrNum
        '
        Me.lblTSSErrNum.AutoSize = True
        Me.lblTSSErrNum.Location = New System.Drawing.Point(125, 268)
        Me.lblTSSErrNum.Name = "lblTSSErrNum"
        Me.lblTSSErrNum.Size = New System.Drawing.Size(15, 17)
        Me.lblTSSErrNum.TabIndex = 33
        Me.lblTSSErrNum.Text = "0"
        '
        'lblTSSDisOnlineNum
        '
        Me.lblTSSDisOnlineNum.AutoSize = True
        Me.lblTSSDisOnlineNum.Location = New System.Drawing.Point(125, 241)
        Me.lblTSSDisOnlineNum.Name = "lblTSSDisOnlineNum"
        Me.lblTSSDisOnlineNum.Size = New System.Drawing.Size(15, 17)
        Me.lblTSSDisOnlineNum.TabIndex = 32
        Me.lblTSSDisOnlineNum.Text = "0"
        '
        'lblTSSOnlineNum
        '
        Me.lblTSSOnlineNum.AutoSize = True
        Me.lblTSSOnlineNum.Location = New System.Drawing.Point(125, 214)
        Me.lblTSSOnlineNum.Name = "lblTSSOnlineNum"
        Me.lblTSSOnlineNum.Size = New System.Drawing.Size(15, 17)
        Me.lblTSSOnlineNum.TabIndex = 31
        Me.lblTSSOnlineNum.Text = "0"
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label26.Location = New System.Drawing.Point(83, 266)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(37, 20)
        Me.Label26.TabIndex = 30
        Me.Label26.Text = "故障"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label27.Location = New System.Drawing.Point(83, 239)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(37, 20)
        Me.Label27.TabIndex = 29
        Me.Label27.Text = "离线"
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label28.Location = New System.Drawing.Point(83, 212)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(37, 20)
        Me.Label28.TabIndex = 28
        Me.Label28.Text = "在线"
        '
        'lblTZBQTotalNum
        '
        Me.lblTZBQTotalNum.AutoSize = True
        Me.lblTZBQTotalNum.Location = New System.Drawing.Point(125, 121)
        Me.lblTZBQTotalNum.Name = "lblTZBQTotalNum"
        Me.lblTZBQTotalNum.Size = New System.Drawing.Size(15, 17)
        Me.lblTZBQTotalNum.TabIndex = 27
        Me.lblTZBQTotalNum.Text = "0"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label17.Location = New System.Drawing.Point(83, 119)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(37, 20)
        Me.Label17.TabIndex = 26
        Me.Label17.Text = "共计"
        '
        'lblTZBQErrPer
        '
        Me.lblTZBQErrPer.AutoSize = True
        Me.lblTZBQErrPer.Location = New System.Drawing.Point(157, 94)
        Me.lblTZBQErrPer.Name = "lblTZBQErrPer"
        Me.lblTZBQErrPer.Size = New System.Drawing.Size(26, 17)
        Me.lblTZBQErrPer.TabIndex = 25
        Me.lblTZBQErrPer.Text = "0%"
        '
        'lblTZBQDisOnlinePer
        '
        Me.lblTZBQDisOnlinePer.AutoSize = True
        Me.lblTZBQDisOnlinePer.Location = New System.Drawing.Point(157, 67)
        Me.lblTZBQDisOnlinePer.Name = "lblTZBQDisOnlinePer"
        Me.lblTZBQDisOnlinePer.Size = New System.Drawing.Size(26, 17)
        Me.lblTZBQDisOnlinePer.TabIndex = 24
        Me.lblTZBQDisOnlinePer.Text = "0%"
        '
        'lblTZBQOnlinePer
        '
        Me.lblTZBQOnlinePer.AutoSize = True
        Me.lblTZBQOnlinePer.Location = New System.Drawing.Point(157, 40)
        Me.lblTZBQOnlinePer.Name = "lblTZBQOnlinePer"
        Me.lblTZBQOnlinePer.Size = New System.Drawing.Size(26, 17)
        Me.lblTZBQOnlinePer.TabIndex = 23
        Me.lblTZBQOnlinePer.Text = "0%"
        '
        'lblTZBQErrNum
        '
        Me.lblTZBQErrNum.AutoSize = True
        Me.lblTZBQErrNum.Location = New System.Drawing.Point(125, 94)
        Me.lblTZBQErrNum.Name = "lblTZBQErrNum"
        Me.lblTZBQErrNum.Size = New System.Drawing.Size(15, 17)
        Me.lblTZBQErrNum.TabIndex = 22
        Me.lblTZBQErrNum.Text = "0"
        '
        'lblTZBQDisOnlineNum
        '
        Me.lblTZBQDisOnlineNum.AutoSize = True
        Me.lblTZBQDisOnlineNum.Location = New System.Drawing.Point(125, 67)
        Me.lblTZBQDisOnlineNum.Name = "lblTZBQDisOnlineNum"
        Me.lblTZBQDisOnlineNum.Size = New System.Drawing.Size(15, 17)
        Me.lblTZBQDisOnlineNum.TabIndex = 21
        Me.lblTZBQDisOnlineNum.Text = "0"
        '
        'lblTZBQOnlineNum
        '
        Me.lblTZBQOnlineNum.AutoSize = True
        Me.lblTZBQOnlineNum.Location = New System.Drawing.Point(125, 40)
        Me.lblTZBQOnlineNum.Name = "lblTZBQOnlineNum"
        Me.lblTZBQOnlineNum.Size = New System.Drawing.Size(15, 17)
        Me.lblTZBQOnlineNum.TabIndex = 20
        Me.lblTZBQOnlineNum.Text = "0"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label9.Location = New System.Drawing.Point(83, 92)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(37, 20)
        Me.Label9.TabIndex = 19
        Me.Label9.Text = "故障"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label8.Location = New System.Drawing.Point(83, 65)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(37, 20)
        Me.Label8.TabIndex = 18
        Me.Label8.Text = "离线"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label6.Location = New System.Drawing.Point(83, 38)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(37, 20)
        Me.Label6.TabIndex = 17
        Me.Label6.Text = "在线"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label5.Location = New System.Drawing.Point(56, 176)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(79, 20)
        Me.Label5.TabIndex = 16
        Me.Label5.Text = "频谱传感器"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label3.Location = New System.Drawing.Point(56, 6)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(79, 20)
        Me.Label3.TabIndex = 15
        Me.Label3.Text = "微型传感器"
        '
        'PictureBox2
        '
        Me.PictureBox2.Cursor = System.Windows.Forms.Cursors.Hand
        Me.PictureBox2.Image = Global.电磁信息云服务.My.Resources.Resources.tss
        Me.PictureBox2.Location = New System.Drawing.Point(7, 176)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(27, 38)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox2.TabIndex = 14
        Me.PictureBox2.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.PictureBox1.Image = Global.电磁信息云服务.My.Resources.Resources.tzbq
        Me.PictureBox1.Location = New System.Drawing.Point(7, 6)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(27, 38)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 13
        Me.PictureBox1.TabStop = False
        '
        'ShapeContainer1
        '
        Me.ShapeContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ShapeContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.ShapeContainer1.Name = "ShapeContainer1"
        Me.ShapeContainer1.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.LineShape1})
        Me.ShapeContainer1.Size = New System.Drawing.Size(232, 526)
        Me.ShapeContainer1.TabIndex = 39
        Me.ShapeContainer1.TabStop = False
        '
        'LineShape1
        '
        Me.LineShape1.BorderColor = System.Drawing.SystemColors.ControlDark
        Me.LineShape1.Name = "LineShape1"
        Me.LineShape1.X1 = 19
        Me.LineShape1.X2 = 212
        Me.LineShape1.Y1 = 159
        Me.LineShape1.Y2 = 159
        '
        'Panel8
        '
        Me.Panel8.BackColor = System.Drawing.Color.FromArgb(CType(CType(57, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(73, Byte), Integer))
        Me.Panel8.Controls.Add(Me.Label4)
        Me.Panel8.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel8.Location = New System.Drawing.Point(5, 0)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Size = New System.Drawing.Size(234, 51)
        Me.Panel8.TabIndex = 7
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(3, 15)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(135, 20)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "云服务设施状态管理"
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.Panel2)
        Me.Panel5.Controls.Add(Me.Panel20)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel5.Location = New System.Drawing.Point(306, 5)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Padding = New System.Windows.Forms.Padding(5, 0, 0, 0)
        Me.Panel5.Size = New System.Drawing.Size(573, 579)
        Me.Panel5.TabIndex = 14
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.WebGis)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(5, 51)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(568, 528)
        Me.Panel2.TabIndex = 13
        '
        'WebGis
        '
        Me.WebGis.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebGis.Location = New System.Drawing.Point(0, 0)
        Me.WebGis.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.WebGis.MinimumSize = New System.Drawing.Size(23, 28)
        Me.WebGis.Name = "WebGis"
        Me.WebGis.Size = New System.Drawing.Size(568, 528)
        Me.WebGis.TabIndex = 0
        '
        'Panel20
        '
        Me.Panel20.BackColor = System.Drawing.Color.FromArgb(CType(CType(57, Byte), Integer), CType(CType(61, Byte), Integer), CType(CType(73, Byte), Integer))
        Me.Panel20.Controls.Add(Me.Label7)
        Me.Panel20.Controls.Add(Me.Panel21)
        Me.Panel20.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel20.Location = New System.Drawing.Point(5, 0)
        Me.Panel20.Name = "Panel20"
        Me.Panel20.Size = New System.Drawing.Size(568, 51)
        Me.Panel20.TabIndex = 9
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Location = New System.Drawing.Point(3, 16)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(107, 20)
        Me.Label7.TabIndex = 10
        Me.Label7.Text = "云服务设施分布"
        '
        'Panel21
        '
        Me.Panel21.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel21.Location = New System.Drawing.Point(407, 0)
        Me.Panel21.Name = "Panel21"
        Me.Panel21.Size = New System.Drawing.Size(161, 51)
        Me.Panel21.TabIndex = 8
        '
        'DeviceAllMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Panel5)
        Me.Controls.Add(Me.Panel6)
        Me.Controls.Add(Me.Panel4)
        Me.Font = New System.Drawing.Font("微软雅黑", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "DeviceAllMap"
        Me.Padding = New System.Windows.Forms.Padding(5)
        Me.Size = New System.Drawing.Size(1123, 589)
        Me.Panel4.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel6.ResumeLayout(False)
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel8.ResumeLayout(False)
        Me.Panel8.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel20.ResumeLayout(False)
        Me.Panel20.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents LVDetail As System.Windows.Forms.ListView
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents PictureBox3 As System.Windows.Forms.PictureBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents Panel8 As System.Windows.Forms.Panel
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents WebGis As System.Windows.Forms.WebBrowser
    Friend WithEvents Panel20 As System.Windows.Forms.Panel
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Panel21 As System.Windows.Forms.Panel
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblTSSTotalNum As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents lblTSSErrPer As System.Windows.Forms.Label
    Friend WithEvents lblTssDisOnlinePer As System.Windows.Forms.Label
    Friend WithEvents lblTSSOnlinePer As System.Windows.Forms.Label
    Friend WithEvents lblTSSErrNum As System.Windows.Forms.Label
    Friend WithEvents lblTSSDisOnlineNum As System.Windows.Forms.Label
    Friend WithEvents lblTSSOnlineNum As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents lblTZBQTotalNum As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents lblTZBQErrPer As System.Windows.Forms.Label
    Friend WithEvents lblTZBQDisOnlinePer As System.Windows.Forms.Label
    Friend WithEvents lblTZBQOnlinePer As System.Windows.Forms.Label
    Friend WithEvents lblTZBQErrNum As System.Windows.Forms.Label
    Friend WithEvents lblTZBQDisOnlineNum As System.Windows.Forms.Label
    Friend WithEvents lblTZBQOnlineNum As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents ShapeContainer1 As Microsoft.VisualBasic.PowerPacks.ShapeContainer
    Friend WithEvents LineShape1 As Microsoft.VisualBasic.PowerPacks.LineShape
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents 选中该设备并进入设备功能地图ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
