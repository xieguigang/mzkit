#Region "Microsoft.VisualBasic::1887377362559c382b1ee13475c90355, Rscript\Library\mzkit\MetaDbXref.vb"

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

' Module MetaDbXref
' 
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop


''' <summary>
''' 
''' </summary>
<Package("metadb")>
Module MetaDbXref

    ''' <summary>
    ''' a generic function for handle ms1 search
    ''' </summary>
    ''' <param name="compounds"></param>
    ''' <param name="precursors"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("ms1_handler")>
    Public Function CreateMs1Handler(<RRawVectorArgument> compounds As Object,
                                     <RRawVectorArgument> precursors As Object,
                                     Optional tolerance As Object = "ppm:20",
                                     Optional env As Environment = Nothing) As Object

        Dim mzdiff = getTolerance(tolerance, env)

        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If

        Dim mz1 = GetPrecursorTypes(precursors, env)
        Dim metabolites As pipeline = pipeline.TryCreatePipeline(Of LipidMaps.MetaData)(compounds, env, suppress:=True)

        If Not metabolites.isError Then
            Return MSSearch(Of LipidMaps.MetaData).CreateIndex(metabolites.populates(Of LipidMaps.MetaData)(env), mz1, mzdiff)
        End If

        metabolites = pipeline.TryCreatePipeline(Of Compound)(compounds, env, suppress:=True)

        If Not metabolites.isError Then
            Return KEGGHandler.CreateIndex(metabolites.populates(Of Compound)(env), mz1, mzdiff)
        End If

        Return metabolites.getError
    End Function

End Module
