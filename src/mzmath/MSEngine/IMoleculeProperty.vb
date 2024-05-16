#Region "Microsoft.VisualBasic::37c666a7ef7b93d77a6631e21967d257, mzmath\MSEngine\IMoleculeProperty.vb"

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

    '   Total Lines: 32
    '    Code Lines: 9
    ' Comment Lines: 19
    '   Blank Lines: 4
    '     File Size: 1.30 KB


    ' Interface IMoleculeProperty
    ' 
    '     Properties: Formula, InChIKey, Name, Ontology, SMILES
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
