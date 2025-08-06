﻿#Region "Microsoft.VisualBasic::2ad3664d212085e47bdbb89074953865, metadb\Massbank\Public\lipidMAPS\MetaTable.vb"

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

    '   Total Lines: 53
    '    Code Lines: 33 (62.26%)
    ' Comment Lines: 11 (20.75%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (16.98%)
    '     File Size: 1.96 KB


    '     Module MetaTable
    ' 
    '         Function: ProjectVectors, ReadRepository, WriteRepository
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports lipidMetab = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference

Namespace LipidMaps

    Public Module MetaTable

        Friend ReadOnly properties As Dictionary(Of String, PropertyInfo) = DataFramework.Schema(Of MetaData)(PropertyAccess.Writeable, True)

        ''' <summary>
        ''' cast object set to dataframe columns
        ''' </summary>
        ''' <param name="lipidmaps"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ProjectVectors(lipidmaps As MetaData()) As IEnumerable(Of NamedValue(Of Array))
            For Each [property] As PropertyInfo In properties.Values
                Dim values As Array = Array.CreateInstance([property].PropertyType, lipidmaps.Length)

                For i As Integer = 0 To lipidmaps.Length - 1
                    values.SetValue([property].GetValue(lipidmaps(i)), i)
                Next

                Yield New NamedValue(Of Array) With {
                    .Name = [property].Name,
                    .Value = values
                }
            Next
        End Function

        ''' <summary>
        ''' write the lipidmaps database in messagepack format
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="file"></param>
        ''' <returns></returns>
        <Extension>
        Public Function WriteRepository(data As IEnumerable(Of MetaData), file As Stream) As Boolean
            Call MsgPackSerializer.SerializeObject(data.ToArray, file)
            Call file.Flush()

            Return True
        End Function

        ''' <summary>
        ''' read the msgpack file that contains the lipidmaps annotation data
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ReadRepository(file As Stream) As MetaData()
            Return MsgPackSerializer.Deserialize(Of MetaData())(file)
        End Function

        <Extension>
        Public Function CreateMetabolite(lipid As MetaData) As lipidMetab
            Return New lipidMetab With {
                .ID = lipid.LM_ID,
                .name = lipid.COMMON_NAME,
                .IUPACName = lipid.SYSTEMATIC_NAME,
                .description = lipid.NAME,
                .[class] = lipid.MAIN_CLASS,
                .formula = lipid.FORMULA,
                .exact_mass = FormulaScanner.EvaluateExactMass(.formula),
                .sub_class = lipid.SUB_CLASS,
                .super_class = lipid.CATEGORY,
                .molecular_framework = lipid.CLASS_LEVEL4,
                .synonym = lipid.SYNONYMS,
                .xref = New xref With {
                    .CAS = {},
                    .chebi = lipid.CHEBI_ID,
                    .HMDB = lipid.HMDB_ID,
                    .KEGG = lipid.KEGG_ID,
                    .SMILES = lipid.SMILES,
                    .lipidmaps = lipid.LM_ID,
                    .pubchem = lipid.PUBCHEM_CID,
                    .InChI = lipid.INCHI,
                    .InChIkey = lipid.INCHI_KEY
                }
            }
        End Function
    End Module
End Namespace
