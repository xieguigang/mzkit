Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text
Imports HeatMapPixel = Microsoft.VisualBasic.Imaging.Pixel
Imports System.Drawing

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

    Public Function GetLayer(mz As Double, Optional dims As Size = Nothing) As SingleIonLayer
        Dim spots = MzMatrix.GetLayer(Of PixelData)(m, mz, mzdiff, index)
        Dim layer As New SingleIonLayer With {
            .DimensionSize = dims,
            .IonMz = mz.ToString,
            .MSILayer = spots.ToArray
        }

        Return layer
    End Function
End Class
