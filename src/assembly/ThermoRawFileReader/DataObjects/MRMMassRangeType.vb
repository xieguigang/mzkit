#Region "Microsoft.VisualBasic::73314f2473beb3b87b43f47d855134e3, assembly\ThermoRawFileReader\DataObjects\MRMMassRangeType.vb"

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
    '    Code Lines: 11
    ' Comment Lines: 18
    '   Blank Lines: 5
    '     File Size: 875 B


    '     Structure MRMMassRangeType
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DataObjects

    ''' <summary>
    ''' Type for storing MRM Mass Ranges
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure MRMMassRangeType

        ''' <summary>
        ''' Start Mass
        ''' </summary>
        Public StartMass As Double

        ''' <summary>
        ''' End Mass
        ''' </summary>
        Public EndMass As Double

        ''' <summary>
        ''' Central Mass
        ''' </summary>
        ''' <remarks>
        ''' Useful for MRM/SRM experiments
        ''' </remarks>
        Public CentralMass As Double

        ''' <summary>
        ''' Return a summary of this object
        ''' </summary>
        Public Overrides Function ToString() As String
            Return StartMass.ToString("0.000") & "-" & EndMass.ToString("0.000")
        End Function
    End Structure
End Namespace
