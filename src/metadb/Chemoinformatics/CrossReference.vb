Imports System.Xml.Serialization

''' <summary>
''' the external database cross reference
''' </summary>
Public Interface CrossReference

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
