#Region "Microsoft.VisualBasic::71051a3406a7ec3115318cb24894336f, mzmath\ms2_math-core\Spectra\Alignment\AlignmentProvider.vb"

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

    '   Total Lines: 106
    '    Code Lines: 67 (63.21%)
    ' Comment Lines: 20 (18.87%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 19 (17.92%)
    '     File Size: 3.98 KB


    '     Class AlignmentProvider
    ' 
    '         Properties: Tolerance
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Cosine, (+3 Overloads) CreateAlignment, GetMeta, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace Spectra

    ''' <summary>
    ''' the spectrum similarity score provider
    ''' </summary>
    Public MustInherit Class AlignmentProvider

        Protected intocutoff As LowAbundanceTrimming
        Protected mzwidth As Tolerance

        Public ReadOnly Property Tolerance As Tolerance
            Get
                Return mzwidth
            End Get
        End Property

        Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming)
            Me.mzwidth = mzwidth
            Me.intocutoff = intocutoff
        End Sub

        ''' <summary>
        ''' default is get the min forward and reverse consine score
        ''' </summary>
        ''' <param name="mzwidth"></param>
        ''' <param name="intocutoff"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Cosine(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming) As CosAlignment
            Return New CosAlignment(mzwidth, intocutoff)
        End Function

        Public MustOverride Function GetScore(a As ms2(), b As ms2()) As Double

        ''' <summary>
        ''' evaluate the [forward,reverse] cosine score
        ''' </summary>
        ''' <param name="alignment"></param>
        ''' <returns></returns>
        Public MustOverride Function GetScore(alignment As SSM2MatrixFragment()) As (forward#, reverse#)

        Public Overrides Function ToString() As String
            Return $"{mzwidth} && {intocutoff}"
        End Function

        Public Overloads Function CreateAlignment(a As PeakMs2, b As PeakMs2) As AlignmentOutput
            Dim align As AlignmentOutput = CreateAlignment(a.mzInto, b.mzInto)

            align.query = GetMeta(a)
            align.reference = GetMeta(b)

            Return align
        End Function

        Public Overloads Function CreateAlignment(a As PeakMs2, b As LibraryMatrix) As AlignmentOutput
            Dim align As AlignmentOutput = CreateAlignment(a.mzInto, b.ms2)

            align.query = GetMeta(a)
            align.reference = New Meta With {
                .id = b.name,
                .intensity = b.intensity.Sum,
                .mz = -1,
                .scan_time = -1
            }

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

        ''' <summary>
        ''' Creates only the ms2 spectrum alignment result output, missing precursor ion and reference information metadata.
        ''' </summary>
        ''' <param name="a">the spectrum peaks of the query.</param>
        ''' <param name="b">the spectrum peaks of the reference.</param>
        ''' <returns></returns>
        Public Overloads Function CreateAlignment(a As ms2(), b As ms2()) As AlignmentOutput
            Dim align As SSM2MatrixFragment() = GlobalAlignment _
                .CreateAlignment(a, b, mzwidth) _
                .ToArray
            Dim scores As (forward#, reverse#) = GetScore(align)
            Dim jIdx As Double = GlobalAlignment.JaccardIndex(a, b, mzwidth)
            Dim entropy As Double = SpectralEntropy.calculate_entropy_similarity(align.StandardizeAlignment.ToArray)

            Return New AlignmentOutput With {
                .alignments = align,
                .forward = scores.forward,
                .reverse = scores.reverse,
                .jaccard = jIdx,
                .entropy = entropy
            }
        End Function

    End Class
End Namespace
