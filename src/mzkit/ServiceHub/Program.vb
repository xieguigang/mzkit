#Region "Microsoft.VisualBasic::65fff0892760f280189711acf6979461, mzkit\src\mzkit\ServiceHub\Program.vb"

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

'   Total Lines: 24
'    Code Lines: 13
' Comment Lines: 7
'   Blank Lines: 4
'     File Size: 598.00 B


' Module Program
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Net.Tcp

<Package("app")>
Module Program

    <ExportAPI("listen.heartbeat")>
    Public Sub ListenHeartBeat(port As Integer)
        Call HeartBeat.Start(port)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="service"></param>
    ''' <param name="debugPort">
    ''' --debug 33361
    ''' </param>
    <ExportAPI("run")>
    Public Sub Main(Optional service As String = "MS-Imaging", Optional debugPort As Integer? = Nothing)
        Select Case service.ToLower
            Case "ms-imaging"
                Call New MSI(debugPort).Run()
            Case Else

        End Select
    End Sub

    <ExportAPI("getMSIData")>
    <RApiReturn(GetType(PixelData))>
    Public Function getData(MSI_service As Integer, mz As Double(), mzdiff As Object, Optional env As Environment = Nothing) As Object
        Dim mzErr = Math.getTolerance(mzdiff, env, [default]:="da:0.1")

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim tcpRequest =
            Function(request As RequestStream) As RequestStream
                Return New TcpRequest("localhost", MSI_service).SendMessage(request)
            End Function

        Return MSIProtocols.LoadPixels(mz, mzErr.TryCast(Of Tolerance), tcpRequest)
    End Function

End Module
