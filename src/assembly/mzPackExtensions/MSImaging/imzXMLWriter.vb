Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache

Public Module imzXMLWriter

    ''' <summary>
    ''' write mzpack file data as imzML file
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="output"></param>
    ''' <returns></returns>
    Public Function WriteXML(mzpack As mzPack, output As String) As Boolean
        Dim writer As imzML.mzPackWriter = imzML.mzPackWriter.OpenOutput(output)
        Dim dimsize As Size

        ' config of the writer
        Call writer _
            .SetMSImagingParameters(dimsize, 17) _
            .SetSpectrumParameters(1) _
            .SetSourceLocation(mzpack.source)

        For Each scan As ScanMS1 In mzpack.MS
            Call writer.WriteScan(scan)
        Next

        Call writer.Dispose()

        Return True
    End Function
End Module
