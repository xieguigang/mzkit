#Region "Microsoft.VisualBasic::afa8d69266a4b7138399668236f81d3c, mzmath\MSEngine\Search\MsScanMatchResult.vb"

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

    '   Total Lines: 127
    '    Code Lines: 75 (59.06%)
    ' Comment Lines: 6 (4.72%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 46 (36.22%)
    '     File Size: 3.00 KB


    ' Enum SourceType
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum DataBaseSource
    ' 
    '     EidLipid, EieioLipid, Fasta, Lbm, Msp
    '     None, OadLipid, Text
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class MsScanMatchResult
    ' 
    '     Properties: AcurateMassSimilarity, AndromedaScore, AnnotatorID, AnyMatched, CcsSimilarity
    '                 EssentialFragmentMatchedScore, InChIKey, IsAnnotationSuggested, IsCcsMatch, IsDecoy
    '                 IsLipidChainsMatch, IsLipidClassMatch, IsLipidDoubleBondPositionMatch, IsLipidPositionMatch, IsManuallyModified
    '                 IsOtherLipidMatch, IsotopeSimilarity, IsPrecursorMzMatch, IsReferenceMatched, IsRiMatch
    '                 IsRtMatch, IsSpectrumMatch, IsUnknown, LibraryID, LibraryIDWhenOrdered
    '                 MatchedPeaksCount, MatchedPeaksPercentage, Name, PEPScore, Priority
    '                 ReverseDotProduct, RiSimilarity, RtSimilarity, SimpleDotProduct, Source
    '                 SpectrumID, TotalScore, WeightedDotProduct
    ' 
    '     Function: Clone
    ' 
    ' /********************************************************************************/

#End Region

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
