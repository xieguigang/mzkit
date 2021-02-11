#Region "Microsoft.VisualBasic::164d9691fabf7bcff85a28e543b6bb33, Massbank\Public\NCBI\PubChem\Web\Extensions.vb"

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
    '         Function: castStrings, GetHMDBId, GetInformation, GetInformationNumber, GetInformationString
    '                   GetInformationStrings, GetInformationTable, InformationNoNull, matchName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace NCBI.PubChem

    <HideModuleName>
    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationNumber(section As Section, key$) As Double
            Dim info = section.GetInformation(key).TryCast(Of Information)

            If info Is Nothing OrElse info.Value.Number Is Nothing Then
                Return 0
            Else
                Return info.Value.Number
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationString(section As Section, key$) As String
            Dim info = section.GetInformation(key).TryCast(Of Information)

            If info Is Nothing OrElse info.Value.StringWithMarkup Is Nothing Then
                Return ""
            Else
                Return info.Value.StringWithMarkup.FirstOrDefault?.String
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationStrings(section As Section, key$, Optional multipleInfo As Boolean = False) As String()
            Dim info = section.GetInformation(key, multipleInfo)

            If info Is Nothing Then
                Return {}
            ElseIf multipleInfo Then
                Dim data As Object() = info _
                    .TryCast(Of Information()) _
                    .Select(Function(i) i.InfoValue) _
                    .Where(Function(s) Not s Is Nothing) _
                    .ToArray

                If data.IsNullOrEmpty Then
                    Return {}
                Else
                    Return data _
                        .Select(Function(part)
                                    Return castStrings(part)
                                End Function) _
                        .IteratesALL _
                        .ToArray
                End If
            Else
                ' 当只有一个字符串的时候,可能会错误的判断为字符串对象
                ' 而非字符串数组
                ' 在这里需要检查一下
                Dim data = info.TryCast(Of Information).InfoValue

                If data Is Nothing Then
                    Return {}
                Else
                    Return castStrings(data)
                End If
            End If
        End Function

        Private Function castStrings(part As Object) As String()
            If part.GetType Is GetType(String) Then
                Return {DirectCast(part, String)}
            ElseIf part.GetType Is GetType(String()) Then
                Return DirectCast(part, String())
            ElseIf part.GetType.IsInheritsFrom(GetType(Array)) Then
                Return (From o In DirectCast(part, Array).AsQueryable Select Scripting.ToString(o)).ToArray
            Else
                Return {Scripting.ToString(part)}
            End If
        End Function

        <Extension>
        Private Function matchName(i As Information, key$) As Boolean
            If key.StringEmpty Then
                Return i.Name.StringEmpty
            Else
                Return i.Name = key OrElse i.Name.TextEquals(key)
            End If
        End Function

        ''' <summary>
        ''' 如果<paramref name="key"/>是使用索引语法,则索引的起始下标是从零开始的
        ''' </summary>
        ''' <param name="section"></param>
        ''' <param name="key$"></param>
        ''' <param name="multipleInfo"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetInformation(section As Section, key$, Optional multipleInfo As Boolean = False) As [Variant](Of Information, Information())
            If section Is Nothing Then
                Return New Information
            Else
                Return section.InformationNoNull(key, multipleInfo)
            End If
        End Function

        <Extension>
        Private Function InformationNoNull(section As Section, key$, multipleInfo As Boolean) As [Variant](Of Information, Information())
            If multipleInfo Then
                Return section _
                    .Information _
                    .SafeQuery _
                    .Where(Function(i)
                               Return i.matchName(key)
                           End Function) _
                    .ToArray
            Else
                If key.IsPattern("[#]\d+") Then
                    Dim index = key.Trim("#"c).ParseInteger

                    ' get by index
                    Return section _
                        .Information _
                        .SafeQuery _
                        .ElementAtOrDefault(index)
                Else
                    ' get by name
                    Return section _
                        .Information _
                        .SafeQuery _
                        .FirstOrDefault(Function(i)
                                            Return i.matchName(key)
                                        End Function)
                End If
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
            Return section.GetInformation(key) _
                ?.TryCast(Of Information) _
                ?.Table
        End Function
    End Module
End Namespace
