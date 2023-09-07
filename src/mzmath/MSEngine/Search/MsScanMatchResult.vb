<Flags> Public Enum SourceType As Byte
    None = 0
    Unknown = 1 << 0
    FastaDB = 1 << 1
    MspDB = 1 << 2
    TextDB = 1 << 4
    GeneratedLipid = 1 << 5
    Manual = 1 << 6
    DataBases = FastaDB Or MspDB Or TextDB Or GeneratedLipid
End Enum
Public Enum DataBaseSource
    None
    Msp
    Lbm
    Text
    Fasta
    EieioLipid
    OadLipid
    EidLipid
End Enum

Public Class MsScanMatchResult
    ' basic annotated information

    Public Property Name As String

    Public Property InChIKey As String


    Public Property TotalScore As Single

    ' spectral similarity

    Public Property WeightedDotProduct As Single

    Public Property SimpleDotProduct As Single

    Public Property ReverseDotProduct As Single

    Public Property MatchedPeaksCount As Single

    Public Property MatchedPeaksPercentage As Single

    Public Property EssentialFragmentMatchedScore As Single

    Public Property AndromedaScore As Single

    Public Property PEPScore As Single

    ' others

    Public Property RtSimilarity As Single

    Public Property RiSimilarity As Single

    Public Property CcsSimilarity As Single

    Public Property IsotopeSimilarity As Single

    Public Property AcurateMassSimilarity As Single

    ' Link to database

    Public Property LibraryID As Integer = -1

    Public Property LibraryIDWhenOrdered As Integer = -1

    ' Checker

    Public Property IsPrecursorMzMatch As Boolean

    Public Property IsSpectrumMatch As Boolean

    Public Property IsRtMatch As Boolean

    Public Property IsRiMatch As Boolean

    Public Property IsCcsMatch As Boolean

    Public Property IsLipidClassMatch As Boolean

    Public Property IsLipidChainsMatch As Boolean

    Public Property IsLipidPositionMatch As Boolean

    Public Property IsLipidDoubleBondPositionMatch As Boolean

    Public Property IsOtherLipidMatch As Boolean

    Public ReadOnly Property IsUnknown As Boolean
        Get
            Return Source.HasFlag(SourceType.Unknown)
        End Get
    End Property

    Public ReadOnly Property AnyMatched As Boolean
        Get
            Return (Source And SourceType.DataBases) <> SourceType.None
        End Get
    End Property

    ' Support for multiple annotation method

    Public ReadOnly Property IsManuallyModified As Boolean
        Get
            Return (Source And SourceType.Manual) <> 0
        End Get
    End Property

    Public Property Source As SourceType

    Public Property AnnotatorID As String

    Public Property SpectrumID As Integer = -1

    Public Property IsDecoy As Boolean = False

    Public Property Priority As Integer = -1

    Public Property IsReferenceMatched As Boolean = False

    Public Property IsAnnotationSuggested As Boolean = False

    Public Function Clone() As MsScanMatchResult
        Return CType(MemberwiseClone(), MsScanMatchResult)
    End Function
End Class
