﻿#Region "Microsoft.VisualBasic::d8e4f87b23d3892d2ce3a6fe059288fb, mzkit\src\metadna\KEGG_MetaDNA\Program.vb"

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

    '   Total Lines: 212
    '    Code Lines: 148
    ' Comment Lines: 34
    '   Blank Lines: 30
    '     File Size: 8.84 KB


    ' Module Program
    ' 
    '     Function: BuildNetwork, Compiler, KEGGMeta, Main, precursorMzMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports R_server = RDotNet.Extensions.VisualBasic.RSystem
Imports Rbase = RDotNet.Extensions.VisualBasic.API.base
Imports RSymbol = RDotNet.Extensions.VisualBasic.var
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
    <Argument("/kegg", False, CLITypes.File,
              AcceptTypes:={GetType(Compound)},
              Description:="The kegg compound xml data model database repository directory.")>
    Public Function KEGGMeta(args As CommandLine) As Integer
        Dim kegg$ = args <= "/kegg"
        Dim out$ = args("/out") Or $"{kegg.TrimDIR}/KEGG_meta.rda"
        Dim compounds = ScanLoad(repository:=kegg) _
            .Where(Function(cpd) Not cpd Is Nothing) _
            .GroupBy(Function(c) c.entry) _
            .Select(Function(g) g.First) _
            .OrderBy(Function(cpd) cpd.entry) _
            .ToArray

        SyncLock R_server.R
            With R_server.R

                !KEGG_meta = Rbase.lapply(
                    x:=compounds,
                    FUN:=Function(compound)
                             Dim exactMass As Double = compound.exactMass
                             ' predefined precursor type data table
                             Dim pos$ = precursorMzMatrix(exactMass, 1)
                             Dim neg$ = precursorMzMatrix(exactMass, -1)

                             Call compound.commonNames?.FirstOrDefault().__DEBUG_ECHO

                             Return Rbase.list(
                                !ID = compound.entry,
                                !exact_mass = exactMass,
                                !name = compound.commonNames,
                                !formula = compound.formula,
                                !positive = pos,
                                !negative = neg
                             )
                         End Function)

                Call out.__DEBUG_ECHO
                Call Rbase.save({"KEGG_meta"}, file:=out)
            End With
        End SyncLock

        Return 0
    End Function

    <Extension>
    Friend Function precursorMzMatrix(exact_mass#, libtype As Integer) As String
        Dim precursorMz As New Dictionary(Of String, NamedValue(Of Double))
        'libname precursor_type, mz
        Dim libnames = precursorMz.Keys.AsList
        Dim precursor_type = libnames.Select(Function(r) precursorMz(r).Name).AsList
        Dim mz = libnames.Select(Function(r) precursorMz(r).Value).AsList
        Dim prefix$ = If(precursorMz.Count > 0, "MzCalculator: ", "")

        If libtype = 1 Then
            For Each type In Provider.Positives
                libnames += prefix & type.ToString
                precursor_type += type.ToString
                mz += type.CalcMZ(exact_mass)
            Next
        Else
            For Each type In Provider.Negatives
                libnames += prefix & type.ToString
                precursor_type += type.ToString
                mz += type.CalcMZ(exact_mass)
            Next
        End If

        Dim matrix$

        SyncLock R_server.R
            With R_server.R

                matrix = Rbase.dataframe(
                    !precursor_type = Rbase.c(precursor_type.ToArray, stringVector:=True),
                    !mz = Rbase.c(mz)
                )
                Rbase.rownames(matrix) = libnames.AsVector

                Return matrix
            End With
        End SyncLock
    End Function

    ''' <summary>
    ''' 这个结构所产生的数据库是MetaDNA R程序包所需要的data数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/compile")>
    <Usage("/compile /br08201 <reactions.repo.directory> [/out <default=metaDNA_kegg.rda>]")>
    <Description("Compile the kegg reaction/compound database as MetaDNA network database into RDA format.")>
    <Argument("/br08201", False, CLITypes.File,
              AcceptTypes:={GetType(Reaction)},
              Description:="The kegg reaction xml data model database repository directory.")>
    <Argument("/out", True, CLITypes.File, Description:="The file save location of the rda database file.")>
    Public Function Compiler(args As CommandLine) As Integer
        Return (args <= "/br08201") _
            .BuildNetwork(args("/out") Or $"metaDNA_kegg.rda") _
            .CLICode
    End Function

    ReadOnly NA As New [Default](Of String)("NA", Function(exp) CStr(exp).StringEmpty OrElse CStr(exp) = "NULL")

    <Extension>
    Private Function BuildNetwork(repository As String, rda$) As Boolean
        Dim reactions = (ls - l - r - "*.Xml" <= repository) _
            .Select(Function(path)
                        Return path.LoadXml(Of ReactionClass)(stripInvalidsCharacter:=True)
                    End Function) _
            .GroupBy(Function(rc) rc.entryId) _
            .Select(Function(group)
                        Return group.First
                    End Function) _
            .ToArray

        Call $"There are {reactions.Length} unique reaction class data".__INFO_ECHO

        SyncLock R_server.R
            With R_server.R

                ' network <- list(rxnID, define, reactants, products);
                ' reactants和products都是KEGG的代谢物编号
                Dim network As New RSymbol(Rbase.list())

                With New VisualBasic

                    'network = Rbase.lapply(
                    '    x:=reactions,
                    '    FUN:=Function(reaction)
                    '             Dim model As GenericEquation = reaction.ReactionModel
                    '             Dim name$ = Rbase.c(reaction.CommonNames, stringVector:=True) Or NA

                    '             ' 生成一个列表之中的反应过程的摘要模型
                    '             ' 相当于网络之中的一个节点
                    '             Return Rbase.list(
                    '                 !rxnID = reaction.ID,
                    '                 !name = (name),
                    '                 !define = reaction.Definition,
                    '                 !reactants = Rbase.c(model.Reactants.Keys, stringVector:=True),
                    '                 !products = Rbase.c(model.Products.Keys, stringVector:=True)
                    '             )
                    '         End Function)

                    Dim key$
                    Dim i As i32

                    For Each rclass In reactions
                        If rclass.reactantPairs.Length > 1 Then
                            i = 1

                            For Each transform In rclass.reactantPairs
                                key = $"{rclass.entryId}_{++i}"
                                ' 生成一个列表之中的反应过程的摘要模型
                                ' 相当于网络之中的一个节点
                                network(key) = Rbase.list(
                                    !rxnID = rclass.entryId,
                                    !name = rclass.definition,
                                    !define = rclass.definition,
                                    !reactants = transform.from,
                                    !products = transform.to
                                )
                            Next
                        End If
                    Next
                End With

                !network = Rbase.list(
                    !network = network.name,
                    !name = "MetaDNA Reaction Class Network",
                    !description = "MetaDNA Reaction Class Network dataset.",
                    !built = Now.ToString
                )

                Call Rbase.save({"network"}, rda)
            End With
        End SyncLock

        Return rda.FileLength > 0
    End Function
End Module
