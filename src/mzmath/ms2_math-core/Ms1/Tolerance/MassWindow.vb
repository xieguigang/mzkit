Imports System.Xml.Serialization
Imports std = System.Math

Namespace Ms1

    Public Interface IMassBin

        Property mass As Double
        Property min As Double
        Property max As Double

    End Interface

    ''' <summary>
    ''' the m/z bin data
    ''' </summary>
    Public Class MassWindow : Implements IMassBin

        ''' <summary>
        ''' the real mass value
        ''' </summary>
        ''' <returns></returns>
        <XmlText>
        Public Property mass As Double Implements IMassBin.mass

        ''' <summary>
        ''' the left of current mass window
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mzmin As Double Implements IMassBin.min
        ''' <summary>
        ''' the right of current mass window
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mzmax As Double Implements IMassBin.max

        Public Property annotation As String

        Sub New()
        End Sub

        Sub New(mass As Double, ppm As Double)
            Me.mass = mass

            With MzWindow(mass, ppm)
                mzmin = .lowerMz
                mzmax = .upperMz
            End With
        End Sub

        Sub New(mass As Double, mzdiff As Tolerance)
            Me.mass = mass

            If mzdiff.Type = MassToleranceType.Da Then
                mzmin = mass - mzdiff.DeltaTolerance
                mzmax = mass + mzdiff.DeltaTolerance
            Else
                With MzWindow(mass, ppm:=mzdiff.DeltaTolerance)
                    mzmin = .lowerMz
                    mzmax = .upperMz
                End With
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim ppm As Double = PPMmethod.PPM(mzmin, mzmax)

            If ppm > 100 Then
                Return $"{mass.ToString("F4")} [{std.Abs(mzmax - mzmin).ToString("F3")}da]{annotation}"
            Else
                Return $"{mass.ToString("F4")} [{CInt(ppm)}ppm]{annotation}"
            End If
        End Function

    End Class
End Namespace