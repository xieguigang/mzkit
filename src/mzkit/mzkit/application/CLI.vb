Imports Microsoft.VisualBasic.CommandLine
Imports mzkit.My

Module CLI

    Public Function openRawFile(filepath As String, args As CommandLine) As Integer
        MyApplication.afterLoad =
            Sub()
                Select Case filepath.ExtensionSuffix.ToLower
                    Case "mzpack"
                        Call MyApplication.host.OpenFile(filepath, showDocument:=True)
                End Select
            End Sub

        Return 0
    End Function
End Module
