#Region "Microsoft.VisualBasic::2117cc4889ff24af58cf93b86cda8e49, assembly\assembly\MarkupData\imzML\XML\IntensitySummary.vb"

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

    '   Total Lines: 21
    '    Code Lines: 8
    ' Comment Lines: 12
    '   Blank Lines: 1
    '     File Size: 548 B


    '     Enum IntensitySummary
    ' 
    '         Average, BasePeak, Median, Total
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MarkupData.imzML

    ''' <summary>
    ''' TIC/BPC/Average
    ''' </summary>
    Public Enum IntensitySummary As Integer
        ''' <summary>
        ''' sum all intensity signal value in a pixel
        ''' </summary>
        Total
        ''' <summary>
        ''' get a max intensity signal value in a pixel
        ''' </summary>
        BasePeak
        ''' <summary>
        ''' get the average intensity signal value in a pixel
        ''' </summary>
        Average
        Median
    End Enum
End Namespace
