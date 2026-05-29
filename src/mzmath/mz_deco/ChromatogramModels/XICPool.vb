#Region "Microsoft.VisualBasic::1147a6802241362934bda5bdfcdfa73d, mzmath\mz_deco\ChromatogramModels\XICPool.vb"

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

    '   Total Lines: 155
    '    Code Lines: 120 (77.42%)
    ' Comment Lines: 10 (6.45%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 25 (16.13%)
    '     File Size: 5.93 KB


    ' Class XICPool
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: (+2 Overloads) DtwXIC, GetXICMatrix, Rt_vector
    ' 
    '     Sub: Add
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Math.SignalProcessing.NDtw
Imports Microsoft.VisualBasic.Math.SignalProcessing.NDtw.Preprocessing

''' <summary>
''' grouping ion XIC data from multiple sample data file
''' </summary>
Public Class XICPool

    ReadOnly samplefiles As New Dictionary(Of String, MzGroup())
    ReadOnly sampleIndex As New Dictionary(Of String, MzPool)

    Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(sample As String, ParamArray ions As MzGroup())
        Call samplefiles.Add(sample, ions)
        Call sampleIndex.Add(sample, New MzPool(ions.Select(Function(i) i.mz)))
    End Sub

    ''' <summary>
    ''' get XIC data for ion matches
    ''' </summary>
    ''' <param name="mz">the given ion target m/z value</param>
    ''' <param name="mzdiff">the mass tolerance error</param>
    ''' <returns></returns>
    Public Iterator Function GetXICMatrix(mz As Double, mzdiff As Tolerance) As IEnumerable(Of NamedValue(Of MzGroup))
        Dim offsets As MzIndex

        For Each file As KeyValuePair(Of String, MzPool) In sampleIndex
            If file.Value Is Nothing Then
                Continue For
            End If

            offsets = file.Value _
                .Search(mz) _
                .Where(Function(q) mzdiff(mz, q.mz)) _
                .OrderBy(Function(q) mzdiff.MassError(q.mz, mz)) _
                .FirstOrDefault

            If offsets Is Nothing Then
                Continue For
            End If

            If offsets.mz > 0 Then
                Dim XIC = samplefiles(file.Key)(offsets.index)
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
            .Select(Function(a)
                        Return New NamedValue(Of MzGroup)(a.Name, a.Value.TrimRTScatter)
                    End Function) _
            .Where(Function(a) a.Value.size >= 3) _
            .OrderByDescending(Function(a) a.Value.MaxInto) _
            .ToArray
        Dim signals As GeneralSignal() = orders _
            .Select(Function(x) x.Value.CreateSignal(x.Name)) _
            .ToArray
        Dim diff_rt As Double = Nothing
        Dim rt As Double() = Rt_vector(signals, diff_rt)
        Dim signals2 As GeneralSignal() = signals _
            .Select(Function(sig)
                        Dim sample = Resampler.CreateSampler(sig, max_dx:=3)
                        Dim intensity As Double() = sample(x:=rt)
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

        If signals2.Length = 0 Then
            Return
        End If

        Dim refer As GeneralSignal = signals2(0)
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
            Dim dtw As New Dtw({refer, query}, preprocessor:=IPreprocessor.None)
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

    Private Shared Function Rt_vector(signals As GeneralSignal(), ByRef diff_rt As Double) As Double()
        Dim rt As Double() = signals.Select(Function(s) s.Measures) _
            .IteratesALL _
            .OrderBy(Function(ti) ti) _
            .ToArray
        Dim diff_v = NumberGroups.diff(rt)

        diff_rt = 0

        If diff_v.Length = 0 Then
            Return {}
        ElseIf diff_v.Length = 1 Then
            Return seq2(rt.Min, rt.Max, by:=diff_v.First)
        Else
            diff_rt = diff_v _
                .OrderByDescending(Function(dt) dt) _
                .Skip(rt.Length * 0.1) _
                .Average

            Return seq2(rt.Min, rt.Max, by:=diff_rt)
        End If
    End Function

End Class
