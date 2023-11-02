#Region "Microsoft.VisualBasic::bdf07d0e5a7e57428e43caff4fa8a49a, mzkit\src\metadb\MoNA\MSP\MspReader.vb"

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

    '   Total Lines: 35
    '    Code Lines: 30
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.40 KB


    ' Module MspReader
    ' 
    '     Function: GetMetadata, ParseFile, ParseSpectrumData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP

Public Module MspReader

    Public Iterator Function ParseFile(path$, Optional parseMs2 As Boolean = True) As IEnumerable(Of SpectraSection)
        For Each spectrum As MspData In MspData.Load(path, ms2:=parseMs2)
            Dim metadata As MetaData = MspReader.GetMetadata(spectrum)
            Dim ms2 As SpectraInfo = ParseSpectrumData(spectrum, metadata)

            Yield New SpectraSection(metadata) With {
                .SpectraInfo = ms2
            }
        Next
    End Function

    Public Function ParseSpectrumData(spectrum As MspData, metadata As MetaData) As SpectraInfo
        Return New SpectraInfo With {
            .MassPeaks = spectrum.Peaks,
            .collision_energy = spectrum.Collision_energy,
            .retention_time = metadata.Read_retention_time,
            .instrument = spectrum.Instrument,
            .instrument_type = spectrum.Instrument_type,
            .MsLevel = metadata.ms_level,
            .mz = Val(spectrum.PrecursorMZ),
            .precursor_type = spectrum.Precursor_type,
            .ion_mode = spectrum.Ion_mode
        }
    End Function

    ''' <summary>
    ''' Extract the metadata from the MONA comment data
    ''' </summary>
    ''' <param name="spectrum"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GetMetadata(spectrum As MspData) As MetaData
        Dim metadata As MetaData = spectrum.Comments.FillData

        If metadata.accession.StringEmpty Then
            metadata.accession = spectrum.DB_id
        End If

        Return metadata
    End Function
End Module
