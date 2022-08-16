
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader.SCiLSLab
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("flexImaging")>
Module flexImaging

    <ExportAPI("read.metadata")>
    Public Function ReadMetadata(mcf As String) As Object
        Dim data = Storage.GetMetaData(mcf).ToArray
        Dim meta As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        meta.add("metadata", data.Select(Function(a) a.Name))
        meta.add("data", data.Select(Function(a) a.Value))
        meta.add("type", data.Select(Function(a) a.Description.Split("|"c).First))
        meta.add("information", data.Select(Function(a) a.Description.GetTagValue("|").Value))

        Return meta
    End Function

    <ExportAPI("importSpotList")>
    Public Function ImportSpots(spots As String,
                                spectrum As String,
                                Optional scale As Boolean = True,
                                Optional env As Environment = Nothing) As mzPack

        Dim println = env.WriteLineHandler

        Using spotFile As Stream = spots.Open(FileMode.Open, doClear:=False, [readOnly]:=True),
            spectraFile As Stream = spectrum.Open(FileMode.Open, doClear:=False, [readOnly]:=True)

            Dim data As mzPack = LoadMSIFromSCiLSLab(spotFile, spectraFile, Sub(txt) println(txt))

            If scale Then
                data = data.ScalePixels
            End If

            Return data
        End Using
    End Function

    <ExportAPI("importsExperiment")>
    Public Function ImportsExperiment(scans As IEnumerable(Of String), Optional env As Environment = Nothing) As mzPack
        Dim tuplefiles = scans.CheckSpotFiles.ToArray
        Dim println = env.WriteLineHandler
        Dim data As mzPack = MSIRawPack.LoadMSIFromSCiLSLab(tuplefiles, Sub(txt) println(txt))

        Return data
    End Function
End Module
