#Region "Microsoft.VisualBasic::a040de50af1da9cb3a4cacc52ec317d7, src\visualize\MsImaging\PixelData.vb"

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

' Class PixelData
' 
'     Properties: intensity, level, mz, x, y
' 
'     Function: ScalePixels, ToString
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

''' <summary>
''' a pixels point of [x,y,color]
''' </summary>
Public Class PixelData

    Public Property x As Integer
    Public Property y As Integer
    Public Property intensity As Double
    Public Property level As Double
    Public Property mz As Double

    Public Overrides Function ToString() As String
        Return $"Dim [{x},{y}] as intensity = {intensity}"
    End Function

    ''' <summary>
    ''' 将响应度数据统一缩放到[0,1]之间
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <returns></returns>
    Public Shared Function ScalePixels(pixels As PixelData()) As PixelData()
        Dim intensityRange As DoubleRange = pixels _
            .Select(Function(p) p.intensity) _
            .Range
        Dim level As Double
        Dim levelRange As DoubleRange = New Double() {0, 1}

        For Each point As PixelData In pixels
            level = intensityRange.ScaleMapping(point.intensity, levelRange)
            point.level = level
        Next

        Return pixels
    End Function

    ''' <summary>
    ''' 将一个二维坐标信息转换为序列索引号
    ''' </summary>
    ''' <param name="size"></param>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这个函数一般是用于处理mzpack文件中的原始数据
    ''' 并且mzpack文件中的像素点信息应该是按行扫描得到的结果
    ''' </remarks>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function SequenceIndex(size As Size, x As Integer, y As Integer) As Integer
        Return y * size.Width + x
    End Function
End Class

