Imports System.Data.Linq.Mapping

Namespace ASCII.MSP

    ''' <summary>
    ''' 通过解析<see cref="MspData.Comments"/>字段而得到的数据读取的模型对象
    ''' </summary>
    Public Class MetaData

        Public Property Mz_exact As Double
        Public Property Parent As Double

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
        <Column(Name:="collision energy voltage")>
        Public Property collision_energy_voltage As String
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
        Public Property chebi As String
        Public Property hmdb As String
        Public Property lipidmaps As String

        Public Overrides Function ToString() As String
            Return accession
        End Function
    End Class
End Namespace