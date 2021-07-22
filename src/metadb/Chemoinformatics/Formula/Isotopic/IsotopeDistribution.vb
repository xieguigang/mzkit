Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics

Namespace Formula.IsotopicPatterns

    Public Class IsotopeDistribution

        Shared ReadOnly PeriodicTable As Dictionary(Of String, Element) = Element.MemoryLoadElements

        Public Property data As IsotopeCount()
        Public Property mz As Double()
        Public Property intensity As Double()

        Public ReadOnly Property Size As Integer
            Get
                Return mz.Length
            End Get
        End Property

        Public Shared Function GenerateDistribution(formula As Formula,
                                                    Optional prob_threshold As Double = 0.001,
                                                    Optional fwhm As Double = 0.1,
                                                    Optional pad_left As Double = 3,
                                                    Optional pad_right As Double = 3,
                                                    Optional interpolate_grid As Double = 0.1) As IsotopeDistribution

            Dim ds As IsotopeCount() = dir(formula, prob_threshold:=prob_threshold) _
                .DoCall(AddressOf IsotopeCount.Normalize) _
                .ToArray
            Dim xs As Double() = (From d In ds Select CDbl(d(3))).ToArray
            Dim ys As Double() = (From d In ds Select CDbl(d(2))).ToArray
            Dim x_min = xs.Min - pad_left
            Dim x_max = xs.Max + pad_right
            Dim plot_xs = Calculator.frange(x_min, x_max, interpolate_grid).ToArray
            Dim plot_ys = plot_xs.Select(Function(_any) 0.0).ToArray

            For Each i As SeqValue(Of Double) In xs.SeqIterator
                Dim peak_x = i.value
                Dim b = peak_x
                Dim a = ys(i)
                Dim gauss_ys = Calculator.gaussian(plot_xs, a, b, fwhm)

                For Each j In gauss_ys.SeqIterator
                    plot_ys(j) += j.value
                Next
            Next

            Return New IsotopeDistribution With {
                .data = ds _
                    .OrderBy(Function(a) a.nom_mass) _
                    .ToArray,
                .mz = plot_xs,
                .intensity = plot_ys
            }
        End Function

        Private Shared Function GetMostAbundance(formula As Formula) As IsotopeCount
            Dim topAtoms As Dictionary(Of String, Isotope) = formula.Elements _
                .ToDictionary(Function(a) a,
                              Function(a)
                                  Return IsotopeDistribution.PeriodicTable(a).isotopes _
                                      .OrderByDescending(Function(i) i.Prob) _
                                      .First
                              End Function)
            Dim allAtoms As String() = formula.CountsByElement _
                .Select(Function(a) a.Key.Repeats(a.Value)) _
                .IteratesALL _
                .ToArray
            Dim prob As Double = 1

            For Each atom As String In allAtoms
                prob *= topAtoms(atom).Prob
            Next

            Return New IsotopeCount With {
                .abs_mass = formula.ExactMass,
                .atoms = allAtoms,
                .prob = prob,
                .nom_mass = .atoms _
                            .Select(Function(a)
                                        Return topAtoms(a).NumNeutrons
                                    End Function) _
                            .Sum
            }
        End Function

        Private Shared Function dir(formula As Formula, Optional prob_threshold As Double = 0.001) As IsotopeCount()
            Dim lst As New List(Of IsotopeCount)
            Dim atom_types As String()

            lst += GetMostAbundance(formula)

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
                    Call lst.Add(([atom_type], [nom_mass], prob, abs_mass))

                    Dim items_to_append As New List(Of IsotopeCount)
                    Dim isotopeProb As Double

                    For Each itm As IsotopeCount In lst
                        For Each i As Integer In Range(1, num_atoms + 1)
                            atom_types = itm.atoms _
                                .JoinIterates([atom_type].Repeats(i)) _
                                .ToArray
                            isotopeProb = itm(2) * ((prob) ^ i) * SpecialFunctions.Binom(num_atoms - i, num_atoms)
                            items_to_append.Add((atom_types, itm(1) + [nom_mass] * i, isotopeProb, itm(3) + abs_mass * i))
                        Next
                    Next

                    ' prevent addition Of very unlikely isotope distributions
                    items_to_append = items_to_append _
                        .Where(Function(itm) itm(2) >= prob_threshold) _
                        .AsList

                    ' prevent duplicates
                    lst = lst + items_to_append.Where(Function(itm) lst.IndexOf(itm) = -1)
                Next

                lst = (From itm As IsotopeCount
                       In lst
                       Where ContainsNumAtomsOfType(itm(0), num_atoms, atom_type)).AsList
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