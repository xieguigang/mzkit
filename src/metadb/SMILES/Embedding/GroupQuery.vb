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
            subgroup = New SubGroup(copy:=subgroup)

            For Each atom As String In subgroup.Elements
                Dim n As Integer = subgroup(atom)

                For i As Integer = 1 To n
                    Dim element As ChemicalElement = GetUnmarked(visited, atom)

                    If element Is Nothing Then
                        Return False
                    End If

                    ' check of the neighbors
                    For Each nei As ChemicalElement In GetNeighbors(element)
                        If Not nei.label Like visited Then
                            If subgroup.CheckElement(nei.elementName) Then
                                subgroup = subgroup - nei.elementName
                            Else

                            End If
                        End If
                    Next
                Next
            Next

            Return True
        End Function

        Private Iterator Function GetNeighbors(element As ChemicalElement) As IEnumerable(Of ChemicalElement)
            For Each bond As ChemicalKey In mol.AllBonds
                If bond.U.ID = element.ID Then
                    Yield bond.V
                ElseIf bond.V.ID = element.ID Then
                    Yield bond.U
                End If
            Next
        End Function

        Private Function GetUnmarked(ByRef visited As Index(Of String), atom As String) As ChemicalElement
            If Not elements.ContainsKey(atom) Then
                Return Nothing
            End If

            For Each candidate As ChemicalElement In elements(atom)
                If Not candidate.label Like visited Then
                    Call visited.Add(candidate.label)
                    Return candidate
                End If
            Next

            Return Nothing
        End Function
    End Class
End Namespace