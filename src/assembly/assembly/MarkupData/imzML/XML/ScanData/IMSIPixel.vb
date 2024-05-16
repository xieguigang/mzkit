#Region "Microsoft.VisualBasic::76c195ebee549485b2e67913549a72f7, assembly\assembly\MarkupData\imzML\XML\ScanData\IMSIPixel.vb"

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

    '   Total Lines: 20
    '    Code Lines: 11
    ' Comment Lines: 3
    '   Blank Lines: 6
    '     File Size: 444 B


    '     Interface IMSIPixel
    ' 
    '         Properties: intensity, x, y
    ' 
    '     Interface ImageScan
    ' 
    '         Properties: IntPtr, MzPtr
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MarkupData.imzML

    Public Interface IMSIPixel

        ReadOnly Property x As Integer
        ReadOnly Property y As Integer
        ReadOnly Property intensity As Double

    End Interface

    ''' <summary>
    ''' a unify model of imzml image pixel scan data
    ''' </summary>
    Public Interface ImageScan

        Property MzPtr As ibdPtr
        Property IntPtr As ibdPtr

    End Interface
End Namespace
