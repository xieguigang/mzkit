Public Interface IWorkspaceReader

    Function LoadMemory() As AnnotationPack

End Interface

''' <summary>
''' A temp workspace
''' </summary>
Public Class LibraryWorkspace

    ''' <summary>
    ''' libname|adducts as unique key
    ''' </summary>
    ReadOnly annotations As New Dictionary(Of String, AlignmentHit)
    ReadOnly tmp As New List(Of Ms2Score)

    Sub New()
    End Sub

    ''' <summary>
    ''' add to workspace temp buffer
    ''' </summary>
    ''' <param name="score"></param>
    ''' <remarks>
    ''' thread unsafe
    ''' </remarks>
    Public Sub add(score As Ms2Score)
        Call tmp.Add(score)
    End Sub

    Public Sub commit(xref_id As String)

    End Sub

End Class