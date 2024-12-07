Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Tasks

    Public Class peak_align_task : Inherits peaktable_task

        Dim peaks As NamedCollection(Of PeakFeature)()
        Dim max_rtwin As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="peaks">
        ''' peaks data should be group by mz
        ''' </param>
        Sub New(peaks As NamedCollection(Of PeakFeature)(), max_rtwin As Double)
            Call MyBase.New(peaks.Length, verbose:=True)

            Me.peaks = peaks
            Me.max_rtwin = max_rtwin
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            Dim rt_shifts As New List(Of RtShift)

            For i As Integer = start To ends
                Dim peaksSet As NamedCollection(Of PeakFeature)() = {peaks(i)}
                Dim xcms = peaksSet _
                    .XicTable(rtwin:=max_rtwin, rt_shifts:=rt_shifts,
                              assign_samples:=False) _
                    .ToArray

                SyncLock out
                    Call out.AddRange(xcms)
                End SyncLock
            Next

            SyncLock Me.rt_shifts
                Me.rt_shifts.AddRange(rt_shifts)
            End SyncLock
        End Sub

        Public Shared Function MakeIonGroups(samples As IEnumerable(Of NamedCollection(Of PeakFeature)),
                                             Optional mzdiff As Double = 0.01) As IEnumerable(Of NamedCollection(Of PeakFeature))

            Dim pool As PeakFeature() = samples.IteratesAll.ToArray
            Dim ions = pool.GroupBy(Function(a) a.mz, offsets:=mzdiff).ToArray

            Return ions
        End Function
    End Class
End Namespace