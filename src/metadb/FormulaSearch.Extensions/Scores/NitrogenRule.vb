Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Class NitrogenRule

    ''' <summary>
    ''' 若有机化合物有偶数个N原子或不含N原子，则其分子离子的质量是偶数；
    ''' 含奇数个N原子，其质量数是奇数。质谱中最高质量峰不符合氮律就不是
    ''' 分子离子峰。
    ''' </summary>
    ''' <param name="exact_mass#"></param>
    ''' <param name="formula"></param>
    ''' <returns></returns>
    Public Shared Function TestRule(exact_mass#, formula As Formula) As Boolean
        Dim isEven As Boolean = CInt(exact_mass) Mod 2 = 0

        If isEven Then
            Return formula("N") = 0 OrElse formula("N") Mod 2 = 0
        Else
            Return formula("N") Mod 2 <> 0
        End If
    End Function
End Class
