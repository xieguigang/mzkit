Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Landscape.Ply

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
            Dim scans As IEnumerable(Of Scan3DReader) = imzML.XML.Load3DScanData(imzML:=xml)
            Dim intensity As Func(Of ms2(), Double) = Function(scan) Aggregate i In scan Into Sum(i.intensity)
            Dim pointcloud As IEnumerable(Of PointCloud) = scans.LoadPointCloud(intensity)
            Dim cachefile As String = ply.ChangeSuffix("pointcloud_cache")

            Using file As Stream = cachefile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Call pointcloud.cache(New BinaryDataWriter(file))
            End Using

            Using file As Stream = ply.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                ' Return SimplePlyWriter.WriteAsciiText(pointcloud, file, colors)
            End Using
        End Function
    End Module

End Namespace