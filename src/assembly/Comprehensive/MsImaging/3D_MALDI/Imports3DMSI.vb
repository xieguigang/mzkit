#Region "Microsoft.VisualBasic::93f59c8ced350663307688beb8c65a15, assembly\Comprehensive\MsImaging\3D_MALDI\Imports3DMSI.vb"

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

    '   Total Lines: 55
    '    Code Lines: 45 (81.82%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (18.18%)
    '     File Size: 2.44 KB


    '     Module Imports3DMSI
    ' 
    '         Function: Convert3DImaging, FileConvert
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices
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
                Call DirectCast(pack, IFileSystemEnvironment).Flush()
            End Using

            Return 0
        End Function
    End Module
End Namespace
