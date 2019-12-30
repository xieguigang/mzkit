#Region "Microsoft.VisualBasic::6d3b225f8279c5c31272cd6e4253c410, Assembly\ProteoWizard.d\ProteoWizardCLI.vb"

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
    ' 
    '     Enum OutFileTypes
    ' 
    ' 
    ' 
    ' 
    '  
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Convert2mzML, convertThermoRawFile, convertWatersRawFile
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder

Public Class ProteoWizardCLI : Inherits InteropService

    Public Enum OutFileTypes
        <Description("--mzXML")> mzXML
        <Description("--mzML")> mzML
    End Enum

    Sub New(bin As String)
        Me._executableAssembly = bin
    End Sub

    Public Function Convert2mzML(input$, output$, Optional type As OutFileTypes = OutFileTypes.mzXML) As String
        If Strings.LCase(input).EndsWith(".raw.zip") Then
            Return convertWatersRawFile(input, output, type)
        Else
            Return convertThermoRawFile(input, output, type)
        End If
    End Function

    Private Function convertThermoRawFile(input$, output$, type As OutFileTypes) As String
        Dim std$ = ""

        Dim args$ = New ScriptBuilder(input.GetFullPath.CLIPath) +
               " " +
               "--mz64" +
               type.Description +
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

        Return std
    End Function

    Private Function convertWatersRawFile(input$, output$, type As OutFileTypes) As String
        Dim std$ = ""

        For Each part In MassWolf.SplitDirectory(waters:=input)
            Dim args$ = New ScriptBuilder(part.In.GetFullPath.CLIPath) +
                " " +
                "--mz64" +
                type.Description +
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
                Call FileSystem.DeleteDirectory(part.In.GetFullPath, DeleteDirectoryOption.DeleteAllContents)
            Catch ex As Exception

            End Try
        Next

        Return std
    End Function
End Class
