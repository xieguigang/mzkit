#Region "Microsoft.VisualBasic::dd030fe61fb75b5409d8c852f38c8d00, G:/mzkit/src/visualize/MsImaging//IndexedCache/MatrixReader.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 56
    '    Code Lines: 40
    ' Comment Lines: 7
    '   Blank Lines: 9
    '     File Size: 1.91 KB


    ' Class MatrixReader
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ForEachLayer, GetLayer, GetSpots, MeasureDimension
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute

''' <summary>
''' A spatial matrix reader
''' </summary>
Public Class MatrixReader

    ReadOnly m As MzMatrix
    ReadOnly index As MzPool
    ReadOnly mzdiff As Tolerance
    ReadOnly dims As Size

    Sub New(m As MzMatrix)
        Me.m = m
        Me.mzdiff = Tolerance.ParseScript(m.tolerance)
        Me.index = New MzPool(m.mz)
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

