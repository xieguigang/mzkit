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
        Dim samples As Dictionary(Of String, Ms2Score) = tmp _
            .GroupBy(Function(a) a.source) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.OrderByDescending(Function(i) i.score).First
                          End Function)

        Call annotations.Add(xref_id, New AlignmentHit With {.samplefiles = samples})
    End Sub

End Class