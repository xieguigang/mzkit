Imports BioNovoGene.BioDeep.Chemoinformatics

Public Class MassGroup : Implements IExactMassProvider

    Public Property exactMass As Double Implements IExactMassProvider.ExactMass
    Public Property name As String

    Public Overrides Function ToString() As String
        Return name
    End Function

End Class
