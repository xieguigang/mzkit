#Region "Microsoft.VisualBasic::dbd9ed206bee3df53b0b6a04e956c262, assembly\assembly\MarkupData\imzML\Writer\mzPackWriter.vb"

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

    '   Total Lines: 340
    '    Code Lines: 286 (84.12%)
    ' Comment Lines: 21 (6.18%)
    '    - Xml Docs: 42.86%
    ' 
    '   Blank Lines: 33 (9.71%)
    '     File Size: 19.64 KB


    '     Class mzPackWriter
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: MeasureIbdSha1, OpenOutput, SetMSImagingParameters, SetSourceLocation, SetSpectrumParameters
    ' 
    '         Sub: (+2 Overloads) Dispose, flushXML, WriteDataProcessing, WriteDataScans, WriteDescriptions
    '              WriteInstruments, WriteMSImgingParameters, WriteParameters, WriteSampleInformation, WriteScan
    '              WriteSoftwareInformation, WriteXMLHeader
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.SecurityString

Namespace MarkupData.imzML

    ''' <summary>
    ''' convert
    ''' </summary>
    Public Class mzPackWriter : Implements IDisposable

        ReadOnly imzMLfile As FileStream
        ReadOnly ibdfile As FileStream
        ReadOnly guid As Guid = Guid.NewGuid
        ReadOnly imzML As StreamWriter
        ReadOnly ibd As BinaryDataWriter
        ReadOnly scans As New List(Of ScanData)

        Dim disposedValue As Boolean
        Dim sourceLocation As String
        Dim scan_type As Integer
        Dim dims As Size
        Dim resolution As Double
        Dim ibdfilepath As String

        Private Sub New(imzML As String)
            Dim text As Encoding = Encoding.GetEncoding("ISO-8859-1")

            Me.imzMLfile = imzML.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Me.ibdfilepath = imzML.ChangeSuffix("ibd")
            Me.ibdfile = ibdfilepath.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Me.ibdfile.Write(guid.ToByteArray, Scan0, guid.ToByteArray.Length)
            Me.ibdfile.Flush()
            Me.ibd = New BinaryDataWriter(ibdfile) With {
                .ByteOrder = ByteOrder.LittleEndian,
                .Encoding = Encoding.ASCII
            }
            Me.imzML = New StreamWriter(imzMLfile, text) With {.NewLine = vbLf}
        End Sub

        Public Function SetSourceLocation(path As String) As mzPackWriter
            sourceLocation = path
            Return Me
        End Function

        Public Function SetSpectrumParameters(ionMode As IonModes) As mzPackWriter
            scan_type = CInt(ionMode)
            Return Me
        End Function

        Public Function SetMSImagingParameters(dims As Size, resolution As Double) As mzPackWriter
            Me.dims = dims
            Me.resolution = resolution
            Return Me
        End Function

        Private Function MeasureIbdSha1() As String
            Dim hash As New SHA1CryptoServiceProvider()
            Dim checksum As String

            Call ibd.Flush()
            Call ibd.Flush()
            Call ibd.Dispose()

            checksum = MD5Hash.GetFileMd5(ibdfilepath)
            checksum = checksum.Replace("-", "")

            Return checksum
        End Function

        Private Sub flushXML()
            Call WriteXMLHeader()
            Call WriteDescriptions()
            Call WriteParameters()
            Call WriteSampleInformation()
            Call WriteSoftwareInformation()
            Call WriteMSImgingParameters()
            Call WriteInstruments()
            Call WriteDataProcessing()
            Call WriteDataScans()
        End Sub

        Private Sub WriteXMLHeader()
            Call imzML.WriteLine("<?xml version=""1.0"" encoding=""ISO-8859-1""?>")
            Call imzML.WriteLine("<mzML xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://psi.hupo.org/ms/mzml http://psidev.info/files/ms/mzML/xsd/mzML1.1.0.xsd"" xmlns=""http://psi.hupo.org/ms/mzml"" version=""1.1"">")
            Call imzML.WriteXml(
                <cvList count="4">
                    <cv URI="http://ontologies.berkeleybop.org/pato.obo" fullName="Phenotype And Trait Ontology" id="PATO" version="releases/2017-07-10"/>
                    <cv URI="http://ontologies.berkeleybop.org/uo.obo" fullName="Units of Measurement Ontology" id="UO" version="releases/2017-09-25"/>
                    <cv URI="https://raw.githubusercontent.com/hupo-psi/psi-ms-cv/master/psi-ms.obo" fullName="Proteomics Standards Initiative Mass Spectrometry Ontology" id="MS" version="4.1.0"/>
                    <cv URI="https://raw.githubusercontent.com/imzML/imzML/master/imagingMS.obo" fullName="Mass Spectrometry Imaging Ontology" id="IMS" version="1.1.0"/>
                </cvList>
            )
        End Sub

        Private Sub WriteDescriptions()
            Call imzML.WriteXml(
                <fileDescription>
                    <fileContent>
                        <cvParam cvRef="MS" accession="MS:1000579" name="MS1 spectrum"/>
                        <cvParam cvRef="MS" accession="MS:1000128" name="profile spectrum"/>
                        <cvParam cvRef="IMS" accession="IMS:1000080" name="universally unique identifier" value=<%= guid.ToString.Replace("-", "") %>/>
                        <cvParam cvRef="IMS" accession="IMS:1000091" name="ibd SHA-1" value=<%= MeasureIbdSha1() %>/>
                        <cvParam cvRef="IMS" accession="IMS:1000031" name="processed"/>
                    </fileContent>
                    <sourceFileList count="1">
                        <sourceFile id="sf1" location=<%= sourceLocation.ParentPath %> name=<%= sourceLocation.FileName %>>
                            <cvParam cvRef="MS" accession="MS:1000563" name="Thermo RAW format"/>
                            <cvParam cvRef="MS" accession="MS:1000768" name="Thermo nativeID format"/>
                            <cvParam cvRef="MS" accession="MS:1000569" name="SHA-1" value="16899F53AF4AEF90F2DF01E6678C728517F7C3EB"/>
                        </sourceFile>
                    </sourceFileList>
                    <contact>
                        <cvParam cvRef="MS" accession="MS:1000586" name="contact name" value="Xieguigang"/>
                        <cvParam cvRef="MS" accession="MS:1000590" name="contact affiliation" value="BioNovoGene Corporation"/>
                        <cvParam cvRef="MS" accession="MS:1000587" name="contact address" value="Building 2, 388 Xinping Street, Suzhou Industrial Park, Jiangsu Province, China"/>
                        <cvParam cvRef="MS" accession="MS:1000589" name="contact email" value="gg.xie@bionovogene.com"/>
                    </contact>
                </fileDescription>)
        End Sub

        Private Sub WriteParameters()
            Call imzML.WriteXml(
                <referenceableParamGroupList count="4">
                    <referenceableParamGroup id="mzArray">
                        <cvParam cvRef="MS" accession="MS:1000576" name="no compression"/>
                        <cvParam cvRef="MS" accession="MS:1000514" name="m/z array" unitCvRef="MS" unitAccession="MS:1000040" unitName="m/z"/>
                        <cvParam cvRef="IMS" accession="IMS:1000101" name="external data" value="true"/>
                        <cvParam cvRef="MS" accession="MS:1000521" name="64-bit float"/>
                    </referenceableParamGroup>
                    <referenceableParamGroup id="intensityArray">
                        <cvParam cvRef="MS" accession="MS:1000576" name="no compression"/>
                        <cvParam cvRef="MS" accession="MS:1000515" name="intensity array" unitCvRef="MS" unitAccession="MS:1000131" unitName="number of detector counts"/>
                        <cvParam cvRef="IMS" accession="IMS:1000101" name="external data" value="true"/>
                        <cvParam cvRef="MS" accession="MS:1000521" name="64-bit float"/>
                    </referenceableParamGroup>
                    <referenceableParamGroup id="scan1">
                        <cvParam cvRef="MS" accession="MS:1000093" name="increasing m/z scan"/>
                        <cvParam cvRef="MS" accession="MS:1000512" name="filter string" value="ITMS + p NSI Full ms [100,00-1000,00]"/>
                    </referenceableParamGroup>
                    <referenceableParamGroup id="spectrum1">
                        <cvParam cvRef="MS" accession="MS:1000579" name="MS1 spectrum"/>
                        <cvParam cvRef="MS" accession="MS:1000511" name="ms level" value="0"/>
                        <cvParam cvRef="MS" accession="MS:1000128" name="profile spectrum"/>
                        <cvParam cvRef="MS" accession="MS:1000130" name=<%= If(scan_type = 1, "positive scan", "negative scan") %>/>
                    </referenceableParamGroup>
                </referenceableParamGroupList>)
        End Sub

        Private Sub WriteSampleInformation()
            Call imzML.WriteXml(
                <sampleList count="1">
                    <sample id="sample1" name="Sample1">
                        <cvParam cvRef="MS" accession="MS:1000001" name="sample number" value="1"/>
                    </sample>
                </sampleList>)
        End Sub

        Private Sub WriteSoftwareInformation()
            Call imzML.WriteXml(
                <softwareList count="2">
                    <software id="Xcalibur" version="2.2">
                        <cvParam cvRef="MS" accession="MS:1000532" name="Xcalibur"/>
                    </software>
                    <software id="BioNovoGeneMZKit" version="3.58.2569">
                        <cvParam cvRef="MS" accession="MS:1000799" name="custom unreleased software tool" value=""/>
                    </software>
                </softwareList>)
        End Sub

        Private Sub WriteMSImgingParameters()
            Call imzML.WriteXml(
                <scanSettingsList count="1">
                    <scanSettings id="scansettings1">
                        <cvParam cvRef="IMS" accession="IMS:1000401" name="top down"/>
                        <cvParam cvRef="IMS" accession="IMS:1000413" name="flyback"/>
                        <cvParam cvRef="IMS" accession="IMS:1000480" name="horizontal line scan"/>
                        <cvParam cvRef="IMS" accession="IMS:1000491" name="linescan left right"/>
                        <cvParam cvRef="IMS" accession="IMS:1000042" name="max count of pixels x" value=<%= dims.Width %>/>
                        <cvParam cvRef="IMS" accession="IMS:1000043" name="max count of pixels y" value=<%= dims.Height %>/>
                        <cvParam cvRef="IMS" accession="IMS:1000044" name="max dimension x" value=<%= dims.Width * resolution %> unitCvRef="UO" unitAccession="UO:0000017" unitName="micrometer"/>
                        <cvParam cvRef="IMS" accession="IMS:1000045" name="max dimension y" value=<%= dims.Height * resolution %> unitCvRef="UO" unitAccession="UO:0000017" unitName="micrometer"/>
                        <cvParam cvRef="IMS" accession="IMS:1000046" name="pixel size (x)" value=<%= resolution %> unitCvRef="UO" unitAccession="UO:0000017" unitName="micrometer"/>
                        <cvParam cvRef="IMS" accession="IMS:1000047" name="pixel size y" value=<%= resolution %> unitCvRef="UO" unitAccession="UO:0000017" unitName="micrometer"/>
                    </scanSettings>
                </scanSettingsList>
            )
        End Sub

        Private Sub WriteInstruments()
            Call imzML.WriteXml(
                <instrumentConfigurationList count="1">
                    <instrumentConfiguration id="LTQFTUltra0">
                        <cvParam cvRef="MS" accession="MS:1000557" name="LTQ FT Ultra"/>
                        <cvParam cvRef="MS" accession="MS:1000529" name="instrument serial number" value="none"/>
                        <componentList count="3">
                            <source order="1">
                                <cvParam cvRef="MS" accession="MS:1000073" name="electrospray ionization"/>
                                <cvParam cvRef="MS" accession="MS:1000485" name="nanospray inlet"/>
                                <cvParam cvRef="MS" accession="MS:1000844" name="focus diameter x" value="10.0"/>
                                <cvParam cvRef="MS" accession="MS:1000845" name="focus diameter y" value="10.0"/>
                                <cvParam cvRef="MS" accession="MS:1000846" name="pulse energy" value="10.0"/>
                                <cvParam cvRef="MS" accession="MS:1000847" name="pulse duration" value="10.0"/>
                                <cvParam cvRef="MS" accession="MS:1000848" name="attenuation" value="50.0"/>
                                <cvParam cvRef="MS" accession="MS:1000850" name="gas laser"/>
                                <cvParam cvRef="MS" accession="MS:1000836" name="dried droplet MALDI matrix preparation"/>
                                <cvParam cvRef="MS" accession="MS:1000835" name="matrix solution concentration" value="10.0"/>
                                <cvParam cvRef="MS" accession="MS:1000834" name="matrix solution" value="DHB"/>
                            </source>
                            <analyzer order="2">
                                <cvParam cvRef="MS" accession="MS:1000264" name="ion trap"/>
                                <cvParam cvRef="MS" accession="MS:1000014" name="accuracy" value="0.0" unitCvRef="MS" unitAccession="MS:1000040" unitName="m/z"/>
                            </analyzer>
                            <detector order="3">
                                <cvParam cvRef="MS" accession="MS:1000253" name="electron multiplier"/>
                                <cvParam cvRef="MS" accession="MS:1000120" name="transient recorder"/>
                            </detector>
                        </componentList>
                        <softwareRef ref="Xcalibur"/>
                    </instrumentConfiguration>
                </instrumentConfigurationList>)
        End Sub

        Private Sub WriteDataProcessing()
            Call imzML.WriteXml(
                <dataProcessingList count="2">
                    <dataProcessing id="XcaliburProcessing">
                        <processingMethod softwareRef="Xcalibur" order="1">
                            <cvParam cvRef="MS" accession="MS:1000594" name="low intensity data point removal"/>
                        </processingMethod>
                    </dataProcessing>
                    <dataProcessing id="MzPackConversion">
                        <processingMethod softwareRef="BioNovoGeneMZKit" order="2">
                            <cvParam cvRef="MS" accession="MS:1000544" name="Conversion to mzML"/>
                        </processingMethod>
                    </dataProcessing>
                </dataProcessingList>
            )
        End Sub

        Private Sub WriteDataScans()
            Dim i As i32 = 1

            Call imzML.WriteLine($"<run defaultInstrumentConfigurationRef=""LTQFTUltra0"" defaultSourceFileRef=""sf1"" id=""Experiment01"" sampleRef=""sample1"" startTimeStamp=""2009-08-19T19:26:13"">")
            Call imzML.WriteLine($"<spectrumList count=""{scans.Count}"" defaultDataProcessingRef=""XcaliburProcessing"">")

            For Each scan In scans
                Call imzML.WriteXml(
                    <spectrum id=<%= $"Scan={++i}" %> defaultArrayLength="0" dataProcessingRef="XcaliburProcessing" sourceFileRef="sf1" index=<%= CInt(i) - 1 %>>
                        <referenceableParamGroupRef ref="spectrum1"/>
                        <cvParam cvRef="MS" accession="MS:1000285" name="total ion current" value=<%= scan.totalIon %>/>
                        <scanList count="1">
                            <cvParam cvRef="MS" accession="MS:1000795" name="no combination"/>
                            <scan instrumentConfigurationRef="LTQFTUltra0">
                                <referenceableParamGroupRef ref="scan1"/>
                                <cvParam cvRef="IMS" accession="IMS:1000050" name="position x" value=<%= scan.x %>/>
                                <cvParam cvRef="IMS" accession="IMS:1000051" name="position y" value=<%= scan.y %>/>
                            </scan>
                        </scanList>
                        <binaryDataArrayList count="2">
                            <binaryDataArray encodedLength="0">
                                <referenceableParamGroupRef ref="mzArray"/>
                                <cvParam cvRef="IMS" accession="IMS:1000103" name="external array length" value=<%= scan.MzPtr.arrayLength %>/>
                                <cvParam cvRef="IMS" accession="IMS:1000102" name="external offset" value=<%= scan.MzPtr.offset %>/>
                                <cvParam cvRef="IMS" accession="IMS:1000104" name="external encoded length" value=<%= scan.MzPtr.encodedLength %>/>
                                <binary/>
                            </binaryDataArray>
                            <binaryDataArray encodedLength="0">
                                <referenceableParamGroupRef ref="intensityArray"/>
                                <cvParam cvRef="IMS" accession="IMS:1000103" name="external array length" value=<%= scan.IntPtr.arrayLength %>/>
                                <cvParam cvRef="IMS" accession="IMS:1000102" name="external offset" value=<%= scan.IntPtr.offset %>/>
                                <cvParam cvRef="IMS" accession="IMS:1000104" name="external encoded length" value=<%= scan.IntPtr.encodedLength %>/>
                                <binary/>
                            </binaryDataArray>
                        </binaryDataArrayList>
                    </spectrum>)
            Next

            Call imzML.WriteLine("</spectrumList>")
            Call imzML.WriteLine("</run>")
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WriteScan(scan As ScanMS1)
            Call scans.Add(DataWriter.WriteMzPack(scan, ibd))
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="output">
        ''' the file path of the target imzML file
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function OpenOutput(output As String) As mzPackWriter
            Return New mzPackWriter(output)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    On Error Resume Next

                    ' TODO: 释放托管状态(托管对象)
                    Call flushXML()
                    Call imzML.WriteLine("</mzML>")
                    Call imzML.Flush()
                    Call imzML.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
