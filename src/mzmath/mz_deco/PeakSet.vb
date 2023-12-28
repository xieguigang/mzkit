Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports std = System.Math

''' <summary>
''' A collection of the <see cref="xcms2"/> peak features data
''' </summary>
Public Class PeakSet

    Dim m_peaksdata As xcms2()
    Dim mz As BlockSearchFunction(Of (mz As Double, Integer))
    Dim rt As BlockSearchFunction(Of (mz As Double, Integer))

    ''' <summary>
    ''' the ROI peaks data
    ''' </summary>
    ''' <returns></returns>
    Public Property peaks As xcms2()
        Get
            Return m_peaksdata
        End Get
        Set(value As xcms2())
            m_peaksdata = value
            makeIndex()
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

        Me.mz = mz.CreateMzIndex(win_size:=1.5)
        Me.rt = rt.CreateMzIndex(win_size:=60)
    End Sub

    ''' <summary>
    ''' get XIC data
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    Public Iterator Function FilterMz(mz As Double, mzdiff As Double) As IEnumerable(Of xcms2)
        For Each hit As (mz As Double, Integer) In Me.mz.Search((mz, -1))
            If hit.Item2 > -1 AndAlso std.Abs(hit.mz - mz) <= mzdiff Then
                Yield m_peaksdata(hit.Item2)
            End If
        Next
    End Function

    Public Iterator Function FilterRt(rt As Double, rt_win As Double) As IEnumerable(Of xcms2)
        For Each hit As (rt As Double, Integer) In Me.rt.Search((rt, -1))
            If hit.Item2 > -1 AndAlso std.Abs(hit.rt - rt) <= rt_win Then
                Yield m_peaksdata(hit.Item2)
            End If
        Next
    End Function

    Public Iterator Function FindIonSet(mz As Double, rt As Double, mzdiff As Double, rt_win As Double) As IEnumerable(Of xcms2)
        Dim mzset = Me.mz.Search((mz, -1)).AsParallel _
            .Where(Function(i) i.Item2 > -1 AndAlso std.Abs(i.mz - mz) <= mzdiff) _
            .ToArray
        Dim rtset = Me.rt.Search((rt, -1)).AsParallel _
            .Where(Function(i) i.Item2 > -1 AndAlso std.Abs(i.Item1 - rt) <= rt_win) _
            .ToArray
        Dim intersect_offsets = mzset.Select(Function(a) a.Item2) _
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
    Public Function Norm() As PeakSet
        Return New PeakSet With {
            .peaks = peaks _
                .Select(Function(pk) pk.totalPeakSum) _
                .ToArray
        }
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

End Class