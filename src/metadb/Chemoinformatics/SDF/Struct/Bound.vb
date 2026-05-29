#Region "Microsoft.VisualBasic::f85a285019bc083f4da4da0978bbbe46, metadb\Chemoinformatics\SDF\Struct\Bound.vb"

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

    '   Total Lines: 55
    '    Code Lines: 33 (60.00%)
    ' Comment Lines: 14 (25.45%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (14.55%)
    '     File Size: 1.67 KB


    '     Class Bound
    ' 
    '         Properties: i, j, Stereo, Type
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.GraphTheory.Network

Namespace SDF.Models

    ''' <summary>
    ''' Connection between atoms
    ''' </summary>
    ''' <remarks>
    ''' [i,j] index tuple of the connected atoms
    ''' </remarks>
    Public Class Bound : Implements IndexEdge

        ''' <summary>
        ''' index of atom 1
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property i As Integer Implements IndexEdge.U
        ''' <summary>
        ''' index of atom 2
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property j As Integer Implements IndexEdge.V
        <XmlAttribute> Public Property Type As BoundTypes
        <XmlAttribute> Public Property Stereo As BoundStereos

        Sub New()
        End Sub

        Sub New(i As Integer, j As Integer, Optional type As BoundTypes = BoundTypes.Single)
            Me.i = i
            Me.j = j
            Me.Type = type
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{i}, {j}] {Type} AND {Stereo}"
        End Function

        Public Shared Function Parse(line As String) As Bound
            Dim t$() = line.StringSplit("\s+")
            Dim i% = t(0)
            Dim j = t(1)
            Dim type As BoundTypes = Byte.Parse(t(2))
            Dim stereo As BoundStereos = CInt(t.ElementAtOrDefault(3, "0"))

            Return New Bound With {
                .i = i,
                .j = j,
                .Type = type,
                .Stereo = stereo
            }
        End Function
    End Class
End Namespace
