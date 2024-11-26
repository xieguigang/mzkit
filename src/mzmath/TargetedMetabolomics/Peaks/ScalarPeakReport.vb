Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Class ScalarPeakReport

    Public Property Compound As String
    Public Property RT As Double
    Public Property Type As String
    Public Property Filename As String
    <Column("m/z (Expected)")> Public Property mz_Expected As Double
    <Column("%Diff")> Public Property Diff As Double
    <Column("Calculated Amt")> Public Property CalculatedAmt As Double
    Public Property Status As String
    <Column("Sample Type")> Public Property SampleType As String
    Public Property Level As String
    Public Property Height As Double
    Public Property Area As Double
    <Column("Response Ratio")> Public Property ResponseRatio As Double
    <Column("ISTD Response")> Public Property ISTDResponse As Double
    <Column("Actual RT")> Public Property ActualRT As Double
    <Column("%RSD")> Public Property RSD As Double
    <Column("% CV")> Public Property CV As Double
    <Column("S/N")> Public Property SN As Double
    <Column("Theoretical Amt")> Public Property TheoreticalAmt As Double
    <Column("Batch Order")> Public Property BatchOrder As Integer
    <Column("Sample Amt")> Public Property SampleAmt As Double
    <Column("Peak Label")> Public Property PeakLabel As String
    Public Property Active As Boolean
    Public Property Excluded As Boolean
    Public Property Confirm As String
    <Column("ISTD Amt")> Public Property ISTDAmt As Double
    Public Property Group As String
    <Column("Sample ID")> Public Property SampleID As String
    <Column("Sample Name")> Public Property SampleName As String
    Public Property Comments As String
    <Column("RT Delta")> Public Property RTDelta As Double
    Public Property Formula As String
    Public Property Adduct As String
    <Column("Std Add Amount")> Public Property StdAddAmount As Double
    <Column("m/z (Apex)")> Public Property mz_Apex As Double
    <Column("m/z (Delta)")> Public Property mz_Delta As String
    <Column("Integration Mode")> Public Property IntegrationMode As String
    Public Property Channel As String
    <Column("Final Units")> Public Property FinalUnits As String
    Public Property PK As String
    Public Property IR As String
    Public Property IP As String
    Public Property LS As String
    Public Property FI As String
    <Column("Isotopic Pattern Score (%)")> Public Property IsotopicPatternScore As String
    <Column("Num Isotopes Matched")> Public Property NumIsotopesMatched As String
    <Column("Lib Match Name")> Public Property LibMatchName As String
    <Column("Library Score (%)")> Public Property LibraryScore As String
    <Column("Library Match Rank")> Public Property LibraryMatchRank As String
    <Column("SI/Dot Product")> Public Property SIDotProduct As String
    <Column("RSI/Rev Dot")> Public Property RSIRevDot As String
    <Column("Library Probability Percent")> Public Property LibraryProbabilityPercent As String
    Public Property ColumnHeader1 As String
    Public Property ColumnHeader2 As String
    Public Property ColumnHeader3 As String
    Public Property ColumnHeader4 As String
    Public Property ColumnHeader5 As String
    Public Property ColumnHeader6 As String
    Public Property Modification As String
    <Column("Target Ion Ratio")> Public Property TargetIonRatio As String
    <Column("Protein Name")> Public Property ProteinName As String
    <Column("Ion Ratio")> Public Property IonRatio As String
    <Column("Peptide Sequence")> Public Property PeptideSequence As String
    <Column("Group Average")> Public Property GroupAverage As String
    <Column("Ion Type")> Public Property IonType As String
    <Column("Relative RT")> Public Property RelativeRT As String
    <Column("Flag Details")> Public Property FlagDetails As String

    Public Overrides Function ToString() As String
        Return $"{Compound} ({Area})"
    End Function

    ''' <summary>
    ''' convert current ion to a simplify single ion table
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetPeakData() As IonPeakTableRow
        Return New IonPeakTableRow With {
            .ID = Compound,
            .raw = Filename,
            .maxinto = Height,
            .Name = Compound,
            .TPA = Area,
            .rtmin = ActualRT - RTDelta,
            .rtmax = ActualRT + RTDelta,
            .base = Area.GetNoise(SN)
        }
    End Function

    ''' <summary>
    ''' extract and populate the peak area data
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="indexBySampleID">
    ''' make index by the sample id or the sample file name?
    ''' </param>
    ''' <returns></returns>
    Public Shared Iterator Function ExtractSampleData(table As IEnumerable(Of ScalarPeakReport), Optional indexBySampleID As Boolean = False) As IEnumerable(Of DataSet)
        Dim samples As IGrouping(Of String, ScalarPeakReport)() = table _
            .GroupBy(Function(a)
                         Return If(indexBySampleID, If(a.SampleID, ""), If(a.Filename, ""))
                     End Function) _
            .ToArray

        If samples.Length = 1 AndAlso samples(Scan0).Key = "" Then
            Call $"The sample data is required of index by {If(indexBySampleID, "sample id", "rawdata file name")}, but all these key value for make index is missing!".Warning
        End If

        For Each sample As IGrouping(Of String, ScalarPeakReport) In samples
            Dim peaks As IonPeakTableRow() = sample _
                .Select(Function(a) a.GetPeakData) _
                .ToArray


        Next
    End Function

End Class
