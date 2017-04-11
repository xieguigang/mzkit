Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace DATA

    Public Class MASS_SPECTROMETRY

        Public Property MS_TYPE As String
        Public Property ION_MODE As String
        Public Property COLLISION_ENERGY As String
        Public Property DATAFORMAT As String
        Public Property DESOLVATION_GAS_FLOW As String
        Public Property DESOLVATION_TEMPERATURE As String
        Public Property FRAGMENTATION_METHOD As String
        Public Property IONIZATION As String
        Public Property SCANNING As String
        Public Property SOURCE_TEMPERATURE As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class CHROMATOGRAPHY

        Public Property COLUMN_NAME As String
        Public Property COLUMN_TEMPERATURE As String
        Public Property FLOW_GRADIENT As String
        Public Property FLOW_RATE As String
        Public Property RETENTION_TIME As String
        Public Property SAMPLING_CONE As String
        Public Property SOLVENT As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class

    Public Class FOCUSED_ION

        Public Property DERIVATIVE_FORM As String
        Public Property DERIVATIVE_MASS As String
        Public Property DERIVATIVE_TYPE As String
        <Column("PRECURSOR_M/Z")>
        Public Property PRECURSOR_MZ As String
        Public Property PRECURSOR_TYPE As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace