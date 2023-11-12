Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.Base64Decoder
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.Extensions
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Chromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader.Chromatogram
Imports RawChromatogram = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

Public Module ChromatogramReader

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

        Dim data = time_array _
                .Select(Function(t, i)
                            Return New ChromatogramTick(t, intensity_array(i))
                        End Function) _
                .ToArray

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
    Public Iterator Function GetTicks(chromatogram As Chromatogram, Optional isbpc As Boolean = False) As IEnumerable(Of ChromatogramTick)
        Dim scan_time = chromatogram.scan_time
        Dim bpc = chromatogram.BPC
        Dim tic = chromatogram.TIC

        For i As Integer = 0 To scan_time.Length - 1
            If isbpc Then
                Yield New ChromatogramTick With {.Time = scan_time(i), .Intensity = bpc(i)}
            Else
                Yield New ChromatogramTick With {.Time = scan_time(i), .Intensity = tic(i)}
            End If
        Next
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
