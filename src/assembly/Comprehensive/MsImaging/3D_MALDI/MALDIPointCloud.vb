Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.Imaging.Landscape.Ply

Namespace MsImaging.MALDI_3D

    Public Module MALDIPointCloud

        <Extension>
        Public Iterator Function LoadPointCloud(raw As IEnumerable(Of Scan3DReader)) As IEnumerable(Of PointCloud)

        End Function

        Public Function FileConvert(xml As String, ply As String) As Boolean
            Dim scans As IEnumerable(Of Scan3DReader) = imzML.XML.Load3DScanData(imzML:=xml)
            Dim pointcloud As IEnumerable(Of PointCloud) = scans.LoadPointCloud

            Using file As Stream = ply.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)

            End Using
        End Function
    End Module

End Namespace