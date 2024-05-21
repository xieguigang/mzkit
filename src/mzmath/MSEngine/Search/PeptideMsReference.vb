#Region "Microsoft.VisualBasic::b71d8d3a768713344367b17dd1ef5591, mzmath\MSEngine\Search\PeptideMsReference.vb"

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
    '    Code Lines: 200
    ' Comment Lines: 1
    '   Blank Lines: 76
    '     File Size: 10.17 KB


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
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.SequenceModel.Polypeptides
Imports std = System.Math

Public Class AminoAcid

    Public Property OneLetter As Char

    Public Property ThreeLetters As String

    Public Property Formula As Formula ' original formula information


    Public Property ModifiedCode As String

    Public Property ModifiedFormula As Formula

    Public Property ModifiedComposition As Formula

    Public Property Modifications As List(Of Modification)

    Public Function IsModified() As Boolean
        Return Not ModifiedCode.StringEmpty
    End Function

    Public Function ExactMass() As Double
        If IsModified() Then Return ModifiedFormula.ExactMass
        Return Formula.ExactMass
    End Function

    Public Function Code() As String
        If IsModified() Then Return ModifiedCode
        Return OneLetter.ToString()
    End Function

    Public Function GetFormula() As Formula
        If IsModified() Then Return ModifiedFormula
        Return Formula
    End Function

    Public Sub New()

    End Sub

    Public Sub New(oneletter As Char)
        Dim char2formula = OneChar2FormulaString
        Dim char2string = OneChar2ThreeLetter
        Me.OneLetter = oneletter
        ThreeLetters = char2string(oneletter)
        Formula = FormulaScanner.Convert2FormulaObjV2(char2formula(oneletter))
    End Sub

    Public Sub New(oneletter As Char, code As String, formula As Formula)
        Me.OneLetter = oneletter
        ThreeLetters = code
        Me.Formula = formula
    End Sub

    Public Sub New(aa As AminoAcid, modifiedCode As String, modifiedComposition As Formula)
        OneLetter = aa.OneLetter
        ThreeLetters = aa.ThreeLetters
        Formula = aa.Formula

        If modifiedCode.StringEmpty Then Return

        Me.ModifiedCode = modifiedCode
        Me.ModifiedComposition = modifiedComposition
        ModifiedFormula = modifiedComposition + aa.Formula
    End Sub

    Public Sub New(aa As AminoAcid, modifiedCode As String, modifiedComposition As Formula, modifications As List(Of Modification))
        OneLetter = aa.OneLetter
        ThreeLetters = aa.ThreeLetters
        Formula = aa.Formula

        If modifiedCode.StringEmpty Then Return

        Me.ModifiedCode = modifiedCode
        Me.ModifiedComposition = modifiedComposition
        ModifiedFormula = modifiedComposition + aa.Formula
        Me.Modifications = modifications
    End Sub

    Public Shared ReadOnly OneChar2Formula As Dictionary(Of Char, Formula) = New Dictionary(Of Char, Formula) From {
        {"A"c, FormulaScanner.Convert2FormulaObjV2("C3H7O2N")},
        {"R"c, FormulaScanner.Convert2FormulaObjV2("C6H14O2N4")},
        {"N"c, FormulaScanner.Convert2FormulaObjV2("C4H8O3N2")},
        {"D"c, FormulaScanner.Convert2FormulaObjV2("C4H7O4N")},
        {"C"c, FormulaScanner.Convert2FormulaObjV2("C3H7O2NS")},
        {"E"c, FormulaScanner.Convert2FormulaObjV2("C5H9O4N")},
        {"Q"c, FormulaScanner.Convert2FormulaObjV2("C5H10O3N2")},
        {"G"c, FormulaScanner.Convert2FormulaObjV2("C2H5O2N")},
        {"H"c, FormulaScanner.Convert2FormulaObjV2("C6H9O2N3")},
        {"I"c, FormulaScanner.Convert2FormulaObjV2("C6H13O2N")},
        {"L"c, FormulaScanner.Convert2FormulaObjV2("C6H13O2N")},
        {"J"c, FormulaScanner.Convert2FormulaObjV2("C6H13O2N")},
        {"K"c, FormulaScanner.Convert2FormulaObjV2("C6H14O2N2")},
        {"M"c, FormulaScanner.Convert2FormulaObjV2("C5H11O2NS")},
        {"F"c, FormulaScanner.Convert2FormulaObjV2("C9H11O2N")},
        {"P"c, FormulaScanner.Convert2FormulaObjV2("C5H9O2N")},
        {"S"c, FormulaScanner.Convert2FormulaObjV2("C3H7O3N")},
        {"T"c, FormulaScanner.Convert2FormulaObjV2("C4H9O3N")},
        {"W"c, FormulaScanner.Convert2FormulaObjV2("C11H12O2N2")},
        {"Y"c, FormulaScanner.Convert2FormulaObjV2("C9H11O3N")},
        {"V"c, FormulaScanner.Convert2FormulaObjV2("C5H11O2N")},
        {"O"c, FormulaScanner.Convert2FormulaObjV2("C12H21N3O3")},
        {"U"c, FormulaScanner.Convert2FormulaObjV2("C3H7NO2Se")}
    }
End Class


Public Class Peptide

    Public Property DatabaseOrigin As String

    Public Property DatabaseOriginID As Integer

    Public ReadOnly Property Sequence As String

        Get
            If cacheSequence Is Nothing Then
                cacheSequence = If(SequenceObj.IsNullOrEmpty, String.Empty, String.Join("", SequenceObj.[Select](Function(n) n.OneLetter.ToString())))
            End If
            Return cacheSequence
        End Get
    End Property ' original amino acid sequence

    Private cacheSequence As String = Nothing


    Public ReadOnly Property ModifiedSequence As String
        Get
            Return If(SequenceObj.IsNullOrEmpty, String.Empty, String.Join("", SequenceObj.[Select](Function(n) n.Code())))
        End Get
    End Property

    Public Property Position As intRange

    Public Property ExactMass As Double

    Public ReadOnly Property Formula As Formula
        Get
            Return If(SequenceObj.IsNullOrEmpty, Nothing, PeptideCalc.CalculatePeptideFormula(SequenceObj))
        End Get
    End Property


    Public Property IsProteinNterminal As Boolean

    Public Property IsProteinCterminal As Boolean

    Public Property SequenceObj As List(Of AminoAcid) ' N -> C, including modified amino acid information


    Public Property IsDecoy As Boolean = False

    Public Property MissedCleavages As Integer = 0

    Public Property SamePeptideNumberInSearchedProteins As Integer = 0

    Public Property ResidueCodeIndexToModificationIndex As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()

    Public Function CountModifiedAminoAcids() As Integer
        If SequenceObj Is Nothing Then Return 0
        Return SequenceObj.Where(Function(n) n.IsModified()).Count
    End Function

    Public Sub GenerateSequenceObj(proteinSeq As String, start As Integer, [end] As Integer, ResidueCodeIndexToModificationIndex As Dictionary(Of Integer, Integer), ID2Code As Dictionary(Of Integer, String), Code2AminoAcidObj As Dictionary(Of String, AminoAcid))
        SequenceObj = GetSequenceObj(proteinSeq, start, [end], ResidueCodeIndexToModificationIndex, ID2Code, Code2AminoAcidObj)
    End Sub

    Private Function GetSequenceObj(proteinSeq As String, start As Integer, [end] As Integer, ResidueCodeIndexToModificationIndex As Dictionary(Of Integer, Integer), iD2Code As Dictionary(Of Integer, String), code2AminoAcidObj As Dictionary(Of String, AminoAcid)) As List(Of AminoAcid)
        Dim sequence = New List(Of AminoAcid)()
        If std.Max(start, [end]) > proteinSeq.Length - 1 Then Return Nothing
        For i = start To [end]
            Dim oneleter = proteinSeq(i)
            If ResidueCodeIndexToModificationIndex.ContainsKey(i) Then
                Dim residueID = ResidueCodeIndexToModificationIndex(i)
                Dim residueCode = iD2Code(residueID)
                Dim aa = code2AminoAcidObj(residueCode)
                sequence.Add(aa)
            Else
                Dim residueCode = oneleter
                Dim aa = code2AminoAcidObj(residueCode.ToString())
                sequence.Add(aa)
            End If
        Next
        Return sequence
    End Function
End Class


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
