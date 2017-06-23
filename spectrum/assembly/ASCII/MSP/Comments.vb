Imports System.Collections.Specialized
Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Parsers
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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

        <Extension> Public Function FillData(comments$) As MetaData
            Dim table As NameValueCollection = comments.ToTable



            Return New MetaData
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

        Public Overrides Function ToString() As String
            Return accession
        End Function
    End Structure
End Namespace