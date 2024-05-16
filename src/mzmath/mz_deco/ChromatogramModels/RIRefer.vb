#Region "Microsoft.VisualBasic::54e81e6cfae8e4868ea42dbe0bb89b03, mzmath\mz_deco\ChromatogramModels\RIRefer.vb"

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

    '   Total Lines: 24
    '    Code Lines: 10
    ' Comment Lines: 7
    '   Blank Lines: 7
    '     File Size: 738 B


    ' Class RIRefer
    ' 
    '     Properties: mz, name, RI, rt
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

    Public Property name As String Implements INamedValue.Key, IReadOnlyId.Identity
    Public Property mz As Double Implements IMs1.mz
    Public Property rt As Double Implements IMs1.rt

    ''' <summary>
    ''' the reference retention index value
    ''' </summary>
    ''' <returns></returns>
    Public Property RI As Double Implements IRetentionIndex.RI

    Public Overrides Function ToString() As String
        Return $"m/z {mz}, {rt} sec, " & name
    End Function



End Class
