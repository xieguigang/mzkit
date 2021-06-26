Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON

<Package("task")>
Module TaskScript

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

    <ExportAPI("cache.MSI")>
    Public Sub CreateMSIIndex(imzML As String, cacheFile As String)
        RunSlavePipeline.SendProgress(0, "Initialize reader...")

        Dim ibd As ibdReader = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        Dim allPixels As ScanData() = XML.LoadScans(imzML).ToArray
        Dim width As Integer = Aggregate p In allPixels Into Max(p.x)
        Dim height As Integer = Aggregate p In allPixels Into Max(p.y)
        Dim cache As New XICWriter(width, height, sourceName:=ibd.fileName Or "n/a".AsDefault)
        Dim i As Integer = 1
        Dim d As Integer = allPixels.Length / 100
        Dim j As i32 = 0

        RunSlavePipeline.SendProgress(0, "Create workspace cache file, wait for a while...")

        For Each pixel As ScanData In allPixels
            Call cache.WritePixels(New ibdPixel(ibd, pixel))

            i += 1

            If ++j = d Then
                j = 0
                RunSlavePipeline.SendProgress(i / allPixels.Length * 100, $"Create workspace cache file, wait for a while... [{i}/{allPixels.Length}]")
            End If
        Next

        Call cache.Flush()
        Call RunSlavePipeline.SendProgress(100, "build pixels index...")

        Try
            Using temp As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True)
                Call XICIndex.WriteIndexFile(cache, temp)
            End Using
        Catch ex As Exception
        Finally
            Call RunSlavePipeline.SendProgress(100, "Job done!")

            Try
                cache.Dispose()
                cache.Clear()
            Catch ex As Exception

            End Try
        End Try
    End Sub

End Module
