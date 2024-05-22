#Region "Microsoft.VisualBasic::6fb02a313751062fc2fd279d2ca5de9b, assembly\SignalReader\ChromatogramReader.vb"

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

    '   Total Lines: 151
    '    Code Lines: 111 (73.51%)
    ' Comment Lines: 22 (14.57%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 18 (11.92%)
    '     File Size: 6.65 KB


    ' Module ChromatogramReader
    ' 
    '     Function: FileAlignment, GetChromatogram, (+3 Overloads) GetIonsChromatogram, GetSignal, GetTicks
    '               Ticks
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.Base64Decoder
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.Extensions
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Chromatogram = BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram
Imports RawChromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Public Module ChromatogramReader

    ''' <summary>
    ''' make timed signal data alignment
    ''' </summary>
    ''' <param name="rawfiles"></param>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function FileAlignment(rawfiles As ChromatogramSerial(), Optional dt As Double = 0.5) As IEnumerable(Of ChromatogramSerial)
        Dim samples As Resampler() = New Resampler(rawfiles.Length - 1) {}

        For i As Integer = 0 To rawfiles.Length - 1
            samples(i) = Resampler.CreateSampler(
                x:=rawfiles(i).GetTime.ToArray,
                y:=rawfiles(i).GetIntensity.ToArray
            )
        Next

        Dim rtmin As Double = Aggregate line As ChromatogramSerial In rawfiles Into Min(line.rtmin)
        Dim rtmax As Double = Aggregate line As ChromatogramSerial In rawfiles Into Max(line.rtmax)
        Dim rt As Double() = seq(rtmin, rtmax, by:=dt).ToArray

        For i As Integer = 0 To samples.Length - 1
            Dim signal As Double() = samples(i).GetVector(rt)
            Dim ticks As IEnumerable(Of ChromatogramTick) = ChromatogramTick.Zip(rt, signal)

            Yield New ChromatogramSerial(rawfiles(i).Name, ticks)
        Next
    End Function

    Public Function GetIonsChromatogram(file As String) As Chromatogram
        Return file.LoadChromatogramList.GetIonsChromatogram
    End Function

    ''' <summary>
    ''' Align the TIC/BPC
    ''' </summary>
    ''' <param name="tic"></param>
    ''' <param name="bpc"></param>
    ''' <returns></returns>
    Public Function GetIonsChromatogram(tic As ChromatogramTick(), bpc As ChromatogramTick()) As Chromatogram
        If tic.IsNullOrEmpty AndAlso bpc.IsNullOrEmpty Then
            Return Nothing
        ElseIf tic.IsNullOrEmpty Then
            Return New Chromatogram With {.BPC = bpc.IntensityArray, .scan_time = bpc.TimeArray, .TIC = .BPC}
        ElseIf bpc.IsNullOrEmpty Then
            Return New Chromatogram With {.TIC = tic.IntensityArray, .scan_time = tic.TimeArray, .BPC = .TIC}
        End If

        Dim union = tic.JoinIterates(bpc).OrderBy(Function(ci) ci.Time).ToArray
        Dim ticReader As Resampler = Resampler.CreateSampler(tic.TimeArray, tic.IntensityArray)
        Dim bpcReader As Resampler = Resampler.CreateSampler(bpc.TimeArray, bpc.IntensityArray)

        Return New Chromatogram With {
            .scan_time = union.TimeArray.Range.AsVector(10000),
            .BPC = .scan_time.Select(Function(ti) bpcReader.GetIntensity(ti)).ToArray,
            .TIC = .scan_time.Select(Function(ti) ticReader.GetIntensity(ti)).ToArray
        }
    End Function

    <Extension>
    Public Function GetIonsChromatogram(channels As IEnumerable(Of RawChromatogram)) As Chromatogram
        Dim allTicks As ChromatogramTick() = channels _
            .Where(Function(chr) chr.id <> "TIC" AndAlso chr.id <> "BPC") _
            .Select(AddressOf Ticks) _
            .IteratesALL _
            .OrderBy(Function(t) t.Time) _
            .ToArray
        Dim time_groups = allTicks _
            .GroupBy(Function(t) t.Time, offsets:=0.1) _
            .OrderBy(Function(d) Val(d.name)) _
            .ToArray
        Dim TIC As Double() = time_groups.Select(Function(d) d.Sum(Function(p) p.Intensity)).ToArray
        Dim BPC As Double() = time_groups.Select(Function(d) d.Max(Function(p) p.Intensity)).ToArray

        Return New Chromatogram With {
            .scan_time = time_groups _
                .Select(Function(p) Val(p.name)) _
                .ToArray,
            .TIC = TIC,
            .BPC = BPC
        }
    End Function

    ''' <summary>
    ''' Get this chromatogram signal ticks.(返回来的时间的单位都统一为秒)
    ''' </summary>
    ''' <param name="chromatogram"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Ticks(chromatogram As RawChromatogram) As ChromatogramTick()
        Dim time = chromatogram.ByteArray("time array")
        Dim into = chromatogram.ByteArray("intensity array")
        Dim timeUnit = time.cvParams.KeyItem("time array").unitName
        Dim intoUnit = into.cvParams.KeyItem("intensity array").unitName
        Dim time_array = time.Base64Decode.AsVector
        Dim intensity_array = into.Base64Decode

        If timeUnit.TextEquals("minute") Then
            time_array = time_array * 60
        End If

        Dim data = ChromatogramTick.Zip(time_array, intensity_array).ToArray
        Return data
    End Function

    ''' <summary>
    ''' Extract the chromatogram data from the mzML/mzXML raw data node
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GetChromatogram(raw As RawChromatogram) As ChromatogramSerial
        Return New ChromatogramSerial(raw.ToString) With {.Chromatogram = raw.Ticks()}
    End Function

    <Extension>
    Public Function GetTicks(chromatogram As Chromatogram, Optional isbpc As Boolean = False) As IEnumerable(Of ChromatogramTick)
        If isbpc Then
            Return ChromatogramTick.Zip(chromatogram.scan_time, chromatogram.BPC)
        Else
            Return ChromatogramTick.Zip(chromatogram.scan_time, chromatogram.TIC)
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GetSignal(chromatogram As Chromatogram, Optional isbpc As Boolean = False) As GeneralSignal
        Return New GeneralSignal With {
            .description = If(isbpc, "BPC", "TIC"),
            .Measures = chromatogram.scan_time,
            .measureUnit = "seconds",
            .reference = "n/a",
            .Strength = If(isbpc, chromatogram.BPC, chromatogram.TIC)
        }
    End Function
End Module
