Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Pixel

    Public Class mzPackPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
            Get
                Return pixel.X
            End Get
        End Property

        Public Overrides ReadOnly Property Y As Integer
            Get
                Return pixel.Y
            End Get
        End Property

        ReadOnly scan As ScanMS1
        ReadOnly pixel As Point

        Public ReadOnly Property mz As Double()
            Get
                Return scan.mz
            End Get
        End Property

        Sub New(scan As ScanMS1)
            Me.scan = scan

            If scan.hasMetaKeys("x", "y") Then
                Me.pixel = New Point With {
                    .X = CInt(Val(scan.meta!x)),
                    .Y = CInt(Val(scan.meta!y))
                }
            Else
                Me.pixel = scan.scan_id _
                    .Match("\[\d+,\d+\]") _
                    .GetStackValue("[", "]") _
                    .DoCall(AddressOf Casting.PointParser)
            End If
        End Sub

        Public Overrides Function GetMs() As ms2()
            Return scan.GetMs.ToArray
        End Function

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Return mz _
                .Any(Function(mzi)
                         Return scan.mz.Any(Function(zzz) tolerance(zzz, mzi))
                     End Function)
        End Function

        Protected Friend Overrides Sub release()
            If Not scan Is Nothing Then
                Erase scan.into
                Erase scan.mz
                Erase scan.products
            End If
        End Sub
    End Class
End Namespace