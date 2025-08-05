Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public Class PeptideMass

    Public Property id As String
    Public Property name As String
    Public Property formula As String
    Public Property exact_mass As Double
    Public Property sequence As String
    Public Property precursors As Dictionary(Of String, Double)

    Public Shared Iterator Function CreateLibrary(len As Integer) As IEnumerable(Of MetaboliteAnnotation)
        For Each prot As String In Polypeptide.Generate(len)
            Yield New MetaboliteAnnotation With {
                .Id = prot,
                .CommonName = prot,
                .Formula = MolecularWeightCalculator.PolypeptideFormula(prot).ToString,
                .ExactMass = MolecularWeightCalculator.CalcMW_Polypeptide(prot)
            }
        Next
    End Function

    Public Shared Function CalculateMass(peptide As MetaboliteAnnotation, ParamArray adductTypes As String()) As PeptideMass
        Dim adducts As New Dictionary(Of String, Double)

        Static adductSet As New Dictionary(Of String, MzCalculator)

        For Each type As String In adductTypes
            adducts(type) = adductSet _
                .ComputeIfAbsent(type, Function(name) Provider.ParseAdductModel(name)) _
                .CalcMZ(peptide.ExactMass)
        Next

        Return New PeptideMass With {
            .exact_mass = peptide.ExactMass,
            .formula = peptide.Formula,
            .id = peptide.Id,
            .name = peptide.CommonName,
            .sequence = peptide.Id,
            .precursors = adducts
        }
    End Function

End Class
