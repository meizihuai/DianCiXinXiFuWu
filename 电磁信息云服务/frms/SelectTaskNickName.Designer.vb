<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SelectTaskNickName
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SelectTaskNickName))
        Me.LVTask = New System.Windows.Forms.ListView()
        Me.SuspendLayout()
        '
        'LVTask
        '
        Me.LVTask.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LVTask.Location = New System.Drawing.Point(0, 0)
        Me.LVTask.Name = "LVTask"
        Me.LVTask.Size = New System.Drawing.Size(760, 512)
        Me.LVTask.TabIndex = 1
        Me.LVTask.UseCompatibleStateImageBehavior = False
        '
        'SelectTaskNickName
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 17.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(760, 512)
        Me.Controls.Add(Me.LVTask)
        Me.Font = New System.Drawing.Font("微软雅黑", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "SelectTaskNickName"
        Me.Text = "SelectTaskNickName"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LVTask As System.Windows.Forms.ListView
End Class
