Imports System
Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential), CLSCompliant(True)>
Public Structure udtFTLabelInfoType
    Public Mass As Double
    Public Intensity As Double
    Public Resolution As Single
    Public Baseline As Single
    Public Noise As Single
    Public Charge As Integer
    Public ReadOnly Property SignalToNoise As Double
        Get
            If (Me.Noise > 0!) Then
                Return (Me.Intensity / CDbl(Me.Noise))
            End If
            Return 0
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("m/z {0,9:F3}, S/N {1,7:F1}, intensity {2,12:F0}", Me.Mass, Me.SignalToNoise, Me.Intensity)
    End Function
End Structure
