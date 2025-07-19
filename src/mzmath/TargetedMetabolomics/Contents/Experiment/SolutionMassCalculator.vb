Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Content

    Public Class SolutionMassCalculator

        ReadOnly Chemical_reagents As Dictionary(Of String, Double)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Chemical_reagents"></param>
        ''' <param name="useExactMass"></param>
        Sub New(Chemical_reagents As Dictionary(Of String, String), Optional useExactMass As Boolean = False)
            If useExactMass Then
                Me.Chemical_reagents = Chemical_reagents.ToDictionary(Function(a) a.Key, Function(a) FormulaScanner.EvaluateExactMass(a.Value))
            End If
        End Sub

        Public Iterator Function CalculateSolutionMasses(target As Dictionary(Of String, Double), VL As Double, unitType As ConcentrationType) As IEnumerable(Of NamedValue(Of Double))
            Dim volumeLiters As Double = VL / 1000  ' 将毫升转换为升

            For Each component As KeyValuePair(Of String, Double) In target
                Dim reagentName As String = component.Key
                Dim concentration As Double = component.Value

                If Chemical_reagents.ContainsKey(reagentName) Then
                    Dim molecularWeight As Double = Chemical_reagents(reagentName)
                    Dim mass As Double

                    Select Case unitType
                        Case ConcentrationType.molL
                            ' 计算公式：质量(g) = 浓度(mol/L) × 分子量(g/mol) × 体积(L)[2,3](@ref)
                            mass = concentration * molecularWeight * volumeLiters

                        Case ConcentrationType.gL
                            ' 计算公式：质量(g) = 浓度(g/L) × 体积(L)[4](@ref)
                            mass = concentration * volumeLiters

                    End Select

                    Yield New NamedValue(Of Double)(reagentName, mass)
                Else
                    Throw New KeyNotFoundException("Molecular weight for reagent '{reagentName}' not found in chemical reagent database.")
                End If
            Next
        End Function
    End Class
End Namespace