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

Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Public Class PixelData

    Public Property x As Integer
    Public Property y As Integer
    Public Property intensity As Double
    Public Property level As Double
    Public Property mz As Double

    Public Overrides Function ToString() As String
        Return $"Dim [{x},{y}] as intensity = {intensity}"
    End Function

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
End Class

