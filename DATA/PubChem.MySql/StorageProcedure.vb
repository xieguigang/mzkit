Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.MassSpectrum.DATA.File
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Public Module StorageProcedure

    ''' <summary>
    ''' Convert the pubchem sdf repository to mysql database file.
    ''' </summary>
    ''' <param name="repository$"></param>
    ''' <param name="mysql$"></param>
    Public Sub CreateMySqlDatabase(repository$, mysql$)
        Call SDF.MoleculePopulator(directory:=repository, takes:=3) _
            .PopulateData _
            .DoCall(Sub(data)
                        Call LinqExports.ProjectDumping(data, EXPORT:=mysql)
                    End Sub)
    End Sub

    <Extension>
    Private Iterator Function PopulateData(source As IEnumerable(Of SDF)) As IEnumerable(Of MySQLTable)
        For Each molecule As SDF In source
            Dim descriptor As ChemicalDescriptor = molecule.ChemicalProperties
            Dim readStr = molecule.getOne
            Dim readStrings = molecule.getAll
            Dim molJSON$ = molecule.Structure.GetJson

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
                .inchi_key = readStr("RDHQFKQIGNGIED-UHFFFAOYSA-N")
            }

            Yield New mysql.IUPAC With {
                .cas_name = readStr("PUBCHEM_IUPAC_CAS_NAME"),
                .cid = molecule.ID,
                .inchi = readStr("PUBCHEM_IUPAC_INCHI"),
                .name = readStr("PUBCHEM_IUPAC_NAME"),
                .name_markup = readStr("PUBCHEM_IUPAC_NAME_MARKUP"),
                .openeye_name = readStr("PUBCHEM_IUPAC_OPENEYE_NAME"),
                .systematic_name = readStr("PUBCHEM_IUPAC_SYSTEMATIC_NAME"),
                .traditional_name = readStr("PUBCHEM_IUPAC_TRADITIONAL_NAME")
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
