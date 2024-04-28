#Region "Microsoft.VisualBasic::091b0f369f7a234fa421cb233a4e3825, E:/mzkit/src/assembly/assembly//ASCII/MSP/Comments.vb"

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

    '   Total Lines: 53
    '    Code Lines: 28
    ' Comment Lines: 16
    '   Blank Lines: 9
    '     File Size: 1.82 KB


    '     Module Comments
    ' 
    '         Function: (+2 Overloads) ToTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.Parser

Namespace ASCII.MSP

    ''' <summary>
    ''' MetaData in comments
    ''' </summary>
    Public Module Comments

        ''' <summary>
        ''' Parse a given string a key-value tuple data list.
        ''' (解析放置于注释之中的代谢物注释元数据)
        ''' </summary>
        ''' <param name="comments$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function ToTable(comments$) As NameValueCollection
            Dim meta As NameValueCollection = DelimiterParser.GetTokens(comments).ToTable

            If (meta.Count = 0 OrElse (meta.Count = 1 AndAlso meta.AllKeys.First.StringEmpty)) AndAlso Not comments.StringEmpty Then
                meta.Add("Comment", comments)
            End If

            Return meta
        End Function

        ''' <summary>
        ''' 解析放置于注释之中的代谢物注释元数据
        ''' </summary>
        ''' <param name="comments"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ToTable(comments As String()) As NameValueCollection
            ' 为了兼容两个SMILES结构
            Dim table As New NameValueCollection

            For Each g As NamedValue(Of String) In comments _
                .Select(Function(s)
                            Return s.GetTagValue("=", trim:=True)
                        End Function)

                Call table.Add(g.Name, g.Value)
            Next

            Return table
        End Function
    End Module
End Namespace
