#Region "Microsoft.VisualBasic::b3c26a926a9cd876db7ae797f331458a, mzmath\ms2_math-core\Ms1\PrecursorType\IonModes.vb"

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

    '   Total Lines: 29
    '    Code Lines: 9 (31.03%)
    ' Comment Lines: 16 (55.17%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (13.79%)
    '     File Size: 718 B


    '     Enum IonModes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' Ion Modes, +/-
    ''' </summary>
    ''' <remarks>
    ''' integer value is matched with the thermo fisher ms raw file reader
    ''' do not modify the constant value at here
    ''' </remarks>
    <CLSCompliant(True)>
    Public Enum IonModes As Integer
        ''' <summary>
        ''' Unknown Ion Mode
        ''' </summary>
        Unknown = 0

        ''' <summary>
        ''' Positive Ion Mode
        ''' </summary>
        <Description("+")> Positive = 1

        ''' <summary>
        ''' Negative Ion Mode
        ''' </summary>
        <Description("-")> Negative = -1
    End Enum
End Namespace
