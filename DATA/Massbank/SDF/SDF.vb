#Region "Microsoft.VisualBasic::76ee535cd54d0bf090d722fb535338da, Massbank\SDF\SDF.vb"

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

'     Class SDF
' 
'         Properties: [Structure], Comment, ID, MetaData, Software
' 
'         Function: IterateParser, MoleculePopulator, ScanKeys, StreamParser
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace File

    ' https://pubchemdocs.ncbi.nlm.nih.gov/data-specification

    ''' <summary>
    ''' #### Structure-data file
    ''' 
    ''' SDF or Structures Data File is a common file format developed by Molecular Design Limited 
    ''' to handle a list of molecular structures with associated properties. 
    ''' The file format has been published (Dalby et al., J. Chem. Inf. Comput. Sci. 1992, 32: 244-255).
    ''' 
    ''' + LMSD Structure-data file (http://www.lipidmaps.org/resources/downloads/index.html)
    ''' + PubChem FTP SDF(ftp://ftp.ncbi.nlm.nih.gov/pubchem/Compound/CURRENT-Full/SDF)
    ''' </summary>
    ''' <remarks>
    ''' https://cactus.nci.nih.gov/SDF_toolkit/
    ''' 
    ''' The SDF Toolkit in Perl 5
    ''' 
    ''' Dalby A, Nourse JG, Hounshell WD, Gushurst Aki, Grier DL, Leland BA, Laufer J. 
    ''' Description of several chemical-structure file formats used by computer-programs developed at Molecular Design Limited. 
    ''' Journal of Chemical Information and Computer Sciences 
    ''' 32:(3) 244-255, May-Jun 1992.
    ''' 
    ''' http://www.nonlinear.com/progenesis/sdf-studio/v0.9/faq/sdf-file-format-guidance.aspx
    ''' https://en.wikipedia.org/wiki/Chemical_table_file#SDF
    ''' </remarks>
    Public Class SDF : Implements INamedValue

        ''' <summary>
        ''' The name of the molecule
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property ID As String Implements IKeyedEntity(Of String).Key
        ''' <summary>
        ''' Details of the software used to generate the compound structure
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Software As String

        <XmlText>
        Public Property Comment As String
        Public Property [Structure] As [Structure]
        Public Property MetaData As Dictionary(Of String, String())

        Public ReadOnly Property ChemicalProperties As ChemicalProperties
            Get
                Return New ChemicalProperties(MetaData)
            End Get
        End Property

        Public ReadOnly Property Name As String
            Get
                Return MetaData.TryGetValue("PUBCHEM_IUPAC_NAME", [default]:={}).FirstOrDefault
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{MetaData.TryGetValue("PUBCHEM_COMPOUND_CID").FirstOrDefault}] {Name}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ScanKeys(directory As String) As String()
            Return SDFParser.ScanKeys(directory)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function IterateParser(path As String) As IEnumerable(Of SDF)
            Return SDFParser.IterateParser(path)
        End Function

        ''' <summary>
        ''' Scan and parsing all of the ``*.sdf`` model file in the target <paramref name="directory"/>
        ''' </summary>
        ''' <param name="directory$"></param>
        ''' <param name="takes%"></param>
        ''' <param name="echo"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function MoleculePopulator(directory$, Optional takes% = -1, Optional echo As Boolean = True) As IEnumerable(Of SDF)
            Return SDFParser.MoleculePopulator(directory, takes, echo)
        End Function
    End Class
End Namespace
