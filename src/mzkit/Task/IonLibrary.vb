Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Public Class IonLibrary

    Dim dadot3 As Tolerance = Tolerance.DeltaMass(0.3)
    Dim ions As IonPair()

    Sub New(ions As IEnumerable(Of IonPair))
        Me.ions = ions.ToArray
    End Sub

    Public Function GetDisplay(ion As IonPair) As String
        Dim namedIon As IonPair = ions.Where(Function(i) i.EqualsTo(ion, dadot3)).FirstOrDefault
        Dim refId As String

        If namedIon Is Nothing Then
            refId = $"Ion [{ion.precursor}/{ion.product}]"
        Else
            refId = namedIon.name
        End If

        Return refId
    End Function

End Class
