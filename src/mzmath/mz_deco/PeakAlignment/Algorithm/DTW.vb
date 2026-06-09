Imports std = System.Math

Namespace PeakAlignment

    Public Module DTW

        ' ========================================================================
        '   DTW（动态时间规整）实现
        ' ========================================================================

        ''' <summary>
        ''' 计算两个序列之间的DTW最优对齐路径
        ''' 
        ''' 使用带约束的DTW算法，通过Sakoe-Chiba带限制搜索范围，
        ''' 减少计算量并避免不合理的对齐。
        ''' </summary>
        ''' <param name="refProfile">参考色谱轮廓</param>
        ''' <param name="sampleProfile">样本色谱轮廓</param>
        ''' <param name="gapPenalty">间隙惩罚值</param>
        ''' <returns>最优对齐路径，每项为(参考索引, 样本索引)</returns>
        Public Function ComputeDTW(refProfile As Double(), sampleProfile As Double(),
                                     gapPenalty As Double) As List(Of Tuple(Of Integer, Integer))
            Dim nRef As Integer = refProfile.Length
            Dim nSample As Integer = sampleProfile.Length

            If nRef = 0 OrElse nSample = 0 Then
                Return New List(Of Tuple(Of Integer, Integer))
            End If

            ' 归一化轮廓
            Dim normRef As Double() = NormalizeProfile(refProfile)
            Dim normSample As Double() = NormalizeProfile(sampleProfile)

            ' 代价矩阵
            Dim cost As Double(,) = New Double(nRef - 1, nSample - 1) {}
            ' 累积代价矩阵
            Dim dtw As Double(,) = New Double(nRef - 1, nSample - 1) {}
            ' 路径追踪矩阵
            Dim traceback As Integer(,) = New Integer(nRef - 1, nSample - 1) {}
            ' 0: 对角线, 1: 上方, 2: 左方

            ' 计算局部代价（使用欧氏距离）
            For i As Integer = 0 To nRef - 1
                For j As Integer = 0 To nSample - 1
                    cost(i, j) = std.Abs(normRef(i) - normSample(j))
                Next
            Next

            ' 初始化
            dtw(0, 0) = cost(0, 0)
            traceback(0, 0) = 0

            ' 填充第一行
            For j As Integer = 1 To nSample - 1
                dtw(0, j) = dtw(0, j - 1) + cost(0, j) + gapPenalty
                traceback(0, j) = 2 ' 左方
            Next

            ' 填充第一列
            For i As Integer = 1 To nRef - 1
                dtw(i, 0) = dtw(i - 1, 0) + cost(i, 0) + gapPenalty
                traceback(i, 0) = 1 ' 上方
            Next

            ' 填充其余部分
            For i As Integer = 1 To nRef - 1
                ' Sakoe-Chiba带约束，限制搜索宽度为对角线附近
                Dim bandWidth As Integer = CInt(std.Max(10, std.Ceiling(nSample * 0.1)))
                Dim jStart As Integer = std.Max(1, i - bandWidth)
                Dim jEnd As Integer = std.Min(nSample - 1, i + bandWidth)

                For j As Integer = jStart To jEnd
                    Dim diag As Double = dtw(i - 1, j - 1) + cost(i, j)
                    Dim up As Double = dtw(i - 1, j) + cost(i, j) + gapPenalty
                    Dim left As Double = dtw(i, j - 1) + cost(i, j) + gapPenalty

                    If diag <= up AndAlso diag <= left Then
                        dtw(i, j) = diag
                        traceback(i, j) = 0
                    ElseIf up <= left Then
                        dtw(i, j) = up
                        traceback(i, j) = 1
                    Else
                        dtw(i, j) = left
                        traceback(i, j) = 2
                    End If
                Next
            Next

            ' 回溯最优路径
            Dim path As New List(Of Tuple(Of Integer, Integer))
            Dim ci As Integer = nRef - 1
            Dim cj As Integer = nSample - 1

            While ci > 0 OrElse cj > 0
                path.Add(Tuple.Create(ci, cj))

                If ci = 0 Then
                    cj -= 1
                ElseIf cj = 0 Then
                    ci -= 1
                Else
                    Select Case traceback(ci, cj)
                        Case 0 : ci -= 1 : cj -= 1
                        Case 1 : ci -= 1
                        Case 2 : cj -= 1
                    End Select
                End If
            End While
            path.Add(Tuple.Create(0, 0))

            ' 反转路径（从起点到终点）
            path.Reverse()

            Return path
        End Function

        ''' <summary>
        ''' 从DTW对齐路径构建RT校正函数
        ''' 
        ''' 将离散的对齐路径转换为连续的RT校正映射。
        ''' 对于任意给定的样本RT值，通过线性插值计算对应的参考RT值。
        ''' </summary>
        Private Function BuildRTCorrectionFromWarpPath(path As List(Of Tuple(Of Integer, Integer)),
                                                        rtBins As Double(),
                                                        rtMin As Double,
                                                        binSize As Double) As Func(Of Double, Double)
            ' 构建样本RT -> 参考RT的映射点
            Dim mappingPoints As New List(Of Tuple(Of Double, Double))

            For Each pt In path
                Dim sampleRt As Double = rtMin + pt.Item2 * binSize
                Dim refRt As Double = rtMin + pt.Item1 * binSize
                mappingPoints.Add(Tuple.Create(sampleRt, refRt))
            Next

            ' 去除重复的样本RT点（保留最后一个映射）
            Dim uniqueMapping As New List(Of Tuple(Of Double, Double))
            Dim lastSampleRt As Double = Double.NaN

            For i As Integer = mappingPoints.Count - 1 To 0 Step -1
                If Double.IsNaN(lastSampleRt) OrElse std.Abs(mappingPoints(i).Item1 - lastSampleRt) > Double.Epsilon Then
                    uniqueMapping.Add(mappingPoints(i))
                    lastSampleRt = mappingPoints(i).Item1
                End If
            Next
            uniqueMapping.Reverse()

            ' 返回线性插值函数
            Return Function(sampleRt As Double) As Double
                       If uniqueMapping.Count = 0 Then Return sampleRt
                       If uniqueMapping.Count = 1 Then Return uniqueMapping(0).Item2

                       ' 边界处理
                       If sampleRt <= uniqueMapping(0).Item1 Then
                           Return uniqueMapping(0).Item2
                       End If
                       If sampleRt >= uniqueMapping.Last.Item1 Then
                           Return uniqueMapping.Last.Item2
                       End If

                       ' 二分查找
                       Dim lo As Integer = 0
                       Dim hi As Integer = uniqueMapping.Count - 1
                       While lo < hi - 1
                           Dim mid As Integer = (lo + hi) \ 2
                           If uniqueMapping(mid).Item1 <= sampleRt Then
                               lo = mid
                           Else
                               hi = mid
                           End If
                       End While

                       ' 线性插值
                       Dim x0 As Double = uniqueMapping(lo).Item1
                       Dim x1 As Double = uniqueMapping(hi).Item1
                       Dim y0 As Double = uniqueMapping(lo).Item2
                       Dim y1 As Double = uniqueMapping(hi).Item2

                       If std.Abs(x1 - x0) < Double.Epsilon Then
                           Return y0
                       End If

                       Dim t As Double = (sampleRt - x0) / (x1 - x0)
                       Return y0 + t * (y1 - y0)
                   End Function
        End Function

    End Module
End Namespace