#Region "Microsoft.VisualBasic::1af300e34865f7c94fa23eda11a6c466, mzkit\services\MZWork\MSImagingReader.vb"

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

    '   Total Lines: 103
    '    Code Lines: 80 (77.67%)
    ' Comment Lines: 15 (14.56%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 8 (7.77%)
    '     File Size: 4.39 KB


    ' Module MSImagingReader
    ' 
    '     Function: ReadmsiPLData, UnifyReadAsMzPack
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports HDF.PInvoke
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Analysis.Spatial.RAID.HDF5

Public Module MSImagingReader

    ''' <summary>
    ''' This method provides a unify method for read raw data as mzpack for ms-maging
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <returns></returns>
    Public Function UnifyReadAsMzPack(filepath As String) As [Variant](Of mzPack, ReadRawPack)
        If filepath.ExtensionSuffix("cdf") Then
            ' read multiple ion layers
            Using cdf As New netCDFReader(filepath)
                Return cdf.CreatePixelReader
            End Using
        End If
        If filepath.ExtensionSuffix("imzml") Then
            Dim mzPack = Converter.LoadimzML(filepath, 0, IonModes.Positive, Nothing, AddressOf RunSlavePipeline.SendProgress)
            mzPack.source = filepath.FileName
            Return mzPack
        End If
        If filepath.ExtensionSuffix("h5") Then
            Return ReadmsiPLData(filepath)
        End If

        ' try to open all other kind of data files as mzpack
        Return mzPack.ReadAll(filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True), ignoreThumbnail:=True)
    End Function

    ''' <summary>
    ''' read msiPLData dataset as MSimaging mzpack data object
    ''' </summary>
    ''' <param name="h5"></param>
    ''' <returns></returns>
    Public Function ReadmsiPLData(h5 As String) As mzPack
        Dim fileId As Long = H5F.open(h5, H5F.ACC_RDONLY)
        Dim mzArray = ReadData.Read_dataset(fileId, "/mzArray") _
            .GetSingles _
            .Select(Function(s) CDbl(s)) _
            .ToArray
        Dim xLocation = ReadData.Read_dataset(fileId, "/xLocation").GetDoubles.ToArray
        Dim yLocation = ReadData.Read_dataset(fileId, "/yLocation").GetDoubles.ToArray
        Dim split_size As Integer = xLocation.Length
        'Dim intos As Double()() = ReadData.Read_chunkset(fileId, "/Data") _
        '    .Select(Function(chk) ReadData.GetDoubles(chk).ToArray) _
        '    .ToArray
        Dim intos As Double()() = ReadData.Read_dataset(fileId, "/Data") _
            .GetDoubles() _
            .Split(split_size) _
            .ToArray
        Dim ms1 As New List(Of ScanMS1)

        For i As Integer = 0 To xLocation.Length - 1
            Dim x As Integer = xLocation(i)
            Dim y As Integer = yLocation(i)
            Dim offset As Integer = i
            Dim ms As ms2() = intos _
                .Select(Function(vi, j)
                            Return New ms2 With {.mz = mzArray(j), .intensity = vi(offset)}
                        End Function) _
                .Where(Function(j) j.intensity > 0) _
                .ToArray
            Dim v As Double() = ms.Select(Function(mzi) mzi.intensity).ToArray

            If v.Length = 0 Then
                Continue For
            End If

            Call ms1.Add(New ScanMS1 With {
                .BPC = v.Max,
                .into = v,
                .meta = New Dictionary(Of String, String) From {
                    {"x", x}, {"y", y}
                },
                .mz = ms.Select(Function(mzi) mzi.mz).ToArray,
                .products = {},
                .rt = i,
                .TIC = v.Sum,
                .scan_id = $"[MS1][{i + 1}][{x},{y}] {ms.Length} ions, BPC={ .BPC.ToString("G3")}, total_ions={ .TIC.ToString("G3")}, basepeak_m/z={ .mz(which.Max(v))}"
            })
        Next

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging,
            .metadata = New Dictionary(Of String, String),
            .source = h5.FileName,
            .MS = ms1.ToArray
        }
    End Function
End Module
