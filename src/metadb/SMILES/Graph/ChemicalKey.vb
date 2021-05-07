Imports Microsoft.VisualBasic.Data.GraphTheory.Network

Public Class ChemicalKey : Inherits Edge(Of ChemicalElement)

    Public Property bond As Bonds

    Public Overrides Function ToString() As String
        Return $"{U.elementName}{bond.Description}{V.elementName} (+{CInt(bond)})"
    End Function

End Class
