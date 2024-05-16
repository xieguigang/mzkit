#Region "Microsoft.VisualBasic::9adc3b18e1a0ac4d90bc752b353a3a12, metadb\MoNA\Metadata\MetaData.vb"

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

    '   Total Lines: 223
    '    Code Lines: 181
    ' Comment Lines: 11
    '   Blank Lines: 31
    '     File Size: 8.20 KB


    ' Class MetaData
    ' 
    '     Properties: [date], accession, activation_parameter, activation_time, author
    '                 automatic_gain_control, capillary_temperature, capillary_voltage, cas_number, cdl_temperature
    '                 charge_state, chebi, chembl, ChemIDplus, chemspider
    '                 collision_energy_level, collision_energy_voltage, collision_gas, column, column_pressure
    '                 column_temperature, comment, compound_class, copyright, data_format
    '                 derivative_formula, derivative_mass, derivatization_type, desolvation_gas_flow, desolvation_temperature
    '                 drugbank, exact_mass, flow_gradient, flow_rate, fragmentation_method
    '                 fragmentation_mode, hmdb, InChI, InChIKey, instrument
    '                 instrument_type, ion_guide_peak_voltage, ion_guide_voltage, ion_spray_voltage, ionization
    '                 ionization_mode, kegg, knapsack, Last_AutoCuration, lens_voltage
    '                 license, lineage, link, lipidbank, lipidmaps
    '                 mass_accuracy, mass_error, mass_range_mz, Mesh, molecular_formula
    '                 ms_level, name, nebulizer, nebulizing_gas, needle_voltage
    '                 orifice_temp, orifice_temperature, orifice_voltage, origin, precursor_intensity
    '                 precursor_mz, precursor_type, pubchem_cid, pubchem_sid, publication
    '                 pubmed_id, raw_data_file, reanalyze, resolution, resolution_setting
    '                 retention_time, ring_voltage, sample, sample_introduction, sampling_cone
    '                 scanning, scanning_range, scientific_name, SMILES, solvent
    '                 solvent_a, solvent_acetonitrile, solvent_b, source_temperature, spectrum_type
    '                 SPLASH, spray_voltage, tube_lens_voltage, whole, wikipedia
    ' 
    '     Function: GetFormula, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP

''' <summary>
''' 通过解析<see cref="MspData.Comments"/>字段而得到的数据读取的模型对象
''' </summary>
Public Class MetaData

    Public Property accession As String
    Public Property author As String
    Public Property license As String
    <Column("exact mass", "exactmass", "total exact mass", "ExactMass")>
    Public Property exact_mass As Double

    <Column(Name:="collision gas")>
    Public Property collision_gas As String
    <Column(Name:="desolvation gas flow")>
    Public Property desolvation_gas_flow As String
    <Column(Name:="desolvation temperature")>
    Public Property desolvation_temperature As String
    Public Property nebulizer As String
    <Column(Name:="column temperature")>
    Public Property column_temperature As String
    Public Property solvent As String

    Public Property instrument As String
    <Column("instrument type", "ion source")>
    Public Property instrument_type As String
    <Column("Spectrum_type", "ms level")>
    Public Property ms_level As String

    <Column(Name:="ionization mode")>
    Public Property ionization_mode As String
    <Column(Name:="Last Auto-Curation")>
    Public Property Last_AutoCuration As String
    <Column("SMILES", "computed SMILES")>
    Public Property SMILES As String()
    Public Property InChI As String
    <Column("molecular formula", "formula")>
    Public Property molecular_formula As String

    Public Property InChIKey As String
    Public Property SPLASH As String

    ''' <summary>
    ''' 电离模式，通常是ESI
    ''' </summary>
    ''' <returns></returns>
    Public Property ionization As String
    <Column(Name:="fragmentation mode")>
    Public Property fragmentation_mode As String
    Public Property resolution As String
    Public Property column As String
    <Column(Name:="column pressure")>
    Public Property column_pressure As String

    <Column(Name:="flow gradient")>
    Public Property flow_gradient As String
    <Column(Name:="flow rate")>
    Public Property flow_rate As String
    <Column(Name:="retention time")>
    Public Property retention_time As String
    <Column("solvent a", "SOLVENT A")>
    Public Property solvent_a As String
    <Column("solvent b", "SOLVENT B")>
    Public Property solvent_b As String

    Public Property reanalyze As String

    <Column("precursor m/z", "PrecursorMZ", "Mz_exact", "Parent")>
    Public Property precursor_mz As String

    <Column("precursor intensity")>
    Public Property precursor_intensity As Double

    <Column("precursor type", "adduct", "ion type", "Precursor_type")>
    Public Property precursor_type As String

    <Column("mass accuracy", "computed mass accuracy")>
    Public Property mass_accuracy As Double

    <Column("mass error", "computed mass error")>
    Public Property mass_error As Double

    ''' <summary>
    ''' the CAS number
    ''' </summary>
    ''' <returns></returns>
    <Column("cas number", "cas")>
    Public Property cas_number As String()
    <Column("pubchem cid", "pubchem")>
    Public Property pubchem_cid As String
    <Column(Name:="pubchem sid")>
    Public Property pubchem_sid As String
    <Column(Name:="pubmed id")>
    Public Property pubmed_id As String
    Public Property knapsack As String
    Public Property lipidbank As String
    Public Property chemspider As String
    <Column(Name:="charge state")>
    Public Property charge_state As Integer

    <Column("compound class", "compound source")>
    Public Property compound_class As String
    Public Property origin As String

    <Column("collision energy",
            "collision energy voltage",
            "source voltage",
            "ionization energy")>
    Public Property collision_energy_voltage As String

    <Column("collision energy level")>
    Public Property collision_energy_level As String

    Public Property kegg As String
    <Column(Name:="capillary temperature")>
    Public Property capillary_temperature As String
    <Column(Name:="capillary voltage")>
    Public Property capillary_voltage As String

    <Column(Name:="sample introduction")>
    Public Property sample_introduction As String
    <Column("raw data file", "source file")>
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

    <Column(Name:="source temperature")>
    Public Property source_temperature As String
    Public Property chebi As String
    Public Property chembl As String
    Public Property wikipedia As String
    Public Property hmdb As String
    Public Property drugbank As String
    Public Property lipidmaps As String
    Public Property Mesh As String
    Public Property ChemIDplus As String

    Public Property scanning As String
    <Column(Name:="sampling cone")>
    Public Property sampling_cone As String
    <Column(Name:="derivative formula")>
    Public Property derivative_formula As String
    <Column(Name:="derivative mass")>
    Public Property derivative_mass As String
    <Column(Name:="derivatization type")>
    Public Property derivatization_type As String
    <Column(Name:="data format")>
    Public Property data_format As String

    <Column(Name:="activation parameter")>
    Public Property activation_parameter As String
    <Column(Name:="activation time")>
    Public Property activation_time As String
    <Column(Name:="automatic gain control")>
    Public Property automatic_gain_control As String
    <Column(Name:="resolution setting")>
    Public Property resolution_setting As String
    <Column(Name:="spray voltage")>
    Public Property spray_voltage As String
    <Column(Name:="tube lens voltage")>
    Public Property tube_lens_voltage As String
    <Column(Name:="cdl temperature")>
    Public Property cdl_temperature As String
    <Column(Name:="scanning range")>
    Public Property scanning_range As String

    <Column(Name:="solvent acetonitrile")>
    Public Property solvent_acetonitrile As String
    <Column(Name:="ion guide peak voltage")>
    Public Property ion_guide_peak_voltage As String

    <Column(Name:="lens voltage")>
    Public Property lens_voltage As String
    <Column(Name:="needle voltage")>
    Public Property needle_voltage As String
    <Column(Name:="nebulizing gas")>
    Public Property nebulizing_gas As String
    <Column(Name:="orifice voltage")>
    Public Property orifice_voltage As String
    <Column(Name:="orifice temperature")>
    Public Property orifice_temperature As String
    <Column(Name:="ion guide voltage")>
    Public Property ion_guide_voltage As String

    <Column(Name:="mass range m/z")>
    Public Property mass_range_mz As String
    <Column(Name:="orifice temp")>
    Public Property orifice_temp As String
    <Column(Name:="ring voltage")>
    Public Property ring_voltage As String

    Public Property whole As String
    Public Property [date] As String
    Public Property copyright As String
    Public Property comment As String()

    Public Function GetFormula() As String
        Dim formula As String = If(molecular_formula, derivative_formula)

        If (Not formula.StringEmpty) AndAlso formula.IndexOf(","c) > 0 Then
            formula = formula.Split(","c).First
        End If

        Return formula
    End Function

    Public Overrides Function ToString() As String
        Return $"{accession}: {name}"
    End Function
End Class
