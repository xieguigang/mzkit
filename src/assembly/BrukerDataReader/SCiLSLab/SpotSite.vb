Imports System.Collections.Generic
Imports System.Drawing

Namespace SCiLSLab

    Public Class SpotPack

        Public Property index As Dictionary(Of String, SpotSite)
        Public Property comment As String

    End Class

    Public Class SpotSite

        Public Property index As String
        Public Property x As Double
        Public Property y As Double

        Public Overrides Function ToString() As String
            Return $"spot_{index} [{x},{y}]"
        End Function

    End Class
End Namespace