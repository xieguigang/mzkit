Public Module MolecularMechanics

    ''' <summary>
    ''' 哈默斯-芬克（Harmes-Finkel）势能函数
    ''' 
    ''' 哈默斯定律势能函数 for 键长，哈默斯-芬克势能函数通常用于描述键长和键角的势能
    ''' </summary>
    ''' <param name="k">力常数</param>
    ''' <param name="r">当前键长</param>
    ''' <param name="r0">平衡键长</param>
    ''' <returns>势能 V = 0.5 * k * (r - r0)^2</returns>
    Public Function CalculateHarmesFinkelPotential(k As Double, r As Double, r0 As Double) As Double
        ' k 是力常数
        ' r 是当前键长
        ' r0 是平衡键长
        ' 势能 V = 0.5 * k * (r - r0)^2
        Return 0.5 * k * (r - r0) ^ 2
    End Function

    ''' <summary>
    ''' 伦纳德-琼斯势能函数
    ''' 
    ''' 伦纳德-琼斯（Lennard-Jones）势能函数，伦纳德-琼斯势能函数用于描述非键相互作用，特别是范德华力。
    ''' </summary>
    ''' <param name="epsilon">
    ''' 势能阱的深度
    ''' </param>
    ''' <param name="sigma">分子间的范德华半径</param>
    ''' <param name="r">分子间的距离</param>
    ''' <returns>势能 V = 4 * epsilon * [(sigma / r)^12 - (sigma / r)^6]</returns>
    Public Function CalculateLennardJonesPotential(epsilon As Double, sigma As Double, r As Double) As Double
        ' epsilon 是势能阱的深度
        ' sigma 是分子间的范德华半径
        ' r 是分子间的距离
        ' 势能 V = 4 * epsilon * [(sigma / r)^12 - (sigma / r)^6]
        Dim sigmaOverR6 As Double = (sigma / r) ^ 6
        Dim sigmaOverR12 As Double = sigmaOverR6 ^ 2
        Return 4 * epsilon * (sigmaOverR12 - sigmaOverR6)
    End Function
End Module
