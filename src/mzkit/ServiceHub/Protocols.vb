#Region "Microsoft.VisualBasic::de663b645e54024d1f24dec77deaad1e, mzkit\src\mzkit\ServiceHub\Protocols.vb"

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


    ' Code Statistics:

    '   Total Lines: 39
    '    Code Lines: 31
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.23 KB


    ' Module Protocols
    ' 
    '     Function: StartServer
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Linq
Imports Task

Public Module Protocols

    Public Function StartServer(Rscript As String, ByRef service As Integer, debugPort As Integer?, Optional heartbeats As Integer? = Nothing) As RunSlavePipeline
        Dim cli As String = If(debugPort Is Nothing, Rscript.CLIPath, $"{Rscript.CLIPath} --debug={debugPort}") ' --heartbeats={heartbeats}
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Rscript.Path, cli)
        Dim tcpPort As Integer = -1

        Call pipeline.CommandLine.__DEBUG_ECHO

        AddHandler pipeline.SetMessage,
            Sub(msg)
                If msg.StartsWith("socket=") Then
                    tcpPort = msg.Match("\d+").DoCall(AddressOf Integer.Parse)
                Else
                    Call msg.__DEBUG_ECHO
                End If
            End Sub

        Call New Thread(AddressOf pipeline.Run).Start()

        For i As Integer = 0 To 1000
            service = tcpPort

            If service > 0 Then
                Exit For
            Else
                Thread.Sleep(500)
            End If
        Next

        Return pipeline
    End Function
End Module
