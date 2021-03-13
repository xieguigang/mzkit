Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS

Public Module GCMSQuantifyIon

    <Extension>
    Public Function GetIon(ions As Dictionary(Of String, QuantifyIon), id As String) As QuantifyIon
        Dim ion As QuantifyIon = ions.TryGetValue(id)

        If ion Is Nothing Then
            If id.IsPattern("[0-9.][/][0-9.]") Then
                ion = New QuantifyIon With {
                    .id = id,
                    .name = id,
                    .ms = {},
                    .rt = id.Split("/"c).Select(AddressOf Val).ToArray
                }
            Else
                ion = New QuantifyIon With {
                    .id = id,
                    .ms = {},
                    .name = id,
                    .rt = {-100, -100}
                }
            End If
        End If

        Return ion
    End Function
End Module
