
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("NMR")>
Module NMR

    <ExportAPI("read.nmrML")>
    Public Function readSmall(file As String) As nmrML.XML
        Return file.LoadXml(Of nmrML.XML)
    End Function

End Module
