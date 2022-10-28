Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class MetaboliteAnnotation
    Implements IReadOnlyId, ICompoundNameProvider, IExactMassProvider, IFormulaProvider

    Public Property UniqueId As String Implements IReadOnlyId.Identity
    Public Property CommonName As String Implements ICompoundNameProvider.CommonName
    Public Property ExactMass As Double Implements IExactMassProvider.ExactMass
    Public Property Formula As String Implements IFormulaProvider.Formula

    Public Overrides Function ToString() As String
        Return $"[{UniqueId}] {CommonName}"
    End Function

End Class
