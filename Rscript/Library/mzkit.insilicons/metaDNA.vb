#Region "Microsoft.VisualBasic::50c1e0115ae5e7dad814123d048dc5e3, Library\mzkit.insilicons\metaDNA.vb"

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

' Module metaDNA
' 
'     Function: loadMetaDNAInferNetwork, readReactionClassTable
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports MetaDNA.visual
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports KeggCompound = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound
Imports kegReactionClass = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.ReactionClass
Imports MetaDNAAlgorithm = BioNovoGene.BioDeep.MetaDNA.Algorithm
Imports ReactionClass = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.ReactionClass
Imports ReactionClassTbl = MetaDNA.visual.ReactionClass
Imports REnv = SMRUCC.Rsharp.Runtime.Internal

<Package("metadna")>
<RTypeExport("metadna", GetType(MetaDNAAlgorithm))>
Module metaDNAInfer

    ''' <summary>
    ''' Load network graph model from the kegg metaDNA infer network data.
    ''' </summary>
    ''' <param name="debugOutput"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.metadna.infer")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function loadMetaDNAInferNetwork(debugOutput As Object, Optional env As Environment = Nothing) As Object
        If debugOutput Is Nothing Then
            Return Nothing
        ElseIf debugOutput.GetType Is GetType(String) Then
            debugOutput = DirectCast(debugOutput, String).LoadXml(Of Global.MetaDNA.visual.XML)
        End If

        If Not TypeOf debugOutput Is Global.MetaDNA.visual.XML Then
            Return REnv.debug.stop(New InvalidCastException, env)
        End If

        Return DirectCast(debugOutput, Global.MetaDNA.visual.XML).CreateGraph
    End Function

    ''' <summary>
    ''' load kegg reaction class data in table format from given file
    ''' </summary>
    ''' <param name="file">csv table file or a directory with raw xml model data file in it.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("reaction_class.table")>
    <RApiReturn(GetType(ReactionClassTbl()))>
    Public Function readReactionClassTable(file As String, Optional env As Environment = Nothing) As Object
        If file.ExtensionSuffix("csv") Then
            Return file.LoadCsv(Of ReactionClassTbl).ToArray
        ElseIf file.DirectoryExists Then
            Return kegReactionClass _
                .ScanRepository(file, loadsAll:=True) _
                .Select(Function(cls)
                            Return cls.reactantPairs _
                                .Select(Function(r)
                                            Return New ReactionClassTbl With {
                                                .define = cls.definition,
                                                .from = r.from,
                                                .[to] = r.to,
                                                .rId = cls.entryId,
                                                .category = Integer.Parse(cls.category.Match("\d"))
                                            }
                                        End Function)
                        End Function) _
                .IteratesALL _
                .ToArray
        Else
            Return Internal.debug.stop($"unable to determin the data source type of the given file '{file}'", env)
        End If
    End Function

#Region "metadna algorithm"

    <ExportAPI("metadna")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function MetaDNAAlgorithm(Optional ms1ppm As Object = "ppm:20",
                                     Optional mzwidth As Object = "da:0.3",
                                     Optional dotcutoff As Double = 0.5,
                                     Optional env As Environment = Nothing) As Object

        Dim ms1Err As [Variant](Of Tolerance, Message) = Math.getTolerance(ms1ppm, env)
        Dim mz2Err As [Variant](Of Tolerance, Message) = Math.getTolerance(mzwidth, env)

        If ms1Err Like GetType(Message) Then
            Return ms1Err.TryCast(Of Message)
        ElseIf mz2Err Like GetType(Message) Then
            Return mz2Err.TryCast(Of Message)
        End If

        Return New MetaDNAAlgorithm(ms1Err, dotcutoff, mz2Err)
    End Function

    <ExportAPI("range")>
    Public Function SetSearchRange(metadna As MetaDNAAlgorithm, precursorTypes As String()) As MetaDNAAlgorithm
        Return metadna.SetSearchRange(precursorTypes)
    End Function

    <ExportAPI("load.kegg")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function SetKeggLibrary(metadna As MetaDNAAlgorithm, <RRawVectorArgument> kegg As Object, Optional env As Environment = Nothing) As Object
        Dim library As pipeline = pipeline.TryCreatePipeline(Of KeggCompound)(kegg, env)

        If library.isError Then
            Return library.getError
        End If

        Return metadna.SetKeggLibrary(library.populates(Of KeggCompound)(env))
    End Function

    <ExportAPI("load.kegg_network")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function SetInferNetwork(metadna As MetaDNAAlgorithm, <RRawVectorArgument> links As Object, Optional env As Environment = Nothing) As Object
        Dim network As pipeline = pipeline.TryCreatePipeline(Of ReactionClass)(links, env)

        If network.isError Then
            Return network.getError
        End If

        Return metadna.SetNetwork(network.populates(Of ReactionClass)(env))
    End Function

    <ExportAPI("load.raw")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function handleSample(metadna As MetaDNAAlgorithm, <RRawVectorArgument> sample As Object, Optional env As Environment = Nothing) As Object
        Dim raw As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(sample, env)

        If raw.isError Then
            Return raw.getError
        End If

        Return metadna.SetSamples(raw.populates(Of PeakMs2)(env))
    End Function

    <ExportAPI("DIA.infer")>
    <RApiReturn(GetType(CandidateInfer))>
    Public Function DIAInfer(metaDNA As MetaDNAAlgorithm, <RRawVectorArgument> sample As Object, Optional env As Environment = Nothing) As Object
        Dim raw As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(sample, env)

        If raw.isError Then
            Return raw.getError
        End If

        Dim infer As CandidateInfer() = metaDNA _
            .SetSamples(raw.populates(Of PeakMs2)(env)) _
            .DIASearch _
            .ToArray

        Return infer
    End Function

#End Region

#Region "kegg"

    <ExportAPI("kegg.library")>
    <RApiReturn(GetType(KeggCompound))>
    Public Function loadCompoundLibrary(repo As String) As Object
        Return CompoundRepository.ScanRepository(repo, ignoreGlycan:=False).DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("kegg.network")>
    <RApiReturn(GetType(ReactionClass))>
    Public Function loadKeggNetwork(repo As String) As Object
        Return ReactionClass.ScanRepository(repo).DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

#End Region

End Module
