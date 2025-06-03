﻿#Region "Microsoft.VisualBasic::7e88bc1f083ae3f66aa05abaa845ee87, metadb\Massbank\MetaLib\NaturalLanguageTerm.vb"

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

    '   Total Lines: 133
    '    Code Lines: 95 (71.43%)
    ' Comment Lines: 16 (12.03%)
    '    - Xml Docs: 81.25%
    ' 
    '   Blank Lines: 22 (16.54%)
    '     File Size: 5.58 KB


    '     Module NaturalLanguageTerm
    ' 
    '         Function: ConvertToAcidName, EnumerateOdorTerms, IsOligopeptideName, ParseVendorName, (+2 Overloads) ProcessingNaturalLanguageName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.NLP
Imports Microsoft.VisualBasic.Linq

Namespace MetaLib

    Public Module NaturalLanguageTerm

        ''' <summary>
        ''' 低聚肽的名称匹配模式
        ''' </summary>
        Public Const OligopeptideName$ = "[A-Z][a-z]{2}"

        ReadOnly oligopeptidePattern As New Regex(OligopeptideName, RegexOptions.Singleline)
        ReadOnly stopWords As New StopWords

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function IsOligopeptideName(name As String) As Boolean
            Dim tokens As String() = name.StringSplit("\s+")
            Dim assert As Boolean = tokens.Length > 1 AndAlso tokens.All(Function(part) oligopeptidePattern.Match(part).Value = part)

            Return assert
        End Function

        ''' <summary>
        ''' 尝试将文本之中的仪器厂商的名称解析出来
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ParseVendorName(text As String) As String
            Static prefix$() = {"Thermo", "Waters", "Agilent"}

            For Each name As String In prefix
                If InStr(text, name) > 0 Then
                    Return name
                End If
            Next

            Dim postfix$ = Strings.Trim(text?.Split(","c).LastOrDefault)
            Dim isNamePattern = Function(name As String) As Boolean
                                    Return name.NotEmpty AndAlso
                                       name.IsPattern("[a-z0-9]", RegexICSng)
                                End Function

            If isNamePattern(postfix) Then
                Return postfix
            ElseIf postfix = Strings.Trim(text) Then
                postfix = Strings.Split(text).FirstOrDefault

                If isNamePattern(postfix) Then
                    Return postfix
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        <Extension>
        Public Iterator Function ProcessingNaturalLanguageName(synonym As IEnumerable(Of String)) As IEnumerable(Of String)
            For Each name As String In synonym.Distinct
                For Each processed As String In ProcessingNaturalLanguageName(name)
                    Yield processed
                Next
            Next
        End Function

        Public Iterator Function ProcessingNaturalLanguageName(compoundName As String) As IEnumerable(Of String)
            Yield compoundName

            If compoundName.StartsWith("L-") OrElse compoundName.StartsWith("D-") Then
                Dim short$ = compoundName.Substring(2)

                Yield [short]

                If [short].EndsWith("ate", StringComparison.OrdinalIgnoreCase) Then
                    Yield [short].Substring(0, [short].Length - 3) & "ic acid"
                End If
            End If

            If compoundName.EndsWith("ate", StringComparison.OrdinalIgnoreCase) Then
                Yield compoundName.Substring(0, compoundName.Length - 3) & "ic acid"
            End If
        End Function

        ''' <summary>
        ''' ate to acid name conversion
        ''' </summary>
        ''' <param name="compoundName"></param>
        ''' <returns></returns>
        Public Function ConvertToAcidName(compoundName As String) As String
            ' 检查输入的化合物名称是否以 "ate" 结尾
            If compoundName.EndsWith("ate", StringComparison.OrdinalIgnoreCase) Then
                ' 将 "ate" 替换为 "ic acid"
                Return compoundName.Substring(0, compoundName.Length - 3) & "ic acid"
            Else
                ' 如果不是以 "ate" 结尾，返回原始名称
                Return compoundName
            End If
        End Function

        <Extension>
        Public Iterator Function EnumerateOdorTerms(info As ChemicalDescriptor) As IEnumerable(Of NamedValue(Of String))
            For Each odor As UnitValue In From oi As UnitValue In info.Odor.SafeQuery Where Not oi Is Nothing
                Dim terms As String() = Strings.Trim(odor.condition.TrimNewLine).ToLower.Words

                For Each term As String In stopWords.Removes(terms)
                    Yield New NamedValue(Of String)("odor", term, odor.condition)
                Next
            Next
            For Each odor As UnitValue In From oi As UnitValue In info.Taste.SafeQuery Where Not oi Is Nothing
                Dim terms As String() = Strings.Trim(odor.condition.TrimNewLine).ToLower.Words

                For Each term As String In stopWords.Removes(terms)
                    Yield New NamedValue(Of String)("taste", term, odor.condition)
                Next
            Next
            For Each odor As UnitValue In From oi As UnitValue In info.Color.SafeQuery Where Not oi Is Nothing
                Dim terms As String() = Strings.Trim(odor.condition.TrimNewLine).ToLower.Words

                For Each term As String In stopWords.Removes(terms)
                    Yield New NamedValue(Of String)("color", term, odor.condition)
                Next
            Next
        End Function
    End Module
End Namespace
