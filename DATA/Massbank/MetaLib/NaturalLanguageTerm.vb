#Region "Microsoft.VisualBasic::6b470b3a6b286bc6840444f626334e99, Massbank\MetaLib\NaturalLanguageTerm.vb"

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

    ' Module NaturalLanguageTerm
    ' 
    '     Function: ParseVendorName
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Public Module NaturalLanguageTerm

    <Extension> Public Function ParseVendorName(text As String) As String
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
        ElseIf postfix = Trim(text) Then
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
End Module

