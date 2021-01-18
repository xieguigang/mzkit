Imports Microsoft.VisualBasic.Math.SignalProcessing

Namespace UV

    Public Class PDAPoint : Implements ITimeSignal

        Public Property scan_time As Double Implements ITimeSignal.time
        Public Property total_ion As Double Implements ITimeSignal.intensity

        Public Shared Iterator Function FromSignal(PDA As GeneralSignal) As IEnumerable(Of PDAPoint)
            Dim x As Double() = PDA.Measures
            Dim y As Double() = PDA.Strength

            For i As Integer = 0 To PDA.Measures.Length - 1
                Yield New PDAPoint With {
                    .scan_time = x(i),
                    .total_ion = y(i)
                }
            Next
        End Function
    End Class
End Namespace