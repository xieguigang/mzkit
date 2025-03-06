#Region "Microsoft.VisualBasic::d766fced9212bbd35a7e2f6d4498881c, assembly\Comprehensive\MsImaging\Converter\imzMLConvertor.vb"

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

    '   Total Lines: 88
    '    Code Lines: 61 (69.32%)
    ' Comment Lines: 18 (20.45%)
    '    - Xml Docs: 88.89%
    ' 
    '   Blank Lines: 9 (10.23%)
    '     File Size: 4.19 KB


    '     Module imzMLConvertor
    ' 
    '         Function: ConvertImzMLOntheFly, ScanShadows
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit

Namespace MsImaging

    Public Module imzMLConvertor

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="target"></param>
        ''' <param name="output"></param>
        ''' <param name="defaultIon"></param>
        ''' <param name="make_centroid"></param>
        ''' <param name="progress"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' this function report the ibd data load progress in a more verbose mode.
        ''' </remarks>
        Public Function ConvertImzMLOntheFly(target As String, output As String,
                                             Optional defaultIon As IonModes = IonModes.Positive,
                                             Optional make_centroid As Tolerance = Nothing,
                                             Optional cutoff As Double = 0.001,
                                             Optional progress As RunSlavePipeline.SetProgressEventHandler = Nothing) As Boolean
            ' convert file on the fly
            Dim allscans As ScanData() = Nothing
            Dim imzml As imzMLMetadata = Nothing
            Dim metadata = Converter.loadimzMLMetadata(target, allscans, metadata:=imzml)
            Dim ibd As String = target.ChangeSuffix("ibd")
            Dim ibdStream As Stream = ibd.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim ibdreader As New ibdReader(ibdStream, imzml.format)
            Dim filename As String = imzml.sourcefiles.First.FileName
            Dim contentMeta As Dictionary(Of String, String) = imzml.AsList

            Using s As Stream = output.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Return allscans _
                    .LoadScanStream(ibdreader, filename, defaultIon, cutoff, make_centroid,
                                    progress:=progress,
                                    verbose:=3) _
                    .WriteStream(file:=s,
                                 source:=filename,
                                 meta_size:=64 * ByteSize.MB,
                                 [class]:=FileApplicationClass.MSImaging,
                                 metadata:=contentMeta)
            End Using
        End Function

        ''' <summary>
        ''' view of the sample outline when ibd data file is missing
        ''' </summary>
        ''' <param name="imzml"></param>
        ''' <returns></returns>
        Public Function ScanShadows(imzml As String) As mzPack
            Dim allscans As ScanData() = Nothing
            Dim header As imzMLMetadata = Nothing
            Dim metadata = Converter.loadimzMLMetadata(imzml, allscans, metadata:=header)
            Dim checkTic = allscans.Any(Function(a) a.totalIon > 0)
            Dim pixelShadows = allscans _
                .Select(Function(si, i)
                            Dim data As Double = si.totalIon

                            If Not checkTic Then
                                data = si.MzPtr.arrayLength
                            End If

                            Return New ScanMS1 With {
                                .BPC = data,
                                .into = {data},
                                .meta = New Dictionary(Of String, String) From {{"x", si.x}, {"y", si.y}},
                                .mz = {100},
                                .rt = i,
                                .scan_id = si.spotID,
                                .TIC = data
                            }
                        End Function) _
                .ToArray

            metadata.MS = pixelShadows

            Return metadata
        End Function
    End Module
End Namespace
