#Region "Microsoft.VisualBasic::a8e3c8576da3ca3ac0f5394a1e420e8e, metadb\SMILES\Language\Token.vb"

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

    '   Total Lines: 41
    '    Code Lines: 30 (73.17%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (26.83%)
    '     File Size: 1.09 KB


    '     Class Token
    ' 
    '         Properties: aromatic, charge, ring
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class MultipleTokens
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports std = System.Math

Namespace Language

    Public Class Token : Inherits CodeToken(Of ElementTypes)

        Public Property ring As Integer?
        Public Property charge As Integer?
        Public Property aromatic As Boolean = False

        Sub New(name As ElementTypes, text As String)
            Call MyBase.New(name, text)
        End Sub

        Public Overrides Function ToString() As String
            Dim s As String

            If charge Is Nothing Then
                s = MyBase.ToString
            Else
                s = $"[{text}{std.Abs(charge.Value)}{If(charge > 0, "+", "-")}]"
            End If

            If aromatic Then
                s = s & " (aromatic)"
            End If

            Return s
        End Function
    End Class

    Friend Class MultipleTokens : Inherits Token

        Friend ReadOnly Multiple As New List(Of Token)

        Public Sub New()
            MyBase.New(ElementTypes.None, "XXX")
        End Sub
    End Class
End Namespace
