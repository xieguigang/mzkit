Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache

Public Module Converter

    ''' <summary>
    ''' A unify method for load mzpack data from mzXML/mzML raw data file
    ''' </summary>
    ''' <param name="xml">the file path of the raw mzXML/mzML data file.</param>
    ''' <returns></returns>
    Public Function LoadRawFileAuto(xml As String) As mzPack
        If xml.ExtensionSuffix("mzXML") Then
            Return New mzPack With {
                .MS = New mzXMLScans().Load(xml).ToArray
            }
        ElseIf xml.ExtensionSuffix("mzML") Then

        End If
    End Function
End Module
