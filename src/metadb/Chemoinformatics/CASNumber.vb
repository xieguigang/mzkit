Imports Microsoft.VisualBasic.Language

''' <summary>
''' Chemical Abstracts Service Number of a specific metabolite
''' </summary>
Public Class CASNumber

    ''' <summary>
    ''' CAS号（第一、二部分数字）的最后一位乘以1，最后第二位乘以2，往前依此类推，
    ''' 然后再把所有的乘积相加，和除以10，余数就是第三部分的校验码。举例来说，
    ''' 水（H2O）的CAS编号前两部分是7732-18，则其校验码
    ''' ( 8×1 + 1×2 + 2×3 + 3×4 + 7×5 + 7×6 ) /10 = 105/10 = 10余5，
    ''' 所以水的CAS号为7732-18-5。
    ''' </summary>
    ''' <param name="cas"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 第一部分有2到7位数字，第二部分有2位数字，第三部分有1位数字作为校验码
    ''' </remarks>
    Public Shared Function Verify(cas As String) As Boolean
        If cas.StringEmpty Then
            Return False
        End If

        Dim tokens As String() = cas.Split("-"c)

        If Not tokens.Any(Function(si) si.IsInteger) Then
            Return False
        End If

        Dim n As Long = 0
        Dim i As i32 = 1

        For Each ni As Char In tokens.Take(2).JoinBy("").Reverse
            n += Integer.Parse(CStr(ni)) * (++i)
        Next

        Dim code As Integer = n Mod 10
        Dim code2 As Integer = Integer.Parse(tokens(2))

        Return code = code2
    End Function
End Class
