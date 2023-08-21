#Region "Microsoft.VisualBasic::566e26e967ba42d1fc39548d0bac8888, mzkit\src\visualize\TissueMorphology\HEMap\[Object].vb"

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

'   Total Lines: 45
'    Code Lines: 34
' Comment Lines: 3
'   Blank Lines: 8
'     File Size: 1.50 KB


' Class [Object]
' 
'     Properties: Density, Pixels, Ratio
' 
'     Function: Eval
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace HEMap

    ''' <summary>
    ''' layer object data
    ''' </summary>
    Public Class [Object]

        Public Property Pixels As Integer
        Public Property Density As Double
        Public Property Ratio As Double

        Public Shared Function Eval(rect As Grid(Of Color),
                                    target As Color,
                                    gridSize As Integer,
                                    Optional tolerance As Integer = 5,
                                    Optional densityGrid As Integer = 5) As [Object]

            Dim hits As New List(Of Integer)
            Dim A As Double = densityGrid ^ 2

            rect = rect _
                .Cells _
                .Where(Function(c)
                           Return c.data.Equals(target, tolerance:=tolerance)
                       End Function) _
                .DoCall(AddressOf Grid(Of Color).CreateReadOnly)

            For x As Integer = 1 To gridSize Step densityGrid
                For y As Integer = 1 To gridSize Step densityGrid
                    Call hits.Add(rect.Query(x, y, densityGrid).Count)
                Next
            Next

            Return New [Object] With {
                .Pixels = rect.Cells.Count,
                .Density = (New Vector(integers:=hits) / A).Average,
                .Ratio = .Pixels / (gridSize ^ 2)
            }
        End Function
    End Class
End Namespace