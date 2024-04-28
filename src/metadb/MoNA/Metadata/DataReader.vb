#Region "Microsoft.VisualBasic::d852c22fb665cbfc77c429296f908e1f, E:/mzkit/src/metadb/MoNA//Metadata/DataReader.vb"

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

    '   Total Lines: 42
    '    Code Lines: 30
    ' Comment Lines: 5
    '   Blank Lines: 7
    '     File Size: 1.56 KB


    ' Module DataReader
    ' 
    '     Function: AsDataFrame, LipidBlastParser
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

Public Module DataReader

    <Extension>
    Public Iterator Function AsDataFrame(msp As IEnumerable(Of MspData)) As IEnumerable(Of EntityObject)
        Dim part1, part2 As Dictionary(Of String, String)

        For Each mspValue As MspData In msp
            part1 = mspValue.DictionaryTable(primitiveType:=True)
            part2 = mspValue.Comments.FillData.DictionaryTable

            Yield New EntityObject With {
                .ID = mspValue.DB_id,
                .Properties = part1.Join(part2).ToDictionary
            }
        Next
    End Function

    ''' <summary>
    ''' Annotation comments text parser for lipidBlast database.
    ''' </summary>
    ''' <param name="comments$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LipidBlastParser(comments$) As MetaData
        Dim meta As MetaData = MspData.ParseCommentMetaTable(comments).FillData
        Dim tokens$() = comments.Split(";"c).Skip(1).ToArray

        meta.name = Strings.Trim(tokens(0))
        meta.precursor_type = Strings.Trim(tokens(1))
        meta.scientific_name = Strings.Trim(tokens(2))
        meta.molecular_formula = Strings.Trim(tokens(3))

        Return meta
    End Function
End Module
