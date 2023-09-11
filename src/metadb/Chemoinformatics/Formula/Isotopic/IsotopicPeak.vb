
Namespace Formula.IsotopicPatterns

    ''' <summary>
    ''' 'IsotopicPeak.cs' and 'Isotope.cs' are the storage of isotope calculation result for a fomula.
    ''' The detail of the properties such as M+1, M+2, etc is stored in this class.
    ''' Relative intensity is standardized to 100 as the maximum
    ''' </summary>
    Public Class IsotopicPeak

        Public Sub New()

        End Sub

        Public Sub New(source As IsotopicPeak)
            RelativeAbundance = source.RelativeAbundance
            AbsoluteAbundance = source.AbsoluteAbundance
            Mass = source.Mass
            MassDifferenceFromMonoisotopicIon = source.MassDifferenceFromMonoisotopicIon
            Comment = source.Comment
        End Sub


        Public Property RelativeAbundance As Double

        Public Property AbsoluteAbundance As Double

        Public Property Mass As Double

        Public Property MassDifferenceFromMonoisotopicIon As Double

        Public Property Comment As String = String.Empty
    End Class
End Namespace