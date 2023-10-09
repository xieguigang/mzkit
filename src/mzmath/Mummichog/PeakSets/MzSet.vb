#Region "Microsoft.VisualBasic::58ec9ecb656a523c38a3c4e8e75ea70e, mzkit\src\mzmath\Mummichog\PeakSets\MzSet.vb"

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

'   Total Lines: 22
'    Code Lines: 17
' Comment Lines: 0
'   Blank Lines: 5
'     File Size: 543 B


' Class MzSet
' 
'     Properties: mz, query, size
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

''' <summary>
''' The collection of the ion query result
''' </summary>
Public Class MzSet

    ''' <summary>
    ''' The target ion m/z value
    ''' </summary>
    ''' <returns></returns>
    Public Property mz As Double
    ''' <summary>
    ''' the metabolite annotation query result, contains all candidates
    ''' </summary>
    ''' <returns></returns>
    Public Property query As MzQuery()

    ''' <summary>
    ''' the candidate size of current m/z matches result 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property size As Integer
        Get
            Return query.Length
        End Get
    End Property

    ''' <summary>
    ''' get a candidate result via a given index value <paramref name="i"/>
    ''' </summary>
    ''' <param name="i"></param>
    ''' <returns></returns>
    Default Public ReadOnly Property Item(i As Integer) As MzQuery
        Get
            Return query(i)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{mz.ToString("F4")}: {query.Select(Function(i) i.name).JoinBy("; ")}"
    End Function

End Class
