#Region "Microsoft.VisualBasic::1e4a3d5fbb2d167ffba1e74a35db7a08, G:/mzkit/src/visualize/TissueMorphology//Scatter/UMAPPoint.vb"

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

    '   Total Lines: 100
    '    Code Lines: 63
    ' Comment Lines: 23
    '   Blank Lines: 14
    '     File Size: 3.23 KB


    ' Class UMAPPoint
    ' 
    '     Properties: [class], label, Pixel, x, y
    '                 z
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: GetClusterLabels, ParseCsvTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 3d scatter data point, a spatial spot or a single cell data
''' </summary>
Public Class UMAPPoint

    ''' <summary>
    ''' the spatial point of current spot if it is the sptial data, value
    ''' of this property is empty for the single cell data
    ''' </summary>
    ''' <returns></returns>
    Public Property Pixel As Point
    ''' <summary>
    ''' the cell label of current spot
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' this property value may be nothing for the spatial data, 
    ''' label value should not be nothing if the data is single 
    ''' cell data.
    ''' </remarks>
    Public Property label As String
    Public Property x As Double
    Public Property y As Double
    Public Property z As Double
    ''' <summary>
    ''' the cell cluster data
    ''' </summary>
    ''' <returns></returns>
    Public Property [class] As String

    Sub New()
    End Sub

    Sub New(label As String, x As Double, y As Double, z As Double)
        Me.label = label
        Me.x = x
        Me.y = y
        Me.z = z
    End Sub

    Private Shared Function GetClusterLabels(df As DataFrame) As String()
        Dim [class] As String()

        Static fields As String() = {
            "class", "Class",
            "phenograph_cluster",
            "Cluster", "cluster"
        }

        For Each name As String In fields
            [class] = df.GetColumnValues(name).SafeQuery.ToArray

            If Not [class].IsNullOrEmpty Then
                Return [class]
            End If
        Next

        Return New String() {}
    End Function

    Public Shared Iterator Function ParseCsvTable(file As String) As IEnumerable(Of UMAPPoint)
        Dim df As DataFrame = DataFrame.Load(file)
        Dim labels As String() = df.GetColumnVectors.First.ToArray
        Dim x As Double() = df.GetColumnValues("x").Select(AddressOf Val).ToArray
        Dim y As Double() = df.GetColumnValues("y").Select(AddressOf Val).ToArray
        Dim z As Double() = df.GetColumnValues("z").Select(AddressOf Val).ToArray
        ' "Noise"
        Dim [class] As String() = GetClusterLabels(df)
        Dim classIndex As Index(Of String) = [class].Distinct.Where(Function(c) c <> "Noise").Indexing
        Dim label As String
        Dim t As String()
        Dim pt As Point
        Dim class_tag As String = ""

        For i As Integer = 0 To labels.Length - 1
            label = labels(i)
            t = label.Split(","c)
            pt = New Point(Val(t(0)), Val(t(1)))

            ' handling of missing class tag data
            If [class].Length > 0 Then
                class_tag = [class](i)
            End If

            Yield New UMAPPoint With {
                .[class] = classIndex.IndexOf(class_tag),
                .label = labels(i),
                .x = x(i), .y = y(i), .z = z(i),
                .Pixel = pt
            }
        Next
    End Function

End Class
