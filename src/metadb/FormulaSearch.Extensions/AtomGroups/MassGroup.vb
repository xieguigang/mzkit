Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics

Public Class MassGroup : Implements IExactMassProvider

    Public Property exactMass As Double Implements IExactMassProvider.ExactMass
    Public Property name As String

    Public Overrides Function ToString() As String
        Return name
    End Function

    Public Shared Function CreateAdducts(anno As FragmentAnnotationHolder, adducts As MzCalculator) As MassGroup
        Dim name As String = $"[{anno.name}{adducts.name}]{adducts.charge}{adducts.mode}"
        Dim mass As Double = adducts.CalcMZ(anno.exactMass)

        Return New MassGroup With {
            .exactMass = mass,
            .name = name
        }
    End Function

End Class
