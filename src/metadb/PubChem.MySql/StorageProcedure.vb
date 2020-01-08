#Region "Microsoft.VisualBasic::30ebe21c1ab8731b65fd3e0143de60a0, src\metadb\PubChem.MySql\StorageProcedure.vb"

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

' Module StorageProcedure
' 
'     Function: getAll, getOne, LoadXref, PopulateData
' 
'     Sub: CreateMySqlDatabase
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Scripting

''' <summary>
''' Save pubchem sdf data files into mysql database format.
''' </summary>
Public Module StorageProcedure

    Private Function LoadXref(xmlfile As String) As Func(Of String, MetaLib)
        Dim empty As New MetaLib With {.xref = New xref}
        Dim database As BucketDictionary(Of String, MetaLib) = xmlfile _
            .LoadUltraLargeXMLDataSet(Of MetaLib)() _
            .CreateBuckets(
                getKey:=Function(m) m.ID,
                overridesDuplicates:=True
             )

        Call database.ToString.__DEBUG_ECHO

        Return Function(cid)
                   If database.ContainsKey(cid) Then
                       Return database(cid)
                   Else
                       Return empty
                   End If
               End Function
    End Function

    ''' <summary>
    ''' Convert the pubchem sdf repository to mysql database file.
    ''' </summary>
    ''' <param name="repository$"></param>
    ''' <param name="mysql$"></param>
    Public Sub CreateMySqlDatabase(repository$, mysql$, metalib$)
        Call SDF.MoleculePopulator(directory:=repository) _
            .PopulateData(getMetaByCID:=LoadXref(xmlfile:=metalib)) _
            .DoCall(Sub(data)
                        Call LinqExports.ProjectDumping(data, EXPORT:=mysql)
                    End Sub)
    End Sub

    <Extension>
    Private Iterator Function PopulateData(source As IEnumerable(Of SDF), getMetaByCID As Func(Of String, MetaLib)) As IEnumerable(Of MySQLTable)
        For Each molecule As SDF In source
            Dim descriptor As ChemicalDescriptor = molecule.ChemicalProperties
            Dim readStr = molecule.getOne
            Dim readStrings = molecule.getAll
            Dim molJSON$ = molecule.Structure.GetJson
            Dim metainfo As MetaLib = getMetaByCID(molecule.ID)

            If Not metainfo.xref.CAS.IsNullOrEmpty Then
                For Each cas As String In metainfo.xref.CAS
                    Yield New mysql.cas_registry With {
                        .cas_number = cas,
                        .cid = molecule.ID
                    }
                Next
            End If
            If Not metainfo.synonym.IsNullOrEmpty Then
                For Each name As String In metainfo.synonym
                    Yield New mysql.synonym With {
                        .cid = molecule.ID,
                        .synonym_name = name.MySqlEscaping
                    }
                Next
            End If

            Yield New mysql.descriptor With {
                .atom_def_stereo_count = readStr("PUBCHEM_ATOM_DEF_STEREO_COUNT"),
                .cid = molecule.CID,
                .complexity = descriptor.Complexity,
                .hbond_acceptor = descriptor.HydrogenAcceptor,
                .hbond_donor = descriptor.HydrogenDonors,
                .xlogp3_aa = descriptor.XLogP3_AA,
                .tpsa = descriptor.TopologicalPolarSurfaceArea,
                .formula = readStr("PUBCHEM_MOLECULAR_FORMULA"),
                .can_smiles = readStr("PUBCHEM_OPENEYE_CAN_SMILES"),
                .iso_smiles = readStr("PUBCHEM_OPENEYE_ISO_SMILES"),
                .exact_mass = readStr("PUBCHEM_EXACT_MASS"),
                .molecular_weight = readStr("PUBCHEM_MOLECULAR_WEIGHT"),
                .subkeys = readStr("PUBCHEM_CACTVS_SUBSKEYS"),
                .tauto_count = readStr("PUBCHEM_CACTVS_TAUTO_COUNT"),
                .rotatable_bond = descriptor.RotatableBonds,
                .heavy_atom_count = descriptor.HeavyAtoms,
                .atom_udef_stereo_count = readStr("PUBCHEM_ATOM_UDEF_STEREO_COUNT"),
                .bond_def_stereo_count = readStr("PUBCHEM_BOND_DEF_STEREO_COUNT"),
                .bond_udef_stereo_count = readStr("PUBCHEM_BOND_UDEF_STEREO_COUNT"),
                .component_count = readStr("PUBCHEM_COMPONENT_COUNT"),
                .isotopic_atom_count = readStr("PUBCHEM_ISOTOPIC_ATOM_COUNT"),
                .monoisotopic_weight = readStr("PUBCHEM_MONOISOTOPIC_WEIGHT"),
                .total_charge = readStr("PUBCHEM_TOTAL_CHARGE")
            }

            Yield New mysql.compound With {
                .canonicalized = readStr("PUBCHEM_COMPOUND_CANONICALIZED"),
                .cid = molecule.ID,
                .inchi_key = readStr("RDHQFKQIGNGIED-UHFFFAOYSA-N"),
                .chebi = metainfo?.xref?.chebi,
                .common_name = metainfo?.name.MySqlEscaping,
                .hmdb = metainfo?.xref.HMDB,
                .kegg = metainfo?.xref.KEGG
            }

            Yield New mysql.IUPAC With {
                .cas_name = readStr("PUBCHEM_IUPAC_CAS_NAME").MySqlEscaping,
                .cid = molecule.ID,
                .inchi = readStr("PUBCHEM_IUPAC_INCHI"),
                .name = readStr("PUBCHEM_IUPAC_NAME").MySqlEscaping,
                .name_markup = readStr("PUBCHEM_IUPAC_NAME_MARKUP").MySqlEscaping,
                .openeye_name = readStr("PUBCHEM_IUPAC_OPENEYE_NAME").MySqlEscaping,
                .systematic_name = readStr("PUBCHEM_IUPAC_SYSTEMATIC_NAME").MySqlEscaping,
                .traditional_name = readStr("PUBCHEM_IUPAC_TRADITIONAL_NAME").MySqlEscaping
            }

            Yield New mysql.structure With {
                .cid = molecule.ID,
                .bond_annotations = readStrings("PUBCHEM_BONDANNOTATIONS").JoinBy(","),
                .coordinate_type = readStrings("PUBCHEM_COORDINATE_TYPE").JoinBy(","),
                .model_base64 = molJSON.Base64String,
                .checksum = molJSON.MD5
            }
        Next
    End Function

    <Extension>
    Private Function getAll(sdf As SDF) As Func(Of String, String())
        Dim meta As Dictionary(Of String, String()) = sdf.MetaData

        Return Function(key)
                   If meta.ContainsKey(key) Then
                       Return meta(key)
                   Else
                       Return {}
                   End If
               End Function
    End Function

    <Extension>
    Private Function getOne(sdf As SDF) As Func(Of String, String)
        Dim meta As Dictionary(Of String, String()) = sdf.MetaData

        Return Function(key)
                   If meta.ContainsKey(key) Then
                       Return meta(key)(Scan0)
                   Else
                       Return Nothing
                   End If
               End Function
    End Function
End Module
