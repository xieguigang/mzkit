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

        Sub New()
            names = Mappings.FieldNameMappings(Of MetaData)(explict:=True)
        End Sub

        <Extension> Public Function FillData(comments$) As MetaData
            Dim table As NameValueCollection = comments.ToTable
            Dim meta As Object = New MetaData
            Dim fields = Mappings.GetFields(Of MetaData)

            For Each field As BindProperty(Of ColumnAttribute) In fields
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
    End Module

    Public Structure MetaData

        Dim accession As String
        Dim author As String
        Dim license As String
        <Column(Name:="exact mass")>
        Dim exact_mass As Double
        Dim instrument As String
        <Column(Name:="instrument type")>
        Dim instrument_type As String
        <Column(Name:="ms level")>
        Dim ms_level As String
        <Column(Name:="ionization energy")>
        Dim ionization_energy As String
        <Column(Name:="ion type")>
        Dim ion_type As String
        <Column(Name:="ionization mode")>
        Dim ionization_mode As String
        <Column(Name:="Last Auto-Curation")>
        Dim Last_AutoCuration As String
        Dim SMILES As String()
        Dim InChI As String
        <Column(Name:="molecular formula")>
        Dim molecular_formula As String
        <Column(Name:="total exact mass")>
        Dim total_exact_mass As Double
        Dim InChIKey As String
        Dim copyright As String
        Dim ionization As String
        <Column(Name:="fragmentation mode")>
        Dim fragmentation_mode As String
        Dim resolution As String
        Dim column As String
        <Column(Name:="flow gradient")>
        Dim flow_gradient As String
        <Column(Name:="flow rate")>
        Dim flow_rate As String
        <Column(Name:="retention time")>
        Dim retention_time As String
        <Column(Name:="solvent a")>
        Dim solvent_a As String
        <Column(Name:="solvent b")>
        Dim solvent_b As String
        <Column(Name:="precursor m/z")>
        Dim precursor_mz As String
        <Column(Name:="precursor type")>
        Dim precursor_type As String
        <Column(Name:="mass accuracy")>
        Dim mass_accuracy As Double
        <Column(Name:="mass error")>
        Dim mass_error As Double
        Dim cas As String
        <Column(Name:="pubchem cid")>
        Dim pubchem_cid As String
        Dim chemspider As String
        <Column(Name:="charge state")>
        Dim charge_state As Integer
        <Column(Name:="compound source")>
        Dim compound_source As String
        <Column(Name:="source file")>
        Dim source_file As String
        Dim origin As String
        Dim adduct As String
        <Column(Name:="ion source")>
        Dim ion_source As String

        Public Overrides Function ToString() As String
            Return accession
        End Function
    End Structure
End Namespace