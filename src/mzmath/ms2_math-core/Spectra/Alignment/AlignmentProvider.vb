#Region "Microsoft.VisualBasic::9acbf76d22b68b6861049237d299efe0, ms2_math-core\Spectra\Alignment\AlignmentProvider.vb"

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

    '     Class AlignmentProvider
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) CreateAlignment, GetMeta
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace Spectra

    Public MustInherit Class AlignmentProvider

        Protected intocutoff As LowAbundanceTrimming
        Protected mzwidth As Tolerance

        Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming)
            Me.mzwidth = mzwidth
            Me.intocutoff = intocutoff
        End Sub

        Public MustOverride Function GetScore(a As ms2(), b As ms2()) As Double
        Public MustOverride Function GetScore(alignment As SSM2MatrixFragment()) As (forward#, reverse#)

        Public Function CreateAlignment(a As PeakMs2, b As PeakMs2) As AlignmentOutput
            Dim align As AlignmentOutput = CreateAlignment(a.mzInto, b.mzInto)

            align.query = GetMeta(a)
            align.reference = GetMeta(b)

            Return align
        End Function

        Private Shared Function GetMeta(peak As PeakMs2) As Meta
            Return New Meta With {
                .id = peak.lib_guid,
                .mz = peak.mz,
                .scan_time = peak.rt,
                .intensity = peak.intensity
            }
        End Function

        Public Function CreateAlignment(a As ms2(), b As ms2()) As AlignmentOutput
            Dim align As SSM2MatrixFragment() = GlobalAlignment _
                .CreateAlignment(a, b, mzwidth) _
                .ToArray
            Dim scores As (forward#, reverse#) = GetScore(align)

            Return New AlignmentOutput With {
                .alignments = align,
                .forward = scores.forward,
                .reverse = scores.reverse
            }
        End Function

    End Class
End Namespace
