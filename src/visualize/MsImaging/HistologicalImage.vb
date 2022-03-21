#Region "Microsoft.VisualBasic::6c3d4fc2faa55528e764af758d51626c, mzkit\src\visualize\MsImaging\HistologicalImage.vb"

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

    '   Total Lines: 34
    '    Code Lines: 21
    ' Comment Lines: 8
    '   Blank Lines: 5
    '     File Size: 1.03 KB


    ' Module HistologicalImage
    ' 
    '     Function: HeatMap, MonoScale
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver

''' <summary>
''' helper module for processing haematoxylin and eosin staining image
''' </summary>
Public Module HistologicalImage

    ''' <summary>
    ''' convert to gray scale
    ''' </summary>
    ''' <param name="HE"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MonoScale(HE As Image) As Image
        Return HE.Grayscale
    End Function

    <Extension>
    Public Function HeatMap(HE As Image,
                            Optional scale As ScalerPalette = ScalerPalette.turbo,
                            Optional mapLevels As Integer = 64) As GraphicsData

        Return HE.Image2DMap(
            scaleName:=scale.Description,
            mapLevels:=mapLevels
        )
    End Function

End Module
