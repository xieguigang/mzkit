Imports BioNovoGene.BioDeep.Chemoinformatics.MorganFingerprint
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models

Module fingerprintTest

    Sub Main333()
        Dim struct As New [Structure] With {.Atoms = {
            New Atom("C"),
            New Atom("O"),
            New Atom("O"),
            New Atom("H"),
            New Atom("H")
        },
        .Bounds = {
            New Bound(0, 1, BoundTypes.Double),
            New Bound(0, 2),
            New Bound(0, 3),
            New Bound(2, 4)
        }}

        Dim hash = struct.CalculateFingerprint
        Dim bytes = hash.

        Call Console.WriteLine(BitConverter.ToString(hash))
    End Sub
End Module
