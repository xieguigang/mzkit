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
    ''' MS-DIAL model
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

        Public Property SpectrumComment As SpectrumComment

        Public Property IsAbsolutelyRequiredFragmentForAnnotation As Boolean

        Public Sub New()
        End Sub
        Public Sub New(mass As Double, intensity As Double, Optional comment As String = Nothing, Optional spectrumcomment As SpectrumComment = SpectrumComment.none, Optional isMust As Boolean = False)
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