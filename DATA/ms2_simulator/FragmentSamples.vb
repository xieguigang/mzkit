Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MachineLearning.StoreProcedure
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.MassSpectrum.Math.Ms1
Imports SMRUCC.MassSpectrum.Math.Spectra

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
                If tolerance.Assert(x, y) Then
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
        Dim equals As Func(Of Double, Double, Boolean) = AddressOf (tolerance Or ppm20).Assert
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

            Yield New Sample With {
                .ID = matrix.ToString,
                .target = {rt},
                .status = matrix.mz + mzSample
            }
        Next
    End Function
End Module
