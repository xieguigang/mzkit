﻿#Region "Microsoft.VisualBasic::c76fd8e2abd88831067cab4943219aac, mzmath\ms2_math-core\Spectra\MzPool.vb"

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

    '   Total Lines: 115
    '    Code Lines: 58 (50.43%)
    ' Comment Lines: 43 (37.39%)
    '    - Xml Docs: 97.67%
    ' 
    '   Blank Lines: 14 (12.17%)
    '     File Size: 4.33 KB


    '     Class MzPool
    ' 
    '         Properties: ionSet, raw, size
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Query, Search, SearchBest
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace Spectra

    ''' <summary>
    ''' a wrapper of the binary search function for do mz search 
    ''' with given tolerance error in fast speed.
    ''' </summary>
    ''' <remarks>
    ''' this class module is works based on the binary search helper <see cref="BlockSearchFunction"/>
    ''' </remarks>
    Public Class MzPool

        ''' <summary>
        ''' a wrapper of the binary search function
        ''' </summary>
        ReadOnly index As BlockSearchFunction(Of MzIndex)

        ''' <summary>
        ''' the number of the mz peaks input
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                Return index.size
            End Get
        End Property

        ''' <summary>
        ''' get a set of the raw input ion mz element values,
        ''' this length of this vector is equals to the <see cref="size"/>
        ''' property value.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property raw As MzIndex()
            Get
                Return index.raw
            End Get
        End Property

        Public ReadOnly Property ionSet As Double()
            Get
                Return raw.Select(Function(i) i.mz).ToArray
            End Get
        End Property

        Default Public ReadOnly Property ion(i As Integer) As Double
            Get
                Return index.raw()(i).mz
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(mz As IEnumerable(Of Double), Optional win_size As Double = 1, Optional verbose As Boolean = True)
            index = mz _
                .ToArray _
                .CreateMzIndex(win_size, verbose:=verbose)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(spec As IEnumerable(Of ms2), Optional win_size As Double = 1)
            index = spec.CreateMzIndex(win_size)
        End Sub

        ''' <summary>
        ''' get a set of the index hits that matches the given absolute error.
        ''' </summary>
        ''' <param name="mz">target mz value to do search</param>
        ''' <param name="mzdiff"></param>
        ''' <returns>this function returns empty collection if no hits was found.</returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Search(mz As Double, Optional mzdiff As Double? = Nothing) As IEnumerable(Of MzIndex)
            Return index.Search(New MzIndex(mz), tolerance:=mzdiff)
        End Function

        ''' <summary>
        ''' A wrapper of the <see cref="Search(Double, Double?)"/> function
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <param name="mzdiff"></param>
        ''' <returns>
        ''' this function convert the search result <see cref="MzIndex"/> object as tuple value
        ''' </returns>
        Public Iterator Function Query(mz As Double, Optional mzdiff As Double? = Nothing) As IEnumerable(Of (Double, Integer))
            For Each hit As MzIndex In index.Search(New MzIndex(mz), tolerance:=mzdiff)
                Yield (hit.mz, hit.index)
            Next
        End Function

        ''' <summary>
        ''' get the index hit with smallest absolute error
        ''' </summary>
        ''' <param name="mz">the target mz numeric value to do search</param>
        ''' <param name="mzdiff"></param>
        ''' <returns>
        ''' this function returns nothing if no hits could be found
        ''' </returns>
        Public Function SearchBest(mz As Double, Optional mzdiff As Double? = Nothing) As MzIndex
            Dim query As MzIndex() = index.Search(New MzIndex(mz), tolerance:=mzdiff).ToArray

            If query.IsNullOrEmpty Then
                Return Nothing
            ElseIf query.Length = 1 Then
                Return query(0)
            Else
                Return query _
                    .OrderBy(Function(mzi) std.Abs(mzi.mz - mz)) _
                    .First
            End If
        End Function

    End Class
End Namespace
