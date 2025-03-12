#Region "Microsoft.VisualBasic::1db67a7156e30b537695fd381486cf6e, mzmath\mz_deco\Task\peak_align_task.vb"

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

    '   Total Lines: 57
    '    Code Lines: 39 (68.42%)
    ' Comment Lines: 6 (10.53%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 12 (21.05%)
    '     File Size: 2.04 KB


    '     Class peak_align_task
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: MakeIonGroups
    ' 
    '         Sub: Solve
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
