Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder

Public Class ProteoWizardCLI : Inherits InteropService

    Sub New(bin As String)
        Me._executableAssembly = bin
    End Sub

    Public Function Convert2mzML(input$, output$) As String
        Dim std$ = ""

        If Strings.LCase(input).EndsWith(".raw.zip") Then
            For Each part In MassWolf.SplitDirectory(waters:=input)
                Dim args$ = New ScriptBuilder(part.In.GetFullPath.CLIPath) +
                    " " +
                    "--mz64" +
                    "--mzML" +
                    "--zlib" +
                    "--filter" +
                    """msLevel 1-2""" +
                    "--ignoreUnknownInstrumentError" +
                   $"-o {output.GetDirectoryFullPath.CLIPath}"

                Call part.Out.__INFO_ECHO
                Call args.SetValue(args.TrimNewLine(" "))

                Dim proc = Me.RunProgram(args,)
                proc.Run()
                std = std & vbCrLf & proc.StandardOutput

                ' cleanup filesystem for avoid file system crash
                Try
                    Call FileIO.FileSystem.DeleteDirectory(part.In.GetFullPath, DeleteDirectoryOption.DeleteAllContents)
                Catch ex As Exception

                End Try
            Next
        Else
            Dim args$ = New ScriptBuilder(input.GetFullPath.CLIPath) +
                " " +
                "--mz64" +
                "--mzML" +
                "--zlib" +
                "--filter" +
                """msLevel 1-2""" +
                "--ignoreUnknownInstrumentError" +
               $"-o {output.GetDirectoryFullPath.CLIPath}"

            Call input.__INFO_ECHO
            Call args.SetValue(args.TrimNewLine(" "))

            Dim proc = Me.RunProgram(args, )
            proc.Run()
            std = proc.StandardOutput
        End If

        Return std
    End Function
End Class
