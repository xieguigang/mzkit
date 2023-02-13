#Region "Microsoft.VisualBasic::62dc0fc4a780867f5a92bda5c69af3da, mzkit\src\visualize\TissueMorphology\Scatter\UMAPPoint.vb"

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

    '   Total Lines: 33
    '    Code Lines: 9
    ' Comment Lines: 21
    '   Blank Lines: 3
    '     File Size: 942 B


    ' Class UMAPPoint
    ' 
    '     Properties: [class], label, Pixel, x, y
    '                 z
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing

''' <summary>
''' 3d scatter data point, a spatial spot or a single cell data
''' </summary>
Public Class UMAPPoint

    ''' <summary>
    ''' the spatial point of current spot if it is the sptial data, value
    ''' of this property is empty for the single cell data
    ''' </summary>
    ''' <returns></returns>
    Public Property Pixel As Point
    ''' <summary>
    ''' the cell label of current spot
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' this property value may be nothing for the sptial data, 
    ''' label value should not be nothing if the data is single 
    ''' cell data.
    ''' </remarks>
    Public Property label As String
    Public Property x As Double
    Public Property y As Double
    Public Property z As Double
    ''' <summary>
    ''' the cell cluster data
    ''' </summary>
    ''' <returns></returns>
    Public Property [class] As Integer

End Class
