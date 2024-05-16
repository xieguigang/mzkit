#Region "Microsoft.VisualBasic::af4c4d077760776b0d5998e1522930eb, mzmath\ms2_math-core\Spectra\CheckInSourceFragments.vb"

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

    '   Total Lines: 71
    '    Code Lines: 35
    ' Comment Lines: 28
    '   Blank Lines: 8
    '     File Size: 2.95 KB


    '     Class CheckInSourceFragments
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) CheckOfFragments
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
