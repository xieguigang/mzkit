Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Math.SignalProcessing.NDtw
Imports Microsoft.VisualBasic.Math.SignalProcessing.NDtw.Preprocessing

Public Class XICPool

    ReadOnly samplefiles As New Dictionary(Of String, MzGroup())
    ReadOnly sampleIndex As New Dictionary(Of String, BlockSearchFunction(Of (mz As Double, Integer)))

    Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(sample As String, ParamArray ions As MzGroup())
        Call samplefiles.Add(sample, ions)
        Call sampleIndex.Add(sample, ions.Select(Function(i) i.mz).ToArray.CreateMzIndex)
    End Sub

    Public Iterator Function GetXICMatrix(mz As Double, mzdiff As Tolerance) As IEnumerable(Of NamedValue(Of MzGroup))
        For Each file As KeyValuePair(Of String, BlockSearchFunction(Of (mz As Double, Integer))) In sampleIndex
            Dim offsets = file.Value _
                .Search((mz, -1)) _
                .Where(Function(q) mzdiff(mz, q.mz)) _
                .OrderBy(Function(q) mzdiff.MassError(q.mz, mz)) _
                .FirstOrDefault

            If offsets.mz > 0 Then
                Dim XIC = samplefiles(file.Key)(offsets.Item2)
                Dim tuple As New NamedValue(Of MzGroup)(file.Key, XIC)

                Yield tuple
            End If
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DtwXIC(mz As Double, mzdiff As Tolerance) As IEnumerable(Of NamedValue(Of MzGroup))
        Return DtwXIC(GetXICMatrix(mz, mzdiff).ToArray)
    End Function

    Public Shared Iterator Function DtwXIC(rawdata As NamedValue(Of MzGroup)()) As IEnumerable(Of NamedValue(Of MzGroup))
        ' make the length equals to each other
        Dim orders = rawdata _
            .OrderByDescending(Function(a) a.Value.MaxInto) _
            .ToArray
        Dim signals As GeneralSignal() = orders _
            .Select(Function(x) x.Value.CreateSignal(x.Name)) _
            .ToArray
        Dim rt As Double() = Rt_vector(signals)
        Dim signals2 As GeneralSignal() = signals _
            .Select(Function(sig)
                        Dim sample = Resampler.CreateSampler(sig)
                        Dim intensity As Double() = sample.GetVector(rt)
                        Dim resample As New GeneralSignal With {
                            .Measures = rt.ToArray,
                            .description = sig.description,
                            .measureUnit = sig.measureUnit,
                            .meta = sig.meta,
                            .reference = sig.reference,
                            .Strength = intensity,
                            .weight = sig.weight
                        }

                        Return resample
                    End Function) _
            .ToArray
        Dim refer = signals2(0)
        Dim offset As Integer = 1

        Yield New NamedValue(Of MzGroup)(
            name:=refer.reference,
            value:=New MzGroup(
                mz:=orders(0).Value.mz,
                xic:=refer.GetTimeSignals(Function(ti, into)
                                              Return New ChromatogramTick(ti, into)
                                          End Function))
            )

        For Each query As GeneralSignal In signals2.Skip(1)
            Dim dtw As New Dtw({refer, query}, preprocessor:=IPreprocessor.Normalization)
            Dim align_dt As Point() = dtw.GetPath.ToArray
            Dim tick As New List(Of ChromatogramTick)
            Dim mz As Double = orders(offset).Value.mz

            For Each point In align_dt
                tick.Add(New ChromatogramTick(rt(point.X), query.Strength(point.Y)))
            Next

            offset += 1

            Yield New NamedValue(Of MzGroup)(query.reference, New MzGroup(mz, tick))
        Next
    End Function

    Private Shared Function Rt_vector(signals As GeneralSignal()) As Double()
        Dim rt As Double() = signals.Select(Function(s) s.Measures) _
            .IteratesALL _
            .OrderBy(Function(ti) ti) _
            .ToArray
        Dim diff_rt As Double = NumberGroups.diff(rt) _
            .OrderByDescending(Function(dt) dt) _
            .Skip(rt.Length * 0.1) _
            .Average

        Return seq2(rt.Min, rt.Max, by:=diff_rt)
    End Function

End Class
