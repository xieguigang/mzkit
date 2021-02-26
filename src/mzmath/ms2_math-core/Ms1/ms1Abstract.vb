#Region "Microsoft.VisualBasic::9d444d3ff9268e0f8aacfa93ee33990a, ms2_math-core\Ms1\ms1Abstract.vb"

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

    ' Interface IMs1
    ' 
    '     Properties: mz
    ' 
    ' Interface IRetentionTime
    ' 
    '     Properties: rt
    ' 
    ' Interface IMs1Scan
    ' 
    '     Properties: intensity
    ' 
    ' /********************************************************************************/

#End Region


Public Interface IMs1 : Inherits IRetentionTime

    Property mz As Double

End Interface

Public Interface IRetentionTime

    ''' <summary>
    ''' Rt in seconds
    ''' </summary>
    ''' <returns></returns>
    Property rt As Double

End Interface

''' <summary>
''' [mz, rt, intensity]
''' </summary>
Public Interface IMs1Scan : Inherits IMs1

    Property intensity As Double

End Interface

