#Region "Microsoft.VisualBasic::8424885cb49620da3963836ff10e11f4, visualize\MsImaging\Analysis\IonStat.vb"

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

    '   Total Lines: 233
    '    Code Lines: 120 (51.50%)
    ' Comment Lines: 95 (40.77%)
    '    - Xml Docs: 94.74%
    ' 
    '   Blank Lines: 18 (7.73%)
    '     File Size: 10.20 KB


    ' Class IonStat
    ' 
    '     Properties: averageIntensity, basePixelX, basePixelY, density, entropy
    '                 maxIntensity, moran, mz, mzmax, mzmin
    '                 mzwidth, pixels, pvalue, Q1Intensity, Q2Intensity
    '                 Q3Intensity, rsd, sparsity
    ' 
    '     Function: (+4 Overloads) DoStat
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.StatsMath
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.ApplicationServices.Plugin
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Point = System.Drawing.Point
Imports std = System.Math

''' <summary>
''' Stats the ion features inside a MSI raw data slide
''' </summary>
Public Class IonStat

    ''' <summary>
    ''' the ion m/z value of current ms-imaging layer feature
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F4")> Public Property mz As Double
    ''' <summary>
    ''' the min range value of current ion m/z
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F4")> Public Property mzmin As Double
    ''' <summary>
    ''' the max range value of current ion m/z
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F4")> Public Property mzmax As Double
    ''' <summary>
    ''' the description text of the mz range: mzmax - mzmin
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <DisplayName("mz.diff")>
    Public Property mzwidth As String

    ''' <summary>
    ''' the total pixel number of current ion m/z occurs.
    ''' </summary>
    ''' <returns></returns>
    <Category("Spatial")> Public Property pixels As Integer
    ''' <summary>
    ''' the average spatial density
    ''' </summary>
    ''' <returns></returns>
    <Category("Spatial")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F2")>
    Public Property density As Double

    ''' <summary>
    ''' the max intensity value of current ion across all pixels
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <DisplayName("max.into")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property maxIntensity As Double

    ''' <summary>
    ''' the average intensity value of current ion across all pixels
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <DisplayName("mean.into")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property averageIntensity As Double

    ''' <summary>
    ''' the x axis position of the pixel which has the max intensity value of current ion layer
    ''' </summary>
    ''' <returns></returns>
    <DisplayName("basepeak.x")>
    <Category("Spatial")> Public Property basePixelX As Integer
    ''' <summary>
    ''' the y axis position of the pixel which has the max intensity value of current ion layer
    ''' </summary>
    ''' <returns></returns>
    <DisplayName("basepeak.y")>
    <Category("Spatial")> Public Property basePixelY As Integer

    ''' <summary>
    ''' the intensity value of quartile Q1 level(25% quantile)
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <DisplayName("intensity(Q1)")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property Q1Intensity As Double
    ''' <summary>
    ''' the intensity value of quartile Q2 level(median value, 50% quantile)
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <DisplayName("intensity(Q2)")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property Q2Intensity As Double
    ''' <summary>
    ''' the intensity value of quartile Q3 level(75% quantile)
    ''' </summary>
    ''' <returns></returns>
    <Category("MSdata")> <DisplayName("intensity(Q3)")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G5")>
    Public Property Q3Intensity As Double

    Public Property rsd As Double
    Public Property entropy As Double
    Public Property sparsity As Double

    ''' <summary>
    ''' Moran-I index value of current ion layer geometry data
    ''' 
    ''' In statistics, Moran's I is a measure of spatial autocorrelation developed by Patrick Alfred Pierce Moran.
    ''' Spatial autocorrelation is characterized by a correlation in a signal among nearby locations in space. 
    ''' Spatial autocorrelation is more complex than one-dimensional autocorrelation because spatial correlation 
    ''' is multi-dimensional (i.e. 2 or 3 dimensions of space) and multi-directional.
    ''' </summary>
    ''' <returns></returns>
    <Category("Spatial")> <DisplayName("moran I")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("F3")>
    Public Property moran As Double

    ''' <summary>
    ''' the Moran-I test p-value
    ''' </summary>
    ''' <returns></returns>
    <Category("Spatial")> <DisplayName("moran p-value")>
    <TypeConverter(GetType(FormattedDoubleConverter)), FormattedDoubleFormatString("G4")>
    Public Property pvalue As Double

    ''' <summary>
    ''' extract the ion features from the pixel data collection
    ''' </summary>
    ''' <param name="allPixels"></param>
    ''' <param name="nsize"></param>
    ''' <param name="da"></param>
    ''' <param name="mz"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    Public Shared Function DoStat(allPixels As PixelScan(),
                                  Optional nsize As Integer = 5,
                                  Optional da As Double = 0.05,
                                  Optional mz As Double() = Nothing,
                                  Optional parallel As Boolean = False) As IEnumerable(Of IonStat)
        Dim ionList = allPixels _
            .Select(Function(i)
                        Dim pt As New Point(i.X, i.Y)
                        Dim ions = i.GetMsPipe.Select(Function(ms) New PixelData(pt, ms))
                        Return ions.ToArray
                    End Function) _
            .ToArray
        Dim total_spots As Integer = allPixels.Length

        If Not mz.IsNullOrEmpty Then
            Dim allHits = ionList _
                .AsParallel _
                .Select(Function(sp)
                            Return sp _
                                .Where(Function(i)
                                           Return mz.Any(Function(mzi) std.Abs(mzi - i.mz) <= da)
                                       End Function) _
                                .ToArray
                        End Function) _
                .DoCall(Function(allIons) allIons.DoStatInternal(nsize, da,
                                                                 total_spots:=total_spots,
                                                                 parallel:=parallel)) _
                .ToList

            For Each mzi As Double In mz
                If allHits.Count = 0 OrElse Not allHits.All(Function(m) std.Abs(m.mz - mzi) <= da) Then
                    ' missing current ion
                    ' fill empty
                    Call allHits.Add(New IonStat With {.mz = mzi})
                End If
            Next

            Return allHits
        Else
            Return ionList.DoStatInternal(nsize, da,
                                          total_spots:=total_spots,
                                          parallel:=parallel)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="nsize">
    ''' the grid size for evaluate the density value
    ''' </param>
    ''' <returns></returns>
    Public Shared Function DoStat(raw As mzPack,
                                  Optional nsize As Integer = 5,
                                  Optional da As Double = 0.05,
                                  Optional parallel As Boolean = False) As IEnumerable(Of IonStat)
        Return raw.MS _
            .AsParallel _
            .Select(Iterator Function(scan) As IEnumerable(Of PixelData)
                        Dim xy As Point = scan.GetMSIPixel

                        For Each ms1 As ms2 In scan.GetMs
                            Yield New PixelData(xy, ms1)
                        Next
                    End Function) _
            .Select(Function(p) p.ToArray) _
            .DoCall(Function(allIons)
                        Call VBDebugger.EchoLine("start to pull all pixel data from the raw data pack...")
                        Return allIons.DoStatInternal(nsize, da,
                                                      total_spots:=raw.MS.Length,
                                                      parallel:=parallel)
                    End Function)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="nsize">
    ''' the grid cell size for evaluate the pixel density
    ''' </param>
    ''' <returns></returns>
    Public Shared Function DoStat(layer As SingleIonLayer, Optional nsize As Integer = 5) As IonStat
        Dim ion As New NamedCollection(Of PixelData)(layer.IonMz, layer.MSILayer)
        Dim stats As IonStat = ion.DoStatSingleIon(nsize,
                                                   total_spots:=layer.DimensionSize.Area,
                                                   parallel:=True)
        Return stats
    End Function

    Public Shared Function DoStat(rawdata As MzMatrix, Optional grid_size As Integer = 5, Optional parallel As Boolean = True) As IEnumerable(Of IonStat)
        Return rawdata.DoStat(grid_size, parallel)
    End Function
End Class
