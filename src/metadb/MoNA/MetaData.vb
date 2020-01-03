#Region "Microsoft.VisualBasic::335cdea7224d783e0dbd8a7b05cbcc24, src\metadb\MoNA\MetaData.vb"

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

    ' Class MetaData
    ' 
    '     Properties: accession, activation_parameter, activation_time, adduct, author
    '                 automatic_gain_control, capillary_temperature, capillary_voltage, cas, cas_number
    '                 cdl_temperature, charge_state, chebi, chemspider, collision_energy
    '                 collision_energy_voltage, collision_gas, column, column_pressure, column_temperature
    '                 compound_class, compound_source, copyright, data_format, derivative_formula
    '                 derivative_mass, derivatization_type, desolvation_gas_flow, desolvation_temperature, exact_mass
    '                 exactmass, flow_gradient, flow_rate, fragmentation_method, fragmentation_mode
    '                 hmdb, InChI, InChIKey, instrument, instrument_type
    '                 ion_guide_peak_voltage, ion_guide_voltage, ion_source, ion_spray_voltage, ion_type
    '                 ionization, ionization_energy, ionization_mode, kegg, knapsack
    '                 Last_AutoCuration, lens_voltage, license, lineage, link
    '                 lipidbank, lipidmaps, mass_accuracy, mass_error, mass_range_mz
    '                 molecular_formula, ms_level, Mz_exact, name, nebulizer
    '                 nebulizing_gas, needle_voltage, orifice_temp, orifice_temperature, orifice_voltage
    '                 origin, Parent, precursor_mz, precursor_type, pubchem
    '                 pubchem_cid, pubchem_sid, publication, pubmed_id, raw_data_file
    '                 resolution, resolution_setting, retention_time, ring_voltage, sample
    '                 sample_introduction, sampling_cone, scanning, scanning_range, scientific_name
    '                 SMILES, solvent, solvent_a, solvent_acetonitrile, solvent_b
    '                 source_file, source_temperature, source_voltage, spectrum_type, spray_voltage
    '                 total_exact_mass, tube_lens_voltage
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports SMRUCC.MassSpectrum.Assembly.ASCII.MSP

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
    <Column(Name:="instrument type")>
    Public Property instrument_type As String
    <Column(Name:="ms level")>
    Public Property ms_level As String
    <Column(Name:="ionization energy")>
    Public Property ionization_energy As String

    ''' <summary>
    ''' precursor type的另一种别称
    ''' </summary>
    ''' <returns></returns>
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
    <Column(Name:="pubchem sid")>
    Public Property pubchem_sid As String

    <Column(Name:="pubmed id")>
    Public Property pubmed_id As String
    Public Property pubchem As String

    Public Property knapsack As String
    Public Property lipidbank As String

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
    <Column(Name:="capillary voltage")>
    Public Property capillary_voltage As String

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

    <Column(Name:="source temperature")>
    Public Property source_temperature As String
    Public Property chebi As String
    Public Property hmdb As String
    Public Property lipidmaps As String

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

    Public Overrides Function ToString() As String
        Return accession
    End Function
End Class
