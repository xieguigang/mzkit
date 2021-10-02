#Region "Microsoft.VisualBasic::8d365e6af661cef5d49a39e62e91cf61, src\visualize\MsImaging\SingleIonLayer.vb"

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

    ' Class SingleIonLayer
    ' 
    '     Properties: DimensionSize, hasZeroPixels, IonMz, MSILayer
    ' 
    '     Function: GetIntensity, (+2 Overloads) GetLayer, GetQuartile, MeasureUninSize, Take
    '               Trim
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile

Public Class SingleIonLayer

    Public Property IonMz As Double
    Public Property MSILayer As PixelData()

    ''' <summary>
    ''' the canvas size of the MSI plot output
    ''' </summary>
    ''' <returns></returns>
    Public Property DimensionSize As Size

    Public ReadOnly Property hasZeroPixels As Boolean
        Get
            Return DimensionSize.Width * DimensionSize.Height > MSILayer.Length
        End Get
    End Property

    Public Function MeasureUninSize(sampling As Integer) As Size
        Return New Size(DimensionSize.Width / sampling, DimensionSize.Height / sampling)
    End Function

    ''' <summary>
    ''' remove a polygon region from the MSI render raw data
    ''' </summary>
    ''' <param name="polygon"></param>
    ''' <param name="unionSize"></param>
    ''' <returns></returns>
    Public Function Trim(polygon As Polygon2D, unionSize As Size) As SingleIonLayer
        Dim takes As PixelData() = MSILayer _
            .TrimRegion(polygon, unionSize) _
            .Distinct _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = IonMz,
            .DimensionSize = DimensionSize,
            .MSILayer = takes
        }
    End Function

    ''' <summary>
    ''' take part of the pixels array from the current layer with given region polygon data.
    ''' </summary>
    ''' <param name="region"></param>
    ''' <param name="unionSize"></param>
    ''' <returns></returns>
    Public Function Take(region As Polygon2D, unionSize As Size) As SingleIonLayer
        Dim takes As PixelData() = MSILayer _
            .TakeRegion(region, unionSize) _
            .Distinct _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = IonMz,
            .DimensionSize = DimensionSize,
            .MSILayer = takes
        }
    End Function

    Public Function GetIntensity() As Double()
        Return MSILayer.Select(Function(p) p.intensity).ToArray
    End Function

    Public Function GetQuartile() As DataQuartile
        Return GetIntensity.Quartile
    End Function

    Public Shared Function GetLayer(mz As Double, viewer As PixelReader, mzErr As Tolerance) As SingleIonLayer
        Dim pixels As PixelData() = viewer _
            .LoadPixels({mz}, mzErr) _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = mz,
            .DimensionSize = viewer.dimension,
            .MSILayer = pixels
        }
    End Function

    Public Shared Function GetLayer(mz As Double, viewer As Drawer, mzErr As Tolerance) As SingleIonLayer
        Dim pixels As PixelData() = viewer _
            .LoadPixels({mz}, mzErr) _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = mz,
            .DimensionSize = viewer.dimension,
            .MSILayer = pixels
        }
    End Function
End Class

