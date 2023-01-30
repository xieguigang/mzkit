Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module MergeSliders

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
                                   Optional println As Action(Of String) = Nothing) As mzPack
        Dim polygons = samples _
            .Select(Function(ms)
                        Call println($"load {ms.source}!")
                        Return (ms, New Polygon2D(ms.MS.Select(Function(a) a.GetMSIPixel)))
                    End Function) _
            .ToArray
        Dim maxHeight As Integer = polygons _
            .Select(Function(a) a.Item2.ypoints) _
            .IteratesALL _
            .Max
        Dim left As Integer = polygons.First.Item2.xpoints.Min
        Dim union As New List(Of ScanMS1)
        Dim mzmin As New List(Of Double)
        Dim mzmax As New List(Of Double)
        Dim res As New List(Of Double)

        ' for each sample mzpack object
        ' do sample join
        For Each sample As (Ms As mzPack, shape As Polygon2D) In polygons
            Call union.JoinOneSample(
                shape:=sample.shape,
                sample:=sample.Ms,
                left:=left,
                top:=0,
                relativePos:=relativePos,
                norm:=norm,
                println:=println
            )
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

    <Extension>
    Public Sub JoinOneSample(union As List(Of ScanMS1),
                             shape As Polygon2D,
                             sample As mzPack,
                             left As Integer,
                             top As Integer,
                             relativePos As Boolean,
                             norm As Boolean,
                             println As Action(Of String))

        Dim minX As Integer = shape.xpoints.Min
        Dim height As Integer = shape.height
        Dim deltaY As Integer = shape.ypoints.Min * -1 + top
        Dim sampleid As String = sample.source

        ' 20230119 the previous spot normalize is not working as expected
        ' so do sample normalized based on the TIC of the entire sample data
        ' at here
        Dim totalIons As Double = Aggregate a As ScanMS1
                                  In sample.MS
                                  Let spot_TIC As Double = a.into.Sum
                                  Into Sum(spot_TIC)

        Const level As Double = 10.0 ^ 8
        Call println(" >>> " & sampleid)

        For Each scan As ScanMS1 In From s As ScanMS1
                                    In sample.MS
                                    Where Not s.into.IsNullOrEmpty

            If norm Then
                ' do normalized of current spot sample
                scan.into = New Vector(scan.into) / totalIons * level
            End If

            If relativePos Then
                union.Add(scan.generateNormScan(minX, left, deltaY, sampleid, norm))
            Else
                ' is absolute position, just merge the collection
                union.Add(scan)
            End If
        Next
    End Sub

    ''' <summary>
    ''' adjust of the sample spot location
    ''' </summary>
    ''' <param name="scan"></param>
    ''' <param name="minX"></param>
    ''' <param name="left"></param>
    ''' <param name="deltaY"></param>
    ''' <param name="sampleid"></param>
    ''' <param name="norm"></param>
    ''' <returns></returns>
    <Extension>
    Private Function generateNormScan(scan As ScanMS1,
                                      minX As Integer,
                                      left As Integer,
                                      deltaY As Double,
                                      sampleid As String,
                                      norm As Boolean) As ScanMS1

        Dim meta As New Dictionary(Of String, String)(scan.meta)
        Dim xy = scan.GetMSIPixel
        Dim x As Integer = xy.X - minX + left
        Dim y As Integer = deltaY + xy.Y
        ' 20221013 try to avoid the duplicated scan id
        ' confliction in data merge by adding a prefix
        ' of the source tag
        Dim scan_id As String = $"{sampleid} - {scan.scan_id}"
        'Dim normInto As New Vector(scan.into)

        'If norm Then
        '    normInto = (normInto / normInto.Sum) * (10 ^ 8)
        'End If

        meta!x = x
        meta!y = y
        meta.Remove("X")
        meta.Remove("Y")

        If Not meta.ContainsKey("sample") Then
            meta.Add("sample", sampleid)
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
