Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.IO

Namespace PoolData

    ''' <summary>
    ''' metadata of the spectrum object
    ''' </summary>
    Public Class Metadata : Implements INamedValue

        Public Property guid As String Implements INamedValue.Key
        Public Property mz As Double
        Public Property rt As Double
        Public Property intensity As Double
        Public Property source_file As String

        ''' <summary>
        ''' the spectrum data is store in mzpack <see cref="ScanMs2"/> format
        ''' </summary>
        ''' <returns></returns>
        Public Property block As BufferRegion

        ''' <summary>
        ''' blood, urine, etc
        ''' </summary>
        ''' <returns></returns>
        Public Property sample_source As String
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property organism As String
        Public Property instrument As String = "Thermo Scientific Q Exactive"
        Public Property name As String
        Public Property biodeep_id As String
        Public Property formula As String
        Public Property adducts As String

        Public Overrides Function ToString() As String
            Return $"[{guid}] {name}"
        End Function

    End Class
End Namespace