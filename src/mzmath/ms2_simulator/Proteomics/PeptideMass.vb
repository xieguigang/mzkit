Imports BioNovoGene.BioDeep.Chemoinformatics
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public Module PeptideMass

    Public Iterator Function CreateLibrary(len As Integer) As IEnumerable(Of MetaboliteAnnotation)
        For Each prot As String In Polypeptide.Generate(len)
            Yield New MetaboliteAnnotation With {
                .Id = prot,
                .CommonName = prot,
                .Formula = MolecularWeightCalculator.PolypeptideFormula(prot).ToString,
                .ExactMass = MolecularWeightCalculator.CalcMW_Polypeptide(prot)
            }
        Next
    End Function

End Module
