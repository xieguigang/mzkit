Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Serialization.JSON
Imports std = System.Math

''' <summary>
''' Make sample spectrum aligns to the peaktable based on the pearson correlation method
''' </summary>
Public Class SpectrumGrid

    ''' <summary>
    ''' spectrum cluster which is indexed via the rt window
    ''' </summary>
    Dim clusters As BlockSearchFunction(Of SpectrumLine)
    Dim filenames As String()

    ReadOnly rt_win As Double = 7.5
    ReadOnly dia_n As Integer = 1

    Sub New(Optional rt_win As Double = 7.5)
        Me.rt_win = rt_win
    End Sub

    Public Function SetRawDataFiles(files As IEnumerable(Of NamedCollection(Of PeakMs2))) As SpectrumGrid
        clusters = New BlockSearchFunction(Of SpectrumLine)(Clustering(files), Function(a) a.rt, 5, fuzzy:=True)
        Return Me
    End Function

    ''' <summary>
    ''' make spectrum clustering
    ''' </summary>
    ''' <param name="rawdata">the spectrum rawdata from multiple rawdata files</param>
    ''' <returns>
    ''' a collection of the ion groups
    ''' </returns>
    Private Iterator Function Clustering(rawdata As IEnumerable(Of NamedCollection(Of PeakMs2))) As IEnumerable(Of SpectrumLine)
        Dim ions As New List(Of PeakMs2)
        Dim files As New List(Of String)
        Dim qc_filter As New Regex(".+QC.+\d+", RegexOptions.Compiled Or RegexOptions.Singleline)

        For Each file As NamedCollection(Of PeakMs2) In rawdata
            Call files.Add(file.name)
            Call ions.AddRange(file)
        Next

        ' group the spectrum ions via the precursor ion m/z
        Dim parent_groups As NamedCollection(Of PeakMs2)() = ions _
            .GroupBy(Function(i) i.mz, offsets:=1) _
            .ToArray

        ' removes QC files for the cor test
        filenames = files _
            .Where(Function(name)
                       Return Not name.IsPattern(qc_filter)
                   End Function) _
            .ToArray

        Call VBDebugger.EchoLine("make spectrum alignment for each precursor ion...")

        For Each ion_group As NamedCollection(Of PeakMs2) In TqdmWrapper.Wrap(parent_groups)
            Dim tree As New BinaryClustering()

            ' ion groups has the same precursor ion m/z
            ' due to the reason of precursor mz has already been
            ' grouped before
            tree = tree.Tree(ion_group)

            For Each cluster As NamedCollection(Of PeakMs2) In tree.GetClusters
                ' split by rt
                Dim rt_groups As NamedCollection(Of PeakMs2)() = cluster _
                    .GroupBy(Function(a) a.rt, offsets:=rt_win) _
                    .ToArray

                For Each group As NamedCollection(Of PeakMs2) In rt_groups
                    Dim fileIndex = group.GroupBy(Function(si) si.file) _
                        .ToDictionary(Function(a) a.Key,
                                      Function(a)
                                          Return a.ToArray
                                      End Function)
                    Dim i2 As Double() = filenames _
                        .Select(Function(name)
                                    Return If(fileIndex.ContainsKey(name),
                                        fileIndex(name).Average(Function(i) i.intensity), 0)
                                End Function) _
                        .ToArray

                    Yield New SpectrumLine With {
                        .intensity = SumNorm(i2),
                        .cluster = group.value,
                        .rt = Val(group.name),
                        .mz = group.Average(Function(si) si.mz)
                    }
                Next
            Next
        Next
    End Function

    Private Function SumNorm(ByRef v As Double()) As Double()
        Dim pos = v.Where(Function(vi) vi > 0).ToArray

        ' all element is zero!
        If pos.Length = 0 Then
            Return v
        End If

        Dim minPos As Double = pos.Min / 2

        For i As Integer = 0 To v.Length - 1
            If v(i) <= 0.0 Then
                v(i) = minPos
            End If
        Next

        Return SIMD.Multiply.f64_scalar_op_multiply_f64(10000, SIMD.Divide.f64_op_divide_f64_scalar(v, v.Sum))
    End Function

    Public Iterator Function AssignPeaks(peaks As IEnumerable(Of xcms2), Optional assign_top As Integer = 3) As IEnumerable(Of RawPeakAssign)
        Dim q As New SpectrumLine

        For Each peak As xcms2 In TqdmWrapper.Wrap(peaks.ToArray)
            Dim i1 As Double() = SumNorm(peak(filenames))
            Dim candidates = clusters _
                .Search(q.SetRT(peak.rt), tolerance:=rt_win) _
                .Where(Function(c)
                           Return std.Abs(c.mz - peak.mz) < 0.3
                       End Function) _
                .AsParallel _
                .Select(Function(c)
                            Dim cor As Double, pval As Double
                            cor = Correlations.GetPearson(i1, c.intensity, prob2:=pval, throwMaxIterError:=False)
                            Return (c, cor, pval, score:=cor / (std.Abs(peak.rt - c.rt) + 1))
                        End Function) _
                .OrderByDescending(Function(c) c.cor) _
                .Take(assign_top) _
                .ToArray

            For Each candidate In candidates
                Yield New RawPeakAssign With {
                    .peak = peak,
                    .ms2 = candidate.c.cluster _
                        .Select(Function(c)
                                    c = New PeakMs2(c)

                                    If c.meta Is Nothing Then
                                        c.meta = New Dictionary(Of String, String) From {
                                            {"ROI", peak.ID}
                                        }
                                    Else
                                        c.meta!ROI = peak.ID
                                    End If

                                    c.mz = peak.mz
                                    c.meta!cor = candidate.cor
                                    c.meta!pval = candidate.pval
                                    c.meta!rt_offset = std.Abs(c.rt - peak.rt)

                                    Return c
                                End Function) _
                        .ToArray,
                    .cor = candidate.cor,
                    .score = candidate.score,
                    .pval = candidate.pval,
                    .v1 = i1,
                    .v2 = candidate.c.intensity
                }
            Next
        Next
    End Function

End Class

Public Class SpectrumLine

    Public Property cluster As PeakMs2()
    Public Property intensity As Double()
    Public Property rt As Double
    Public Property mz As Double

    Friend Function SetRT(rt As Double) As SpectrumLine
        _rt = rt
        Return Me
    End Function

    Public Overrides Function ToString() As String
        Return $"{mz.ToString("F3")}@{(rt / 60).ToString("F2")}min, {cluster.Length} files: {cluster.Select(Function(s) s.file).GetJson}"
    End Function

End Class

Public Class RawPeakAssign : Implements IReadOnlyId

    Public Property peak As xcms2
    Public Property ms2 As PeakMs2()
    Public Property cor As Double
    Public Property score As Double
    Public Property pval As Double

    Public Property v1 As Double()
    Public Property v2 As Double()

    Public ReadOnly Property Id As String Implements IReadOnlyId.Identity
        Get
            Return peak.ID
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return peak.ToString & $" correlated with {ms2.Length} spectrum, pearson={cor}"
    End Function

End Class