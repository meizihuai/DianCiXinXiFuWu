Imports System
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports Newtonsoft
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Drawing
Imports System.Drawing.Drawing2D
Public Class HistoryImg

    Private Sub HistoryImg_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        Label2.Visible = False
        Label3.Visible = False
        Control.CheckForIllegalCrossThreadCalls = False
        ini()
        GetHistoryImageList()
    End Sub
    Private Sub ini()
        LVDetail.View = View.Details
        LVDetail.GridLines = True
        LVDetail.FullRowSelect = True
        LVDetail.Columns.Add("序号", 50)
        LVDetail.Columns.Add("截图名称", 150)
        LVDetail.Columns.Add("截图备注", 100)
        LVDetail.Columns.Add("截图用户", 100)
    End Sub
    Private Sub GetHistoryImageList()
        Label2.Visible = True
        Dim result As String = GetServerResult("func=GetHistoryImageList")
        Dim dt As DataTable = JsonConvert.DeserializeObject(result, GetType(DataTable))
        Label2.Visible = False
        If IsNothing(dt) Then Return
        LVDetail.Items.Clear()
        For Each row As DataRow In dt.Rows
            Dim imgName As String = row("imgName")
            Dim imgText As String = row("imgText")
            Dim userName As String = row("userName")
            Dim itm As New ListViewItem(LVDetail.Items.Count + 1)
            itm.SubItems.Add(imgName)
            itm.SubItems.Add(imgText)
            itm.SubItems.Add(userName)
            LVDetail.Items.Add(itm)
        Next
        If LVDetail.Items.Count > 0 Then
            lvdetailBeSelect(0)
        End If

    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        GetHistoryImageList()
    End Sub

    Private Sub LVDetail_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LVDetail.SelectedIndexChanged
        If LVDetail.SelectedIndices.Count = 0 Then Return
        lvdetailBeSelect(LVDetail.SelectedIndices(0))
    End Sub
    Private Sub lvdetailBeSelect(ByVal index As Integer)
        Dim th As New Thread(AddressOf th_showimg)
        th.Start(index)
       
    End Sub
    Private Sub th_showimg(ByVal index As Integer)
        Label3.Visible = True
        Dim itm As ListViewItem = LVDetail.Items(index)
        Dim imgName As String = itm.SubItems(1).Text
        Dim result As String = GetServerResult("func=GetImageBase64ByImgName&imgname=" & imgName)
        Label3.Visible = False
        Dim bitmap As Bitmap = data2img(result)
        Label3.Visible = False
        If IsNothing(bitmap) Then Return
        Me.Invoke(Sub() PictureBox2.Image = bitmap)
    End Sub
    Private Sub 删除ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 删除ToolStripMenuItem.Click
        If LVDetail.SelectedIndices.Count = 0 Then Return
        Dim itm As ListViewItem = LVDetail.SelectedItems(0)
        Dim imgName As String = itm.SubItems(1).Text
        If MsgBox("是否确认删除该任务记录?", MsgBoxStyle.YesNoCancel, "提示") <> MsgBoxResult.Yes Then
            Exit Sub
        End If
        Dim result As String = GetServerResult("func=DeleteImageBase64ByImgName&imgname=" & imgName)
        If GetResultPara("result", result) = "success" Then
            GetHistoryImageList()
            Dim w As New WarnBox("删除成功！")
            w.Show()
        Else
            MsgBox(result)
        End If
    End Sub
End Class
