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
    ReadOnly dims As Size

    Sub New(m As MzMatrix)
        Me.m = m
        Me.mzdiff = Tolerance.ParseScript(m.tolerance)
        Me.index = m.mz.CreateMzIndex
        Me.dims = MeasureDimension()
    End Sub

    ''' <summary>
    ''' Try to measure the ms-imaging dimension size based on the spatial spot information
    ''' </summary>
    ''' <returns></returns>
    Private Function MeasureDimension() As Size
        Dim w = Aggregate pi In m.matrix Into Max(pi.X)
        Dim h = Aggregate pi In m.matrix Into Max(pi.Y)

        Return New Size(w, h)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSpots(mz As Double) As IEnumerable(Of PixelData)
        Return MzMatrix.GetLayer(Of PixelData)(m, mz, mzdiff, index)
    End Function

    Public Function GetLayer(mz As Double, Optional dims As Size = Nothing) As SingleIonLayer
        Dim spots As PixelData() = GetSpots(mz).ToArray
        Dim layer As New SingleIonLayer With {
            .DimensionSize = If(dims.IsEmpty, Me.dims, dims),
            .IonMz = mz.ToString,
            .MSILayer = spots
        }

        Return layer
    End Function

    Public Iterator Function ForEachLayer(ions As IEnumerable(Of Double), Optional dims As Size = Nothing) As IEnumerable(Of SingleIonLayer)
        For Each mzi As Double In ions
            Yield GetLayer(mzi, dims)
        Next
    End Function
End Class
