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
Public Class fatherUiNode
    Private isExpand As Boolean = False
    Public Event beClick(ByVal name As String)
    Private Title As String
    Public Event OnUiOpen(ByVal name As String)
    Sub New(ByVal _Title As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()
        Title = _Title
        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Public Sub SetSons(ByVal sonsTree As uiTree)
        If IsNothing(sonsTree) Then
            Exit Sub
        End If
        If IsNothing(sonsTree.NodeList) Then
            Panel_Son.Controls.Clear()
            Exit Sub
        End If
        Panel_Son.Controls.Clear()
        For j = sonsTree.NodeList.Count - 1 To 0 Step -1
            Dim itm As uiTree = sonsTree.NodeList(j)
            Dim name As String = itm.name
            Dim p As New sonUiNode(name)
            Panel_Son.Controls.Add(p)
            p.Dock = DockStyle.Top
            AddHandler p.beClick, AddressOf p_beClick
        Next
        If isExpand Then
            uiOpen()
        Else
            uiClose()
        End If
    End Sub
    Private Sub p_beClick(ByVal name As String)
        name = Title & "," & name
        RaiseEvent beClick(name)
    End Sub
    Private Sub fatherUiNode_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ico.Image = uiCloseImg
        ico.SizeMode = PictureBoxSizeMode.StretchImage
        ico.Width = 15
        ico.Height = 15
        ico.Left = 14
        ico.Left = 7
        Panel_Son.AutoSize = True
        Me.AutoSize = True
        Me.Dock = DockStyle.Top
        Me.Cursor = Cursors.Hand
    End Sub
    Public Sub uiOpen()
        ico.Image = uiOpenImg
        isExpand = True
        Panel_Son.Visible = True
        RaiseEvent OnUiOpen(Title)
    End Sub
    Public Sub uiClose()
        ico.Image = uiCloseImg
        isExpand = False
        Panel_Son.Visible = False
    End Sub

    Private Sub Panel1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Panel1.Click
        If isExpand Then
            uiClose()
        Else
            uiOpen()
        End If
        'RaiseEvent beClick()
    End Sub
    Private Sub lable_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lable.Click
        If isExpand Then
            uiClose()
        Else
            uiOpen()
        End If
        'RaiseEvent beClick()
    End Sub

  
End Class
