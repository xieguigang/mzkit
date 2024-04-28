#Region "Microsoft.VisualBasic::3d52bb0017b5d17254a59d3a84e325b5, E:/mzkit/src/assembly/Comprehensive//MsImaging/MergeSliders/MergeSliders.vb"

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

    '   Total Lines: 170
    '    Code Lines: 116
    ' Comment Lines: 34
    '   Blank Lines: 20
    '     File Size: 6.42 KB


    ' Module MergeSliders
    ' 
    '     Function: generateNormScan, JoinMSISamples, PullPolygons
    ' 
    '     Sub: MoveLayout
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Public Module MergeSliders

    <Extension>
    Friend Iterator Function PullPolygons(samples As IEnumerable(Of mzPack), println As Action(Of String)) As IEnumerable(Of (ms As mzPack, polygon As Polygon2D))
        For Each ms As mzPack In samples
            Call println($"load {ms.source}!")
            Yield (ms, New Polygon2D(ms.MS.Select(Function(a) a.GetMSIPixel)))
        Next
    End Function

    ''' <summary>
    ''' Merge multiple sample object into one sample file
    ''' </summary>
    ''' <param name="samples">
    ''' put the samples from left to right based on the 
    ''' data orders in this input sequence.
    ''' </param>
    ''' <param name="relativePos">
    ''' this parameter is set to True by default, which 
    ''' means all of the scan position will be adjusted 
    ''' automatically based on its input orders
    ''' </param>
    ''' <param name="norm">
    ''' do intensity data normalization for each scan data?
    ''' default is yes!
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function JoinMSISamples(samples As IEnumerable(Of mzPack),
                                   Optional relativePos As Boolean = True,
                                   Optional padding As Integer = 20,
                                   Optional norm As Boolean = True,
                                   Optional println As Action(Of String) = Nothing,
                                   Optional mergeLinear As MergeLinear = Nothing) As mzPack

        ' load polygon shape for each imaging slider
        Dim polygons = samples.PullPolygons(println).ToArray
        Dim maxHeight As Integer = polygons _
            .Select(Function(a) a.Item2.ypoints) _
            .IteratesALL _
            .Max
        Dim left As Integer = polygons.First.Item2.xpoints.Min
        Dim union As New List(Of ScanMS1)
        Dim mzmin As New List(Of Double)
        Dim mzmax As New List(Of Double)
        Dim res As New List(Of Double)

        If mergeLinear Is Nothing Then
            mergeLinear = New MergeSMSlides(relativePos, norm, println)
        End If

        ' for each sample mzpack object
        ' do sample join
        For Each sample As (Ms As mzPack, shape As Polygon2D) In polygons
            Call mergeLinear.JoinOneSample(
                shape:=sample.shape,
                sample:=sample.Ms,
                left:=left,
                top:=0
            ).DoCall(AddressOf union.AddRange)

            left += padding * 2 + (
                sample.shape.xpoints.Max - sample.shape.xpoints.Min
            )

            With sample.Ms.MS _
                .Select(Function(i) i.mz) _
                .IteratesALL _
                .ToArray

                If .Length > 0 Then
                    Call mzmin.Add(.Min)
                    Call mzmax.Add(.Max)
                End If
            End With

            If Not sample.Ms.metadata.IsNullOrEmpty Then
                res.Add(sample.Ms.metadata.TryGetValue("resolution", [default]:=17))
            End If
        Next

        Dim poly As New Polygon2D(union.Select(Function(i) i.GetMSIPixel))
        Dim metadata As New Metadata With {
            .[class] = FileApplicationClass.MSImaging,
            .mass_range = New DoubleRange(mzmin.Min, mzmax.Max),
            .resolution = If(res.IsNullOrEmpty, 17, res.Average),
            .scan_x = poly.width + padding,
            .scan_y = poly.height + padding
        }

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging.ToString,
            .source = polygons _
                .Select(Function(i) i.ms.source) _
                .JoinBy("+"),
            .MS = union.ToArray,
            .metadata = metadata.GetMetadata
        }
    End Function

    Public Sub MoveLayout(<Out> ByRef x As Integer,
                          <Out> ByRef y As Integer,
                          minX As Integer,
                          left As Integer,
                          deltaY As Integer)

        x = x - minX + left
        y = deltaY + y
    End Sub

    ''' <summary>
    ''' adjust of the sample spot location
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <param name="minX"></param>
    ''' <param name="left"></param>
    ''' <param name="deltaY"></param>
    ''' <param name="sampleid"></param>
    ''' <returns></returns>
    <Extension>
    Friend Function generateNormScan(scan As ScanMS1,
                                     minX As Integer,
                                     left As Integer,
                                     deltaY As Double,
                                     sampleid As String) As ScanMS1

        Dim meta As New Dictionary(Of String, String)(scan.meta)
        Dim xy = scan.GetMSIPixel
        Dim x As Integer = xy.X
        Dim y As Integer = xy.Y
        ' 20221013 try to avoid the duplicated scan id
        ' confliction in data merge by adding a prefix
        ' of the source tag
        Dim scan_id As String = $"{sampleid} - {scan.scan_id}"

        Call MoveLayout(x, y, minX, left, deltaY)

        meta!x = x
        meta!y = y
        meta.Remove("X")
        meta.Remove("Y")

        If Not meta.ContainsKey(mzStreamWriter.SampleMetaName) Then
            Call meta.Add(mzStreamWriter.SampleMetaName, sampleid)
        ElseIf meta(mzStreamWriter.SampleMetaName).StringEmpty Then
            meta(mzStreamWriter.SampleMetaName) = sampleid
        End If

        ' the location of current pixel must be
        ' adjusted use the relative location.
        Return New ScanMS1 With {
            .meta = meta,
            .BPC = scan.BPC,
            .into = scan.into, ' normInto.ToArray,
            .mz = scan.mz,
            .products = scan.products,
            .rt = scan.rt,
            .scan_id = scan_id,
            .TIC = scan.TIC
        }
    End Function
End Module
