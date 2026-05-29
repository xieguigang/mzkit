Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports Microsoft.VisualBasic.Serialization.JSON

Module peptide_test

    Sub Main()
        Dim peptides = PeptideMass.CreateLibrary(4) _
            .AsParallel _
            .Select(Function(s) PeptideMass.CalculateMass(s, "[M+H]+", "[M-H]-", "[M+Na]+", "[M+K]+", "[M+NH4]+")) _
            .ToArray

        Call peptides.GetJson.SaveTo("Z:/peptides.json")

        Pause()
    End Sub
End Module
