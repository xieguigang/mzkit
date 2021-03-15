#Region "Microsoft.VisualBasic::eb977560f0f23984cf17d7ed817de938, ms2_math-core\Spectra\Models\Trimming\RelativeIntensityCutoff.vb"

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

    '     Class RelativeIntensityCutoff
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: lowAbundanceTrimming, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Spectra

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

            Return spectrum _
                .Where(Function(a) (a.intensity / maxInto) >= m_threshold) _
                .ToArray
        End Function

        Public Overrides Function ToString() As String
            Return $"relative_intensity >= {m_threshold * 100}%"
        End Function

    End Class
End Namespace
