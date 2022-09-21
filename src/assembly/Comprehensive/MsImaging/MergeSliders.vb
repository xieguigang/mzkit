Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

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
    ''' <returns></returns>
    <Extension>
    Public Function JoinMSISamples(samples As IEnumerable(Of mzPack),
                                   Optional relativePos As Boolean = True,
                                   Optional padding As Integer = 10) As mzPack
        Dim polygons = samples _
            .Select(Function(ms) (ms, New Polygon2D(ms.MS.Select(Function(a) a.GetMSIPixel)))) _
            .ToArray
        Dim maxHeight As Integer = polygons.Select(Function(a) a.Item2.ypoints).IteratesALL.Max
        Dim left As Integer = polygons.First.Item2.xpoints.Min + padding
        Dim union As New List(Of ScanMS1)

        For Each sample As (Ms As mzPack, shape As Polygon2D) In polygons
            Dim minX = sample.shape.xpoints.Min
            Dim height As Double = sample.shape.height
            Dim deltaY As Double = (maxHeight - height) / 2
            Dim sampleid As String = sample.Ms.source

            For Each scan As ScanMS1 In sample.Ms.MS
                If relativePos Then
                    Dim meta As New Dictionary(Of String, String)(scan.meta)
                    Dim xy = scan.GetMSIPixel
                    Dim x As Integer = xy.X - minX + left
                    Dim y As Integer = deltaY + xy.Y

                    meta!x = x
                    meta!y = y
                    meta.Remove("X")
                    meta.Remove("Y")

                    If Not meta.ContainsKey("sample") Then
                        meta.Add("sample", sampleid)
                    End If

                    ' the location of current pixel must be
                    ' adjusted use the relative location.
                    scan = New ScanMS1 With {
                        .meta = meta,
                        .BPC = scan.BPC,
                        .into = scan.into,
                        .mz = scan.mz,
                        .products = scan.products,
                        .rt = scan.rt,
                        .scan_id = scan.scan_id,
                        .TIC = scan.TIC
                    }
                    union.Add(scan)
                Else
                    ' is absolute position, just merge the collection
                    union.Add(scan)
                End If
            Next

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
End Module
