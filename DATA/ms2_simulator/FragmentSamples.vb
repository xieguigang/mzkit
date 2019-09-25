Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
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
        Dim offset As Func(Of Double, Double, Boolean) = AddressOf (tolerance Or ppm20).Assert
        Dim productMz As New NaiveBinaryTree(Of Double, Double)(
            Function(x, y)
                If offset(x, y) Then
                    Return 0
                ElseIf x > y Then
                    Return 1
                Else
                    Return -1
                End If
            End Function)

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

End Module
