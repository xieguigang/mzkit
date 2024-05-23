#Region "Microsoft.VisualBasic::8e3193edb715bfb4da8962ac8ee75d4f, assembly\Comprehensive\MsImaging\MergeSliders\MergeLinear.vb"

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

    '   Total Lines: 138
    '    Code Lines: 93 (67.39%)
    ' Comment Lines: 18 (13.04%)
    '    - Xml Docs: 16.67%
    ' 
    '   Blank Lines: 27 (19.57%)
    '     File Size: 5.42 KB


    ' Class MergeLinear
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' Class MergeSMSlides
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: JoinOneSample, Padding
    ' 
    ' Class MergeSTSlides
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: JoinOneSample, Padding
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection
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

    Public MustOverride Function Padding() As Size

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
                Yield scan.generateNormScan(minX, left, deltaY, sampleid)
            Else
                ' is absolute position, just merge the collection
                Yield scan
            End If
        Next
    End Function

    Public Overrides Function Padding() As Size
        Return New Size(30, 30)
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
                Yield scan.generateNormScan(minX, left, deltaY, sampleid)
            Else
                ' is absolute position, just merge the collection
                Yield scan
            End If
        Next
    End Function

    Public Overrides Function Padding() As Size
        Return New Size(0, 0)
    End Function
End Class
