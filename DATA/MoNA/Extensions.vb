#Region "Microsoft.VisualBasic::acbcd11fadc8694b0d42e9fa1fd1b4b9, DATA\MoNA\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: AsDataFrame, FillData, LipidBlastParser
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.MassSpectrum.Assembly.ASCII.MSP

<HideModuleName>
Public Module Extensions

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

    Friend ReadOnly names As Dictionary(Of String, String)
    Friend ReadOnly fields As Dictionary(Of BindProperty(Of ColumnAttribute))

    Sub New()
        names = Mappings.FieldNameMappings(Of MetaData)(explict:=True, reversed:=True)
        fields = Mappings.GetFields(Of MetaData).ToDictionary

        For Each field As BindProperty(Of ColumnAttribute) In fields.Values.ToArray
            If Not fields.ContainsKey(field.member.Name) Then
                fields.Add(field.member.Name, field)
            End If
        Next
    End Sub

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

    ''' <summary>
    ''' 从头部的<see cref="MspData.Comments"/>字符串之中解析出具体的物质注释信息
    ''' </summary>
    ''' <param name="comments$"></param>
    ''' <returns></returns>
    <Extension> Public Function FillData(comments As NameValueCollection) As MetaData
        Dim meta As Object = New MetaData
        Dim castValue As Object

        For Each field As BindProperty(Of ColumnAttribute) In fields.Values
            Dim name$ = field.Identity

            If field.Type.IsInheritsFrom(GetType(Array)) Then
                Dim value As String()

                value = comments.GetValues(name)

                If value.IsNullOrEmpty Then
                    value = comments.GetValues(names(name))
                End If

                Call field.SetValue(meta, value)
            Else
                Dim value$ = comments(name)

                If value.StringEmpty Then
                    value = comments(names(name))
                End If

                castValue = Scripting.CTypeDynamic(value, field.Type)

                Call field.SetValue(meta, castValue)
            End If

            'table.Remove(name)
            'table.Remove(names(name))
        Next

        'If table.Count > 0 Then
        '    Throw New NotImplementedException(table.ToDictionary.GetJson)
        'End If

        Return DirectCast(meta, MetaData)
    End Function
End Module

