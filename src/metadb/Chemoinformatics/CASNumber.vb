#Region "Microsoft.VisualBasic::6d004b17bc7690af452da19b0c811a52, G:/mzkit/src/metadb/Chemoinformatics//CASNumber.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 43
    '    Code Lines: 20
    ' Comment Lines: 15
    '   Blank Lines: 8
    '     File Size: 1.48 KB


    ' Class CASNumber
    ' 
    '     Function: Verify
    ' 
    ' /********************************************************************************/

#End Region

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

