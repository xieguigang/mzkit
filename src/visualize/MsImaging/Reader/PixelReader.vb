#Region "Microsoft.VisualBasic::b016866f45bbd9faf668f5067c7fe020, E:/mzkit/src/visualize/MsImaging//Reader/PixelReader.vb"

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

    '   Total Lines: 333
    '    Code Lines: 240
    ' Comment Lines: 44
    '   Blank Lines: 49
    '     File Size: 13.92 KB


    '     Class PixelReader
    ' 
    '         Function: (+2 Overloads) FindMatchedPixels, GetIntensitySummary, (+2 Overloads) GetPixel, GetSummary, LoadLayer
    '                   LoadPixels, LoadRatioPixels, LoadRatioPixelsInternal, ReadDimensions
    ' 
    '         Sub: (+2 Overloads) Dispose
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports stdNum = System.Math

#If UNIX = 0 Then
Imports Microsoft.VisualBasic.ApplicationServices
#End If

Namespace Reader

    ''' <summary>
    ''' a unify raw data reader for MSI render
    ''' </summary>
    Public MustInherit Class PixelReader : Implements IDisposable

        Dim disposedValue As Boolean
        Dim summary As MSISummary

        Public MustOverride ReadOnly Property resolution As Double
        Public MustOverride ReadOnly Property dimension As Size
        Public MustOverride Function GetPixel(x As Integer, y As Integer) As PixelScan

        Public Shared Function ReadDimensions(raw As IEnumerable(Of Point)) As Size
            With raw.ToArray
                Return New Size(
                    width:=(Aggregate p As Point In .AsEnumerable Into Max(p.X)),
                    height:=(Aggregate p As Point In .AsEnumerable Into Max(p.Y))
                )
            End With
        End Function

        ''' <summary>
        ''' get pixels in region range
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPixel(region As Rectangle) As IEnumerable(Of PixelScan)
            Return GetPixel(region.Left, region.Top, region.Right, region.Bottom)
        End Function

        ''' <summary>
        ''' get pixels in region range
        ''' </summary>
        ''' <param name="x1"></param>
        ''' <param name="y1"></param>
        ''' <param name="x2"></param>
        ''' <param name="y2"></param>
        ''' <returns></returns>
        Public Iterator Function GetPixel(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer) As IEnumerable(Of PixelScan)
            For i As Integer = x1 To x2
                For j As Integer = y1 To y2
                    Dim pixel As PixelScan = GetPixel(i, j)

                    If Not pixel Is Nothing Then
                        Yield pixel
                    End If
                Next
            Next
        End Function

        Public Function GetSummary() As MSISummary
            If summary Is Nothing Then
                Dim rows = AllPixels _
                    .GroupBy(Function(p) p.Y) _
                    .ToArray _
                    .AsParallel _
                    .Select(Function(r)
                                Return (r.Key, GetIntensitySummary(r))
                            End Function) _
                    .OrderBy(Function(r) r.Key) _
                    .Select(Function(r) r.Item2) _
                    .ToArray

                summary = New MSISummary With {
                    .rowScans = rows,
                    .size = dimension
                }
            End If

            Return summary
        End Function

        Private Function GetIntensitySummary(r As IEnumerable(Of PixelScan)) As iPixelIntensity()
            Return r _
                .Where(Function(i) i.HasAnyMzIon) _
                .Select(Function(p)
                            Dim matrix = p.GetMs
                            Dim into As Double() = matrix.Select(Function(i) i.intensity).ToArray
                            Dim mz As Double() = matrix.Select(Function(i) i.mz).ToArray

                            Return New iPixelIntensity With {
                                .average = into.Average,
                                .basePeakIntensity = into.Max,
                                .basePeakMz = mz(which.Max(into)),
                                .totalIon = into.Sum,
                                .x = p.X,
                                .y = p.Y,
                                .median = into.Median,
                                .min = into.Min,
                                .numIons = into.Length
                            }
                        End Function) _
                .ToArray
        End Function

        Protected MustOverride Sub release()

        Public MustOverride Function AllPixels() As IEnumerable(Of PixelScan)

        Public Iterator Function FindMatchedPixels(mz As Double, tolerance As Tolerance) As IEnumerable(Of PixelScan)
            Dim allpixels As PixelScan() = Me.AllPixels.ToArray

            For Each pixel As PixelScan In allpixels
                If pixel.GetMzIonIntensity(mz, tolerance) > 0 Then
                    Yield pixel
                End If
            Next
        End Function

        Public Iterator Function FindMatchedPixels(mz As Double(), tolerance As Tolerance) As IEnumerable(Of PixelScan)
            Dim allpixels As PixelScan() = Me.AllPixels.ToArray

            For Each pixel As PixelScan In allpixels
                If pixel.HasAnyMzIon(mz, tolerance) Then
                    Yield pixel
                End If
            Next
        End Function

        Public Function LoadLayer(mz As Double, mzdiff As Tolerance) As SingleIonLayer
            Dim pixels As New List(Of PixelData)
            Dim into As Double

            For Each pixel As PixelScan In AllPixels()
                into = pixel.GetMzIonIntensity(mz, mzdiff)

                If into > 0 Then
                    Call New PixelData With {
                        .mz = mz,
                        .intensity = into,
                        .level = 0,
                        .x = pixel.X,
                        .y = pixel.Y
                    }.DoCall(AddressOf pixels.Add)
                End If
            Next

            Return New SingleIonLayer With {
                .DimensionSize = dimension,
                .IonMz = mz.ToString("F4"),
                .MSILayer = pixels.ToArray
            }
        End Function

        Public Iterator Function LoadRatioPixels(mz1 As Double, mz2 As Double, tolerance As Tolerance,
                                                 Optional skipZero As Boolean = True,
                                                 Optional polygonFilter As Point() = Nothing) As IEnumerable(Of PixelData)

            Dim all = LoadRatioPixelsInternal(mz1, mz2, tolerance, skipZero, polygonFilter).ToArray
            Dim amin = all.Where(Function(i) i.a > 0).Select(Function(i) i.a).Min / 2
            Dim bmin = all.Where(Function(i) i.b > 0).Select(Function(i) i.b).Min / 2

            For Each p In all
                Dim a As Double = If(p.a = 0.0, amin, p.a)
                Dim b As Double = If(p.b = 0.0, bmin, p.b)

                Yield New PixelData With {
                    .intensity = stdNum.Log(a / b, 2),
                    .x = p.x,
                    .y = p.y
                }
            Next
        End Function

        Private Iterator Function LoadRatioPixelsInternal(mz1 As Double, mz2 As Double, tolerance As Tolerance,
                                                          Optional skipZero As Boolean = True,
                                                          Optional polygonFilter As Point() = Nothing) As IEnumerable(Of (x As Integer, y As Integer, a As Double, b As Double))

            Dim filter As Index(Of String) = Nothing

            If Not polygonFilter.IsNullOrEmpty Then
                filter = polygonFilter _
                    .Select(Function(p) $"{p.X},{p.Y}") _
                    .Indexing
            End If

            Dim twoMz As Double() = {mz1, mz2}

            For Each point As PixelScan In FindMatchedPixels(twoMz, tolerance) _
                .Where(Function(p)
                           If filter Is Nothing Then
                               Return True
                           Else
                               Return $"{p.X},{p.Y}" Like filter
                           End If
                       End Function)

                Dim msScan As ms2() = point.GetMs
                Dim into As NamedCollection(Of ms2)() = msScan _
                    .Where(Function(mzi)
                               Return twoMz.Any(Function(dmz) tolerance(mzi.mz, dmz))
                           End Function) _
                    .GroupBy(Function(a) a.mz, tolerance) _
                    .ToArray

                If skipZero AndAlso into.Length = 0 Then
                    Continue For
                Else
                    Dim a = into.Where(Function(i) tolerance(mz1, Val(i.name))).Select(Function(i) i.value).IteratesALL.Select(Function(i) i.intensity).ToArray
                    Dim b = into.Where(Function(i) tolerance(mz2, Val(i.name))).Select(Function(i) i.value).IteratesALL.Select(Function(i) i.intensity).ToArray

                    Yield (point.X, point.Y, If(a.Length = 0, 0.0, a.Max), If(b.Length = 0, 0.0, b.Max))
                End If
            Next
        End Function

        ''' <summary>
        ''' load pixels data for match a given list of m/z ions with tolerance
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <param name="tolerance"></param>
        ''' <param name="skipZero"></param>
        ''' <param name="polygonFilter">
        ''' Only select the pixels in this polygon
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function LoadPixels(mz As Double(), tolerance As Tolerance,
                                            Optional skipZero As Boolean = True,
                                            Optional polygonFilter As Point() = Nothing) As IEnumerable(Of PixelData)
            Dim pixel As PixelData
            Dim filter As Index(Of String) = Nothing

            If Not polygonFilter.IsNullOrEmpty Then
                filter = polygonFilter _
                    .Select(Function(p) $"{p.X},{p.Y}") _
                    .Indexing
            End If

            Dim rawMatches = FindMatchedPixels(mz, tolerance).ToArray
            Dim filterMatches = rawMatches _
                .Where(Function(p)
                           If filter Is Nothing Then
                               Return True
                           Else
                               Return $"{p.X},{p.Y}" Like filter
                           End If
                       End Function) _
                .ToArray
            Dim mzbin As Tolerance = Tolerance.DeltaMass(0.01)

            ' group the mzbins always by a smaller tolerance 
            If mzbin.GetErrorPPM > tolerance.GetErrorPPM Then
                mzbin = tolerance
            End If

            For Each point As PixelScan In filterMatches
                Dim msScan As ms2() = point.GetMs
                Dim into As NamedCollection(Of ms2)() = msScan _
                    .Where(Function(m) m.intensity > 0) _
                    .Where(Function(mzi)
                               Return mz.Any(Function(dmz) tolerance(mzi.mz, dmz))
                           End Function) _
                    .GroupBy(Function(a) a.mz, mzbin) _
                    .ToArray

                If skipZero AndAlso into.Length = 0 Then
                    Continue For
                Else
                    Dim tag As String = point.sampleTag
                    Dim max As ms2

                    ' populate each m/z hit which has matched 
                    ' with the given tolerance value
                    For Each mzi As NamedCollection(Of ms2) In into
                        max = mzi.OrderByDescending(Function(t) t.intensity).First
                        pixel = New PixelData With {
                            .x = point.X,
                            .y = point.Y,
                            .mz = max.mz,
                            .sampleTag = tag,
                            .intensity = max.intensity
                        }

                        Yield pixel
                    Next
                End If
            Next
        End Function

        ''' <summary>
        ''' load all ions m/z in the raw data file
        ''' </summary>
        ''' <param name="ppm"></param>
        ''' <returns></returns>
        Public MustOverride Function LoadMzArray(ppm As Double) As Double()

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call release()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并替代终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
