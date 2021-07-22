Namespace Formula.IsotopicPatterns

    Public Structure CountItem

        Dim atom_type As String()
        Dim nom_mass#, prob#, abs_mass#

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