#Region "Microsoft.VisualBasic::a723b944a08b7a9fd966ed34a293bd6a, mzkit\src\visualize\MsImaging\Extensions.vb"

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

    '   Total Lines: 236
    '    Code Lines: 178
    ' Comment Lines: 30
    '   Blank Lines: 28
    '     File Size: 8.25 KB


    ' Module Extensions
    ' 
    '     Function: DensityCut, GetPixelKeys, (+2 Overloads) PixelScanPadding, Reset, ScanMeltdown
    '               Shape
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.DataMining.DensityQuery
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Point = System.Drawing.Point

<HideModuleName>
Public Module Extensions

    ''' <summary>
    ''' reset sample position
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Reset(raw As mzPack) As mzPack
        Dim rect As Rectangle = raw.Shape
        Dim scans As New List(Of ScanMS1)
        Dim pos As Point
        Dim meta As Dictionary(Of String, String)

        For Each scan As ScanMS1 In raw.MS
            pos = scan.GetMSIPixel
            pos = New Point With {
                .X = pos.X - rect.Left,
                .Y = pos.Y - rect.Top
            }
            meta = New Dictionary(Of String, String)(scan.meta)
            meta("x") = pos.X.ToString
            meta("y") = pos.Y.ToString

            scans += New ScanMS1 With {
                .BPC = scan.BPC,
                .into = scan.into,
                .mz = scan.mz,
                .products = scan.products,
                .rt = scan.rt,
                .TIC = scan.TIC,
                .scan_id = scan.scan_id,
                .meta = meta
            }
        Next

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging,
            .Chromatogram = raw.Chromatogram,
            .MS = scans.ToArray,
            .Scanners = raw.Scanners,
            .source = $"reset({raw.source})",
            .Thumbnail = Nothing
        }
    End Function

    ''' <summary>
    ''' get pixels boundary of the MSImaging
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Shape(raw As mzPack) As Rectangle
        Dim allPixels As Point() = raw.MS.Select(Function(scan) scan.GetMSIPixel).ToArray
        Dim top = Aggregate pt As Point In allPixels Into Min(pt.Y)
        Dim left = Aggregate pt As Point In allPixels Into Min(pt.X)
        Dim right = Aggregate pt As Point In allPixels Into Max(pt.X)
        Dim bottom = Aggregate pt As Point In allPixels Into Max(pt.Y)
        Dim rect As New Rectangle With {
            .X = left,
            .Y = top,
            .Width = right - left,
            .Height = bottom - top
        }

        Return rect
    End Function

    ''' <summary>
    ''' parse pixel mapping from 
    ''' </summary>
    ''' <returns>
    ''' [xy => index]
    ''' </returns>
    <Extension>
    Public Function GetPixelKeys(raw As IMzPackReader) As Dictionary(Of String, String())
        Return raw.EnumerateIndex _
            .Select(Function(id)
                        Dim meta = raw.GetMetadata(id)
                        Dim pxy = $"{meta!x},{meta!y}"

                        Return (id, pxy)
                    End Function) _
            .GroupBy(Function(t) t.pxy) _
            .ToDictionary(Function(t) t.Key,
                            Function(t)
                                Return t.Select(Function(i) i.id).ToArray
                            End Function)
    End Function

    <Extension>
    Public Function PixelScanPadding(raw As mzPack, padding As Padding) As mzPack
        Dim dims As Size = PixelReader.ReadDimensions(raw.MS.Select(Function(scan) scan.GetMSIPixel))
        Dim paddingData As ScanMS1() = raw.MS.PixelScanPadding(padding, dims).ToArray

        Return New mzPack With {
            .MS = paddingData,
            .Application = FileApplicationClass.MSImaging,
            .Chromatogram = raw.Chromatogram,
            .Scanners = raw.Scanners,
            .source = raw.source,
            .Thumbnail = raw.Thumbnail,
            .metadata = raw.metadata
        }
    End Function

    <Extension>
    Private Iterator Function PixelScanPadding(raw As IEnumerable(Of ScanMS1), padding As Padding, dims As Size) As IEnumerable(Of ScanMS1)
        Dim marginRight As Integer = dims.Width - padding.Right
        Dim marginLeft As Integer = padding.Left
        Dim marginTop As Integer = padding.Top
        Dim marginBottom As Integer = dims.Height - padding.Bottom

        For Each sample As ScanMS1 In raw
            Dim pxy As Point = sample.GetMSIPixel

            If pxy.X < marginLeft Then
                Continue For
            ElseIf pxy.X > marginRight Then
                Continue For
            ElseIf pxy.Y < marginTop Then
                Continue For
            ElseIf pxy.Y > marginBottom Then
                Continue For
            End If

            Yield sample
        Next
    End Function

    ''' <summary>
    ''' removes the pixel points by the average density cutoff
    ''' </summary>
    ''' <param name="layer"></param>
    ''' <param name="qcut"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function DensityCut(layer As IEnumerable(Of PixelData), Optional qcut As Double = 0.1) As IEnumerable(Of PixelData)
        Dim raw As PixelData() = layer.ToArray
        Dim densityList = raw _
            .Density(Function(p) $"{p.x},{p.y}", Function(p) p.x, Function(p) p.y, New Size(5, 5)) _
            .OrderByDescending(Function(d)
                                   Return d.Value
                               End Function) _
            .ToArray
        Dim q As New FastRankQuantile(densityList.Select(Function(d) d.Value))

        qcut = q.Query(qcut)

        Dim pid As Index(Of String) = (From d As NamedValue(Of Double)
                                       In densityList
                                       Where d.Value >= qcut
                                       Select d.Name).Indexing

        For Each point As PixelData In raw
            If $"{point.x},{point.y}" Like pid Then
                Yield point
            End If
        Next
    End Function

    ''' <summary>
    ''' make bugs fixed for RT pixel correction
    ''' </summary>
    ''' <param name="MSI"></param>
    ''' <param name="println">
    ''' set this parameter value to nothing will mute the log message echo. 
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function ScanMeltdown(MSI As mzPack,
                                 Optional gridSize As Integer = 2,
                                 Optional println As Action(Of String) = Nothing) As mzPack

        Dim graph = Grid(Of ScanMS1).Create(MSI.MS, Function(d) d.GetMSIPixel)
        Dim scans As New List(Of ScanMS1)
        Dim dims As New Size(graph.width, graph.height)
        Dim pixel As ScanMS1

        If println Is Nothing Then
            println =
                Sub()

                End Sub
        Else
            Call println("make bugs fixed for RT pixel correction!")
        End If

        For i As Integer = 1 To dims.Width
            For j As Integer = 1 To dims.Height
                pixel = graph.GetData(i, j)

                If pixel Is Nothing Then
                    For xi As Integer = -1 To -gridSize Step -1
                        pixel = graph.GetData(i + xi, j)

                        If Not pixel Is Nothing Then
                            Exit For
                        End If
                    Next
                End If

                If Not pixel Is Nothing Then
                    scans.Add(pixel)
                Else
                    Call println($"Missing pixel data at [{i}, {j}]!")
                End If
            Next
        Next

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging,
            .Chromatogram = MSI.Chromatogram,
            .MS = scans.ToArray,
            .Scanners = MSI.Scanners,
            .source = MSI.source,
            .Thumbnail = MSI.Thumbnail
        }
    End Function
End Module
