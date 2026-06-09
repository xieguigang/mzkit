Imports std = System.Math

Namespace PeakAlignment

    Public Module TIC

        ' ========================================================================
        '   TIC色谱轮廓构建
        ' ========================================================================

        ''' <summary>
        ''' 从峰列表构建TIC色谱轮廓
        ''' 将保留时间范围划分为等宽的分段，在每个分段内累加所有峰的响应强度
        ''' </summary>
        ''' <returns>轮廓强度数组和分段中心RT数组</returns>
        Public Function BuildTICProfile(peaks As PeakFeature(), rtMin As Double, rtMax As Double,
                                          binSize As Double) As Tuple(Of Double(), Double())
            Dim nBins As Integer = CInt(std.Ceiling((rtMax - rtMin) / binSize))
            nBins = std.Max(nBins, 1)

            Dim profile As Double() = New Double(nBins - 1) {}
            Dim rtBins As Double() = New Double(nBins - 1) {}

            ' 初始化分段中心RT
            For i As Integer = 0 To nBins - 1
                rtBins(i) = rtMin + (i + 0.5) * binSize
            Next

            ' 累加每个峰的响应强度到对应的分段
            For Each p In peaks
                Dim binIdx As Integer = CInt(std.Floor((p.rt - rtMin) / binSize))
                If binIdx >= 0 AndAlso binIdx < nBins Then
                    ' 使用高斯加权将峰强度分配到相邻分段
                    Dim centerBin As Double = (p.rt - rtMin) / binSize - 0.5
                    Dim sigma As Double = std.Max((p.rtmax - p.rtmin) / binSize / 2.0, 1.0)

                    For b As Integer = std.Max(0, CInt(std.Floor(centerBin - 3 * sigma))) To std.Min(nBins - 1, CInt(std.Ceiling(centerBin + 3 * sigma)))
                        Dim dist As Double = b - centerBin
                        Dim weight As Double = std.Exp(-0.5 * dist * dist / (sigma * sigma))
                        profile(b) += p.maxInto * weight
                    Next
                End If
            Next

            Return Tuple.Create(profile, rtBins)
        End Function

        ''' <summary>
        ''' 归一化轮廓到[0,1]范围
        ''' </summary>
        Public Function NormalizeProfile(profile As Double()) As Double()
            Dim minVal As Double = profile.Min()
            Dim maxVal As Double = profile.Max()
            Dim range As Double = maxVal - minVal

            If range < Double.Epsilon Then
                Dim result As Double() = New Double(profile.Length - 1) {}
                Return result
            End If

            Dim normalized As Double() = New Double(profile.Length - 1) {}
            For i As Integer = 0 To profile.Length - 1
                normalized(i) = (profile(i) - minVal) / range
            Next
            Return normalized
        End Function

    End Module
End Namespace