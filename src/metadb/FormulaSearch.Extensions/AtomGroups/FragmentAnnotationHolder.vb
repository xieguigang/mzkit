Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class FragmentAnnotationHolder

    Public ReadOnly Property name As String
    Public ReadOnly Property exactMass As Double
    Public ReadOnly Property annotation As IExactMassProvider

    Sub New(anno As IExactMassProvider)
        exactMass = anno.ExactMass
        annotation = anno

        If TypeOf anno Is MassGroup Then
            name = DirectCast(anno, MassGroup).name
        ElseIf TypeOf anno Is Formula Then
            name = DirectCast(anno, Formula).EmpiricalFormula
        Else
            Throw New NotImplementedException(anno.GetType.FullName)
        End If
    End Sub

End Class
