Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ComponentModel.Algorithm

''' <summary>
''' A spatial matrix reader
''' </summary>
Public Class MatrixReader

    ReadOnly m As MzMatrix
    ReadOnly index As BlockSearchFunction(Of (mz As Double, Integer))
    ReadOnly mzdiff As Tolerance

    Sub New(m As MzMatrix)
        Me.m = m
        Me.mzdiff = Tolerance.ParseScript(m.tolerance)
        Me.index = m.mz.CreateMzIndex
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSpots(mz As Double) As IEnumerable(Of PixelData)
        Return MzMatrix.GetLayer(Of PixelData)(m, mz, mzdiff, index)
    End Function

    Public Function GetLayer(mz As Double, Optional dims As Size = Nothing) As SingleIonLayer
        Dim spots As PixelData() = GetSpots(mz).ToArray
        Dim layer As New SingleIonLayer With {
            .DimensionSize = dims,
            .IonMz = mz.ToString,
            .MSILayer = spots
        }

        Return layer
    End Function
End Class
