Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Linq

Namespace Spectra

    ''' <summary>
    ''' module for check LC-MS in-source fragment result
    ''' </summary>
    Public Class CheckInSourceFragments

        ''' <summary>
        ''' index of rt
        ''' </summary>
        ReadOnly peakdata As BlockSearchFunction(Of (rt As Double, ms2 As Double()))
        ReadOnly mzdiff As Tolerance

        Sub New(data As IEnumerable(Of PeakMs2), Optional da As Double = 0.1)
            Dim peakdata = data _
                .Select(Function(p) (p.rt, p.mzInto.Select(Function(mi) mi.mz).ToArray)) _
                .ToArray

            Me.mzdiff = Tolerance.DeltaMass(da)
            Me.peakdata = New BlockSearchFunction(Of (rt As Double, ms2 As Double()))(
                data:=peakdata,
                eval:=Function(a) a.rt,
                tolerance:=5,
                fuzzy:=True
            )
        End Sub

        ''' <summary>
        ''' Check the given ms1 ion is generated from the in-source fragment result?
        ''' </summary>
        ''' <param name="mz">the m/z of the target ms1 ion</param>
        ''' <param name="rt">the rt of the target ms1 ion</param>
        ''' <returns>
        ''' this function returns value TRUE means the given ms1 information 
        ''' is possible ab in-source fragment data
        ''' </returns>
        Public Function CheckOfFragments(mz As Double, rt As Double, Optional rt_win As Double = 5) As Boolean
            ' get the ms2 products scan around the given rt
            Dim ms2Cols = peakdata _
                .Search((rt, Nothing), tolerance:=rt_win) _
                .Where(Function(s) s.rt <= rt) _
                .ToArray
            ' and then get all product fragments m/z
            Dim mz2 As Double() = ms2Cols.Select(Function(mi) mi.ms2).IteratesALL.ToArray
            ' search for the ms1 m/z in the result product fragments
            Dim checkProduct As Boolean = mz2.Any(Function(mzi) mzdiff(mzi, mz))

            Return checkProduct
        End Function

        ''' <summary>
        ''' Check the given ms1 ion is generated from the in-source fragment result?
        ''' </summary>
        ''' <param name="ms1">
        ''' the target ms1 ion data for check in-source fragment
        ''' </param>
        ''' <returns>
        ''' this function returns value TRUE means the given ms1 information 
        ''' is possible ab in-source fragment data
        ''' </returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CheckOfFragments(ms1 As ms1_scan, Optional rt_win As Double = 5) As Boolean
            Return CheckOfFragments(ms1.mz, ms1.scan_time, rt_win)
        End Function
    End Class
End Namespace