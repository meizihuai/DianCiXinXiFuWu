<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CbDeviceSelectHelper
    Inherits System.Windows.Forms.UserControl

    'UserControl 重写释放以清理组件列表。
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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LkUnSelectAll = New System.Windows.Forms.LinkLabel()
        Me.LkReverse = New System.Windows.Forms.LinkLabel()
        Me.LkSelectAll = New System.Windows.Forms.LinkLabel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.LkHuMen = New System.Windows.Forms.LinkLabel()
        Me.LKChangAn = New System.Windows.Forms.LinkLabel()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.LkUnSelectAll)
        Me.Panel1.Controls.Add(Me.LkReverse)
        Me.Panel1.Controls.Add(Me.LkSelectAll)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(35, 99)
        Me.Panel1.TabIndex = 3
        '
        'LkUnSelectAll
        '
        Me.LkUnSelectAll.AutoSize = True
        Me.LkUnSelectAll.Dock = System.Windows.Forms.DockStyle.Top
        Me.LkUnSelectAll.Location = New System.Drawing.Point(0, 54)
        Me.LkUnSelectAll.Name = "LkUnSelectAll"
        Me.LkUnSelectAll.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.LkUnSelectAll.Size = New System.Drawing.Size(32, 27)
        Me.LkUnSelectAll.TabIndex = 5
        Me.LkUnSelectAll.TabStop = True
        Me.LkUnSelectAll.Text = "不选"
        '
        'LkReverse
        '
        Me.LkReverse.AutoSize = True
        Me.LkReverse.Dock = System.Windows.Forms.DockStyle.Top
        Me.LkReverse.Location = New System.Drawing.Point(0, 27)
        Me.LkReverse.Name = "LkReverse"
        Me.LkReverse.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.LkReverse.Size = New System.Drawing.Size(32, 27)
        Me.LkReverse.TabIndex = 4
        Me.LkReverse.TabStop = True
        Me.LkReverse.Text = "反选"
        '
        'LkSelectAll
        '
        Me.LkSelectAll.AutoSize = True
        Me.LkSelectAll.Dock = System.Windows.Forms.DockStyle.Top
        Me.LkSelectAll.Location = New System.Drawing.Point(0, 0)
        Me.LkSelectAll.Name = "LkSelectAll"
        Me.LkSelectAll.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.LkSelectAll.Size = New System.Drawing.Size(32, 27)
        Me.LkSelectAll.TabIndex = 3
        Me.LkSelectAll.TabStop = True
        Me.LkSelectAll.Text = "全选"
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.LkHuMen)
        Me.Panel2.Controls.Add(Me.LKChangAn)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(35, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(37, 99)
        Me.Panel2.TabIndex = 4
        '
        'LkHuMen
        '
        Me.LkHuMen.AutoSize = True
        Me.LkHuMen.Dock = System.Windows.Forms.DockStyle.Top
        Me.LkHuMen.Location = New System.Drawing.Point(0, 27)
        Me.LkHuMen.Name = "LkHuMen"
        Me.LkHuMen.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.LkHuMen.Size = New System.Drawing.Size(32, 27)
        Me.LkHuMen.TabIndex = 4
        Me.LkHuMen.TabStop = True
        Me.LkHuMen.Text = "虎门"
        '
        'LKChangAn
        '
        Me.LKChangAn.AutoSize = True
        Me.LKChangAn.Dock = System.Windows.Forms.DockStyle.Top
        Me.LKChangAn.Location = New System.Drawing.Point(0, 0)
        Me.LKChangAn.Name = "LKChangAn"
        Me.LKChangAn.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.LKChangAn.Size = New System.Drawing.Size(32, 27)
        Me.LKChangAn.TabIndex = 3
        Me.LKChangAn.TabStop = True
        Me.LKChangAn.Text = "长安"
        '
        'CbDeviceSelectHelper
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("微软雅黑", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "CbDeviceSelectHelper"
        Me.Size = New System.Drawing.Size(72, 99)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents LkUnSelectAll As LinkLabel
    Friend WithEvents LkReverse As LinkLabel
    Friend WithEvents LkSelectAll As LinkLabel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents LkHuMen As LinkLabel
    Friend WithEvents LKChangAn As LinkLabel
End Class
