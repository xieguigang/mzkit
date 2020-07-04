Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Signal
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' helper package module for read ``electromagnetic radiation spectrum`` data
''' </summary>
''' 
<Package("mzML.ERS")>
Module ERS

    ''' <summary>
    ''' Get photodiode array detector instrument configuration id
    ''' </summary>
    ''' <param name="mzml"></param>
    ''' <returns></returns>
    <ExportAPI("get_instrument")>
    Public Function GetPhotodiodeArrayDetectorInstrumentConfigurationId(mzml As String) As String
        Return ExtractUVData.GetPhotodiodeArrayDetectorInstrumentConfigurationId(rawdata:=mzml)
    End Function

    <ExportAPI("extract_UVsignals")>
    <RApiReturn(GetType(GeneralSignal))>
    Public Function ExtractERSUVData(<RRawVectorArgument> rawscans As Object, instrumentId As String, Optional env As Environment = Nothing) As Object
        Dim raw As pipeline = pipeline.TryCreatePipeline(Of spectrum)(rawscans, env)

        If raw.isError Then
            Return raw.getError
        End If

        Return raw _
            .populates(Of spectrum)(env) _
            .PopulatesElectromagneticRadiationSpectrum(instrumentId) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("write.UVsignals")>
    <RApiReturn(GetType(Boolean))>
    Public Function WriteSignal(<RRawVectorArgument> signals As Object, file$, Optional env As Environment = Nothing) As Object
        Dim raw As pipeline = pipeline.TryCreatePipeline(Of GeneralSignal)(signals, env)

        If raw.isError Then
            Return raw.getError
        End If

        If file.ExtensionSuffix("cdf", "netcdf", "nc") Then
            Call raw.populates(Of GeneralSignal)(env).WriteCDF(file, "electromagnetic radiation spectrum ERS_UVsignal")
        Else
            ' write text
            Using writer As StreamWriter = file.OpenWriter
                For Each scan As GeneralSignal In raw.populates(Of GeneralSignal)(env)
                    Call writer.WriteLine(scan.GetText)
                Next
            End Using
        End If

        Return True
    End Function
End Module
