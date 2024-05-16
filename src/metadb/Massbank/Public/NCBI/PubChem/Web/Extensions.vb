#Region "Microsoft.VisualBasic::8cfdf72a4bba582dc0c942a3e9938e70, metadb\Massbank\Public\NCBI\PubChem\Web\Extensions.vb"

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

    '   Total Lines: 207
    '    Code Lines: 155
    ' Comment Lines: 28
    '   Blank Lines: 24
    '     File Size: 8.78 KB


    '     Module Extensions
    ' 
    '         Function: castStrings, GetInformation, (+2 Overloads) GetInformationNumber, GetInformationString, GetInformationStrings
    '                   GetInformationTable, GetReferenceID, InformationNoNull, matchReferenceNumber
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
            Dim info As Information = section.GetInformation(key).TryCast(Of Information)
            Dim value As Double = info.GetInformationNumber

            Return value
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationNumber(info As Information) As Double
            If info Is Nothing OrElse info.Value Is Nothing Then
                Return 0
            ElseIf Not info.Value.Number Is Nothing Then
                Return info.Value.Number
            ElseIf Not info.Value.StringWithMarkup Is Nothing Then
                Dim str As String = CStr(info.InfoValue)

                If str.IsSimpleNumber Then
                    Return Double.Parse(str)
                Else
                    str = str.Match(SimpleNumberPattern)

                    If Not str.StringEmpty Then
                        Return Double.Parse(str)
                    Else
                        Return 0.0
                    End If
                End If
            Else
                Return 0
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetInformationString(section As Section, key$) As String
            Dim info = section.GetInformation(key).TryCast(Of Information)

            If info Is Nothing OrElse
                info.Value Is Nothing OrElse
                info.Value.StringWithMarkup Is Nothing Then

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
        Private Function matchReferenceNumber(i As Information, key$) As Boolean
            If key.StringEmpty Then
                Return i.ReferenceNumber.StringEmpty
            Else
                Return i.ReferenceNumber = key OrElse i.ReferenceNumber.TextEquals(key)
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
                ' Error in <globalEnvironment> -> InitializeEnvironment -> for_loop_[1] -> else_false -> "dumpJSON" -> "dumpJSON" -> dumpJSON<$anonymous_0025a> -> as.list -> "metadata.pugView" -> metadata.pugView
                ' 1. InvalidCastException: Unable to cast object of type 'BioNovoGene.BioDeep.Chemistry.NCBI.PubChem.Information' to type 'BioNovoGene.BioDeep.Chemistry.NCBI.PubChem.Information[]'.
                ' 2. stackFrames:
                ' at BioNovoGene.BioDeep.Chemistry.NCBI.PubChem.MetaInfoReader.GetMetaInfo(PugViewRecord view)
                ' at mzkit.PubChemToolKit.GetMetaInfo(PugViewRecord pugView)

                ' R# source: "demo" <- Call "as.list"(Call "metadata.pugView"(&demo))

                ' pubchem_kit.R#_interop::.metadata.pugView at mzkit.dll:line <unknown>
                ' SMRUCC/R#.call_function."metadata.pugView" at simple_graph.R:line 6
                ' RConversion.R#_interop::.as.list at REnv.dll:line <unknown>
                ' SMRUCC/R#.declare_function.dumpJSON<$anonymous_0025a> at simple_graph.R:line 4
                ' SMRUCC/R#.call_function."dumpJSON" at query.R:line 42
                ' SMRUCC/R#.call_function."dumpJSON" at query.R:line 42
                ' SMRUCC/R#_runtime.n/a.else_false at query.R:line 38
                ' SMRUCC/R#.forloop.for_loop_[1] at query.R:line 32
                ' SMRUCC/R#.n/a.InitializeEnvironment at query.R:line 0
                ' SMRUCC/R#.global.<globalEnvironment> at <globalEnvironment>:line n/a
                If multipleInfo Then
                    Return New Information() {}
                Else
                    Return New Information
                End If
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
                        .FirstOrDefault
                End If
            End If
        End Function

        <Extension>
        Friend Function GetReferenceID(refs As Reference(), sourceName As String, Optional name As Boolean = False) As String
            Dim refObj = refs.SafeQuery _
                .FirstOrDefault(Function(ref)
                                    Return ref.SourceName = sourceName AndAlso ref.Name.Match("\s+Tree", RegexICSng).StringEmpty
                                End Function)

            If refObj Is Nothing Then
                Return Nothing
            ElseIf name Then
                Return refObj.Name
            Else
                Return refObj.SourceID
            End If
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
