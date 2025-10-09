Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Namespace Spectra

    ''' <summary>
    ''' the spectrum fragment matches method 
    ''' </summary>
    Public Module Matches

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sortMz">
        ''' data must be sorted by the <see cref="ms2.mz"/> fragment mass value in asc order.
        ''' </param>
        ''' <param name="mz"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        <Extension>
        Public Function BinarySearch(sortMz As ms2(), mz As Double, tolerance As Tolerance) As ms2

        End Function

    End Module
End Namespace