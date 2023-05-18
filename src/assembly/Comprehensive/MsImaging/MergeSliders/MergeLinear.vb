Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public MustInherit Class MergeLinear

    Protected ReadOnly relativePos As Boolean,
        norm As Boolean,
        println As Action(Of String)

    Sub New(relativePos As Boolean,
            norm As Boolean,
            println As Action(Of String))

        Me.relativePos = relativePos
        Me.norm = norm
        Me.println = println
    End Sub

    Public MustOverride Function JoinOneSample(shape As Polygon2D,
                                               sample As mzPack,
                                               left As Integer,
                                               top As Integer) As IEnumerable(Of ScanMS1)

End Class

Public Class MergeSMSlides : Inherits MergeLinear

    Public Sub New(relativePos As Boolean, norm As Boolean, println As Action(Of String))
        MyBase.New(relativePos, norm, println)
    End Sub

    Public Overrides Iterator Function JoinOneSample(shape As Polygon2D, sample As mzPack, left As Integer, top As Integer) As IEnumerable(Of ScanMS1)
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
                Yield scan.generateNormScan(minX, left, deltaY, sampleid, norm)
            Else
                ' is absolute position, just merge the collection
                Yield scan
            End If
        Next
    End Function
End Class

Public Class MergeSTSlides : Inherits MergeLinear

    ''' <summary>
    ''' a global union id list set
    ''' </summary>
    ReadOnly unionGeneIds As Index(Of String)

    Public Sub New(relativePos As Boolean, norm As Boolean, println As Action(Of String), unionGeneIds As Index(Of String))
        MyBase.New(relativePos, norm, println)
        Me.unionGeneIds = unionGeneIds
    End Sub

    Public Overrides Iterator Function JoinOneSample(shape As Polygon2D, sample As mzPack, left As Integer, top As Integer) As IEnumerable(Of ScanMS1)
        Dim minX As Integer = shape.xpoints.Min
        Dim height As Integer = shape.height
        Dim deltaY As Integer = shape.ypoints.Min * -1 + top
        Dim sampleid As String = sample.source
        Dim mapping As Dictionary(Of String, String) = sample.Annotations
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

            ' the m/z value in STimaging data set is actually the gene id index
            ' needs to union across multiple raw data pack files
            ' 1. restore to the gene id list at first
            Dim geneIdsInScan As String() = scan.mz.Select(Function(i) mapping(CInt(i).ToString)).ToArray
            ' 2. and then alignment to the global union idset
            Dim union_maps As Double() = geneIdsInScan _
                .Select(Function(id) CDbl(unionGeneIds.IndexOf(id))) _
                .ToArray

            ' 3. finally, save back to the ms scan object
            scan.mz = union_maps

            If relativePos Then
                Yield scan.generateNormScan(minX, left, deltaY, sampleid, norm)
            Else
                ' is absolute position, just merge the collection
                Yield scan
            End If
        Next
    End Function
End Class