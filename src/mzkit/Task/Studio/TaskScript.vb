#Region "Microsoft.VisualBasic::8b7cb70482cd70c8a59223a59024bcba, mzkit\src\mzkit\Task\Studio\TaskScript.vb"

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

    '   Total Lines: 380
    '    Code Lines: 311
    ' Comment Lines: 15
    '   Blank Lines: 54
    '     File Size: 16.32 KB


    ' Module TaskScript
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: alignMz, RunFeatureDetections
    ' 
    '     Sub: CreateMSIIndex, CreateMzpack, DrawMs1Contour, ExportMSISampleTable, formulaSearch
    '          MetaDNASearch, MSI_rowbind, SetBioDeepSession
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports BioDeep
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
' Imports SMRUCC.genomics.Analysis.Microarray.PhenoGraph
Imports stdNum = System.Math

<Package("task")>
Module TaskScript

    Sub New()
        FrameworkInternal.ConfigMemory(MemoryLoads.Heavy)
    End Sub

    <ExportAPI("phenograph")>
    Public Function RunFeatureDetections(mzpackRaw As String, topN As Integer, dims As Integer, mzdiff As String) As NetworkGraph
        Dim mzpack As mzPack = mzPack.Read(mzpackRaw, ignoreThumbnail:=True)
        Dim mzErr As Tolerance = Tolerance.ParseScript(mzdiff)
        Dim intocutoff As New RelativeIntensityCutoff(0.01)
        Dim pixelsData = mzpack.MS _
            .AsParallel _
            .Select(Function(m)
                        Dim msArray = m.GetMs _
                            .ToArray _
                            .Centroid(mzErr, intocutoff) _
                            .OrderByDescending(Function(d) d.intensity) _
                            .Take(topN) _
                            .ToArray

                        Return (m.GetMSIPixel, msArray)
                    End Function) _
            .ToArray
        Dim allMz = pixelsData _
            .Select(Function(d) d.msArray) _
            .IteratesALL _
            .ToArray _
            .Centroid(mzErr, intocutoff) _
            .OrderByDescending(Function(d) d.intensity) _
            .Take(dims) _
            .Select(Function(d) d.mz) _
            .ToArray
        Dim mzData As DataSet() = pixelsData _
            .AsParallel _
            .Select(Function(d)
                        Dim vec As New Dictionary(Of String, Double)

                        For Each mz As Double In allMz
                            vec(mz.ToString("F4")) = d.msArray _
                                .Where(Function(di) mzErr(di.mz, mz)) _
                                .Select(Function(di)
                                            Return di.intensity
                                        End Function) _
                                .Sum
                        Next

                        Return New DataSet With {
                            .ID = $"{d.GetMSIPixel.X},{d.GetMSIPixel.Y}",
                            .Properties = vec
                        }
                    End Function) _
            .ToArray

        Throw New NotImplementedException
        ' Return mzData.CreatePhenoGraph(k:=120)
    End Function

    <ExportAPI("MSI_peaktable")>
    Public Sub ExportMSISampleTable(raw As String, regions As Rectangle(), save As Stream)
        Dim data As New Dictionary(Of String, ms2())

        Call RunSlavePipeline.SendMessage("Initialize raw data file...")

        Dim render As New Drawer(mzPack.ReadAll(raw.Open(FileMode.Open, doClear:=False, [readOnly]:=True), ignoreThumbnail:=True))
        Dim ppm20 As Tolerance = Tolerance.PPM(20)
        Dim j As i32 = 1
        Dim regionId As String

        For Each region As Rectangle In regions
            regionId = $"region[{region.Left},{region.Top},{region.Width},{region.Height}]_{++j}"
            RunSlavePipeline.SendProgress(j / regions.Length * 100, $"scan for region {regionId}... [{j}/{regions.Length}]")
            data.Add(regionId, render.pixelReader.GetPixel(region).Select(Function(i) i.GetMs).IteratesALL.ToArray)
        Next

        Dim allMz As Double() = data.Values _
            .IteratesALL _
            .ToArray _
            .Centroid(ppm20, New RelativeIntensityCutoff(0.01)) _
            .Select(Function(i) i.mz) _
            .OrderBy(Function(mz) mz) _
            .ToArray

        RunSlavePipeline.SendProgress(100, $"Run peak alignment for {allMz.Length} m/z features!")

        Dim dataSet As DataSet() = allMz _
            .AsParallel _
            .Select(Function(mz)
                        Return New DataSet With {
                            .ID = $"MZ_{mz.ToString("F5")}",
                            .Properties = data _
                                .ToDictionary(Function(a) a.Key,
                                              Function(a)
                                                  Return a.Value.alignMz(mz, ppm20)
                                              End Function)
                        }
                    End Function) _
            .ToArray
        Dim file As New StreamWriter(save)

        Call RunSlavePipeline.SendProgress(100, $"Save peaktable!")
        Call file.WriteLine({"MID"}.JoinIterates(data.Keys).JoinBy(","))

        For Each line As DataSet In dataSet
            Call file.WriteLine({line.ID}.JoinIterates(line(data.Keys).Select(Function(d) d.ToString)).JoinBy(","))
        Next

        Call file.Flush()
    End Sub

    <Extension>
    Public Function alignMz(data As ms2(), mz As Double, tolerance As Tolerance) As Double
        Return data _
            .Where(Function(i) tolerance(mz, i.mz)) _
            .OrderByDescending(Function(a) a.intensity) _
            .Select(Function(a) a.intensity) _
            .FirstOrDefault
    End Function

    <ExportAPI("biodeep.session")>
    Public Sub SetBioDeepSession(ssid As String)
        SingletonHolder(Of BioDeepSession).Instance.ssid = ssid
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw">the file path of *.mzpack</param>
    ''' <param name="outputdir"></param>
    <ExportAPI("metaDNA")>
    Public Sub MetaDNASearch(raw As String, outputdir As String)
        Dim metaDNA As New Algorithm(Tolerance.PPM(20), 0.4, Tolerance.DeltaMass(0.3))
        Dim mzpack As mzPack
        Dim range As String()

        Using file As Stream = raw.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            mzpack = mzPack.ReadAll(file)
        End Using

        Dim ionMode As Integer = mzpack.MS _
            .Select(Function(a) a.products) _
            .IteratesALL _
            .First _
            .polarity

        If ionMode = 1 Then
            range = {"[M]+", "[M+H]+"}
        Else
            range = {"[M]-", "[M-H]-"}
        End If

        Dim println As Action(Of String) = AddressOf RunSlavePipeline.SendMessage
        Dim infer As CandidateInfer() = metaDNA _
            .SetSearchRange(range) _
            .SetNetwork(KEGGRepo.RequestKEGGReactions(println)) _
            .SetKeggLibrary(KEGGRepo.RequestKEGGcompounds(println)) _
            .SetSamples(mzpack.GetMs2Peaks, autoROIid:=True) _
            .SetReportHandler(println) _
            .DIASearch() _
            .ToArray

        Dim output As MetaDNAResult() = metaDNA _
            .ExportTable(infer, unique:=True) _
            .ToArray

        Call output.SaveTo($"{outputdir}/metaDNA_annotation.csv")
        Call infer.GetJson.SaveTo($"{outputdir}/infer_network.json")
    End Sub

    ''' <summary>
    ''' convert any kind of raw data file as mzPack
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="cacheFile"></param>
    <ExportAPI("cache.mzpack")>
    Public Sub CreateMzpack(raw As String, cacheFile As String)
        Dim mzpack As mzPack

        If raw.ExtensionSuffix("raw") Then
            Using msraw As New MSFileReader(raw)
                mzpack = msraw.LoadFromXRaw(AddressOf RunSlavePipeline.SendMessage)
            End Using
        Else
            mzpack = Converter.LoadRawFileAuto(raw, "ppm:20", , AddressOf RunSlavePipeline.SendMessage)
        End If

        If Not mzpack.MS.IsNullOrEmpty Then
            RunSlavePipeline.SendMessage("Create snapshot...")
            mzpack.Thumbnail = mzpack.DrawScatter
        End If

        Using file As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call RunSlavePipeline.SendMessage("Write mzPack cache data...")
            Call mzpack.Write(file)
        End Using

        Call Thread.Sleep(1500)
        Call RunSlavePipeline.SendMessage("Job Done!")
    End Sub

    ''' <summary>
    ''' convert imzML to mzpack
    ''' </summary>
    ''' <param name="imzML"></param>
    ''' <param name="cacheFile"></param>
    <ExportAPI("cache.MSI")>
    Public Sub CreateMSIIndex(imzML As String, cacheFile As String)
        RunSlavePipeline.SendProgress(0, "Create workspace cache file, wait for a while...")

        Dim mzpack As mzPack = Converter.LoadimzML(imzML, AddressOf RunSlavePipeline.SendProgress)

        Call RunSlavePipeline.SendProgress(100, "build pixels index...")

        Try
            Using temp As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True)
                Call mzpack.Write(temp)
            End Using
        Catch ex As Exception
        Finally
            Call RunSlavePipeline.SendProgress(100, "Job done!")
        End Try
    End Sub

    <ExportAPI("MSI_rowbind")>
    Public Sub MSI_rowbind(files As String(), save As String)
        Dim exttype As String() = files.Select(Function(path) path.ExtensionSuffix.ToLower).Distinct.ToArray
        Dim combineMzPack As Func(Of IEnumerable(Of mzPack), Correction, mzPack) =
            Function(pip, cor)
                Return pip.MSICombineRowScans(cor, 0.05, progress:=AddressOf RunSlavePipeline.SendMessage)
            End Function

        If exttype.Length > 1 Then
            Call RunSlavePipeline.SendMessage($"Multipe file type is not allowed!")
            Return
        End If

        Using file As FileStream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Select Case exttype(Scan0)
                Case "raw"
                    Dim loadXRaw = Iterator Function() As IEnumerable(Of MSFileReader)
                                       For Each path As String In files
                                           If path.FileExists Then
                                               Using raw As New MSFileReader(path)
                                                   Yield raw
                                               End Using

                                               Call RunSlavePipeline.SendMessage($"Measuring MSI Information... {path.BaseName}")
                                           Else
                                               Call RunSlavePipeline.SendMessage($"Missing file in path: '{path}'!")
                                           End If
                                       Next
                                   End Function
                    Dim correction As Correction = MSIMeasurement.Measure(loadXRaw()).GetCorrection

                    Call combineMzPack(
                        Iterator Function() As IEnumerable(Of mzPack)
                            Dim i As i32 = 0

                            For Each path As String In files
                                Dim raw As New MSFileReader(path)
                                Dim cache As mzPack = raw.LoadFromXRaw

                                Yield cache

                                Try
                                    raw.Dispose()
                                Catch ex As Exception
                                Finally
                                    Call RunSlavePipeline.SendProgress(CInt((++i / files.Length) * 100), $"Combine Raw Data Files... {path.BaseName}")
                                End Try
                            Next
                        End Function(), correction).Write(file)

                Case "mzpack"

                    Dim loadRaw = Iterator Function() As IEnumerable(Of BinaryStreamReader)
                                      For Each path As String In files
                                          Using bin As New BinaryStreamReader(path)
                                              Yield bin
                                          End Using

                                          Call RunSlavePipeline.SendProgress(0, $"Measuring MSI Information... {path.BaseName}")
                                      Next
                                  End Function
                    Dim correction As Correction = MSIMeasurement.Measure(loadRaw()).GetCorrection

                    Call combineMzPack(
                        Iterator Function() As IEnumerable(Of mzPack)
                            Dim i As i32 = 0

                            For Each path As String In files
                                Using buffer As Stream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                                    Call RunSlavePipeline.SendProgress(CInt((++i / files.Length) * 100), $"Combine Raw Data Files... {path.BaseName}")
                                    Yield mzPack.ReadAll(buffer, ignoreThumbnail:=True)
                                End Using
                            Next
                        End Function(), correction).Write(file)

                Case Else
                    Call RunSlavePipeline.SendMessage($"Unsupported file type: {exttype(Scan0)}!")
            End Select
        End Using
    End Sub

    <ExportAPI("formula")>
    Public Sub formulaSearch()

    End Sub

    <ExportAPI("Ms1Contour")>
    Public Sub DrawMs1Contour(mzpackFile As String, cache As String)
        Dim ms1 As ms1_scan() = mzPack _
            .Read(mzpackFile, ignoreThumbnail:=True).MS _
            .GetMs1Points() _
            .GroupBy(Tolerance.DeltaMass(1.125)) _
            .AsParallel _
            .Select(Function(mz)
                        Return mz _
                            .GroupBy(Function(t)
                                         Return t.scan_time
                                     End Function,
                                     Function(a, b)
                                         Return stdNum.Abs(a - b) <= 5
                                     End Function) _
                            .Select(Function(p)
                                        Return New ms1_scan With {
                                            .mz = Val(mz.name),
                                            .intensity = p.Select(Function(t) t.intensity).Average,
                                            .scan_time = Val(p.name)
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
        Dim data As MeasureData() = ms1 _
            .Select(Function(p)
                        Return New MeasureData(p.scan_time, p.mz, If(p.intensity <= 1, 0, stdNum.Log(p.intensity)))
                    End Function) _
            .ToArray
        Dim layers = ContourLayer _
            .GetContours(data, interpolateFill:=False) _
            .Select(Function(g) g.GetContour) _
            .ToArray

        Call layers.GetJson.SaveTo(cache)
    End Sub

End Module
