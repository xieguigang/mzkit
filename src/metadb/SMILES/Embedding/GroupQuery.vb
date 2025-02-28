Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SubGroup = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula

Namespace Embedding

    Public Class GroupQuery

        ReadOnly mol As ChemicalFormula
        ReadOnly elements As Dictionary(Of String, ChemicalElement())

        Sub New(mol As ChemicalFormula)
            Me.mol = mol
            Me.elements = mol.AllElements _
                .GroupBy(Function(a) a.elementName) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function)
        End Sub

        ''' <summary>
        ''' Check of the given sub-group of the atom elements is existsed inside the specific molecule?
        ''' </summary>
        ''' <param name="subgroup"></param>
        ''' <returns></returns>
        Public Function CheckSubGroup(subgroup As SubGroup) As Boolean
            Dim visited As New Index(Of String)
            ' make value copy
            subgroup = New SubGroup(subgroup)

            For Each atom As String In subgroup.Elements
                Dim n As Integer = subgroup(atom)

                For i As Integer = 1 To n
                    Dim element As ChemicalElement = GetUnmarked(visited, atom)

                    If element Is Nothing Then
                        Return False
                    End If
                Next
            Next

            Return True
        End Function

        Private Function GetUnmarked(ByRef visited As Index(Of String), atom As String) As ChemicalElement

        End Function
    End Class
End Namespace