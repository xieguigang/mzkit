#Region "Microsoft.VisualBasic::1a3ff9e966dd8a06def929c886bc5528, E:/mzkit/src/visualize/MsImaging//Layer/SingleIonLayer.vb"

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

    '   Total Lines: 253
    '    Code Lines: 155
    ' Comment Lines: 69
    '   Blank Lines: 29
    '     File Size: 8.65 KB


    ' Class SingleIonLayer
    ' 
    '     Properties: DimensionSize, hasMultipleSamples, hasZeroPixels, IonMz, Item
    '                 maxinto, MSILayer, sampleTags, size
    ' 
    '     Function: GetIntensity, (+3 Overloads) GetLayer, GetQuartile, IntensityCutoff, MeasureUninSize
    '               Take, (+2 Overloads) ToString, Trim
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile

''' <summary>
''' a collection of the <see cref="PixelData"/> spots, the spot pixel
''' data was used to the spatial heatmap rendering.
''' </summary>
Public Class SingleIonLayer

    ''' <summary>
    ''' the layer tag label, usually be a single ion m/z value.
    ''' </summary>
    ''' <returns></returns>
    Public Property IonMz As String
    ''' <summary>
    ''' the spatial spot data collection, that used for create spatial heatmap rendering.
    ''' </summary>
    ''' <returns></returns>
    Public Property MSILayer As PixelData()

    ''' <summary>
    ''' the canvas size of the MSI plot output, it indicates the max scan x and 
    ''' max scan y of the <see cref="MSILayer"/> heatmap.
    ''' </summary>
    ''' <returns></returns>
    Public Property DimensionSize As Size

    ''' <summary>
    ''' check is there some missing spot data inside the <see cref="MSILayer"/> by 
    ''' simply measuring the collection size.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property hasZeroPixels As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return DimensionSize.Area > size
        End Get
    End Property

    ''' <summary>
    ''' get the max intensity value inside the <see cref="MSILayer"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property maxinto As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            If MSILayer.IsNullOrEmpty Then
                Return 0
            End If

            Return Aggregate p As PixelData
                   In MSILayer
                   Into Max(p.intensity)
        End Get
    End Property

    ''' <summary>
    ''' Get by pixels
    ''' </summary>
    ''' <param name="xy">x,y</param>
    ''' <returns></returns>
    Public ReadOnly Property Item(xy As String()) As SingleIonLayer
        Get
            Dim xyIndex As Index(Of String) = xy.Indexing
            Dim pixels As PixelData() = MSILayer _
                .Where(Function(p)
                           Return $"{p.x},{p.y}" Like xyIndex
                       End Function) _
                .ToArray

            Return New SingleIonLayer With {
                .DimensionSize = DimensionSize,
                .IonMz = IonMz,
                .MSILayer = pixels
            }
        End Get
    End Property

    ''' <summary>
    ''' check if the spot data has multiple sample label data tags
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property hasMultipleSamples As Boolean
        Get
            Return sampleTags.Length > 1
        End Get
    End Property

    ''' <summary>
    ''' get the sample tags of the spatial spots
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property sampleTags As String()
        Get
            Return MSILayer _
                .Select(Function(a) a.sampleTag) _
                .Distinct _
                .ToArray
        End Get
    End Property

    ''' <summary>
    ''' count the number of the pixel spots in current metabolite 
    ''' ion ms-imaging layer.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property size As Integer
        Get
            If MSILayer.IsNullOrEmpty Then
                Return 0
            Else
                Return MSILayer.Length
            End If
        End Get
    End Property

    ''' <summary>
    ''' Removes pixels which relative intensity value is 
    ''' less than the given <paramref name="intocutoff"/> 
    ''' threshold.
    ''' </summary>
    ''' <param name="intocutoff">
    ''' relative intensity cutoff value in range ``[0,1]``.
    ''' </param>
    ''' <returns></returns>
    Public Function IntensityCutoff(intocutoff As Double) As SingleIonLayer
        Dim maxinto As Double = Me.maxinto

        Return New SingleIonLayer With {
            .DimensionSize = DimensionSize,
            .IonMz = IonMz,
            .MSILayer = MSILayer _
                .Where(Function(p)
                           Return p.intensity / maxinto >= intocutoff
                       End Function) _
                .ToArray
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"({MSILayer.Length} pixels) {ToString(Me)}"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Function ToString(ion As SingleIonLayer) As String
        Return If(ion.IonMz.IsNumeric, $"m/z {Double.Parse(ion.IonMz).ToString("F4")}", ion.IonMz)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
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

    ''' <summary>
    ''' get intensity value from all spot data inside current layer object
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetIntensity() As Double()
        Return MSILayer.Select(Function(p) p.intensity).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetQuartile() As DataQuartile
        Return GetIntensity.Quartile
    End Function

    Public Shared Function GetLayer(mz As Double(), viewer As PixelReader, mzErr As Tolerance) As SingleIonLayer
        Dim pixels As PixelData() = viewer _
            .LoadPixels(mz, mzErr) _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = mz.FirstOrDefault.ToString("F4"),
            .DimensionSize = viewer.dimension,
            .MSILayer = pixels
        }
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

    Public Shared Function GetLayer(mz As Double(), viewer As Drawer, mzErr As Tolerance) As SingleIonLayer
        Dim pixels As PixelData() = viewer _
            .LoadPixels(mz, mzErr) _
            .ToArray

        Return New SingleIonLayer With {
            .IonMz = If(mz.Length = 1, mz(0).ToString, mz.Select(Function(d) d.ToString("F4")).JoinBy("+")),
            .DimensionSize = viewer.dimension,
            .MSILayer = pixels
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Narrowing Operator CType(ion As SingleIonLayer) As PixelData()
        Return If(ion Is Nothing, Nothing, ion.MSILayer)
    End Operator
End Class
