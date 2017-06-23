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

        ReadOnly names As Dictionary(Of String, String)

        Sub New()
            names = New Dictionary(Of String, String)

            With New MetaData
                names(NameOf(.accession)) = NameOf(.accession)
                names(NameOf(.author)) = NameOf(.author)
                names(NameOf(.exact_mass)) = "exact mass"
                names(NameOf(.InChI)) = NameOf(.InChI)
                names(NameOf(.InChIKey)) = NameOf(.InChIKey)
                names(NameOf(.instrument)) = NameOf(.instrument)
                names(NameOf(.instrument_type)) = "instrument type"
                names(NameOf(.ionization_energy)) = "ionization energy"
                names(NameOf(.ionization_mode)) = "ionization mode"
                names(NameOf(.ion_type)) = "ion type"
                names(NameOf(.Last_AutoCuration)) = "Last Auto-Curation"
                names(NameOf(.license)) = NameOf(.license)
                names(NameOf(.molecular_formula)) = "molecular formula"
                names(NameOf(.ms_level)) = "ms level"
                names(NameOf(.SMILES)) = NameOf(.SMILES)
                names(NameOf(.total_exact_mass)) = "total exact mass"
            End With
        End Sub

        <Extension> Public Function FillData(comments$) As MetaData
            Dim table As NameValueCollection = comments.ToTable
            Dim meta As New MetaData With {
                .total_exact_mass = Val(table(names(NameOf(.total_exact_mass)))),
                .SMILES = table.GetValues(NameOf(.SMILES)),
                .accession = table(NameOf(.accession)),
                .author = table(NameOf(.author)),
                .exact_mass = table(names(NameOf(.exact_mass))),
                .InChI = table(NameOf(.InChI)),
                .InChIKey = table(NameOf(.InChIKey)),
                .instrument = table(NameOf(.instrument)),
                .instrument_type = table(names(NameOf(.instrument_type))),
                .ionization_energy = table(names(NameOf(.ionization_energy))),
                .ionization_mode = table(names(NameOf(.ionization_mode))),
                .ion_type = table(names(NameOf(.ion_type))),
                .Last_AutoCuration = table(names(NameOf(.Last_AutoCuration))),
                .license = table(NameOf(.license)),
                .molecular_formula = table(names(NameOf(.molecular_formula))),
                .ms_level = table(names(NameOf(.ms_level)))
            }

            Return meta
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