Imports std = System.Math

Namespace MaxLFQ

    ' 定义数据结构
    Public Class PeptideQuant
        Public Property PeptideID As String       ' 多肽唯一标识
        Public Property ProteinID As String       ' 所属蛋白质ID
        Public Property Intensities As Double()  ' 各样本中该多肽的强度值（数组索引=样本ID）
    End Class

    Public Class MaxLFQResult
        Public Property ProteinID As String
        Public Property LFQIntensity As Double()  ' 优化后的蛋白质丰度（每个样本一个值）
    End Class

    Public Module Math

        ' MaxLFQ核心计算函数
        Public Function RunMaxLFQ(peptides As List(Of PeptideQuant), sampleCount As Integer) As List(Of MaxLFQResult)
            ' 步骤1: 构建肽段-样本强度矩阵
            Dim peptideMatrix(sampleCount - 1)() As Double
            For i As Integer = 0 To sampleCount - 1
                peptideMatrix(i) = peptides.Select(Function(p) p.Intensities(i)).ToArray()
            Next

            ' 步骤2: 按蛋白质分组肽段
            Dim proteinGroups = peptides.GroupBy(Function(p) p.ProteinID).ToList()

            ' 存储结果
            Dim results As New List(Of MaxLFQResult)

            ' 遍历每个蛋白质组
            For Each proteinGroup In proteinGroups
                Dim proteinID As String = proteinGroup.Key
                Dim groupPeptides = proteinGroup.ToList()

                ' 步骤3: 构建肽段比率矩阵（样本间两两比较）
                Dim ratioMatrix(sampleCount - 1, sampleCount - 1) As Double
                Dim validPairs(sampleCount - 1, sampleCount - 1) As Integer ' 记录有效比率数量

                For i As Integer = 0 To sampleCount - 1
                    For j As Integer = 0 To sampleCount - 1
                        If i = j Then Continue For

                        Dim ratios As New List(Of Double)
                        ' 遍历该蛋白质的所有肽段
                        For Each pep In groupPeptides
                            ' 仅当两样本均检测到该肽段时才计算比率
                            If pep.Intensities(i) > 0 AndAlso pep.Intensities(j) > 0 Then
                                ratios.Add(pep.Intensities(i) / pep.Intensities(j))
                            End If
                        Next

                        ' 若有有效比率，取几何均值
                        If ratios.Count > 0 Then
                            ratioMatrix(i, j) = std.Exp(ratios.Average(Function(r) std.Log(r)))
                            validPairs(i, j) = ratios.Count
                        End If
                    Next
                Next

                ' 步骤4: 构建样本连通图并求解线性方程组
                Dim proteinIntensity(sampleCount - 1) As Double
                Dim connectedComponents = FindConnectedComponents(validPairs, sampleCount)

                For Each comp In connectedComponents
                    ' 构建方程组: log(Int_i) - log(Int_j) = log(Ratio_ij)
                    Dim equations As New List(Of Equation)
                    Dim variables As Integer = comp.Count

                    For i As Integer = 0 To comp.Count - 1
                        For j As Integer = 0 To comp.Count - 1
                            If i <> j AndAlso validPairs(comp(i), comp(j)) > 0 Then
                                equations.Add(New Equation With {
                                    .SampleA = comp(i),
                                    .SampleB = comp(j),
                                    .Value = std.Log(ratioMatrix(comp(i), comp(j)))
                                })
                            End If
                        Next
                    Next

                    ' 求解最小二乘问题: A * x = b
                    Dim A(equations.Count - 1, variables - 1) As Double
                    Dim b(equations.Count - 1) As Double

                    For eqIdx As Integer = 0 To equations.Count - 1
                        Dim eq = equations(eqIdx)
                        ' 设置系数: SampleA列=1, SampleB列=-1
                        A(eqIdx, comp.IndexOf(eq.SampleA)) = 1
                        A(eqIdx, comp.IndexOf(eq.SampleB)) = -1
                        b(eqIdx) = eq.Value
                    Next

                    ' 使用奇异值分解（SVD）求解
                    Dim x = SolveLeastSquares(A, b)
                    ' 将解映射回样本强度（取指数还原为线性值）
                    For i As Integer = 0 To variables - 1
                        proteinIntensity(comp(i)) = std.Exp(x(i))
                    Next
                Next

                ' 步骤5: 归一化（可选，通常以参考样本为基准）
                Dim reference = proteinIntensity(0) ' 取第一个样本为基准
                For i As Integer = 0 To sampleCount - 1
                    proteinIntensity(i) /= reference
                Next

                results.Add(New MaxLFQResult With {
                    .ProteinID = proteinID,
                    .LFQIntensity = proteinIntensity
                })
            Next

            Return results
        End Function

        ' --- 辅助函数 ---
        ' 查找样本连通分量（通过有效比率连接的样本子图）
        Private Function FindConnectedComponents(validPairs(,) As Integer, sampleCount As Integer) As List(Of List(Of Integer))
            Dim visited(sampleCount - 1) As Boolean
            Dim components As New List(Of List(Of Integer))

            For i As Integer = 0 To sampleCount - 1
                If Not visited(i) Then
                    Dim comp As New List(Of Integer)
                    DFS(i, visited, comp, validPairs)
                    If comp.Count > 1 Then components.Add(comp) ' 仅包含连通样本
                End If
            Next
            Return components
        End Function

        ' 深度优先搜索（DFS）遍历连通样本
        Private Sub DFS(sample As Integer, visited() As Boolean, comp As List(Of Integer), validPairs(,) As Integer)
            visited(sample) = True
            comp.Add(sample)
            For j As Integer = 0 To visited.Length - 1
                If Not visited(j) AndAlso validPairs(sample, j) > 0 Then
                    DFS(j, visited, comp, validPairs)
                End If
            Next
        End Sub

        ' 最小二乘求解器（伪代码，实际需调用数学库）
        Private Function SolveLeastSquares(A(,) As Double, b() As Double) As Double()
            ' 实际实现应使用SVD或QR分解（如MathNet.Numerics）
        End Function
    End Module

    Public Class Equation
        Public Property SampleA As Integer
        Public Property SampleB As Integer
        Public Property Value As Double
    End Class
End Namespace