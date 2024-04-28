#Region "Microsoft.VisualBasic::b53a0170d63ac2984e5d15bdce3ddb4e, E:/mzkit/src/mzmath/ms2_math-core//Spectra/Models/SpectrumPeak.vb"

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

    '   Total Lines: 102
    '    Code Lines: 73
    ' Comment Lines: 10
    '   Blank Lines: 19
    '     File Size: 2.89 KB


    '     Enum SpectrumComment
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Interface ISpectrumPeak
    ' 
    '         Properties: Intensity, Mass
    ' 
    '     Enum PeakQuality
    ' 
    '         Ideal, Leading, Saturated, Tailing
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class SpectrumPeak
    ' 
    '         Properties: Charge, IsAbsolutelyRequiredFragmentForAnnotation, IsMatched, IsotopeFrag, IsotopeParentPeakID
    '                     IsotopeWeightNumber, PeakID, PeakQuality, Resolution, SpectrumComment
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: Clone
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Spectra

    <Flags>
    Public Enum SpectrumComment
        none = 0
        experiment = 1
        reference = 2
        precursor = 4
        b = 8
        y = &H10
        b2 = &H20
        y2 = &H40
        b_h2o = &H80
        y_h2o = &H100
        b_nh3 = &H200
        y_nh3 = &H400
        b_h3po4 = &H800
        y_h3po4 = &H1000
        tyrosinep = &H2000
        metaboliteclass = &H4000
        acylchain = &H8000
        doublebond = &H10000
        snposition = &H20000
        doublebond_high = &H40000
        doublebond_low = &H80000
        c = &H100000
        z = &H200000
        c2 = &H400000
        z2 = &H800000
    End Enum

    Public Interface ISpectrumPeak
        Property Mass As Double
        Property Intensity As Double
    End Interface

    Public Enum PeakQuality
        Ideal
        Saturated
        Leading
        Tailing
    End Enum

    ''' <summary>
    ''' A more details of the spectrum peak model than <see cref="ms2"/> object
    ''' </summary>
    ''' <remarks>
    ''' MS-DIAL model, use the property <see cref="SpectrumComment"/> for set the metabolite related information 
    ''' </remarks>
    Public Class SpectrumPeak : Inherits ms2
        Implements ISpectrumPeak

        Public Property Resolution As Double

        Public Property Charge As Integer

        Public Property IsotopeFrag As Boolean

        Public Property PeakQuality As PeakQuality

        Public Property PeakID As Integer

        Public Property IsotopeParentPeakID As Integer = -1

        Public Property IsotopeWeightNumber As Integer = -1

        Public Property IsMatched As Boolean = False

        Public Property SpectrumComment As SpectrumComment = SpectrumComment.none

        Public Property IsAbsolutelyRequiredFragmentForAnnotation As Boolean

        Public Sub New()
        End Sub

        ''' <summary>
        ''' do make data copy
        ''' </summary>
        ''' <param name="fragment"></param>
        Sub New(fragment As ms2)
            mz = fragment.mz
            intensity = fragment.intensity
            Annotation = fragment.Annotation
        End Sub

        Public Sub New(mass As Double, intensity As Double,
                       Optional comment As String = Nothing,
                       Optional spectrumcomment As SpectrumComment = SpectrumComment.none,
                       Optional isMust As Boolean = False)

            Me.mz = mass
            Me.intensity = intensity
            Me.Annotation = comment
            Me.SpectrumComment = spectrumcomment
            IsAbsolutelyRequiredFragmentForAnnotation = isMust
        End Sub

        Public Function Clone() As SpectrumPeak
            Return CType(MemberwiseClone(), SpectrumPeak)
        End Function
    End Class
End Namespace
