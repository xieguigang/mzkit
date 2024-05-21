#Region "Microsoft.VisualBasic::2a0e923b688e91a7d9bc9acce3e7bb4d, mzmath\MSFinder\FormulaResult.vb"

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

    '   Total Lines: 41
    '    Code Lines: 36
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.90 KB


    ' Class FormulaResult
    ' 
    '     Properties: AnnotatedIonResult, ChemicalOntologyDescriptions, ChemicalOntologyIDs, ChemicalOntologyRepresentativeInChIKeys, ChemicalOntologyScores
    '                 Formula, IsotopicScore, IsSelected, M1IsotopicDiff, M1IsotopicIntensity
    '                 M2IsotopicDiff, M2IsotopicIntensity, MassDiff, MassDiffScore, MatchedMass
    '                 NeutralLossHits, NeutralLossNum, NeutralLossResult, NeutralLossScore, ProductIonHits
    '                 ProductIonNum, ProductIonResult, ProductIonScore, PubchemResources, ResourceNames
    '                 ResourceRecords, ResourceScore, TotalScore
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class FormulaResult
    Public Property Formula As New Formula()
    Public Property MatchedMass As Double
    Public Property MassDiff As Double
    Public Property M1IsotopicDiff As Double
    Public Property M2IsotopicDiff As Double
    Public Property M1IsotopicIntensity As Double
    Public Property M2IsotopicIntensity As Double
    Public Property MassDiffScore As Double
    Public Property IsotopicScore As Double
    Public Property ProductIonScore As Double
    Public Property NeutralLossHits As Integer
    Public Property NeutralLossScore As Double
    Public Property ProductIonHits As Integer
    Public Property ProductIonNum As Integer
    Public Property NeutralLossNum As Integer
    Public Property ResourceScore As Double
    Public Property ResourceNames As String = String.Empty
    Public Property ResourceRecords As Integer
    Public Property TotalScore As Double
    Public Property IsSelected As Boolean
    Public Property ProductIonResult As List(Of ProductIon) = New List(Of ProductIon)()
    Public Property NeutralLossResult As List(Of NeutralLoss) = New List(Of NeutralLoss)()
    Public Property AnnotatedIonResult As List(Of AnnotatedIon) = New List(Of AnnotatedIon)()
    Public Property PubchemResources As List(Of Integer) = New List(Of Integer)()
    Public Property ChemicalOntologyDescriptions As List(Of String) = New List(Of String)()
    Public Property ChemicalOntologyIDs As List(Of String) = New List(Of String)()
    Public Property ChemicalOntologyScores As List(Of Double) = New List(Of Double)()
    Public Property ChemicalOntologyRepresentativeInChIKeys As List(Of String) = New List(Of String)()

End Class
