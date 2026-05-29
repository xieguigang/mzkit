#Region "Microsoft.VisualBasic::037ba1293af43b9d9b24129404405f0e, mzmath\mz_deco\Task\xic_deco_task.vb"

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

    '   Total Lines: 106
    '    Code Lines: 82 (77.36%)
    ' Comment Lines: 6 (5.66%)
    '    - Xml Docs: 50.00%
    ' 
    '   Blank Lines: 18 (16.98%)
    '     File Size: 4.03 KB


    '     Class xic_deco_task
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ExtractAlignedPeaks
    ' 
    '         Sub: Solve
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Tasks

    ''' <summary>
    ''' make peak deconvolution and alignment between the samples
    ''' </summary>
    Public Class xic_deco_task : Inherits peaktable_task

        Dim pool As (mz As Double, samples As NamedValue(Of MzGroup)())()
        Dim rtRange As DoubleRange
        Dim baseline As Double
        Dim joint As Boolean
        Dim dtw As Boolean

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

                Dim result As xcms2() = ExtractAlignedPeaks(samples_xic,
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

        Public Shared Function ExtractAlignedPeaks(dtw_aligned As NamedValue(Of MzGroup)(), rtRange As DoubleRange,
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
End Namespace
