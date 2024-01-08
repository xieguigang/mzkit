#Region "Microsoft.VisualBasic::021b9dc0adf1db588e2f8a3c06bf29c5, mzkit\src\visualize\TissueMorphology\HEMap\Cell.vb"

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

'   Total Lines: 52
'    Code Lines: 21
' Comment Lines: 24
'   Blank Lines: 7
'     File Size: 1.33 KB


' Class Cell
' 
'     Properties: B, Black, G, isBlack, layers
'                 R, ScaleX, ScaleY, X, Y
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph

Namespace HEMap

    ''' <summary>
    ''' A cell block on the image
    ''' </summary>
    ''' <remarks>
    ''' the channel layers data of the colors is consist with the <see cref="HEMap.[Object]"/> collection.
    ''' </remarks>
    Public Class Cell : Implements IPoint2D

        ''' <summary>
        ''' the physical image x
        ''' </summary>
        ''' <returns></returns>
        Public Property X As Integer
        ''' <summary>
        ''' the physical image y
        ''' </summary>
        ''' <returns></returns>
        Public Property Y As Integer

        ''' <summary>
        ''' the location X of the grid
        ''' </summary>
        ''' <returns></returns>
        Public Property ScaleX As Integer Implements IPoint2D.X
        ''' <summary> 
        ''' the location Y of the grid
        ''' </summary>
        ''' <returns></returns>
        Public Property ScaleY As Integer Implements IPoint2D.Y

        ''' <summary>
        ''' average value of Red channel
        ''' </summary>
        ''' <returns></returns>
        Public Property R As Double
        ''' <summary>
        ''' average value of Green channel
        ''' </summary>
        ''' <returns></returns>
        Public Property G As Double
        ''' <summary>
        ''' average value of Blue channel
        ''' </summary>
        ''' <returns></returns>
        Public Property B As Double
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property black As [Object]

        Public Property layers As New Dictionary(Of String, [Object])

        ''' <summary>
        ''' does current cell block is in color black?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property isBlack As Boolean
            Get
                Return black.Ratio > 0.975
            End Get
        End Property

    End Class
End Namespace