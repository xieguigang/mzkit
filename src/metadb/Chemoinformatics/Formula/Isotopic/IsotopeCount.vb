#Region "Microsoft.VisualBasic::4eb6aa5548520f30cc37b963c7db4478, src\metadb\Chemoinformatics\Formula\Isotopic\IsotopeCount.vb"

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

    '     Structure IsotopeCount
    ' 
    '         Properties: Formula
    ' 
    '         Function: Normalize, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math

Namespace Formula.IsotopicPatterns

    Public Structure IsotopeCount

        ''' <summary>
        ''' [0] atom_type
        ''' </summary>
        Dim atoms As String()
        ''' <summary>
        ''' [1] nom_mass
        ''' </summary>
        Dim nom_mass As Double()

        ''' <summary>
        ''' [2] probility
        ''' </summary>
        Dim prob#
        ''' <summary>
        ''' normalized by <see cref="prob"/>
        ''' </summary>
        Dim abundance As Double

        ''' <summary>
        ''' [3] exact mass
        ''' </summary>
        Dim abs_mass#

        Default Public ReadOnly Property Item(i As Integer) As Object
            Get
                Select Case i
                    Case 0 : Return atoms
                    Case 1 : Return nom_mass
                    Case 2 : Return prob
                    Case 3 : Return abs_mass
                    Case Else
                        Throw New NotImplementedException
                End Select
            End Get
        End Property

        Public ReadOnly Property Formula As Formula
            Get
                Return New Formula(atoms.GroupBy(Function(a) a).ToDictionary(Function(a) a.Key, Function(a) a.Count))
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{nom_mass.Sum}][{Formula.ToString}, {abs_mass.ToString("F4")}], prob = {prob.ToString("G3")}, abundance = {(abundance).ToString("F2")}"
        End Function

        Public Shared Iterator Function Normalize(isotopes As IEnumerable(Of IsotopeCount)) As IEnumerable(Of IsotopeCount)
            Dim all As NamedCollection(Of IsotopeCount)() = isotopes.GroupBy(Function(i) i.abs_mass, offsets:=0.3).ToArray

            If all.Length = 0 Then
                Return
            End If

            Dim j As i32 = 0
            Dim prob As Double() = all.Select(Function(i) i.Sum(Function(a) a.prob)).ToArray
            Dim maxProb As Double = Aggregate i In prob Into Max(i)

            For Each i As NamedCollection(Of IsotopeCount) In all
                Dim top = i.OrderByDescending(Function(a) a.prob).First

                Yield New IsotopeCount With {
                    .abs_mass = top.abs_mass,
                    .abundance = 100 * prob(j) / maxProb,' logRange.ScaleMapping(log(++j), percentage),
                    .atoms = top.atoms,
                    .prob = prob(++j),
                    .nom_mass = top.nom_mass
                }
            Next
        End Function

        Public Shared Widening Operator CType(itm As (atom_type As String(), nom_mass As Double(), prob#, abs_mass#)) As IsotopeCount
            Return New IsotopeCount With {
                .abs_mass = itm.abs_mass,
                .atoms = itm.atom_type,
                .nom_mass = itm.nom_mass,
                .prob = itm.prob
            }
        End Operator
    End Structure
End Namespace
