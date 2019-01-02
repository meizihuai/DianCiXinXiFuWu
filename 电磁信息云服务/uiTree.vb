Public Class uiTree
    Public name As String
    Public NodeList As List(Of uiTree)
    Public pfather As fatherUiNode
    Public Event OnFatherUiNodeUiOpen(ByVal name As String)
    Public Sub New(ByVal _Name As String)
        name = _Name
        pfather = New fatherUiNode(name)
        pfather.lable.Text = name
        pfather.Dock = DockStyle.Top
        pfather.Cursor = Cursors.Hand
        AddHandler pfather.OnUiOpen, AddressOf On_FatherUiNodeUiOpen
    End Sub
    Private Sub On_FatherUiNodeUiOpen(ByVal name As String)
        RaiseEvent OnFatherUiNodeUiOpen(name)
    End Sub
    Public Sub AddNode(ByVal _Name As String)
        If IsNothing(NodeList) Then
            NodeList = New List(Of uiTree)
        End If
        NodeList.Add(New uiTree(_Name))
        pfather.SetSons(Me)
    End Sub
    Public Function item(ByVal index As Integer) As uiTree
        If IsNothing(NodeList) = False Then
            If index < NodeList.Count Then
                Return NodeList(index)
            End If
        End If
    End Function
    Public Function item(ByVal nodeName As String) As uiTree
        If IsNothing(NodeList) = False Then
            For j = NodeList.Count - 1 To 0 Step -1
                If NodeList(j).name = nodeName Then
                    Return NodeList(j)
                End If
            Next
        End If
        Return Nothing
    End Function
    Public Sub AddNode(ByVal _UINode As uiTree)
        If IsNothing(NodeList) Then
            NodeList = New List(Of uiTree)
        End If
        NodeList.Add(_UINode)
        pfather.SetSons(Me)
    End Sub
    Public Sub RemoveNode(ByVal _Name As String)
        If IsNothing(NodeList) = False Then
            For j = NodeList.Count - 1 To 0 Step -1
                If NodeList(j).name = _Name Then
                    NodeList.RemoveAt(j)
                    Exit For
                End If
            Next
        End If
        pfather.SetSons(Me)
    End Sub
    Public Sub Clear()
        If IsNothing(NodeList) = False Then
            NodeList.Clear()
        End If
        pfather.SetSons(Me)
    End Sub

End Class
