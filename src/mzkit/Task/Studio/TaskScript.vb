#Region "Microsoft.VisualBasic::e479175900d51f52deb4b6a4b99d6706, src\mzkit\Task\Studio\TaskScript.vb"

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

' Module TaskScript
' 
'     Sub: CreateMSIIndex, CreateMzpack, MetaDNASearch, SetBioDeepSession
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Threading
Imports BioDeep
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports stdNum = System.Math

<Package("task")>
Module TaskScript

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

    <ExportAPI("cache.mzpack")>
    Public Sub CreateMzpack(raw As String, cacheFile As String)
        Dim mzpack As mzPack = Converter.LoadRawFileAuto(raw, AddressOf RunSlavePipeline.SendMessage)

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
                Return pip.MSICombineRowScans(cor, 0.05, AddressOf RunSlavePipeline.SendMessage)
            End Function

        If exttype.Length = 1 Then
            Using file As FileStream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Dim scans As New List(Of Integer)
                Dim maxrt As New List(Of Double)

                Select Case exttype(Scan0)
                    Case "raw"

                        For Each path As String In files
                            Dim raw As New MSFileReader(path)

                            scans.Add(raw.ThermoReader.GetNumScans)
                            maxrt.Add(raw.ScanTimeMax)
                            raw.Dispose()
                        Next

                        Call combineMzPack(
                           Iterator Function() As IEnumerable(Of mzPack)
                               For Each path As String In files
                                   Dim raw As New MSFileReader(path)
                                   Dim cache As mzPack = raw.LoadFromXRaw

                                   Try
                                       raw.Dispose()
                                   Catch ex As Exception

                                   End Try
                               Next
                           End Function(), New Correction(maxrt.Average, scans.Average)).Write(file)

                    Case "mzpack"

                        For Each path As String In files
                            Using bin As New BinaryStreamReader(path)
                                scans.Add(bin.EnumerateIndex.Count)
                                maxrt.Add(bin.rtmax)
                            End Using
                        Next

                        Call combineMzPack(
                            Iterator Function() As IEnumerable(Of mzPack)
                                For Each path As String In files
                                    Using buffer As Stream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                                        Yield mzPack.ReadAll(buffer, ignoreThumbnail:=True)
                                    End Using
                                Next
                            End Function(), New Correction(maxrt.Average, scans.Average)).Write(file)

                    Case Else
                        Call RunSlavePipeline.SendMessage($"Unsupported file type: {exttype(Scan0)}!")
                End Select
            End Using
        Else
            Call RunSlavePipeline.SendMessage($"Multipe file type is not allowed!")
        End If
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
            .GetContours(data) _
            .Select(Function(g) g.GetContour) _
            .ToArray

        Call layers.GetJson.SaveTo(cache)
    End Sub

End Module

