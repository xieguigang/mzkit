﻿#Region "Microsoft.VisualBasic::aefb60e02cff0029728e70002ba08118, mzmath\SingleCells\Matrix\PixelData.vb"

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

    '   Total Lines: 64
    '    Code Lines: 33 (51.56%)
    ' Comment Lines: 20 (31.25%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (17.19%)
    '     File Size: 2.23 KB


    '     Class PixelData
    ' 
    '         Properties: intensity, label, X, Y, Z
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: FromSingleCellExpression, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math

Namespace Deconvolute

    ''' <summary>
    ''' A pixel data or single cell data
    ''' </summary>
    ''' <remarks>
    ''' the value of property <see cref="X"/> and <see cref="Y"/> is zero when the 
    ''' matrix data is a single cell data matrix
    ''' </remarks>
    Public Class PixelData : Implements IVector, IPoint3D, RasterPixel, IPoint2D

        Public Property X As Integer Implements RasterPixel.X, IPoint2D.X
        Public Property Y As Integer Implements RasterPixel.Y, IPoint2D.Y
        Public Property Z As Integer Implements IPoint3D.Z

        ''' <summary>
        ''' scan id or the single cell label
        ''' </summary>
        ''' <returns></returns>
        Public Property label As String

        ''' <summary>
        ''' the ion features expression data in current spot/single_cell
        ''' </summary>
        ''' <returns></returns>
        Public Property intensity As Double() Implements IVector.Data

        ''' <summary>
        ''' get nth ion feature expression value
        ''' </summary>
        ''' <param name="offset"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property feature(offset As Integer) As Double
            Get
                Return intensity(offset)
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(label As String, expression As Double())
            Me.label = label
            Me.intensity = expression
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{X},{Y}] {label} total_ions:{intensity.Sum}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromSingleCellExpression(Of T As {INamedValue, IVector})(v As T) As PixelData
            Return New PixelData(v.Key, v.Data)
        End Function

    End Class
End Namespace
