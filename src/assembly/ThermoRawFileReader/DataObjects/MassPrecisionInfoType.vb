#Region "Microsoft.VisualBasic::a2bf43a23ab2278de9f6c08f13fe3945, assembly\ThermoRawFileReader\DataObjects\MassPrecisionInfoType.vb"

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
    '    Code Lines: 10
    ' Comment Lines: 18
    '   Blank Lines: 5
    '     File Size: 786 B


    '     Structure MassPrecisionInfoType
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DataObjects

    ''' <summary>
    ''' Type for Mass Precision Information
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure MassPrecisionInfoType
        ''' <summary>
        ''' Peak Intensity
        ''' </summary>
        Public Intensity As Double

        ''' <summary>
        ''' Peak Mass
        ''' </summary>
        Public Mass As Double

        ''' <summary>
        ''' Peak Accuracy (in MMU)
        ''' </summary>
        Public AccuracyMMU As Double

        ''' <summary>
        ''' Peak Accuracy (in PPM)
        ''' </summary>
        Public AccuracyPPM As Double

        ''' <summary>
        ''' Peak Resolution
        ''' </summary>
        Public Resolution As Double
    End Structure
End Namespace
