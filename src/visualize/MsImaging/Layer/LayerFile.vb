#Region "Microsoft.VisualBasic::9ed1ff4706aec9df3712af0422c26566, visualize\MsImaging\Layer\LayerFile.vb"

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

    '   Total Lines: 91
    '    Code Lines: 74 (81.32%)
    ' Comment Lines: 3 (3.30%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (15.38%)
    '     File Size: 3.10 KB


    ' Module LayerFile
    ' 
    '     Function: LoadSummaryLayer, ParseLayer, readPixels
    ' 
    '     Sub: SaveLayer, SaveMSISummary
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

''' <summary>
''' binary file r/w for <see cref="SingleIonLayer"/>
''' </summary>
Public Module LayerFile

    <Extension>
    Public Sub SaveLayer(layer As SingleIonLayer, file As Stream)
        Dim wr As New BinaryDataWriter(file, Encodings.UTF8)

        Call wr.Write(layer.IonMz)
        Call wr.Write(layer.DimensionSize.Width)
        Call wr.Write(layer.DimensionSize.Height)
        Call wr.Write(0&)
        Call PixelData.GetBuffer(layer.MSILayer, file:=wr)
        Call wr.Flush()
    End Sub

    Public Function ParseLayer(bin As Stream) As SingleIonLayer
        Dim rd As New BinaryDataReader(bin, Encodings.UTF8)
        Dim label As String = rd.ReadString
        Dim size As Integer() = rd.ReadInt32s(count:=2)
        Dim pixels As PixelData()

        rd.ReadInt64()
        pixels = PixelData.Parse(rd).ToArray

        Return New SingleIonLayer With {
            .DimensionSize = New Size(size(0), size(1)),
            .IonMz = label,
            .MSILayer = pixels
        }
    End Function

    <Extension>
    Public Sub SaveMSISummary(layer As MSISummary, file As Stream)
        Dim wr As New BinaryDataWriter(file)
        Dim vec As iPixelIntensity() = layer.rowScans.IteratesALL.ToArray

        wr.Write(layer.size.Width)
        wr.Write(layer.size.Height)
        wr.Write(vec.Length)

        For Each pixel As iPixelIntensity In vec
            Call wr.Write(pixel.x)
            Call wr.Write(pixel.y)
            Call wr.Write(pixel.totalIon)
            Call wr.Write(pixel.basePeakIntensity)
            Call wr.Write(pixel.average)
            Call wr.Write(pixel.basePeakMz)
            Call wr.Write(pixel.min)
            Call wr.Write(pixel.median)
            Call wr.Write(pixel.numIons)
        Next

        Call wr.Flush()
    End Sub

    Public Function LoadSummaryLayer(bin As Stream) As MSISummary
        Dim rd As New BinaryDataReader(bin)
        Dim size As Integer() = rd.ReadInt32s(count:=2)
        Dim n As Integer = rd.ReadInt32

        Return MSISummary.FromPixels(rd.readPixels(n), dims:=New Size(size(0), size(1)))
    End Function

    <Extension>
    Private Iterator Function readPixels(rd As BinaryDataReader, nsize As Integer) As IEnumerable(Of iPixelIntensity)
        For i As Integer = 0 To nsize - 1
            Yield New iPixelIntensity With {
                .x = rd.ReadInt32,
                .y = rd.ReadInt32,
                .totalIon = rd.ReadDouble,
                .basePeakIntensity = rd.ReadDouble,
                .average = rd.ReadDouble,
                .basePeakMz = rd.ReadDouble,
                .min = rd.ReadDouble,
                .median = rd.ReadDouble,
                .numIons = rd.ReadInt32
            }
        Next
    End Function

End Module
