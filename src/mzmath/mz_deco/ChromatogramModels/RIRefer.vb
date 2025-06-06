#Region "Microsoft.VisualBasic::e8cc35aa049aec60f31ed78fc852244b, mzmath\mz_deco\ChromatogramModels\RIRefer.vb"

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

    '   Total Lines: 52
    '    Code Lines: 13 (25.00%)
    ' Comment Lines: 31 (59.62%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (15.38%)
    '     File Size: 1.71 KB


    ' Class RIRefer
    ' 
    '     Properties: mz, name, reference_mz, reference_rt, RI
    '                 rt, xcms_id
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' the retention index reference
''' </summary>
Public Class RIRefer : Implements INamedValue, IReadOnlyId, IRetentionIndex, IRetentionTime, IMs1

    ''' <summary>
    ''' the unique key reference id in sample data
    ''' </summary>
    ''' <returns></returns>
    Public Property xcms_id As String Implements INamedValue.Key, IReadOnlyId.Identity

    ''' <summary>
    ''' the ion m/z value in sample data, debug used only
    ''' </summary>
    ''' <returns></returns>
    Public Property mz As Double Implements IMs1.mz
    ''' <summary>
    ''' the ion rt value in sample data, the sample RI calculation use this sample value
    ''' </summary>
    ''' <returns></returns>
    Public Property rt As Double Implements IMs1.rt

    ''' <summary>
    ''' the reference retention index value
    ''' </summary>
    ''' <returns></returns>
    Public Property RI As Double Implements IRetentionIndex.RI

    ''' <summary>
    ''' the reference m/z input
    ''' </summary>
    ''' <returns></returns>
    Public Property reference_mz As Double
    ''' <summary>
    ''' the reference rt input, the reference rt for find the sample rt, <strong>do not use this property value for make RI evaluation</strong>.
    ''' </summary>
    ''' <returns></returns>
    Public Property reference_rt As Double

    ''' <summary>
    ''' the name of current reference RI object
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String

    Public Overrides Function ToString() As String
        Return $"m/z {mz}, {rt} sec, " & xcms_id
    End Function

End Class
