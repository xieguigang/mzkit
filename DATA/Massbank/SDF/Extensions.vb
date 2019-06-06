#Region "Microsoft.VisualBasic::d9a39f3b7fee9c16fc0815e55b5e2bde, Massbank\SDF\Extensions.vb"

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

    '     Module SDFExtensions
    ' 
    '         Function: Data, DumpingPubChemAnnotations, Get2DMolStruct, Get3DMolStruct, GetStructure
    '                   MetaValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

Namespace File

    Public Module SDFExtensions

        <Extension>
        Public Function Get2DMolStruct(chemical As ChEBIEntity) As [Structure]
            With chemical.GetStructure("mol", "2D")
                If .StringEmpty Then
                    Return Nothing
                Else
                    Return [Structure].Parse(.ByRef)
                End If
            End With
        End Function

        <Extension>
        Public Function Get3DMolStruct(chemical As ChEBIEntity) As [Structure]
            With chemical.GetStructure("mol", "3D")
                If .StringEmpty Then
                    Return Nothing
                Else
                    Return [Structure].Parse(.ByRef)
                End If
            End With
        End Function

        <Extension>
        Public Function GetStructure(chemical As ChEBIEntity, type$, dim$) As String
            If chemical _
                .ChemicalStructures _
                .IsNullOrEmpty Then

                Return Nothing
            Else
                Return chemical _
                    .ChemicalStructures _
                    .Where(Function(struct)
                               Return struct.type.TextEquals(type) AndAlso
                                      struct.dimension.TextEquals([dim])
                           End Function) _
                    .FirstOrDefault _
                   ?.structure
            End If
        End Function

        ''' <summary>
        ''' Generic method for parsing the SDF meta annotation data.
        ''' </summary>
        ''' <typeparam name="MetaData"></typeparam>
        ''' <param name="sdf"></param>
        ''' <param name="properties">将<see cref="defaultKeys"/>之中的键名称映射为对应的成员属性的字典</param>
        ''' <returns></returns>
        <Extension>
        Public Function Data(Of MetaData As {New, Class})(sdf As SDF, properties As Dictionary(Of String, PropertyInfo)) As MetaData
            Dim table As Dictionary(Of String, String()) = sdf.MetaData
            Dim meta As Object = New MetaData

            For Each key As String In table.Keys
                Call properties(key).SetValue(meta, table(key).JoinBy(ASCII.LF))
            Next

            Return DirectCast(meta, MetaData)
        End Function

        ReadOnly defaultKeys As [Default](Of String()) = (
            <json>
[
    "PUBCHEM_COMPOUND_CID",
    "PUBCHEM_COMPOUND_CANONICALIZED",
    "PUBCHEM_CACTVS_COMPLEXITY",
    "PUBCHEM_CACTVS_HBOND_ACCEPTOR",
    "PUBCHEM_CACTVS_HBOND_DONOR",
    "PUBCHEM_CACTVS_ROTATABLE_BOND",
    "PUBCHEM_CACTVS_SUBSKEYS",
    "PUBCHEM_IUPAC_OPENEYE_NAME",
    "PUBCHEM_IUPAC_CAS_NAME",
    "PUBCHEM_IUPAC_NAME",
    "PUBCHEM_IUPAC_SYSTEMATIC_NAME",
    "PUBCHEM_IUPAC_TRADITIONAL_NAME",
    "PUBCHEM_IUPAC_INCHI",
    "PUBCHEM_IUPAC_INCHIKEY",
    "PUBCHEM_XLOGP3_AA",
    "PUBCHEM_EXACT_MASS",
    "PUBCHEM_MOLECULAR_FORMULA",
    "PUBCHEM_MOLECULAR_WEIGHT",
    "PUBCHEM_OPENEYE_CAN_SMILES",
    "PUBCHEM_OPENEYE_ISO_SMILES",
    "PUBCHEM_CACTVS_TPSA",
    "PUBCHEM_MONOISOTOPIC_WEIGHT",
    "PUBCHEM_TOTAL_CHARGE",
    "PUBCHEM_HEAVY_ATOM_COUNT",
    "PUBCHEM_ATOM_DEF_STEREO_COUNT",
    "PUBCHEM_ATOM_UDEF_STEREO_COUNT",
    "PUBCHEM_BOND_DEF_STEREO_COUNT",
    "PUBCHEM_BOND_UDEF_STEREO_COUNT",
    "PUBCHEM_ISOTOPIC_ATOM_COUNT",
    "PUBCHEM_COMPONENT_COUNT",
    "PUBCHEM_CACTVS_TAUTO_COUNT",
    "PUBCHEM_COORDINATE_TYPE",
    "PUBCHEM_BONDANNOTATIONS",
    "PUBCHEM_XLOGP3",
    "PUBCHEM_NONSTANDARDBOND"
]
            </json>
        ).Value _
         .LoadJSON(Of String())

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="repository$">The NCBI ftp download directory</param>
        ''' <param name="keys">Using user custom selected keys or pubchem default keys for save the meta data.</param>
        ''' <param name="save$"></param>
        ''' <returns></returns>
        Public Function DumpingPubChemAnnotations(repository$, save$, Optional keys$() = Nothing) As Boolean
            Using writer As New WriteStream(Of EntityObject)(path:=save, metaKeys:=keys Or defaultKeys)
                Dim write As EntityObject

                For Each molecule As SDF In SDF.MoleculePopulator(directory:=repository)
                    write = New EntityObject With {
                        .ID = molecule.ID,
                        .Properties = molecule _
                            .MetaData _
                            .ToDictionary(Function(map) map.Key,
                                          Function(map) MetaValue(map.Key, map.Value))
                    }
                    writer.Flush(write)
                Next
            End Using

            Return 0
        End Function

        Private Function MetaValue(key As String, value$()) As String
            Select Case key
                Case "PUBCHEM_COORDINATE_TYPE", "PUBCHEM_BONDANNOTATIONS", "PUBCHEM_NONSTANDARDBOND"
                    Return value.GetJson

                Case Else
                    If value.Length > 1 Then
                        Throw New NotImplementedException($"{key} => {value.GetJson}")
                    Else
                        Return value(Scan0)
                    End If
            End Select
        End Function
    End Module
End Namespace
