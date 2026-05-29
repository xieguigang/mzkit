#Region "Microsoft.VisualBasic::e65d4b3b28eb7af6c103b7517cb4b769, assembly\BrukerDataReader\Raw\Exceptions\InvariantException.vb"

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
    '    Code Lines: 16 (53.33%)
    ' Comment Lines: 12 (40.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (6.67%)
    '     File Size: 776 B


    '     Class InvariantException
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System

Namespace Raw

    ''' <summary>
    ''' Exception raised when an invariant fails.
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class InvariantException
        Inherits DesignByContractException
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New()
        End Sub
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class
End Namespace
