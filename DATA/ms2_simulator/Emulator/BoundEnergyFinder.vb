Public Class BoundEnergyFinder

    ReadOnly boundTable As Dictionary(Of String, BoundEnergy)

    Public ReadOnly Property min As Double

    Sub New()
        Dim bounds = BoundEnergy.GetEnergyTable.GroupBy(Function(b) b.bound).ToArray
        Dim table As Dictionary(Of String, BoundEnergy) = bounds _
            .ToDictionary(Function(b) b.Key,
                          Function(g)
                              If g.Count = 1 Then
                                  Return g.First
                              Else
                                  Dim energy = Aggregate b In g Into Average(b.H0)
                                  Dim template As BoundEnergy = g.First
                                  Dim avg As New BoundEnergy With {
                                      .atom1 = template.atom1,
                                      .atom2 = template.atom2,
                                      .bound = template.bound,
                                      .bounds = template.bounds,
                                      .H0 = energy
                                  }

                                  Return avg
                              End If
                          End Function)

        boundTable = table
        min = Aggregate g In bounds Into Min(g.Min(Function(b) b.H0))
    End Sub

    ''' <summary>
    ''' Find energy by KCF atoms pair
    ''' </summary>
    ''' <param name="atom1"></param>
    ''' <param name="atom2"></param>
    ''' <returns></returns>
    Public Function FindByKCFAtoms(atom1 As String, atom2 As String) As Double

    End Function
End Class
