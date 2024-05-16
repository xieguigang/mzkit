#Region "Microsoft.VisualBasic::4dd9692535477a2b5a6beac4ea298e76, visualize\TissueMorphology\HEMap\HeatMapAnalysis\HeatMapLayer.vb"

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

    '   Total Lines: 79
    '    Code Lines: 65
    ' Comment Lines: 3
    '   Blank Lines: 11
    '     File Size: 3.08 KB


    '     Module HeatMapLayer
    ' 
    '         Function: GetHeatMapLayer, RSD
    ' 
    '     Enum Layers
    ' 
    '         Density, Pixels, Ratio
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions

Namespace HEMap

    Public Module HeatMapLayer

        <Extension>
        Public Function GetHeatMapLayer(grid As Cell(),
                                        Optional heatmap As Layers = Layers.Density,
                                        Optional channel As String = "black") As PixelData()

            Dim objs As (obj As [Object], cell As Cell)() = Nothing
            Dim project =
                Function(selector As Func(Of [Object], Double))
                    Return objs _
                        .Where(Function(i) i.obj IsNot Nothing) _
                        .Select(Function(i)
                                    Return New PixelData With {
                                        .Scale = selector(i.obj),
                                        .X = i.cell.ScaleX,
                                        .Y = i.cell.ScaleY
                                    }
                                End Function) _
                        .ToArray
                End Function

            Select Case Strings.LCase(channel)
                Case "black"
                    objs = grid.Select(Function(i) (i.Black, i)).ToArray
                Case Else
                    objs = grid _
                        .Select(Function(i)
                                    If i.layers.ContainsKey(channel) Then
                                        Return (i.layers(channel), i)
                                    Else
                                        Return Nothing
                                    End If
                                End Function) _
                        .ToArray
            End Select

            Select Case heatmap
                Case Layers.Pixels : Return project(Function(i) i.Pixels)
                Case Layers.Density : Return project(Function(i) i.Density)
                Case Layers.Ratio : Return project(Function(i) i.Ratio)
                Case Else
                    Throw New NotImplementedException(heatmap.Description)
            End Select
        End Function

        <Extension>
        Public Function RSD(layer As PixelData(),
                            Optional nbags As Integer = 300,
                            Optional nsamples As Integer = 32) As Double()

            Dim sampling = Bootstraping.Samples(layer, N:=nsamples, bags:=nbags).ToArray
            Dim rsdVals As Double() = sampling _
                .Select(Function(bag)
                            Return bag.value.Select(Function(a) a.Scale).RSD
                        End Function) _
                .ToArray

            Return rsdVals
        End Function

    End Module

    ''' <summary>
    ''' the layer data of <see cref="HEMap.Object"/>
    ''' </summary>
    Public Enum Layers
        Pixels
        Density
        Ratio
    End Enum
End Namespace
