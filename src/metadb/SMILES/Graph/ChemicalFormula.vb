#Region "Microsoft.VisualBasic::bce953cdf2f1f5a3f108a05080b30d6f, metadb\SMILES\Graph\ChemicalFormula.vb"

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

    '   Total Lines: 160
    '    Code Lines: 110 (68.75%)
    ' Comment Lines: 29 (18.12%)
    '    - Xml Docs: 96.55%
    ' 
    '   Blank Lines: 21 (13.12%)
    '     File Size: 5.66 KB


    ' Class ChemicalFormula
    ' 
    '     Properties: AllBonds, AllElements, EmpiricalFormula
    ' 
    '     Function: Decomposition, FindKeys, GetFormula, GetGraph, Join
    '               ParseGraph, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph
Imports Microsoft.VisualBasic.Language
Imports EmpiricalFormula = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula

''' <summary>
''' the molecule graph
''' </summary>
Public Class ChemicalFormula : Inherits NetworkGraph(Of ChemicalElement, ChemicalKey)
    Implements ISparseGraph

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

    Public ReadOnly Property EmpiricalFormula As String
        Get
            Dim formula As String = Nothing
            Call New FormulaBuilder(Me).GetComposition(formula)
            Return formula
        End Get
    End Property

    ''' <summary>
    ''' find atom element by unique reference id
    ''' </summary>
    ''' <param name="elementkey"></param>
    ''' <returns></returns>
    Public Iterator Function FindKeys(elementkey As String) As IEnumerable(Of ChemicalKey)
        For Each key As ChemicalKey In AllBonds
            If key.U.label = elementkey OrElse key.V.label = elementkey Then
                Yield key
            End If
        Next
    End Function

    Public Function GetFormula(Optional canonical As Boolean = False) As EmpiricalFormula
        Dim empiricalFormula As String = Nothing
        Dim composition As Dictionary(Of String, Integer) = New FormulaBuilder(Me).GetComposition(empiricalFormula)

        If canonical Then
            empiricalFormula = Nothing
        End If

        composition = Decomposition(composition) _
            .GroupBy(Function(a) a.Key) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return Aggregate element In a Into Sum(element.Value)
                          End Function)

        Return New EmpiricalFormula(composition, empiricalFormula)
    End Function

    Private Shared Iterator Function Decomposition(composition As Dictionary(Of String, Integer)) As IEnumerable(Of KeyValuePair(Of String, Integer))
        For Each group As KeyValuePair(Of String, Integer) In composition
            If group.Key.Length = 1 Then
                Yield group
            Else
                Dim group_formula = FormulaScanner.ScanFormula(group.Key)
                Dim multiply As Integer = group.Value

                If multiply = 1 Then
                    For Each item In group_formula.CountsByElement
                        Yield item
                    Next
                Else
                    For Each item In group_formula.CountsByElement
                        Yield New KeyValuePair(Of String, Integer)(item.Key, item.Value * multiply)
                    Next
                End If
            End If
        Next
    End Function

    ''' <summary>
    ''' Add new independent part of the molecule into current molecule part
    ''' </summary>
    ''' <param name="part"></param>
    ''' <returns></returns>
    Public Function Join(part As ChemicalFormula) As ChemicalFormula
        Dim union As New ChemicalFormula
        Dim key As ChemicalKey
        Dim element As ChemicalElement
        Dim mapping As New Dictionary(Of ChemicalElement, ChemicalElement)

        For Each atom In vertex
            element = New ChemicalElement(atom, union.vertex.Count + 1)

            Call mapping.Add(atom, element)
            Call union.AddVertex(element)
        Next
        For Each atom In part.vertex
            element = New ChemicalElement(atom, union.vertex.Count + 1)

            Call mapping.Add(atom, element)
            Call union.AddVertex(element)
        Next
        For Each edge In graphEdges
            key = New ChemicalKey With {
                .bond = edge.bond,
                .U = mapping(edge.U),
                .V = mapping(edge.V),
                .weight = edge.weight
            }

            Call union.Insert(key)
        Next
        For Each edge In part.graphEdges
            key = New ChemicalKey With {
                .bond = edge.bond,
                .U = mapping(edge.U),
                .V = mapping(edge.V),
                .weight = edge.weight
            }

            Call union.Insert(key)
        Next

        Return union
    End Function

    ''' <summary>
    ''' Make smiles graph model conversion to the basic SDF molecule structure model
    ''' </summary>
    ''' <returns>a structure model which could be used for the QSAR analysis</returns>
    Public Function CreateStructureGraph() As [Structure]
        Dim atoms As New Dictionary(Of String, SDF.Models.Atom)
        Dim offset As New Dictionary(Of String, Integer)
        Dim links As New List(Of Bound)
        Dim index As i32 = 0

        For Each group As ChemicalElement In AllElements
            Call offset.Add(group.label, ++index)
            Call atoms.Add(group.label, New SDF.Models.Atom(group.group))
        Next

        For Each bond As ChemicalKey In AllBonds
            Dim i As Integer = offset(bond.U.label)
            Dim j As Integer = offset(bond.V.label)
            Dim link As New Bound(i, i, DirectCast(bond.bond, BoundTypes))

            Call links.Add(link)
        Next

        Return New [Structure](atoms.Values, links)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SMILES"></param>
    ''' <param name="strict"></param>
    ''' <returns>
    ''' this function may returns nothing if the given smiles string is invalid and case the parser error when strict is false.
    ''' </returns>
    Public Shared Function ParseGraph(SMILES As String, Optional strict As Boolean = True) As ChemicalFormula
        Return ParseChain.ParseGraph(SMILES, strict:=strict)
    End Function

    Public Overrides Function ToString() As String
        Return GetFormula.ToString
    End Function

    Public Iterator Function GetGraph() As IEnumerable(Of IInteraction) Implements ISparseGraph.GetGraph
        For Each edge As ChemicalKey In graphEdges
            Yield edge
        Next
    End Function
End Class
