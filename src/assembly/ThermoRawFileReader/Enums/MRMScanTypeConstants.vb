#Region "Microsoft.VisualBasic::22abfcd3abc0c49d29b4262692a49c3d, E:/mzkit/src/assembly/ThermoRawFileReader//Enums/MRMScanTypeConstants.vb"

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

    '   Total Lines: 30
    '    Code Lines: 8
    ' Comment Lines: 18
    '   Blank Lines: 4
    '     File Size: 626 B


    ' Enum MRMScanTypeConstants
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' MRM Scan Types
''' </summary>
<CLSCompliant(True)>
Public Enum MRMScanTypeConstants
    ''' <summary>
    ''' Not MRM
    ''' </summary>
    NotMRM = 0

    ''' <summary>
    ''' Multiple SIM ranges in a single scan
    ''' </summary>
    MRMQMS = 1

    ''' <summary>
    ''' Monitoring a parent ion and one or more daughter ions
    ''' </summary>
    SRM = 2

    ''' <summary>
    ''' Full neutral loss scan
    ''' </summary>
    FullNL = 3

    ''' <summary>
    ''' Selected Ion Monitoring (SIM), which is MS1 of a limited m/z range
    ''' </summary>
    SIM = 4
End Enum
