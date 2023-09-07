
Public Class MsRefSearchParameterBase

    Public Property MassRangeBegin As Single = 0

    Public Property MassRangeEnd As Single = 2000

    Public Property RtTolerance As Single = 100.0F

    Public Property RiTolerance As Single = 100.0F

    Public Property CcsTolerance As Single = 20.0F

    Public Property Ms1Tolerance As Single = 0.01F

    Public Property Ms2Tolerance As Single = 0.025F

    Public Property RelativeAmpCutoff As Single = 0F

    Public Property AbsoluteAmpCutoff As Single = 0

    ' by [0-1]

    Public Property WeightedDotProductCutOff As Single = 0.6F

    Public Property SimpleDotProductCutOff As Single = 0.6F

    Public Property ReverseDotProductCutOff As Single = 0.8F

    Public Property MatchedPeaksPercentageCutOff As Single = 0.25F

    Public Property AndromedaScoreCutOff As Single = 0.1F

    Public Property TotalScoreCutoff As Single = 0.8F

    ' by absolute value

    Public Property MinimumSpectrumMatch As Single = 3

    ' option

    Public Property IsUseTimeForAnnotationFiltering As Boolean = False

    Public Property IsUseTimeForAnnotationScoring As Boolean = False

    Public Property IsUseCcsForAnnotationFiltering As Boolean = False

    Public Property IsUseCcsForAnnotationScoring As Boolean = False


    Public Sub New()
    End Sub
    Public Sub New(ByVal parameter As MsRefSearchParameterBase)
        MassRangeBegin = parameter.MassRangeBegin
        MassRangeEnd = parameter.MassRangeEnd
        RtTolerance = parameter.RtTolerance
        RiTolerance = parameter.RiTolerance
        CcsTolerance = parameter.CcsTolerance
        Ms1Tolerance = parameter.Ms1Tolerance
        Ms2Tolerance = parameter.Ms2Tolerance
        RelativeAmpCutoff = parameter.RelativeAmpCutoff
        AbsoluteAmpCutoff = parameter.AbsoluteAmpCutoff
        WeightedDotProductCutOff = parameter.WeightedDotProductCutOff
        SimpleDotProductCutOff = parameter.SimpleDotProductCutOff
        ReverseDotProductCutOff = parameter.ReverseDotProductCutOff
        MatchedPeaksPercentageCutOff = parameter.MatchedPeaksPercentageCutOff
        TotalScoreCutoff = parameter.TotalScoreCutoff
        MinimumSpectrumMatch = parameter.MinimumSpectrumMatch
        IsUseTimeForAnnotationFiltering = parameter.IsUseTimeForAnnotationFiltering
        IsUseTimeForAnnotationScoring = parameter.IsUseTimeForAnnotationScoring
        IsUseCcsForAnnotationFiltering = parameter.IsUseCcsForAnnotationFiltering
        IsUseCcsForAnnotationScoring = parameter.IsUseCcsForAnnotationScoring
    End Sub
End Class
