Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Blender

    Public Class MzLayerColorSet

        Public Property mz As Double()
        Public Property colorSet As Color()
        Public Property tolerance As Tolerance

        Default Public ReadOnly Property GetColor(i As Integer) As Color
            Get
                Return colorSet(i)
            End Get
        End Property

        Public Function FindColor(mz As Double) As Color
            Dim i As Integer = which(Me.mz.Select(Function(mzi) tolerance(mz, mzi))).FirstOrDefault(-1)

            If i = -1 Then
                Return Color.Transparent
            Else
                Return colorSet(i)
            End If
        End Function

        Public Function SelectGroup(pixels As PixelData()) As IEnumerable(Of NamedCollection(Of PixelData))
            Return pixels.GroupBy(Function(p) p.mz, tolerance)
        End Function

    End Class
End Namespace