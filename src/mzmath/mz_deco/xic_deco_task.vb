Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel

Public Class xic_deco_task : Inherits VectorTask

    Dim pool As (mz As Double, samples As NamedValue(Of MzGroup)())(),
        rtRange As DoubleRange,
        baseline As Double,
        joint As Boolean
    Dim dtw As Boolean

    Public ReadOnly out As New List(Of xcms2)
    Public ReadOnly rt_shifts As New List(Of RtShift)

    Public Sub New(pool As XICPool, features_mz As Double(),
                   errors As Tolerance,
                   rtRange As DoubleRange,
                   baseline As Double,
                   joint As Boolean,
                   dtw As Boolean)

        Call MyBase.New(features_mz.Length, verbose:=True)

        Me.pool = features_mz _
            .Select(Function(mz)
                        Return (mz, pool.GetXICMatrix(mz, errors).ToArray)
                    End Function) _
            .ToArray
        Me.dtw = dtw
        Me.rtRange = rtRange
        Me.baseline = baseline
        Me.joint = joint
    End Sub

    Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
        Dim rt_shifts As New List(Of RtShift)

        For i As Integer = start To ends
            Dim samples_xic = pool(i).samples
            Dim shifts As New List(Of RtShift)

            If dtw Then
                samples_xic = XICPool.DtwXIC(samples_xic).ToArray
            End If

            Dim result As xcms2() = extractAlignedPeaks(samples_xic,
                rtRange:=rtRange,
                baseline:=baseline,
                joint:=joint, xic_align:=True,
                rt_shifts:=shifts)

            Call rt_shifts.AddRange(shifts)

            SyncLock out
                Call out.AddRange(result)
            End SyncLock
        Next

        SyncLock Me.rt_shifts
            Me.rt_shifts.AddRange(rt_shifts)
        End SyncLock
    End Sub

    Public Shared Function extractAlignedPeaks(dtw_aligned As NamedValue(Of MzGroup)(), rtRange As DoubleRange,
                                     baseline As Double,
                                     joint As Boolean,
                                     xic_align As Boolean,
                                     ByRef rt_shifts As List(Of RtShift)) As xcms2()

        ' and then export the peaks and area data
        Dim peaksSet As NamedCollection(Of PeakFeature)() = dtw_aligned _
            .Select(Function(sample)
                        ' extract all peaks from the XIC data
                        ' for a single sample
                        Dim peaks = sample.Value.GetPeakGroups(
                                peakwidth:=rtRange,
                                quantile:=baseline,
                                sn_threshold:=0,
                                joint:=joint) _
                            .ExtractFeatureGroups _
                            .ToArray

                        Return New NamedCollection(Of PeakFeature)(sample.Name, peaks)
                    End Function) _
            .ToArray
        Dim xcms As xcms2()

        If xic_align Then
            xcms = peaksSet _
                .XicTable(rtwin:=rtRange.Max, rt_shifts:=rt_shifts) _
                .ToArray
        Else
            xcms = peaksSet.XcmsTable.ToArray
        End If

        Return xcms
    End Function
End Class
