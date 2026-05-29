#Region "Microsoft.VisualBasic::7d18fa51882abfff1227dd37aa681f1e, mzmath\MSEngine\AnnotationPack\WorkspaceReader.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 217
    '    Code Lines: 126 (58.06%)
    ' Comment Lines: 55 (25.35%)
    '    - Xml Docs: 61.82%
    ' 
    '   Blank Lines: 36 (16.59%)
    '     File Size: 7.78 KB


    ' Interface IWorkspaceReader
    ' 
    '     Function: LoadMemory
    ' 
    ' Class LibraryWorkspace
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetAnnotations, read
    ' 
    '     Sub: add, (+2 Overloads) commit, ReadWithHandleConflicts, save
    ' 
    ' /********************************************************************************/

#End Region

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
            Dim key As String = $"{xref_id}|{peak.ID}"

            annotation.xcms_id = peak.ID
            annotation.mz = peak.mz
            annotation.rt = peak.rt
            annotation.RI = peak.RI
            annotation.npeaks = npeaks

            If annotations.ContainsKey(key) Then
                ' keeps the best ion?
                key = key & "_" & annotations.Count + 1
            End If

            Call annotations.Add(key, annotation)
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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="mz_bin"></param>
    ''' <param name="filter_ms1">
    ''' do not load ms1 annotation result? default is yes
    ''' </param>
    ''' <returns></returns>
    Public Shared Function read(file As Stream, Optional mz_bin As Boolean = False, Optional filter_ms1 As Boolean = True) As LibraryWorkspace
        Dim text As New StreamReader(file)
        Dim libs As New LibraryWorkspace
        Dim line As Value(Of String) = ""

        If mz_bin Then
            Call VBDebugger.EchoLine("annotation reference id will be attached mz integer tag for make unique!")
        End If
        If filter_ms1 Then
            Call VBDebugger.EchoLine("only loads the annotation result that has ms2 spectrum aligned!")
        End If

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
            Dim key As String

            ' no ms1 peak assigned
            For Each annotation As AlignmentHit In load
                If annotation.samplefiles.IsNullOrEmpty Then
                    If filter_ms1 Then
                        Continue For
                    End If
                End If

                If mz_bin Then
                    ' attach mz_bin for make unique
                    key = $"{annotation.libname}|{annotation.adducts}|{CInt(annotation.mz)}"
                Else
                    key = $"{annotation.libname}|{annotation.adducts}"
                End If

                Call ReadWithHandleConflicts(libs, key, annotation)
            Next
        Else
            ' already has ms1 peak assigned information
            For Each annotation As AlignmentHit In load
                If annotation.samplefiles.IsNullOrEmpty Then
                    If filter_ms1 Then
                        Continue For
                    End If
                End If

                Dim key As String = $"{annotation.libname}|{annotation.adducts}|{annotation.xcms_id}"

                Call ReadWithHandleConflicts(libs, key, annotation)
            Next
        End If

        Return libs
    End Function

    Private Shared Sub ReadWithHandleConflicts(libs As LibraryWorkspace, key As String, annotation As AlignmentHit)
        If libs.annotations.ContainsKey(key) Then
            ' has duplicted annotation result
            Dim a = libs.annotations(key)

            If a.samplefiles.TryCount > annotation.samplefiles.TryCount Then
                ' just merge current annotation to a
                For Each sample In annotation.samplefiles
                    If Not a.samplefiles.ContainsKey(sample.Key) Then
                        Call a.samplefiles.Add(sample.Key, sample.Value)
                    End If
                Next
            Else
                ' merge a to current annotation, and then replace the a
                For Each sample In a.samplefiles
                    If Not annotation.samplefiles.ContainsKey(sample.Key) Then
                        Call annotation.samplefiles.Add(sample.Key, sample.Value)
                    End If
                Next

                ' make replacement
                libs.annotations(key) = annotation
            End If
        Else
            Call libs.annotations.Add(key, annotation)
        End If
    End Sub

End Class
