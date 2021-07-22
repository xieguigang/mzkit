Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Formula.IsotopicPatterns

    Public Structure IsotopeCount

        Dim atoms As String()
        Dim nom_mass#, prob#

        ''' <summary>
        ''' exact mass
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
            Return $"[{nom_mass}][{Formula.ToString}, {abs_mass}], prob = {prob}"
        End Function

        Public Shared Widening Operator CType(itm As (atom_type As String, nom_mass#, prob#, abs_mass#)) As IsotopeCount
            Return New IsotopeCount With {
                .abs_mass = itm.abs_mass,
                .atoms = {itm.atom_type},
                .nom_mass = itm.nom_mass,
                .prob = itm.prob
            }
        End Operator

        Public Shared Widening Operator CType(itm As (atom_type As String(), nom_mass#, prob#, abs_mass#)) As IsotopeCount
            Return New IsotopeCount With {
                .abs_mass = itm.abs_mass,
                .atoms = itm.atom_type,
                .nom_mass = itm.nom_mass,
                .prob = itm.prob
            }
        End Operator
    End Structure
End Namespace