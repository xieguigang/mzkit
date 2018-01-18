Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting

Public Module PeakArea

    ''' <summary>
    ''' ``B + A = S``
    ''' </summary>
    ''' <param name="chromatogram"></param>
    ''' <param name="peak">The time range of the peak</param>
    ''' <param name="baseline">The quantile threshold of the baseline</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function PeakArea(chromatogram As VectorModel(Of ChromatogramTick), peak As DoubleRange, Optional baseline# = 0.65) As Double
        Dim S = chromatogram((chromatogram!Time >= peak.Min) & (chromatogram!Time <= peak.Max))  ' TPA
        Dim B = chromatogram.Base(quantile:=baseline)
        Dim A = Aggregate signal As ChromatogramTick
                In S
                Let PA = signal.Intensity - B
                Into Sum(PA)

        Return A
    End Function

    ''' <summary>
    ''' 只是返回peak的顶点坐标和顶点值
    ''' </summary>
    ''' <param name="problem#"></param>
    ''' <param name="left%"></param>
    ''' <param name="right%"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' https://github.com/fekberg/Algorithms/blob/master/Peak%20Finding/Peak%20Finding/Program.cs
    ''' </remarks>
    <Extension>
    Public Function FindPeak(problem#()(), Optional left% = 0, Optional right% = -1) As Double
        If problem.Length <= 0 Then
            Return 0
        End If
        If right = -1 Then
            right = problem(0).Length
        End If

        Dim j% = (left + right) / 2
        Dim globalMax = problem.FindGlobalMax(j)

        If (globalMax - 1 > 0 AndAlso problem(globalMax)(j) >= problem(globalMax - 1)(j)) AndAlso
           (globalMax + 1 < problem.Length AndAlso problem(globalMax)(j) >= problem(globalMax + 1)(j)) AndAlso
           (j - 1 > 0 AndAlso problem(globalMax)(j) >= problem(globalMax)(j - 1)) AndAlso
           (j + 1 < problem(globalMax).Length AndAlso problem(globalMax)(j) >= problem(globalMax)(j + 1)) Then

            Return problem(globalMax)(j)
        ElseIf j > 0 AndAlso problem(globalMax)(j - 1) > problem(globalMax)(j) Then
            right = j
            Return FindPeak(problem, left, right)
        ElseIf j + 1 < problem(globalMax).Length AndAlso problem(globalMax)(j + 1) > problem(globalMax)(j) Then
            left = j
            Return FindPeak(problem, left, right)
        End If

        Return problem(globalMax)(j)
    End Function

    <Extension>
    Public Function FindGlobalMax(problem#()(), column%) As Integer
        Dim max#, index%

        For i As Integer = 0 To problem.Length - 1
            If max < problem(i)(column) Then
                max = problem(i)(column)
                index = i
            End If
        Next

        Return index
    End Function
End Module
