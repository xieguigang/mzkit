Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Settings

    Public Property precursor_search As PrecursorSearchSettings

    Public Shared ReadOnly Property configFile As String = App.LocalData & "/settings.json"

    Public Shared Function DefaultProfile() As Settings
        Return New Settings With {
            .precursor_search = New PrecursorSearchSettings With {
                .ppm = 5, .precursor_types = {"M", "M+H", "M-H", "M+H-H2O", "M-H2O-H"}
            }
        }
    End Function

    Public Shared Function GetConfiguration() As Settings
        Dim config As Settings = configFile.LoadJsonFile(Of Settings)

        If config Is Nothing Then
            config = DefaultProfile()
        End If

        Return config
    End Function

    Public Shared Function Save(config As Settings) As Boolean
        Return config.GetJson.SaveTo(configFile)
    End Function
End Class
