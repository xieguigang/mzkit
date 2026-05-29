#Region "Microsoft.VisualBasic::9cfc2fa25188bcb5daecaff892242c2a, assembly\assembly\ASCII\MSP\MspData.vb"

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

    '   Total Lines: 88
    '    Code Lines: 51 (57.95%)
    ' Comment Lines: 27 (30.68%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (11.36%)
    '     File Size: 3.53 KB


    '     Class MspData
    ' 
    '         Properties: Collision_energy, Comments, DB_id, Formula, InChIKey
    '                     Instrument, Instrument_type, Ion_mode, MW, Name
    '                     Peaks, Precursor_type, PrecursorMZ, RetentionTime, Spectrum_type
    '                     Synonyms
    ' 
    '         Function: Load, ParseCommentMetaTable, ToScan2, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ASCII.MSP

    ''' <summary>
    ''' a spectrum file format data
    ''' </summary>
    ''' <remarks>
    ''' a collection of the spectrum peaks <see cref="ms2"/> data.
    ''' </remarks>
    Public Class MspData : Implements INamedValue, IFormulaProvider

        ''' <summary>
        ''' the metabolite name
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        Public Property Synonyms As String()

        ''' <summary>
        ''' ``DB#``
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="DB#")>
        Public Property DB_id As String Implements INamedValue.Key
        Public Property InChIKey As String
        Public Property MW As Double
        Public Property Formula As String Implements IFormulaProvider.Formula
        Public Property PrecursorMZ As String
        Public Property Precursor_type As String
        Public Property Spectrum_type As String
        Public Property Instrument_type As String
        Public Property Instrument As String
        Public Property Ion_mode As String
        Public Property Collision_energy As String
        Public Property RetentionTime As String

        ''' <summary>
        ''' the spectrum data
        ''' </summary>
        ''' <returns></returns>
        Public Property Peaks As ms2()

        ''' <summary>
        ''' 如果这个文件是来自于MoNA数据库的话, 则在这里是物质的注释信息
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' data in this property consist with the metadata 
        ''' in the msp fields and the metadata from the 
        ''' comment text data field.
        ''' </remarks>
        Public Property Comments As NameValueCollection

        Public Overrides Function ToString() As String
            Return Name
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load(path$, Optional ms2 As Boolean = True) As IEnumerable(Of MspData)
            Return MspParser.Load(path, ms2)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseCommentMetaTable(comment As String) As NameValueCollection
            Return comment.ToTable
        End Function

        Public Shared Function ToScan2(msp As MspData) As ScanMS2
            Return New ScanMS2 With {
                .centroided = True,
                .collisionEnergy = Val(msp.Collision_energy),
                .into = msp.Peaks.Select(Function(a) a.intensity).ToArray,
                .intensity = .into.Sum,
                .mz = msp.Peaks.Select(Function(a) a.mz).ToArray,
                .parentMz = Val(msp.PrecursorMZ),
                .rt = Val(msp.RetentionTime),
                .scan_id = $"[MS2] {msp.Name} basepeak_m/z={msp.Peaks.OrderByDescending(Function(a) a.intensity).First.mz.ToString("F4")}, total_ions={ .intensity}"
            }
        End Function
    End Class
End Namespace
