#Region "Microsoft.VisualBasic::5ef9c1a55af8de4c14316575ceb1d5c0, mzmath\MoleculeNetworking\Consensus.vb"

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

    '   Total Lines: 58
    '    Code Lines: 43 (74.14%)
    ' Comment Lines: 4 (6.90%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 11 (18.97%)
    '     File Size: 2.11 KB


    ' Class Consensus
    ' 
    '     Properties: intensity, mz, ratio
    ' 
    '     Function: Measure
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.SplashID
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SIMD

''' <summary>
''' Evaluate the consensus fragment peaks
''' </summary>
Public Class Consensus

    Public Property mz As Double
    Public Property intensity As Double
    Public Property ratio As Double

    Public Shared Iterator Function Measure(Of T As ISpectrum)(cluster As IEnumerable(Of T)) As IEnumerable(Of Consensus)
        ' upstream data maybe contains null value
        Dim total = (From spec As T
                     In cluster.SafeQuery
                     Where Not spec Is Nothing).ToArray

        If total.Length = 0 Then
            Return
        End If

        Dim mzSet As MzPool = total.Select(Function(a) a.GetIons) _
            .IteratesALL _
            .Select(Function(i) i.mz) _
            .CreateCentroidFragmentSet(window_size:=0.75, verbose:=False)
        Dim rowPacks As New List(Of Double())
        Dim fragment_size = mzSet.size

        For Each spec As T In total
            Dim ions = spec.GetIons.ToArray
            Dim mz As Double() = ions.Select(Function(i) i.mz).ToArray
            Dim into As Double() = ions.Select(Function(i) i.intensity).ToArray

            into = Divide.f64_op_divide_f64_scalar(into, into.Max)

            Call rowPacks.Add(SpectraEncoder.DeconvoluteScan(mz, into, fragment_size, mzSet))
        Next

        For i As Integer = 0 To fragment_size - 1
            Dim offset As Integer = i
            Dim v = rowPacks.Select(Function(a) a(offset)).ToArray
            Dim rank As Double = v.Sum / rowPacks.Count * 100
            Dim hits As Integer = v.Where(Function(a) a > 0).Count

            If hits > 1 Then
                Yield New Consensus With {
                    .mz = mzSet(i),
                    .intensity = rank,
                    .ratio = hits / rowPacks.Count
                }
            End If
        Next
    End Function

End Class

