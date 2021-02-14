Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq

Public Class IonLibrary : Implements Enumeration(Of IonPair)

    Dim dadot3 As Tolerance = Tolerance.DeltaMass(0.3)
    Dim ions As IonPair()

    Sub New(ions As IEnumerable(Of IonPair))
        Me.ions = ions.ToArray
    End Sub

    Public Function GetDisplay(ion As IonPair) As String
        Dim namedIon As IonPair = ions _
            .Where(Function(i)
                       Return i.EqualsTo(ion, dadot3)
                   End Function) _
            .FirstOrDefault
        Dim refId As String

        If namedIon Is Nothing Then
            refId = $"Ion [{ion.precursor}/{ion.product}]"
        Else
            refId = namedIon.name
        End If

        Return refId
    End Function

    Public Function GetIon(precursor As Double, product As Double) As IonPair
        Return ions _
            .Where(Function(i)
                       Return dadot3(precursor, i.precursor) AndAlso
                              dadot3(product, i.product)
                   End Function) _
            .FirstOrDefault
    End Function

    Public Function GetIonByKey(key As String) As IonPair
        If key.StringEmpty Then
            Return Nothing
        ElseIf key.IsPattern("Ion \[.+[/].+\]") Then
            Dim tuples = key.GetStackValue("[", "]").Split("/"c).Select(AddressOf Val).ToArray
            Dim ion As IonPair = GetIon(tuples(0), tuples(1))

            If ion Is Nothing Then
                Return New IonPair With {
                    .accession = key,
                    .name = key,
                    .precursor = tuples(0),
                    .product = tuples(1)
                }
            Else
                Return ion
            End If
        Else
            Return ions _
                .Where(Function(i)
                           Return i.accession = key OrElse i.name = key
                       End Function) _
                .FirstOrDefault
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"'{ions.Length}' ions in library"
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of IonPair) Implements Enumeration(Of IonPair).GenericEnumerator
        For Each ion As IonPair In ions
            Yield ion
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of IonPair).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class
