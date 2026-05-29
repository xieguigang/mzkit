#Region "Microsoft.VisualBasic::5cb2479121d14f5367522a9747bd7ed3, mzmath\GCxGC\Peak2D.vb"

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

    '   Total Lines: 57
    '    Code Lines: 15 (26.32%)
    ' Comment Lines: 39 (68.42%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (5.26%)
    '     File Size: 1.57 KB


    ' Class Peak2D
    ' 
    '     Properties: id, maxIntensity, rt1, rt2, rtmax1
    '                 rtmax2, rtmin1, rtmin2, sn, volumn
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' Model of the GCxGC 2d peak
''' </summary>
Public Class Peak2D

    ''' <summary>
    ''' the unique reference id of current peak data
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String
    ''' <summary>
    ''' the rt of dimension 1
    ''' </summary>
    ''' <returns></returns>
    Public Property rt1 As Double
    ''' <summary>
    ''' the rt of dimension 2
    ''' </summary>
    ''' <returns></returns>
    Public Property rt2 As Double
    ''' <summary>
    ''' the rt range of dimension 1
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmin1 As Double
    ''' <summary>
    ''' the rt range of dimension 1
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmax1 As Double
    ''' <summary>
    ''' the rt range of dimension 2
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmin2 As Double
    ''' <summary>
    ''' the rt range of dimension 2
    ''' </summary>
    ''' <returns></returns>
    Public Property rtmax2 As Double
    ''' <summary>
    ''' the volumn data of current peak shape, the metabolite expression value
    ''' </summary>
    ''' <returns></returns>
    Public Property volumn As Double
    ''' <summary>
    ''' the max intensity of current peak object
    ''' </summary>
    ''' <returns></returns>
    Public Property maxIntensity As Double
    Public Property sn As Double

    Public Overrides Function ToString() As String
        Return id
    End Function

End Class
