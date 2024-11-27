#Region "Microsoft.VisualBasic::9bd74d615b4b753bba781fe21e7eebf7, mzmath\MSEngine\AnnotationPack\AnnotationPack.vb"

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

    '   Total Lines: 142
    '    Code Lines: 80 (56.34%)
    ' Comment Lines: 46 (32.39%)
    '    - Xml Docs: 76.09%
    ' 
    '   Blank Lines: 16 (11.27%)
    '     File Size: 4.79 KB


    ' Class AnnotationPack
    ' 
    '     Properties: file, libraries, peaks, XIC
    ' 
    '     Function: CreatePeakSet, GetAnnotation, GetLibraryResult, LoadMemory, samplefiles
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Result data pack for save the annotation result data
''' </summary>
''' <remarks>
''' data export for internal annotation workflow, handling to customer report and view on mzkit workbench.
''' </remarks>
Public Class AnnotationPack : Implements IWorkspaceReader, IDisposable

    Private disposedValue As Boolean

    ''' <summary>
    ''' the ms2 spectrum alignment search hits
    ''' </summary>
    ''' <returns></returns>
    Public Property libraries As Dictionary(Of String, AlignmentHit())

    ''' <summary>
    ''' [xcms_id => XIC across multiple sample files]
    ''' </summary>
    ''' <returns></returns>
    Public Property XIC As Dictionary(Of String, MzGroup())

    ''' <summary>
    ''' the ms1 peaktable
    ''' </summary>
    ''' <returns></returns>
    Public Property peaks As xcms2()

    Public Property file As String

    ''' <summary>
    ''' get all sample file names
    ''' </summary>
    ''' <returns>
    ''' this function returns the sample file name without extension names
    ''' </returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function samplefiles() As IEnumerable(Of String)
        Return peaks _
            .Select(Function(sample)
                        Return sample.Properties.Keys
                    End Function) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(name)
                         Return name
                     End Function)
    End Function

    ''' <summary>
    ''' get peaktable
    ''' </summary>
    ''' <returns></returns>
    Public Function CreatePeakSet() As PeakSet
        Return New PeakSet(peaks)
    End Function

    Public Function GetLibraryResult(libraryName As String) As AlignmentHit()
        If libraries.ContainsKey(libraryName) Then
            Return _libraries(libraryName)
        Else
            Return {}
        End If
    End Function

    ''' <summary>
    ''' Make a copy of current in-memory data pack
    ''' </summary>
    ''' <returns></returns>
    Private Function LoadMemory() As AnnotationPack Implements IWorkspaceReader.LoadMemory
        Return New AnnotationPack With {
            .libraries = libraries _
                .ToDictionary(Function(li) li.Key,
                              Function(li)
                                  Return li.Value.ToArray
                              End Function),
            .peaks = peaks.ToArray
        }
    End Function

    Public Iterator Function GetAnnotation() As IEnumerable(Of Peaktable)
        For Each result As AlignmentHit In libraries.Values.IteratesALL
            Yield New Peaktable With {
                .annotation = result.adducts,
                .energy = 0,
                .formula = result.formula,
                .id = result.biodeep_id,
                .index = result.RI,
                .into = 0,
                .intb = 0,
                .ionization = "HCD",
                .mass = result.mz,
                .maxo = 0,
                .mzmax = result.mz,
                .mzmin = result.mz,
                .name = result.name,
                .rt = result.rt,
                .rtmax = .rt,
                .rtmin = .rt,
                .sample = result.xcms_id,
                .scan = 0,
                .sn = 0
            }
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Erase peaks

                ' clear all annotation data
                Call libraries.Clear()
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    ''' <summary>
    ''' just release the memory content data at here
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
