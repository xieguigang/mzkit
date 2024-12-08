#Region "Microsoft.VisualBasic::9f8e76cbcc623f7b82ecd4adbd61f4aa, mzmath\MSEngine\Search\PeptideMsReference.vb"

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

    '   Total Lines: 277
    '    Code Lines: 200 (72.20%)
    ' Comment Lines: 1 (0.36%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 76 (27.44%)
    '     File Size: 10.37 KB


    ' Class AminoAcid
    ' 
    '     Properties: Formula, Modifications, ModifiedCode, ModifiedComposition, ModifiedFormula
    '                 OneLetter, ThreeLetters
    ' 
    '     Constructor: (+5 Overloads) Sub New
    '     Function: Code, ExactMass, GetFormula, IsModified
    ' 
    ' Class Peptide
    ' 
    '     Properties: DatabaseOrigin, DatabaseOriginID, ExactMass, Formula, IsDecoy
    '                 IsProteinCterminal, IsProteinNterminal, MissedCleavages, ModifiedSequence, Position
    '                 ResidueCodeIndexToModificationIndex, SamePeptideNumberInSearchedProteins, Sequence, SequenceObj
    ' 
    '     Function: CountModifiedAminoAcids, GetSequenceObj
    ' 
    '     Sub: GenerateSequenceObj
    ' 
    ' Class PeptideMsReference
    ' 
    '     Properties: AdductType, ChromXs, CollisionCrossSection, CollisionType, Fs
    '                 IonMode, IsAnnotated, MaxMs2, MinMs2, Peptide
    '                 PrecursorMz, ScanID, SeekPoint2MS, Spectrum
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: ReadSpectrum
    ' 
    '     Sub: AddPeak, SetAdductType
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Class PeptideMsReference
    Implements IMSScanProperty, IIonProperty

    Public ReadOnly Property Peptide As Peptide

    Public Property Fs As Stream

    Public Property SeekPoint2MS As Long

    Public Property MinMs2 As Single

    Public Property MaxMs2 As Single

    Public Property CollisionType As ActivationMethods

    Public Sub New(peptide As Peptide)
        Me.Peptide = peptide
    End Sub

    Public Sub New(peptide As Peptide, fs As Stream, seekPoint As Long, adduct As AdductIon, id As Integer, minMs2 As Single, maxMs2 As Single, type As ActivationMethods)
        Me.Peptide = peptide
        Me.Fs = fs
        SeekPoint2MS = seekPoint
        AdductType = adduct
        PrecursorMz = adduct.ConvertToMz(peptide.ExactMass)
        ScanID = id
        Me.MinMs2 = minMs2
        Me.MaxMs2 = maxMs2
        CollisionType = type
    End Sub


    Public Property ScanID As Integer Implements IMSScanProperty.ScanID

    Public Property Spectrum As List(Of SpectrumPeak) Implements IMSScanProperty.Spectrum
        Get
            If cacheSpectrum Is Nothing Then
                'cacheSpectrum = ReadSpectrum(Fs, SeekPoint2MS);
                cacheSpectrum = ReadSpectrum()
                Return cacheSpectrum
            End If
            Return cacheSpectrum
        End Get
        Set(value As List(Of SpectrumPeak))
            Throw New NotSupportedException()
        End Set
    End Property


    Private cacheSpectrum As List(Of SpectrumPeak) = Nothing


    Private Function ReadSpectrum() As List(Of SpectrumPeak)
        Return SequenceToSpec.Convert2SpecPeaks(Peptide, AdductType, CollisionType, MinMs2, MaxMs2)
    End Function

    Public Sub AddPeak(mass As Double, intensity As Double, Optional comment As String = Nothing) Implements IMSScanProperty.AddPeak

    End Sub


    Public Property ChromXs As ChromXs = New ChromXs() Implements IMSProperty.ChromXs

    Public Property IonMode As IonModes = IonModes.Positive Implements IMSProperty.IonMode

    Public Property PrecursorMz As Double Implements IMSProperty.PrecursorMz


    Public Property AdductType As AdductIon Implements IIonProperty.AdductType

    Public Sub SetAdductType(adduct As AdductIon) Implements IIonProperty.SetAdductType
        AdductType = adduct
    End Sub

    Public Property CollisionCrossSection As Double Implements IIonProperty.CollisionCrossSection

    Public Property IsAnnotated As Boolean
End Class
