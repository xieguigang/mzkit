Imports BioNovoGene.Analytical.MassSpectrometry.Math.AtomGroups
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Friend Class ParentValue

    Friend ReadOnly parentMz As Double
    Friend ReadOnly formula As Formula

    Sub New(parentMz As Double, formula As String)
        Me.parentMz = parentMz
        Me.formula = FormulaScanner.ScanFormula(formula)
    End Sub

    Public Function TestFragments(frag As String) As Boolean
        If formula Is Nothing Then
            Return True
        End If

        Return True
    End Function

    Public Function GetFragment(candidates As IEnumerable(Of FragmentAnnotationHolder)) As FragmentAnnotationHolder

    End Function
End Class