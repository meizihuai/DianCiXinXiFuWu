Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class usrManager
    Dim allUserDt As DataTable
    Private Sub usrManager_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        Label2.Visible = False
        Control.CheckForIllegalCrossThreadCalls = False
        ini()
        Dim th As New Thread(AddressOf GetAllUser)
        th.Start()
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 50)
        LVDetail.Columns.Add("用户名", 150)
        LVDetail.Columns.Add("类型", 150)
        LVDetail.Columns.Add("账号状态", 150)
    End Sub
    Private Sub GetAllUser()
        Label2.Visible = True
        Dim result As String = GetServerResult("func=GetAllUser")
        Label2.Visible = False
        If result = "[]" Then Return
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        If IsNothing(dt) Then Return
        allUserDt = dt
        LVDetail.Items.Clear()
        For Each row As DataRow In dt.Rows
            Dim susr As String = row("usr")
            Dim power As String = row("power")
            Dim status As String = row("status")
            If status = "-1" Then status = "审核未通过"
            If status = "0" Then status = "待审核"
            If status = "1" Then status = "已审核"
            If power = "1" Then power = "领导"
            If power = "2" Then power = "值班员"
            If power = "9" Then power = "管理员"
            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            itm.SubItems.Add(susr)
            itm.SubItems.Add(power)
            itm.SubItems.Add(status)
            itm.UseItemStyleForSubItems = False
            If status = "审核未通过" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Red
                Next
            End If
            If status = "待审核" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Gold
                Next
            End If
            LVDetail.Items.Add(itm)
        Next
        If LVDetail.Items.Count > 0 Then
            lvBeSelect(0)
        End If
    End Sub
    Private Sub ShowUser(ByVal kStatus As String)
        LVDetail.Items.Clear()
        If IsNothing(allUserDt) Then Return
        For Each row As DataRow In allUserDt.Rows
            Dim susr As String = row("usr")
            Dim power As String = row("power")
            Dim status As String = row("status")
            If status = "-1" Then status = "审核未通过"
            If status = "0" Then status = "待审核"
            If status = "1" Then status = "已审核"
            If power = "1" Then power = "领导"
            If power = "2" Then power = "值班员"
            If power = "9" Then power = "管理员"
            If kStatus <> "All" Then
                If kStatus <> status Then Continue For
            End If
            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            itm.SubItems.Add(susr)
            itm.SubItems.Add(power)
            itm.SubItems.Add(status)
            itm.UseItemStyleForSubItems = False
            If status = "审核未通过" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Red
                Next
            End If
            If status = "待审核" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Gold
                Next
            End If
            LVDetail.Items.Add(itm)
        Next
        If LVDetail.Items.Count > 0 Then
            lvBeSelect(0)
        End If
    End Sub
    Private Sub ShowUserByPower(ByVal kPower As String)
        LVDetail.Items.Clear()
        If IsNothing(allUserDt) Then Return
        For Each row As DataRow In allUserDt.Rows
            Dim susr As String = row("usr")
            Dim power As String = row("power")
            Dim status As String = row("status")
            If status = "-1" Then status = "审核未通过"
            If status = "0" Then status = "待审核"
            If status = "1" Then status = "已审核"
            If power = "1" Then power = "领导"
            If power = "2" Then power = "值班员"
            If power = "9" Then power = "管理员"
            If kPower <> "All" Then
                If kPower <> power Then Continue For
            End If
            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            itm.SubItems.Add(susr)
            itm.SubItems.Add(power)
            itm.SubItems.Add(status)
            itm.UseItemStyleForSubItems = False
            If status = "审核未通过" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Red
                Next
            End If
            If status = "待审核" Then
                For i = 0 To LVDetail.Columns.Count - 1
                    itm.SubItems(i).BackColor = Color.Gold
                Next
            End If
            LVDetail.Items.Add(itm)
        Next
        If LVDetail.Items.Count > 0 Then
            lvBeSelect(0)
        End If
    End Sub
    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetAllUser()
    End Sub
    Private Sub lvBeSelect(ByVal index As Integer)
        Dim itm As ListViewItem = LVDetail.Items(index)
        Dim susr As String = itm.SubItems(1).Text
        Dim power As String = itm.SubItems(2).Text
        Dim status As String = itm.SubItems(3).Text
        lblusr.Text = susr
        lblPower.Text = power
        lblStatus.Text = status
        Dim powerString As String
        Dim sb As New StringBuilder
        If power = "管理员" Then
            sb.AppendLine("1.审核领导账号")
            sb.AppendLine("2.维护系统数据")
            sb.AppendLine("3.调配系统资源")
        End If
        If power = "领导" Then
            sb.AppendLine("1.审核值班员账号")
            sb.AppendLine("2.下达值班任务")
            sb.AppendLine("3.管理值班进程")
            RadioButton5.Checked = True
        End If
        If power = "值班员" Then
            sb.AppendLine("1.接受值班任务")
            sb.AppendLine("2.下发传感器任务")
            sb.AppendLine("3.管理传感器状态")
            sb.AppendLine("4.处理预警事件")
            sb.AppendLine("5.管理信息报告")
            RadioButton4.Checked = True
        End If
        If status = "审核未通过" Then
            RadioButton6.Checked = True
        Else
            RadioButton7.Checked = True
        End If
        powerString = sb.ToString
        lblPowerString.Text = powerString
    End Sub
    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedIndices.Count > 0 Then
            lvBeSelect(LVDetail.SelectedIndices(0))
        End If
    End Sub

    Private Sub RBWait_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RBWait.CheckedChanged
        ShowUser("待审核")
    End Sub

    Private Sub RBDone_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RBDone.CheckedChanged
        ShowUser("已审核")
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        ShowUserByPower("领导")
    End Sub

    Private Sub RB2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RB2.CheckedChanged
        ShowUserByPower("值班员")
    End Sub

    Private Sub RBALL_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RBALL.CheckedChanged
        ShowUserByPower("All")
    End Sub

    Private Sub Panel7_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)

    End Sub

    Private Sub Label10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label10.Click
        btn_Confirm_Click()
    End Sub

    Private Sub Panel34_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel34.Click
        btn_Confirm_Click()
    End Sub
    Private Sub btn_Confirm_Click()
        Dim sUsr As String = lblusr.Text
        Dim power As Integer = 1
        If RadioButton4.Checked Then power = 2
        Dim status As Integer = 1
        If RadioButton6.Checked Then status = -1
        Dim dik As New Dictionary(Of String, String)
        dik.Add("func", "changeUsrInfo")
        dik.Add("usr", sUsr)
        dik.Add("power", power)
        dik.Add("status", status)
        Dim str As String = TransforPara2Query(dik)
        Dim result As String = GetServerResult(str)
        Dim r As String = GetNorResult("result", result)
        Dim msg As String = GetNorResult("msg", result)
        Dim errsg As String = GetNorResult("errsg", result)
        If r = "success" Then            
            Dim w As New WarnBox("更改成功!")
            w.Show()
            Dim th As New Thread(AddressOf GetAllUser)
            th.Start()
        Else
            Dim sb As New StringBuilder
            sb.AppendLine("更改失败")
            sb.AppendLine(msg)
            sb.AppendLine(errsg)
            MsgBox(sb.ToString)
        End If
    End Sub
    
End Class
