Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
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
        Dim maxHeight As Integer = polygons.Select(Function(a) a.Item2.ypoints).IteratesALL.Max
        Dim left As Integer = polygons.First.Item2.xpoints.Min
        Dim union As New List(Of ScanMS1)

        For Each sample As (Ms As mzPack, shape As Polygon2D) In polygons
            union.JoinOneSample(shape:=sample.shape, sample:=sample.Ms, left, relativePos, norm, println)
            left += padding * 2 + (sample.shape.xpoints.Max - sample.shape.xpoints.Min)
        Next

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging,
            .source = polygons _
                .Select(Function(i) i.ms.source) _
                .JoinBy("+"),
            .MS = union.ToArray
        }
    End Function

    <Extension>
    Private Sub JoinOneSample(union As List(Of ScanMS1),
                              shape As Polygon2D,
                              sample As mzPack,
                              left As Integer,
                              relativePos As Boolean,
                              norm As Boolean,
                              println As Action(Of String))

        Dim minX As Integer = shape.xpoints.Min
        Dim height As Integer = shape.height
        Dim deltaY As Integer = shape.ypoints.Min * -1
        Dim sampleid As String = sample.source

        Call println(" >>> " & sampleid)

        For Each scan As ScanMS1 In sample.MS.Where(Function(s) Not s.into.IsNullOrEmpty)
            If relativePos Then
                union.Add(scan.generateNormScan(minX, left, deltaY, sampleid, norm))
            Else
                ' is absolute position, just merge the collection
                union.Add(scan)
            End If
        Next
    End Sub

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
        Dim normInto As New Vector(scan.into)

        If norm Then
            normInto = (normInto / normInto.Sum) * (10 ^ 8)
        End If

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
            .into = normInto.ToArray,
            .mz = scan.mz,
            .products = scan.products,
            .rt = scan.rt,
            .scan_id = scan_id,
            .TIC = scan.TIC
        }
    End Function
End Module
