Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Statistics

Namespace Formula.IsotopicPatterns

    Public Class IsotopeDistribution

        Shared ReadOnly pse As Dictionary(Of String, Element) = Element.MemoryLoadElements
        Shared ReadOnly _max_list_len As Integer = 100000

        Public Property ds As CountItem()
        Public Property xs As Double()
        Public Property ys As Double()

        Public Shared Function generate(sum_formula As Formula, Optional prob_threshold As Double = 0.001, Optional fwhm As Double = 0.1,
                                Optional pad_left As Double = 3,
                Optional pad_right As Double = 3, Optional interpolate_grid As Double = 0.005) As IsotopeDistribution
            Console.WriteLine("Simulating isotopic distribution ...")

            Dim ds As CountItem() = _generate_dir(sum_formula, prob_threshold = prob_threshold)

            Console.WriteLine("Simulating gaussians...")

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
                .ds = ds,
                .xs = plot_xs,
                .ys = plot_ys
            }
        End Function

        Private Shared Function _generate_dir(sum_formula As Formula, Optional prob_threshold As Double = 0.001) As CountItem()
            Dim lst As New List(Of CountItem)

            For Each a As KeyValuePair(Of String, Integer) In sum_formula.CountsByElement
                Dim atom_type = a.Key
                Dim num_atoms = a.Value
                Dim elt As Element = pse(atom_type)
                ' Iterate over isotopes indexed by nominal masses
                For Each isotope As Isotope In elt.isotopes
                    Dim prob = isotope.Prob
                    Dim abs_mass = isotope.Mass
                    Dim nom_mass As Double = isotope.NumNeutrons
                    ' Each iso dist Is made up Of atom types, nominal masses,
                    ' the probability And the mass Of all atoms together.
                    lst.Append(([atom_type], [nom_mass], prob, abs_mass))
                    Dim items_to_append = New List(Of CountItem)
                    For Each itm In lst
                        For Each i In Range(1, num_atoms + 1)
                            items_to_append.Append(({itm(0)}.JoinIterates(RepeatString([atom_type], i)).ToArray, itm(1) + [nom_mass] * i, itm(2) * ((prob) ^ i) * SpecialFunctions.Binom(num_atoms - i, num_atoms), itm(3) + abs_mass * i))
                        Next
                    Next
                    ' prevent addition Of very unlikely isotope distributions
                    items_to_append = items_to_append.Where(Function(itm) itm(2) > prob_threshold).AsList
                    ' prevent duplicates
                    lst = lst + items_to_append.Where(Function(itm) lst.IndexOf(itm) = -1)

                    If Not (lst < _max_list_len) Then
                        Throw New NotImplementedException
                    End If
                Next
                lst = lst.Where(Function(itm) _contains_num_atoms_of_type(itm(0), num_atoms, atom_type))
            Next

            Return _filter_according_to_sum_formula(lst, sum_formula).ToArray
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

    Public Structure CountItem

        Dim atom_type(), nom_mass#, prob#, abs_mass#

        Default Public ReadOnly Property Item(i As Integer) As Object
            Get
                Select Case i
                    Case 0 : Return atom_type
                    Case 1 : Return nom_mass
                    Case 2 : Return prob
                    Case 3 : Return abs_mass
                    Case Else
                        Throw New NotImplementedException
                End Select
            End Get
        End Property

        Public Shared Widening Operator CType(itm As (atom_type As String, nom_mass#, prob#, abs_mass#)) As CountItem
            Return New CountItem With {
                .abs_mass = itm.abs_mass,
                .atom_type = {itm.atom_type},
                .nom_mass = itm.nom_mass,
                .prob = itm.prob
            }
        End Operator

        Public Shared Widening Operator CType(itm As (atom_type As String(), nom_mass#, prob#, abs_mass#)) As CountItem
            Return New CountItem With {
                .abs_mass = itm.abs_mass,
                .atom_type = itm.atom_type,
                .nom_mass = itm.nom_mass,
                .prob = itm.prob
            }
        End Operator
    End Structure
End Namespace