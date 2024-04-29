#Region "Microsoft.VisualBasic::e3cd6f4384937a26488a913803a37397, E:/mzkit/src/mzmath/MSEngine//Search/MsRefSearchParameterBase.vb"

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

    '   Total Lines: 73
    '    Code Lines: 45
    ' Comment Lines: 3
    '   Blank Lines: 25
    '     File Size: 2.61 KB


    ' Class MsRefSearchParameterBase
    ' 
    '     Properties: AbsoluteAmpCutoff, AndromedaScoreCutOff, CcsTolerance, IsUseCcsForAnnotationFiltering, IsUseCcsForAnnotationScoring
    '                 IsUseTimeForAnnotationFiltering, IsUseTimeForAnnotationScoring, MassRangeBegin, MassRangeEnd, MatchedPeaksPercentageCutOff
    '                 MinimumSpectrumMatch, Ms1Tolerance, Ms2Tolerance, RelativeAmpCutoff, ReverseDotProductCutOff
    '                 RiTolerance, RtTolerance, SimpleDotProductCutOff, TotalScoreCutoff, WeightedDotProductCutOff
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

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
    Public Sub New(parameter As MsRefSearchParameterBase)
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
