#Region "Microsoft.VisualBasic::552f1a63aa52ee839a1d9fd98f52a733, Rscript\Library\mzkit\assembly\MSI.vb"

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

    ' Module MSI
    ' 
    '     Function: basePeakMz, Correction, loadRowSummary, MSI_summary, MSIScanMatrix
    '               open_imzML, PixelMatrix, pixels, pixels2D, rowScans
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports imzML = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.XML

<Package("MSI")>
Module MSI

    ''' <summary>
    ''' get pixels size from the raw data file
    ''' </summary>
    ''' <param name="file">
    ''' imML/mzPack
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("pixels")>
    Public Function pixels(file As String, Optional env As Environment = Nothing) As Object
        If file.ExtensionSuffix("imzml") Then
            Dim allScans = XML.LoadScans(file).ToArray
            Dim width As Integer = Aggregate p In allScans Into Max(p.x)
            Dim height As Integer = Aggregate p In allScans Into Max(p.y)

            Return New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"w", width},
                    {"h", height}
                }
            }
        ElseIf file.ExtensionSuffix("mzpack") Then
            Using reader As New BinaryStreamReader(file)
                Dim allMeta = reader.EnumerateIndex _
                    .Select(AddressOf reader.GetMetadata) _
                    .IteratesALL _
                    .ToArray
                Dim x As Integer() = allMeta _
                    .Where(Function(p) p.Key.TextEquals("x")) _
                    .Select(Function(p) p.Value) _
                    .Select(AddressOf Integer.Parse) _
                    .ToArray
                Dim y As Integer() = allMeta _
                    .Where(Function(p) p.Key.TextEquals("y")) _
                    .Select(Function(p) p.Value) _
                    .Select(AddressOf Integer.Parse) _
                    .ToArray

                Return New list With {
                    .slots = New Dictionary(Of String, Object) From {
                        {"w", x.Max},
                        {"h", y.Max}
                    }
                }
            End Using
        Else
            Return Internal.debug.stop("unsupported file!", env)
        End If
    End Function

    <ExportAPI("open.imzML")>
    Public Function open_imzML(file As String) As Object
        Dim scans As ScanData() = imzML.LoadScans(file:=file).ToArray
        Dim ibd = ibdReader.Open(file.ChangeSuffix("ibd"))

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"scans", scans},
                {"ibd", ibd}
            }
        }
    End Function

    ''' <summary>
    ''' each raw data file is a row scan data
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("row.scans")>
    Public Function rowScans(raw As String(),
                             Optional y As Integer = 0,
                             Optional correction As Correction = Nothing,
                             Optional env As Environment = Nothing) As Object

        If raw.IsNullOrEmpty Then
            Return Internal.debug.stop("the required raw data file list is empty!", env)
        ElseIf raw.Length = 1 Then
            If y > 0 Then
                Using file As FileStream = raw(Scan0).Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Return file.loadRowSummary(y, correction)
                End Using
            Else
                Return Internal.debug.stop("the pixels of column must be specific!", env)
            End If
        Else
            Dim loader = Iterator Function() As IEnumerable(Of mzPack)
                             For Each path As String In raw
                                 Using file As FileStream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                                     Yield mzPack.ReadAll(file, ignoreThumbnail:=True)
                                 End Using
                             Next
                         End Function
            Return pipeline.CreateFromPopulator(loader())
        End If
    End Function

    <Extension>
    Private Function loadRowSummary(file As Stream, y As Integer, correction As Correction) As iPixelIntensity()
        Dim mzpack As mzPack = mzPack.ReadAll(file, ignoreThumbnail:=True)
        Dim pixels As iPixelIntensity() = mzpack.MS _
            .Select(Function(col, i)
                        Dim basePeakMz As Double = col.mz(which.Max(col.into))

                        Return New iPixelIntensity With {
                            .average = col.into.Average,
                            .basePeakIntensity = col.into.Max,
                            .totalIon = col.into.Sum,
                            .x = If(correction Is Nothing, i + 1, correction.GetPixelRow(col.rt)),
                            .y = y,
                            .basePeakMz = basePeakMz
                        }
                    End Function) _
            .ToArray

        Return pixels
    End Function

    <ExportAPI("MSI_summary")>
    Public Function MSI_summary(raw As mzPack) As MSISummary
        Return New MSISummary With {
            .rowScans = raw.MS _
                .Select(Function(p)
                            Return New iPixelIntensity With {
                                .x = Integer.Parse(p.meta("x")),
                                .y = Integer.Parse(p.meta("y")),
                                .average = p.into.Average,
                                .basePeakIntensity = p.into.Max,
                                .totalIon = p.into.Sum,
                                .basePeakMz = p.mz(which.Max(p.into))
                            }
                        End Function) _
                .GroupBy(Function(p) p.y) _
                .OrderBy(Function(p) p.Key) _
                .Select(Function(y) y.OrderBy(Function(p) p.x).ToArray) _
                .ToArray,
            .size = New Size(
                width:= .rowScans.IteratesALL.Select(Function(p) p.x).Max,
                height:= .rowScans.IteratesALL.Select(Function(p) p.y).Max
            )
        }
    End Function

    <ExportAPI("correction")>
    Public Function Correction(totalTime As Double, pixels As Integer) As Correction
        Return New Correction(totalTime, pixels)
    End Function

    <ExportAPI("basePeakMz")>
    Public Function basePeakMz(summary As MSISummary) As LibraryMatrix
        Return summary.GetBasePeakMz
    End Function

    ''' <summary>
    ''' combine each row scan raw data files as the pixels 2D matrix
    ''' </summary>
    ''' <param name="rowScans"></param>
    ''' <returns></returns>
    <ExportAPI("scans2D")>
    Public Function pixels2D(<RRawVectorArgument>
                             rowScans As Object,
                             Optional correction As Correction = Nothing,
                             Optional intocutoff As Double = 0.05,
                             Optional env As Environment = Nothing) As Object

        Dim pipeline As pipeline = pipeline.TryCreatePipeline(Of mzPack)(rowScans, env)

        If pipeline.isError Then
            Return pipeline.getError
        Else
            Return pipeline _
                .populates(Of mzPack)(env) _
                .MSICombineRowScans(correction, intocutoff, progress:=Sub(msg) Call base.print(msg, env))
        End If
    End Function

    ''' <summary>
    ''' combine each row scan summary vector as the pixels 2D matrix
    ''' </summary>
    ''' <param name="rowScans"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("scanMatrix")>
    Public Function MSIScanMatrix(<RRawVectorArgument> rowScans As Object, Optional env As Environment = Nothing) As Object
        Dim data As pipeline = pipeline.TryCreatePipeline(Of iPixelIntensity)(rowScans, env)

        If data.isError Then
            Return data.getError
        End If

        Dim rows = data _
            .populates(Of iPixelIntensity)(env) _
            .GroupBy(Function(p) p.y) _
            .Select(Function(r) r.ToArray) _
            .ToArray
        Dim width As Integer = rows.Select(Function(p) p.Select(Function(pi) pi.x).Max).Max
        Dim height As Integer = rows.Select(Function(p) p.Select(Function(pi) pi.y).Max).Max

        Return New MSISummary With {
            .rowScans = rows,
            .size = New Size(width, height)
        }
    End Function

    <ExportAPI("pixelMatrix")>
    Public Function PixelMatrix(raw As mzPack, file As Stream, Optional tolerance As Object = "da:0.05", Optional env As Environment = Nothing) As Message
        Dim mzErr = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim da As Tolerance = mzErr.TryCast(Of Tolerance)
        Dim allMz As ms2() = raw.MS.Select(Function(i) i.GetMs).IteratesALL.ToArray.Centroid(da, New RelativeIntensityCutoff(0.01)).OrderBy(Function(i) i.mz).ToArray
        Dim text As New StreamWriter(file)

        Call text.WriteLine({""}.JoinIterates(allMz.Select(Function(a) a.mz.ToString("F4"))).JoinBy(","))
        Call text.Flush()

        For Each pixel As ScanMS1 In raw.MS
            Dim pid As String = $"{pixel.meta!x};{pixel.meta!y}"
            Dim msData = pixel.GetMs.ToArray
            Dim vec As String() = allMz.Select(Function(mzi)
                                                   Dim mz = msData.Where(Function(i) da(i.mz, mzi.mz)).FirstOrDefault

                                                   If mz Is Nothing Then
                                                       Return "0"
                                                   Else
                                                       Return mz.intensity.ToString
                                                   End If
                                               End Function).ToArray

            Call text.WriteLine({pid}.JoinIterates(vec).JoinBy(","))
            Call Console.WriteLine(pixel.scan_id)
        Next

        Call text.Flush()

        Return Nothing
    End Function
End Module
