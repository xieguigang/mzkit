#Region "Microsoft.VisualBasic::5766dbe21639c15a068d93ec6afec438, G:/mzkit/src/mzmath/MSFinder//ExistFormulaQuery.vb"

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

    '   Total Lines: 33
    '    Code Lines: 19
    ' Comment Lines: 5
    '   Blank Lines: 9
    '     File Size: 1.07 KB


    ' Class ExistFormulaQuery
    ' 
    '     Properties: Formula, FormulaRecords, PubchemCidList, ResourceNames, ResourceNumber
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' This is the class variable to store the internal query of molecular formula.
''' The queries are stored in .EFD file of the same folder of main program (should be).
''' The EFD file will be retrieved by ExistFormulaDbParcer.cs.
''' </summary>
Public Class ExistFormulaQuery

    Public Sub New()
        PubchemCidList = New List(Of Integer)()
        Formula = New Formula()
    End Sub

    Public Sub New(formula As Formula, pubchemCidList As List(Of Integer), formulaRecords As Integer, dbRecords As Integer, dbNames As String)
        Me.Formula = formula
        Me.PubchemCidList = pubchemCidList
        Me.FormulaRecords = formulaRecords
        ResourceNumber = dbRecords
        ResourceNames = dbNames
    End Sub

    Public Property Formula As Formula

    Public Property PubchemCidList As List(Of Integer)

    Public Property FormulaRecords As Integer

    Public Property ResourceNumber As Integer

    Public Property ResourceNames As String

End Class
