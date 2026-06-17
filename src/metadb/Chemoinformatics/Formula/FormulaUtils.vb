' ========================================================================
' 分子式解析与同位素预测工具模块
' ========================================================================

Namespace Formula

    ''' <summary>
    ''' 分子式解析与单同位素质量/同位素模式计算工具
    ''' </summary>
    Public Module FormulaUtils

        ' --- 同位素质量差 (相对于最丰富同位素) ---
        Public ReadOnly C13_DELTA As Double = 1.0033548378
        Public ReadOnly N15_DELTA As Double = 0.9970349
        Public ReadOnly O18_DELTA As Double = 1.995794
        Public ReadOnly S34_DELTA As Double = 1.995796
        Public ReadOnly CL37_DELTA As Double = 1.9970499
        Public ReadOnly BR81_DELTA As Double = 1.9979528
        Public ReadOnly H2_DELTA As Double = 1.0062767

        ' --- 同位素自然丰度 (分数) ---
        Public ReadOnly C13_ABUNDANCE As Double = 0.0107
        Public ReadOnly N15_ABUNDANCE As Double = 0.00368
        Public ReadOnly O18_ABUNDANCE As Double = 0.00205
        Public ReadOnly S34_ABUNDANCE As Double = 0.0421
        Public ReadOnly CL37_ABUNDANCE As Double = 0.2422
        Public ReadOnly BR81_ABUNDANCE As Double = 0.4929
        Public ReadOnly H2_ABUNDANCE As Double = 0.000115

        ''' <summary>
        ''' 预测同位素模式 (M+1, M+2相对于M的强度比)
        ''' <para>
        ''' 简化模型:
        '''   M+1强度 ≈ 1.1%×nC + 0.37%×nN + 0.015%×nH + 0.04%×nO + 0.75%×nS
        '''   M+2强度 ≈ (1.1%×nC)²/2 + 4.2%×nS + 0.2%×nO + 32.5%×nCl + 97.3%×nBr
        ''' </para>
        ''' </summary>
        ''' <param name="formula">分子式</param>
        ''' <returns>包含(M+1/M, M+2/M)强度比的数组</returns>
        Public Function PredictIsotopeRatios(formula As String) As Double()
            Dim atoms = FormulaScanner.ScanFormula(formula)
            Dim nC As Integer = If(atoms.CheckElement("C"), atoms("C"), 0)
            Dim nH As Integer = If(atoms.CheckElement("H"), atoms("H"), 0)
            Dim nN As Integer = If(atoms.CheckElement("N"), atoms("N"), 0)
            Dim nO As Integer = If(atoms.CheckElement("O"), atoms("O"), 0)
            Dim nS As Integer = If(atoms.CheckElement("S"), atoms("S"), 0)
            Dim nCl As Integer = If(atoms.CheckElement("Cl"), atoms("Cl"), 0)
            Dim nBr As Integer = If(atoms.CheckElement("Br"), atoms("Br"), 0)

            ' M+1 相对强度 (相对于M峰=1.0)
            Dim m1 As Double = nC * C13_ABUNDANCE +
                          nN * N15_ABUNDANCE +
                          nH * H2_ABUNDANCE +
                          nO * 0.00038 +
                          nS * 0.0076

            ' M+2 相对强度
            ' 包含: 2个C13的组合 + S34 + Cl37 + Br81 + O18
            Dim m2 As Double = (nC * C13_ABUNDANCE) ^ 2 / 2.0 +
                          nS * S34_ABUNDANCE +
                          nCl * CL37_ABUNDANCE +
                          nBr * BR81_ABUNDANCE +
                          nO * O18_ABUNDANCE

            Return New Double() {m1, m2}
        End Function

        ''' <summary>
        ''' 获取同位素峰的理论m/z偏移
        ''' <para>M+1主要由13C贡献, M+2由2个13C或34S等贡献</para>
        ''' </summary>
        Public Function GetIsotopeMassDelta(isotopeOrder As Integer) As Double
            Select Case isotopeOrder
                Case 1 : Return C13_DELTA   ' M+1
                Case 2 : Return 2 * C13_DELTA ' M+2 (近似)
                Case Else : Return isotopeOrder * C13_DELTA
            End Select
        End Function
    End Module
End Namespace