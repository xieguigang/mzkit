#Region "Microsoft.VisualBasic::6b57fbabaf79964825d937d0d79c6e07, visualize\MsImaging\Blender\Renderer\CompositionBlender.vb"

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
    '    Code Lines: 24 (66.67%)
    ' Comment Lines: 5 (13.89%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (19.44%)
    '     File Size: 1.37 KB


    '     Class CompositionBlender
    ' 
    '         Properties: dimension
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: HeatmapBlending
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace Blender

    Public MustInherit Class CompositionBlender

        Protected ReadOnly defaultBackground As Color
        Protected ReadOnly heatmapMode As Boolean

        Public Property dimension As Size

        Sub New(defaultBackground As Color, Optional heatmapMode As Boolean = False)
            Me.heatmapMode = heatmapMode
            Me.defaultBackground = defaultBackground
        End Sub

        ''' <summary>
        ''' do imaging heatmap rendering
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="region"></param>
        Public MustOverride Sub Render(ByRef g As IGraphics, region As GraphicsRegion)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Shared Function HeatmapBlending(channel As Func(Of Integer, Integer, Byte), x As Integer, y As Integer) As Byte
            Return New Integer() {
                channel(x - 1, y - 1), channel(x, y - 1), channel(x + 1, y - 1),
                channel(x - 1, y), channel(x, y), channel(x + 1, y),
                channel(x - 1, y + 1), channel(x, y + 1), channel(x + 1, y + 1)
            }.Average
        End Function
    End Class
End Namespace
