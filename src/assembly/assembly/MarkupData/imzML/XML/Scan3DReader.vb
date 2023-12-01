Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace MarkupData.imzML

    Public Class Scan3DReader

        ReadOnly ibd As ibdReader
        ReadOnly xyz As ScanData3D

        Public ReadOnly Property x As Double
            Get
                Return xyz.x
            End Get
        End Property

        Public ReadOnly Property y As Double
            Get
                Return xyz.y
            End Get
        End Property

        Public ReadOnly Property z As Double
            Get
                Return xyz.z
            End Get
        End Property

        Sub New(scan As ScanData3D, ibd As ibdReader)
            Me.xyz = scan
            Me.ibd = ibd
        End Sub

        Public Function LoadMsData() As ms2()
            Dim mz As Double() = ibd.ReadArray(xyz.MzPtr)
            Dim intensity As Double() = ibd.ReadArray(xyz.IntPtr)
            Dim scanData As New List(Of ms2)

            For i As Integer = 0 To mz.Length - 1
                If intensity(i) > 0 Then
                    Call scanData.Add(New ms2 With {
                        .mz = mz(i),
                        .intensity = intensity(i)
                    })
                End If
            Next

            Return scanData.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return $"{ibd.UUID} {MyBase.ToString}"
        End Function

    End Class

    Public Class PointF3D : Implements Imaging.PointF3D

        Public Property X As Double Implements Imaging.PointF3D.X
        Public Property Y As Double Implements Imaging.PointF3D.Y
        Public Property Z As Double Implements Imaging.PointF3D.Z

    End Class
End Namespace