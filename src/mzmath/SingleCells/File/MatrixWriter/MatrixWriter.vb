#Region "Microsoft.VisualBasic::95c446f8ad64ee51af6404632243ca37, mzmath\SingleCells\File\MatrixWriter\MatrixWriter.vb"

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

    '   Total Lines: 112
    '    Code Lines: 58
    ' Comment Lines: 34
    '   Blank Lines: 20
    '     File Size: 3.48 KB


    ' Class MatrixWriter
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: WriteHeader
    ' 
    '     Sub: Write, WriteIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' helper module for write the matrix as binary data
''' </summary>
Public Class MatrixWriter

    ''' <summary>
    ''' the rawdata matrix object to save to file
    ''' </summary>
    ReadOnly m As MzMatrix

    ''' <summary>
    ''' the binary file magic header string
    ''' </summary>
    Public Const magic As String = "single_cell"

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <DebuggerStepThrough>
    Sub New(m As MzMatrix)
        Me.m = m
    End Sub

    ''' <summary>
    ''' save the matrix to a file
    ''' </summary>
    ''' <param name="s">target file to save the matrix data</param>
    Public Sub Write(s As Stream)
        Dim bin As New BinaryWriter(s, encoding:=Encoding.ASCII)
        Dim offset As Long = WriteHeader(bin, m.GetHeader)

        ' write index placeholder
        Call bin.Write(0&)
        Call bin.Write(0&)

        Dim writeSpots As New SpotWriter(bin)
        Dim offset1, offset2 As Long

        For Each spot As PixelData In m.matrix.SafeQuery
            Call writeSpots.AddSpot(spot)
        Next

        Call WriteIndex(bin, writeSpots, offset1, offset2)

        Call s.Seek(offset, SeekOrigin.Begin)
        Call bin.Write(offset1)
        Call bin.Write(offset2)
        Call bin.Flush()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="bin"></param>
    ''' <param name="header"></param>
    ''' <returns>
    ''' the start position of the cell spot data block, this position minus 4 for 
    ''' get/set the position of the data <see cref="MatrixHeader.numSpots"/>.
    ''' </returns>
    Public Shared Function WriteHeader(bin As BinaryWriter, header As MatrixHeader) As Long
        Dim s As Stream = bin.BaseStream

        Call bin.Write(magic.Select(Function(c) CByte(Asc(c))).ToArray, Scan0, magic.Length)
        Call bin.Write(header.tolerance)
        Call bin.Write(header.featureSize)
        Call bin.Write(header.matrixType)  ' int32

        For Each mzi As Double In header.mz.SafeQuery
            Call bin.Write(mzi)
        Next

        ' save count of the spots
        ' int32
        Call bin.Write(header.numSpots)
        Call bin.Flush()

        Return s.Position
    End Function

    ''' <summary>
    ''' write the spot offset index: x,y,z coordinate offset andalso the cell label offset
    ''' </summary>
    ''' <param name="bin"></param>
    ''' <param name="tmp"></param>
    ''' <param name="offset1"></param>
    ''' <param name="offset2"></param>
    Public Shared Sub WriteIndex(bin As BinaryWriter, tmp As SpotWriter, ByRef offset1 As Long, ByRef offset2 As Long)
        Dim s As Stream = bin.BaseStream

        ' write spatial spot index
        offset1 = s.Position

        For Each index As SpatialIndex In tmp.GetSpotOffSets
            Call bin.Write(index.X)
            Call bin.Write(index.Y)
            Call bin.Write(index.Z)
            Call bin.Write(index.offset)
        Next

        ' write singlecell label index
        offset2 = s.Position

        For Each index In tmp.GetLabelOffsetIndex
            Call bin.Write(index.Item1)
            Call bin.Write(index.Item2)
        Next
    End Sub
End Class
