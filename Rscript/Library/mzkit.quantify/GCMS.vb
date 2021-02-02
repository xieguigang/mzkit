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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Rlist = SMRUCC.Rsharp.Runtime.Internal.Object.list

<Package("mzkit.gcms")>
Module GCMSLinear

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
                                 Optional env As Environment = Nothing) As Object

        Dim MSL As pipeline = pipeline.TryCreatePipeline(Of MSLIon)(ions, env)

        If MSL.isError Then
            Return MSL.getError
        End If

        If TypeOf contentVector Is Rlist Then
            Dim vec As Dictionary(Of String, Double) = DirectCast(contentVector, Rlist).AsGeneric(Of Double)(env)
            Dim table As ContentTable = GCMS.ContentTable(MSL.populates(Of MSLIon)(env), vec)

            Return table
        Else
            Return Message.InCompatibleType(GetType(Rlist), contentVector.GetType, env)
        End If
    End Function
End Module
