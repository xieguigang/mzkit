#Region "Microsoft.VisualBasic::4329c0cd0b6b9c66c06017d1e641d56b, Rscript\Library\mzkit\math\Formula.vb"

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
    '     Function: CreateGraph, DownloadKCF, EvalFormula, FormulaCompositionString, FormulaFinder
    '               LoadChemicalDescriptorsMatrix, openChemicalDescriptorDatabase, printFormulas, readKCF, readSDF
    '               ScanFormula, SDF2KCF
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

''' <summary>
''' The chemical formulae toolkit
''' </summary>
<Package("formula", Category:=APICategories.UtilityTools)>
Module Formula

    Sub New()
        Call REnv.AttachConsoleFormatter(Of FormulaComposition)(AddressOf FormulaCompositionString)
        Call REnv.AttachConsoleFormatter(Of FormulaComposition())(AddressOf printFormulas)
    End Sub

    Private Function printFormulas(formulas As FormulaComposition()) As String
        Dim table As New List(Of String())

        table += {"formula", "mass", "da", "ppm", "charge", "m/z"}

        For Each formula As FormulaComposition In formulas
            table += New String() {
                formula.EmpiricalFormula,
                formula.ExactMass,
                formula.ppm,
                formula.ppm,
                formula.charge,
                formula.ExactMass / formula.charge
            }
        Next

        Return table.Print(addBorder:=False)
    End Function

    Private Function FormulaCompositionString(formula As FormulaComposition) As String
        Return formula.EmpiricalFormula & $" ({formula.CountsByElement.GetJson})"
    End Function

    <ExportAPI("find.formula")>
    Public Function FormulaFinder(mass#,
                                  Optional ppm# = 5,
                                  <RRawVectorArgument(GetType(String))>
                                  Optional candidateElements As Object = "C|H|N|O") As FormulaComposition()

        Dim opts As New SearchOption(-9999, 9999, ppm)

        For Each element As String In DirectCast(candidateElements, String())
            Call opts.AddElement(element, 0, 30)
        Next

        Dim oMwtWin As New FormulaSearch(opts)
        Dim results As FormulaComposition() = oMwtWin.SearchByExactMass(mass).ToArray

        Return results
    End Function

    ''' <summary>
    ''' evaluate exact mass for the given formula strings.
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <returns></returns>
    <ExportAPI("eval_formula")>
    <RApiReturn(GetType(Double))>
    Public Function EvalFormula(<RRawVectorArgument> formula As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of String, Double)(
            x:=formula,
            eval:=Function(str)
                      Dim composition As FormulaComposition = FormulaScanner.ScanFormula(str)
                      Dim mass = Aggregate atom In composition.CountsByElement
                                 Let eval As Double = ExactMass.Eval(atom.Key) * atom.Value
                                 Into Sum(eval)

                      Return mass
                  End Function)
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

    ''' <summary>
    ''' parse a single sdf text block
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="parseStruct"></param>
    ''' <returns></returns>
    <ExportAPI("parse.SDF")>
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

    ''' <summary>
    ''' open the file handles of the chemical descriptor database. 
    ''' </summary>
    ''' <param name="dbFile">
    ''' A directory path which contains the multiple database file of the 
    ''' chemical descriptors.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("open.descriptor.db")>
    Public Function openChemicalDescriptorDatabase(dbFile As String) As PubChemDescriptorRepo
        Return New PubChemDescriptorRepo(dir:=dbFile)
    End Function

    <ExportAPI("descriptor.matrix")>
    <RApiReturn(GetType(DataSet()))>
    Public Function LoadChemicalDescriptorsMatrix(repo As PubChemDescriptorRepo, cid As Long(), Optional env As Environment = Nothing) As Object
        If repo Is Nothing Then
            Return Internal.debug.stop("no chemical descriptor database was provided!", env)
        ElseIf cid.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim matrix As New List(Of DataSet)
        Dim descriptor As ChemicalDescriptor
        Dim row As DataSet

        For Each id As Long In cid
            descriptor = repo.GetDescriptor(cid:=id)
            row = New DataSet With {
                .ID = id,
                .Properties = New Dictionary(Of String, Double) From {
                    {NameOf(descriptor.AtomDefStereoCount), descriptor.AtomDefStereoCount},
                    {NameOf(descriptor.AtomUdefStereoCount), descriptor.AtomUdefStereoCount},
                    {NameOf(descriptor.BondDefStereoCount), descriptor.BondDefStereoCount},
                    {NameOf(descriptor.BondUdefStereoCount), descriptor.BondUdefStereoCount},
                    {NameOf(descriptor.Complexity), descriptor.Complexity},
                    {NameOf(descriptor.ComponentCount), descriptor.ComponentCount},
                    {NameOf(descriptor.ExactMass), descriptor.ExactMass},
                    {NameOf(descriptor.FormalCharge), descriptor.FormalCharge},
                    {NameOf(descriptor.HeavyAtoms), descriptor.HeavyAtoms},
                    {NameOf(descriptor.HydrogenAcceptor), descriptor.HydrogenAcceptor},
                    {NameOf(descriptor.HydrogenDonors), descriptor.HydrogenDonors},
                    {NameOf(descriptor.IsotopicAtomCount), descriptor.IsotopicAtomCount},
                    {NameOf(descriptor.RotatableBonds), descriptor.RotatableBonds},
                    {NameOf(descriptor.TautoCount), descriptor.TautoCount},
                    {NameOf(descriptor.TopologicalPolarSurfaceArea), descriptor.TopologicalPolarSurfaceArea},
                    {NameOf(descriptor.XLogP3), descriptor.XLogP3},
                    {NameOf(descriptor.XLogP3_AA), descriptor.XLogP3_AA}
                }
            }

            If Not row.Properties.Values.All(Function(x) x = 0.0) Then
                ' is not empty
                matrix += row
            End If
        Next

        Return matrix.ToArray
    End Function
End Module
