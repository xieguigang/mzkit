#Region "Microsoft.VisualBasic::786ccf128c1209c3d8ed094a128020dc, mzkit\src\metadb\SMILES\Graph\ChemicalFormula.vb"

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

    '   Total Lines: 39
    '    Code Lines: 29
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 1.31 KB


    ' Class ChemicalFormula
    ' 
    '     Properties: AllBonds, AllElements
    ' 
    '     Function: FindKeys, GetFormula, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports EmpiricalFormula = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula

''' <summary>
''' the molecule graph
''' </summary>
Public Class ChemicalFormula : Inherits NetworkGraph(Of ChemicalElement, ChemicalKey)

    ''' <summary>
    ''' the graph edges is the connection links between the atom groups
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AllBonds As IEnumerable(Of ChemicalKey)
        Get
            Return graphEdges
        End Get
    End Property

    ''' <summary>
    ''' the atom groups
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AllElements As IEnumerable(Of ChemicalElement)
        Get
            Return vertex
        End Get
    End Property

    Public Iterator Function FindKeys(elementkey As String) As IEnumerable(Of ChemicalKey)
        For Each key As ChemicalKey In AllBonds
            If key.U.label = elementkey OrElse key.V.label = elementkey Then
                Yield key
            End If
        Next
    End Function

    Public Function GetFormula() As EmpiricalFormula
        Dim empiricalFormula As String = Nothing
        Dim composition As Dictionary(Of String, Integer) = New FormulaBuilder(Me).GetComposition(empiricalFormula)

        Return New EmpiricalFormula(composition, empiricalFormula)
    End Function

    Public Overrides Function ToString() As String
        Return GetFormula.ToString
    End Function
End Class
