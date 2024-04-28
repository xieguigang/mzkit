#Region "Microsoft.VisualBasic::a2de10998aee0ddf8fee33ec63f1f55f, E:/mzkit/src/assembly/assembly//MarkupData/imzML/XML/imzMLMetadata.vb"

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

    '   Total Lines: 137
    '    Code Lines: 102
    ' Comment Lines: 16
    '   Blank Lines: 19
    '     File Size: 6.34 KB


    '     Class imzMLMetadata
    ' 
    '         Properties: cv, dims, direction1, direction2, format
    '                     guid, ibd_checksum, ms_analyzer, ms_detector, ms_instrument
    '                     ms_source, physical_size, resolution, softwares, sourcefiles
    ' 
    '         Function: ReadHeaders
    ' 
    '         Sub: GetFileContents, GetInstrument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData.imzML

    ''' <summary>
    ''' the MS-imaging metadata that read from the imzML header data
    ''' </summary>
    Public Class imzMLMetadata

        ''' <summary>
        ''' a set of the control vocabulary that used in current dataset
        ''' </summary>
        ''' <returns></returns>
        Public Property cv As cvList
        Public Property guid As String
        Public Property format As Format
        Public Property ibd_checksum As String
        Public Property sourcefiles As String()
        ''' <summary>
        ''' software name => version
        ''' </summary>
        ''' <returns></returns>
        Public Property softwares As NamedValue(Of String)()

#Region "ms-imaging specific metadata"

        Public Property direction1 As String
        Public Property direction2 As String
        Public Property dims As Size
        Public Property physical_size As Size
        Public Property resolution As Size

#End Region

#Region "ms instrument"
        Public Property ms_instrument As String
        Public Property ms_source As String
        Public Property ms_analyzer As String
        Public Property ms_detector As String
#End Region

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="imzml">the file path to the target imzml file</param>
        ''' <returns></returns>
        Public Shared Function ReadHeaders(imzml As String) As imzMLMetadata
            Dim cv As cvList = LoadUltraLargeXMLDataSet(Of cvList)(imzml, typeName:=NameOf(cvList)).FirstOrDefault
            Dim desc As fileDescription = LoadUltraLargeXMLDataSet(Of fileDescription)(imzml, typeName:=NameOf(fileDescription)).FirstOrDefault
            Dim softwares As softwareList = LoadUltraLargeXMLDataSet(Of softwareList)(imzml, typeName:=NameOf(mzML.softwareList)).FirstOrDefault
            Dim scanMeta As scanSettingsList = LoadUltraLargeXMLDataSet(Of scanSettingsList)(imzml, typeName:=NameOf(scanSettingsList)).FirstOrDefault
            Dim instrument As instrumentConfigurationList = LoadUltraLargeXMLDataSet(Of instrumentConfigurationList)(imzml, typeName:=NameOf(instrumentConfigurationList)).FirstOrDefault

            Dim guid As String = Nothing
            Dim format As Format = Nothing
            Dim checksum As String = Nothing

            Call GetFileContents(desc?.fileContent, guid, format, checksum)

            Dim softwareList As NamedValue(Of String)() = softwares.ToArray
            Dim instrumentName As String = Nothing
            Dim source As String = Nothing
            Dim analyzer As String = Nothing
            Dim detector As String = Nothing

            Call GetInstrument(instrument.instrumentConfiguration.FirstOrDefault, instrumentName, source, analyzer, detector)

            Dim settings = scanMeta.scanSettings.First
            Dim dir1 As String = settings.FindVocabulary("IMS:1000401")?.name
            Dim dir2 As String = settings.FindVocabulary("IMS:1000490")?.name
            Dim dim1 As Integer = settings.FindVocabulary("IMS:1000042")?.value
            Dim dim2 As Integer = settings.FindVocabulary("IMS:1000043")?.value
            Dim size1 As Integer = settings.FindVocabulary("IMS:1000044")?.value
            Dim size2 As Integer = settings.FindVocabulary("IMS:1000045")?.value
            Dim res1 As Double = settings.FindVocabulary("IMS:1000046")?.value
            Dim res2 As Double = settings.FindVocabulary("IMS:1000047")?.value

            Return New imzMLMetadata With {
                .cv = cv,
                .sourcefiles = desc.sourceFileList.GetFileList.ToArray,
                .guid = guid,
                .format = format,
                .ibd_checksum = checksum,
                .softwares = softwareList,
                .ms_instrument = instrumentName,
                .ms_analyzer = analyzer,
                .ms_detector = detector,
                .ms_source = source,
                .dims = New Size(dim1, dim2),
                .direction1 = dir1,
                .direction2 = dir2,
                .physical_size = New Size(size1, size2),
                .resolution = New Size(res1, res2)
            }
        End Function

        Private Shared Sub GetInstrument(ms As instrumentConfiguration,
                                         <Out> ByRef instrument As String,
                                         <Out> ByRef source As String,
                                         <Out> ByRef analyzer As String,
                                         <Out> ByRef detector As String)
            If Not ms Is Nothing Then
                instrument = ms.cvParams.First.name
                source = ms.componentList.source.cvParams.First.name
                analyzer = ms.componentList.analyzer.First.cvParams.First.name
                detector = ms.componentList.detector.First.cvParams.First.name
            End If
        End Sub

        Private Shared Sub GetFileContents(content As Params,
                                           <Out> ByRef guid As String,
                                           <Out> ByRef format As Format,
                                           <Out> ByRef checksum As String)

            If Not content Is Nothing Then
                Dim format_str As String = content.FindVocabulary("IMS:1000030")?.value

                guid = content.FindVocabulary("IMS:1000080")?.value
                checksum = content.FindVocabulary("IMS:1000090")?.value

                If Not format_str.StringEmpty Then
                    If format_str.ToLower = "continuous" Then
                        format = Format.Continuous
                    Else
                        format = Format.Processed
                    End If
                End If
            End If
        End Sub
    End Class
End Namespace
