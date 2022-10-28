Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class FragmentAnnotationHolder

    Public ReadOnly Property name As String
    Public ReadOnly Property exactMass As Double
    Public ReadOnly Property base As IExactMassProvider

    Sub New(anno As IExactMassProvider, Optional name As String = Nothing)
        exactMass = anno.ExactMass
        base = anno

        If TypeOf anno Is MassGroup Then
            Me.name = If(name.StringEmpty, DirectCast(anno, MassGroup).name, name)
        ElseIf TypeOf anno Is Formula Then
            Me.name = If(name.StringEmpty, DirectCast(anno, Formula).EmpiricalFormula, name)
        Else
            Throw New NotImplementedException(anno.GetType.FullName)
        End If
    End Sub

    Private Sub New(name As String, exactMass As Double, base As IExactMassProvider)
        Me.name = name
        Me.exactMass = exactMass
        Me.base = base
    End Sub

    Public Overrides Function ToString() As String
        Return name
    End Function

    Public Shared Operator Like(a As FragmentAnnotationHolder, b As FragmentAnnotationHolder) As Boolean
        If a.base.GetType Is b.base.GetType Then
            Return True
        Else
            Return False
        End If
    End Operator

    Public Shared Operator *(formula As FragmentAnnotationHolder, n As Integer) As FragmentAnnotationHolder
        Dim name As String = $"({formula.name}){n}"
        Dim mass As Double = formula.exactMass * n

        Return New FragmentAnnotationHolder(name, mass, formula.base)
    End Operator

    Private Shared Function getReferMathName(name As String) As String
        If name.Contains("+"c) OrElse name.Contains("-"c) Then
            If name.First = "("c AndAlso name.Last = ")"c Then
                Return name
            Else
                Return $"({name})"
            End If
        Else
            Return name
        End If
    End Function

    Public Shared Operator +(a As FragmentAnnotationHolder, b As FragmentAnnotationHolder) As FragmentAnnotationHolder
        Dim newName As String = $"{getReferMathName(a.name)}+{getReferMathName(b.name)}"

        If TypeOf a.base Is Formula AndAlso a Like b Then
            Dim newBase As Formula = DirectCast(a.base, Formula) + DirectCast(b.base, Formula)
            Dim newMass As Double = newBase.ExactMass

            Return New FragmentAnnotationHolder(newName, newMass, newBase)
        Else
            Dim newGroup As New MassGroup With {
                .name = newName,
                .exactMass = a.exactMass + b.exactMass
            }

            Return New FragmentAnnotationHolder(newGroup)
        End If
    End Operator

    Public Shared Operator -(a As FragmentAnnotationHolder, b As FragmentAnnotationHolder) As FragmentAnnotationHolder
        Dim newName As String = $"{getReferMathName(a.name)}-{getReferMathName(b.name)}"

        If TypeOf a.base Is Formula AndAlso a Like b Then
            Dim newBase As Formula = DirectCast(a.base, Formula) - DirectCast(b.base, Formula)
            Dim newMass As Double = newBase.ExactMass

            Return New FragmentAnnotationHolder(newName, newMass, newBase)
        Else
            Dim newGroup As New MassGroup With {
                .name = newName,
                .exactMass = a.exactMass - b.exactMass
            }

            Return New FragmentAnnotationHolder(newGroup)
        End If
    End Operator

End Class
