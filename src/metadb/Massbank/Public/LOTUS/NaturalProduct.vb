Imports System.IO
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Metabolite = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

Namespace LOTUS

    ''' <summary>
    ''' Natural Products Online is an open source project for Natural Products (NPs) storage,
    ''' search and analysis. This page hosts LOTUS, one of the biggest and best annotated
    ''' resources for NPs occurrences available free of charge and without any restriction. 
    ''' LOTUS is a living database, which is hosted both here and on Wikidata.The Wikidata 
    ''' version allows for community curation and addition of novel data. The current version
    ''' allows a more user friendly experience (such as structural search, taxonomy oriented 
    ''' query, flat table and structures exports). If you use LOTUS in your research, please 
    ''' cite the following work: Adriano Rutz, Maria Sorokina, Jakub Galgonek, Daniel Mietchen,
    ''' Egon Willighagen, Arnaud Gaudry, James G Graham, Ralf Stephan, Roderic Page, Jiří 
    ''' Vondrášek, Christoph Steinbeck, Guido F Pauli, Jean-Luc Wolfender, Jonathan Bisson, 
    ''' Pierre-Marie Allard (2022) The LOTUS initiative for open knowledge management in 
    ''' natural products research. eLife 11:e70780. https://doi.org/10.7554/eLife.70780
    ''' </summary>
    ''' <remarks>
    ''' https://lotus.naturalproducts.net/
    ''' </remarks>
    Public Class NaturalProduct

        Public Property lotus_id As String
        Public Property wikidata_id As String
        Public Property smiles As String
        Public Property inchi As String
        Public Property inchikey As String
        Public Property traditional_name As String
        Public Property synonyms As String()
        Public Property iupac_name As String
        Public Property molecular_formula As String


        ''' <summary>
        ''' Convert the lotus natural product model as mzkit internal metabolite object.
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateMetabolite() As Metabolite
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Parse the lotus NPOC2021 bson dump file as metabolite data model
        ''' </summary>
        ''' <param name="NPOC2021"></param>
        ''' <returns></returns>
        Public Shared Iterator Function Parse(NPOC2021 As Stream) As IEnumerable(Of NaturalProduct)
            For Each np As JsonObject In BSONFormat.LoadList(NPOC2021)
                Yield np.CreateObject(Of NaturalProduct)(decodeMetachar:=False)
            Next
        End Function
    End Class
End Namespace