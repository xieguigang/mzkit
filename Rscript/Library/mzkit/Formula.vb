#Region "Microsoft.VisualBasic::2001f71cf11abc90cd02dd7a267a6344, Rscript\Library\mzkit\Formula.vb"

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
'     Function: FormulaCompositionString, ScanFormula
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemistry.Model.Graph
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.MassSpectrum.DATA.MetaLib
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

''' <summary>
''' The chemical formulae toolkit
''' </summary>
<Package("mzkit.formula", Category:=APICategories.UtilityTools)>
Module Formula

    Sub New()
        Call REnv.AttachConsoleFormatter(Of FormulaComposition)(AddressOf FormulaCompositionString)
    End Sub

    Private Function FormulaCompositionString(formula As FormulaComposition) As String
        Return formula.EmpiricalFormula & $" ({formula.CountsByElement.GetJson})"
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
End Module

