#Region "Microsoft.VisualBasic::40fbe21a1ea2eaba9c253acce2a1cd45, metadb\Massbank\Public\CASDetails.vb"

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

    '   Total Lines: 72
    '    Code Lines: 57 (79.17%)
    ' Comment Lines: 3 (4.17%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (16.67%)
    '     File Size: 2.46 KB


    ' Class CASDetails
    ' 
    '     Properties: canonicalSmile, experimentalProperties, hasMolfile, images, inchi
    '                 inchikey, molecularFormula, molecularMass, name, propertyCitations
    '                 replacedRns, rn, smile, synonyms, uri
    ' 
    '     Function: GetDetails, GetModel, ToString
    ' 
    ' Class propertyCitation
    ' 
    '     Properties: docUri, source, sourceNumber
    ' 
    ' Class experimentalProperty
    ' 
    '     Properties: [property], name, sourceNumber
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite.CrossReference
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Metabolite = BioNovoGene.BioDeep.Chemoinformatics.Metabolite.MetaLib

''' <summary>
''' The metabolite data model from the cas registry details
''' </summary>
Public Class CASDetails

    Public Property uri As String
    Public Property rn As String
    Public Property name As String
    Public Property images As String()
    Public Property inchi As String
    Public Property inchikey As String
    Public Property smile As String
    Public Property canonicalSmile As String
    Public Property molecularFormula As String
    Public Property molecularMass As String
    Public Property experimentalProperties As experimentalProperty()
    Public Property propertyCitations As propertyCitation()
    Public Property synonyms As String()
    Public Property replacedRns As String()
    Public Property hasMolfile As Boolean

    Public Overrides Function ToString() As String
        Return $"{name} ({molecularFormula.StripHTMLTags})"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function GetDetails(cas_id As String) As CASDetails
        Return $"https://commonchemistry.cas.org/api/detail?cas_rn={cas_id}".GET.LoadJSON(Of CASDetails)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetModel() As Metabolite
        Return New Metabolite With {
            .ID = rn,
            .formula = molecularFormula.StripHTMLTags,
            .exact_mass = FormulaScanner.EvaluateExactMass(.formula),
            .name = name,
            .IUPACName = name,
            .synonym = synonyms,
            .xref = New xref With {
                .CAS = {rn},
                .InChI = inchi,
                .InChIkey = inchikey,
                .SMILES = smile
            }
        }
    End Function

End Class

Public Class propertyCitation

    Public Property docUri As String
    Public Property sourceNumber As Integer
    Public Property source As String

End Class

Public Class experimentalProperty

    Public Property name As String
    Public Property [property] As String
    Public Property sourceNumber As Integer

End Class
