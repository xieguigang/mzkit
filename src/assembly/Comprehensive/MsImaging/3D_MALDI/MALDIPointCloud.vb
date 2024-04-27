#Region "Microsoft.VisualBasic::d701cb919d9dab7c7e84ebc4f8d4cb1b, G:/mzkit/src/assembly/Comprehensive//MsImaging/3D_MALDI/MALDIPointCloud.vb"

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

    '   Total Lines: 113
    '    Code Lines: 91
    ' Comment Lines: 0
    '   Blank Lines: 22
    '     File Size: 4.96 KB


    '     Module MALDIPointCloud
    ' 
    '         Function: FileConvert, LoadPointCloud, ReadPointCloud
    ' 
    '         Sub: cache, ExportHeatMapModel, SaveCache
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Landscape.Ply
Imports Microsoft.VisualBasic.Serialization.BinaryDumping

Namespace MsImaging.MALDI_3D

    Public Module MALDIPointCloud

        <Extension>
        Public Iterator Function LoadPointCloud(raw As IEnumerable(Of Scan3DReader), eval As Func(Of ms2(), Double)) As IEnumerable(Of PointCloud)
            For Each scan As Scan3DReader In raw
                Dim ms1 As ms2() = scan.LoadMsData
                Dim intensity As Double = eval(ms1)

                Yield New PointCloud With {
                    .x = scan.x,
                    .y = scan.y,
                    .z = scan.x,
                    .intensity = intensity
                }
            Next
        End Function

        <Extension>
        Private Sub cache(pointCloud As IEnumerable(Of PointCloud), file As BinaryDataWriter)
            For Each point As PointCloud In pointCloud
                Call file.Write(New Double() {point.x, point.y, point.z, point.intensity})
            Next

            Call file.Flush()
        End Sub

        Public Function FileConvert(xml As String, ply As String, Optional colors As ScalerPalette = ScalerPalette.turbo) As Boolean
            Dim cachefile As String = ply.ChangeSuffix("pointcloud_cache")

            Call saveCache(xml, cachefile)

            Using file As Stream = ply.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Return SimplePlyWriter.WriteAsciiText(ReadPointCloud(cachefile), file, colors)
            End Using
        End Function

        Public Sub SaveCache(xml As String, cachefile As String)
            Dim scans As IEnumerable(Of Scan3DReader) = imzML.XML.Load3DScanData(imzML:=xml)
            Dim intensity As Func(Of ms2(), Double) = Function(scan) Aggregate i In scan Into Sum(i.intensity)
            Dim pointcloud As IEnumerable(Of PointCloud) = scans.LoadPointCloud(intensity)

            Using file As Stream = cachefile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Call pointcloud.cache(New BinaryDataWriter(file) With {.ByteOrder = ByteOrder.BigEndian})
            End Using
        End Sub

        Private Iterator Function ReadPointCloud(cachefile As String) As IEnumerable(Of PointCloud)
            Using file As Stream = cachefile.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Dim bin As New NetworkByteOrderBuffer
                Dim buffer As Byte() = New Byte(4 * 8 - 1) {}
                Dim dbls As Double()

                Do While file.Position < file.Length
                    file.Read(buffer, Scan0, buffer.Length)
                    dbls = bin.decode(buffer)

                    Yield New PointCloud With {
                        .x = dbls(0),
                        .y = dbls(1),
                        .z = dbls(2),
                        .intensity = dbls(3)
                    }
                Loop
            End Using
        End Function

        Public Sub ExportHeatMapModel(cachefile As String, model As String,
                                      Optional colors As ScalerPalette = ScalerPalette.turbo,
                                      Optional levels As Integer = 255)

            Dim points As PointCloud() = ReadPointCloud(cachefile).ToArray
            Dim colorSet As String() = Designer _
                .GetColors(colors.Description, levels) _
                .Select(Function(c) c.ToHtmlColor) _
                .ToArray
            Dim value As New DoubleRange(From p As PointCloud In points Select p.intensity)
            Dim index As New DoubleRange(0, levels)

            Using file As Stream = model.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Dim buf As New BinaryDataWriter(file) With {.ByteOrder = ByteOrder.BigEndian}

                Call buf.Write(points.Length)
                Call buf.Write(colorSet.Length)

                For Each color As String In colorSet
                    Call buf.Write(color, BinaryStringFormat.NoPrefixOrTermination)
                Next

                For Each p As PointCloud In points
                    Call buf.Write({p.x, p.y, p.z, p.intensity})
                    Call buf.Write(CInt(value.ScaleMapping(p.intensity, index)))
                Next

                Call buf.Flush()
            End Using
        End Sub
    End Module

End Namespace
