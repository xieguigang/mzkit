﻿#Region "Microsoft.VisualBasic::bd6d6a2711bd88d2e9e5c77e4ac5de78, metadb\Massbank\Public\NCBI\PubChem\Web\Query\QueryXml.vb"

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

    '   Total Lines: 148
    '    Code Lines: 92 (62.16%)
    ' Comment Lines: 41 (27.70%)
    '    - Xml Docs: 70.73%
    ' 
    '   Blank Lines: 15 (10.14%)
    '     File Size: 6.70 KB


    '     Class QueryXml
    ' 
    '         Properties: annotation, canonicalsmiles, charge, cid, cmpdname
    '                     cmpdsynonym, complexity, covalentunitcnt, definedatomstereocnt, definedbondstereocnt
    '                     exactmass, gpfamilycnt, gpidcnt, hbondacc, hbonddonor
    '                     heavycnt, inchi, inchikey, isosmiles, isotopeatomcnt
    '                     iupacname, meshheadings, mf, monoisotopicmass, mw
    '                     neighbortype, pclidcnt, polararea, rotbonds, sidsrcname
    '                     totalatomstereocnt, totalbondstereocnt, undefinedatomstereocnt, undefinedbondstereocnt, xlogp
    ' 
    '         Function: CreateMetadata, DownloadURL, GetText, Load, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports Metadata2 = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

Namespace NCBI.PubChem.Web

    ' https://pubchem.ncbi.nlm.nih.gov/sdq/sdqagent.cgi?infmt=json&outfmt=xml&query={%22download%22:%22*%22,%22collection%22:%22compound%22,%22order%22:[%22relevancescore,desc%22],%22start%22:1,%22limit%22:10000000,%22downloadfilename%22:%22PubChem_compound_text_kegg%22,%22where%22:{%22ands%22:[{%22*%22:%22kegg%22}]}}

    ' {%22download%22:%22*%22,%22collection%22:%22compound%22,%22order%22:[%22relevancescore,desc%22],%22start%22:1,%22limit%22:10000000,%22downloadfilename%22:%22PubChem_compound_text_kegg%22,%22where%22:{%22ands%22:[{%22*%22:%22kegg%22}]}}
    ' {"download":"*","collection":"compound","order":["relevancescore,desc"],"start":1,"limit":10000000,"downloadfilename":"PubChem_compound_text_kegg","where":{"ands":[{"*":"kegg"}]}}

    ''' <summary>
    ''' result data set for pubchem query summary, download in xml format
    ''' </summary>
    ''' <remarks>
    ''' [Download]
    ''' Summary (Search Results)
    ''' XML format
    ''' 
    ''' this xml data model is a kind of summary of the pubchem <see cref="PugViewRecord"/> xml data.
    ''' </remarks>
    ''' 
    <XmlType("row"), XmlRoot("row")>
    Public Class QueryXml

        Public Property cid As Integer
        ''' <summary>
        ''' a string array
        ''' </summary>
        ''' <returns></returns>
        Public Property cmpdname As Object
        Public Property mw As Double
        Public Property mf As String
        Public Property polararea As Double
        Public Property complexity As Double
        Public Property xlogp As Double
        Public Property heavycnt As Double
        Public Property hbonddonor As Double
        Public Property hbondacc As Double
        Public Property rotbonds As Double
        Public Property inchi As String
        Public Property isosmiles As String
        Public Property canonicalsmiles As String
        Public Property inchikey As String
        Public Property iupacname As String
        Public Property exactmass As Double
        Public Property monoisotopicmass As Double
        Public Property charge As Double
        Public Property covalentunitcnt As Double
        Public Property isotopeatomcnt As Double
        Public Property totalatomstereocnt As Double
        Public Property definedatomstereocnt As Double
        Public Property undefinedatomstereocnt As Double
        Public Property totalbondstereocnt As Double
        Public Property definedbondstereocnt As Double
        Public Property undefinedbondstereocnt As Double
        Public Property pclidcnt As Double
        Public Property gpidcnt As Double
        Public Property gpfamilycnt As Double
        Public Property neighbortype As String

        ''' <summary>
        ''' a string array
        ''' </summary>
        ''' <returns></returns>
        Public Property meshheadings As Object
        ''' <summary>
        ''' a string array
        ''' </summary>
        ''' <returns></returns>
        Public Property sidsrcname As Object
        ''' <summary>
        ''' a string array
        ''' </summary>
        ''' <returns></returns>
        Public Property annotation As Object
        ''' <summary>
        ''' a string array
        ''' </summary>
        ''' <returns></returns>
        Public Property cmpdsynonym As Object

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CreateMetadata() As Metadata2
            Return MetadataConvertor.CreateMetadata(Me)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Iterator Function Load(filepath As String) As IEnumerable(Of QueryXml)
            For Each metabo As QueryXml In filepath.LoadUltraLargeXMLDataSet(Of QueryXml)(
                typeName:="row",
                variants:={}
            )
                ' 20231127
                '
                ' some data field may be variant type:
                ' could be a single string value
                ' or a string array list object
                ' this operation for unify such variant type problem
                Dim t_anno As XmlNode() = TryCast(metabo.annotation, XmlNode())
                Dim t_mesh As XmlNode() = TryCast(metabo.meshheadings, XmlNode())
                Dim t_sids As XmlNode() = TryCast(metabo.sidsrcname, XmlNode())
                Dim t_name As XmlNode() = TryCast(metabo.cmpdsynonym, XmlNode())
                Dim t_cname As XmlNode() = TryCast(metabo.cmpdname, XmlNode())

                metabo.cmpdname = GetText(t_cname).ToArray
                metabo.annotation = GetText(t_anno).ToArray
                metabo.meshheadings = GetText(t_mesh).ToArray
                metabo.sidsrcname = GetText(t_sids).ToArray
                metabo.cmpdsynonym = GetText(t_name).ToArray

                Yield metabo
            Next
        End Function

        Private Shared Iterator Function GetText(elements As XmlNode()) As IEnumerable(Of String)
            If elements Is Nothing Then
                Return
            End If

            For Each xml As XmlNode In elements
                If xml.NodeType = XmlNodeType.Text Then
                    Yield xml.InnerText
                Else
                    Yield xml.ChildNodes(0).InnerText
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return Me.GetXml
        End Function

        Public Shared Function DownloadURL(q As String, Optional limit As Integer = 10000000) As String
            ' {"download":"*","collection":"compound","order":["relevancescore,desc"],"start":1,"limit":10000000,"downloadfilename":"PubChem_compound_text_kegg","where":{"ands":[{"*":"kegg"}]}}
            Dim json As String = sprintf(<JSON>{"download":"*","collection":"compound","order":["relevancescore,desc"],"start":1,"limit":%s,"downloadfilename":"PubChem_compound_text_kegg","where":{"ands":[{"*":"%s"}]}}</JSON>,
                                         limit,
                                         q.Replace(""""c, "'")).UrlEncode
            Dim url As String = $"https://pubchem.ncbi.nlm.nih.gov/sdq/sdqagent.cgi?infmt=json&outfmt=xml&query={json}"

            Return url
        End Function
    End Class
End Namespace
