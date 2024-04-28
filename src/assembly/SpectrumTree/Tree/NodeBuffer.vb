#Region "Microsoft.VisualBasic::f32332c6af9dcf01711040f4ceb36b0f, G:/mzkit/src/assembly/SpectrumTree//Tree/NodeBuffer.vb"

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

    '   Total Lines: 70
    '    Code Lines: 58
    ' Comment Lines: 3
    '   Blank Lines: 9
    '     File Size: 2.82 KB


    '     Module NodeBuffer
    ' 
    '         Function: Read
    ' 
    '         Sub: Write
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO

Namespace Tree

    ''' <summary>
    ''' read/writer filesystem api for the spectrum tree reference library
    ''' </summary>
    Public Module NodeBuffer

        Friend Sub Write(node As BlockNode, file As BinaryDataWriter)
            Call file.Write(node.Id, BinaryStringFormat.ZeroTerminated)
            Call file.Write(node.mz.Count)
            Call file.Write(node.mz.ToArray)
            Call file.Write(node.rt)
            Call file.Write(node.Block.position)
            Call file.Write(node.Block.size)
            Call file.Write(node.childs.Length)
            Call file.Write(node.childs)

            If Not node.childs.IsNullOrEmpty Then
                Call file.Write(node.Members.Count)
                Call file.Write(node.Members.ToArray)
            End If

            Call file.Write(node.centroid.Length)
            Call file.Write(node.centroid.Select(Function(a) a.mz).ToArray)
            Call file.Write(node.centroid.Select(Function(a) a.intensity).ToArray)
        End Sub

        Friend Function Read(file As BinaryDataReader) As BlockNode
            Dim id As String = file.ReadString(BinaryStringFormat.ZeroTerminated)
            Dim mz_size As Integer = file.ReadInt32
            Dim mz1 As Double() = file.ReadDoubles(mz_size)
            Dim rt As Double = file.ReadDouble
            Dim pos As Long = file.ReadInt64
            Dim size As Integer = file.ReadInt32
            Dim nchilds As Integer = file.ReadInt32
            Dim childs As Integer() = file.ReadInt32s(nchilds)
            Dim nMembers As Integer = 0
            Dim members As Integer() = Nothing

            If nchilds > 0 Then
                nMembers = file.ReadInt32
                members = file.ReadInt32s(nMembers)
            End If

            Dim nsize As Integer = file.ReadInt32
            Dim mz As Double() = file.ReadDoubles(nsize)
            Dim into As Double() = file.ReadDoubles(nsize)

            Return New BlockNode With {
                .Id = id,
                .Block = New BufferRegion(pos, size),
                .childs = childs,
                .Members = If(childs.Length = 0, Nothing, New List(Of Integer)(members)),
                .centroid = mz _
                    .Select(Function(mzi, i)
                                Return New ms2 With {
                                    .mz = mzi,
                                    .intensity = into(i)
                                }
                            End Function) _
                    .ToArray,
                .rt = rt,
                .mz = New List(Of Double)(mz1)
            }
        End Function
    End Module
End Namespace
