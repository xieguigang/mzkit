#Region "Microsoft.VisualBasic::b3044b444ca519df4765c7df98e05762, Massbank\SDF\Elements.vb"

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
    '     Enum BoundTypes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum BoundStereos
    ' 
    '         Other
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Windows.Media.Media3D
Imports System.Xml.Serialization

Namespace File

    Public Class Atom

        <XmlAttribute> Public Property Atom As String
        <XmlElement("xyz")>
        Public Property Coordination As Point3D

        Public Overrides Function ToString() As String
            Return $"({Coordination}) {Atom}"
        End Function

        Public Shared Function Parse(line As String) As Atom
            Dim t$() = line.StringSplit("\s+")
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
