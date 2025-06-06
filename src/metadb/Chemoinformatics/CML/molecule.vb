﻿#Region "Microsoft.VisualBasic::c57dc327def10c830cebd500a5b0c372, metadb\Chemoinformatics\CML\molecule.vb"

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

    '   Total Lines: 66
    '    Code Lines: 42 (63.64%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 24 (36.36%)
    '     File Size: 1.56 KB


    '     Class bond
    ' 
    '         Properties: atomRefs2, id, order
    ' 
    '     Class atom
    ' 
    '         Properties: elementType, hydrogenCount, id, x2, y2
    ' 
    '     Class molecule
    ' 
    '         Properties: atomArray, bondArray, id
    ' 
    '     Class ArrayList
    ' 
    '         Properties: id
    ' 
    '     Class atomArray
    ' 
    '         Properties: atoms
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class bondArray
    ' 
    '         Properties: bonds
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace ChemicalMarkupLanguage

    Public Class bond

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property atomRefs2 As String()
        <XmlAttribute> Public Property order As Integer

    End Class

    Public Class atom

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property elementType As String
        <XmlAttribute> Public Property hydrogenCount As Integer
        <XmlAttribute> Public Property x2 As Double
        <XmlAttribute> Public Property y2 As Double

    End Class

    Public Class molecule

        <XmlAttribute>
        Public Property id As String

        Public Property atomArray As atomArray
        Public Property bondArray As bondArray

    End Class

    Public Class ArrayList

        <XmlAttribute> Public Property id As String

    End Class

    Public Class atomArray : Inherits ArrayList

        <XmlElement("atom")> Public Property atoms As atom()

        Sub New()
        End Sub

        Sub New(id As String, atoms As IEnumerable(Of atom))
            Me.id = id
            Me.atoms = atoms.ToArray
        End Sub

    End Class

    Public Class bondArray : Inherits ArrayList

        <XmlElement("bond")> Public Property bonds As bond()

        Sub New()
        End Sub

        Sub New(id As String, bonds As IEnumerable(Of bond))
            Me.id = id
            Me.bonds = bonds.ToArray
        End Sub

    End Class
End Namespace
