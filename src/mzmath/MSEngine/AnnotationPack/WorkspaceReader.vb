Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Public Interface IWorkspaceReader

    Function LoadMemory() As AnnotationPack

End Interface

''' <summary>
''' A temp workspace of a single reference library
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

    Public Sub commit(xref_id As String, peak As xcms2, npeaks As Integer)
        If annotations.ContainsKey(xref_id) Then
            Dim annotation As New AlignmentHit(annotations(xref_id))
            annotation.xcms_id = peak.ID
            annotation.mz = peak.mz
            annotation.rt = peak.rt
            annotation.RI = peak.RI
            annotation.npeaks = npeaks

            Call annotations.Add($"{xref_id}|{peak.ID}", annotation)
        End If
    End Sub

    Public Sub save(file As Stream, Optional commit_peaks As Boolean = False)
        Dim text As New StreamWriter(file)

        For Each annotation In annotations
            If commit_peaks Then
                If String.IsNullOrEmpty(annotation.Value.xcms_id) Then
                    Continue For
                End If
            End If

            Call text.WriteLine(annotation.Value.GetJson)
        Next

        Call text.Flush()
    End Sub

    Public Shared Function read(file As Stream) As LibraryWorkspace
        Dim text As New StreamReader(file)
        Dim libs As New LibraryWorkspace
        Dim line As Value(Of String) = ""

        Do While Not (line = text.ReadLine) Is Nothing
            Dim annotation As AlignmentHit = CStr(line).LoadJSON(Of AlignmentHit)
            Dim xref_id As String = $"{annotation.libname}|{annotation.adducts}"

            Call libs.annotations.Add(xref_id, annotation)
        Loop

        Return libs
    End Function

End Class