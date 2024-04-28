#Region "Microsoft.VisualBasic::a93dbef743ad1142f952ceba37e42bbd, G:/mzkit/src/assembly/ProteoWizard.Interop//Filters/scanTime.vb"

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

    '   Total Lines: 23
    '    Code Lines: 17
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 590 B


    '     Class scanTime
    ' 
    '         Properties: timeRange
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: getFilterArgs, getFilterName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace filters

    Public Class scanTime : Inherits Filter

        Public ReadOnly Property timeRange As String

        Sub New(timeRange As String)
            Me.timeRange = timeRange
        End Sub

        Sub New(start#, stop#)
            Me.timeRange = $"[{start}, {[stop]}]"
        End Sub

        Protected Overrides Function getFilterName() As String
            Return NameOf(scanTime)
        End Function

        Protected Overrides Function getFilterArgs() As String
            Return timeRange
        End Function
    End Class
End Namespace
