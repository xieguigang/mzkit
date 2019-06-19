Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Public Class BoundEnergy

    ' Bonds,H0*,bounds.n,atom1,atom2,Comments

    <Column("Bounds")>
    Public Property bound As String
    ''' <summary>
    ''' Energy consumption
    ''' </summary>
    ''' <returns></returns>
    <Column("H0*")>
    Public Property H0 As Double
    <Column("bounds.n")>
    Public Property bounds As Integer
    Public Property atom1 As String
    Public Property atom2 As String
    Public Property comments As String

    Public Overrides Function ToString() As String
        Return $"Dim {bound} = {H0}"
    End Function

    Public Shared Function GetEnergyTable() As BoundEnergy()
        Return My.Resources.Standard_bond_energies _
            .LineTokens _
            .AsDataSource(Of BoundEnergy) _
            .ToArray
    End Function

End Class
