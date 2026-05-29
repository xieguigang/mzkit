#Region "Microsoft.VisualBasic::ee90b7f48da33441825765c4e048679b, mzmath\mz_deco\Task\peaktable_task.vb"

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
    '    Code Lines: 12 (48.00%)
    ' Comment Lines: 8 (32.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (20.00%)
    '     File Size: 804 B


    '     Class peaktable_task
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Parallel

Namespace Tasks

    Public MustInherit Class peaktable_task : Inherits VectorTask

        Public ReadOnly out As New List(Of xcms2)
        Public ReadOnly rt_shifts As New List(Of RtShift)

        ''' <summary>
        ''' construct a new parallel task executator
        ''' </summary>
        ''' <param name="nsize"></param>
        ''' <remarks>
        ''' the thread count for run the parallel task is configed
        ''' via the <see cref="n_threads"/> by default.
        ''' </remarks>
        Sub New(nsize As Integer,
                Optional verbose As Boolean = False,
                Optional workers As Integer? = Nothing)

            Call MyBase.New(nsize, verbose, workers)
        End Sub
    End Class
End Namespace
