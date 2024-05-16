#Region "Microsoft.VisualBasic::bb72acae88455f03700a6405241137ec, metadb\SMILES\Language\Token.vb"

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

    '   Total Lines: 32
    '    Code Lines: 24
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 903 B


    '     Class Token
    ' 
    '         Properties: charge, ring
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
Imports stdNum = System.Math

Namespace Language

    Public Class Token : Inherits CodeToken(Of ElementTypes)

        Public Property ring As Integer?
        Public Property charge As Integer?

        Sub New(name As ElementTypes, text As String)
            Call MyBase.New(name, text)
        End Sub

        Public Overrides Function ToString() As String
            If charge Is Nothing Then
                Return MyBase.ToString
            Else
                Return $"[{text}{stdNum.Abs(charge.Value)}{If(charge > 0, "+", "-")}]"
            End If
        End Function
    End Class

    Friend Class MultipleTokens : Inherits Token

        Friend ReadOnly Multiple As New List(Of Token)

        Public Sub New()
            MyBase.New(ElementTypes.None, "XXX")
        End Sub
    End Class
End Namespace
