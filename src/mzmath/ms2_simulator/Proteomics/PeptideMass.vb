Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.Polypeptides
Imports std = System.Math

Public Class PeptideMass

    Public Property id As String
    Public Property name As String
    Public Property formula As String
    Public Property exact_mass As Double
    Public Property sequence As String
    Public Property precursors As Dictionary(Of String, Double)
    Public Property fragments As PeptideMass()
    Public Property intensity As Double

    Public Overrides Function ToString() As String
        Return $"{sequence} [{formula}, {precursors.GetJson}]"
    End Function

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
        Dim adducts = CalculateMass(peptide.ExactMass, adductTypes)
        Dim fragments As PeptideMass() = CreateFragments(peptide.Id) _
            .GroupBy(Function(s) s) _
            .Select(Function(group)
                        Return CalculateSubMassInternal(group.Key, group.Count, adductTypes)
                    End Function) _
            .ToArray

        Return New PeptideMass With {
            .exact_mass = std.Round(peptide.ExactMass, 4),
            .formula = peptide.Formula,
            .id = peptide.Id,
            .name = peptide.CommonName,
            .sequence = peptide.Id,
            .precursors = adducts,
            .intensity = 1,
            .fragments = fragments
        }
    End Function

    Private Shared Function CalculateMass(peptide As Double, adductTypes As String()) As Dictionary(Of String, Double)
        Dim adducts As New Dictionary(Of String, Double)

        Static adductSet As New Dictionary(Of String, MzCalculator)

        For Each type As String In adductTypes
            adducts(type) = std.Round(adductSet _
                .ComputeIfAbsent(type, Function(name) Provider.ParseAdductModel(name)) _
                .CalcMZ(peptide), 4)
        Next

        Return adducts
    End Function

    Private Shared Function CalculateSubMassInternal(seq As String, n As Integer, adductTypes As String()) As PeptideMass
        Return New PeptideMass With {
            .id = seq,
            .name = seq,
            .sequence = seq,
            .exact_mass = std.Round(MolecularWeightCalculator.CalcMW_Polypeptide(seq), 4),
            .formula = MolecularWeightCalculator.PolypeptideFormula(seq).ToString,
            .intensity = n,
            .precursors = CalculateMass(.exact_mass, adductTypes)
        }
    End Function

    Private Shared Iterator Function CreateFragments(seq As String) As IEnumerable(Of String)
        If seq.Length = 0 Then
            Return
        Else
            Yield seq
        End If

        Yield seq.First.ToString

        For Each [sub] As String In CreateFragments(seq.Substring(1))
            Yield [sub]
        Next
        For Each [sub] As String In CreateFragments(seq.Substring(0, seq.Length - 1))
            Yield [sub]
        Next

        Yield seq.Last.ToString
    End Function

End Class
