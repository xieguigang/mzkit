
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.nmrML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("NMR")>
Module NMR

    <ExportAPI("read.nmrML")>
    Public Function readSmall(file As String) As nmrML.XML
        Return file.LoadXml(Of nmrML.XML)
    End Function

    ''' <summary>
    ''' get all acquisition data in the raw data file
    ''' </summary>
    ''' <param name="nmrML"></param>
    ''' <returns></returns>
    <ExportAPI("acquisition")>
    Public Function acquisition(nmrML As nmrML.XML) As acquisition()
        Return nmrML.acquisition
    End Function

    ''' <summary>
    ''' Read Free Induction Decay data matrix
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <ExportAPI("FID")>
    Public Function GetMatrix(data As acquisition) As LibraryMatrix
        Return data.ParseMatrix
    End Function

End Module
