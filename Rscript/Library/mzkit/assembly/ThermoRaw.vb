Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("ThermoRaw")>
Module ThermoRaw

    ''' <summary>
    ''' open a Thermo raw file
    ''' </summary>
    ''' <param name="rawfile">the file path of the ``*.raw``.</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("open.raw")>
    Public Function OpenRaw(rawfile As String) As MSFileReader
        Return New MSFileReader(rawfile)
    End Function

    <ExportAPI("read.rawscan")>
    Public Function readRawScan(raw As MSFileReader, scanId As Integer) As SingleScanInfo
        Return raw.GetScanInfo(scanId)
    End Function

    <ExportAPI("events")>
    Public Function events(scan As SingleScanInfo) As dataframe
        Dim key As Array = scan.ScanEvents.Select(Function(evt) evt.Key).ToArray
        Dim evts As Array = scan.ScanEvents.Select(Function(evt) evt.Value).ToArray

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"event", key},
                {"data", evts}
            }
        }
    End Function

    <ExportAPI("logs")>
    Public Function logs(scan As SingleScanInfo) As dataframe
        Dim key As Array = scan.StatusLog.Select(Function(evt) evt.Key).ToArray
        Dim evts As Array = scan.StatusLog.Select(Function(evt) evt.Value).ToArray

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"log", key},
                {"text", evts}
            }
        }
    End Function
End Module
