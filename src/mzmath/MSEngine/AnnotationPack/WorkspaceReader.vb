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

    ''' <summary>
    ''' commit the annotation and ms2 alignment details
    ''' </summary>
    ''' <param name="xref_id"></param>
    ''' <param name="annotation"></param>
    ''' <remarks>
    ''' thread unsafe
    ''' </remarks>
    Public Sub commit(xref_id As String, annotation As AlignmentHit)
        Dim samples As Dictionary(Of String, Ms2Score) = tmp _
            .GroupBy(Function(a) a.source) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.OrderByDescending(Function(i) i.score).First
                          End Function)

        Call tmp.Clear()

        annotation.occurrences = samples.Count
        annotation.samplefiles = samples
        annotations.Add(xref_id, annotation)
    End Sub

End Class