''' <summary>
''' LC-MS表达矩阵数据预处理模块 - 数学工具函数库
''' 
''' 提供预处理流程所需的基础数学运算函数，包括：
''' - 描述统计函数（均值、中位数、标准差、分位数等）
''' - 线性代数运算（矩阵乘法、转置、求解线性方程组等）
''' - 特殊函数（Beta函数、Gamma函数、逆正态分布等）
''' - 数据处理辅助函数（排序、索引、插值、LOESS、PCA等）
''' 
''' 所有函数仅使用VB.NET基础数学函数（System.Math）实现，
''' 不依赖任何第三方数学库。
''' </summary>
Namespace LCMS.Preprocessing

    ''' <summary>
    ''' 数学工具函数模块
    ''' 提供静态方法供其他模块调用
    ''' </summary>
    Public Module MathHelpers

        ' ================================================================
        '  常量定义
        ' ================================================================

        Public Const E As Double = 2.71828182845904523536
        Public Const LOG2 As Double = 0.69314718055994530942
        Public Const PI As Double = 3.14159265358979323846
        Public Const EPSILON As Double = 1.0E-15

        ' ================================================================
        '  描述统计函数
        ' ================================================================

        ''' <summary>计算数组的算术平均值（自动跳过NaN）</summary>
        Public Function Mean(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN
            Dim sum As Double = 0.0
            Dim count As Integer = 0
            For i As Integer = 0 To values.Length - 1
                If Not Double.IsNaN(values(i)) AndAlso Not Double.IsInfinity(values(i)) Then
                    sum += values(i)
                    count += 1
                End If
            Next
            If count = 0 Then Return Double.NaN
            Return sum / count
        End Function

        ''' <summary>计算数组的中位数（自动跳过NaN）</summary>
        Public Function Median(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN
            Dim valid As New List(Of Double)
            For Each v In values
                If Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v) Then
                    valid.Add(v)
                End If
            Next
            If valid.Count = 0 Then Return Double.NaN
            valid.Sort()
            Dim n As Integer = valid.Count
            If n Mod 2 = 1 Then
                Return valid(n \ 2)
            Else
                Return (valid(n \ 2 - 1) + valid(n \ 2)) / 2.0
            End If
        End Function

        ''' <summary>计算数组的样本标准差（分母n-1，自动跳过NaN）</summary>
        Public Function StdDev(values As Double()) As Double
            If values Is Nothing OrElse values.Length < 2 Then Return Double.NaN
            Dim avg As Double = Mean(values)
            If Double.IsNaN(avg) Then Return Double.NaN
            Dim sumSq As Double = 0.0
            Dim count As Integer = 0
            For Each v In values
                If Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v) Then
                    sumSq += (v - avg) * (v - avg)
                    count += 1
                End If
            Next
            If count < 2 Then Return Double.NaN
            Return System.Math.Sqrt(sumSq / (count - 1))
        End Function

        ''' <summary>计算数组的总体标准差（分母n）</summary>
        Public Function StdDevP(values As Double()) As Double
            If values Is Nothing OrElse values.Length < 1 Then Return Double.NaN
            Dim avg As Double = Mean(values)
            If Double.IsNaN(avg) Then Return Double.NaN
            Dim sumSq As Double = 0.0
            Dim count As Integer = 0
            For Each v In values
                If Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v) Then
                    sumSq += (v - avg) * (v - avg)
                    count += 1
                End If
            Next
            If count < 1 Then Return Double.NaN
            Return System.Math.Sqrt(sumSq / count)
        End Function

        ''' <summary>计算方差（样本方差，分母n-1）</summary>
        Public Function Variance(values As Double()) As Double
            Dim sd As Double = StdDev(values)
            If Double.IsNaN(sd) Then Return Double.NaN
            Return sd * sd
        End Function

        ''' <summary>计算变异系数 CV = StdDev / |Mean|</summary>
        Public Function CV(values As Double()) As Double
            Dim avg As Double = Mean(values)
            Dim sd As Double = StdDev(values)
            If Double.IsNaN(avg) OrElse Double.IsNaN(sd) OrElse avg = 0 Then Return Double.NaN
            Return sd / System.Math.Abs(avg)
        End Function

        ''' <summary>计算数组最小值（自动跳过NaN）</summary>
        Public Function Min(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN
            Dim minVal As Double = Double.NaN
            For Each v In values
                If Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v) Then
                    If Double.IsNaN(minVal) OrElse v < minVal Then minVal = v
                End If
            Next
            Return minVal
        End Function

        ''' <summary>计算数组最大值（自动跳过NaN）</summary>
        Public Function Max(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN
            Dim maxVal As Double = Double.NaN
            For Each v In values
                If Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v) Then
                    If Double.IsNaN(maxVal) OrElse v > maxVal Then maxVal = v
                End If
            Next
            Return maxVal
        End Function

        ''' <summary>计算百分位数（自动跳过NaN）</summary>
        Public Function Percentile(values As Double(), p As Double) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN
            Dim valid As New List(Of Double)
            For Each v In values
                If Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v) Then
                    valid.Add(v)
                End If
            Next
            If valid.Count = 0 Then Return Double.NaN
            valid.Sort()
            Return QuantileFromSorted(valid, p / 100.0)
        End Function

        ''' <summary>从已排序数组中计算分位数（R type=7算法）</summary>
        Private Function QuantileFromSorted(sorted As List(Of Double), q As Double) As Double
            Dim n As Integer = sorted.Count
            If n = 1 Then Return sorted(0)
            Dim h As Double = (n - 1) * q
            Dim lo As Integer = CInt(System.Math.Floor(h))
            Dim hi As Integer = CInt(System.Math.Ceiling(h))
            If lo = hi Then Return sorted(lo)
            Dim frac As Double = h - lo
            Return sorted(lo) * (1 - frac) + sorted(hi) * frac
        End Function

        ''' <summary>计算四分位距 IQR = Q3 - Q1</summary>
        Public Function IQR(values As Double()) As Double
            Dim q1 As Double = Percentile(values, 25)
            Dim q3 As Double = Percentile(values, 75)
            If Double.IsNaN(q1) OrElse Double.IsNaN(q3) Then Return Double.NaN
            Return q3 - q1
        End Function

        ''' <summary>计算MAD（Median Absolute Deviation），含1.4826缩放因子</summary>
        Public Function MAD(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN
            Dim med As Double = Median(values)
            If Double.IsNaN(med) Then Return Double.NaN
            Dim deviations As New List(Of Double)
            For Each v In values
                If Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v) Then
                    deviations.Add(System.Math.Abs(v - med))
                End If
            Next
            If deviations.Count = 0 Then Return Double.NaN
            Return Median(deviations.ToArray()) * 1.4826
        End Function

        ''' <summary>计算皮尔逊相关系数</summary>
        Public Function PearsonCorrelation(x As Double(), y As Double()) As Double
            If x Is Nothing OrElse y Is Nothing OrElse x.Length <> y.Length OrElse x.Length < 2 Then
                Return Double.NaN
            End If
            Dim n As Integer = x.Length
            Dim sumX As Double = 0, sumY As Double = 0
            Dim sumXY As Double = 0, sumX2 As Double = 0, sumY2 As Double = 0
            Dim count As Integer = 0

            For i As Integer = 0 To n - 1
                If Not Double.IsNaN(x(i)) AndAlso Not Double.IsNaN(y(i)) Then
                    sumX += x(i) : sumY += y(i)
                    sumXY += x(i) * y(i)
                    sumX2 += x(i) * x(i) : sumY2 += y(i) * y(i)
                    count += 1
                End If
            Next

            If count < 2 Then Return Double.NaN
            Dim num As Double = count * sumXY - sumX * sumY
            Dim den As Double = System.Math.Sqrt((count * sumX2 - sumX * sumX) * (count * sumY2 - sumY * sumY))
            If System.Math.Abs(den) < EPSILON Then Return 0.0
            Return num / den
        End Function

        ''' <summary>计算欧氏距离</summary>
        Public Function EuclideanDistance(x As Double(), y As Double()) As Double
            If x Is Nothing OrElse y Is Nothing OrElse x.Length <> y.Length Then Return Double.NaN
            Dim sumSq As Double = 0.0
            For i As Integer = 0 To x.Length - 1
                Dim diff As Double = x(i) - y(i)
                sumSq += diff * diff
            Next
            Return System.Math.Sqrt(sumSq)
        End Function

        ' ================================================================
        '  线性代数运算
        ' ================================================================

        ''' <summary>矩阵转置</summary>
        Public Function Transpose(matrix As Double(,)) As Double(,)
            Dim rows As Integer = matrix.GetLength(0)
            Dim cols As Integer = matrix.GetLength(1)
            Dim result(cols - 1, rows - 1) As Double
            For i As Integer = 0 To rows - 1
                For j As Integer = 0 To cols - 1
                    result(j, i) = matrix(i, j)
                Next
            Next
            Return result
        End Function

        ''' <summary>矩阵乘法 C = A * B</summary>
        Public Function MatrixMultiply(a As Double(,), b As Double(,)) As Double(,)
            Dim m As Integer = a.GetLength(0)
            Dim n As Integer = b.GetLength(1)
            Dim k As Integer = a.GetLength(1)
            If k <> b.GetLength(0) Then Throw New ArgumentException("矩阵维度不匹配")
            Dim result(m - 1, n - 1) As Double
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To n - 1
                    Dim sum As Double = 0.0
                    For p As Integer = 0 To k - 1
                        sum += a(i, p) * b(p, j)
                    Next
                    result(i, j) = sum
                Next
            Next
            Return result
        End Function

        ''' <summary>向量点积</summary>
        Public Function DotProduct(a As Double(), b As Double()) As Double
            If a.Length <> b.Length Then Throw New ArgumentException("向量长度不匹配")
            Dim sum As Double = 0.0
            For i As Integer = 0 To a.Length - 1
                sum += a(i) * b(i)
            Next
            Return sum
        End Function

        ''' <summary>向量L2范数</summary>
        Public Function VectorNorm(v As Double()) As Double
            Dim sumSq As Double = 0.0
            For Each x In v
                sumSq += x * x
            Next
            Return System.Math.Sqrt(sumSq)
        End Function

        ''' <summary>向量归一化为单位向量</summary>
        Public Function VectorNormalize(v As Double()) As Double()
            Dim norm As Double = VectorNorm(v)
            If norm < EPSILON Then Return v
            Dim result(v.Length - 1) As Double
            For i As Integer = 0 To v.Length - 1
                result(i) = v(i) / norm
            Next
            Return result
        End Function

        ''' <summary>求解线性方程组 Ax=b（列主元高斯消元法）</summary>
        Public Function SolveLinearSystem(A As Double(,), b As Double()) As Double()
            Dim n As Integer = A.GetLength(0)
            If A.GetLength(1) <> n OrElse b.Length <> n Then
                Throw New ArgumentException("矩阵和向量维度不匹配")
            End If

            ' 创建增广矩阵
            Dim aug(n - 1, n) As Double
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    aug(i, j) = A(i, j)
                Next
                aug(i, n) = b(i)
            Next

            ' 前向消元（列主元）
            For col As Integer = 0 To n - 1
                Dim maxRow As Integer = col
                Dim maxVal As Double = System.Math.Abs(aug(col, col))
                For row As Integer = col + 1 To n - 1
                    If System.Math.Abs(aug(row, col)) > maxVal Then
                        maxVal = System.Math.Abs(aug(row, col))
                        maxRow = row
                    End If
                Next

                If maxRow <> col Then
                    For j As Integer = col To n
                        Dim temp As Double = aug(col, j)
                        aug(col, j) = aug(maxRow, j)
                        aug(maxRow, j) = temp
                    Next
                End If

                If System.Math.Abs(aug(col, col)) < EPSILON Then
                    Throw New InvalidOperationException("矩阵奇异，无法求解")
                End If

                For row As Integer = col + 1 To n - 1
                    Dim factor As Double = aug(row, col) / aug(col, col)
                    For j As Integer = col To n
                        aug(row, j) -= factor * aug(col, j)
                    Next
                Next
            Next

            ' 回代
            Dim x(n - 1) As Double
            For i As Integer = n - 1 To 0 Step -1
                Dim sum As Double = aug(i, n)
                For j As Integer = i + 1 To n - 1
                    sum -= aug(i, j) * x(j)
                Next
                x(i) = sum / aug(i, i)
            Next

            Return x
        End Function

        ''' <summary>矩阵求逆（高斯-约旦消元法）</summary>
        Public Function MatrixInverse(A As Double(,)) As Double(,)
            Dim n As Integer = A.GetLength(0)
            If A.GetLength(1) <> n Then Throw New ArgumentException("矩阵必须为方阵")

            Dim aug(n - 1, 2 * n - 1) As Double
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    aug(i, j) = A(i, j)
                Next
                aug(i, n + i) = 1.0
            Next

            For col As Integer = 0 To n - 1
                Dim maxRow As Integer = col
                Dim maxVal As Double = System.Math.Abs(aug(col, col))
                For row As Integer = col + 1 To n - 1
                    If System.Math.Abs(aug(row, col)) > maxVal Then
                        maxVal = System.Math.Abs(aug(row, col))
                        maxRow = row
                    End If
                Next
                If maxRow <> col Then
                    For j As Integer = 0 To 2 * n - 1
                        Dim temp As Double = aug(col, j)
                        aug(col, j) = aug(maxRow, j)
                        aug(maxRow, j) = temp
                    Next
                End If

                If System.Math.Abs(aug(col, col)) < EPSILON Then
                    Throw New InvalidOperationException("矩阵奇异，无法求逆")
                End If

                Dim pivot As Double = aug(col, col)
                For j As Integer = 0 To 2 * n - 1
                    aug(col, j) /= pivot
                Next

                For row As Integer = 0 To n - 1
                    If row <> col Then
                        Dim factor As Double = aug(row, col)
                        For j As Integer = 0 To 2 * n - 1
                            aug(row, j) -= factor * aug(col, j)
                        Next
                    End If
                Next
            Next

            Dim inv(n - 1, n - 1) As Double
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    inv(i, j) = aug(i, n + j)
                Next
            Next
            Return inv
        End Function

        ' ================================================================
        '  特殊函数
        ' ================================================================

        ''' <summary>Gamma函数（Lanczos近似）</summary>
        Public Function GammaFunc(z As Double) As Double
            If z < 0.5 Then
                Return PI / (System.Math.Sin(PI * z) * GammaFunc(1 - z))
            End If
            z -= 1
            Dim g As Double = 7
            Dim c() As Double = {
                0.99999999999980993, 676.5203681218851, -1259.1392167224028,
                771.32342877765313, -176.61502916214059, 12.507343278686905,
                -0.13857109526572012, 9.9843695780195716E-06, 1.5056327351493116E-07
            }
            Dim x As Double = c(0)
            For i As Integer = 1 To CInt(g) + 1
                x += c(i) / (z + i)
            Next
            Dim t As Double = z + g + 0.5
            Return System.Math.Sqrt(2 * PI) * System.Math.Pow(t, z + 0.5) * System.Math.Exp(-t) * x
        End Function

        ''' <summary>Log-Gamma函数（Stirling近似）</summary>
        Public Function LogGamma(z As Double) As Double
            If z <= 0 Then Return Double.NaN
            If z < 7 Then Return LogGamma(z + 1) - System.Math.Log(z)
            Dim x As Double = 1.0 / (z * z)
            x = (z - 0.5) * System.Math.Log(z) - z + 0.5 * System.Math.Log(2 * PI) +
                ((((-5.9523809523809524E-04 * x + 7.9365079365079365E-04) * x -
                2.7777777777777778E-03) * x + 8.3333333333333333E-02) * x) / z
            Return x
        End Function

        ''' <summary>Beta函数 B(a,b) = Gamma(a)*Gamma(b)/Gamma(a+b)</summary>
        Public Function BetaFunc(a As Double, b As Double) As Double
            Return System.Math.Exp(LogGamma(a) + LogGamma(b) - LogGamma(a + b))
        End Function

        ''' <summary>标准正态分布CDF（Abramowitz and Stegun近似）</summary>
        Public Function StandardNormalCDF(z As Double) As Double
            If z < -8 Then Return 0.0
            If z > 8 Then Return 1.0
            Dim t As Double = 1.0 / (1.0 + 0.2316419 * System.Math.Abs(z))
            Dim d As Double = 0.3989422804014327
            Dim prob As Double = d * System.Math.Exp(-z * z / 2.0) *
                (t * (0.319381530 + t * (-0.356563782 + t *
                (1.781477937 + t * (-1.821255978 + t * 1.330274429)))))
            If z > 0 Then prob = 1.0 - prob
            Return prob
        End Function

        ''' <summary>正态分布CDF</summary>
        Public Function NormalCDF(x As Double, mean As Double, sd As Double) As Double
            If sd <= 0 Then Return Double.NaN
            Return StandardNormalCDF((x - mean) / sd)
        End Function

        ''' <summary>标准正态分布逆CDF（Peter Acklam算法，精度~1.15e-9）</summary>
        Public Function NormalInvCDF(p As Double) As Double
            If p <= 0 Then Return Double.NegativeInfinity
            If p >= 1 Then Return Double.PositiveInfinity
            If p = 0.5 Then Return 0.0

            Const a1 As Double = -3.969683028665376E+01
            Const a2 As Double = 2.209460984245205E+02
            Const a3 As Double = -2.759285104469687E+02
            Const a4 As Double = 1.383577518672690E+02
            Const a5 As Double = -3.066479806614716E+01
            Const a6 As Double = 2.506628277459239E+00
            Const b1 As Double = -5.447609879822406E+01
            Const b2 As Double = 1.615858368580409E+02
            Const b3 As Double = -1.556989798598866E+02
            Const b4 As Double = 6.680131188771972E+01
            Const b5 As Double = -1.328068155288572E+01
            Const c1 As Double = -7.784894002430293E-03
            Const c2 As Double = -3.223964580411365E-01
            Const c3 As Double = -2.400758277161838E+00
            Const c4 As Double = -2.549732539343734E+00
            Const c5 As Double = 4.374664141464968E+00
            Const c6 As Double = 2.938163982698783E+00
            Const d1 As Double = 7.784695709041462E-03
            Const d2 As Double = 3.224671290700398E-01
            Const d3 As Double = 2.445134137142996E+00
            Const d4 As Double = 3.754408661907416E+00

            Dim pLow As Double = 0.02425
            Dim pHigh As Double = 1 - pLow
            Dim q As Double, r As Double

            If p < pLow Then
                q = System.Math.Sqrt(-2 * System.Math.Log(p))
                Return (((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) /
                       ((((d1 * q + d2) * q + d3) * q + d4) * q + 1)
            ElseIf p <= pHigh Then
                q = p - 0.5
                r = q * q
                Return (((((a1 * r + a2) * r + a3) * r + a4) * r + a5) * r + a6) * q /
                       (((((b1 * r + b2) * r + b3) * r + b4) * r + b5) * r + 1)
            Else
                q = System.Math.Sqrt(-2 * System.Math.Log(1 - p))
                Return -(((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) /
                        ((((d1 * q + d2) * q + d3) * q + d4) * q + 1)
            End If
        End Function

        ''' <summary>学生t分布逆CDF（Cornish-Fisher近似）</summary>
        Public Function TInvCDF(p As Double, df As Double) As Double
            If df <= 0 Then Return Double.NaN
            Dim x As Double = NormalInvCDF(p)
            Dim g1 As Double = (x * x * x + x) / 4.0
            Dim g2 As Double = (5 * x ^ 5 + 16 * x * x * x + 3 * x) / 96.0
            Return x + g1 / df + g2 / (df * df)
        End Function

        ' ================================================================
        '  数据处理辅助函数
        ' ================================================================

        ''' <summary>获取排序后的索引（升序）</summary>
        Public Function OrderBy(values As Double()) As Integer()
            Dim n As Integer = values.Length
            Dim indices(n - 1) As Integer
            For i As Integer = 0 To n - 1
                indices(i) = i
            Next
            Array.Sort(indices, Function(a, b) values(a).CompareTo(values(b)))
            Return indices
        End Function

        ''' <summary>获取排名（1-based，相同值取平均排名）</summary>
        Public Function Rank(values As Double()) As Double()
            Dim n As Integer = values.Length
            Dim ranks(n - 1) As Double
            Dim sortedIdx = OrderBy(values)

            Dim i As Integer = 0
            While i < n
                Dim j As Integer = i
                While j < n - 1 AndAlso values(sortedIdx(j + 1)) = values(sortedIdx(j))
                    j += 1
                End While
                Dim avgRank As Double = (i + j) / 2.0 + 1
                For k As Integer = i To j
                    ranks(sortedIdx(k)) = avgRank
                Next
                i = j + 1
            End While
            Return ranks
        End Function

        ''' <summary>线性插值</summary>
        Public Function LinearInterpolate(x0 As Double, x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double
            If System.Math.Abs(x2 - x1) < EPSILON Then Return (y1 + y2) / 2.0
            Return y1 + (x0 - x1) * (y2 - y1) / (x2 - x1)
        End Function

        ''' <summary>三次权重函数（Tricube weight，用于LOESS）</summary>
        Public Function TricubeWeight(d As Double) As Double
            If d >= 1.0 Then Return 0.0
            Dim tmp As Double = 1.0 - d * d * d
            Return tmp * tmp * tmp
        End Function

        ''' <summary>高斯核函数 K(x,y) = exp(-gamma * ||x-y||^2)</summary>
        Public Function RBFKernel(x As Double(), y As Double(), gamma As Double) As Double
            Dim sumSq As Double = 0.0
            For i As Integer = 0 To x.Length - 1
                Dim diff As Double = x(i) - y(i)
                sumSq += diff * diff
            Next
            Return System.Math.Exp(-gamma * sumSq)
        End Function

        ''' <summary>限制值在指定范围内</summary>
        Public Function Clamp(value As Double, minVal As Double, maxVal As Double) As Double
            Return System.Math.Max(minVal, System.Math.Min(maxVal, value))
        End Function

        ''' <summary>安全取对数</summary>
        Public Function SafeLog(value As Double, base As Double, offset As Double) As Double
            If value + offset <= 0 Then Return Double.NaN
            If System.Math.Abs(base - 2.0) < EPSILON Then
                Return System.Math.Log(value + offset) / LOG2
            ElseIf System.Math.Abs(base - 10.0) < EPSILON Then
                Return System.Math.Log10(value + offset)
            Else
                Return System.Math.Log(value + offset) / System.Math.Log(base)
            End If
        End Function

        ''' <summary>确定性伪随机数（线性同余法）</summary>
        Public Function PseudoRandom(ByRef seed As Long) As Double
            seed = (seed * 6364136223846793005L + 1442695040888963407L) And Long.MaxValue
            Return (seed >> 33) / 2147483648.0
        End Function

        ''' <summary>加权线性拟合 y = a + b*x</summary>
        Public Function WeightedLinearFit(x As Double(), y As Double(), weights As Double()) As Double()
            Dim n As Integer = x.Length
            Dim sumW As Double = 0, sumWX As Double = 0, sumWY As Double = 0
            Dim sumWXX As Double = 0, sumWXY As Double = 0

            For i As Integer = 0 To n - 1
                Dim w As Double = weights(i)
                sumW += w : sumWX += w * x(i) : sumWY += w * y(i)
                sumWXX += w * x(i) * x(i) : sumWXY += w * x(i) * y(i)
            Next

            If System.Math.Abs(sumW) < EPSILON Then Return New Double() {0, 0}

            Dim denom As Double = sumW * sumWXX - sumWX * sumWX
            If System.Math.Abs(denom) < EPSILON Then
                Return New Double() {sumWY / sumW, 0}
            End If

            Dim b As Double = (sumW * sumWXY - sumWX * sumWY) / denom
            Dim a As Double = (sumWY - b * sumWX) / sumW
            Return New Double() {a, b}
        End Function

        ''' <summary>加权二阶多项式拟合 y = a + b*x + c*x^2</summary>
        Public Function WeightedQuadraticFit(x As Double(), y As Double(), weights As Double()) As Double()
            Dim n As Integer = x.Length
            Dim XtWX(2, 2) As Double
            Dim XtWy(2) As Double

            For i As Integer = 0 To n - 1
                Dim w As Double = weights(i)
                Dim xi As Double = x(i)
                Dim yi As Double = y(i)
                Dim xi2 As Double = xi * xi

                XtWX(0, 0) += w
                XtWX(0, 1) += w * xi
                XtWX(0, 2) += w * xi2
                XtWX(1, 0) += w * xi
                XtWX(1, 1) += w * xi2
                XtWX(1, 2) += w * xi2 * xi
                XtWX(2, 0) += w * xi2
                XtWX(2, 1) += w * xi2 * xi
                XtWX(2, 2) += w * xi2 * xi2

                XtWy(0) += w * yi
                XtWy(1) += w * xi * yi
                XtWy(2) += w * xi2 * yi
            Next

            Try
                Return SolveLinearSystem(XtWX, XtWy)
            Catch ex As Exception
                Dim linFit = WeightedLinearFit(x, y, weights)
                Return New Double() {linFit(0), linFit(1), 0}
            End Try
        End Function

        ' ================================================================
        '  LOESS回归
        ' ================================================================

        ''' <summary>
        ''' LOESS（局部加权散点平滑）回归
        ''' 
        ''' 对每个预测点，使用其邻域内的数据点进行加权多项式回归，
        ''' 权重随距离增加而减小（Tricube权重函数）。
        ''' </summary>
        ''' <param name="x">自变量数组</param>
        ''' <param name="y">因变量数组</param>
        ''' <param name="xPred">需要预测的x值数组</param>
        ''' <param name="span">平滑参数(0,1]，控制邻域大小</param>
        ''' <param name="degree">多项式阶数（1或2）</param>
        ''' <returns>预测值数组</returns>
        Public Function LOESS(x As Double(), y As Double(), xPred As Double(),
                               span As Double, degree As Integer) As Double()
            If x Is Nothing OrElse y Is Nothing OrElse x.Length <> y.Length Then
                Return Nothing
            End If

            Dim n As Integer = x.Length
            Dim nPred As Integer = xPred.Length
            Dim result(nPred - 1) As Double

            ' 邻域内包含的数据点数
            Dim q As Integer = CInt(System.Math.Floor(span * n))
            q = System.Math.Max(System.Math.Max(q, degree + 1), 2)
            q = System.Math.Min(q, n)

            For p As Integer = 0 To nPred - 1
                Dim x0 As Double = xPred(p)

                ' 计算所有点到x0的距离
                Dim distances(n - 1) As Double
                For i As Integer = 0 To n - 1
                    distances(i) = System.Math.Abs(x(i) - x0)
                Next

                ' 找到q个最近的点
                Dim sortedIdx = OrderBy(distances)
                Dim maxDist As Double = distances(sortedIdx(q - 1))
                If maxDist < EPSILON Then maxDist = EPSILON

                ' 计算权重
                Dim wX As New List(Of Double)
                Dim wY As New List(Of Double)
                Dim wW As New List(Of Double)

                For k As Integer = 0 To q - 1
                    Dim idx As Integer = sortedIdx(k)
                    Dim d As Double = distances(idx) / maxDist
                    Dim weight As Double = TricubeWeight(d)
                    If weight > 0 Then
                        wX.Add(x(idx))
                        wY.Add(y(idx))
                        wW.Add(weight)
                    End If
                Next

                If wX.Count = 0 Then
                    result(p) = Double.NaN
                    Continue For
                End If

                ' 加权多项式拟合
                Dim xArr = wX.ToArray()
                Dim yArr = wY.ToArray()
                Dim wArr = wW.ToArray()

                If degree <= 1 OrElse wX.Count < 3 Then
                    Dim coeff = WeightedLinearFit(xArr, yArr, wArr)
                    result(p) = coeff(0) + coeff(1) * x0
                Else
                    Dim coeff = WeightedQuadraticFit(xArr, yArr, wArr)
                    result(p) = coeff(0) + coeff(1) * x0 + coeff(2) * x0 * x0
                End If
            Next

            Return result
        End Function

        ''' <summary>LOESS回归（简化版，直接对原始x点预测）</summary>
        Public Function LOESS(x As Double(), y As Double(), span As Double, degree As Integer) As Double()
            Return LOESS(x, y, x, span, degree)
        End Function

        ' ================================================================
        '  PCA（主成分分析）
        ' ================================================================

        ''' <summary>
        ''' PCA分析（使用NIPALS算法，支持缺失值）
        ''' 返回样本得分矩阵（样本×主成分）
        ''' </summary>
        ''' <param name="matrix">数据矩阵（特征×样本），已中心化</param>
        ''' <param name="nComponents">提取的主成分数</param>
        ''' <returns>得分矩阵（样本×主成分）</returns>
        Public Function PCA(matrix As Double(,), nComponents As Integer) As Double(,)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)
            nComponents = System.Math.Min(nComponents, System.Math.Min(nFeatures, nSamples))

            ' 中心化
            Dim centered(nFeatures - 1, nSamples - 1) As Double
            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next
                Dim avg As Double = Mean(row)
                For j As Integer = 0 To nSamples - 1
                    centered(i, j) = matrix(i, j) - If(Double.IsNaN(avg), 0, avg)
                Next
            Next

            ' NIPALS算法
            Dim scores(nSamples - 1, nComponents - 1) As Double
            Dim residual = CType(centered.Clone(), Double(,))

            For comp As Integer = 0 To nComponents - 1
                ' 选择具有最大方差的列作为初始t
                Dim t(nSamples - 1) As Double
                Dim maxVar As Double = Double.NegativeInfinity
                Dim maxCol As Integer = 0

                For j As Integer = 0 To nFeatures - 1
                    Dim col(nSamples - 1) As Double
                    For k As Integer = 0 To nSamples - 1
                        col(k) = residual(j, k)
                    Next
                    Dim v As Double = Variance(col)
                    If Not Double.IsNaN(v) AndAlso v > maxVar Then
                        maxVar = v
                        maxCol = j
                    End If
                Next

                For k As Integer = 0 To nSamples - 1
                    t(k) = residual(maxCol, k)
                Next

                ' 迭代
                Dim converged As Boolean = False
                For iter As Integer = 1 To 100
                    ' p = X'*t / (t'*t)
                    Dim p(nFeatures - 1) As Double
                    Dim tSqSum As Double = 0
                    For k As Integer = 0 To nSamples - 1
                        tSqSum += t(k) * t(k)
                    Next
                    If tSqSum < EPSILON Then Exit For

                    For i As Integer = 0 To nFeatures - 1
                        Dim dot As Double = 0
                        For k As Integer = 0 To nSamples - 1
                            dot += residual(i, k) * t(k)
                        Next
                        p(i) = dot / tSqSum
                    Next

                    ' 归一化p
                    Dim pNorm As Double = VectorNorm(p)
                    If pNorm < EPSILON Then Exit For
                    For i As Integer = 0 To nFeatures - 1
                        p(i) /= pNorm
                    Next

                    ' t_new = X*p / (p'*p)
                    Dim tNew(nSamples - 1) As Double
                    Dim pSqSum As Double = 0
                    For i As Integer = 0 To nFeatures - 1
                        pSqSum += p(i) * p(i)
                    Next

                    For k As Integer = 0 To nSamples - 1
                        Dim dot As Double = 0
                        For i As Integer = 0 To nFeatures - 1
                            dot += residual(i, k) * p(i)
                        Next
                        tNew(k) = dot / pSqSum
                    Next

                    ' 检查收敛
                    Dim diff As Double = 0
                    For k As Integer = 0 To nSamples - 1
                        diff += (tNew(k) - t(k)) * (tNew(k) - t(k))
                    Next
                    t = tNew

                    If diff < 1.0E-10 Then
                        converged = True
                        Exit For
                    End If
                Next

                ' 保存得分
                For k As Integer = 0 To nSamples - 1
                    scores(k, comp) = t(k)
                Next

                ' 更新残差 E = E - t*p'
                For i As Integer = 0 To nFeatures - 1
                    For k As Integer = 0 To nSamples - 1
                        Dim p_val As Double = 0
                        ' 重新计算p
                        Dim dot As Double = 0
                        Dim tSqSum As Double = 0
                        For kk As Integer = 0 To nSamples - 1
                            tSqSum += t(kk) * t(kk)
                        Next
                        If tSqSum > EPSILON Then
                            For ii As Integer = 0 To nFeatures - 1
                                Dim d As Double = 0
                                For kk As Integer = 0 To nSamples - 1
                                    d += residual(ii, kk) * t(kk)
                                Next
                            Next
                        End If
                    Next
                Next

                ' 简化的残差更新
                Dim loadings(nFeatures - 1) As Double
                Dim tSqSum2 As Double = 0
                For k As Integer = 0 To nSamples - 1
                    tSqSum2 += t(k) * t(k)
                Next
                If tSqSum2 > EPSILON Then
                    For i As Integer = 0 To nFeatures - 1
                        Dim dot As Double = 0
                        For k As Integer = 0 To nSamples - 1
                            dot += residual(i, k) * t(k)
                        Next
                        loadings(i) = dot / tSqSum2
                    Next
                End If

                For i As Integer = 0 To nFeatures - 1
                    For k As Integer = 0 To nSamples - 1
                        residual(i, k) -= t(k) * loadings(i)
                    Next
                End If
            Next

            Return scores
        End Function

        ''' <summary>
        ''' NIPALS算法进行PCA，支持缺失值
        ''' 返回得分矩阵和载荷矩阵，用于缺失值插补
        ''' </summary>
        Public Sub NIPALSWithMissing(matrix As Double(,), nComponents As Integer,
                                      maxIter As Integer, tolerance As Double,
                                      ByRef scores As Double(,), ByRef loadings As Double(,))

            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)
            nComponents = System.Math.Min(nComponents, System.Math.Min(nFeatures, nSamples))

            scores = New Double(nSamples - 1, nComponents - 1) {}
            loadings = New Double(nFeatures - 1, nComponents - 1) {}

            ' 创建缺失值标记
            Dim isMissing(nFeatures - 1, nSamples - 1) As Boolean
            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    isMissing(i, j) = Double.IsNaN(matrix(i, j))
                Next
            Next

            ' 用列均值初始化缺失值
            Dim X(nFeatures - 1, nSamples - 1) As Double
            For i As Integer = 0 To nFeatures - 1
                Dim colAvg As Double = 0
                Dim colCount As Integer = 0
                For j As Integer = 0 To nSamples - 1
                    If Not isMissing(i, j) Then
                        colAvg += matrix(i, j)
                        colCount += 1
                    End If
                Next
                If colCount > 0 Then colAvg /= colCount Else colAvg = 0

                For j As Integer = 0 To nSamples - 1
                    If isMissing(i, j) Then
                        X(i, j) = colAvg
                    Else
                        X(i, j) = matrix(i, j)
                    End If
                Next
            Next

            ' 中心化
            For i As Integer = 0 To nFeatures - 1
                Dim rowAvg As Double = 0
                For j As Integer = 0 To nSamples - 1
                    rowAvg += X(i, j)
                Next
                rowAvg /= nSamples
                For j As Integer = 0 To nSamples - 1
                    X(i, j) -= rowAvg
                Next
            Next

            ' NIPALS迭代
            For comp As Integer = 0 To nComponents - 1
                ' 选择最大方差列
                Dim t(nSamples - 1) As Double
                Dim maxVar As Double = Double.NegativeInfinity
                Dim maxCol As Integer = 0

                For i As Integer = 0 To nFeatures - 1
                    Dim row(nSamples - 1) As Double
                    For j As Integer = 0 To nSamples - 1
                        row(j) = X(i, j)
                    Next
                    Dim v As Double = Variance(row)
                    If Not Double.IsNaN(v) AndAlso v > maxVar Then
                        maxVar = v
                        maxCol = i
                    End If
                Next

                For j As Integer = 0 To nSamples - 1
                    t(j) = X(maxCol, j)
                Next

                ' 迭代
                For iter As Integer = 1 To maxIter
                    ' p = X'*t / (t'*t)（仅非缺失值参与）
                    Dim p(nFeatures - 1) As Double
                    Dim tSqSum As Double = 0
                    For j As Integer = 0 To nSamples - 1
                        tSqSum += t(j) * t(j)
                    Next
                    If tSqSum < EPSILON Then Exit For

                    For i As Integer = 0 To nFeatures - 1
                        Dim dot As Double = 0
                        For j As Integer = 0 To nSamples - 1
                            If Not isMissing(i, j) Then
                                dot += X(i, j) * t(j)
                            End If
                        Next
                        p(i) = dot / tSqSum
                    Next

                    ' 归一化p
                    Dim pNorm As Double = VectorNorm(p)
                    If pNorm < EPSILON Then Exit For
                    For i As Integer = 0 To nFeatures - 1
                        p(i) /= pNorm
                    Next

                    ' t_new = X*p / (p'*p)（仅非缺失值参与）
                    Dim tNew(nSamples - 1) As Double
                    Dim pSqSum As Double = 0
                    For i As Integer = 0 To nFeatures - 1
                        pSqSum += p(i) * p(i)
                    Next

                    For j As Integer = 0 To nSamples - 1
                        Dim dot As Double = 0
                        For i As Integer = 0 To nFeatures - 1
                            If Not isMissing(i, j) Then
                                dot += X(i, j) * p(i)
                            End If
                        Next
                        tNew(j) = dot / pSqSum
                    Next

                    ' 检查收敛
                    Dim diff As Double = 0
                    For j As Integer = 0 To nSamples - 1
                        diff += (tNew(j) - t(j)) * (tNew(j) - t(j))
                    Next
                    t = tNew

                    If diff < tolerance Then Exit For
                Next

                ' 保存得分和载荷
                For j As Integer = 0 To nSamples - 1
                    scores(j, comp) = t(j)
                Next

                ' 重新计算载荷
                Dim tSqSum2 As Double = 0
                For j As Integer = 0 To nSamples - 1
                    tSqSum2 += t(j) * t(j)
                Next
                If tSqSum2 > EPSILON Then
                    For i As Integer = 0 To nFeatures - 1
                        Dim dot As Double = 0
                        For j As Integer = 0 To nSamples - 1
                            dot += X(i, j) * t(j)
                        Next
                        loadings(i, comp) = dot / tSqSum2
                    Next
                End If

                ' 更新残差
                For i As Integer = 0 To nFeatures - 1
                    For j As Integer = 0 To nSamples - 1
                        X(i, j) -= t(j) * loadings(i, comp)
                    Next
                Next
            Next
        End Sub

    End Module

End Namespace
