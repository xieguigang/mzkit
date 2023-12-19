Imports System.Drawing
Imports System.Runtime.CompilerServices
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

    Public Function DtwXIC(mz As Double, mzdiff As Tolerance) As NamedValue(Of MzGroup)()
        Dim rawdata = GetXICMatrix(mz, mzdiff).ToArray
        ' make the length equals to each other
        Dim signals As GeneralSignal() = rawdata _
            .OrderByDescending(Function(a) a.Value.MaxInto) _
            .Select(Function(x) x.Value.CreateSignal(x.Name)) _
            .ToArray
        Dim rt As Double() = signals.Select(Function(s) s.Measures) _
            .IteratesALL _
            .OrderBy(Function(ti) ti) _
            .ToArray
        Dim diff_rt As Double() = NumberGroups.diff(rt).OrderByDescending(Function(dt) dt).ToArray
        Dim sampler As Resampler() = signals _
            .Select(Function(sig) Resampler.CreateSampler(sig)) _
            .ToArray

        Dim dtw As New Dtw(signals, preprocessor:=IPreprocessor.Normalization)
        Dim align_dt As Point() = dtw.GetPath.ToArray


    End Function

End Class
