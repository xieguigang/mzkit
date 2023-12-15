Imports BioNovoGene.Analytical.MassSpectrometry.Math.Oligonucleotide_MS
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program
    Sub Main(args As String())
        Oligonucleotide_Composition_from_Mass_Calculator_v2()
    End Sub

    Sub Oligonucleotide_Composition_from_Mass_Calculator_v2()
        Dim test As New Composition(ppm:=5)
        Dim result = test.FindCompositions(2247.316306, 2167.34997497712, 1838.297, 2798.42).ToArray

        ' Observed Mass	Theoretical Mass	Error (ppm)	# of pA	# of pG	# of pC	# of pV	Modification		# of Bases
        ' 2247.316	2247.316	0.0	1	1	3	2			7
        ' 2167.350	2167.350	0.0	1	1	3	2	minus p		7
        ' 1838.297	1838.297	0.0	0	1	3	2	minus p		6
        ' 2798.420	2798.420	0.0	0	1	3	5	minus p		9

        'For Each hit In result
        '    Call hit.GetJson.DoCall(AddressOf Console.WriteLine)
        'Next
        Call OligonucleotideCompositionOutput.Print(result, dev:=App.StdOut)
    End Sub
End Module
