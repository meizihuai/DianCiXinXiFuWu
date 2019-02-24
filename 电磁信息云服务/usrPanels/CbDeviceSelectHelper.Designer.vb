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
        Me.LkSelectAll = New System.Windows.Forms.LinkLabel()
        Me.LkReverse = New System.Windows.Forms.LinkLabel()
        Me.LkUnSelectAll = New System.Windows.Forms.LinkLabel()
        Me.SuspendLayout()
        '
        'LkSelectAll
        '
        Me.LkSelectAll.AutoSize = True
        Me.LkSelectAll.Dock = System.Windows.Forms.DockStyle.Top
        Me.LkSelectAll.Location = New System.Drawing.Point(0, 0)
        Me.LkSelectAll.Name = "LkSelectAll"
        Me.LkSelectAll.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.LkSelectAll.Size = New System.Drawing.Size(32, 27)
        Me.LkSelectAll.TabIndex = 0
        Me.LkSelectAll.TabStop = True
        Me.LkSelectAll.Text = "全选"
        '
        'LkReverse
        '
        Me.LkReverse.AutoSize = True
        Me.LkReverse.Dock = System.Windows.Forms.DockStyle.Top
        Me.LkReverse.Location = New System.Drawing.Point(0, 27)
        Me.LkReverse.Name = "LkReverse"
        Me.LkReverse.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.LkReverse.Size = New System.Drawing.Size(32, 27)
        Me.LkReverse.TabIndex = 1
        Me.LkReverse.TabStop = True
        Me.LkReverse.Text = "反选"
        '
        'LkUnSelectAll
        '
        Me.LkUnSelectAll.AutoSize = True
        Me.LkUnSelectAll.Dock = System.Windows.Forms.DockStyle.Top
        Me.LkUnSelectAll.Location = New System.Drawing.Point(0, 54)
        Me.LkUnSelectAll.Name = "LkUnSelectAll"
        Me.LkUnSelectAll.Padding = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.LkUnSelectAll.Size = New System.Drawing.Size(32, 27)
        Me.LkUnSelectAll.TabIndex = 2
        Me.LkUnSelectAll.TabStop = True
        Me.LkUnSelectAll.Text = "不选"
        '
        'CbDeviceSelectHelper
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.LkUnSelectAll)
        Me.Controls.Add(Me.LkReverse)
        Me.Controls.Add(Me.LkSelectAll)
        Me.Font = New System.Drawing.Font("微软雅黑", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "CbDeviceSelectHelper"
        Me.Size = New System.Drawing.Size(32, 92)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LkSelectAll As LinkLabel
    Friend WithEvents LkReverse As LinkLabel
    Friend WithEvents LkUnSelectAll As LinkLabel
End Class
