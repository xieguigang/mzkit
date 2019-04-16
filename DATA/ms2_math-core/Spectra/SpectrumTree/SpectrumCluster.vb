Imports System.Runtime.CompilerServices
Imports sys = System.Math

Namespace Spectra

    Public Class SpectrumCluster : Implements IEnumerable(Of PeakMs2)

        Public Property Representative As PeakMs2
        ''' <summary>
        ''' 在这个属性之中也会通过包含有<see cref="Representative"/>代表质谱图
        ''' </summary>
        ''' <returns></returns>
        Public Property cluster As PeakMs2()

        Public ReadOnly Property Length As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return cluster.Length
            End Get
        End Property

        Public ReadOnly Property MID As String
            Get
                Return $"M{sys.Round(Representative.mz)}T{sys.Round(Representative.rt)}"
            End Get
        End Property

        Public ReadOnly Property RepresentativeFeature As String
            Get
                Return $"{Representative.file}#{Representative.scan}"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Representative.ToString & $"  with {cluster.Length} cluster members."
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of PeakMs2) Implements IEnumerable(Of PeakMs2).GetEnumerator
            For Each spectrum As PeakMs2 In cluster
                Yield spectrum
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace