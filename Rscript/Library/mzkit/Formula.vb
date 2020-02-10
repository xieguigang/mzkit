#Region "Microsoft.VisualBasic::7c19f83adacec6da32f9a806ba3539f5, Rscript\Library\mzkit\Formula.vb"

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

' Module Formula
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: CreateGraph, FormulaCompositionString, readKCF, readSDF, ScanFormula
'               SDF2KCF
' 
' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.Rsharp.Runtime.Interop
Imports MwtWin = SMRUCC.proteomics.PNL.OMICS.MwtWinDll
Imports MwtWinFormula = SMRUCC.proteomics.PNL.OMICS.MwtWinDll.FormulaFinder.FormulaFinderResult
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

''' <summary>
''' The chemical formulae toolkit
''' </summary>
<Package("mzkit.formula", Category:=APICategories.UtilityTools)>
Module Formula

    Sub New()
        Call REnv.AttachConsoleFormatter(Of FormulaComposition)(AddressOf FormulaCompositionString)
        Call REnv.AttachConsoleFormatter(Of MwtWinFormula())(AddressOf printFormulas)
    End Sub

    Private Function printFormulas(formulas As MwtWinFormula()) As String
        Dim table As New List(Of String())

        table += {"formula", "mass", "da", "ppm", "charge", "m/z"}

        For Each formula As MwtWinFormula In formulas
            table += New String() {
                formula.EmpiricalFormula,
                formula.Mass,
                formula.DeltaMass,
                formula.DeltaMassIsPPM,
                formula.ChargeState,
                formula.MZ
            }
        Next

        Return table.Print(addBorder:=False)
    End Function

    Private Function FormulaCompositionString(formula As FormulaComposition) As String
        Return formula.EmpiricalFormula & $" ({formula.CountsByElement.GetJson})"
    End Function

    <ExportAPI("find.formula")>
    Public Function FormulaFinder(mass#, Optional tolerance# = 0.1,
                                  <RRawVectorArgument(GetType(String))>
                                  Optional candidateElements As Object = "C|H|N|O",
                                  Optional elementMode As MwtWin.MWElementAndMassRoutines.emElementModeConstants = MwtWin.MWElementAndMassRoutines.emElementModeConstants.emIsotopicMass) As MwtWinFormula()

        Dim oMwtWin As New MwtWin.MolecularWeightCalculator()

        oMwtWin.SetElementMode(elementMode)
        oMwtWin.FormulaFinder.CandidateElements.Clear()

        For Each element As String In DirectCast(candidateElements, String())
            Call oMwtWin.FormulaFinder.AddCandidateElement(element)
        Next

        Dim searchOptions As New MwtWin.FormulaFinder.FormulaFinderOptions() With {
            .LimitChargeRange = False,
            .ChargeMin = 1,
            .ChargeMax = 1,
            .FindTargetMZ = False
        }
        Dim results As MwtWinFormula() = oMwtWin.FormulaFinder _
            .FindMatchesByMass(mass, tolerance, searchOptions) _
            .ToArray

        Return results
    End Function

    ''' <summary>
    ''' Get atom composition from a formula string
    ''' </summary>
    ''' <param name="formula">The input formula string text.</param>
    ''' <param name="n">for counting polymers atoms</param>
    ''' <returns></returns>
    <ExportAPI("scan.formula")>
    Public Function ScanFormula(formula$, Optional n% = 9999) As FormulaComposition
        Return FormulaScanner.ScanFormula(formula, n)
    End Function

    ''' <summary>
    ''' Read KCF model data
    ''' </summary>
    ''' <param name="data">The text data or file path</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("read.KCF")>
    Public Function readKCF(data As String) As Model.KCF
        Return Model.IO.LoadKCF(data)
    End Function

    ''' <summary>
    ''' Create molecular network graph model
    ''' </summary>
    ''' <param name="kcf">The KCF molecule model</param>
    ''' <returns></returns>
    <ExportAPI("KCF.graph")>
    Public Function CreateGraph(kcf As Model.KCF) As NetworkGraph
        Return kcf.CreateGraph
    End Function

    <ExportAPI("read.SDF")>
    Public Function readSDF(data As String, Optional parseStruct As Boolean = True) As SDF
        Return SDF.ParseSDF(data.SolveStream, parseStruct)
    End Function

    <ExportAPI("SDF.convertKCF")>
    Public Function SDF2KCF(sdfModel As SDF) As Model.KCF
        Return sdfModel.ToKCF
    End Function

    <ExportAPI("download.kcf")>
    Public Function DownloadKCF(keggcompoundIDs As String(), save$) As Object
        Dim result As New List(Of Object)
        Dim KCF$

        For Each id As String In keggcompoundIDs.SafeQuery
            KCF = MetaboliteWebApi.DownloadKCF(id, saveDIR:=save)

            Call result.Add(KCF)
            Call Thread.Sleep(1000)
        Next

        Return result.ToArray
    End Function
End Module
