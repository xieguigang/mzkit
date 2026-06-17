Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

''' <summary>
''' Mummichog算法参数配置
''' </summary>
Public Class MummichogParams

    ''' <summary>质量匹配容忍度 (ppm), 默认5.0</summary>
    Public Property PpmTolerance As Double = 5.0

    ''' <summary>差异峰p值阈值, 默认0.05</summary>
    Public Property PValueCutoff As Double = 0.05

    ''' <summary>通路富集FDR阈值, 默认0.2</summary>
    Public Property FdrCutoff As Double = 0.2

    ''' <summary>电离模式, 默认正离子</summary>
    Public Property Mode As IonModes = IonModes.Positive

    ''' <summary>同位素搜索m/z窗口 (ppm), 默认5.0</summary>
    Public Property IsotopePpmTolerance As Double = 10.0

    ''' <summary>同位素强度匹配容忍倍数, 默认3倍</summary>
    Public Property IsotopeIntensityTolerance As Double = 3.0

    ' --- 优先级打分权重 ---
    Public Property WeightPathway As Double = 3.0
    Public Property WeightAdduct As Double = 2.0
    Public Property WeightIsotope As Double = 1.5
    Public Property WeightMass As Double = 1.0

    ''' <summary>最大保留的注释结果数 (0=不限制)</summary>
    Public Property MaxResults As Integer = 0

    ''' <summary>每个峰最多保留的候选注释数</summary>
    Public Property MaxCandidatesPerPeak As Integer = 5
End Class
