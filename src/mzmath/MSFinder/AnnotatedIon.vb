
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
''' <summary>
''' This class is the storage of adduct or isotope ion assignment used in MS-FINDER program.
''' </summary>
Public Class AnnotatedIon
    Public Enum AnnotationType
        Precursor
        Product
        Isotope
        Adduct
    End Enum
    Public Property PeakType As AnnotationType
    Public Property AccurateMass As Double
    Public Property Intensity As Double
    Public Property LinkedAccurateMass As Double
    Public Property LinkedIntensity As Double
    Public Property AdductIon As AdductIon
    Public Property IsotopeWeightNumber As Integer
    ''' <summary> C-13, O-18, and something </summary>
    Public Property IsotopeName As String
    Public Sub New()
        PeakType = AnnotationType.Product
    End Sub

    Public Sub SetIsotopeC13(linkedMz As Double)
        LinkedAccurateMass = linkedMz
        PeakType = AnnotationType.Isotope
        IsotopeWeightNumber = 1
        IsotopeName = "C-13"
    End Sub

    Public Sub SetIsotope(linkedMz As Double, intensity As Double, linkedIntensity As Double, isotomeName As String, weightNumber As Integer)
        PeakType = AnnotationType.Isotope
        LinkedAccurateMass = linkedMz
        Me.Intensity = intensity
        Me.LinkedIntensity = linkedIntensity
        IsotopeWeightNumber = weightNumber
        IsotopeName = isotomeName
    End Sub

    Public Sub SetAdduct(linkedMz As Double, intensity As Double, linkedIntensity As Double, adduct As AdductIon)
        PeakType = AnnotationType.Adduct
        Me.Intensity = intensity
        Me.LinkedIntensity = linkedIntensity
        LinkedAccurateMass = linkedMz
        AdductIon = adduct
    End Sub
End Class