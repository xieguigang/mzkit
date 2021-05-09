Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.Massbank.KNApSAcK.Data
Imports BioNovoGene.BioDeep.Chemoinformatics.NaturalProduct

Public Module Extensions

    <Extension>
    Public Function CreateReference(data As Information, solver As GlycosylNameSolver) As KNApSAcKRef
        Dim chemicalName As String = Nothing
        Dim table As InformationTable = InformationTable.FromDetails(data, solver, chemicalName)
        Dim glycosyl As String() = table.glycosyl _
            .GroupBy(Function(n) n) _
            .Select(Function(n) $"{n.Count} {n.Key}") _
            .ToArray
        Dim ref As New KNApSAcKRef With {
            .CAS = table.CAS.FirstOrDefault,
            .exact_mass = table.exact_mass,
            .formula = table.formula,
            .InChI = table.InChI,
            .InChIKey = table.InChIKey,
            .SMILES = table.SMILES,
            .name = chemicalName,
            .xrefId = table.CID,
            .glycosyl = glycosyl
        }

        Return ref
    End Function

End Module
