#Region "Microsoft.VisualBasic::9a4759518e4a91550b6a521216b8437c, MetaDNA\KEGG_MetaDNA\Program.vb"

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

    ' Module Program
    ' 
    '     Function: BuildNetwork, Compiler, KEGGMeta, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports GenericEquation = SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes.Equation
Imports R_server = RDotNet.Extensions.VisualBasic.RSystem
Imports Rbase = RDotNet.Extensions.VisualBasic.API.base
Imports VisualBasic = Microsoft.VisualBasic.Language.Runtime

Module Program

    ''' <summary>
    ''' 生成MetaDNA进行计算所需要的数据包
    ''' </summary>
    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    ''' <summary>
    ''' 创建一个KEGG代谢物基本信息数据集
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KEGG.meta")>
    <Usage("/KEGG.meta /kegg <compounds.repo.directory> [/out <default=KEGG_meta.rda>]")>
    <Description("Compile a KEGG compound basic information database.")>
    Public Function KEGGMeta(args As CommandLine) As Integer
        Dim kegg$ = args <= "/kegg"
        Dim out$ = args("/out") Or $"{kegg.TrimDIR}/KEGG_meta.rda"
        Dim compounds = ScanLoad(repository:=kegg) _
            .GroupBy(Function(c) c.Entry) _
            .Select(Function(g) g.First) _
            .ToArray

        SyncLock R_server.R
            With R_server.R

                !KEGG_meta = Rbase.lapply(
                    x:=compounds,
                    FUN:=Function(compound)
                             Call compound.CommonNames?.FirstOrDefault().__DEBUG_ECHO

                             Return Rbase.list(
                                !ID = compound.entry,
                                !exact_mass = compound.exactMass,
                                !name = compound.commonNames,
                                !formula = compound.formula
                             )
                         End Function)

                Call out.__DEBUG_ECHO
                Call Rbase.save({"KEGG_meta"}, file:=out)
            End With
        End SyncLock

        Return 0
    End Function

    ''' <summary>
    ''' 这个结构所产生的数据库是MetaDNA R程序包所需要的data数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/compile")>
    <Usage("/compile /br08201 <reactions.repo.directory> /KEGG_cpd <compounds.repo.directory> [/out <default=metaDNA_kegg.rda>]")>
    <Description("Compile the kegg reaction/compound database as MetaDNA network database into RDA format.")>
    <Argument("/br08201", False, CLITypes.File,
              AcceptTypes:={GetType(Reaction)},
              Description:="The kegg reaction xml data model database repository directory.")>
    <Argument("/KEGG_cpd", False, CLITypes.File,
              AcceptTypes:={GetType(Compound)},
              Description:="The kegg compound xml data model database repository directory.")>
    <Argument("/out", True, CLITypes.File, Description:="The file save location of the rda database file.")>
    Public Function Compiler(args As CommandLine) As Integer
        Return (args <= "/br08201", args <= "/KEGG_cpd") _
            .BuildNetwork(args("/out") Or $"metaDNA_kegg.rda") _
            .CLICode
    End Function

    ReadOnly NA As New [Default](Of String)("NA", Function(exp) CStr(exp).StringEmpty OrElse CStr(exp) = "NULL")

    <Extension> Private Function BuildNetwork(repository As (reaction$, compound$), rda$) As Boolean
        Dim reactions = (ls - l - r - "*.Xml" <= repository.reaction) _
            .Select(Function(path)
                        Return path.LoadXml(Of Reaction)(stripInvalidsCharacter:=True)
                    End Function)
        Dim compounds = ScanLoad(repository.compound) _
            .GroupBy(Function(c) c.Entry) _
            .ToDictionary(Function(c) c.Key,
                          Function(g)
                              Return g.First
                          End Function)

        SyncLock R_server.R
            With R_server.R

                ' network <- list(rxnID, define, reactants, products);
                ' reactants和products都是KEGG的代谢物编号
                Dim network$ = Rbase.list()

                With New VisualBasic

                    network = Rbase.lapply(
                        x:=reactions,
                        FUN:=Function(reaction)
                                 Dim model As GenericEquation = reaction.ReactionModel
                                 Dim name$ = Rbase.c(reaction.CommonNames, stringVector:=True) Or NA

                                 ' 生成一个列表之中的反应过程的摘要模型
                                 ' 相当于网络之中的一个节点
                                 Return Rbase.list(
                                     !rxnID = reaction.ID,
                                     !name = (name),
                                     !define = reaction.Definition,
                                     !reactants = Rbase.c(model.Reactants.Keys, stringVector:=True),
                                     !products = Rbase.c(model.Products.Keys, stringVector:=True)
                                 )
                             End Function)
                End With

                !network = network

                Call Rbase.save({"network"}, rda)
            End With
        End SyncLock

        Return rda.FileLength > 0
    End Function
End Module
