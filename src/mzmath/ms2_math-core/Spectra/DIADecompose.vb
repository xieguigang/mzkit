#Region "Microsoft.VisualBasic::0f125e204e9bfccb0de3807551967bd7, mzmath\ms2_math-core\Spectra\DIADecompose.vb"

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

'   Total Lines: 103
'    Code Lines: 73 (70.87%)
' Comment Lines: 14 (13.59%)
'    - Xml Docs: 92.86%
' 
'   Blank Lines: 16 (15.53%)
'     File Size: 4.22 KB


' Module DIADecompose
' 
'     Function: DecomposeSpectrum
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.SIMD
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class DIADecompose

    ReadOnly rowPacks As New List(Of Double())
    ReadOnly fragments As MzPool
    ReadOnly tqdm As Boolean = True
    ReadOnly specPool As PeakMs2()

    Sub New(spectrum As IEnumerable(Of PeakMs2), Optional tqdm As Boolean = True)
        Dim specPool As New List(Of PeakMs2)
        Dim fragmentSet As New List(Of Double)

        For Each spec As PeakMs2 In spectrum
            Call specPool.Add(spec)
            Call fragmentSet.AddRange(spec.mzInto.Select(Function(a) a.mz))
        Next

        Me.fragments = fragmentSet.CreateCentroidFragmentSet(window_size:=0.75, verbose:=tqdm)
        Me.tqdm = tqdm
        Me.specPool = specPool.ToArray

        Dim fragment_size As Integer = fragments.size

        For Each spec As PeakMs2 In specPool
            Dim mz As Double() = spec.mzInto.Select(Function(i) i.mz).ToArray
            Dim into As Double() = spec.mzInto.Select(Function(i) i.intensity).ToArray

            into = Divide.f64_op_divide_f64_scalar(into, into.Max)
            into = Multiply.f64_scalar_op_multiply_f64(100, into)

            Call rowPacks.Add(SpectraEncoder.DeconvoluteScan(mz, into, fragment_size, fragments))
        Next
    End Sub

    Dim w, h As NumericMatrix

    ''' <summary>
    ''' make the spectrum set decompose into multiple spectrum groups via the NMF method
    ''' </summary>
    ''' <param name="n"></param>
    ''' <param name="maxItrs"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="eps"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' NOTE: this decompose method may returns empty collection due to the reason of one of 
    ''' the collection its weight maybe all smaller than given cutoff.
    ''' </remarks>
    Public Iterator Function DecomposeSpectrum(n As Integer,
                                               Optional maxItrs As Integer = 1000,
                                               Optional tolerance As Double = 0.001,
                                               Optional eps As Double = 0.0001) As IEnumerable(Of NamedCollection(Of PeakMs2))
        Dim decompose = NMF.Factorisation(
            A:=New NumericMatrix(rowPacks),
            k:=n,
            max_iterations:=maxItrs,
            tolerance:=tolerance,
            epsilon:=eps,
            tqdm:=tqdm)
        Dim mzSet As Double() = fragments.ionSet

        w = decompose.W
        h = decompose.H

        ' produce the decompose spectrum set
        For i As Integer = 0 To n - 1
            Dim factor = H(i)
            Dim decomposer As New List(Of PeakMs2)
            Dim sum As Double() = New Double(mzSet.Length - 1) {}

            For j As Integer = 0 To rowPacks.Count - 1
                Dim offset = W(j)
                Dim weight = offset(i)

                If weight < 0.0001 Then
                    Continue For
                End If

                Dim intensity = (factor * rowPacks(j)) * weight
                Dim ms2 As ms2() = mzSet _
                    .Select(Function(mzi, k) New ms2(mzi, intensity(k))) _
                    .Where(Function(a) a.intensity > 0.001) _
                    .ToArray
                Dim spectral As New PeakMs2(specPool(j)) With {
                    .mzInto = ms2,
                    .lib_guid = specPool(j).lib_guid & $"_decompose_{i + 1}"
                }

                sum = Add.f64_op_add_f64(sum, (intensity / intensity.Max).Array)

                Call decomposer.Add(spectral)
            Next

            Yield New NamedCollection(Of PeakMs2) With {
                .name = $"decompose_{i + 1}",
                .value = decomposer.ToArray,
                .description = mzSet _
                    .Select(Function(mzi, k) New ms2(mzi, sum(k))) _
                    .Where(Function(a) a.intensity > 0) _
                    .ToArray _
                    .GetJson
            }
        Next
    End Function

End Class

