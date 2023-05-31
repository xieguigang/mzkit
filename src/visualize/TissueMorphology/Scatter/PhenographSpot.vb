#Region "Microsoft.VisualBasic::91a0ef791bb512ea061c2a468bd7e39e, mzkit\src\visualize\TissueMorphology\Scatter\PhenographSpot.vb"

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

    '   Total Lines: 36
    '    Code Lines: 24
    ' Comment Lines: 4
    '   Blank Lines: 8
    '     File Size: 1.25 KB


    ' Class PhenographSpot
    ' 
    '     Properties: id, phenograph_cluster
    ' 
    '     Function: GetPixel, GetSpotColorIndex, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Public Class PhenographSpot : Implements IPoint2D, INamedValue

    ''' <summary>
    ''' the spot pixel id, in string format of ``[x,y]``.
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String Implements INamedValue.Key
    Public Property phenograph_cluster As String
    Public Property x As Integer Implements IPoint2D.X
    Public Property y As Integer Implements IPoint2D.Y
    Public Property color As String
    Public Property sample_tag As String

    Public Overrides Function ToString() As String
        Return $"[{phenograph_cluster}] {id}"
    End Function

    Public Function GetPixel() As Point
        Dim t As String() = id.Split(","c)
        Dim x As Integer = Integer.Parse(t(0))
        Dim y As Integer = Integer.Parse(t(1))

        Return New Point(x, y)
    End Function

    Public Shared Function GetSpotColorIndex(phenograph As IEnumerable(Of PhenographSpot),
                                             Optional colorSet As String = "paper") As Dictionary(Of String, Color)

        Dim classes As String() = phenograph.Select(Function(p) p.phenograph_cluster).Distinct.ToArray
        Dim colors As Color() = Designer.GetColors(colorSet, n:=classes.Length)
        Dim index As New Dictionary(Of String, Color)

        For i As Integer = 0 To classes.Length - 1
            Call index.Add(classes(i), colors(i))
        Next

        Return index
    End Function
End Class
