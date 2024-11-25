Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Public Class ScalarPeakReport

    Public Property Compound As String
    Public Property RT
    Public Property Type
    Public Property Filename
    <Column("m/z (Expected)")> Public Property mz_Expected
    <Column("%Diff")> Public Property Diff
    <Column("Calculated Amt")> Public Property CalculatedAmt
    Public Property Status
    <Column("Sample Type")> Public Property SampleType
    Public Property Level
    Public Property Height
    Public Property Area
    <Column("Response Ratio")> Public Property ResponseRatio
    <Column("ISTD Response")> Public Property ISTDResponse
    <Column("Actual RT")> Public Property ActualRT
    <Column("%RSD")> Public Property RSD
    <Column("% CV")> Public Property CV
    <Column("S/N")> Public Property SN
    <Column("Theoretical Amt")> Public Property TheoreticalAmt
    <Column("Batch Order")> Public Property BatchOrder
    <Column("Sample Amt")> Public Property SampleAmt
    <Column("Peak Label")> Public Property PeakLabel
    Public Property Active
    Public Property Excluded
    Public Property Confirm
    <Column("ISTD Amt")> Public Property ISTDAmt
    Public Property Group
    <Column("Sample ID")> Public Property SampleID
    <Column("Sample Name")> Public Property SampleName
    Public Property Comments
    <Column("RT Delta")> Public Property RTDelta
    Public Property Formula
    Public Property Adduct
    <Column("Std Add Amount")> Public Property StdAddAmount
    <Column("m/z (Apex)")> Public Property mz_Apex
    <Column("m/z (Delta)")> Public Property mz_Delta
    <Column("Integration Mode")> Public Property IntegrationMode
    Public Property Channel
    <Column("Final Units")> Public Property FinalUnits
    Public Property PK
    Public Property IR
    Public Property IP
    Public Property LS
    Public Property FI
    <Column("Isotopic Pattern Score (%)")> Public Property IsotopicPatternScore
    <Column("Num Isotopes Matched")> Public Property NumIsotopesMatched
    <Column("Lib Match Name")> Public Property LibMatchName
    <Column("Library Score (%)")> Public Property LibraryScore
    <Column("Library Match Rank")> Public Property LibraryMatchRank
    <Column("SI/Dot Product")> Public Property SIDotProduct
    <Column("RSI/Rev Dot")> Public Property RSIRevDot
    <Column("Library Probability Percent")> Public Property LibraryProbabilityPercent
    Public Property ColumnHeader1
    Public Property ColumnHeader2
    Public Property ColumnHeader3
    Public Property ColumnHeader4
    Public Property ColumnHeader5
    Public Property ColumnHeader6
    Public Property Modification
    <Column("Target Ion Ratio")> Public Property TargetIonRatio
    <Column("Protein Name")> Public Property ProteinName
    <Column("Ion Ratio")> Public Property IonRatio
    <Column("Peptide Sequence")> Public Property PeptideSequence
    <Column("Group Average")> Public Property GroupAverage
    <Column("Ion Type")> Public Property IonType
    <Column("Relative RT")> Public Property RelativeRT
    <Column("Flag Details")> Public Property FlagDetails

End Class
