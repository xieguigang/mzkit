Public Interface IWorkspaceReader

    Function LoadMemory() As AnnotationPack

End Interface

Public Class LibraryWorkspace

    ''' <summary>
    ''' libname|adducts as unique key
    ''' </summary>
    ReadOnly annotations As New Dictionary(Of String, AlignmentHit)

    Sub New()
    End Sub

    Public Sub Add(db_xref As String)

    End Sub

End Class