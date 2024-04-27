#Region "Microsoft.VisualBasic::7a03ed646b45d29a0b8b54bc1de74f60, G:/mzkit/src/mzmath/MSFinder//AnalysisParamOfMsfinder.vb"

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

    '   Total Lines: 301
    '    Code Lines: 238
    ' Comment Lines: 23
    '   Blank Lines: 40
    '     File Size: 11.56 KB


    ' Enum FseaNonsignificantDef
    ' 
    '     LowAbundantIons, OntologySpace, ReverseSpectrum
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class AnalysisParamOfMsfinder
    ' 
    '     Properties: BrLabelMass, CanExcuteMS1AdductSearch, CanExcuteMS2AdductSearch, CcsAdductInChIKeyDictionaryFilepath, CcsToleranceForSpectralSearching
    '                 CcsToleranceForStructureElucidation, CLabelMass, ClLabelMass, Coeff_RtPrediction, CoverRange
    '                 DatabaseQuery, FLabelMass, FormulaMaximumReportNumber, FormulaPredictionTimeOut, FormulaScoreCutOff
    '                 FseanonsignificantDef, FseaPvalueCutOff, FseaRelativeAbundanceCutOff, HLabelMass, ILabelMass
    '                 Intercept_RtPrediction, IsAllProcess, IsBrCheck, IsClCheck, IsElementProbabilityCheck
    '                 IsFcheck, IsFormulaFinder, IsIcheck, IsLewisAndSeniorCheck, IsMinesAllTime
    '                 IsMinesNeverUse, IsMinesOnlyUseForNecessary, IsMmnFormulaBioreaction, IsMmnLocalCytoscape, IsMmnMsdialOutput
    '                 IsMmnOntologySimilarityUsed, IsMmnRetentionRestrictionUsed, IsMmnSelectedFileCentricProcess, IsNcheck, IsNeutralLossCheck
    '                 IsNitrogenRule, IsOcheck, IsotopicAbundanceTolerance, IsPcheck, IsPrecursorOrientedSearch
    '                 IsPubChemAllTime, IsPubChemNeverUse, IsPubChemOnlyUseForNecessary, IsRunInSilicoFragmenterSearch, IsRunSpectralDbSearch
    '                 IsScheck, IsSiCheck, IsStructureFinder, IsTmsMeoxDerivative, IsUseCcsForFilteringCandidates
    '                 IsUseCcsInchikeyAdductLibrary, IsUseEiFragmentDB, IsUseExperimentalCcsForSpectralSearching, IsUseExperimentalRtForSpectralSearching, IsUseInSilicoSpectralDbForLipids
    '                 IsUseInternalExperimentalSpectralDb, IsUsePredictedCcsForStructureElucidation, IsUsePredictedRtForStructureElucidation, IsUserDefinedDB, IsUseRtForFilteringCandidates
    '                 IsUseRtInchikeyLibrary, IsUseUserDefinedSpectralDb, IsUseXlogpPrediction, Mass1Tolerance, Mass2Tolerance
    '                 MassRangeMax, MassRangeMin, MassTolType, MinimumMeoxCount, MinimumTmsCount
    '                 MmnMassSimilarityCutOff, MmnMassTolerance, MmnOntologySimilarityCutOff, MmnOutputFolderPath, MmnRelativeCutoff
    '                 MmnRtTolerance, MmnRtToleranceForReaction, MS1NegativeAdductIonList, MS1PositiveAdductIonList, MS2NegativeAdductIonList
    '                 MS2PositiveAdductIonList, NLabelMass, OLabelMass, PLabelMass, RelativeAbundanceCutOff
    '                 RtInChIKeyDictionaryFilepath, RtPredictionSummaryReport, RtSmilesDictionaryFilepath, RtToleranceForSpectralSearching, RtToleranceForStructureElucidation
    '                 ScoreCutOffForSpectralMatch, SiLabelMass, SLabelMass, StructureMaximumReportNumber, StructurePredictionTimeOut
    '                 StructureScoreCutOff, TreeDepth, TryTopNmolecularFormulaSearch, UserDefinedDbFilePath, UserDefinedSpectralDbFilePath
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region


Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS

Public Enum FseaNonsignificantDef
    OntologySpace
    ReverseSpectrum
    LowAbundantIons
End Enum
''' <summary>
''' This is the storage of analysis parameters used in MS-FINDER program.
''' </summary>
Public Class AnalysisParamOfMsfinder
    Public Sub New()
        IsLewisAndSeniorCheck = True
        MassTolType = MassToleranceType.Da
        Mass1Tolerance = 0.001
        Mass2Tolerance = 0.01
        IsotopicAbundanceTolerance = 20
        CoverRange = CoverRange.CommonRange
        IsElementProbabilityCheck = True
        IsOcheck = True
        IsNcheck = True
        IsPcheck = True
        IsScheck = True
        IsFcheck = False
        IsClCheck = False
        IsBrCheck = False
        IsIcheck = False
        IsSiCheck = False
        IsNitrogenRule = True

        CanExcuteMS2AdductSearch = False
        FormulaScoreCutOff = 70
        FormulaMaximumReportNumber = 100

        CcsToleranceForStructureElucidation = 10.0
        IsUsePredictedCcsForStructureElucidation = False
        IsUseCcsInchikeyAdductLibrary = False
        CcsAdductInChIKeyDictionaryFilepath = String.Empty
        IsUseExperimentalCcsForSpectralSearching = True
        CcsToleranceForSpectralSearching = 10.0
        IsUseCcsForFilteringCandidates = True

        IsNeutralLossCheck = True

        TreeDepth = 2
        RelativeAbundanceCutOff = 1

        StructureScoreCutOff = 60
        StructureMaximumReportNumber = 100

        IsAllProcess = False
        IsFormulaFinder = True
        IsStructureFinder = False
        TryTopNmolecularFormulaSearch = 5

        DatabaseQuery = New DatabaseQuery() With {
.Chebi = True,
.Ymdb = True,
.Unpd = True,
.Smpdb = True,
.Pubchem = True,
.Hmdb = True,
.Plantcyc = True,
.Knapsack = True,
.Bmdb = True,
.Drugbank = True,
.Ecmdb = True,
.Foodb = True,
.T3db = True,
.Stoff = True,
.Nanpdb = True,
.Blexp = True,
.Npa = True,
.Coconut = True
}

        IsUserDefinedDB = False
        UserDefinedDbFilePath = String.Empty

        IsPubChemAllTime = False
        IsPubChemNeverUse = True
        IsPubChemOnlyUseForNecessary = False

        IsMinesOnlyUseForNecessary = True
        IsMinesNeverUse = False
        IsMinesAllTime = False
        IsUseEiFragmentDB = False

        CLabelMass = 0
        HLabelMass = 0
        NLabelMass = 0
        PLabelMass = 0
        OLabelMass = 0
        SLabelMass = 0
        FLabelMass = 0
        ClLabelMass = 0
        BrLabelMass = 0
        ILabelMass = 0
        SiLabelMass = 0

        IsTmsMeoxDerivative = False
        MinimumMeoxCount = 0
        MinimumTmsCount = 1

        IsRunSpectralDbSearch = False
        IsRunInSilicoFragmenterSearch = True
        IsPrecursorOrientedSearch = True

        IsUseInternalExperimentalSpectralDb = True
        IsUseInSilicoSpectralDbForLipids = False
        IsUseUserDefinedSpectralDb = False

        MassRangeMax = 2000
        MassRangeMin = 0
        ' RetentionType = RetentionType.RT

        IsUsePredictedRtForStructureElucidation = False
        IsUseRtInchikeyLibrary = True
        IsUseXlogpPrediction = False
        RtInChIKeyDictionaryFilepath = String.Empty
        RtSmilesDictionaryFilepath = String.Empty
        Coeff_RtPrediction = -1
        Intercept_RtPrediction = -1
        RtToleranceForStructureElucidation = 2.5 'min
        RtPredictionSummaryReport = String.Empty
        IsUseRtForFilteringCandidates = False

        IsUseExperimentalRtForSpectralSearching = False
        RtToleranceForSpectralSearching = 0.5 'min

        ' fsea parameter
        FseaRelativeAbundanceCutOff = 5.0 ' %
        FseanonsignificantDef = FseaNonsignificantDef.OntologySpace
        FseaPvalueCutOff = 1.0 ' %

        ' msfinder molecular networking
        IsMmnLocalCytoscape = True
        IsMmnMsdialOutput = False
        IsMmnFormulaBioreaction = False
        IsMmnRetentionRestrictionUsed = False
        IsMmnOntologySimilarityUsed = True
        IsMmnSelectedFileCentricProcess = True

        MmnMassTolerance = 0.025
        MmnRelativeCutoff = 1.0
        MmnMassSimilarityCutOff = 75.0
        MmnRtTolerance = 100
        MmnRtToleranceForReaction = 0.5
        MmnOntologySimilarityCutOff = 90.0
        MmnOutputFolderPath = String.Empty

        FormulaPredictionTimeOut = -1.0 ' means no timeout if number is existed, the unit is [min]
        StructurePredictionTimeOut = -1.0 ' means no timeout 

        StructureScoreCutOff = 0
        ScoreCutOffForSpectralMatch = 80
    End Sub

#Region "Properties"
    ' basic
    Public Property MassTolType As MassToleranceType
    Public Property Mass1Tolerance As Double
    Public Property Mass2Tolerance As Double
    Public Property IsotopicAbundanceTolerance As Double
    Public Property MassRangeMin As Double
    Public Property MassRangeMax As Double
    Public Property RelativeAbundanceCutOff As Double
    ' Public Property SolventType As SolventType
    ' Public Property RetentionType As RetentionType

    ' formula generator
    Public Property CoverRange As CoverRange
    Public Property IsLewisAndSeniorCheck As Boolean
    Public Property IsElementProbabilityCheck As Boolean
    Public Property IsOcheck As Boolean
    Public Property IsNcheck As Boolean
    Public Property IsPcheck As Boolean
    Public Property IsScheck As Boolean
    Public Property IsFcheck As Boolean
    Public Property IsClCheck As Boolean
    Public Property IsBrCheck As Boolean
    Public Property IsIcheck As Boolean
    Public Property IsSiCheck As Boolean
    Public Property IsNitrogenRule As Boolean
    Public Property FormulaScoreCutOff As Double
    Public Property CanExcuteMS1AdductSearch As Boolean = False
    Public Property CanExcuteMS2AdductSearch As Boolean
    Public Property FormulaMaximumReportNumber As Integer
    Public Property IsNeutralLossCheck As Boolean

    ' structure finder
    Public Property TreeDepth As Integer
    Public Property StructureScoreCutOff As Double
    Public Property StructureMaximumReportNumber As Integer
    Public Property IsUserDefinedDB As Boolean
    Public Property UserDefinedDbFilePath As String
    Public Property IsAllProcess As Boolean
    Public Property IsUseEiFragmentDB As Boolean
    Public Property TryTopNmolecularFormulaSearch As Integer

    'batch job
    Public Property IsFormulaFinder As Boolean
    Public Property IsStructureFinder As Boolean
    Public Property DatabaseQuery As DatabaseQuery
    Public Property IsPubChemNeverUse As Boolean
    Public Property IsPubChemOnlyUseForNecessary As Boolean
    Public Property IsPubChemAllTime As Boolean
    Public Property IsMinesNeverUse As Boolean
    Public Property IsMinesOnlyUseForNecessary As Boolean
    Public Property IsMinesAllTime As Boolean


    'labeled compound info
    Public Property CLabelMass As Double
    Public Property HLabelMass As Double
    Public Property NLabelMass As Double
    Public Property OLabelMass As Double
    Public Property PLabelMass As Double
    Public Property SLabelMass As Double
    Public Property FLabelMass As Double
    Public Property ClLabelMass As Double
    Public Property BrLabelMass As Double
    Public Property ILabelMass As Double
    Public Property SiLabelMass As Double

    'TMS-derivative compound
    Public Property IsTmsMeoxDerivative As Boolean
    Public Property MinimumTmsCount As Integer
    Public Property MinimumMeoxCount As Integer

    'Spectral database search
    Public Property IsRunSpectralDbSearch As Boolean
    Public Property IsRunInSilicoFragmenterSearch As Boolean
    Public Property IsPrecursorOrientedSearch As Boolean
    Public Property IsUseInternalExperimentalSpectralDb As Boolean
    Public Property IsUseInSilicoSpectralDbForLipids As Boolean
    Public Property IsUseUserDefinedSpectralDb As Boolean
    Public Property UserDefinedSpectralDbFilePath As String
    ' Public Property LipidQueryBean As LipidQueryBean
    Public Property ScoreCutOffForSpectralMatch As Double

    'retention time setting for structure elucidation
    Public Property IsUsePredictedRtForStructureElucidation As Boolean
    Public Property RtSmilesDictionaryFilepath As String
    Public Property Coeff_RtPrediction As Double
    Public Property Intercept_RtPrediction As Double
    Public Property RtToleranceForStructureElucidation As Double
    Public Property IsUseRtInchikeyLibrary As Boolean
    Public Property IsUseXlogpPrediction As Boolean
    Public Property RtInChIKeyDictionaryFilepath As String
    Public Property IsUseRtForFilteringCandidates As Boolean

    'retention time setting for spectral searching
    Public Property IsUseExperimentalRtForSpectralSearching As Boolean
    Public Property RtToleranceForSpectralSearching As Double
    Public Property RtPredictionSummaryReport As String

    ' FSEA parameter
    Public Property FseaRelativeAbundanceCutOff As Double
    Public Property FseanonsignificantDef As FseaNonsignificantDef
    Public Property FseaPvalueCutOff As Double

    'msfinder molecular networking
    Public Property IsMmnLocalCytoscape As Boolean ' The node and edge files are created if this is true. else, the json file including node and edge is created for cytoscape.js
    Public Property IsMmnMsdialOutput As Boolean ' the IDs in Comment field are used when this property is true.  else, the title field is used for the ID.
    Public Property IsMmnFormulaBioreaction As Boolean
    Public Property IsMmnRetentionRestrictionUsed As Boolean
    Public Property IsMmnOntologySimilarityUsed As Boolean
    Public Property MmnMassTolerance As Double ' general parameters
    Public Property MmnRelativeCutoff As Double ' general parameters
    Public Property MmnMassSimilarityCutOff As Double ' MS/MS network cut off %
    Public Property MmnRtTolerance As Double
    Public Property MmnOntologySimilarityCutOff As Double ' ontology network cut off %
    Public Property MmnOutputFolderPath As String
    Public Property MmnRtToleranceForReaction As Double ' formula bioreaction parameter
    Public Property IsMmnSelectedFileCentricProcess As Boolean

    'timeout
    Public Property FormulaPredictionTimeOut As Double
    Public Property StructurePredictionTimeOut As Double

    ' pos/neg adduct ion list
    Public Property MS1PositiveAdductIonList As List(Of AdductIon) = New List(Of AdductIon)()
    Public Property MS2PositiveAdductIonList As List(Of AdductIon) = New List(Of AdductIon)()
    Public Property MS1NegativeAdductIonList As List(Of AdductIon) = New List(Of AdductIon)()
    Public Property MS2NegativeAdductIonList As List(Of AdductIon) = New List(Of AdductIon)()

    'collision cross section
    Public Property CcsToleranceForStructureElucidation As Double
    Public Property IsUsePredictedCcsForStructureElucidation As Boolean
    Public Property IsUseCcsInchikeyAdductLibrary As Boolean
    Public Property CcsAdductInChIKeyDictionaryFilepath As String
    Public Property IsUseExperimentalCcsForSpectralSearching As Boolean
    Public Property CcsToleranceForSpectralSearching As Double
    Public Property IsUseCcsForFilteringCandidates As Boolean

#End Region
End Class
