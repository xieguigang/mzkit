#Region "Microsoft.VisualBasic::8afabd3fabc1bb2b0bce7138b85687f5, G:/mzkit/src/mzmath/MSEngine//MoleculeMsReference.vb"

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

    '   Total Lines: 128
    '    Code Lines: 79
    ' Comment Lines: 5
    '   Blank Lines: 44
    '     File Size: 4.80 KB


    ' Interface IIonProperty
    ' 
    '     Properties: AdductType, CollisionCrossSection
    ' 
    '     Sub: SetAdductType
    ' 
    ' Interface IMSIonProperty
    ' 
    ' 
    ' 
    ' Interface IMoleculeMsProperty
    ' 
    ' 
    ' 
    ' Class MoleculeMsReference
    ' 
    '     Properties: AdductType, Charge, ChromXs, CollisionCrossSection, CollisionEnergy
    '                 Comment, CompoundClass, DatabaseID, DatabaseUniqueIdentifier, Formula
    '                 FragmentationCondition, InChIKey, InstrumentModel, InstrumentType, IonMode
    '                 IsotopicPeaks, IsTargetMolecule, Links, MassTolerance, MinimumPeakHeight
    '                 MsLevel, Name, Ontology, OntologyOrCompoundClass, PrecursorMz
    '                 QuantMass, RetentionTimeTolerance, ScanID, SMILES, Spectrum
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Sub: AddPeak, SetAdductType
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Interface IIonProperty
    ' Molecule ion metadata
    ReadOnly Property AdductType As AdductIon
    Sub SetAdductType(adduct As AdductIon)
    ReadOnly Property CollisionCrossSection As Double
End Interface

Public Interface IMSIonProperty
    Inherits IMSProperty, IIonProperty
End Interface

Public Interface IMoleculeMsProperty
    Inherits IMSScanProperty, IMoleculeProperty, IMSIonProperty ' especially used for library record
End Interface

Public Class MoleculeMsReference : Implements IMoleculeMsProperty

    Public Sub New()
        ChromXs = New ChromXs()
        Spectrum = New List(Of SpectrumPeak)()
        Formula = New Formula()
        AdductType = AdductIon.Default
    End Sub

    <Obsolete("This constructor is for MessagePack only, don't use.")>
    Public Sub New(scanID As Integer, precursorMz As Double, chromXs As ChromXs, ionMode As IonModes, spectrum As List(Of SpectrumPeak), name As String, formula As Formula, ontology As String, sMILES As String, inChIKey As String, adductType As AdductIon)
        Me.ScanID = scanID
        Me.PrecursorMz = precursorMz
        Me.ChromXs = chromXs
        Me.IonMode = ionMode
        Me.Spectrum = spectrum
        Me.Name = name
        Me.Formula = formula
        Me.Ontology = ontology
        Me.SMILES = sMILES
        Me.InChIKey = inChIKey
        Me.AdductType = adductType
    End Sub

    ' set for IMMScanProperty

    Public Property ScanID As Integer Implements IMSScanProperty.ScanID

    Public Property PrecursorMz As Double Implements IMSProperty.PrecursorMz

    Public Property ChromXs As ChromXs Implements IMSProperty.ChromXs

    Public Property IonMode As IonModes Implements IMSProperty.IonMode

    Public Property Spectrum As List(Of SpectrumPeak) Implements IMSScanProperty.Spectrum

    ' set for IMoleculeProperty

    Public Property Name As String = String.Empty Implements IMoleculeProperty.Name

    Public Property Formula As Formula Implements IMoleculeProperty.Formula

    Public Property Ontology As String = String.Empty Implements IMoleculeProperty.Ontology

    Public Property SMILES As String = String.Empty Implements IMoleculeProperty.SMILES

    Public Property InChIKey As String = String.Empty Implements IMoleculeProperty.InChIKey

    ' ion physiochemical information

    Public Property AdductType As AdductIon Implements IIonProperty.AdductType

    Public Sub SetAdductType(adduct As AdductIon) Implements IIonProperty.SetAdductType
        AdductType = adduct
    End Sub


    Public Property CollisionCrossSection As Double Implements IIonProperty.CollisionCrossSection

    Public Property IsotopicPeaks As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

    Public Property QuantMass As Double ' used for GCMS project

    ' other additional metadata

    Public Property CompoundClass As String ' lipidomics

    Public Property Comment As String = String.Empty

    Public Property InstrumentModel As String = String.Empty

    Public Property InstrumentType As String = String.Empty

    Public Property Links As String = String.Empty ' used to link molecule record to databases. Each database must be separated by semi-colon (;)

    Public Property CollisionEnergy As Single

    Public Property DatabaseID As Integer ' used for binbase, fastaDB etc

    Public Property DatabaseUniqueIdentifier As String ' used for binbase, fastaDB etc

    Public Property Charge As Integer

    Public Property MsLevel As Integer

    Public Property RetentionTimeTolerance As Single = 0.05F ' used for text library searching

    Public Property MassTolerance As Single = 0.05F ' used for text library searching

    Public Property MinimumPeakHeight As Single = 1000.0F ' used for text library searching

    Public Property IsTargetMolecule As Boolean = True ' used for text library searching

    Public Property FragmentationCondition As String

    Public Sub AddPeak(mass As Double, intensity As Double, Optional comment As String = Nothing) Implements IMSScanProperty.AddPeak
        Spectrum.Add(New SpectrumPeak(mass, intensity, comment))
    End Sub


    Public ReadOnly Property OntologyOrCompoundClass As String
        Get
            Return If(String.IsNullOrEmpty(Ontology), CompoundClass, Ontology)
        End Get
    End Property
End Class

