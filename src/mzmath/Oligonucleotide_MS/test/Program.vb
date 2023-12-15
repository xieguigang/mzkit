Imports System
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Oligonucleotide_MS
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program
    Sub Main(args As String())
        Oligonucleotide_Composition_from_Mass_Calculator_v2()
    End Sub

    Sub Oligonucleotide_Composition_from_Mass_Calculator_v2()
        Dim test As New Composition(ppm:=5)
        Dim result = test.FindCompositions({2247.316306, 2167.34997497712, 1838.297, 2798.42}, Monoisotopic:=True).ToArray

        For Each hit In result
            Call hit.GetJson.DoCall(AddressOf Console.WriteLine)
        Next
    End Sub
End Module
