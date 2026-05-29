#Region "Microsoft.VisualBasic::af46a4a98a588c66f570766cca85acf2, visualize\TissueMorphology\SpatialMapping\SpatialMapping.vb"

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

    '   Total Lines: 99
    '    Code Lines: 40 (40.40%)
    ' Comment Lines: 47 (47.47%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (12.12%)
    '     File Size: 3.04 KB


    ' Class SpatialMapping
    ' 
    '     Properties: color, label, spots, transform
    ' 
    '     Function: getCollection, getSize, GetSpatialMetabolismRectangle
    ' 
    ' Class Transform
    ' 
    ' 
    '     Enum Operation
    ' 
    '         Mirror, Rotate
    ' 
    ' 
    ' 
    '  
    ' 
    '     Properties: argument, op
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

''' <summary>
''' the spatial mapping between two spatial omics data
''' </summary>
''' <remarks>
''' this mapping data is consist with a collection of the spatial <see cref="SpotMap"/>,
''' the data <see cref="Transform"/> records how to mapping from one layer to another
''' omics layer, example as the geometry point rotation, offsets, and other operations.
''' </remarks>
Public Class SpatialMapping : Inherits ListOf(Of SpotMap)

    ''' <summary>
    ''' the sample data labels
    ''' </summary>
    ''' <returns></returns>
    <XmlElement> Public Property label As String

    ''' <summary>
    ''' a collection of the spatial spot geometry mapping result
    ''' </summary>
    ''' <returns></returns>
    <XmlElement("spot")>
    Public Property spots As SpotMap()
    ''' <summary>
    ''' the transform operation that generates
    ''' the current spot location in STdata.
    ''' </summary>
    ''' <returns></returns>
    Public Property transform As Transform()
    ''' <summary>
    ''' html color code
    ''' </summary>
    ''' <returns></returns>
    <XmlElement> Public Property color As String

    Protected Overrides Function getSize() As Integer
        Return spots.TryCount
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of SpotMap)
        Return spots
    End Function

    Public Function GetSpatialMetabolismRectangle() As RectangleF
        Dim path = spots _
            .Select(Function(p)
                        Return p.SMX.Select(Function(xi, i) New PointF(xi, p.SMY(i)))
                    End Function) _
            .IteratesALL _
            .ToArray
        Dim poly As New Polygon2D(path)
        Dim rect As RectangleF = poly.GetRectangle

        Return rect
    End Function
End Class

''' <summary>
''' the geometry mapping transform frame data consists with a 
''' transform <see cref="op"/> type and the operation argument 
''' value
''' </summary>
Public Class Transform

    ''' <summary>
    ''' the geometry transform operation type
    ''' </summary>
    Public Enum Operation
        ''' <summary>
        ''' point rotation
        ''' </summary>
        Rotate
        ''' <summary>
        ''' point mirror translation
        ''' </summary>
        Mirror
    End Enum

    ''' <summary>
    ''' the operation type
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property op As Operation
    ''' <summary>
    ''' rotate angle in degree if the <see cref="op"/> code is <see cref="Transform.Operation.Rotate"/>
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property argument As Double

    Public Overrides Function ToString() As String
        Return $"{op.Description}({argument})"
    End Function

End Class
