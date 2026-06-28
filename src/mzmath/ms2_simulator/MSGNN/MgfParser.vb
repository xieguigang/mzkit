' ============================================================================
' MgfParser.vb
' ============================================================================
' MGF (Mascot Generic Format) 文件解析器
' 读取MS/MS二级质谱图数据
' ============================================================================

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Module MgfParser

    ''' <summary>
    ''' 解析MGF文件，返回所有谱图
    ''' </summary>
    ''' <param name="filePath">MGF文件路径</param>
    ''' <returns>谱图列表</returns>
    Public Function ParseMGF(filePath As String) As List(Of Spectrum)
        Dim spectra As New List(Of Spectrum)()

        If Not File.Exists(filePath) Then
            Throw New FileNotFoundException($"MGF文件未找到: {filePath}")
        End If

        Dim lines = File.ReadAllLines(filePath)
        Dim currentSpectrum As Spectrum = Nothing
        Dim inIonsBlock As Boolean = False

        For Each line In lines
            line = line.Trim()

            If line = "" Then Continue For

            ' 开始一个新谱图
            If line.Equals("BEGIN IONS", StringComparison.OrdinalIgnoreCase) Then
                currentSpectrum = New Spectrum()
                inIonsBlock = True
                Continue For
            End If

            ' 结束当前谱图
            If line.Equals("END IONS", StringComparison.OrdinalIgnoreCase) Then
                If currentSpectrum IsNot Nothing Then
                    ' 如果没有指定MSLevel，默认为2
                    If currentSpectrum.MsLevel = 0 Then
                        currentSpectrum.MsLevel = 2
                    End If
                    ' 如果母离子强度为0，设为基峰强度
                    If currentSpectrum.PepIntensity = 0.0 AndAlso currentSpectrum.Peaks.Count > 0 Then
                        currentSpectrum.PepIntensity = currentSpectrum.BasePeakIntensity
                    End If
                    spectra.Add(currentSpectrum)
                End If
                currentSpectrum = Nothing
                inIonsBlock = False
                Continue For
            End If

            If Not inIonsBlock OrElse currentSpectrum Is Nothing Then
                Continue For
            End If

            ' 解析属性行 (KEY=VALUE)
            If line.Contains("=") Then
                Dim eqIdx = line.IndexOf("="c)
                Dim key = line.Substring(0, eqIdx).Trim().ToUpper()
                Dim value = line.Substring(eqIdx + 1).Trim()

                Select Case key
                    Case "TITLE"
                        currentSpectrum.Title = value
                    Case "PEPMASS"
                        Dim parts = value.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
                        If parts.Length > 0 AndAlso Double.TryParse(parts(0), currentSpectrum.PepMass) Then
                            ' 成功解析
                        End If
                        If parts.Length > 1 Then
                            Double.TryParse(parts(1), currentSpectrum.PepIntensity)
                        End If
                    Case "CHARGE"
                        ' 解析电荷，如 "2+" 或 "2"
                        Dim chargeStr = value.Replace("+", "").Replace("-", "").Trim()
                        Integer.TryParse(chargeStr, currentSpectrum.Charge)
                        If value.Contains("-") Then
                            currentSpectrum.Charge = -currentSpectrum.Charge
                        End If
                    Case "MSLEVEL"
                        Integer.TryParse(value, currentSpectrum.MsLevel)
                    Case "RTINSECONDS"
                        Double.TryParse(value, currentSpectrum.RetentionTime)
                    Case "SCANS"
                        currentSpectrum.Metadata("SCANS") = value
                    Case "NATIVEID"
                        currentSpectrum.Metadata("NATIVEID") = value
                    Case "INSTRUMENT"
                        currentSpectrum.Metadata("INSTRUMENT") = value
                    Case "PID"
                        currentSpectrum.Metadata("PID") = value
                    Case Else
                        currentSpectrum.Metadata(key) = value
                End Select
                Continue For
            End If

            ' 解析峰行 (mz intensity [additional...])
            Dim peakParts = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
            If peakParts.Length >= 2 Then
                Dim mz As Double, intensity As Double
                If Double.TryParse(peakParts(0), mz) AndAlso Double.TryParse(peakParts(1), intensity) Then
                    currentSpectrum.Peaks.Add(New ms2(mz, intensity))
                End If
            End If
        Next

        Return spectra
    End Function

    ''' <summary>
    ''' 解析MGF文件，返回所有谱图 (带进度回调)
    ''' </summary>
    Public Function ParseMGF(filePath As String, progressCallback As Action(Of Integer, Integer)) As List(Of Spectrum)
        Dim spectra = ParseMGF(filePath)
        progressCallback?.Invoke(spectra.Count, spectra.Count)
        Return spectra
    End Function

End Module
