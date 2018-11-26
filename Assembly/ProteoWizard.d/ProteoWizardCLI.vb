#Region "Microsoft.VisualBasic::5d4713a6eb9212eb07c4c829d0c22a04, ProteoWizard.d\ProteoWizardCLI.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Class ProteoWizardCLI
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Convert2mzML
    ' 
    ' /********************************************************************************/

#End Region

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

