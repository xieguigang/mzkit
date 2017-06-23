Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace ASCII.MSP

    Public Class MspData

        Public Property Name As String
        Public Property Synonym As String

        <Column("DB#")>
        Public Property DBId As String
        Public Property InChIKey As String
        Public Property MW As Double
        Public Property Formula As String
        Public Property PrecursorMZ As Double
        Public Property Comments As String

        Public ReadOnly Property MetaDB As MetaData
            Get
                Return Comments.FillData
            End Get
        End Property

        Public Property Peaks As MSSignal()

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace