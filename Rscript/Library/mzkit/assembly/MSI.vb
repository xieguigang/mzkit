
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports imzML = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.XML

<Package("MSI")>
Module MSI

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
    Public Function rowScans(raw As String, y As Integer, Optional env As Environment = Nothing) As Object

    End Function
End Module
