Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports stdNum = System.Math

Public Class FormulaSearch

    ReadOnly opts As SearchOption
    ReadOnly elements As Dictionary(Of String, Element)

    Sub New(opts As SearchOption)
        Me.opts = opts
        Me.elements = Element.MemoryLoadElements
    End Sub

    Public Iterator Function SearchByExactMass(exact_mass As Double) As IEnumerable(Of FormulaComposition)
        Dim elements As New Stack(Of ElementSearchCandiate)(opts.candidateElements)
        Dim seed As New FormulaComposition(New Dictionary(Of String, Integer), "")

        For Each formula As FormulaComposition In SearchByExactMass(exact_mass, seed, elements)
            If opts.chargeRange.IsInside(formula.charge) Then
                Yield formula
            End If
        Next
    End Function

    Private Iterator Function SearchByExactMass(exact_mass As Double, parent As FormulaComposition, candidates As Stack(Of ElementSearchCandiate)) As IEnumerable(Of FormulaComposition)
        Dim current As ElementSearchCandiate = candidates.Pop
        Dim isto As Double = elements(current.Element).isotopic

        For n As Integer = current.MinCount To current.MaxCount
            Dim formula As FormulaComposition = parent.AppendElement(current.Element, n)
            Dim ppm As Double = FormulaSearch.PPM(formula.exact_mass, exact_mass)

            If ppm <= opts.ppm Then
                formula.ppm = ppm
                ' populate current formula that match exact mass ppm condition
                Yield formula
            ElseIf formula.exact_mass < exact_mass Then
                If candidates.Count > 0 Then
                    ' 还可以再增加分子质量
                    For Each subtree In SearchByExactMass(exact_mass, parent:=formula, New Stack(Of ElementSearchCandiate)(candidates))
                        Yield subtree
                    Next
                End If
            End If
        Next
    End Function

    Public Overrides Function ToString() As String
        Return opts.ToString
    End Function

    ''' <summary>
    ''' 分子量差值
    ''' </summary>
    ''' <param name="measured#"></param>
    ''' <param name="actualValue#"></param>
    ''' <returns></returns>
    Public Overloads Shared Function PPM(measured#, actualValue#) As Double
        ' （测量值-实际分子量）/ 实际分子量
        ' |(实验数据 - 数据库结果)| / 实验数据 * 1000000
        Dim ppmd# = (stdNum.Abs(measured - actualValue) / actualValue) * 1000000

        If ppmd < 0 Then
            ' 计算溢出了
            Return 10000000000
        End If

        Return ppmd
    End Function
End Class
