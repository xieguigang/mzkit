#Region "Microsoft.VisualBasic::cd351dbd9a73ecff3b37ec71364bdc8f, metadb\Chemoinformatics\Lipid\BondPosition.vb"

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

    '   Total Lines: 69
    '    Code Lines: 49 (71.01%)
    ' Comment Lines: 7 (10.14%)
    '    - Xml Docs: 57.14%
    ' 
    '   Blank Lines: 13 (18.84%)
    '     File Size: 2.61 KB


    '     Class BondPosition
    ' 
    '         Properties: [structure], index
    ' 
    '         Function: ParseStructure, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Lipidomics

    Public Class BondPosition

        Public Property index As Integer
        ''' <summary>
        ''' E/Z
        ''' </summary>
        ''' <returns></returns>
        Public Property [structure] As String

        Public Overrides Function ToString() As String
            Return $"{index}{[structure]}"
        End Function

        Friend Shared Iterator Function ParseStructure(components As String) As IEnumerable(Of BondPosition)
            If components = "" Then
                Return
            End If

            Dim debug As String = components
            Dim groupInfo As String = components.GetTagValue("-", failureNoName:=False).Value
            Dim tokens As String() = groupInfo.Split("-"c)
            Dim is_empty As Boolean = tokens.Length = 1 AndAlso tokens(Scan0) = ""

            components = components.Replace($"-{groupInfo}", "")

            If Not is_empty Then
                For Each token As String In tokens
                    Dim index = token.Match("\(\d+[a-zA-Z]\)")
                    Dim t As String = index.StringReplace("\d+", "").Trim("("c, ")"c)

                    ' 20250421 fix of the possible un-matched error:
                    ' ArgumentException: The value cannot be an empty string. (Parameter 'oldValue')
                    '
                    If Not index.StringEmpty() Then
                        token = token.Replace(index, "")
                    Else
                        Call $"lipid structure parser error when deal with: {debug}.".Warning
                    End If

                    index = index.Match("\d+")

                    If Not index.StringEmpty() Then
                        Yield New Group With {
                            .groupName = token,
                            .index = Integer.Parse(index),
                            .[structure] = t
                        }
                    End If
                Next
            End If

            If components <> "" Then
                tokens = components.GetStackValue("(", ")").Split(","c)

                For Each token As String In tokens
                    Dim index As String = token.Match("\d+")
                    Dim t As String = token.StringReplace("\d+", "")

                    Yield New BondPosition With {
                        .index = If(index.StringEmpty, 1, Integer.Parse(index)),
                        .[structure] = t
                    }
                Next
            End If
        End Function
    End Class
End Namespace
