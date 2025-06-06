﻿#Region "Microsoft.VisualBasic::32ca3f38c9d4a861ea8023c0188aaf16, assembly\assembly\MarkupData\imzML\Extensions.vb"

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

    '   Total Lines: 44
    '    Code Lines: 25 (56.82%)
    ' Comment Lines: 12 (27.27%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (15.91%)
    '     File Size: 1.53 KB


    '     Module Extensions
    ' 
    '         Function: CreatePixelData, ExtractPixels
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging

Namespace MarkupData.imzML

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' Make conversion of the pixel data model
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="shape"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ExtractPixels(Of T As IMSIPixel)(shape As IEnumerable(Of T)) As IEnumerable(Of PixelScanIntensity)
            If shape Is Nothing Then
                Return
            End If

            For Each pixel As T In shape
                Yield New PixelScanIntensity With {.x = pixel.x, .y = pixel.y, .totalIon = pixel.intensity}
            Next
        End Function

        ''' <summary>
        ''' Make conversion of the pixel data model
        ''' </summary>
        ''' <typeparam name="PixelData"></typeparam>
        ''' <param name="layer"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function CreatePixelData(Of PixelData As {New, Pixel})(layer As IEnumerable(Of PixelScanIntensity)) As IEnumerable(Of PixelData)
            If layer Is Nothing Then
                Return
            End If

            For Each pixel As PixelScanIntensity In layer
                Yield New PixelData With {.X = pixel.x, .Y = pixel.y, .Scale = pixel.totalIon}
            Next
        End Function

    End Module
End Namespace
