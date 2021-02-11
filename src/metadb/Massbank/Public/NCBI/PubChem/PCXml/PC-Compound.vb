#Region "Microsoft.VisualBasic::58b3d4127bdc9e6c187c844693343f09, Massbank\Public\NCBI\PubChem\PCXml\PC-Compound.vb"

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

    '     Class Compound
    ' 
    '         Properties: count, id
    ' 
    '         Function: LoadFromXml
    ' 
    '         Class count
    ' 
    '             Properties: Count
    ' 
    '         Class id
    ' 
    '             Properties: Type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace NCBI.PubChem.PCCompound

    <XmlType("PC-Compound")> Public Class Compound

        Public Property id As PC.id
        Public Property count As PC.count

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadFromXml(xml As String) As Compound
            Return TrimXml(xml).LoadFromXml(Of Compound)
        End Function
    End Class

    Namespace PC

        Public Class count
            <XmlElement("PC-Count")>
            Public Property Count As Elements.Count
        End Class

        Public Class id
            <XmlElement("PC-CompoundType")>
            Public Property Type As Elements.CompoundType
        End Class
    End Namespace

End Namespace
