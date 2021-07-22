Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics

Namespace Formula.IsotopicPatterns

    Public Class IsotopeDistribution

        Shared ReadOnly PeriodicTable As Dictionary(Of String, Element) = Element.MemoryLoadElements
        Shared ReadOnly MaxListLen As Integer = 100000

        Public Property data As CountItem()
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
                                                    Optional interpolate_grid As Double = 0.005) As IsotopeDistribution

            Dim ds As CountItem() = dir(formula, prob_threshold:=prob_threshold)
            Dim xs As Double() = (From d In ds Select CDbl(d(3))).ToArray
            Dim ys As Double() = (From d In ds Select CDbl(d(2))).ToArray
            Dim x_min = xs.Min - pad_left
            Dim x_max = xs.Max + pad_right
            Dim plot_xs = Calculator.frange(x_min, x_max, interpolate_grid).ToArray
            Dim plot_ys = plot_xs.Select(Function(_any) 0.0).ToArray

            For Each i In xs.SeqIterator
                Dim peak_x = i.value

                Console.WriteLine($"peak_x: {peak_x}")
                Dim b = peak_x
                Dim a = ys(i)
                Dim gauss_ys = Calculator.gaussian(plot_xs, a, b, fwhm)
                For Each j In gauss_ys.SeqIterator
                    plot_ys(j) += j.value
                Next
            Next

            Return New IsotopeDistribution With {
                .data = ds,
                .mz = plot_xs,
                .intensity = plot_ys
            }
        End Function

        Private Shared Function dir(formula As Formula, Optional prob_threshold As Double = 0.001) As CountItem()
            Dim lst As New List(Of CountItem)
            Dim atom_types As String()

            For Each a As KeyValuePair(Of String, Integer) In formula.CountsByElement
                Dim atom_type = a.Key
                Dim num_atoms = a.Value
                Dim elt As Element = PeriodicTable(atom_type)
                ' Iterate over isotopes indexed by nominal masses
                For Each isotope As Isotope In elt.isotopes
                    Dim prob = isotope.Prob
                    Dim abs_mass = isotope.Mass
                    Dim nom_mass As Double = isotope.NumNeutrons
                    ' Each iso dist Is made up Of atom types, nominal masses,
                    ' the probability And the mass Of all atoms together.
                    lst.Add(([atom_type], [nom_mass], prob, abs_mass))
                    Dim items_to_append = New List(Of CountItem)
                    For Each itm In lst
                        For Each i In Range(1, num_atoms + 1)
                            atom_types = itm.atom_type.JoinIterates(RepeatString([atom_type], i)).ToArray
                            items_to_append.Add((atom_types, itm(1) + [nom_mass] * i, itm(2) * ((prob) ^ i) * SpecialFunctions.Binom(num_atoms - i, num_atoms), itm(3) + abs_mass * i))
                        Next
                    Next
                    ' prevent addition Of very unlikely isotope distributions
                    items_to_append = items_to_append.Where(Function(itm) itm(2) > prob_threshold).AsList
                    ' prevent duplicates
                    lst = lst + items_to_append.Where(Function(itm) lst.IndexOf(itm) = -1)

                    If Not (lst < MaxListLen) Then
                        Throw New NotImplementedException
                    End If
                Next
                lst = lst.Where(Function(itm) _contains_num_atoms_of_type(itm(0), num_atoms, atom_type)).AsList
            Next

            Return _filter_according_to_sum_formula(lst, formula).ToArray
        End Function

        Private Shared Iterator Function _filter_according_to_sum_formula(lst As List(Of CountItem), sf As Formula) As IEnumerable(Of CountItem)
            For Each itm In lst
                If (From tup In sf.CountsByElement Select _contains_num_atoms_of_type(itm(0), tup.Value, tup.Key)).All Then
                    Yield itm
                End If
            Next
        End Function

        Private Shared Function _contains_num_atoms_of_type(lst_of_atom_types As String(), num_atoms As Integer, atom_type As String) As Boolean
            Dim predicate = (From itm In lst_of_atom_types Where itm = atom_type).Count = num_atoms
            Return predicate
        End Function
    End Class
End Namespace