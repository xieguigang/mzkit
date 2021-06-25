Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("task")>
Public Class TaskScript

    <ExportAPI("cache.MSI")>
    Public Sub CreateMSIIndex(imzML As String, cacheFile As String)

    End Sub

End Class
