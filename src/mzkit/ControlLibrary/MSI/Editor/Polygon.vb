#Region "Microsoft.VisualBasic::7d1b6b0dc534f97232ebe7a55a81fa2d, mzkit\src\mzkit\ControlLibrary\MSI\Editor\Polygon.vb"

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

    '   Total Lines: 26
    '    Code Lines: 21
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 713.00 B


    '     Class Polygon
    ' 
    '         Properties: Edges, Vertices
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: HasEdge
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace PolygonEditor
    Friend Class Polygon
        Public Property Vertices As List(Of Vertex)
        Public Property Edges As List(Of Edge)

        Public Sub New()
            Vertices = New List(Of Vertex)()
            Edges = New List(Of Edge)()
        End Sub

        Public Function HasEdge(ByVal e As Edge) As Boolean
            For Each edge In Edges
                If edge Is e Then Return True
            Next

            Return False
        End Function

        Public Sub New(ByVal vertices As List(Of Vertex), ByVal edges As List(Of Edge))
            Me.Vertices = vertices
            Me.Edges = edges
        End Sub
    End Class
End Namespace
