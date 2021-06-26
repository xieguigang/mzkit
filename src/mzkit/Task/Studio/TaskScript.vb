Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("task")>
Module TaskScript

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
