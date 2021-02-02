#Region "Microsoft.VisualBasic::0b95bef54a7cb5170cd71695d89cad95, Rscript\Library\mzkit.quantify\GCMS.vb"

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

' Module GCMS
' 
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.QuantifyAnalysis
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Rlist = SMRUCC.Rsharp.Runtime.Internal.Object.list
Imports SMRUCC.Rsharp

<Package("GCMS")>
Module GCMSLinear

    <ExportAPI("as.quantify.ion")>
    Public Function quantifyIons(ions As MSLIon(), Optional rtwin As Double = 1) As QuantifyIon()
        Return QuantifyIon.FromIons(ions, rtwin).ToArray
    End Function

    <ExportAPI("SIMIonExtractor")>
    <RApiReturn(GetType(SIMIonExtract))>
    Public Function createSIMIonExtract(ions As QuantifyIon(),
                                        <RRawVectorArgument(GetType(Double))>
                                        Optional peakwidth As Object = "3,5",
                                        Optional centroid As Object = "da:0.3",
                                        Optional env As Environment = Nothing) As Object

        Dim peakwin = GetDoubleRange(peakwidth, env)
        Dim ms1ppm = Math.getTolerance(centroid, env)

        If peakwin Like GetType(Message) Then
            Return peakwin.TryCast(Of Message)
        ElseIf ms1ppm Like GetType(Message) Then
            Return ms1ppm.TryCast(Of Message)
        End If

        Return New SIMIonExtract(ions, peakwin, ms1ppm)
    End Function

    <ExportAPI("parseContents")>
    Public Function FileNames2Contents(<RRawVectorArgument> files As Object) As Rlist
        Dim names As String() = REnv.asVector(Of String)(files)
        Dim vec As Dictionary(Of String, Object) = names _
            .ContentVector _
            .ToDictionary(Function(L) L.Key,
                          Function(L)
                              Return CObj(L.Value)
                          End Function)

        Return New Rlist(RType.GetRSharpType(GetType(Double))) With {
            .slots = vec
        }
    End Function

    <ExportAPI("contentTable")>
    <RApiReturn(GetType(ContentTable))>
    Public Function ContentTable(<RRawVectorArgument> ions As Object,
                                 <RRawVectorArgument> contentVector As Object,
                                 <RRawVectorArgument> Optional [IS] As Object = Nothing,
                                 Optional env As Environment = Nothing) As Object

        Dim MSL As pipeline = pipeline.TryCreatePipeline(Of MSLIon)(ions, env)
        Dim ISMap As Dictionary(Of String, String)

        If MSL.isError Then
            Return MSL.getError
        End If

        Dim MSLIons As MSLIon() = MSL.populates(Of MSLIon)(env).ToArray

        If TypeOf [IS] Is list Then
            ISMap = DirectCast([IS], list).AsGeneric(Of String)(env)
        ElseIf TypeOf [IS] Is String() OrElse TypeOf [IS] Is vector OrElse TypeOf [IS] Is String Then
            Dim ISnames As String() = REnv.asVector(Of String)([IS])

            If ISnames.Length = 1 AndAlso ISnames(Scan0) = "IS" Then
                ISMap = New Dictionary(Of String, String)

                For Each ion As MSLIon In MSLIons
                    ISMap(ion.Name) = "IS"
                Next
            Else
                Return Internal.debug.stop(New NotImplementedException, env)
            End If
        Else
            Return Internal.debug.stop(New NotImplementedException, env)
        End If

        If TypeOf contentVector Is Rlist Then
            Dim vec As Dictionary(Of String, Double) = DirectCast(contentVector, Rlist).AsGeneric(Of Double)(env)
            Dim table As ContentTable = GCMS.ContentTable(MSLIons, vec, ISMap)

            Return table
        Else
            Return Message.InCompatibleType(GetType(Rlist), contentVector.GetType, env)
        End If
    End Function
End Module
