<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fatherUiNode
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(fatherUiNode))
        Me.ico = New System.Windows.Forms.PictureBox()
        Me.lable = New System.Windows.Forms.Label()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel_Son = New System.Windows.Forms.Panel()
        CType(Me.ico, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ico
        '
        Me.ico.Image = CType(resources.GetObject("ico.Image"), System.Drawing.Image)
        Me.ico.Location = New System.Drawing.Point(8, 12)
        Me.ico.Name = "ico"
        Me.ico.Size = New System.Drawing.Size(15, 15)
        Me.ico.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.ico.TabIndex = 6
        Me.ico.TabStop = False
        '
        'lable
        '
        Me.lable.AutoSize = True
        Me.lable.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.lable.ForeColor = System.Drawing.Color.White
        Me.lable.Location = New System.Drawing.Point(31, 9)
        Me.lable.Name = "lable"
        Me.lable.Size = New System.Drawing.Size(107, 20)
        Me.lable.TabIndex = 7
        Me.lable.Text = "监测设备分布图"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "uiTreeCLose")
        Me.ImageList1.Images.SetKeyName(1, "uiTreeOpen")
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lable)
        Me.Panel1.Controls.Add(Me.ico)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(214, 32)
        Me.Panel1.TabIndex = 8
        '
        'Panel_Son
        '
        Me.Panel_Son.AutoSize = True
        Me.Panel_Son.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel_Son.Location = New System.Drawing.Point(0, 32)
        Me.Panel_Son.Name = "Panel_Son"
        Me.Panel_Son.Size = New System.Drawing.Size(214, 0)
        Me.Panel_Son.TabIndex = 9
        '
        'fatherUiNode
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(21, Byte), Integer), CType(CType(77, Byte), Integer), CType(CType(124, Byte), Integer))
        Me.Controls.Add(Me.Panel_Son)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "fatherUiNode"
        Me.Size = New System.Drawing.Size(214, 35)
        CType(Me.ico, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ico As System.Windows.Forms.PictureBox
    Friend WithEvents lable As System.Windows.Forms.Label
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel_Son As System.Windows.Forms.Panel

End Class
