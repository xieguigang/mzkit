#Region "Microsoft.VisualBasic::bc4ebbceb464de7ba19edfbaa97da4c9, Massbank\Public\NCBI\PubChem\PCXml\Elements.vb"

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

    '     Class Count
    ' 
    '         Properties: atom_chiral, atom_chiral_def, atom_chiral_undef, bond_chiral, bond_chiral_def
    '                     bond_chiral_undef, covalent_unit, heavy_atom, isotope_atom, tautomers
    ' 
    '     Class CompoundType
    ' 
    '         Properties: id
    ' 
    '     Class id
    ' 
    '         Properties: cid
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace NCBI.PubChem.PCCompound.Elements

    Public Class Count
        <XmlElement("heavy-atom")> Public Property heavy_atom As Integer
        <XmlElement("atom-chiral")> Public Property atom_chiral As Integer
        <XmlElement("atom-chiral-def")> Public Property atom_chiral_def As Integer
        <XmlElement("atom-chiral-undef")> Public Property atom_chiral_undef As Integer
        <XmlElement("bond-chiral")> Public Property bond_chiral As Integer
        <XmlElement("bond-chiral-def")> Public Property bond_chiral_def As Integer
        <XmlElement("bond-chiral-undef")> Public Property bond_chiral_undef As Integer
        <XmlElement("isotope-atom")> Public Property isotope_atom As Integer
        <XmlElement("covalent-unit")> Public Property covalent_unit As Integer
        <XmlElement("tautomers")> Public Property tautomers As Integer
    End Class

    Public Class CompoundType
        Public Property id As id
    End Class

    Public Class id
        Public Property cid As String
    End Class
End Namespace
