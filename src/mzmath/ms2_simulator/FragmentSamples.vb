#Region "Microsoft.VisualBasic::9007fe1596949edad562a9f230ffbfb6, ms2_simulator\FragmentSamples.vb"

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

    ' Module FragmentSamples
    ' 
    '     Function: GetFragmentsMz, productTree, RtSamples
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MachineLearning.StoreProcedure
Imports Microsoft.VisualBasic.Math.Quantile

Public Module FragmentSamples

    ''' <summary>
    ''' Get all unique product m/z values
    ''' </summary>
    ''' <param name="ms2"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetFragmentsMz(ms2 As IEnumerable(Of LibraryMatrix), Optional tolerance As Tolerance = Nothing) As Double()
        Dim productMz As NaiveBinaryTree(Of Double, Double) = (tolerance Or ppm20).productTree

        For Each ion As LibraryMatrix In ms2
            For Each mz As ms2 In ion
                Call productMz.insert(mz.mz, mz.mz, append:=False)
            Next
        Next

        Return productMz _
            .GetAllNodes _
            .Select(Function(mz) mz.Key) _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function productTree(tolerance As Tolerance) As NaiveBinaryTree(Of Double, Double)
        Return New NaiveBinaryTree(Of Double, Double)(
            Function(x, y)
                If tolerance.Equals(x, y) Then
                    Return 0
                ElseIf x > y Then
                    Return 1
                Else
                    Return -1
                End If
            End Function)
    End Function

    ''' <summary>
    ''' Create samples data for ML training or rt evaluation
    ''' </summary>
    ''' <param name="ms2"></param>
    ''' <param name="fragments#"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function RtSamples(ms2 As IEnumerable(Of PeakMs2), fragments#(), Optional tolerance As Tolerance = Nothing) As IEnumerable(Of Sample)
        Dim rt#
        ' parent/mz, product/mz intensity
        Dim mzSample As New List(Of Double)
        Dim equals As GenericLambda(Of Double).IEquals = tolerance Or ppm20
        Dim into As ms2()

        fragments = fragments _
            .OrderBy(Function(x) x) _
            .ToArray

        For Each matrix As PeakMs2 In ms2
            rt = matrix.rt
            mzSample *= 0

            For Each indexMz As Double In fragments
                into = matrix.mzInto _
                    .Where(Function(m) equals(m.mz, indexMz)) _
                    .ToArray

                If into.Length = 0 Then
                    mzSample += 0
                Else
                    mzSample += into _
                        .Select(Function(m) m.intensity) _
                        .Quartile _
                        .ModelSamples _
                        .normal _
                        .Average
                End If
            Next

            Yield New Sample(matrix.mz + mzSample) With {
                .ID = matrix.ToString,
                .target = {rt}
            }
        Next
    End Function
End Module
