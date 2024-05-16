#Region "Microsoft.VisualBasic::7886f2d5f485d530be881114f302a11d, assembly\ThermoRawFileReader\Enums\InstFlags.vb"

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

    '   Total Lines: 25
    '    Code Lines: 7
    ' Comment Lines: 15
    '   Blank Lines: 3
    '     File Size: 603 B


    ' Module InstFlags
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' Instrument Flags
''' </summary>
<CLSCompliant(True)>
Public Module InstFlags
    ''' <summary>
    ''' Total Ion Map
    ''' </summary>
    Public Const TIM As String = "Total Ion Map"

    ''' <summary>
    ''' Neutral Loss Map
    ''' </summary>
    Public Const NLM As String = "Neutral Loss Map"

    ''' <summary>
    ''' Parent Ion Map
    ''' </summary>
    Public Const PIM As String = "Parent Ion Map"

    ''' <summary>
    ''' Data Dependent ZoomScan Map
    ''' </summary>
    Public Const DDZMap As String = "Data Dependent ZoomScan Map"
End Module
