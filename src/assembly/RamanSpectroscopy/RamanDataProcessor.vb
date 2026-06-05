' ============================================================================
' 拉曼光谱数据处理模块 (Raman Spectroscopy Data Processing Module)
' ============================================================================
' 基于VB.NET基础数学函数实现，涵盖：
'   一、数据预处理：去噪/平滑、基线校正、归一化/标准化、裁剪/重采样、宇宙射线去除
'   二、特征提取与峰处理：峰检测、峰拟合/分峰、全谱降维(PCA)
' ============================================================================

Imports System.IO
Imports System.Math
Imports Microsoft.VisualBasic.Data.Bootstrapping

Namespace RamanProcessing

    ' ========================================================================
    ' 核心数据结构：拉曼光谱数据容器
    ' ========================================================================
    ''' <summary>
    ''' 表示一条拉曼光谱数据，包含波数轴和强度轴
    ''' </summary>
    Public Class RamanSpectrum
        ''' <summary>波数数组 (cm⁻¹)</summary>
        Public Wavenumbers As Double()
        ''' <summary>强度数组 (任意单位)</summary>
        Public Intensities As Double()
        ''' <summary>光谱名称/标识</summary>
        Public Name As String = ""

        ''' <summary>
        ''' 创建一条空的拉曼光谱
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' 使用给定的波数和强度数组创建拉曼光谱
        ''' </summary>
        ''' <param name="wn">波数数组</param>
        ''' <param name="inten">强度数组</param>
        Public Sub New(wn As Double(), inten As Double())
            If wn Is Nothing OrElse inten Is Nothing Then
                Throw New ArgumentNullException("波数和强度数组不能为空")
            End If
            If wn.Length <> inten.Length Then
                Throw New ArgumentException("波数和强度数组长度必须一致")
            End If
            Wavenumbers = CType(wn.Clone(), Double())
            Intensities = CType(inten.Clone(), Double())
        End Sub

        ''' <summary>数据点数量</summary>
        Public ReadOnly Property PointCount As Integer
            Get
                Return If(Wavenumbers?.Length, 0)
            End Get
        End Property

        ''' <summary>返回深拷贝</summary>
        Public Function Clone() As RamanSpectrum
            Dim sp As New RamanSpectrum()
            sp.Wavenumbers = CType(Wavenumbers.Clone(), Double())
            sp.Intensities = CType(Intensities.Clone(), Double())
            sp.Name = Me.Name
            Return sp
        End Function

        ''' <summary>
        ''' 从两列文本文件加载光谱数据（波数 强度）
        ''' </summary>
        Public Shared Function LoadFromFile(filePath As String, Optional delimiter As Char = ControlChars.Tab) As RamanSpectrum
            Dim lines As String() = System.IO.File.ReadAllLines(filePath)
            Dim wnList As New List(Of Double)
            Dim intList As New List(Of Double)

            For Each line In lines
                line = line.Trim()
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("#") OrElse line.StartsWith("'") Then Continue For

                Dim parts As String() = line.Split(delimiter)
                If parts.Length < 2 Then
                    parts = line.Split(" "c)
                    parts = parts.Where(Function(p) Not String.IsNullOrWhiteSpace(p)).ToArray()
                End If

                If parts.Length >= 2 Then
                    Dim w, v As Double
                    If Double.TryParse(parts(0).Trim(), w) AndAlso Double.TryParse(parts(1).Trim(), v) Then
                        wnList.Add(w)
                        intList.Add(v)
                    End If
                End If
            Next

            If wnList.Count = 0 Then
                Throw New InvalidDataException("文件中未找到有效的光谱数据")
            End If

            Return New RamanSpectrum(wnList.ToArray(), intList.ToArray())
        End Function

        ''' <summary>
        ''' 将光谱数据保存到文本文件
        ''' </summary>
        Public Sub SaveToFile(filePath As String, Optional delimiter As String = vbTab)
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("# Wavenumber(cm-1)" & delimiter & "Intensity")
            For i As Integer = 0 To PointCount - 1
                sb.AppendLine(Wavenumbers(i).ToString("G") & delimiter & Intensities(i).ToString("G"))
            Next
            System.IO.File.WriteAllText(filePath, sb.ToString())
        End Sub
    End Class

    ' ========================================================================
    ' 峰信息数据结构
    ' ========================================================================
    ''' <summary>
    ''' 表示一个检测到的拉曼峰的信息
    ''' </summary>
    Public Class PeakInfo
        ''' <summary>峰位波数 (cm⁻¹)</summary>
        Public Position As Double
        ''' <summary>峰高（强度）</summary>
        Public Height As Double
        ''' <summary>半高宽 (FWHM, cm⁻¹)</summary>
        Public FWHM As Double
        ''' <summary>峰面积</summary>
        Public Area As Double
        ''' <summary>峰的左边界波数</summary>
        Public LeftBound As Double
        ''' <summary>峰的右边界波数</summary>
        Public RightBound As Double
        ''' <summary>拟合优度 R²</summary>
        Public FitR2 As Double

        Public Overrides Function ToString() As String
            Return String.Format("峰位={0:F2} cm-1, 峰高={1:F4}, FWHM={2:F2} cm-1, 面积={3:F4}", Position, Height, FWHM, Area)
        End Function
    End Class

    ' ========================================================================
    ' 峰拟合参数结构
    ' ========================================================================
    ''' <summary>
    ''' 单峰拟合参数（用于多峰拟合）
    ''' </summary>
    Public Class PeakParams
        ''' <summary>峰位 (cm⁻¹)</summary>
        Public Center As Double
        ''' <summary>峰高</summary>
        Public Amplitude As Double
        ''' <summary>半宽参数 (sigma)</summary>
        Public Sigma As Double
        ''' <summary>峰类型: "Gaussian", "Lorentzian", "Voigt"</summary>
        Public PeakType As String = "Gaussian"
        ''' <summary>Voigt峰的Lorentz比例 (0=纯高斯, 1=纯洛伦兹)</summary>
        Public LorentzRatio As Double = 0.5

        Public Sub New(center As Double, amplitude As Double, sigma As Double, Optional peakType As String = "Gaussian")
            Me.Center = center
            Me.Amplitude = amplitude
            Me.Sigma = sigma
            Me.PeakType = peakType
        End Sub
    End Class

    ' ========================================================================
    ' 第一部分：数据预处理类
    ' ========================================================================
    ''' <summary>
    ''' 拉曼光谱数据预处理器，包含去噪、基线校正、归一化、裁剪、宇宙射线去除等功能
    ''' </summary>
    Public Class SpectralPreprocessor

        ' --------------------------------------------------------------------
        ' 1. 去噪 / 平滑
        ' --------------------------------------------------------------------

#Region "去噪/平滑"

        ''' <summary>
        ''' Savitzky-Golay 滤波平滑
        ''' 原理：在滑动窗口内用多项式进行最小二乘拟合，取中心点拟合值作为输出
        ''' 优点：既去噪又较好保留峰形与峰宽，是拉曼光谱最常用的平滑方法
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="windowSize">窗口宽度（必须为奇数，推荐5~21）</param>
        ''' <param name="polyOrder">多项式阶数（推荐2~4，须小于窗口宽度）</param>
        ''' <returns>平滑后的光谱</returns>
        Public Shared Function SavitzkyGolaySmooth(spectrum As RamanSpectrum, windowSize As Integer, polyOrder As Integer) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If
            If windowSize Mod 2 = 0 Then
                Throw New ArgumentException("窗口宽度必须为奇数")
            End If
            If polyOrder >= windowSize Then
                Throw New ArgumentException("多项式阶数必须小于窗口宽度")
            End If

            Dim n As Integer = spectrum.PointCount
            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(n - 1) {}
            result.Name = spectrum.Name & "_SG"

            Dim halfW As Integer = windowSize \ 2
            Dim coeffs As Double() = ComputeSGCoefficients(windowSize, polyOrder, 0)

            For i As Integer = 0 To n - 1
                Dim sum As Double = 0.0
                For j As Integer = -halfW To halfW
                    Dim idx As Integer = i + j
                    ' 边界处理：镜像扩展
                    If idx < 0 Then idx = -idx
                    If idx >= n Then idx = 2 * n - 2 - idx
                    sum += coeffs(j + halfW) * spectrum.Intensities(idx)
                Next
                result.Intensities(i) = sum
            Next

            Return result
        End Function

        ''' <summary>
        ''' 计算Savitzky-Golay滤波器系数
        ''' 通过求解正规方程 (X^T X)^-1 X^T 的中心行获得
        ''' </summary>
        Private Shared Function ComputeSGCoefficients(windowSize As Integer, polyOrder As Integer, derivOrder As Integer) As Double()
            Dim halfW As Integer = windowSize \ 2
            Dim m As Integer = polyOrder + 1

            ' 构建范德蒙矩阵 X
            Dim X As Double(,) = New Double(windowSize - 1, m - 1) {}
            For i As Integer = -halfW To halfW
                For j As Integer = 0 To polyOrder
                    X(i + halfW, j) = Pow(i, j)
                Next
            Next

            ' 计算 X^T * X
            Dim XtX As Double(,) = New Double(m - 1, m - 1) {}
            For i As Integer = 0 To m - 1
                For j As Integer = 0 To m - 1
                    Dim s As Double = 0.0
                    For k As Integer = 0 To windowSize - 1
                        s += X(k, i) * X(k, j)
                    Next
                    XtX(i, j) = s
                Next
            Next

            ' 求逆 (X^T X)^-1
            Dim invXtX As Double(,) = MatrixInverse(XtX, m)

            ' 计算 (X^T X)^-1 * X^T 的中心行
            Dim coeffs As Double() = New Double(windowSize - 1) {}
            For j As Integer = 0 To windowSize - 1
                Dim s As Double = 0.0
                For k As Integer = 0 To m - 1
                    s += invXtX(derivOrder, k) * X(j, k)
                Next
                coeffs(j) = s
            Next

            ' 乘以导数阶数的阶乘
            Dim factorial As Double = 1.0
            For i As Integer = 1 To derivOrder
                factorial *= i
            Next
            For i As Integer = 0 To windowSize - 1
                coeffs(i) *= factorial
            Next

            Return coeffs
        End Function

        ''' <summary>
        ''' 移动平均平滑
        ''' 原理：在滑动窗口内取算术平均值
        ''' 特点：简单但会增宽峰，一般只在粗略查看时使用
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="windowSize">窗口宽度（必须为奇数）</param>
        ''' <returns>平滑后的光谱</returns>
        Public Shared Function MovingAverageSmooth(spectrum As RamanSpectrum, windowSize As Integer) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If
            If windowSize Mod 2 = 0 Then
                Throw New ArgumentException("窗口宽度必须为奇数")
            End If

            Dim n As Integer = spectrum.PointCount
            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(n - 1) {}
            result.Name = spectrum.Name & "_MA"

            Dim halfW As Integer = windowSize \ 2
            Dim invWin As Double = 1.0 / windowSize

            For i As Integer = 0 To n - 1
                Dim sum As Double = 0.0
                For j As Integer = -halfW To halfW
                    Dim idx As Integer = i + j
                    If idx < 0 Then idx = -idx
                    If idx >= n Then idx = 2 * n - 2 - idx
                    sum += spectrum.Intensities(idx)
                Next
                result.Intensities(i) = sum * invWin
            Next

            Return result
        End Function

        ''' <summary>
        ''' 高斯核平滑
        ''' 原理：使用高斯函数作为权重核进行加权平均平滑
        ''' 特点：距离中心越远权重越小，比移动平均更好地保留峰形
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="sigma">高斯核标准差（控制平滑程度，推荐1~5）</param>
        ''' <param name="kernelRadius">核半径（默认3*sigma）</param>
        ''' <returns>平滑后的光谱</returns>
        Public Shared Function GaussianSmooth(spectrum As RamanSpectrum, sigma As Double, Optional kernelRadius As Integer = -1) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If
            If sigma <= 0 Then
                Throw New ArgumentException("sigma必须大于0")
            End If

            If kernelRadius < 0 Then kernelRadius = CInt(Math.Ceiling(3 * sigma))
            Dim kernelSize As Integer = 2 * kernelRadius + 1

            ' 构建高斯核
            Dim kernel As Double() = New Double(kernelSize - 1) {}
            Dim sum As Double = 0.0
            Dim twoSigmaSq As Double = 2.0 * sigma * sigma
            For i As Integer = -kernelRadius To kernelRadius
                Dim val As Double = Exp(-(i * i) / twoSigmaSq)
                kernel(i + kernelRadius) = val
                sum += val
            Next
            ' 归一化
            For i As Integer = 0 To kernelSize - 1
                kernel(i) /= sum
            Next

            Dim n As Integer = spectrum.PointCount
            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(n - 1) {}
            result.Name = spectrum.Name & "_Gauss"

            For i As Integer = 0 To n - 1
                Dim val As Double = 0.0
                For j As Integer = -kernelRadius To kernelRadius
                    Dim idx As Integer = i + j
                    If idx < 0 Then idx = -idx
                    If idx >= n Then idx = 2 * n - 2 - idx
                    val += kernel(j + kernelRadius) * spectrum.Intensities(idx)
                Next
                result.Intensities(i) = val
            Next

            Return result
        End Function

        ''' <summary>
        ''' Savitzky-Golay 一阶导数
        ''' 用于峰检测中的导数法
        ''' </summary>
        Public Shared Function SavitzkyGolayFirstDerivative(spectrum As RamanSpectrum, windowSize As Integer, polyOrder As Integer) As RamanSpectrum
            Return SGDerivative(spectrum, windowSize, polyOrder, 1)
        End Function

        ''' <summary>
        ''' Savitzky-Golay 二阶导数
        ''' 用于峰检测中的导数法
        ''' </summary>
        Public Shared Function SavitzkyGolaySecondDerivative(spectrum As RamanSpectrum, windowSize As Integer, polyOrder As Integer) As RamanSpectrum
            Return SGDerivative(spectrum, windowSize, polyOrder, 2)
        End Function

        Private Shared Function SGDerivative(spectrum As RamanSpectrum, windowSize As Integer, polyOrder As Integer, derivOrder As Integer) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount < 2 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If
            If windowSize Mod 2 = 0 Then
                Throw New ArgumentException("窗口宽度必须为奇数")
            End If

            Dim n As Integer = spectrum.PointCount
            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(n - 1) {}
            result.Name = spectrum.Name & "_SGd" & derivOrder.ToString()

            Dim halfW As Integer = windowSize \ 2
            Dim coeffs As Double() = ComputeSGCoefficients(windowSize, polyOrder, derivOrder)

            ' 计算波数间距（用于导数缩放）
            Dim avgSpacing As Double = (spectrum.Wavenumbers(n - 1) - spectrum.Wavenumbers(0)) / (n - 1)

            For i As Integer = 0 To n - 1
                Dim sum As Double = 0.0
                For j As Integer = -halfW To halfW
                    Dim idx As Integer = i + j
                    If idx < 0 Then idx = -idx
                    If idx >= n Then idx = 2 * n - 2 - idx
                    sum += coeffs(j + halfW) * spectrum.Intensities(idx)
                Next
                result.Intensities(i) = sum / Pow(avgSpacing, derivOrder)
            Next

            Return result
        End Function

#End Region

        ' --------------------------------------------------------------------
        ' 2. 基线校正 / 荧光背景扣除
        ' --------------------------------------------------------------------

#Region "基线校正"

        ''' <summary>
        ''' 多项式拟合基线校正
        ''' 原理：用指定阶数的多项式拟合基线趋势，然后从原始光谱中扣除
        ''' 特点：简单直观，但阶数选择敏感，易过拟合或欠拟合
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="polyOrder">多项式阶数（推荐3~6）</param>
        ''' <param name="iterations">迭代次数（每次迭代排除高于拟合基线的点，增强鲁棒性）</param>
        ''' <returns>基线校正后的光谱</returns>
        Public Shared Function PolynomialBaselineCorrect(spectrum As RamanSpectrum, polyOrder As Integer, Optional iterations As Integer = 3) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            Dim n As Integer = spectrum.PointCount
            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = CType(spectrum.Intensities.Clone(), Double())
            result.Name = spectrum.Name & "_PolyBL"

            ' 归一化波数到[0,1]避免数值问题
            Dim wnMin As Double = spectrum.Wavenumbers.Min()
            Dim wnMax As Double = spectrum.Wavenumbers.Max()
            Dim wnRange As Double = wnMax - wnMin
            If wnRange = 0 Then wnRange = 1.0

            Dim normWn As Double() = New Double(n - 1) {}
            For i As Integer = 0 To n - 1
                normWn(i) = (spectrum.Wavenumbers(i) - wnMin) / wnRange
            Next

            ' 迭代多项式拟合：每次排除高于拟合基线的点
            Dim weights As Double() = New Double(n - 1) {}
            For i As Integer = 0 To n - 1
                weights(i) = 1.0
            Next

            Dim baseline As Double() = New Double(n - 1) {}

            For iter As Integer = 0 To iterations - 1
                ' 加权多项式拟合
                Dim coeffs As WeightedFit = WeightedLinearRegression.Regress(normWn, spectrum.Intensities, weights, polyOrder)

                ' 计算基线
                For i As Integer = 0 To n - 1
                    baseline(i) = coeffs(x:=normWn(i))
                Next

                ' 更新权重：低于基线的点权重增大，高于基线的点权重减小
                If iter < iterations - 1 Then
                    For i As Integer = 0 To n - 1
                        If spectrum.Intensities(i) > baseline(i) Then
                            weights(i) *= 0.01  ' 峰区域权重降低
                        Else
                            weights(i) = 1.0    ' 基线区域权重恢复
                        End If
                    Next
                End If
            Next

            ' 扣除基线
            For i As Integer = 0 To n - 1
                result.Intensities(i) = spectrum.Intensities(i) - baseline(i)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 不对称最小二乘（AsLS）基线校正
        ''' 原理：通过惩罚最小二乘拟合平滑基线，对低于基线的点赋予更大权重
        ''' 特点：自适应拟合基线，对强荧光、复杂背景效果好
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="lambda">平滑参数（推荐1e4~1e7，越大基线越平滑）</param>
        ''' <param name="p">不对称参数（推荐0.001~0.01，越小基线越贴近底部）</param>
        ''' <param name="maxIter">最大迭代次数</param>
        ''' <returns>基线校正后的光谱</returns>
        Public Shared Function ALSBaselineCorrect(spectrum As RamanSpectrum, Optional lambda As Double = 100000.0, Optional p As Double = 0.005, Optional maxIter As Integer = 30) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            Dim n As Integer = spectrum.PointCount
            Dim y As Double() = spectrum.Intensities
            Dim w As Double() = New Double(n - 1) {}
            Dim baseline As Double() = New Double(n - 1) {}

            ' 初始权重
            For i As Integer = 0 To n - 1
                w(i) = 1.0
            Next

            ' 构建差分矩阵 D (二阶差分)
            ' 求解: (W + lambda * D^T D) z = W y

            For iter As Integer = 0 To maxIter - 1
                ' 求解三对角系统
                baseline = SolveALS(y, w, lambda, n)

                ' 更新权重
                Dim maxDiff As Double = 0.0
                For i As Integer = 0 To n - 1
                    Dim diff As Double = Abs(y(i) - baseline(i))
                    If diff > maxDiff Then maxDiff = diff
                Next
                If maxDiff = 0 Then maxDiff = 1.0

                Dim converged As Boolean = True
                For i As Integer = 0 To n - 1
                    Dim oldW As Double = w(i)
                    If y(i) > baseline(i) Then
                        w(i) = p
                    Else
                        w(i) = 1.0 - p
                    End If
                    If Abs(w(i) - oldW) > 0.000001 Then converged = False
                Next

                If converged Then Exit For
            Next

            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(n - 1) {}
            result.Name = spectrum.Name & "_ALS"

            For i As Integer = 0 To n - 1
                result.Intensities(i) = y(i) - baseline(i)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 求解ALS三对角系统 (W + lambda * D^T D) z = W y
        ''' 使用追赶法（Thomas算法）求解
        ''' </summary>
        Private Shared Function SolveALS(y As Double(), w As Double(), lambda As Double, n As Integer) As Double()
            ' 构建三对角矩阵的对角线
            ' D^T D 的结构：主对角线 [1, 4, 6, ..., 6, 4, 1]（二阶差分）
            ' 简化处理：构建完整的五对角系统并用迭代法求解

            ' 使用Gauss-Seidel迭代法求解
            Dim z As Double() = New Double(n - 1) {}
            Dim rhs As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                rhs(i) = w(i) * y(i)
                z(i) = y(i)  ' 初始猜测
            Next

            For gsIter As Integer = 0 To 99
                Dim maxChange As Double = 0.0
                For i As Integer = 0 To n - 1
                    Dim oldZ As Double = z(i)

                    ' 计算 (D^T D * z)[i] 的贡献
                    Dim dtdz As Double = 0.0
                    If i >= 2 Then dtdz += z(i - 2)
                    If i >= 1 Then dtdz -= 4.0 * z(i - 1)
                    dtdz += 6.0 * z(i)
                    If i + 1 < n Then dtdz -= 4.0 * z(i + 1)
                    If i + 2 < n Then dtdz += z(i + 2)

                    ' 边界修正
                    If i = 0 OrElse i = n - 1 Then dtdz -= 5.0 * z(i) : dtdz += 1.0 * z(i) ' 修正为1
                    If i = 1 OrElse i = n - 2 Then dtdz += 2.0 * z(i) ' 修正为4

                    Dim diag As Double = w(i) + lambda * 6.0
                    If i = 0 OrElse i = n - 1 Then diag = w(i) + lambda * 1.0
                    If i = 1 OrElse i = n - 2 Then diag = w(i) + lambda * 4.0

                    z(i) = (rhs(i) - lambda * dtdz + lambda * diag * oldZ) / diag

                    Dim change As Double = Abs(z(i) - oldZ)
                    If change > maxChange Then maxChange = change
                Next

                If maxChange < 0.00000001 Then Exit For
            Next

            Return z
        End Function

        ''' <summary>
        ''' 形态学顶帽变换基线校正
        ''' 原理：使用形态学开运算（先腐蚀后膨胀）估计背景，然后扣除
        ''' 特点：对宽缓背景有效，计算速度快
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="structElementSize">结构元素大小（推荐50~200，越大基线越平滑）</param>
        ''' <returns>基线校正后的光谱</returns>
        Public Shared Function MorphologicalTopHatBaseline(spectrum As RamanSpectrum, structElementSize As Integer) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If
            If structElementSize Mod 2 = 0 Then structElementSize += 1

            Dim n As Integer = spectrum.PointCount
            Dim y As Double() = spectrum.Intensities

            ' 腐蚀运算（局部最小值）
            Dim eroded As Double() = MorphologicalErode(y, structElementSize)

            ' 膨胀运算（局部最大值）
            Dim dilated As Double() = MorphologicalDilate(eroded, structElementSize)

            ' 开运算结果即为基线估计
            Dim baseline As Double() = dilated

            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(n - 1) {}
            result.Name = spectrum.Name & "_TopHat"

            For i As Integer = 0 To n - 1
                result.Intensities(i) = y(i) - baseline(i)
            Next

            Return result
        End Function

        ''' <summary>形态学腐蚀（局部最小值）</summary>
        Private Shared Function MorphologicalErode(data As Double(), windowSize As Integer) As Double()
            Dim n As Integer = data.Length
            Dim result As Double() = New Double(n - 1) {}
            Dim halfW As Integer = windowSize \ 2

            For i As Integer = 0 To n - 1
                Dim minVal As Double = Double.MaxValue
                For j As Integer = -halfW To halfW
                    Dim idx As Integer = i + j
                    If idx < 0 Then idx = 0
                    If idx >= n Then idx = n - 1
                    If data(idx) < minVal Then minVal = data(idx)
                Next
                result(i) = minVal
            Next

            Return result
        End Function

        ''' <summary>形态学膨胀（局部最大值）</summary>
        Private Shared Function MorphologicalDilate(data As Double(), windowSize As Integer) As Double()
            Dim n As Integer = data.Length
            Dim result As Double() = New Double(n - 1) {}
            Dim halfW As Integer = windowSize \ 2

            For i As Integer = 0 To n - 1
                Dim maxVal As Double = Double.MinValue
                For j As Integer = -halfW To halfW
                    Dim idx As Integer = i + j
                    If idx < 0 Then idx = 0
                    If idx >= n Then idx = n - 1
                    If data(idx) > maxVal Then maxVal = data(idx)
                Next
                result(i) = maxVal
            Next

            Return result
        End Function

#End Region

        ' --------------------------------------------------------------------
        ' 3. 归一化 / 标准化
        ' --------------------------------------------------------------------

#Region "归一化/标准化"

        ''' <summary>
        ''' 最大值归一化
        ''' 将最强峰强度设为1，其余按比例缩放
        ''' </summary>
        Public Shared Function MaxNormalize(spectrum As RamanSpectrum) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            Dim maxVal As Double = spectrum.Intensities.Max()
            If maxVal = 0 Then maxVal = 1.0

            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(spectrum.PointCount - 1) {}
            result.Name = spectrum.Name & "_MaxNorm"

            For i As Integer = 0 To spectrum.PointCount - 1
                result.Intensities(i) = spectrum.Intensities(i) / maxVal
            Next

            Return result
        End Function

        ''' <summary>
        ''' 面积归一化
        ''' 对整个光谱积分面积为1，常用于定量分析
        ''' </summary>
        Public Shared Function AreaNormalize(spectrum As RamanSpectrum) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount < 2 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If

            ' 使用梯形法则计算积分面积
            Dim area As Double = 0.0
            For i As Integer = 0 To spectrum.PointCount - 2
                Dim dx As Double = spectrum.Wavenumbers(i + 1) - spectrum.Wavenumbers(i)
                area += 0.5 * (spectrum.Intensities(i) + spectrum.Intensities(i + 1)) * Abs(dx)
            Next

            If area = 0 Then area = 1.0

            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(spectrum.PointCount - 1) {}
            result.Name = spectrum.Name & "_AreaNorm"

            For i As Integer = 0 To spectrum.PointCount - 1
                result.Intensities(i) = spectrum.Intensities(i) / area
            Next

            Return result
        End Function

        ''' <summary>
        ''' 向量归一化（L2归一化）
        ''' 将光谱向量归一化到单位长度
        ''' </summary>
        Public Shared Function VectorNormalize(spectrum As RamanSpectrum) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            Dim sumSq As Double = 0.0
            For i As Integer = 0 To spectrum.PointCount - 1
                sumSq += spectrum.Intensities(i) * spectrum.Intensities(i)
            Next
            Dim norm As Double = Sqrt(sumSq)
            If norm = 0 Then norm = 1.0

            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(spectrum.PointCount - 1) {}
            result.Name = spectrum.Name & "_VecNorm"

            For i As Integer = 0 To spectrum.PointCount - 1
                result.Intensities(i) = spectrum.Intensities(i) / norm
            Next

            Return result
        End Function

        ''' <summary>
        ''' 标准正态变量变换（SNV）
        ''' 每条光谱减均值再除标准差，常用于固体/粉末样品
        ''' </summary>
        Public Shared Function SNVNormalize(spectrum As RamanSpectrum) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            Dim n As Integer = spectrum.PointCount
            Dim mean As Double = spectrum.Intensities.Average()

            Dim sumSqDiff As Double = 0.0
            For i As Integer = 0 To n - 1
                sumSqDiff += (spectrum.Intensities(i) - mean) * (spectrum.Intensities(i) - mean)
            Next
            Dim stdDev As Double = Sqrt(sumSqDiff / n)
            If stdDev = 0 Then stdDev = 1.0

            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(n - 1) {}
            result.Name = spectrum.Name & "_SNV"

            For i As Integer = 0 To n - 1
                result.Intensities(i) = (spectrum.Intensities(i) - mean) / stdDev
            Next

            Return result
        End Function

        ''' <summary>
        ''' 均值中心化
        ''' 只减均值不做缩放，常配合PCA/PLS使用
        ''' </summary>
        Public Shared Function MeanCentering(spectrum As RamanSpectrum) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            Dim mean As Double = spectrum.Intensities.Average()

            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(spectrum.PointCount - 1) {}
            result.Name = spectrum.Name & "_MeanCtr"

            For i As Integer = 0 To spectrum.PointCount - 1
                result.Intensities(i) = spectrum.Intensities(i) - mean
            Next

            Return result
        End Function

        ''' <summary>
        ''' 内标归一化
        ''' 使用已知内标峰（如KSCN 2115 cm⁻¹峰）的强度校正其它峰
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="internalStdWavenumber">内标峰波数位置</param>
        ''' <param name="tolerance">搜索容差（cm⁻¹）</param>
        Public Shared Function InternalStandardNormalize(spectrum As RamanSpectrum, internalStdWavenumber As Double, Optional tolerance As Double = 10.0) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            ' 搜索内标峰附近的最大强度
            Dim refIntensity As Double = 0.0
            For i As Integer = 0 To spectrum.PointCount - 1
                If Abs(spectrum.Wavenumbers(i) - internalStdWavenumber) <= tolerance Then
                    If spectrum.Intensities(i) > refIntensity Then
                        refIntensity = spectrum.Intensities(i)
                    End If
                End If
            Next

            If refIntensity = 0 Then
                Throw New InvalidOperationException(String.Format("在波数 {0:F1} ± {1:F1} cm-1 范围内未找到内标峰", internalStdWavenumber, tolerance))
            End If

            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(spectrum.PointCount - 1) {}
            result.Name = spectrum.Name & "_ISNorm"

            For i As Integer = 0 To spectrum.PointCount - 1
                result.Intensities(i) = spectrum.Intensities(i) / refIntensity
            Next

            Return result
        End Function

#End Region

        ' --------------------------------------------------------------------
        ' 4. 裁剪 / 重采样 / 波长校准
        ' --------------------------------------------------------------------

#Region "裁剪/重采样/校准"

        ''' <summary>
        ''' 波数范围裁剪
        ''' 只保留指定波数范围内的数据点
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="wnMin">最小波数</param>
        ''' <param name="wnMax">最大波数</param>
        Public Shared Function CropSpectrum(spectrum As RamanSpectrum, wnMin As Double, wnMax As Double) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            Dim wnList As New List(Of Double)
            Dim intList As New List(Of Double)

            For i As Integer = 0 To spectrum.PointCount - 1
                If spectrum.Wavenumbers(i) >= wnMin AndAlso spectrum.Wavenumbers(i) <= wnMax Then
                    wnList.Add(spectrum.Wavenumbers(i))
                    intList.Add(spectrum.Intensities(i))
                End If
            Next

            If wnList.Count = 0 Then
                Throw New ArgumentException("指定波数范围内无数据点")
            End If

            Dim result As New RamanSpectrum(wnList.ToArray(), intList.ToArray())
            result.Name = spectrum.Name & "_Crop"
            Return result
        End Function

        ''' <summary>
        ''' 重采样/插值
        ''' 将光谱统一到指定波数网格，便于不同仪器/分辨率光谱的比较与建模
        ''' 使用线性插值方法
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="newWavenumbers">目标波数网格</param>
        Public Shared Function ResampleSpectrum(spectrum As RamanSpectrum, newWavenumbers As Double()) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount < 2 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If

            Dim n As Integer = newWavenumbers.Length
            Dim newIntensities As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                newIntensities(i) = LinearInterpolate(spectrum.Wavenumbers, spectrum.Intensities, newWavenumbers(i))
            Next

            Dim result As New RamanSpectrum(CType(newWavenumbers.Clone(), Double()), newIntensities)
            result.Name = spectrum.Name & "_Resample"
            Return result
        End Function

        ''' <summary>
        ''' 重采样到均匀波数网格
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="wnStart">起始波数</param>
        ''' <param name="wnEnd">结束波数</param>
        ''' <param name="step">波数步长</param>
        Public Shared Function ResampleUniform(spectrum As RamanSpectrum, wnStart As Double, wnEnd As Double, [step] As Double) As RamanSpectrum
            Dim numPoints As Integer = CInt(Math.Floor((wnEnd - wnStart) / [step])) + 1
            Dim newWn As Double() = New Double(numPoints - 1) {}
            For i As Integer = 0 To numPoints - 1
                newWn(i) = wnStart + i * [step]
            Next
            Return ResampleSpectrum(spectrum, newWn)
        End Function

        ''' <summary>
        ''' 波数校准
        ''' 使用已知标准峰位（如Si 520.7 cm⁻¹）校准仪器的波数轴
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="measuredPeakPos">测量到的标准峰位置</param>
        ''' <param name="truePeakPos">标准峰的真实位置</param>
        Public Shared Function WavenumberCalibrate(spectrum As RamanSpectrum, measuredPeakPos As Double, truePeakPos As Double) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If

            Dim shift As Double = truePeakPos - measuredPeakPos

            Dim result As New RamanSpectrum()
            result.Wavenumbers = New Double(spectrum.PointCount - 1) {}
            result.Intensities = CType(spectrum.Intensities.Clone(), Double())
            result.Name = spectrum.Name & "_Calib"

            For i As Integer = 0 To spectrum.PointCount - 1
                result.Wavenumbers(i) = spectrum.Wavenumbers(i) + shift
            Next

            Return result
        End Function

#End Region

        ' --------------------------------------------------------------------
        ' 5. 宇宙射线 / 尖峰去除
        ' --------------------------------------------------------------------

#Region "宇宙射线/尖峰去除"

        ''' <summary>
        ''' 中值滤波去尖峰
        ''' 原理：在指定窗口内取中值替代中心点，有效抑制单点尖峰
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="windowSize">窗口大小（推荐3~7，必须为奇数）</param>
        Public Shared Function MedianFilterSpikeRemoval(spectrum As RamanSpectrum, windowSize As Integer) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount = 0 Then
                Throw New ArgumentException("输入光谱不能为空")
            End If
            If windowSize Mod 2 = 0 Then windowSize += 1

            Dim n As Integer = spectrum.PointCount
            Dim result As New RamanSpectrum()
            result.Wavenumbers = CType(spectrum.Wavenumbers.Clone(), Double())
            result.Intensities = New Double(n - 1) {}
            result.Name = spectrum.Name & "_Median"

            Dim halfW As Integer = windowSize \ 2

            For i As Integer = 0 To n - 1
                Dim window As New List(Of Double)
                For j As Integer = -halfW To halfW
                    Dim idx As Integer = i + j
                    If idx < 0 Then idx = 0
                    If idx >= n Then idx = n - 1
                    window.Add(spectrum.Intensities(idx))
                Next
                window.Sort()
                result.Intensities(i) = window(halfW)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 基于导数/阈值检测尖峰并替换
        ''' 原理：检测尖锐局部极值（一阶导数过零且二阶导数负，且幅度超过阈值），用线性插值替换
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="thresholdFactor">阈值因子（推荐5~15，为局部标准差的倍数）</param>
        ''' <param name="sgWindow">SG导数计算窗口</param>
        Public Shared Function DerivativeSpikeRemoval(spectrum As RamanSpectrum, Optional thresholdFactor As Double = 8.0, Optional sgWindow As Integer = 7) As RamanSpectrum
            If spectrum Is Nothing OrElse spectrum.PointCount < 5 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If

            Dim n As Integer = spectrum.PointCount
            Dim result = spectrum.Clone()
            result.Name = spectrum.Name & "_Despike"

            ' 计算一阶差分
            Dim diff As Double() = New Double(n - 1) {}
            diff(0) = 0
            For i As Integer = 1 To n - 1
                diff(i) = spectrum.Intensities(i) - spectrum.Intensities(i - 1)
            Next

            ' 计算局部标准差（使用滑动窗口）
            Dim localStdWin As Integer = Math.Min(51, n)
            If localStdWin Mod 2 = 0 Then localStdWin -= 1
            Dim halfLSW As Integer = localStdWin \ 2

            ' 检测尖峰
            Dim isSpike As Boolean() = New Boolean(n - 1) {}
            For i As Integer = 1 To n - 2
                ' 计算局部MAD（中值绝对偏差）作为鲁棒的标准差估计
                Dim window As New List(Of Double)
                For j As Integer = -halfLSW To halfLSW
                    Dim idx As Integer = i + j
                    If idx < 0 Then idx = 0
                    If idx >= n Then idx = n - 1
                    window.Add(spectrum.Intensities(idx))
                Next
                window.Sort()
                Dim localMedian As Double = window(halfLSW)
                Dim mad As Double = 0.0
                For Each v In window
                    mad += Abs(v - localMedian)
                Next
                mad /= window.Count
                Dim localStd As Double = mad * 1.4826  ' MAD到标准差的转换因子

                ' 检测：当前点与局部中值的偏差超过阈值
                If localStd > 0 AndAlso Abs(spectrum.Intensities(i) - localMedian) > thresholdFactor * localStd Then
                    ' 进一步确认是尖峰：检查是否为局部极值且非常窄
                    If (spectrum.Intensities(i) > spectrum.Intensities(i - 1) AndAlso spectrum.Intensities(i) > spectrum.Intensities(i + 1)) OrElse
                       (spectrum.Intensities(i) < spectrum.Intensities(i - 1) AndAlso spectrum.Intensities(i) < spectrum.Intensities(i + 1)) Then
                        isSpike(i) = True
                    End If
                End If
            Next

            ' 用线性插值替换尖峰
            For i As Integer = 0 To n - 1
                If isSpike(i) Then
                    ' 找到左右非尖峰点
                    Dim leftIdx As Integer = i - 1
                    While leftIdx >= 0 AndAlso isSpike(leftIdx)
                        leftIdx -= 1
                    End While

                    Dim rightIdx As Integer = i + 1
                    While rightIdx < n AndAlso isSpike(rightIdx)
                        rightIdx += 1
                    End While

                    If leftIdx >= 0 AndAlso rightIdx < n Then
                        ' 线性插值
                        Dim t As Double = CDbl(i - leftIdx) / (rightIdx - leftIdx)
                        result.Intensities(i) = spectrum.Intensities(leftIdx) * (1 - t) + spectrum.Intensities(rightIdx) * t
                    ElseIf leftIdx >= 0 Then
                        result.Intensities(i) = spectrum.Intensities(leftIdx)
                    ElseIf rightIdx < n Then
                        result.Intensities(i) = spectrum.Intensities(rightIdx)
                    End If
                End If
            Next

            Return result
        End Function

#End Region

    End Class

    ' ========================================================================
    ' 第二部分：特征提取与峰处理类
    ' ========================================================================
    ''' <summary>
    ''' 拉曼光谱峰分析器，包含峰检测、峰拟合/分峰等功能
    ''' </summary>
    Public Class PeakAnalyzer

        ' --------------------------------------------------------------------
        ' 1. 峰检测 / 峰识别
        ' --------------------------------------------------------------------

#Region "峰检测"

        ''' <summary>
        ''' 一阶导数法峰检测
        ''' 原理：一阶导数由正变负的过零点对应峰位
        ''' </summary>
        ''' <param name="spectrum">输入光谱（建议先平滑去噪）</param>
        ''' <param name="sgWindow">SG导数计算窗口</param>
        ''' <param name="sgPolyOrder">SG多项式阶数</param>
        ''' <param name="minHeight">最小峰高阈值</param>
        ''' <param name="minWidthPoints">最小峰宽（数据点数）</param>
        Public Shared Function DerivativePeakDetect(spectrum As RamanSpectrum, Optional sgWindow As Integer = 11, Optional sgPolyOrder As Integer = 3, Optional minHeight As Double = 0, Optional minWidthPoints As Integer = 3) As List(Of PeakInfo)
            If spectrum Is Nothing OrElse spectrum.PointCount < 5 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If

            Dim peaks As New List(Of PeakInfo)

            ' 计算一阶导数
            Dim d1 As RamanSpectrum = SpectralPreprocessor.SavitzkyGolayFirstDerivative(spectrum, sgWindow, sgPolyOrder)

            ' 寻找一阶导数由正变负的过零点
            For i As Integer = 1 To spectrum.PointCount - 1
                If d1.Intensities(i - 1) > 0 AndAlso d1.Intensities(i) <= 0 Then
                    ' 精确过零点位置（线性插值）
                    Dim t As Double = d1.Intensities(i - 1) / (d1.Intensities(i - 1) - d1.Intensities(i))
                    Dim peakWn As Double = spectrum.Wavenumbers(i - 1) + t * (spectrum.Wavenumbers(i) - spectrum.Wavenumbers(i - 1))
                    Dim peakIdx As Integer = If(spectrum.Intensities(i) > spectrum.Intensities(i - 1), i, i - 1)
                    Dim peakHeight As Double = spectrum.Intensities(peakIdx)

                    ' 最小峰高过滤
                    If peakHeight < minHeight Then Continue For

                    ' 估计峰宽：向两侧搜索到半高位置
                    Dim fwhm As Double = EstimateFWHM(spectrum, peakIdx)
                    Dim halfWidthPts As Integer = CInt(fwhm / ((spectrum.Wavenumbers(spectrum.PointCount - 1) - spectrum.Wavenumbers(0)) / spectrum.PointCount))

                    If halfWidthPts < minWidthPoints Then Continue For

                    ' 估计峰面积（梯形积分）
                    Dim leftB As Integer = Math.Max(0, peakIdx - halfWidthPts * 2)
                    Dim rightB As Integer = Math.Min(spectrum.PointCount - 1, peakIdx + halfWidthPts * 2)
                    Dim area As Double = 0.0
                    For j As Integer = leftB To rightB - 1
                        Dim dx As Double = spectrum.Wavenumbers(j + 1) - spectrum.Wavenumbers(j)
                        area += 0.5 * (spectrum.Intensities(j) + spectrum.Intensities(j + 1)) * Abs(dx)
                    Next

                    Dim pk As New PeakInfo()
                    pk.Position = peakWn
                    pk.Height = peakHeight
                    pk.FWHM = fwhm
                    pk.Area = area
                    pk.LeftBound = spectrum.Wavenumbers(leftB)
                    pk.RightBound = spectrum.Wavenumbers(rightB)
                    peaks.Add(pk)
                End If
            Next

            Return peaks
        End Function

        ''' <summary>
        ''' 二阶导数法峰检测
        ''' 原理：二阶导数的局部极小值对应峰位（对重叠峰分辨能力更强）
        ''' </summary>
        Public Shared Function SecondDerivativePeakDetect(spectrum As RamanSpectrum, Optional sgWindow As Integer = 11, Optional sgPolyOrder As Integer = 3, Optional minHeight As Double = 0, Optional minCurvature As Double = 0) As List(Of PeakInfo)
            If spectrum Is Nothing OrElse spectrum.PointCount < 5 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If

            Dim peaks As New List(Of PeakInfo)

            ' 计算二阶导数
            Dim d2 As RamanSpectrum = SpectralPreprocessor.SavitzkyGolaySecondDerivative(spectrum, sgWindow, sgPolyOrder)

            ' 寻找二阶导数的局部极小值
            For i As Integer = 1 To spectrum.PointCount - 2
                If d2.Intensities(i) < d2.Intensities(i - 1) AndAlso d2.Intensities(i) < d2.Intensities(i + 1) Then
                    ' 二阶导数极小值点对应峰位
                    If d2.Intensities(i) >= -minCurvature Then Continue For

                    Dim peakHeight As Double = spectrum.Intensities(i)
                    If peakHeight < minHeight Then Continue For

                    Dim fwhm As Double = EstimateFWHM(spectrum, i)

                    Dim pk As New PeakInfo()
                    pk.Position = spectrum.Wavenumbers(i)
                    pk.Height = peakHeight
                    pk.FWHM = fwhm
                    pk.Area = EstimatePeakArea(spectrum, i, fwhm)
                    pk.LeftBound = spectrum.Wavenumbers(Math.Max(0, i - CInt(fwhm)))
                    pk.RightBound = spectrum.Wavenumbers(Math.Min(spectrum.PointCount - 1, i + CInt(fwhm)))
                    peaks.Add(pk)
                End If
            Next

            Return peaks
        End Function

        ''' <summary>
        ''' 局部极大搜索 + 阈值过滤
        ''' 原理：直接搜索局部极大值点，配合高度/面积阈值过滤
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="minHeight">最小峰高（绝对值）</param>
        ''' <param name="minProminence">最小突出度（峰高与两侧谷底之差的最小值）</param>
        ''' <param name="minDistance">相邻峰最小间距（数据点数）</param>
        Public Shared Function LocalMaximumPeakDetect(spectrum As RamanSpectrum, Optional minHeight As Double = 0, Optional minProminence As Double = 0, Optional minDistance As Integer = 5) As List(Of PeakInfo)
            If spectrum Is Nothing OrElse spectrum.PointCount < 3 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If

            Dim n As Integer = spectrum.PointCount
            Dim candidates As New List(Of Integer)

            ' 搜索局部极大值
            For i As Integer = 1 To n - 2
                If spectrum.Intensities(i) >= spectrum.Intensities(i - 1) AndAlso
                   spectrum.Intensities(i) >= spectrum.Intensities(i + 1) Then
                    If spectrum.Intensities(i) >= minHeight Then
                        candidates.Add(i)
                    End If
                End If
            Next

            ' 计算突出度并过滤
            Dim filtered As New List(Of Integer)
            For Each idx In candidates
                Dim prominence As Double = ComputeProminence(spectrum.Intensities, idx)
                If prominence >= minProminence Then
                    filtered.Add(idx)
                End If
            Next

            ' 按最小间距合并
            Dim finalPeaks As New List(Of Integer)
            For Each idx In filtered
                Dim tooClose As Boolean = False
                For Each existing In finalPeaks
                    If Abs(idx - existing) < minDistance Then
                        tooClose = True
                        Exit For
                    End If
                Next
                If Not tooClose Then
                    finalPeaks.Add(idx)
                End If
            Next

            ' 构建PeakInfo列表
            Dim peaks As New List(Of PeakInfo)
            For Each idx In finalPeaks
                Dim fwhm As Double = EstimateFWHM(spectrum, idx)
                Dim pk As New PeakInfo()
                pk.Position = spectrum.Wavenumbers(idx)
                pk.Height = spectrum.Intensities(idx)
                pk.FWHM = fwhm
                pk.Area = EstimatePeakArea(spectrum, idx, fwhm)
                pk.LeftBound = spectrum.Wavenumbers(Math.Max(0, idx - CInt(fwhm * 1.5)))
                pk.RightBound = spectrum.Wavenumbers(Math.Min(n - 1, idx + CInt(fwhm * 1.5)))
                peaks.Add(pk)
            Next

            Return peaks
        End Function

        ''' <summary>计算峰的突出度</summary>
        Private Shared Function ComputeProminence(intensities As Double(), peakIdx As Integer) As Double
            Dim n As Integer = intensities.Length
            Dim peakVal As Double = intensities(peakIdx)

            ' 向左搜索谷底
            Dim leftMin As Double = peakVal
            For i As Integer = peakIdx - 1 To 0 Step -1
                If intensities(i) > peakVal Then Exit For
                If intensities(i) < leftMin Then leftMin = intensities(i)
            Next

            ' 向右搜索谷底
            Dim rightMin As Double = peakVal
            For i As Integer = peakIdx + 1 To n - 1
                If intensities(i) > peakVal Then Exit For
                If intensities(i) < rightMin Then rightMin = intensities(i)
            Next

            Return peakVal - Math.Max(leftMin, rightMin)
        End Function

        ''' <summary>估计半高宽</summary>
        Private Shared Function EstimateFWHM(spectrum As RamanSpectrum, peakIdx As Integer) As Double
            Dim n As Integer = spectrum.PointCount
            Dim halfMax As Double = spectrum.Intensities(peakIdx) / 2.0

            ' 向左搜索半高点
            Dim leftIdx As Integer = peakIdx
            For i As Integer = peakIdx - 1 To 0 Step -1
                If spectrum.Intensities(i) <= halfMax Then
                    leftIdx = i
                    Exit For
                End If
                leftIdx = i
            Next

            ' 向右搜索半高点
            Dim rightIdx As Integer = peakIdx
            For i As Integer = peakIdx + 1 To n - 1
                If spectrum.Intensities(i) <= halfMax Then
                    rightIdx = i
                    Exit For
                End If
                rightIdx = i
            Next

            Return Abs(spectrum.Wavenumbers(rightIdx) - spectrum.Wavenumbers(leftIdx))
        End Function

        ''' <summary>估计峰面积</summary>
        Private Shared Function EstimatePeakArea(spectrum As RamanSpectrum, peakIdx As Integer, fwhm As Double) As Double
            Dim n As Integer = spectrum.PointCount
            Dim halfWidthPts As Integer = Math.Max(2, CInt(fwhm * 1.5 / ((spectrum.Wavenumbers(n - 1) - spectrum.Wavenumbers(0)) / n)))
            Dim leftB As Integer = Math.Max(0, peakIdx - halfWidthPts)
            Dim rightB As Integer = Math.Min(n - 1, peakIdx + halfWidthPts)

            Dim area As Double = 0.0
            For j As Integer = leftB To rightB - 1
                Dim dx As Double = spectrum.Wavenumbers(j + 1) - spectrum.Wavenumbers(j)
                area += 0.5 * (spectrum.Intensities(j) + spectrum.Intensities(j + 1)) * Abs(dx)
            Next
            Return area
        End Function

#End Region

        ' --------------------------------------------------------------------
        ' 2. 峰拟合 / 分峰
        ' --------------------------------------------------------------------

#Region "峰拟合"

        ''' <summary>
        ''' 高斯峰函数
        ''' G(x) = A * exp(-(x-c)^2 / (2*sigma^2))
        ''' 适用于对称峰
        ''' </summary>
        Public Shared Function GaussianPeak(x As Double, center As Double, amplitude As Double, sigma As Double) As Double
            Dim diff As Double = x - center
            Return amplitude * Exp(-(diff * diff) / (2.0 * sigma * sigma))
        End Function

        ''' <summary>
        ''' 洛伦兹峰函数
        ''' L(x) = A * sigma^2 / ((x-c)^2 + sigma^2)
        ''' 适用于寿命展宽为主的峰
        ''' </summary>
        Public Shared Function LorentzianPeak(x As Double, center As Double, amplitude As Double, sigma As Double) As Double
            Dim diff As Double = x - center
            Return amplitude * sigma * sigma / (diff * diff + sigma * sigma)
        End Function

        ''' <summary>
        ''' 伪Voigt峰函数（高斯+洛伦兹线性组合）
        ''' V(x) = eta * L(x) + (1-eta) * G(x)
        ''' 更接近实际谱线形状
        ''' </summary>
        ''' <param name="x">波数</param>
        ''' <param name="center">峰位</param>
        ''' <param name="amplitude">峰高</param>
        ''' <param name="sigma">半宽参数</param>
        ''' <param name="lorentzRatio">洛伦兹比例 (0=纯高斯, 1=纯洛伦兹)</param>
        Public Shared Function PseudoVoigtPeak(x As Double, center As Double, amplitude As Double, sigma As Double, lorentzRatio As Double) As Double
            Dim gPart As Double = GaussianPeak(x, center, amplitude, sigma)
            Dim lPart As Double = LorentzianPeak(x, center, amplitude, sigma)
            Return lorentzRatio * lPart + (1.0 - lorentzRatio) * gPart
        End Function

        ''' <summary>
        ''' 单峰拟合（Levenberg-Marquardt算法）
        ''' 对指定范围内的单个峰进行拟合，自动优化峰位、峰高、峰宽参数
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="peakCenterInit">峰位初始估计</param>
        ''' <param name="fitRange">拟合范围（波数），默认为±50 cm⁻¹</param>
        ''' <param name="peakType">峰类型: "Gaussian", "Lorentzian", "Voigt"</param>
        ''' <param name="maxIter">最大迭代次数</param>
        Public Shared Function SinglePeakFit(spectrum As RamanSpectrum, peakCenterInit As Double, Optional fitRange As Double = 50.0, Optional peakType As String = "Gaussian", Optional maxIter As Integer = 200) As PeakInfo
            If spectrum Is Nothing OrElse spectrum.PointCount < 5 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If

            ' 提取拟合范围内的数据
            Dim fitIndices As New List(Of Integer)
            For i As Integer = 0 To spectrum.PointCount - 1
                If Abs(spectrum.Wavenumbers(i) - peakCenterInit) <= fitRange Then
                    fitIndices.Add(i)
                End If
            Next

            If fitIndices.Count < 4 Then
                Throw New ArgumentException("拟合范围内数据点不足")
            End If

            Dim m As Integer = fitIndices.Count
            Dim xData As Double() = New Double(m - 1) {}
            Dim yData As Double() = New Double(m - 1) {}
            For j As Integer = 0 To m - 1
                xData(j) = spectrum.Wavenumbers(fitIndices(j))
                yData(j) = spectrum.Intensities(fitIndices(j))
            Next

            ' 初始参数估计
            Dim peakIdx As Integer = 0
            Dim maxInten As Double = Double.MinValue
            For j As Integer = 0 To m - 1
                If yData(j) > maxInten Then
                    maxInten = yData(j)
                    peakIdx = j
                End If
            Next

            Dim numParams As Integer = If(peakType = "Voigt", 4, 3)
            Dim params As Double() = New Double(numParams - 1) {}
            params(0) = xData(peakIdx)       ' center
            params(1) = maxInten              ' amplitude
            params(2) = 10.0                  ' sigma (初始估计)

            If peakType = "Voigt" Then
                params(3) = 0.5               ' lorentzRatio
            End If

            ' Levenberg-Marquardt 拟合
            Dim lambda As Double = 0.001
            Dim bestParams As Double() = CType(params.Clone(), Double())
            Dim bestCost As Double = ComputeFitCost(xData, yData, bestParams, peakType)

            For iter As Integer = 0 To maxIter - 1
                ' 计算Jacobian矩阵和梯度
                Dim J As Double(,) = New Double(m - 1, numParams - 1) {}
                Dim residuals As Double() = New Double(m - 1) {}

                For ji As Integer = 0 To m - 1
                    residuals(ji) = yData(ji) - EvaluatePeakFunction(xData(ji), bestParams, peakType)
                    For k As Integer = 0 To numParams - 1
                        J(ji, k) = ComputePeakDerivative(xData(ji), bestParams, peakType, k)
                    Next
                Next

                ' J^T * J + lambda * diag(J^T * J)
                Dim JtJ As Double(,) = New Double(numParams - 1, numParams - 1) {}
                Dim JtR As Double() = New Double(numParams - 1) {}

                For i2 As Integer = 0 To numParams - 1
                    For j2 As Integer = 0 To numParams - 1
                        Dim s As Double = 0.0
                        For k As Integer = 0 To m - 1
                            s += J(k, i2) * J(k, j2)
                        Next
                        JtJ(i2, j2) = s
                    Next

                    Dim s2 As Double = 0.0
                    For k As Integer = 0 To m - 1
                        s2 += J(k, i2) * residuals(k)
                    Next
                    JtR(i2) = s2
                Next

                ' 添加阻尼
                For i2 As Integer = 0 To numParams - 1
                    JtJ(i2, i2) *= (1.0 + lambda)
                Next

                ' 求解增量
                Dim delta As Double() = SolveLinearSystem(JtJ, JtR, numParams)

                ' 更新参数
                Dim newParams As Double() = CType(bestParams.Clone(), Double())
                For k As Integer = 0 To numParams - 1
                    newParams(k) += delta(k)
                Next

                ' 确保sigma为正
                If newParams(2) < 0.1 Then newParams(2) = 0.1
                ' 确保amplitude为正
                If newParams(1) < 0 Then newParams(1) = 0.01
                ' Voigt的lorentzRatio限制在[0,1]
                If peakType = "Voigt" Then
                    If newParams(3) < 0 Then newParams(3) = 0.0
                    If newParams(3) > 1.0 Then newParams(3) = 1.0
                End If

                Dim newCost As Double = ComputeFitCost(xData, yData, newParams, peakType)

                If newCost < bestCost Then
                    bestParams = newParams
                    bestCost = newCost
                    lambda *= 0.5
                    If lambda < 0.0000000001 Then lambda = 0.0000000001
                Else
                    lambda *= 2.0
                    If lambda > 10000000000.0 Then Exit For
                End If

                If bestCost < 0.000000000001 Then Exit For
            Next

            ' 构建结果
            Dim result As New PeakInfo()
            result.Position = bestParams(0)
            result.Height = bestParams(1)
            result.FWHM = 2.0 * Sqrt(2.0 * Log(2.0)) * bestParams(2)  ' 高斯FWHM = 2*sqrt(2*ln2)*sigma
            If peakType = "Lorentzian" Then
                result.FWHM = 2.0 * bestParams(2)
            End If

            ' 计算峰面积
            result.Area = ComputePeakArea(bestParams, peakType)

            ' 计算R²
            Dim ssTot As Double = 0.0
            Dim yMean As Double = yData.Average()
            For j As Integer = 0 To m - 1
                ssTot += (yData(j) - yMean) ^ 2
            Next
            result.FitR2 = If(ssTot > 0, 1.0 - bestCost * m / ssTot, 0.0)

            result.LeftBound = xData(0)
            result.RightBound = xData(m - 1)

            Return result
        End Function

        ''' <summary>
        ''' 多峰拟合
        ''' 对多个重叠峰同时进行拟合分解，精确获得每个峰的参数
        ''' </summary>
        ''' <param name="spectrum">输入光谱</param>
        ''' <param name="initialParams">各峰初始参数列表</param>
        ''' <param name="fitRangeLeft">拟合范围左边界波数</param>
        ''' <param name="fitRangeRight">拟合范围右边界波数</param>
        ''' <param name="maxIter">最大迭代次数</param>
        Public Shared Function MultiPeakFit(spectrum As RamanSpectrum, initialParams As List(Of PeakParams),
                                            Optional fitRangeLeft As Double = Double.MinValue,
                                            Optional fitRangeRight As Double = Double.MaxValue,
                                            Optional maxIter As Integer = 500) As List(Of PeakInfo)

            If spectrum Is Nothing OrElse spectrum.PointCount < 5 Then
                Throw New ArgumentException("输入光谱数据点不足")
            End If
            If initialParams Is Nothing OrElse initialParams.Count = 0 Then
                Throw New ArgumentException("必须提供至少一个峰的初始参数")
            End If

            ' 确定拟合范围
            If fitRangeLeft = Double.MinValue Then
                fitRangeLeft = initialParams.Min(Function(p) p.Center) - 100
            End If
            If fitRangeRight = Double.MaxValue Then
                fitRangeRight = initialParams.Max(Function(p) p.Center) + 100
            End If

            ' 提取拟合范围内的数据
            Dim fitIndices As New List(Of Integer)
            For i As Integer = 0 To spectrum.PointCount - 1
                If spectrum.Wavenumbers(i) >= fitRangeLeft AndAlso spectrum.Wavenumbers(i) <= fitRangeRight Then
                    fitIndices.Add(i)
                End If
            Next

            Dim m As Integer = fitIndices.Count
            If m < initialParams.Count * 3 Then
                Throw New ArgumentException("拟合范围内数据点不足以支持" & initialParams.Count & "个峰的拟合")
            End If

            Dim xData As Double() = New Double(m - 1) {}
            Dim yData As Double() = New Double(m - 1) {}
            For j As Integer = 0 To m - 1
                xData(j) = spectrum.Wavenumbers(fitIndices(j))
                yData(j) = spectrum.Intensities(fitIndices(j))
            Next

            ' 构建参数向量
            Dim numPeaks As Integer = initialParams.Count
            Dim paramsPerPeak As Integer = 3  ' center, amplitude, sigma
            Dim totalParams As Integer = numPeaks * paramsPerPeak
            Dim isVoigt As Boolean = initialParams.Any(Function(p) p.PeakType = "Voigt")
            If isVoigt Then
                paramsPerPeak = 4
                totalParams = 0
                For Each p In initialParams
                    totalParams += If(p.PeakType = "Voigt", 4, 3)
                Next
            End If

            Dim params As Double() = New Double(totalParams - 1) {}
            Dim peakTypes As String() = New String(numPeaks - 1) {}
            Dim paramOffset As Integer = 0
            For i As Integer = 0 To numPeaks - 1
                params(paramOffset) = initialParams(i).Center
                params(paramOffset + 1) = initialParams(i).Amplitude
                params(paramOffset + 2) = initialParams(i).Sigma
                peakTypes(i) = initialParams(i).PeakType
                If initialParams(i).PeakType = "Voigt" Then
                    params(paramOffset + 3) = initialParams(i).LorentzRatio
                    paramOffset += 4
                Else
                    paramOffset += 3
                End If
            Next

            ' Levenberg-Marquardt 多峰拟合
            Dim lambda As Double = 0.001
            Dim bestParams As Double() = CType(params.Clone(), Double())
            Dim bestCost As Double = ComputeMultiPeakCost(xData, yData, bestParams, peakTypes)

            For iter As Integer = 0 To maxIter - 1
                ' 计算Jacobian和残差
                Dim J As Double(,) = New Double(m - 1, totalParams - 1) {}
                Dim residuals As Double() = New Double(m - 1) {}

                For ji As Integer = 0 To m - 1
                    residuals(ji) = yData(ji) - EvaluateMultiPeakFunction(xData(ji), bestParams, peakTypes)
                    For k As Integer = 0 To totalParams - 1
                        J(ji, k) = ComputeMultiPeakDerivative(xData(ji), bestParams, peakTypes, k)
                    Next
                Next

                ' J^T * J + lambda * diag
                Dim JtJ As Double(,) = New Double(totalParams - 1, totalParams - 1) {}
                Dim JtR As Double() = New Double(totalParams - 1) {}

                For i2 As Integer = 0 To totalParams - 1
                    For j2 As Integer = 0 To totalParams - 1
                        Dim s As Double = 0.0
                        For k As Integer = 0 To m - 1
                            s += J(k, i2) * J(k, j2)
                        Next
                        JtJ(i2, j2) = s
                    Next
                    Dim s2 As Double = 0.0
                    For k As Integer = 0 To m - 1
                        s2 += J(k, i2) * residuals(k)
                    Next
                    JtR(i2) = s2
                Next

                For i2 As Integer = 0 To totalParams - 1
                    JtJ(i2, i2) *= (1.0 + lambda)
                Next

                Dim delta As Double() = SolveLinearSystem(JtJ, JtR, totalParams)

                Dim newParams As Double() = CType(bestParams.Clone(), Double())
                For k As Integer = 0 To totalParams - 1
                    newParams(k) += delta(k)
                Next

                ' 参数约束
                Dim offset As Integer = 0
                For i2 As Integer = 0 To numPeaks - 1
                    If newParams(offset + 1) < 0 Then newParams(offset + 1) = 0.01  ' amplitude > 0
                    If newParams(offset + 2) < 0.1 Then newParams(offset + 2) = 0.1  ' sigma > 0
                    If peakTypes(i2) = "Voigt" Then
                        If newParams(offset + 3) < 0 Then newParams(offset + 3) = 0.0
                        If newParams(offset + 3) > 1.0 Then newParams(offset + 3) = 1.0
                        offset += 4
                    Else
                        offset += 3
                    End If
                Next

                Dim newCost As Double = ComputeMultiPeakCost(xData, yData, newParams, peakTypes)

                If newCost < bestCost Then
                    bestParams = newParams
                    bestCost = newCost
                    lambda *= 0.5
                    If lambda < 0.0000000001 Then lambda = 0.0000000001
                Else
                    lambda *= 2.0
                    If lambda > 10000000000.0 Then Exit For
                End If

                If bestCost < 0.000000000001 Then Exit For
            Next

            ' 构建结果
            Dim results As New List(Of PeakInfo)
            Dim pOffset As Integer = 0
            For i2 As Integer = 0 To numPeaks - 1
                Dim pk As New PeakInfo()
                pk.Position = bestParams(pOffset)
                pk.Height = bestParams(pOffset + 1)
                Dim sigma As Double = bestParams(pOffset + 2)

                If peakTypes(i2) = "Lorentzian" Then
                    pk.FWHM = 2.0 * sigma
                ElseIf peakTypes(i2) = "Voigt" Then
                    pk.FWHM = 2.0 * Sqrt(2.0 * Log(2.0)) * sigma  ' 近似
                Else
                    pk.FWHM = 2.0 * Sqrt(2.0 * Log(2.0)) * sigma
                End If

                pk.Area = ComputeSinglePeakArea(bestParams, pOffset, peakTypes(i2))
                pk.LeftBound = xData(0)
                pk.RightBound = xData(m - 1)

                ' 计算该峰的R²
                Dim ssTot As Double = 0.0
                Dim yMean As Double = yData.Average()
                For j As Integer = 0 To m - 1
                    ssTot += (yData(j) - yMean) ^ 2
                Next
                pk.FitR2 = If(ssTot > 0, 1.0 - bestCost * m / ssTot, 0.0)

                results.Add(pk)

                If peakTypes(i2) = "Voigt" Then
                    pOffset += 4
                Else
                    pOffset += 3
                End If
            Next

            Return results
        End Function

        ''' <summary>
        ''' 生成拟合峰的合成光谱
        ''' </summary>
        Public Shared Function GenerateFittedSpectrum(wavenumbers As Double(), peakParams As List(Of PeakParams)) As Double()
            Dim n As Integer = wavenumbers.Length
            Dim result As Double() = New Double(n - 1) {}

            For Each pp In peakParams
                For i As Integer = 0 To n - 1
                    Select Case pp.PeakType
                        Case "Gaussian"
                            result(i) += GaussianPeak(wavenumbers(i), pp.Center, pp.Amplitude, pp.Sigma)
                        Case "Lorentzian"
                            result(i) += LorentzianPeak(wavenumbers(i), pp.Center, pp.Amplitude, pp.Sigma)
                        Case "Voigt"
                            result(i) += PseudoVoigtPeak(wavenumbers(i), pp.Center, pp.Amplitude, pp.Sigma, pp.LorentzRatio)
                    End Select
                Next
            Next

            Return result
        End Function

#End Region

        ' --------------------------------------------------------------------
        ' 峰拟合辅助方法
        ' --------------------------------------------------------------------

#Region "峰拟合辅助"

        ''' <summary>评估单峰函数值</summary>
        Private Shared Function EvaluatePeakFunction(x As Double, params As Double(), peakType As String) As Double
            Select Case peakType
                Case "Gaussian"
                    Return GaussianPeak(x, params(0), params(1), params(2))
                Case "Lorentzian"
                    Return LorentzianPeak(x, params(0), params(1), params(2))
                Case "Voigt"
                    Return PseudoVoigtPeak(x, params(0), params(1), params(2), params(3))
                Case Else
                    Return GaussianPeak(x, params(0), params(1), params(2))
            End Select
        End Function

        ''' <summary>计算峰函数对某参数的数值导数</summary>
        Private Shared Function ComputePeakDerivative(x As Double, params As Double(), peakType As String, paramIdx As Integer) As Double
            Dim h As Double = Math.Max(Abs(params(paramIdx)) * 0.000001, 0.00000001)
            Dim paramsPlus As Double() = CType(params.Clone(), Double())
            Dim paramsMinus As Double() = CType(params.Clone(), Double())
            paramsPlus(paramIdx) += h
            paramsMinus(paramIdx) -= h
            Return (EvaluatePeakFunction(x, paramsPlus, peakType) - EvaluatePeakFunction(x, paramsMinus, peakType)) / (2.0 * h)
        End Function

        ''' <summary>计算拟合代价（均方误差）</summary>
        Private Shared Function ComputeFitCost(xData As Double(), yData As Double(), params As Double(), peakType As String) As Double
            Dim sum As Double = 0.0
            For i As Integer = 0 To xData.Length - 1
                Dim diff As Double = yData(i) - EvaluatePeakFunction(xData(i), params, peakType)
                sum += diff * diff
            Next
            Return sum / xData.Length
        End Function

        ''' <summary>评估多峰函数值</summary>
        Private Shared Function EvaluateMultiPeakFunction(x As Double, params As Double(), peakTypes As String()) As Double
            Dim sum As Double = 0.0
            Dim offset As Integer = 0
            For i As Integer = 0 To peakTypes.Length - 1
                If peakTypes(i) = "Voigt" Then
                    sum += PseudoVoigtPeak(x, params(offset), params(offset + 1), params(offset + 2), params(offset + 3))
                    offset += 4
                Else
                    sum += EvaluatePeakFunction(x, New Double() {params(offset), params(offset + 1), params(offset + 2)}, peakTypes(i))
                    offset += 3
                End If
            Next
            Return sum
        End Function

        ''' <summary>计算多峰函数对某参数的数值导数</summary>
        Private Shared Function ComputeMultiPeakDerivative(x As Double, params As Double(), peakTypes As String(), paramIdx As Integer) As Double
            Dim h As Double = Math.Max(Abs(params(paramIdx)) * 0.000001, 0.00000001)
            Dim paramsPlus As Double() = CType(params.Clone(), Double())
            Dim paramsMinus As Double() = CType(params.Clone(), Double())
            paramsPlus(paramIdx) += h
            paramsMinus(paramIdx) -= h
            Return (EvaluateMultiPeakFunction(x, paramsPlus, peakTypes) - EvaluateMultiPeakFunction(x, paramsMinus, peakTypes)) / (2.0 * h)
        End Function

        ''' <summary>计算多峰拟合代价</summary>
        Private Shared Function ComputeMultiPeakCost(xData As Double(), yData As Double(), params As Double(), peakTypes As String()) As Double
            Dim sum As Double = 0.0
            For i As Integer = 0 To xData.Length - 1
                Dim diff As Double = yData(i) - EvaluateMultiPeakFunction(xData(i), params, peakTypes)
                sum += diff * diff
            Next
            Return sum / xData.Length
        End Function

        ''' <summary>计算单峰面积（解析公式）</summary>
        Private Shared Function ComputePeakArea(params As Double(), peakType As String) As Double
            Select Case peakType
                Case "Gaussian"
                    ' 面积 = A * sigma * sqrt(2*pi)
                    Return params(1) * params(2) * Sqrt(2.0 * PI)
                Case "Lorentzian"
                    ' 面积 = A * sigma^2 * pi / sigma = A * sigma * pi
                    Return params(1) * PI * params(2)
                Case "Voigt"
                    ' 近似：按高斯和洛伦兹的加权平均
                    Dim gArea As Double = params(1) * params(2) * Sqrt(2.0 * PI)
                    Dim lArea As Double = params(1) * PI * params(2)
                    Return params(3) * lArea + (1.0 - params(3)) * gArea
                Case Else
                    Return params(1) * params(2) * Sqrt(2.0 * PI)
            End Select
        End Function

        ''' <summary>计算多峰拟合中单个峰的面积</summary>
        Private Shared Function ComputeSinglePeakArea(params As Double(), offset As Integer, peakType As String) As Double
            Return ComputePeakArea(New Double() {params(offset), params(offset + 1), params(offset + 2), If(peakType = "Voigt", params(offset + 3), 0.0)}, peakType)
        End Function

#End Region

    End Class

    ' ========================================================================
    ' 第三部分：全谱降维 / 特征提取
    ' ========================================================================
    ''' <summary>
    ''' 拉曼光谱降维分析器，包含PCA等降维方法
    ''' </summary>
    Public Class DimensionalityReducer

        ''' <summary>
        ''' 主成分分析（PCA）
        ''' 原理：把光谱投影到少数几个主成分，保留主要方差
        ''' 用途：降维、聚类、异常检测、可视化
        ''' </summary>
        ''' <param name="spectra">光谱数组（每条光谱为一行）</param>
        ''' <param name="numComponents">要提取的主成分数</param>
        ''' <returns>PCA结果对象</returns>
        Public Shared Function PCA(spectra As RamanSpectrum(), numComponents As Integer) As PCAResult
            If spectra Is Nothing OrElse spectra.Length = 0 Then
                Throw New ArgumentException("输入光谱数组不能为空")
            End If
            If numComponents <= 0 Then
                Throw New ArgumentException("主成分数必须大于0")
            End If

            Dim nSamples As Integer = spectra.Length
            Dim nFeatures As Integer = spectra(0).PointCount

            ' 检查所有光谱长度一致
            For i As Integer = 1 To nSamples - 1
                If spectra(i).PointCount <> nFeatures Then
                    Throw New ArgumentException("所有光谱的数据点数必须一致")
                End If
            Next

            ' 构建数据矩阵 (nSamples x nFeatures)
            Dim dataMatrix As Double(,) = New Double(nSamples - 1, nFeatures - 1) {}
            For i As Integer = 0 To nSamples - 1
                For j As Integer = 0 To nFeatures - 1
                    dataMatrix(i, j) = spectra(i).Intensities(j)
                Next
            Next

            ' 均值中心化
            Dim means As Double() = New Double(nFeatures - 1) {}
            For j As Integer = 0 To nFeatures - 1
                Dim s As Double = 0.0
                For i As Integer = 0 To nSamples - 1
                    s += dataMatrix(i, j)
                Next
                means(j) = s / nSamples
                For i As Integer = 0 To nSamples - 1
                    dataMatrix(i, j) -= means(j)
                Next
            Next

            ' 计算协方差矩阵 (nFeatures x nFeatures)
            ' Cov = (1/(n-1)) * X^T * X
            Dim covMatrix As Double(,) = New Double(nFeatures - 1, nFeatures - 1) {}
            Dim invN As Double = 1.0 / (nSamples - 1)

            For i As Integer = 0 To nFeatures - 1
                For j As Integer = i To nFeatures - 1
                    Dim s As Double = 0.0
                    For k As Integer = 0 To nSamples - 1
                        s += dataMatrix(k, i) * dataMatrix(k, j)
                    Next
                    covMatrix(i, j) = s * invN
                    covMatrix(j, i) = covMatrix(i, j)
                Next
            Next

            ' 幂迭代法求特征值和特征向量
            Dim eigenValues As Double() = New Double(numComponents - 1) {}
            Dim eigenVectors As Double(,) = New Double(nFeatures - 1, numComponents - 1) {}

            ' 使用逐个提取的deflation方法
            Dim workMatrix As Double(,) = CType(covMatrix.Clone(), Double(,))

            For comp As Integer = 0 To numComponents - 1
                ' 随机初始化特征向量
                Dim v As Double() = New Double(nFeatures - 1) {}
                Dim rng As New Random(comp + 42)
                For i As Integer = 0 To nFeatures - 1
                    v(i) = rng.NextDouble() - 0.5
                Next

                ' 归一化
                Dim norm As Double = 0.0
                For i As Integer = 0 To nFeatures - 1
                    norm += v(i) * v(i)
                Next
                norm = Sqrt(norm)
                For i As Integer = 0 To nFeatures - 1
                    v(i) /= norm
                Next

                ' 幂迭代
                For iter As Integer = 0 To 499
                    Dim newV As Double() = New Double(nFeatures - 1) {}

                    ' 矩阵-向量乘法
                    For i As Integer = 0 To nFeatures - 1
                        Dim s As Double = 0.0
                        For j As Integer = 0 To nFeatures - 1
                            s += workMatrix(i, j) * v(j)
                        Next
                        newV(i) = s
                    Next

                    ' 归一化
                    norm = 0.0
                    For i As Integer = 0 To nFeatures - 1
                        norm += newV(i) * newV(i)
                    Next
                    norm = Sqrt(norm)
                    If norm < 0.000000000000001 Then Exit For

                    For i As Integer = 0 To nFeatures - 1
                        newV(i) /= norm
                    Next

                    ' 检查收敛
                    Dim dotProd As Double = 0.0
                    For i As Integer = 0 To nFeatures - 1
                        dotProd += newV(i) * v(i)
                    Next

                    v = newV

                    If Abs(Abs(dotProd) - 1.0) < 0.0000000001 Then Exit For
                Next

                ' 计算特征值 (Rayleigh商)
                Dim eigenVal As Double = 0.0
                For i As Integer = 0 To nFeatures - 1
                    Dim s As Double = 0.0
                    For j As Integer = 0 To nFeatures - 1
                        s += workMatrix(i, j) * v(j)
                    Next
                    eigenVal += v(i) * s
                Next

                eigenValues(comp) = eigenVal
                For i As Integer = 0 To nFeatures - 1
                    eigenVectors(i, comp) = v(i)
                Next

                ' Deflation: 从工作矩阵中减去该成分
                For i As Integer = 0 To nFeatures - 1
                    For j As Integer = 0 To nFeatures - 1
                        workMatrix(i, j) -= eigenVal * v(i) * v(j)
                    Next
                Next
            Next

            ' 计算得分矩阵 (投影)
            Dim scores As Double(,) = New Double(nSamples - 1, numComponents - 1) {}
            For i As Integer = 0 To nSamples - 1
                For j As Integer = 0 To numComponents - 1
                    Dim s As Double = 0.0
                    For k As Integer = 0 To nFeatures - 1
                        s += dataMatrix(i, k) * eigenVectors(k, j)
                    Next
                    scores(i, j) = s
                Next
            Next

            ' 计算方差解释比例
            Dim totalVariance As Double = 0.0
            For i As Integer = 0 To nFeatures - 1
                totalVariance += covMatrix(i, i)
            Next

            Dim varianceExplained As Double() = New Double(numComponents - 1) {}
            Dim cumulativeVariance As Double() = New Double(numComponents - 1) {}
            Dim cumSum As Double = 0.0
            For i As Integer = 0 To numComponents - 1
                varianceExplained(i) = eigenValues(i) / totalVariance
                cumSum += varianceExplained(i)
                cumulativeVariance(i) = cumSum
            Next

            ' 构建结果
            Dim result As New PCAResult()
            result.EigenValues = eigenValues
            result.EigenVectors = eigenVectors
            result.Scores = scores
            result.Means = means
            result.VarianceExplained = varianceExplained
            result.CumulativeVariance = cumulativeVariance
            result.NumComponents = numComponents
            result.NumSamples = nSamples
            result.NumFeatures = nFeatures

            Return result
        End Function

    End Class

    ' ========================================================================
    ' PCA结果数据结构
    ' ========================================================================
    ''' <summary>
    ''' PCA分析结果
    ''' </summary>
    Public Class PCAResult
        ''' <summary>特征值数组</summary>
        Public EigenValues As Double()
        ''' <summary>特征向量矩阵 (nFeatures x numComponents)</summary>
        Public EigenVectors As Double(,)
        ''' <summary>得分矩阵 (nSamples x numComponents)</summary>
        Public Scores As Double(,)
        ''' <summary>均值向量</summary>
        Public Means As Double()
        ''' <summary>各主成分方差解释比例</summary>
        Public VarianceExplained As Double()
        ''' <summary>累积方差解释比例</summary>
        Public CumulativeVariance As Double()
        ''' <summary>主成分数</summary>
        Public NumComponents As Integer
        ''' <summary>样本数</summary>
        Public NumSamples As Integer
        ''' <summary>特征数</summary>
        Public NumFeatures As Integer

        ''' <summary>
        ''' 将新光谱投影到PCA空间
        ''' </summary>
        Public Function Transform(spectrum As RamanSpectrum) As Double()
            If spectrum.PointCount <> NumFeatures Then
                Throw New ArgumentException("光谱数据点数与PCA模型不匹配")
            End If

            Dim result As Double() = New Double(NumComponents - 1) {}
            For j As Integer = 0 To NumComponents - 1
                Dim s As Double = 0.0
                For k As Integer = 0 To NumFeatures - 1
                    s += (spectrum.Intensities(k) - Means(k)) * EigenVectors(k, j)
                Next
                result(j) = s
            Next
            Return result
        End Function

        ''' <summary>
        ''' 从PCA得分重建光谱
        ''' </summary>
        Public Function Reconstruct(scores As Double()) As Double()
            Dim result As Double() = New Double(NumFeatures - 1) {}
            For k As Integer = 0 To NumFeatures - 1
                Dim s As Double = Means(k)
                For j As Integer = 0 To NumComponents - 1
                    s += scores(j) * EigenVectors(k, j)
                Next
                result(k) = s
            Next
            Return result
        End Function

        ''' <summary>
        ''' 打印PCA摘要信息
        ''' </summary>
        Public Function GetSummary() As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("=== PCA 分析结果 ===")
            sb.AppendLine(String.Format("样本数: {0}", NumSamples))
            sb.AppendLine(String.Format("特征数: {0}", NumFeatures))
            sb.AppendLine(String.Format("提取主成分数: {0}", NumComponents))
            sb.AppendLine()
            sb.AppendLine("主成分    特征值      方差解释比例    累积方差解释比例")
            sb.AppendLine("------    ------      ----------    --------------")
            For i As Integer = 0 To NumComponents - 1
                sb.AppendLine(String.Format("PC{0}      {1:E4}      {2:P2}          {3:P2}",
                    i + 1, EigenValues(i), VarianceExplained(i), CumulativeVariance(i)))
            Next
            Return sb.ToString()
        End Function
    End Class

    ' ========================================================================
    ' 通用数学辅助工具
    ' ========================================================================
    ''' <summary>
    ''' 数学辅助工具类，提供矩阵运算、插值、多项式拟合等基础功能
    ''' </summary>
    Public Class MathHelper

        ''' <summary>
        ''' 矩阵求逆（使用Gauss-Jordan消元法）
        ''' </summary>
        Public Shared Function MatrixInverse(mat As Double(,), n As Integer) As Double(,)
            ' 增广矩阵 [A | I]
            Dim aug As Double(,) = New Double(n - 1, 2 * n - 1) {}
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    aug(i, j) = mat(i, j)
                Next
                aug(i, i + n) = 1.0
            Next

            ' 前向消元
            For col As Integer = 0 To n - 1
                ' 选主元
                Dim maxRow As Integer = col
                Dim maxVal As Double = Abs(aug(col, col))
                For row As Integer = col + 1 To n - 1
                    If Abs(aug(row, col)) > maxVal Then
                        maxVal = Abs(aug(row, col))
                        maxRow = row
                    End If
                Next

                ' 交换行
                If maxRow <> col Then
                    For j As Integer = 0 To 2 * n - 1
                        Dim tmp As Double = aug(col, j)
                        aug(col, j) = aug(maxRow, j)
                        aug(maxRow, j) = tmp
                    Next
                End If

                ' 归一化
                Dim pivot As Double = aug(col, col)
                If Abs(pivot) < 0.000000000000001 Then
                    Throw New InvalidOperationException("矩阵奇异，无法求逆")
                End If
                For j As Integer = 0 To 2 * n - 1
                    aug(col, j) /= pivot
                Next

                ' 消元
                For row As Integer = 0 To n - 1
                    If row = col Then Continue For
                    Dim factor As Double = aug(row, col)
                    For j As Integer = 0 To 2 * n - 1
                        aug(row, j) -= factor * aug(col, j)
                    Next
                Next
            Next

            ' 提取逆矩阵
            Dim inv As Double(,) = New Double(n - 1, n - 1) {}
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    inv(i, j) = aug(i, j + n)
                Next
            Next

            Return inv
        End Function

        ''' <summary>
        ''' 求解线性方程组 Ax = b（使用Gauss消元法）
        ''' </summary>
        Public Shared Function SolveLinearSystem(A As Double(,), b As Double(), n As Integer) As Double()
            ' 增广矩阵
            Dim aug As Double(,) = New Double(n - 1, n) {}
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    aug(i, j) = A(i, j)
                Next
                aug(i, n) = b(i)
            Next

            ' 前向消元（部分选主元）
            For col As Integer = 0 To n - 1
                Dim maxRow As Integer = col
                Dim maxVal As Double = Abs(aug(col, col))
                For row As Integer = col + 1 To n - 1
                    If Abs(aug(row, col)) > maxVal Then
                        maxVal = Abs(aug(row, col))
                        maxRow = row
                    End If
                Next

                If maxRow <> col Then
                    For j As Integer = 0 To n
                        Dim tmp As Double = aug(col, j)
                        aug(col, j) = aug(maxRow, j)
                        aug(maxRow, j) = tmp
                    Next
                End If

                Dim pivot As Double = aug(col, col)
                If Abs(pivot) < 0.000000000000001 Then
                    ' 奇异矩阵，返回最小范数解
                    aug(col, col) = 0.0000000001
                    pivot = 0.0000000001
                End If

                For row As Integer = col + 1 To n - 1
                    Dim factor As Double = aug(row, col) / pivot
                    For j As Integer = col To n
                        aug(row, j) -= factor * aug(col, j)
                    Next
                Next
            Next

            ' 回代
            Dim x As Double() = New Double(n - 1) {}
            For i As Integer = n - 1 To 0 Step -1
                Dim s As Double = aug(i, n)
                For j As Integer = i + 1 To n - 1
                    s -= aug(i, j) * x(j)
                Next
                x(i) = s / aug(i, i)
            Next

            Return x
        End Function

        ''' <summary>
        ''' 加权多项式拟合
        ''' 使用加权最小二乘法拟合多项式系数
        ''' </summary>
        Public Shared Function WeightedPolynomialFit(x As Double(), y As Double(), weights As Double(), order As Integer) As Double()
            Dim m As Integer = order + 1
            Dim n As Integer = x.Length

            ' 构建正规方程 (X^T W X) coeffs = X^T W y
            Dim XtWX As Double(,) = New Double(m - 1, m - 1) {}
            Dim XtWy As Double() = New Double(m - 1) {}

            For i As Integer = 0 To m - 1
                For j As Integer = 0 To m - 1
                    Dim s As Double = 0.0
                    For k As Integer = 0 To n - 1
                        s += weights(k) * Pow(x(k), i + j)
                    Next
                    XtWX(i, j) = s
                Next

                Dim s2 As Double = 0.0
                For k As Integer = 0 To n - 1
                    s2 += weights(k) * Pow(x(k), i) * y(k)
                Next
                XtWy(i) = s2
            Next

            Return SolveLinearSystem(XtWX, XtWy, m)
        End Function

        ''' <summary>
        ''' 评估多项式值
        ''' </summary>
        Public Shared Function EvaluatePolynomial(coeffs As Double(), x As Double) As Double
            Dim result As Double = 0.0
            Dim xPow As Double = 1.0
            For i As Integer = 0 To coeffs.Length - 1
                result += coeffs(i) * xPow
                xPow *= x
            Next
            Return result
        End Function

        ''' <summary>
        ''' 线性插值
        ''' </summary>
        Public Shared Function LinearInterpolate(xData As Double(), yData As Double(), x As Double) As Double
            Dim n As Integer = xData.Length

            ' 边界处理
            If x <= xData(0) Then Return yData(0)
            If x >= xData(n - 1) Then Return yData(n - 1)

            ' 二分搜索
            Dim lo As Integer = 0
            Dim hi As Integer = n - 1
            While hi - lo > 1
                Dim mid As Integer = (lo + hi) \ 2
                If xData(mid) <= x Then
                    lo = mid
                Else
                    hi = mid
                End If
            End While

            ' 线性插值
            Dim t As Double = (x - xData(lo)) / (xData(hi) - xData(lo))
            Return yData(lo) + t * (yData(hi) - yData(lo))
        End Function

        ''' <summary>
        ''' 三次样条插值
        ''' </summary>
        Public Shared Function CubicSplineInterpolate(xData As Double(), yData As Double(), x As Double) As Double
            Dim n As Integer = xData.Length
            If n < 3 Then Return LinearInterpolate(xData, yData, x)

            ' 计算二阶导数（自然样条边界条件）
            Dim h As Double() = New Double(n - 2) {}
            Dim alpha As Double() = New Double(n - 2) {}
            For i As Integer = 0 To n - 2
                h(i) = xData(i + 1) - xData(i)
            Next

            For i As Integer = 1 To n - 2
                alpha(i) = 3.0 * (yData(i + 1) - yData(i)) / h(i) - 3.0 * (yData(i) - yData(i - 1)) / h(i - 1)
            Next

            ' 求解三对角系统
            Dim c As Double() = New Double(n - 1) {}
            Dim l As Double() = New Double(n - 1) {}
            Dim mu As Double() = New Double(n - 1) {}
            Dim z As Double() = New Double(n - 1) {}

            l(0) = 1.0
            mu(0) = 0.0
            z(0) = 0.0

            For i As Integer = 1 To n - 2
                l(i) = 2.0 * (xData(i + 1) - xData(i - 1)) - h(i - 1) * mu(i - 1)
                If Abs(l(i)) < 0.000000000000001 Then l(i) = 0.000000000000001
                mu(i) = h(i) / l(i)
                z(i) = (alpha(i) - h(i - 1) * z(i - 1)) / l(i)
            Next

            l(n - 1) = 1.0
            z(n - 1) = 0.0
            c(n - 1) = 0.0

            Dim b As Double() = New Double(n - 2) {}
            Dim d As Double() = New Double(n - 2) {}

            For j As Integer = n - 2 To 0 Step -1
                c(j) = z(j) - mu(j) * c(j + 1)
                b(j) = (yData(j + 1) - yData(j)) / h(j) - h(j) * (c(j + 1) + 2.0 * c(j)) / 3.0
                d(j) = (c(j + 1) - c(j)) / (3.0 * h(j))
            Next

            ' 找到x所在区间
            If x <= xData(0) Then Return yData(0)
            If x >= xData(n - 1) Then Return yData(n - 1)

            Dim idx As Integer = 0
            For i As Integer = 0 To n - 2
                If x >= xData(i) AndAlso x <= xData(i + 1) Then
                    idx = i
                    Exit For
                End If
            Next

            ' 评估样条
            Dim dx As Double = x - xData(idx)
            Return yData(idx) + b(idx) * dx + c(idx) * dx * dx + d(idx) * dx * dx * dx
        End Function

    End Class

    ' ========================================================================
    ' 流水线处理类：一键式预处理流程
    ' ========================================================================
    ''' <summary>
    ''' 拉曼光谱处理流水线，提供一键式预处理流程
    ''' </summary>
    Public Class RamanProcessingPipeline

        ''' <summary>是否启用宇宙射线去除</summary>
        Public EnableDespike As Boolean = True
        ''' <summary>宇宙射线去除阈值因子</summary>
        Public DespikeThreshold As Double = 8.0

        ''' <summary>是否启用去噪平滑</summary>
        Public EnableSmoothing As Boolean = True
        ''' <summary>平滑方法: "SG", "MA", "Gaussian"</summary>
        Public SmoothMethod As String = "SG"
        ''' <summary>SG窗口大小</summary>
        Public SGWindow As Integer = 11
        ''' <summary>SG多项式阶数</summary>
        Public SGPolyOrder As Integer = 3
        ''' <summary>移动平均窗口</summary>
        Public MAWindow As Integer = 5
        ''' <summary>高斯平滑sigma</summary>
        Public GaussianSigma As Double = 2.0

        ''' <summary>是否启用基线校正</summary>
        Public EnableBaselineCorrection As Boolean = True
        ''' <summary>基线校正方法: "Polynomial", "ALS", "TopHat"</summary>
        Public BaselineMethod As String = "ALS"
        ''' <summary>多项式基线阶数</summary>
        Public PolyBaselineOrder As Integer = 5
        ''' <summary>ALS lambda参数</summary>
        Public ALSLambda As Double = 100000.0
        ''' <summary>ALS p参数</summary>
        Public ALSP As Double = 0.005
        ''' <summary>顶帽结构元素大小</summary>
        Public TopHatSize As Integer = 100

        ''' <summary>是否启用归一化</summary>
        Public EnableNormalization As Boolean = True
        ''' <summary>归一化方法: "Max", "Area", "Vector", "SNV", "MeanCenter"</summary>
        Public NormalizeMethod As String = "Max"

        ''' <summary>是否启用波数裁剪</summary>
        Public EnableCrop As Boolean = False
        ''' <summary>裁剪起始波数</summary>
        Public CropWnMin As Double = 400
        ''' <summary>裁剪结束波数</summary>
        Public CropWnMax As Double = 4000

        ''' <summary>
        ''' 执行完整的预处理流水线
        ''' </summary>
        Public Function Process(spectrum As RamanSpectrum) As RamanSpectrum
            Dim result As RamanSpectrum = spectrum.Clone()

            ' Step 1: 宇宙射线去除
            If EnableDespike Then
                result = SpectralPreprocessor.DerivativeSpikeRemoval(result, DespikeThreshold)
            End If

            ' Step 2: 去噪平滑
            If EnableSmoothing Then
                Select Case SmoothMethod
                    Case "SG"
                        result = SpectralPreprocessor.SavitzkyGolaySmooth(result, SGWindow, SGPolyOrder)
                    Case "MA"
                        result = SpectralPreprocessor.MovingAverageSmooth(result, MAWindow)
                    Case "Gaussian"
                        result = SpectralPreprocessor.GaussianSmooth(result, GaussianSigma)
                End Select
            End If

            ' Step 3: 基线校正
            If EnableBaselineCorrection Then
                Select Case BaselineMethod
                    Case "Polynomial"
                        result = SpectralPreprocessor.PolynomialBaselineCorrect(result, PolyBaselineOrder)
                    Case "ALS"
                        result = SpectralPreprocessor.ALSBaselineCorrect(result, ALSLambda, ALSP)
                    Case "TopHat"
                        result = SpectralPreprocessor.MorphologicalTopHatBaseline(result, TopHatSize)
                End Select
            End If

            ' Step 4: 归一化
            If EnableNormalization Then
                Select Case NormalizeMethod
                    Case "Max"
                        result = SpectralPreprocessor.MaxNormalize(result)
                    Case "Area"
                        result = SpectralPreprocessor.AreaNormalize(result)
                    Case "Vector"
                        result = SpectralPreprocessor.VectorNormalize(result)
                    Case "SNV"
                        result = SpectralPreprocessor.SNVNormalize(result)
                    Case "MeanCenter"
                        result = SpectralPreprocessor.MeanCentering(result)
                End Select
            End If

            ' Step 5: 波数裁剪
            If EnableCrop Then
                result = SpectralPreprocessor.CropSpectrum(result, CropWnMin, CropWnMax)
            End If

            Return result
        End Function

    End Class

End Namespace
