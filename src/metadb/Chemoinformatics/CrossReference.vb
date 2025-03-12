#Region "Microsoft.VisualBasic::d14ebd15e8b3f5204ce9219a2472d3f3, metadb\Chemoinformatics\CrossReference.vb"

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

    '   Total Lines: 43
    '    Code Lines: 24 (55.81%)
    ' Comment Lines: 15 (34.88%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (9.30%)
    '     File Size: 1.16 KB


    ' Interface ICrossReference
    ' 
    '     Properties: CAS, chebi, ChEMBL, ChemIDplus, chemspider
    '                 DrugBank, foodb, HMDB, InChI, InChIkey
    '                 KEGG, KEGGdrug, KNApSAcK, lipidmaps, MeSH
    '                 MetaCyc, metlin, pubchem, SMILES, Wikipedia
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

''' <summary>
''' the external database cross reference
''' </summary>
Public Interface ICrossReference

    ''' <summary>
    ''' chebi main id, Chemical Entities of Biological Interest (ChEBI)
    ''' </summary>
    ''' <returns></returns>
    Property chebi As String
    Property KEGG As String
    Property KEGGdrug As String
    ''' <summary>
    ''' The pubchem cid
    ''' </summary>
    ''' <returns></returns>
    Property pubchem As String
    Property HMDB As String
    Property metlin As String
    Property DrugBank As String
    Property ChEMBL As String
    Property Wikipedia As String
    Property lipidmaps As String
    Property MeSH As String
    Property ChemIDplus As String
    Property MetaCyc As String
    Property KNApSAcK As String
    Property foodb As String
    Property chemspider As String
    ''' <summary>
    ''' Multiple CAS id may exists
    ''' </summary>
    ''' <returns></returns>
    <XmlElement>
    Property CAS As String()

    Property InChIkey As String
    Property InChI As String
    Property SMILES As String

End Interface

