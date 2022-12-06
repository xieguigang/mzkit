Public Module Utils

    Public Function SolveTagSource(fileName As String) As String
        fileName = fileName.BaseName

        Do While True
            If fileName.StringEmpty Then
                Return ""
            End If
            If fileName.ExtensionSuffix("mzml", "mzxml", "t2d", "txt", "mgf", "msp", "csv", "xls", "mzpack") Then
                fileName = fileName.BaseName
            Else
                Return fileName
            End If
        Loop

        Return fileName
    End Function
End Module
