#Region "Microsoft.VisualBasic::9167f8da917f66bde814246393f6fc5b, src\mzmath\ms2_math-core\Spectra\LowAbundanceTrimming.vb"

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

    ' Class LowAbundanceTrimming
    ' 
    '     Properties: [Default], intoCutff, quantCutoff, threshold
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Trim
    ' 
    ' Class RelativeIntensityCutoff
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: lowAbundanceTrimming, ToString
    ' 
    ' Class QuantileIntensityCutoff
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: lowAbundanceTrimming, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Math.Quantile

Public MustInherit Class LowAbundanceTrimming

    Protected ReadOnly m_threshold As Double

    Public ReadOnly Property threshold As Double
        Get
            Return m_threshold
        End Get
    End Property

    Sub New(cutoff As Double)
        If cutoff > 1 Then
            m_threshold = cutoff / 100
        Else
            m_threshold = cutoff
        End If

        If cutoff <= 0 Then
            Call $"the threshold value for trimming low abundance fragment is ZERO or negative value, no item will be trimmed!".Warning
        End If
    End Sub

    Public Shared ReadOnly Property intoCutff As New RelativeIntensityCutoff(0.05)
    Public Shared ReadOnly Property quantCutoff As New QuantileIntensityCutoff(0.65)

    Public Shared ReadOnly Property [Default] As New [Default](Of LowAbundanceTrimming)(intoCutff)

    Public Function Trim(spectrum As IEnumerable(Of ms2)) As ms2()
        If m_threshold <= 0 Then
            Return spectrum.ToArray
        Else
            Return lowAbundanceTrimming(spectrum.ToArray)
        End If
    End Function

    Protected MustOverride Function lowAbundanceTrimming(spectrum As ms2()) As ms2()
    Public MustOverride Overrides Function ToString() As String

End Class

Public Class RelativeIntensityCutoff : Inherits LowAbundanceTrimming

    Public Sub New(cutoff As Double)
        MyBase.New(cutoff)
    End Sub

    Protected Overrides Function lowAbundanceTrimming(spectrum() As ms2) As ms2()
        Dim maxInto As Double = -999

        For Each fragment As ms2 In spectrum
            If fragment.intensity > maxInto Then
                maxInto = fragment.intensity
            End If
        Next

        For Each fragment As ms2 In spectrum
            fragment.quantity = fragment.intensity / maxInto
        Next

        Return spectrum.Where(Function(a) a.quantity >= m_threshold).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return $"relative_intensity >= {m_threshold * 100}%"
    End Function

End Class

Public Class QuantileIntensityCutoff : Inherits LowAbundanceTrimming

    Public Sub New(cutoff As Double)
        MyBase.New(cutoff)
    End Sub

    Protected Overrides Function lowAbundanceTrimming(spectrum() As ms2) As ms2()
        Dim quantile = spectrum.Select(Function(a) a.intensity).GKQuantile
        Dim threshold As Double = quantile.Query(Me.m_threshold)

        Return spectrum.Where(Function(a) a.intensity >= threshold).ToArray
    End Function

    Public Overrides Function ToString() As String
        Return $"intensity_quantile >= {m_threshold}"
    End Function

End Class
