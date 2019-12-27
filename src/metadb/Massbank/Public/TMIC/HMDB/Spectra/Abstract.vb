#Region "Microsoft.VisualBasic::13168afaf814aa0624854d8d15828ea6, DATA\Massbank\Public\TMIC\HMDB\Spectra\Abstract.vb"

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

    '     Structure NullableValue
    ' 
    '         Properties: nil, value
    ' 
    '         Function: ToString
    ' 
    '     Class reference
    ' 
    '         Properties: database, database_id, id, pubmed_id, ref_text
    '                     spectra_id, spectra_type
    ' 
    '     Class SpectraFile
    ' 
    '         Properties: base_peak, chromatography_type, collection_date, column_type, database_id
    '                     id, instrument_type, ionization_mode, notes, peak_counter
    '                     predicted, references, retention_index, retention_time, ri_type
    '                     sample_assessment, sample_concentration, sample_concentration_units, sample_mass, sample_mass_units
    '                     sample_source, solvent, spectra_assessment, splash_key, structure_id
    ' 
    '     Interface IPeakList
    ' 
    '         Properties: peakList
    ' 
    '     Class Peak
    ' 
    '         Properties: id, intensity
    ' 
    '         Function: ToString
    ' 
    '     Class MSPeak
    ' 
    '         Properties: mass_charge
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel

Namespace TMIC.HMDB.Spectra

    Public Structure NullableValue

        <XmlAttribute>
        Public Property nil As Boolean
        <XmlText>
        Public Property value As String

        Public Overrides Function ToString() As String
            If nil Then
                Return "null"
            Else
                Return value
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(value As NullableValue) As String
            Return value.value
        End Operator
    End Structure

    Public Class reference
        Public Property id As String

        <XmlElement("spectra-id")> Public Property spectra_id As String
        <XmlElement("spectra-type")> Public Property spectra_type As String
        <XmlElement("pubmed-id")> Public Property pubmed_id As String
        <XmlElement("ref-text")> Public Property ref_text As String
        <XmlElement("database")> Public Property database As String
        <XmlElement("database-id")> Public Property database_id As String

    End Class

    Public MustInherit Class SpectraFile : Inherits XmlDataModel

        Public Property id As String
        Public Property notes As NullableValue
        Public Property predicted As Boolean

        ''' <summary>
        ''' HMDB ID
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("database-id")> Public Property database_id As NullableValue
        <XmlElement("peak-counter")> Public Property peak_counter As NullableValue

        <XmlElement("sample-concentration")> Public Property sample_concentration As NullableValue
        <XmlElement("sample-concentration-units")> Public Property sample_concentration_units As NullableValue

        <XmlElement("solvent")> Public Property solvent As NullableValue
        <XmlElement("sample-mass")> Public Property sample_mass As NullableValue
        <XmlElement("sample-mass-units")> Public Property sample_mass_units As NullableValue
        <XmlElement("sample-assessment")> Public Property sample_assessment As NullableValue
        <XmlElement("spectra-assessment")> Public Property spectra_assessment As NullableValue
        <XmlElement("sample-source")> Public Property sample_source As NullableValue
        <XmlElement("collection-date")> Public Property collection_date As NullableValue
        <XmlElement("instrument-type")> Public Property instrument_type As NullableValue
        <XmlElement("chromatography-type")> Public Property chromatography_type As NullableValue
        <XmlElement("retention-index")> Public Property retention_index As NullableValue
        <XmlElement("retention-time")> Public Property retention_time As NullableValue
        <XmlElement("ionization-mode")> Public Property ionization_mode As NullableValue
        <XmlElement("column-type")> Public Property column_type As NullableValue
        <XmlElement("base-peak")> Public Property base_peak As NullableValue
        <XmlElement("ri-type")> Public Property ri_type As NullableValue
        <XmlElement("structure-id")> Public Property structure_id As NullableValue
        <XmlElement("splash-key")> Public Property splash_key As NullableValue

        Public Property references As reference()

    End Class

    Public Interface IPeakList(Of T As Peak)
        Property peakList As T()
    End Interface

    Public MustInherit Class Peak

        Public Overridable Property id As String
        Public Overridable Property intensity As Double

        Public Overrides Function ToString() As String
            Return $"[{id} => {intensity}]"
        End Function
    End Class

    Public Class MSPeak : Inherits Peak
        <XmlElement("mass-charge")>
        Public Property mass_charge As Double
    End Class
End Namespace
