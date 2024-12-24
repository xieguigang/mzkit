#Region "Microsoft.VisualBasic::2ea9a1bb4db8a4fe2ebdf5edf865d7cc, metadb\Massbank\MetaLib\NaturalLanguageTerm.vb"

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

    '   Total Lines: 60
    '    Code Lines: 41 (68.33%)
    ' Comment Lines: 8 (13.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (18.33%)
    '     File Size: 2.13 KB


    '     Module NaturalLanguageTerm
    ' 
    '         Function: IsOligopeptideName, ParseVendorName
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
                Yield name

                If name.StartsWith("L-") OrElse name.StartsWith("D-") Then
                    Dim short$ = name.Substring(2)

                    Yield [short]

                    If [short].EndsWith("ate") Then
                        Yield [short].Substring(0, [short].Length - 3) & " acid"
                    End If
                End If

                If name.EndsWith("ate") Then
                    Yield name.Substring(0, name.Length - 3) & " acid"
                End If
            Next
        End Function

        <Extension>
        Public Iterator Function EnumerateOdorTerms(info As ChemicalDescriptor) As IEnumerable(Of NamedValue(Of String))
            For Each odor As UnitValue In info.Odor.SafeQuery
                Dim terms As String() = Strings.Trim(odor.condition.TrimNewLine).ToLower.Words

                For Each term As String In stopWords.Removes(terms)
                    Yield New NamedValue(Of String)("odor", term, odor.condition)
                Next
            Next
            For Each odor As UnitValue In info.Taste.SafeQuery
                Dim terms As String() = Strings.Trim(odor.condition.TrimNewLine).ToLower.Words

                For Each term As String In stopWords.Removes(terms)
                    Yield New NamedValue(Of String)("taste", term, odor.condition)
                Next
            Next
            For Each odor As UnitValue In info.Color.SafeQuery
                Dim terms As String() = Strings.Trim(odor.condition.TrimNewLine).ToLower.Words

                For Each term As String In stopWords.Removes(terms)
                    Yield New NamedValue(Of String)("color", term, odor.condition)
                Next
            Next
        End Function
    End Module
End Namespace
