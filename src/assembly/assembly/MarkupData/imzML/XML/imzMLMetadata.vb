
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.imzML

    ''' <summary>
    ''' the MS-imaging metadata that read from the imzML header data
    ''' </summary>
    Public Class imzMLMetadata

        ''' <summary>
        ''' a set of the control vocabulary that used in current dataset
        ''' </summary>
        ''' <returns></returns>
        Public Property cv As cvList



        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="imzml">the file path to the target imzml file</param>
        ''' <returns></returns>
        Public Shared Function ReadHeaders(imzml As String) As imzMLMetadata

        End Function
    End Class
End Namespace