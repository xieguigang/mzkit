﻿#Region "Microsoft.VisualBasic::21513b6866837ae518cee7ac851af5f4, mzmath\TargetedMetabolomics\Peaks\ScalarPeakReport.vb"

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

    '   Total Lines: 178
    '    Code Lines: 142 (79.78%)
    ' Comment Lines: 20 (11.24%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 16 (8.99%)
    '     File Size: 8.35 KB


    ' Class ScalarPeakReport
    ' 
    '     Properties: Active, ActualRT, Adduct, Area, BatchOrder
    '                 CalculatedAmt, Channel, ColumnHeader1, ColumnHeader2, ColumnHeader3
    '                 ColumnHeader4, ColumnHeader5, ColumnHeader6, Comments, Compound
    '                 Confirm, CV, Diff, Excluded, FI
    '                 Filename, FinalUnits, FlagDetails, Formula, Group
    '                 GroupAverage, Height, IntegrationMode, IonRatio, IonType
    '                 IP, IR, IsotopicPatternScore, ISTDAmt, ISTDResponse
    '                 Level, LibMatchName, LibraryMatchRank, LibraryProbabilityPercent, LibraryScore
    '                 LS, Modification, mz_Apex, mz_Delta, mz_Expected
    '                 NumIsotopesMatched, PeakLabel, PeptideSequence, PK, ProteinName
    '                 RelativeRT, ResponseRatio, RSD, RSIRevDot, RT
    '                 RTDelta, SampleAmt, SampleID, SampleName, SampleType
    '                 SIDotProduct, SN, Status, StdAddAmount, TargetIonRatio
    '                 TheoreticalAmt, Type
    ' 
    '     Function: ExtractSampleData, GetPeakData, MeasureInternalStandardTuple, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Linq
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

    Public Property Metadata As Dictionary(Of String, String)

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
    Public Shared Iterator Function ExtractSampleData(table As IEnumerable(Of ScalarPeakReport), Optional indexBySampleID As Boolean = False) As IEnumerable(Of DataFile)
        Dim samples As IGrouping(Of String, ScalarPeakReport)() = table _
            .GroupBy(Function(a)
                         Return If(indexBySampleID, If(a.SampleID, ""), If(a.Filename, ""))
                     End Function) _
            .ToArray

        If samples.Length = 1 AndAlso samples(Scan0).Key = "" Then
            Call $"The sample data is required of index by {If(indexBySampleID, "sample id", "rawdata file name")}, but all these key value for make index is missing!".Warning
        End If

        For Each sample As IGrouping(Of String, ScalarPeakReport) In samples
            Dim types As Dictionary(Of String, ScalarPeakReport()) = sample _
                .Where(Function(a) a.Confirm <> "NotFound") _
                .GroupBy(Function(a) a.Type) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function)
            Dim targetCompound As Dictionary(Of IonPeakTableRow) = types("Target Compound") _
                .Select(Function(a) a.GetPeakData) _
                .ToDictionary
            Dim internalStandard As Dictionary(Of IonPeakTableRow) = types("Internal Standard") _
                .Select(Function(a) a.GetPeakData) _
                .ToDictionary
            Dim links = MeasureInternalStandardTuple(targetCompound.Keys, internalStandard.Keys).ToArray
            Dim compounds As New List(Of IonPeakTableRow)

            For Each compound As (compound$, is$) In links
                Dim target = targetCompound(compound.compound)
                Dim isdata = internalStandard(compound.is)

                target.IS = compound.is

                If isdata IsNot Nothing Then
                    target.maxinto_IS = isdata.maxinto
                    target.TPA_IS = isdata.TPA
                End If

                Call compounds.Add(target)
            Next

            Yield New DataFile(sample.Key, compounds)
        Next
    End Function

    ''' <summary>
    ''' This default linking function required of the compound name and IS name should start with the same token word
    ''' </summary>
    ''' <param name="compounds"></param>
    ''' <param name="isList"></param>
    ''' <returns></returns>
    Private Shared Iterator Function MeasureInternalStandardTuple(compounds As IEnumerable(Of String), isList As IEnumerable(Of String)) As IEnumerable(Of (compound As String, [is] As String))
        Dim isSet As String() = isList.ToArray

        For Each compound_id As String In compounds
            ' check with start with
            Dim upperKey As String = compound_id.ToUpper
            Dim check_is As String = isSet _
                .AsParallel _
                .Where(Function(is_id)
                           Return is_id.ToUpper.StartsWith(upperKey)
                       End Function) _
                .FirstOrDefault

            Yield (compound_id, check_is)
        Next
    End Function

End Class
