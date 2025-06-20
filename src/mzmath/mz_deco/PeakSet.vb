#Region "Microsoft.VisualBasic::30882171cc9d138ca1cc3ef0b5c1099a, mzmath\mz_deco\PeakSet.vb"

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

    '   Total Lines: 223
    '    Code Lines: 147 (65.92%)
    ' Comment Lines: 51 (22.87%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 25 (11.21%)
    '     File Size: 7.69 KB


    ' Class PeakSet
    ' 
    '     Properties: annotations, peaks, ROIs, sampleNames
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: FilterMz, FilterRt, FindIonSet, GenericEnumerator, GetById
    '               Norm, SetPeakSet, (+2 Overloads) Subset, ToFeatures, ToString
    ' 
    '     Sub: makeIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports std = System.Math

''' <summary>
''' A collection of the <see cref="xcms2"/> peak features data
''' </summary>
Public Class PeakSet : Implements Enumeration(Of xcms2)

    Dim m_peaksdata As xcms2()
    Dim m_peakindex As Dictionary(Of String, xcms2)
    Dim mz As MzPool
    Dim rt As MzPool

    ''' <summary>
    ''' the ROI peaks data
    ''' </summary>
    ''' <returns></returns>
    Public Property peaks As xcms2()
        Get
            Return m_peaksdata
        End Get
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Set(value As xcms2())
            Call SetPeakSet(value)
        End Set
    End Property

    ''' <summary>
    ''' the samples names in current ROI peak set
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property sampleNames As String()

    ''' <summary>
    ''' get number of the ROI peaks in current dataset
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ROIs As Integer
        Get
            Return peaks.TryCount
        End Get
    End Property

    ''' <summary>
    ''' annotation of the part of the <see cref="peaks"/>
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' the hash key of the dictionary is the <see cref="xcms2.ID"/>.
    ''' </remarks>
    Public Property annotations As Dictionary(Of String, MetID)

    Public Sub New()
    End Sub

    Sub New(peakset As IEnumerable(Of xcms2))
        Call SetPeakSet(peakset.SafeQuery.ToArray)
    End Sub

    Public Function SetPeakSet(peakset As xcms2()) As PeakSet
        m_peaksdata = peakset
        makeIndex()
        Return Me
    End Function

    Private Sub makeIndex()
        _sampleNames = peaks _
            .SafeQuery _
            .AsParallel _
            .Select(Function(pk) pk.Properties.Keys.ToArray) _
            .IteratesALL _
            .Distinct _
            .ToArray

        Dim mz As Double() = New Double(peaks.Length - 1) {}
        Dim rt As Double() = New Double(peaks.Length - 1) {}

        For i As Integer = 0 To m_peaksdata.Length - 1
            mz(i) = m_peaksdata(i).mz
            rt(i) = m_peaksdata(i).rt
        Next

        Me.mz = New MzPool(mz, win_size:=1.5)
        Me.rt = New MzPool(rt, win_size:=60)
        Me.m_peakindex = peaks.ToDictionary(Function(a) a.ID)
    End Sub

    ''' <summary>
    ''' try to get a peak by its unique reference id
    ''' </summary>
    ''' <param name="xcms_id"></param>
    ''' <returns>null value will be returns if the given 
    ''' <paramref name="xcms_id"/> is not existed inside
    ''' index.</returns>
    Public Function GetById(xcms_id As String) As xcms2
        Return m_peakindex.TryGetValue(xcms_id)
    End Function

    ''' <summary>
    ''' get XIC data
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    Public Iterator Function FilterMz(mz As Double, mzdiff As Double) As IEnumerable(Of xcms2)
        For Each hit As MzIndex In Me.mz.Search(mz)
            If hit.index > -1 AndAlso std.Abs(hit.mz - mz) <= mzdiff Then
                Yield m_peaksdata(hit.index)
            End If
        Next
    End Function

    Public Iterator Function FilterRt(rt As Double, rt_win As Double) As IEnumerable(Of xcms2)
        For Each hit As (rt As Double, index As Integer) In Me.rt.Query(rt)
            If hit.index > -1 AndAlso std.Abs(hit.rt - rt) <= rt_win Then
                Yield m_peaksdata(hit.index)
            End If
        Next
    End Function

    ''' <summary>
    ''' Make grid search of the ion ROI features via a given [mz,rt] coordinate point data
    ''' within the given mz/rt window size.
    ''' </summary>
    ''' <remarks>
    ''' this function will return a set of the <see cref="xcms2"/> object
    ''' that is inside the given mz/rt window size.
    ''' </remarks>
    ''' <example>
    ''' Dim ionset As IEnumerable(Of xcms2) = peakset.FindIonSet(mz, rt, mzdiff:=0.005, rt_win:=0.5)
    ''' </example>
    ''' <param name="mz"></param>
    ''' <param name="rt"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="rt_win"></param>
    ''' <returns></returns>
    Public Iterator Function FindIonSet(mz As Double, rt As Double, mzdiff As Double, rt_win As Double) As IEnumerable(Of xcms2)
        Dim mzset = Me.mz.Search(mz).AsParallel _
            .Where(Function(i) i.index > -1 AndAlso std.Abs(i.mz - mz) <= mzdiff) _
            .ToArray
        Dim rtset = Me.rt.Query(rt).AsParallel _
            .Where(Function(i) i.Item2 > -1 AndAlso std.Abs(i.Item1 - rt) <= rt_win) _
            .ToArray
        Dim intersect_offsets = mzset.Select(Function(a) a.index) _
            .Intersect(rtset.Select(Function(b) b.Item2)) _
            .ToArray

        For Each i As Integer In intersect_offsets
            Yield m_peaksdata(i)
        Next
    End Function

    Public Overrides Function ToString() As String
        Return sampleNames.GetJson
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Norm(Optional scale As Double = 10 ^ 8) As PeakSet
        Return New PeakSet With {.peaks = xcms2.TotalPeakSum(peaks, scale).ToArray}
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function Subset(pk As xcms2, sampleNames As String()) As xcms2
        Return New xcms2 With {
            .ID = pk.ID,
            .mz = pk.mz,
            .mzmax = pk.mzmax,
            .mzmin = pk.mzmin,
            .rt = pk.rt,
            .rtmax = pk.rtmax,
            .rtmin = pk.rtmin,
            .Properties = sampleNames _
                .ToDictionary(Function(name) name,
                                Function(name)
                                    Return pk(name)
                                End Function)
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function SampleVector(sampleName As String) As Double()
        Return m_peaksdata _
            .Select(Function(roi) roi(sampleName)) _
            .ToArray
    End Function

    Public Function Subset(sampleNames As String()) As PeakSet
        Dim subpeaks As xcms2() = peaks _
            .Select(Function(pk)
                        Return Subset(pk, sampleNames)
                    End Function) _
            .ToArray

        Return New PeakSet With {
            .peaks = subpeaks
        }
    End Function

    Public Iterator Function ToFeatures() As IEnumerable(Of PeakFeature)
        For Each peak As xcms2 In m_peaksdata
            For Each sample In peak.Properties
                Yield New PeakFeature With {
                    .area = sample.Value,
                    .baseline = 0,
                    .integration = sample.Value,
                    .maxInto = sample.Value,
                    .mz = peak.mz,
                    .noise = 0,
                    .nticks = 0,
                    .rawfile = sample.Key,
                    .RI = peak.RI,
                    .rt = peak.rt,
                    .rtmax = peak.rtmax,
                    .rtmin = peak.rtmin,
                    .xcms_id = peak.ID
                }
            Next
        Next
    End Function

    Private Iterator Function GenericEnumerator() As IEnumerator(Of xcms2) Implements Enumeration(Of xcms2).GenericEnumerator
        For Each peak As xcms2 In m_peaksdata.SafeQuery
            Yield peak
        Next
    End Function
End Class
