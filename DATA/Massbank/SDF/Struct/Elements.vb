#Region "Microsoft.VisualBasic::3a3b6397e82285f42101ae2f53af0a0d, Massbank\SDF\Struct\Elements.vb"

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

'     Class Atom
' 
'         Properties: Atom, Coordination
' 
'         Function: Parse, ToString
' 
'     Class Bound
' 
'         Properties: i, j, Stereo, Type
' 
'         Function: Parse, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Windows.Media.Media3D
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace File

    Public Class Atom

        <XmlAttribute> Public Property Atom As String
        <XmlElement("xyz")>
        Public Property Coordination As Point3D

        Public Overrides Function ToString() As String
            Return $"({Coordination}) {Atom}"
        End Function

        ''' <summary>
        ''' 三维坐标的轴的值只有4位小数
        ''' </summary>
        ''' <param name="tokens"></param>
        ''' <returns></returns>
        Private Shared Iterator Function ensureValidFormat(tokens As String()) As IEnumerable(Of String)
            ' deal with the incorrect format like
            ' 2.0000-9999.9999    0.0000 I   0  0  0  0  0  0  0  0  0  0  0  0
            ' 99999.999949999.1992
            Dim i As Integer = 0
            Dim countDots As Integer

            For Each t As String In tokens
                If i < 3 Then
                    countDots = t.Count("."c)

                    If t.IndexOf("-"c) > 0 Then
                        With t.Split("-"c)
                            Yield .First

                            For Each token As String In .Skip(1)
                                Yield "-" & token
                            Next
                        End With
                    ElseIf countDots > 1 Then
                        Dim posDot As Integer
                        Dim axis As String

                        For j As Integer = 0 To countDots - 2
                            posDot = InStr(t, ".")
                            axis = Mid(t, 1, posDot + 4)
                            t = Mid(t, axis.Length)

                            Yield axis
                        Next

                        Yield t
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
                .X = t(0),
                .Y = t(1),
                .Z = t(2)
            }
            Dim name$ = t(3)

            Return New Atom With {
                .Atom = name,
                .Coordination = xyz
            }
        End Function
    End Class

    ''' <summary>
    ''' Connection between atoms
    ''' </summary>
    Public Class Bound

        <XmlAttribute> Public Property i As Integer
        <XmlAttribute> Public Property j As Integer
        <XmlAttribute> Public Property Type As BoundTypes
        <XmlAttribute> Public Property Stereo As BoundStereos

        Public Overrides Function ToString() As String
            Return $"[{i}, {j}] {Type} AND {Stereo}"
        End Function

        Public Shared Function Parse(line As String) As Bound
            Dim t$() = line.StringSplit("\s+")
            Dim i% = t(0)
            Dim j = t(1)
            Dim type As BoundTypes = CInt(t(2))
            Dim stereo As BoundStereos = CInt(t(3))

            Return New Bound With {
                .i = i,
                .j = j,
                .Type = type,
                .Stereo = stereo
            }
        End Function
    End Class
End Namespace
