#Region "Microsoft.VisualBasic::2077da851aeacaff678a23b26cdaae9a, mzkit\src\metadb\Chemoinformatics\Formula\Isotopic\IsotopeDistribution.vb"

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

    '   Total Lines: 94
    '    Code Lines: 68
    ' Comment Lines: 9
    '   Blank Lines: 17
    '     File Size: 4.40 KB


    '     Class IsotopeDistribution
    ' 
    '         Properties: data, exactMass, formula, intensity, mz
    '                     Size
    ' 
    '         Function: ContainsNumAtomsOfType, Distribution, FilterAccordingToSumFormula
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Formula.IsotopicPatterns

    Public Class IsotopeDistribution

        Shared ReadOnly PeriodicTable As Dictionary(Of String, Element) = Element.MemoryLoadElements

        Public Property data As IsotopeCount()
        Public Property mz As Double()
        Public Property intensity As Double()
        Public Property formula As String
        Public Property exactMass As Double

        Public ReadOnly Property Size As Integer
            Get
                Return mz.Length
            End Get
        End Property

        Public Shared Function Distribution(formula As Formula, Optional prob_threshold As Double = 0.0000000001) As IsotopeCount()
            Dim lst As New List(Of IsotopeCount)
            Dim atom_types As String()

            ' lst += GetMostAbundance(formula)

            For Each atom As KeyValuePair(Of String, Integer) In formula.CountsByElement
                Dim atom_type As String = atom.Key
                Dim num_atoms As Integer = atom.Value
                Dim elt As Element = PeriodicTable(atom_type)

                ' Iterate over isotopes indexed by nominal masses
                For Each isotope As Isotope In elt.isotopes
                    Dim prob As Double = isotope.Prob
                    Dim abs_mass As Double = isotope.Mass
                    Dim nom_mass As Double = isotope.NumNeutrons

                    ' Each iso dist Is made up Of atom types, nominal masses,
                    ' the probability And the mass Of all atoms together.
                    Call lst.Add(({atom_type}, {nom_mass}, prob, abs_mass))

                    Dim items_to_append As New List(Of IsotopeCount)
                    Dim isotopeProb As Double
                    Dim nom_massList As Double()

                    For Each itm As IsotopeCount In lst
                        For Each i As Integer In Range(1, num_atoms + 1)
                            atom_types = itm.atoms _
                                .JoinIterates([atom_type].Repeats(i)) _
                                .ToArray
                            nom_massList = itm.nom_mass _
                                .JoinIterates(nom_mass.Repeats(i)) _
                                .ToArray

                            ' isotopeProb = SpecialFunctions.Binomial(prob, num_atoms - i, num_atoms)
                            ' isotopeProb = itm.prob * isotopeProb * (prob ^ i)
                            isotopeProb = itm.prob * prob
                            ' isotopeProb = itm.prob * (prob ^ i) * isotopeProb
                            items_to_append.Add((atom_types, nom_massList, isotopeProb, itm.abs_mass + abs_mass * i))
                        Next
                    Next

                    ' prevent addition Of very unlikely isotope distributions
                    ' and prevent duplicates
                    lst = lst + items_to_append _
                        .Where(Function(itm)
                                   Return itm(2) >= prob_threshold AndAlso
                                       lst.IndexOf(itm) = -1 AndAlso
                                       ContainsNumAtomsOfType(itm(0), num_atoms, atom_type)
                               End Function)
                Next
            Next

            Return FilterAccordingToSumFormula(lst, formula).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Iterator Function FilterAccordingToSumFormula(lst As List(Of IsotopeCount), sf As Formula) As IEnumerable(Of IsotopeCount)
            For Each itm As IsotopeCount In lst
                If (From tup In sf.CountsByElement Select ContainsNumAtomsOfType(itm(0), tup.Value, tup.Key)).All Then
                    Yield itm
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function ContainsNumAtomsOfType(lstOfAtomTypes As String(), numAtoms As Integer, atomType As String) As Boolean
            Return (From itm As String In lstOfAtomTypes Where itm = atomType).Count = numAtoms
        End Function
    End Class
End Namespace
