#Region "Microsoft.VisualBasic::9f05ee6a81ab7e67fd45c5c37327dadb, metadb\Chemoinformatics\SDF\Struct\Atom.vb"

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

    '   Total Lines: 143
    '    Code Lines: 99 (69.23%)
    ' Comment Lines: 20 (13.99%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 24 (16.78%)
    '     File Size: 4.63 KB


    '     Class Atom
    ' 
    '         Properties: Atom, Coordination
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ensureValidFormat, Parse, splitJointNegativeNum, splitJointNum, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace SDF.Models

    ''' <summary>
    ''' atom element key and its spatial coordinates
    ''' </summary>
    Public Class Atom : Implements IReadOnlyId

        ''' <summary>
        ''' the element atom key
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Atom As String Implements IReadOnlyId.Identity
        <XmlElement("xyz")>
        Public Property Coordination As Point3D

        Sub New()
        End Sub

        ''' <summary>
        ''' construct an atom element model by its atom name
        ''' </summary>
        ''' <param name="atom"></param>
        Sub New(atom As String)
            Me.Atom = atom
        End Sub

        Public Overrides Function ToString() As String
            Return $"({Coordination}) {Atom}"
        End Function

        Private Shared Iterator Function splitJointNum(t As String) As IEnumerable(Of String)
            Dim posDot As Integer
            Dim axis As String
            Dim countDots = t.Count("."c)

            For j As Integer = 0 To countDots - 2
                posDot = InStr(t, ".")
                axis = Mid(t, 1, posDot + 4)
                t = Mid(t, axis.Length + 1)

                Yield axis
            Next

            Yield t
        End Function

        Private Shared Iterator Function splitJointNegativeNum(t As String) As IEnumerable(Of String)
            Dim offset As Integer = 0
            Dim countDots As Integer

            With t.Split("-"c)
                t = .First

                If String.IsNullOrEmpty(t) Then
                    ' 第一个数是负数
                    t = "-" & .ByRef()(1)
                    offset = 1
                End If

                countDots = t.Count("."c)

                If countDots > 1 Then
                    For Each number As String In splitJointNum(t)
                        Yield number
                    Next
                Else
                    Yield t
                End If

                For Each token As String In .Skip(1 + offset)
                    t = "-" & token
                    countDots = t.Count("."c)

                    If countDots > 1 Then
                        For Each number As String In splitJointNum(t)
                            Yield number
                        Next
                    Else
                        Yield t
                    End If
                Next
            End With
        End Function

        ''' <summary>
        ''' 三维坐标的轴的值只有4位小数
        ''' </summary>
        ''' <param name="tokens"></param>
        ''' <returns></returns>
        Private Shared Iterator Function ensureValidFormat(tokens As String()) As IEnumerable(Of String)
            ' deal with the incorrect format like
            ' 2.0000-9999.9999    0.0000 I   0  0  0  0  0  0  0  0  0  0  0  0
            ' -96589-9999.999949999.1992 H   0  0  0  0  0  0  0  0  0  0  0  0
            Dim i As Integer = 0
            Dim countDots As Integer

            For Each t As String In tokens
                If i < 3 Then
                    countDots = t.Count("."c)

                    If t.LastIndexOf("-"c) > 0 AndAlso t.Count("-"c) > 1 Then
                        For Each number As String In splitJointNegativeNum(t)
                            Yield number
                        Next
                    ElseIf countDots > 1 Then
                        For Each number As String In splitJointNum(t)
                            Yield number
                        Next
                    Else
                        Yield t
                    End If

                    i += 1
                Else
                    Yield t
                End If
            Next
        End Function

        Public Shared Function Parse(line As String) As Atom
            Dim t$() = line.StringSplit("\s+") _
                .DoCall(AddressOf ensureValidFormat) _
                .ToArray
            Dim xyz As New Point3D With {
                .X = Val(t(0)),
                .Y = Val(t(1)),
                .Z = Val(t(2))
            }
            Dim name As String = t(3)

            Return New Atom With {
                .Atom = name,
                .Coordination = xyz
            }
        End Function
    End Class

End Namespace
