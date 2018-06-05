Imports System.Collections.Specialized
Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Parsers
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ASCII.MSP

    Public Module Comments

        ''' <summary>
        ''' 解析放置于注释之中的代谢物注释元数据
        ''' </summary>
        ''' <param name="comments$"></param>
        ''' <returns></returns>
        <Extension> Public Function ToTable(comments$) As NameValueCollection
            Dim tokens$() = CLIParser.GetTokens(comments)
            Dim data = tokens _
                .Select(Function(s)
                            Return s.GetTagValue("=", trim:=True)
                        End Function) _
                .GroupBy(Function(o) o.Name)
            Dim table As New NameValueCollection  ' 为了兼容两个SMILES结构

            For Each g In data
                For Each s As NamedValue(Of String) In g
                    Call table.Add(g.Key, s.Value)
                Next
            Next

            Return table
        End Function

        ReadOnly names As Dictionary(Of String, String)
        ReadOnly fields As Dictionary(Of BindProperty(Of ColumnAttribute))

        Sub New()
            names = Mappings.FieldNameMappings(Of MetaData)(explict:=True)
            fields = Mappings.GetFields(Of MetaData).ToDictionary

            For Each field In fields.Values
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
            Dim meta As MetaData = comments.FillData
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
        <Extension> Public Function FillData(comments$) As MetaData
            Dim table As NameValueCollection = comments.ToTable
            Dim meta As Object = New MetaData
            Dim castValue As Object

            For Each field As BindProperty(Of ColumnAttribute) In fields.Values
                Dim name$ = field.Identity

                If field.Type.IsInheritsFrom(GetType(Array)) Then
                    Dim value As String()

                    value = table.GetValues(name)

                    If value.IsNullOrEmpty Then
                        value = table.GetValues(names(name))
                    End If

                    Call field.SetValue(meta, value)
                Else
                    Dim value$ = table(name)

                    If value.StringEmpty Then
                        value = table(names(name))
                    End If

                    castValue = Scripting.CTypeDynamic(value, field.Type)

                    Call field.SetValue(meta, castValue)
                End If
            Next

            Return DirectCast(meta, MetaData)
        End Function

#Region "Reader Views"

        <Extension>
        Public Function Read_compound_source(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple({NameOf(.compound_class), NameOf(.compound_source)})
            End With
        End Function

        <Extension>
        Public Function Read_collision_energy(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple(
                    {
                        NameOf(.collision_energy),
                        NameOf(.collision_energy_voltage),
                        NameOf(.source_voltage),
                        NameOf(.ionization_energy)
                    })
            End With
        End Function

        <Extension>
        Public Function Read_instrument_type(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple({NameOf(.ion_source), NameOf(.instrument_type)})
            End With
        End Function

        ''' <summary>
        ''' 这个函数会自动对保留时间进行单位的转换，返回结果的单位为秒
        ''' </summary>
        ''' <param name="meta"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Read_retention_time(meta As MetaData) As Double
            With meta
                Dim s$ = Trim(.ReadStringMultiple({NameOf(.retention_time)}))

                If s.StringEmpty Then
                    Return 0
                ElseIf InStr(s, "min", CompareMethod.Text) Then
                    Return Val(s) * 60
                Else
                    Return Val(s)
                End If
            End With
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="meta"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Read_precursor_type(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple(
                    {
                        NameOf(.precursor_type),
                        NameOf(.adduct),
                        NameOf(.ion_type)
                    })
            End With
        End Function

        <Extension>
        Public Function Read_source_file(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple({NameOf(.raw_data_file), NameOf(.source_file)})
            End With
        End Function

        <Extension>
        Public Function Read_exact_mass(meta As MetaData) As Double
            With meta
                Dim mw = .ReadDoubleMultiple({NameOf(.exact_mass), NameOf(.exactmass)})
                If mw = 0R Then
                    Return .ReadDoubleMultiple({NameOf(.total_exact_mass)})
                Else
                    Return mw
                End If
            End With
        End Function

        <Extension>
        Public Function Read_CAS(meta As MetaData) As String
            With meta
                Return .ReadStringMultiple({NameOf(.cas), NameOf(.cas_number)})
            End With
        End Function

        <Extension>
        Private Function ReadStringMultiple(meta As MetaData, names$()) As String
            Dim value As Object = meta.ReadMultiple(names)
            Return Scripting.ToString(value)
        End Function

        <Extension>
        Public Function ReadDoubleMultiple(meta As MetaData, names$()) As Double
            Dim s$ = meta.ReadStringMultiple(names)
            Return Val(s)
        End Function

        <Extension>
        Private Function ReadMultiple(meta As MetaData, names$()) As Object
            Dim value As Object = Nothing

            For Each name$ In names
                Dim field As BindProperty(Of ColumnAttribute) = fields(name)

                value = field.GetValue(meta)
                If Not value Is Nothing Then
                    Return value
                End If
            Next

            Return Nothing
        End Function

        <Extension>
        Private Function ReadStringsMultiple(meta As MetaData, names$()) As String()
            Dim value = meta.ReadMultiple(names)
            If value Is Nothing Then
                Return value
            Else
                Return DirectCast(value, String())
            End If
        End Function

        <Extension>
        Private Function ReadDoublesMultiple(meta As MetaData, names$()) As Double()
            Dim value = meta.ReadMultiple(names)
            If value Is Nothing Then
                Return value
            Else
                Return DirectCast(value, Double())
            End If
        End Function
#End Region
    End Module
End Namespace