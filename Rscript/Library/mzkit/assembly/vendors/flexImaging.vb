
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader
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
                                Optional env As Environment = Nothing) As mzPack

        Dim println = env.WriteLineHandler

        Using spotFile As Stream = spots.Open(FileMode.Open, doClear:=False, [readOnly]:=True),
            spectraFile As Stream = spectrum.Open(FileMode.Open, doClear:=False, [readOnly]:=True)

            Return LoadMSIFromSCiLSLab(spotFile, spectraFile, Sub(txt) println(txt))
        End Using
    End Function
End Module
