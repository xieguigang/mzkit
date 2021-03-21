#Region "Microsoft.VisualBasic::a4b262792cadfc5115b47d1f2607373b, src\metadb\Massbank\Public\NCBI\PubChem\MetaData.vb"

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

    '     Class MetaData
    ' 
    '         Properties: PUBCHEM_ATOM_DEF_STEREO_COUNT, PUBCHEM_ATOM_UDEF_STEREO_COUNT, PUBCHEM_BOND_DEF_STEREO_COUNT, PUBCHEM_BOND_UDEF_STEREO_COUNT, PUBCHEM_BONDANNOTATIONS
    '                     PUBCHEM_CACTVS_COMPLEXITY, PUBCHEM_CACTVS_HBOND_ACCEPTOR, PUBCHEM_CACTVS_HBOND_DONOR, PUBCHEM_CACTVS_ROTATABLE_BOND, PUBCHEM_CACTVS_SUBSKEYS
    '                     PUBCHEM_CACTVS_TAUTO_COUNT, PUBCHEM_CACTVS_TPSA, PUBCHEM_COMPONENT_COUNT, PUBCHEM_COMPOUND_CANONICALIZED, PUBCHEM_COMPOUND_CID
    '                     PUBCHEM_COORDINATE_TYPE, PUBCHEM_EXACT_MASS, PUBCHEM_HEAVY_ATOM_COUNT, PUBCHEM_ISOTOPIC_ATOM_COUNT, PUBCHEM_IUPAC_CAS_NAME
    '                     PUBCHEM_IUPAC_INCHI, PUBCHEM_IUPAC_INCHIKEY, PUBCHEM_IUPAC_NAME, PUBCHEM_IUPAC_OPENEYE_NAME, PUBCHEM_IUPAC_SYSTEMATIC_NAME
    '                     PUBCHEM_IUPAC_TRADITIONAL_NAME, PUBCHEM_MOLECULAR_FORMULA, PUBCHEM_MOLECULAR_WEIGHT, PUBCHEM_MONOISOTOPIC_WEIGHT, PUBCHEM_OPENEYE_CAN_SMILES
    '                     PUBCHEM_OPENEYE_ISO_SMILES, PUBCHEM_TOTAL_CHARGE, PUBCHEM_XLOGP3_AA
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Data
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace NCBI.PubChem

    ''' <summary>
    ''' NCBI compound annotation meta data in sdf file.
    ''' </summary>
    Public Class MetaData

        Public Property PUBCHEM_COMPOUND_CID As String
        Public Property PUBCHEM_COMPOUND_CANONICALIZED As String
        Public Property PUBCHEM_CACTVS_COMPLEXITY As String
        Public Property PUBCHEM_CACTVS_HBOND_ACCEPTOR As String
        Public Property PUBCHEM_CACTVS_HBOND_DONOR As String
        Public Property PUBCHEM_CACTVS_ROTATABLE_BOND As String
        Public Property PUBCHEM_CACTVS_SUBSKEYS As String
        Public Property PUBCHEM_IUPAC_OPENEYE_NAME As String
        Public Property PUBCHEM_IUPAC_CAS_NAME As String
        Public Property PUBCHEM_IUPAC_NAME As String
        Public Property PUBCHEM_IUPAC_SYSTEMATIC_NAME As String
        Public Property PUBCHEM_IUPAC_TRADITIONAL_NAME As String
        Public Property PUBCHEM_IUPAC_INCHI As String
        Public Property PUBCHEM_IUPAC_INCHIKEY As String
        Public Property PUBCHEM_XLOGP3_AA As String
        Public Property PUBCHEM_EXACT_MASS As String
        Public Property PUBCHEM_MOLECULAR_FORMULA As String
        Public Property PUBCHEM_MOLECULAR_WEIGHT As String
        Public Property PUBCHEM_OPENEYE_CAN_SMILES As String
        Public Property PUBCHEM_OPENEYE_ISO_SMILES As String
        Public Property PUBCHEM_CACTVS_TPSA As String
        Public Property PUBCHEM_MONOISOTOPIC_WEIGHT As String
        Public Property PUBCHEM_TOTAL_CHARGE As String
        Public Property PUBCHEM_HEAVY_ATOM_COUNT As String
        Public Property PUBCHEM_ATOM_DEF_STEREO_COUNT As String
        Public Property PUBCHEM_ATOM_UDEF_STEREO_COUNT As String
        Public Property PUBCHEM_BOND_DEF_STEREO_COUNT As String
        Public Property PUBCHEM_BOND_UDEF_STEREO_COUNT As String
        Public Property PUBCHEM_ISOTOPIC_ATOM_COUNT As String
        Public Property PUBCHEM_COMPONENT_COUNT As String
        Public Property PUBCHEM_CACTVS_TAUTO_COUNT As String
        Public Property PUBCHEM_COORDINATE_TYPE As String
        Public Property PUBCHEM_BONDANNOTATIONS As String

        ''' <summary>
        ''' Schema cache of current data reader class object
        ''' </summary>
        Shared ReadOnly properties As Dictionary(Of String, PropertyInfo)

        Shared Sub New()
            properties = DataFramework.Schema(Of MetaData)(PropertyAccess.Writeable, True)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Data(sdf As SDF) As MetaData
            Return sdf.Data(Of MetaData)(properties)
        End Function
    End Class
End Namespace
