Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Interface IMoleculeProperty
    Property Name As String
    Property Formula As Formula
    Property Ontology As String
    Property SMILES As String
    Property InChIKey As String
End Interface

'Public Module MoelculePropertyExtension
'    Private ReadOnly CHEM_OBJECT_BUILDER As IChemObjectBuilder = CDK.Builder
'    Private ReadOnly SMILES_PARSER As SmilesParser = New SmilesParser(CHEM_OBJECT_BUILDER)

'    <Extension()>
'    Public Function ToAtomContainer(molecule As IMoleculeProperty) As IAtomContainer
'        If String.IsNullOrEmpty(molecule.SMILES) Then
'            Throw New InvalidSmilesException("No information on SMILES.")
'        End If
'        Dim container = SMILES_PARSER.ParseSmiles(molecule.SMILES)
'        If Not ConnectivityChecker.IsConnected(container) Then
'            Throw New InvalidSmilesException("The connectivity is not correct.")
'        End If
'        Return container
'    End Function

'    <Extension()>
'    Public Function IsValidInChIKey(molecule As IMoleculeProperty) As Boolean
'        Return Not String.IsNullOrWhiteSpace(molecule.InChIKey) AndAlso molecule.InChIKey.Length = 27
'    End Function
'End Module