#Region "Microsoft.VisualBasic::10acb8c238e44c0b5af4186fc53c7335, E:/mzkit/src/assembly/SpectrumTree//Tree/InternalFileSystem.vb"

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

    '   Total Lines: 135
    '    Code Lines: 89
    ' Comment Lines: 25
    '   Blank Lines: 21
    '     File Size: 4.72 KB


    '     Class InternalFileSystem
    ' 
    '         Properties: size
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Append, GetEnumerator, getMz, IEnumerable_GetEnumerator, ReadSpectrum
    '                   WriteSpectrum
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Tree

    Public Class InternalFileSystem : Implements IEnumerable(Of BlockNode)

        ''' <summary>
        ''' A collection of the metabolite reference spectrum data
        ''' </summary>
        Protected ReadOnly tree As New List(Of BlockNode)
        Protected ReadOnly nbranch As Integer = 10

        Default Public ReadOnly Property GetNodeByIndex(i As Integer) As BlockNode
            Get
                Return tree(i)
            End Get
        End Property

        Public ReadOnly Property size As Integer
            Get
                Return tree.Count
            End Get
        End Property

        Sub New(nbranch As Integer)
            Me.nbranch = nbranch
        End Sub

        Public Shared Iterator Function getMz(data As PeakMs2) As IEnumerable(Of Double)
            If data.mz > 0 Then
                Yield data.mz
            Else
                If Not data.meta.IsNullOrEmpty Then
                    If data.meta.ContainsKey("mz1") Then
                        For Each mzi As Double In data.meta("mz1").LoadJSON(Of Double())
                            Yield mzi
                        Next
                    End If
                End If
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data">
        ''' the reference spectrum data
        ''' </param>
        ''' <param name="centroid"></param>
        ''' <param name="isMember"></param>
        ''' <param name="spectrum">
        ''' used for save the raw spectrum data, if this parameter
        ''' value just set to NULL, then it means just create object
        ''' in memory
        ''' </param>
        ''' <returns></returns>
        Public Function Append(data As PeakMs2,
                               Optional centroid As ms2() = Nothing,
                               Optional isMember As Boolean = False,
                               Optional spectrum As BinaryDataWriter = Nothing) As Integer

            Dim n As Integer = tree.Count
            Dim childs As Integer()

            If isMember Then
                childs = {}
            Else
                childs = New Integer(nbranch - 1) {}
            End If

            If centroid Is Nothing Then
                centroid = data.mzInto.ToArray
            End If

            Call tree.Add(New BlockNode With {
                .Block = WriteSpectrum(data, spectrum),
                .childs = childs,
                .Id = data.lib_guid,
                .Members = If(isMember, Nothing, New List(Of Integer)),
                .centroid = centroid,
                .rt = data.rt,
                .mz = New List(Of Double)(getMz(data))
            })

            Return n
        End Function

        ''' <summary>
        ''' save the raw spectrum data
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="spectrum">
        ''' nothing means do not save the raw spectrum data
        ''' </param>
        ''' <returns></returns>
        Public Shared Function WriteSpectrum(data As PeakMs2, spectrum As BinaryDataWriter) As BufferRegion
            If spectrum Is Nothing Then
                Return BufferRegion.Zero
            Else
                Dim start As Long = spectrum.Position
                Dim scan As ScanMS2 = data.Scan2

                Call Serialization.WriteBuffer(scan, file:=spectrum)

                Return New BufferRegion With {
                    .position = start,
                    .size = spectrum.Position - start
                }
            End If
        End Function

        Public Shared Function ReadSpectrum(pool As BinaryDataReader, block As BufferRegion) As PeakMs2
            Dim scan2 As ScanMS2

            pool.Seek(block.position, SeekOrigin.Begin)
            scan2 = pool.ReadScanMs2

            Return scan2.GetSpectrum2
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of BlockNode) Implements IEnumerable(Of BlockNode).GetEnumerator
            For Each node As BlockNode In tree
                Yield node
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
