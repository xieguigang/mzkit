Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language

Namespace MsImaging.MALDI_3D

    Public Module Imports3DMSI

        Public Iterator Function Convert3DImaging(raw As IEnumerable(Of Scan3DReader)) As IEnumerable(Of ScanMS1)
            Dim i As i32 = 1

            For Each scan As Scan3DReader In raw
                Dim ms1 As ms2() = scan.LoadMsData
                Dim metadata As New Dictionary(Of String, String)

                Call metadata.Add("x", scan.x)
                Call metadata.Add("y", scan.y)
                Call metadata.Add("z", scan.z)

                Yield New ScanMS1 With {
                    .mz = ms1.Select(Function(a) a.mz).ToArray,
                    .into = ms1.Select(Function(a) a.intensity).ToArray,
                    .BPC = If(.into.Length = 0, 0, .into.Max),
                    .TIC = .into.Sum,
                    .meta = metadata,
                    .products = Nothing,
                    .rt = 0,
                    .scan_id = $"[MS1] [{scan.x},{scan.y},{scan.z}] {++i} total_ions={ .TIC}"
                }
            Next
        End Function

        Public Function FileConvert(xml As String, mzpack As String) As Boolean
            Dim scans As IEnumerable(Of Scan3DReader) = imzML.XML.Load3DScanData(imzML:=xml)
            Dim ms1 As IEnumerable(Of ScanMS1) = Convert3DImaging(raw:=scans)

            Using buffer As Stream = mzpack.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Dim pack As New StreamPack(buffer, meta_size:=128 * 1024 * 1024, [readonly]:=False)
                Dim metadata As New Dictionary(Of String, Double)
                Dim samples As New List(Of String)

                Call ms1.WriteStream(pack, metadata, samples)
                Call WriteApplicationClass(FileApplicationClass.MSImaging3D, pack)
                Call pack.Flush()
            End Using

            Return 0
        End Function
    End Module
End Namespace