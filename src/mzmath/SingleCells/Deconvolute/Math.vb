﻿#Region "Microsoft.VisualBasic::e577b462a9257235ec7f1000a098c77b, mzmath\SingleCells\Deconvolute\Math.vb"

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

    '   Total Lines: 167
    '    Code Lines: 106 (63.47%)
    ' Comment Lines: 34 (20.36%)
    '    - Xml Docs: 85.29%
    ' 
    '   Blank Lines: 27 (16.17%)
    '     File Size: 6.74 KB


    '     Module Math
    ' 
    '         Function: (+5 Overloads) GetMzIndex, GetMzIndexFastBin
    '         Class IndexTask
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Sub: Solve
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Parallel

Namespace Deconvolute

    Public Module Math

        ''' <summary>
        ''' get a m/z vector for run matrix deconvolution by pick top N ions in each pixel
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="topN"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetMzIndex(raw As IMZPack, mzdiff As Double, topN As Integer) As MassWindow()
            Dim scanMz As New List(Of Double)
            Dim top As IEnumerable(Of ms2)

            For Each x As ScanMS1 In raw.MS
                top = x.GetMs _
                    .OrderByDescending(Function(i) i.intensity) _
                    .Take(topN)
                scanMz.AddRange(top.Select(Function(mzi) mzi.mz))
            Next

            ' just pick the top intensity ion, 
            ' no frequency filter
            Return GetMzIndex(scanMz, mzdiff, freq:=0.0)
        End Function

        ''' <summary>
        ''' get a m/z vector for run matrix deconvolution
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzdiff"></param>
        ''' <param name="freq"></param>
        ''' <returns>
        ''' m/z data vector has been re-order ascding
        ''' </returns>
        Public Function GetMzIndex(raw As IMZPack, mzdiff As Double, freq As Double,
                                   Optional fast As Boolean = True,
                                   Optional verbose As Boolean = False) As MassWindow()
            If fast Then
                Dim scanMz As New List(Of Double())

                For Each x As Double() In raw.MS.Select(Function(ms) ms.mz)
                    Call scanMz.Add(x)
                Next

                Call scanMz.Shuffle

                If verbose Then
                    Call VBDebugger.EchoLine($"processing {scanMz.Count} ion feature blocks...")
                End If

                Return GetMzIndexFastBin(scanMz, mzdiff, freq, verbose:=verbose)
            Else
                Dim scanMz As New List(Of Double)

                For Each x As Double() In raw.MS.Select(Function(ms) ms.mz)
                    Call scanMz.AddRange(x)
                Next

                Return GetMzIndex(scanMz, mzdiff, freq)
            End If
        End Function

        <Extension>
        Public Function GetMzIndexFastBin(scanMz As List(Of Double()), mzdiff As Double, freq As Double, Optional verbose As Boolean = False) As MassWindow()
            Dim par As New IndexTask(scanMz, mzdiff, verbose)
            Dim subgroups = DirectCast(par.Run(), IndexTask).groups
            Dim merge = subgroups.IteratesALL _
                .Where(Function(n) n.Length > 0) _
                .GroupBy(Function(a) a.Average, offsets:=mzdiff) _
                .ToArray
            Dim bins = merge _
                .Select(Function(g)
                            Dim bin As Double() = g _
                                .Select(Function(i) i.value) _
                                .IteratesALL _
                                .ToArray

                            Return New NamedCollection(Of Double)(bin.Average.ToString, bin)
                        End Function) _
                .OrderByDescending(Function(g) g.Length) _
                .ToArray

            Return GetMzIndex(bins, freq)
        End Function

        Private Class IndexTask : Inherits VectorTask

            Friend ReadOnly blocks As List(Of Double())
            Friend ReadOnly groups As NamedCollection(Of Double)()()
            Friend ReadOnly mzdiff As Double

            Sub New(blocks As List(Of Double()), mzdiff As Double, verbose As Boolean)
                Call MyBase.New(blocks.Count, verbose)

                Me.blocks = blocks
                Me.mzdiff = mzdiff
                Me.groups = Me.Allocate(Of NamedCollection(Of Double)())(all:=False)
            End Sub

            Protected Overrides Sub Solve(start As Integer, ends As Integer, thread_id As Integer)
                Dim subblocks As New List(Of Double())

                For i As Integer = start To ends
                    Call subblocks.Add(blocks(i))
                Next

                groups(thread_id) = NumberGroups.GroupByTree(subblocks, offset:=mzdiff).ToArray
            End Sub
        End Class

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMzIndex(raw As IEnumerable(Of ms2), mzdiff As Double, freq As Double) As MassWindow()
            Return GetMzIndex(raw.Select(Function(r) r.mz), mzdiff, freq)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mzBins">
        ''' the bin data should be sorted via the bin size desc!
        ''' </param>
        ''' <param name="freq"></param>
        ''' <returns></returns>
        Private Function GetMzIndex(mzBins As NamedCollection(Of Double)(), freq As Double) As MassWindow()
            Dim counts As Vector = mzBins.Select(Function(a) a.Length).AsVector
            ' normalize to [0,1]
            Dim norm As Vector = counts / counts.Max
            Dim n As Integer = (norm > freq).Sum
            Dim mzUnique As MassWindow() = mzBins _
                .Take(n) _
                .Select(Function(v) New MassWindow(v)) _
                .OrderBy(Function(mzi) mzi.mass) _
                .ToArray

            Return mzUnique
        End Function

        ''' <summary>
        ''' get a m/z vector for run matrix deconvolution
        ''' </summary>
        ''' <param name="scanMz"></param>
        ''' <param name="mzdiff"></param>
        ''' <param name="freq">[0,1] percentage</param>
        ''' <returns></returns>
        Public Function GetMzIndex(scanMz As IEnumerable(Of Double), mzdiff As Double, freq As Double) As MassWindow()
            Dim mzBins As NamedCollection(Of Double)() = scanMz _
                .GroupBy(offset:=mzdiff) _
                .Where(Function(v) v.Length > 0) _
                .OrderByDescending(Function(a) a.Length) _
                .ToArray

            Return GetMzIndex(mzBins, freq)
        End Function
    End Module
End Namespace
