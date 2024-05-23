#Region "Microsoft.VisualBasic::c81a78ef06129f87590627636d4964f5, metadb\Massbank\MetaLib\NameRanks\RankingRule.vb"

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

    '   Total Lines: 101
    '    Code Lines: 62 (61.39%)
    ' Comment Lines: 16 (15.84%)
    '    - Xml Docs: 93.75%
    ' 
    '   Blank Lines: 23 (22.77%)
    '     File Size: 3.30 KB


    '     Class RankingRule
    ' 
    '         Function: ToString
    ' 
    '     Class ChemicalFormulaRule
    ' 
    '         Properties: Penalty, Type
    ' 
    '         Function: Match
    ' 
    '     Class NumberRule
    ' 
    '         Properties: Penalty, Type
    ' 
    '         Function: Match
    ' 
    '     Class DatabaseIdRule
    ' 
    '         Properties: Penalty, Type
    ' 
    '         Function: Match
    ' 
    '     Class InchiKeyRule
    ' 
    '         Properties: Penalty, Type
    ' 
    '         Function: Match
    ' 
    '     Class UpperCaseName
    ' 
    '         Properties: Penalty, Type
    ' 
    '         Function: Match
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions

Namespace MetaLib.CommonNames

    Friend MustInherit Class RankingRule

        Public MustOverride ReadOnly Property Penalty As Double
        Public MustOverride ReadOnly Property Type As String
        Public MustOverride Function Match(name As String) As Boolean

        Public Overrides Function ToString() As String
            Return Type
        End Function

    End Class

    ''' <summary>
    ''' avoid the chemical formula string
    ''' Ca3
    ''' </summary>
    Friend Class ChemicalFormulaRule : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 1.5
        Public Overrides ReadOnly Property Type As String = "chemical formula string"

        Public Overrides Function Match(name As String) As Boolean
            Return name.IsPattern("([A-Z]([a-z]?)(\d+)?)+", RegexOptions.Singleline)
        End Function
    End Class

    ''' <summary>
    ''' is number?
    ''' avoid the number as name
    ''' 1.22
    ''' </summary>
    Friend Class NumberRule : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 10000
        Public Overrides ReadOnly Property Type As String = "number"

        Public Overrides Function Match(name As String) As Boolean
            Return name.IsPattern("\d+(\.\d+)?")
        End Function
    End Class

    ''' <summary>
    ''' avoid the database id
    ''' </summary>
    Friend Class DatabaseIdRule : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 2.5
        Public Overrides ReadOnly Property Type As String = "database id"

        Public Overrides Function Match(name As String) As Boolean
            Return name.IsPattern("[a-zA-Z]+\s*\d+")
        End Function
    End Class

    ''' <summary>
    ''' inchikey
    ''' </summary>
    Friend Class InchiKeyRule : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 100
        Public Overrides ReadOnly Property Type As String = "inchikey"

        Public Overrides Function Match(name As String) As Boolean
            If name.ToUpper = name Then
                ' inchikey
                If name.All(Function(c) Char.IsDigit(c) OrElse Char.IsLetter(c) OrElse c = "-"c) Then
                    Return True
                End If
            End If

            Return False
        End Function
    End Class

    Friend Class UpperCaseName : Inherits RankingRule

        Public Overrides ReadOnly Property Penalty As Double = 1.5
        Public Overrides ReadOnly Property Type As String = "upper case name"

        Public Overrides Function Match(name As String) As Boolean
            If name.All(Function(c)
                            If Char.IsLetter(c) AndAlso Char.IsUpper(c) Then
                                Return True
                            ElseIf c = " "c Then
                                Return True
                            Else
                                Return False
                            End If
                        End Function) Then

                Return True
            End If

            Return False
        End Function
    End Class
End Namespace
