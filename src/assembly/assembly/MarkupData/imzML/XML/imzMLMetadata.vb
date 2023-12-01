
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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
        Public Property guid As String
        Public Property format As Format
        Public Property ibd_checksum As String
        Public Property sourcefiles As String()
        ''' <summary>
        ''' software name => version
        ''' </summary>
        ''' <returns></returns>
        Public Property softwares As NamedValue(Of String)()

#Region "ms-imaging specific metadata"

        Public Property direction1 As String
        Public Property direction2 As String
        Public Property dims As Size
        Public Property physical_size As Size
        Public Property resolution As Size

#End Region

#Region "ms instrument"
        Public Property ms_instrument As String
        Public Property ms_source As String
        Public Property ms_analyzer As String
        Public Property ms_detector As String
#End Region

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="imzml">the file path to the target imzml file</param>
        ''' <returns></returns>
        Public Shared Function ReadHeaders(imzml As String) As imzMLMetadata

        End Function
    End Class
End Namespace