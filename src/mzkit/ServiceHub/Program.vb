Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("app")>
Module Program

    <ExportAPI("run")>
    Public Sub Main(Optional service As String = "MS-Imaging", Optional debugPort As Integer? = Nothing)
        Select Case service.ToLower
            Case "ms-imaging"
                Call New MSI(debugPort).Run()
            Case Else

        End Select
    End Sub

End Module
