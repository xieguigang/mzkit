#Region "Microsoft.VisualBasic::16202e58d34078bfaf25aa341d607e14, Massbank\Public\NCBI\PubChem\Web\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: GetHMDBId, getInformation, GetInformationNumber, GetInformationString, GetInformationStrings
    '                   GetInformationTable
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace NCBI.PubChem

    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationNumber(section As Section, key$) As Double
            Dim info = section.getInformation(key)

            If info Is Nothing OrElse info.Value.Number.StringEmpty Then
                Return 0
            Else
                Return Val(info.Value.Number)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationString(section As Section, key$) As String
            Dim info = section.getInformation(key)

            If info Is Nothing OrElse info.Value.StringWithMarkup Is Nothing Then
                Return ""
            Else
                Return info.Value.StringWithMarkup.FirstOrDefault?.String
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationStrings(section As Section, key$) As String()
            Dim info = section.getInformation(key)

            If info Is Nothing Then
                Return {}
            Else
                ' 当只有一个字符串的时候,可能会错误的判断为字符串对象
                ' 而非字符串数组
                ' 在这里需要检查一下
                Dim data = info.InfoValue

                If data Is Nothing Then
                    Return {}
                ElseIf TypeOf data Is String Then
                    Return {DirectCast(data, String)}
                ElseIf TypeOf data Is String() Then
                    Return data
                Else
                    Return {Scripting.ToString(data)}
                End If
            End If
        End Function

        <Extension>
        Private Function getInformation(section As Section, key$) As Information
            If section Is Nothing Then
                Return Nothing
            Else
                Return section _
                    .Information _
                    .SafeQuery _
                    .FirstOrDefault(Function(i) i.Name = key)
            End If
        End Function

        <Extension>
        Friend Function GetHMDBId(refs As Reference()) As String
            Return refs.SafeQuery _
                .FirstOrDefault(Function(ref)
                                    Return ref.SourceName = PugViewRecord.HMDB
                                End Function) _
               ?.SourceID
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationTable(section As Section, key$) As Table
            Return section.getInformation(key)?.Table
        End Function
    End Module
End Namespace
