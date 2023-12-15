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
        Dim result = test.FindCompositions({2247.316306}, Monoisotopic:=True).ToArray

        Call result.GetJson.DoCall(AddressOf Console.WriteLine)
    End Sub
End Module
