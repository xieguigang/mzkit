#Region "Microsoft.VisualBasic::6aef9a7888261a90dc629b486f928cfa, G:/mzkit/src/metadb/Massbank//MetaLib/NameRanking.vb"

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

    '   Total Lines: 85
    '    Code Lines: 56
    ' Comment Lines: 16
    '   Blank Lines: 13
    '     File Size: 3.25 KB


    '     Module NameRanking
    ' 
    '         Function: Ranking, Score
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges

Namespace MetaLib

    Public Module NameRanking

        ReadOnly empty_symbols As Index(Of String) = {".", "_", "?"}

        ''' <summary>
        ''' the white space is exclude from the symbol list, due to 
        ''' the reason of the white space is recommended in the 
        ''' common name, example as: "Magneson I"
        ''' </summary>
        ReadOnly symbols As Char() = {"-", "/", "\", ":", "<", ">", "?", "(", ")", "[", "]", "{", "}", "|", ";", ",", "'", """"c, ".", "_"}

        Public Function Score(name As String, Optional maxLen As Integer = 32, Optional minLen As Integer = 5) As Double
            If name.StringEmpty(testEmptyFactor:=True) OrElse name Like empty_symbols Then
                Return -1
            End If

            Dim eval As Double

            If name.Length < minLen Then
                eval = 1
            ElseIf name.Length < maxLen Then
                eval = 10 * (maxLen / name.Length)
            Else
                eval = 10 / name.Length
            End If

            ' avoid the chemical formula string
            If name.IsPattern("([A-Z]([a-z]?)(\d+)?)+", RegexOptions.Singleline) Then
                eval /= 1.356
            End If
            ' is number?
            ' avoid the number as name
            If name.IsPattern("\d+(\.\d+)?") Then
                eval /= 10000
            End If
            ' avoid the database id
            If name.IsPattern("[a-zA-Z]+\s*\d+") Then
                eval /= 2.3
            End If
            If name.All(Function(c)
                            If Char.IsLetter(c) AndAlso Char.IsUpper(c) Then
                                Return True
                            ElseIf c = " "c Then
                                Return True
                            Else
                                Return False
                            End If
                        End Function) Then
                eval *= 0.953
            End If

            Dim count As Integer = Aggregate c As Char
                                   In symbols
                                   Into Sum(name.Count(c))
            eval /= (count / 3 + 1)

            Return eval
        End Function

        ''' <summary>
        ''' ranking the input names with score order in desc
        ''' </summary>
        ''' <param name="names"></param>
        ''' <param name="maxLen"></param>
        ''' <param name="minLen"></param>
        ''' <returns></returns>
        Public Function Ranking(names As IEnumerable(Of String),
                                Optional maxLen As Integer = 32,
                                Optional minLen As Integer = 5) As IEnumerable(Of NumericTagged(Of String))

            Return From name As String
                   In names
                   Let score As Double = NameRanking.Score(name, maxLen, minLen)
                   Select out = New NumericTagged(Of String)(score, name)
                   Order By out.tag Descending
        End Function

    End Module
End Namespace
