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
    ''' 
    ''' </summary>
    ''' <returns>
    ''' Only populate the alignment result which has ms1 peak assigned by default.
    ''' </returns>
    Public Iterator Function GetAnnotations(Optional filterPeaks As Boolean = True) As IEnumerable(Of AlignmentHit)
        For Each annotation As AlignmentHit In annotations.Values
            ' if filter ms1 peak assigned information,
            ' and also current annotation result has no ms1 peak id
            ' then skip
            If filterPeaks AndAlso String.IsNullOrEmpty(annotation.xcms_id) Then
                Continue For
            End If

            Yield annotation
        Next
    End Function

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

        For Each annotation As AlignmentHit In annotations.Values
            If commit_peaks Then
                If String.IsNullOrEmpty(annotation.xcms_id) Then
                    Continue For
                End If
            End If

            Call text.WriteLine(annotation.GetJson)
        Next

        Call text.Flush()
    End Sub

    Public Shared Function read(file As Stream, Optional mz_bin As Boolean = False) As LibraryWorkspace
        Dim text As New StreamReader(file)
        Dim libs As New LibraryWorkspace
        Dim line As Value(Of String) = ""

        ' the workspace has two status:
        '
        ' no ms1 peak assigned, missing xcms_id, unique id via: $"{annotation.libname}|{annotation.adducts}"
        ' has ms1 peak assigned, $"{annotation.libname}|{annotation.adducts}" will be duplicated, then indexed via $"{annotation.libname}|{annotation.adducts}|{annotation.xcms_id}"
        Dim load As New List(Of AlignmentHit)

        Do While Not (line = text.ReadLine) Is Nothing
            ' Dim annotation As AlignmentHit = CStr(line).LoadJSON(Of AlignmentHit)
            ' Dim xref_id As String = $"{annotation.libname}|{annotation.adducts}"

            ' Call libs.annotations.Add(xref_id, annotation)
            Call load.Add(CStr(line).LoadJSON(Of AlignmentHit))
        Loop

        ' check of the peak assign status
        If load.All(Function(a) a.xcms_id.StringEmpty(, True)) Then
            ' no ms1 peak assigned
            For Each annotation As AlignmentHit In load
                If mz_bin Then
                    ' attach mz_bin for make unique
                    Call libs.annotations.Add($"{annotation.libname}|{annotation.adducts}|{CInt(annotation.mz)}", annotation)
                Else
                    Call libs.annotations.Add($"{annotation.libname}|{annotation.adducts}", annotation)
                End If
            Next
        Else
            ' already has ms1 peak assigned information
            For Each annotation As AlignmentHit In load
                Call libs.annotations.Add($"{annotation.libname}|{annotation.adducts}|{annotation.xcms_id}", annotation)
            Next
        End If

        Return libs
    End Function

End Class