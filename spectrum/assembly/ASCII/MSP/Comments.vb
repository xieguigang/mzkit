Imports System.Collections.Specialized
Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Parsers
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
        ReadOnly fields As Dictionary(Of BindProperty(Of ColumnAttribute)) = Mappings.GetFields(Of MetaData).ToDictionary

        Sub New()
            names = Mappings.FieldNameMappings(Of MetaData)(explict:=True)
        End Sub

        <Extension> Public Function FillData(comments$) As MetaData
            Dim table As NameValueCollection = comments.ToTable
            Dim meta As Object = New MetaData

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
                    Dim value As String

                    value = table(name)
                    If value.StringEmpty Then
                        value = table(names(name))
                    End If

                    Call field.SetValue(meta, Scripting.CTypeDynamic(value, field.Type))
                End If
            Next

            Return DirectCast(meta, MetaData)
        End Function

#Region "Reader Views"

        <Extension>
        Public Function Read_retention_time(meta As MetaData) As Double
            With meta
                Dim s$ = .ReadStringMultiple({NameOf(.retention_time)})

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
                Return .ReadDoubleMultiple({NameOf(.exact_mass), NameOf(.exactmass)})
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

    Public Class MetaData

        Public Property accession As String
        Public Property author As String
        Public Property license As String
        <Column(Name:="exact mass")>
        Public Property exact_mass As Double
        Public Property instrument As String
        <Column(Name:="instrument type")>
        Public Property instrument_type As String
        <Column(Name:="ms level")>
        Public Property ms_level As String
        <Column(Name:="ionization energy")>
        Public Property ionization_energy As String
        <Column(Name:="ion type")>
        Public Property ion_type As String
        <Column(Name:="ionization mode")>
        Public Property ionization_mode As String
        <Column(Name:="Last Auto-Curation")>
        Public Property Last_AutoCuration As String
        Public Property SMILES As String()
        Public Property InChI As String
        <Column(Name:="molecular formula")>
        Public Property molecular_formula As String
        <Column(Name:="total exact mass")>
        Public Property total_exact_mass As Double
        Public Property InChIKey As String
        Public Property copyright As String
        Public Property ionization As String
        <Column(Name:="fragmentation mode")>
        Public Property fragmentation_mode As String
        Public Property resolution As String
        Public Property column As String
        <Column(Name:="flow gradient")>
        Public Property flow_gradient As String
        <Column(Name:="flow rate")>
        Public Property flow_rate As String
        <Column(Name:="retention time")>
        Public Property retention_time As String
        <Column(Name:="solvent a")>
        Public Property solvent_a As String
        <Column(Name:="solvent b")>
        Public Property solvent_b As String
        <Column(Name:="precursor m/z")>
        Public Property precursor_mz As String
        <Column(Name:="precursor type")>
        Public Property precursor_type As String
        <Column(Name:="mass accuracy")>
        Public Property mass_accuracy As Double
        <Column(Name:="mass error")>
        Public Property mass_error As Double
        Public Property cas As String
        <Column(Name:="cas number")>
        Public Property cas_number As String
        <Column(Name:="pubchem cid")>
        Public Property pubchem_cid As String
        <Column(Name:="pubmed id")>
        Public Property pubmed_id As String
        Public Property chemspider As String
        <Column(Name:="charge state")>
        Public Property charge_state As Integer
        <Column(Name:="compound source")>
        Public Property compound_source As String
        <Column(Name:="compound class")>
        Public Property compound_class As String
        <Column(Name:="source file")>
        Public Property source_file As String
        Public Property origin As String
        Public Property adduct As String
        <Column(Name:="ion source")>
        Public Property ion_source As String
        Public Property exactmass As Double
        <Column(Name:="collision energy")>
        Public Property collision_energy As String
        Public Property kegg As String
        <Column(Name:="capillary temperature")>
        Public Property capillary_temperature As String
        <Column(Name:="source voltage")>
        Public Property source_voltage As String
        <Column(Name:="sample introduction")>
        Public Property sample_introduction As String
        <Column(Name:="raw data file")>
        Public Property raw_data_file As String
        Public Property publication As String
        <Column(Name:="scientific name")>
        Public Property scientific_name As String
        Public Property name As String
        Public Property lineage As String
        Public Property link As String
        Public Property sample As String
        <Column(Name:="ion spray voltage")>
        Public Property ion_spray_voltage As String
        <Column(Name:="fragmentation method")>
        Public Property fragmentation_method As String
        <Column(Name:="spectrum type")>
        Public Property spectrum_type As String
        Public Property source_temperature As String

        Public Overrides Function ToString() As String
            Return accession
        End Function
    End Class
End Namespace