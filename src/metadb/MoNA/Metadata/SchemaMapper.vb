#Region "Microsoft.VisualBasic::3b8e054c1179110fb2645e361ce9e24d, metadb\MoNA\Metadata\SchemaMapper.vb"

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

    '   Total Lines: 100
    '    Code Lines: 73 (73.00%)
    ' Comment Lines: 12 (12.00%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 15 (15.00%)
    '     File Size: 3.67 KB


    ' Module SchemaMapper
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: fillArrays, FillData, fillScalar
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

<HideModuleName>
Public Module SchemaMapper

    Friend ReadOnly names As Dictionary(Of String, String)
    ''' <summary>
    ''' has the alias name mapping
    ''' </summary>
    Friend ReadOnly fields As Dictionary(Of BindProperty(Of ColumnAttribute))

    Sub New()
        ' 20221025 the name/alias name between the data
        ' property fields can be not duplictaed or some
        ' information may be loose
        fields = Mappings.GetFields(Of MetaData)(explict:=True) _
            .Select(Function(p) p.GetAliasNames.Select(Function(name) (name, p))) _
            .IteratesALL _
            .Where(Function(m) Not m.name.StringEmpty) _
            .GroupBy(Function(a) a.name) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.First.p
                          End Function)
        names = fields.Values _
            .GroupBy(Function(f) f.memberName) _
            .Select(Function(a) a.First) _
            .FieldNameMappings(Of MetaData)(
                reversed:=True,
                includesAliasNames:=True
             )

        For Each field As BindProperty(Of ColumnAttribute) In fields.Values.ToArray
            If Not fields.ContainsKey(field.memberName) Then
                fields.Add(field.memberName, field)
            End If
        Next
    End Sub

    ''' <summary>
    ''' a unify metadata reader for MoNA database.
    ''' (从头部的<see cref="MspData.Comments"/>字符串之中解析出具体的物质注释信息)
    ''' </summary>
    ''' <param name="comments$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FillData(comments As NameValueCollection) As MetaData
        Dim meta As New MetaData
        Dim type As Type
        Dim castValue As Object

        For Each field As KeyValuePair(Of String, BindProperty(Of ColumnAttribute)) In fields
            type = field.Value.Type

            If type.IsArray Then
                castValue = fillArrays(field.Key, field.Value, comments)
            Else
                castValue = fillScalar(field.Key, field.Value, comments)
            End If

            Call field.Value.SetValue(meta, castValue)
        Next

        Return meta
    End Function

    Private Function fillScalar(name As String, field As BindProperty(Of ColumnAttribute), comments As NameValueCollection) As Object
        Dim value As String = comments(name)
        Dim castValue As Object = field.Parse(value)

        Return castValue
    End Function

    Private Function fillArrays(name As String, field As BindProperty(Of ColumnAttribute), comments As NameValueCollection) As Object
        Dim value As String() = comments.GetValues(name)

        If value.IsNullOrEmpty Then
            Return Nothing
        ElseIf field.Type Is GetType(String()) Then
            Return value
        Else
            Dim vec As Array = Array.CreateInstance(
                elementType:=field.Type.GetElementType,
                length:=value.Length
            )

            For i As Integer = 0 To value.Length - 1
                vec(i) = field.Parse(value(i))
            Next

            Return vec
        End If
    End Function
End Module
