#Region "Microsoft.VisualBasic::69199f6588f3be419d903fc0c7165b2f, Library\mzkit\assembly\Assembly.vb"

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

    ' Module Assembly
    ' 
    '     Function: GetFileType, getMs1Scans, ionMode, IonPeaks, mzMLMs1
    '               mzMLScanLoader, mzXML2Mgf, mzXMLMs1, mzXMLScanLoader, PeakMs2FileIndex
    '               rawScans, ReadMgfIons, ReadMslIons, summaryIons, writeMgfIons
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSL
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.ValueTypes
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports mzXMLAssembly = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Rlist = SMRUCC.Rsharp.Runtime.Internal.Object.list

''' <summary>
''' The mass spectrum assembly file read/write library module.
''' </summary>
<Package("assembly", Category:=APICategories.UtilityTools)>
Module Assembly

    <RInitialize>
    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(Ions()), AddressOf summaryIons)
    End Sub

    ''' <summary>
    ''' summary of the mgf ions
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Private Function summaryIons(x As Ions(), args As Rlist, env As Environment) As dataframe
        Dim title As MetaData() = x.Select(Function(a) New MetaData(a.Meta)).ToArray
        Dim rt As Array = x.Select(Function(a) CInt(a.RtInSeconds)).ToArray
        Dim mz As Array = x.Select(Function(a) Val(a.PepMass.name).ToString("F3")).ToArray
        Dim into As Array = x.Select(Function(a) Val(a.PepMass.text).ToString("G2")).ToArray
        Dim charge As Array = x.Select(Function(a) a.Charge).ToArray
        Dim accession As Array = x.Select(Function(a) a.Accession).ToArray
        Dim raw As Array = x.Select(Function(a) a.Rawfile).ToArray
        Dim fragments As Array = x.Select(Function(a) a.Peaks.Length).ToArray
        Dim da3 = Tolerance.DeltaMass(0.3)
        Dim topN As Integer = args.getValue(Of Integer)("top.n", env, 3)
        Dim metaFields As Index(Of String) = args.getValue(Of String())("meta", env, {})
        Dim topNProduct As Array = x _
            .Select(Function(a)
                        Return a.Peaks _
                            .Centroid(da3, LowAbundanceTrimming.Default) _
                            .OrderByDescending(Function(p) p.intensity) _
                            .Take(topN) _
                            .Select(Function(p)
                                        Return p.mz.ToString("F3")
                                    End Function) _
                            .JoinBy(", ")
                    End Function) _
            .ToArray
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {NameOf(mz), mz},
                {NameOf(rt), rt},
                {NameOf(into), into},
                {NameOf(charge), charge},
                {NameOf(raw), raw},
                {"product(m/z)", topNProduct}
            }
        }

        If "accession" Like metaFields Then
            df.columns.Add(NameOf(accession), accession)
        End If
        If "fragments" Like metaFields Then
            df.columns.Add(NameOf(fragments), fragments)
        End If

        df.columns.Add(NameOf(MetaData.activation), title.Select(Function(a) a.activation).ToArray)
        df.columns.Add(NameOf(MetaData.collisionEnergy), title.Select(Function(a) a.collisionEnergy).ToArray)
        df.columns.Add(NameOf(MetaData.kegg), title.Select(Function(a) a.kegg).ToArray)
        df.columns.Add(NameOf(MetaData.precursor_type), title.Select(Function(a) a.precursor_type).ToArray)

        If "compound_class" Like metaFields Then
            df.columns.Add(NameOf(MetaData.compound_class), title.Select(Function(a) a.compound_class).ToArray)
        End If
        If "formula" Like metaFields Then
            df.columns.Add(NameOf(MetaData.formula), title.Select(Function(a) a.formula).ToArray)
        End If
        If "mass" Like metaFields Then
            df.columns.Add(NameOf(MetaData.mass), title.Select(Function(a) a.mass).ToArray)
        End If
        If "name" Like metaFields Then
            df.columns.Add(NameOf(MetaData.name), title.Select(Function(a) a.name).ToArray)
        End If
        If "polarity" Like metaFields Then
            df.columns.Add(NameOf(MetaData.polarity), title.Select(Function(a) a.polarity).ToArray)
        End If

        Return df
    End Function

    ''' <summary>
    ''' read MSL data files
    ''' </summary>
    ''' <param name="file$"></param>
    ''' <param name="unit"></param>
    ''' <returns></returns>
    <ExportAPI("read.msl")>
    Public Function ReadMslIons(file$, Optional unit As TimeScales = TimeScales.Second) As MSLIon()
        Return MSL.FileReader.Load(file, unit).ToArray
    End Function

    <ExportAPI("read.mgf")>
    Public Function ReadMgfIons(file As String) As Ions()
        Return MgfReader.StreamParser(path:=file).ToArray
    End Function

    ''' <summary>
    ''' this function ensure that the output result of the any input ion objects is peakms2 data type.
    ''' </summary>
    ''' <param name="ions">a vector of mgf <see cref="Ions"/> from the ``read.mgf`` function or other data source.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("mgf.ion_peaks")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function IonPeaks(<RRawVectorArgument> ions As Object, Optional env As Environment = Nothing) As Object
        Dim pipeline As pipeline = pipeline.TryCreatePipeline(Of Ions)(ions, env)

        If pipeline.isError Then
            Return pipeline.getError
        End If

        Return pipeline.populates(Of Ions)(env) _
            .IonPeaks _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("open.xml_seek")>
    Public Function openXmlSeeks(file As String, Optional env As Environment = Nothing) As Object
        If Not file.FileExists Then
            Return Internal.debug.stop({
                $"the given file '{file}' is not found on your file system!",
                $"file: {file}"
            }, env)
        Else
            Return New XmlSeek(file).LoadIndex
        End If
    End Function

    ''' <summary>
    ''' write spectra data in mgf file format.
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="file">the file path of the mgf file to write spectra data.</param>
    ''' <param name="relativeInto">
    ''' write relative intensity value into the mgf file instead of the raw intensity value.
    ''' no recommended...
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("write.mgf")>
    <RApiReturn(GetType(Boolean))>
    Public Function writeMgfIons(<RRawVectorArgument> ions As Object, file$, Optional relativeInto As Boolean = False, Optional env As Environment = Nothing) As Object
        If ions Is Nothing Then
            Return Internal.debug.stop("the required ions data can not be nothing!", env)
        End If

        If ions.GetType() Is GetType(pipeline) Then
            Using mgfWriter As StreamWriter = file.OpenWriter(Encodings.ASCII, append:=False)
                Dim pipeStream As pipeline = DirectCast(ions, pipeline)

                If Not pipeStream.elementType Is Nothing AndAlso pipeStream.elementType Like GetType(Ions) Then
                    For Each ion As Ions In pipeStream.populates(Of Ions)(env)
                        Call ion.WriteAsciiMgf(mgfWriter, relativeInto)
                    Next
                Else
                    For Each ionPeak As PeakMs2 In pipeStream.populates(Of PeakMs2)(env)
                        Call ionPeak _
                            .MgfIon _
                            .WriteAsciiMgf(mgfWriter, relativeInto)
                    Next
                End If
            End Using
        ElseIf ions.GetType Is GetType(LibraryMatrix) Then
            Using mgf As StreamWriter = file.OpenWriter
                Call DirectCast(ions, LibraryMatrix) _
                    .MgfIon _
                    .WriteAsciiMgf(mgf)
            End Using
        ElseIf ions.GetType Is GetType(PeakMs2()) Then
            Using mgfWriter As StreamWriter = file.OpenWriter(Encodings.ASCII, append:=False)
                For Each ionPeak As PeakMs2 In DirectCast(ions, PeakMs2())
                    Call ionPeak _
                        .MgfIon _
                        .WriteAsciiMgf(mgfWriter, relativeInto)
                Next
            End Using
        ElseIf ions.GetType Is GetType(Ions()) Then
            Using mgfWriter As StreamWriter = file.OpenWriter(Encodings.ASCII, append:=False)
                For Each ion As Ions In DirectCast(ions, Ions())
                    Call ion.WriteAsciiMgf(mgfWriter, relativeInto)
                Next
            End Using
        ElseIf TypeOf ions Is Ions Then
            Using mgfWriter As StreamWriter = file.OpenWriter(Encodings.ASCII, append:=False)
                Call DirectCast(ions, Ions).WriteAsciiMgf(mgfWriter, relativeInto)
            End Using
        ElseIf ions.GetType Is GetType(PeakMs2) Then
            Using mgfWriter As StreamWriter = file.OpenWriter(Encodings.ASCII, append:=False)
                Call DirectCast(ions, PeakMs2) _
                    .MgfIon _
                    .WriteAsciiMgf(mgfWriter, relativeInto)
            End Using
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(PeakMs2), ions.GetType, env), env)
        End If

        Return True
    End Function

    ''' <summary>
    ''' is mzxml or mzml?
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Function GetFileType(path As String) As Type
        Using xml As StreamReader = path.OpenReader
            Dim header As String

            For i As Integer = 0 To 1
                header = xml.ReadLine

                If InStr(header, "<indexedmzML ") > 0 Then
                    Return GetType(indexedmzML)
                ElseIf InStr(header, "<mzXML ") > 0 Then
                    Return GetType(mzXMLAssembly.XML)
                End If
            Next

            Return Nothing
        End Using
    End Function

    ''' <summary>
    ''' get file index string of the given ms2 peak data.
    ''' </summary>
    ''' <param name="ms2"></param>
    ''' <returns></returns>
    <ExportAPI("file.index")>
    Public Function PeakMs2FileIndex(ms2 As PeakMs2) As String
        Return $"{ms2.file}#{ms2.scan}"
    End Function

    ''' <summary>
    ''' Convert mzxml file as mgf ions.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("mzxml.mgf")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function mzXML2Mgf(file$,
                              Optional relativeInto As Boolean = False,
                              Optional onlyMs2 As Boolean = True,
                              Optional env As Environment = Nothing) As pipeline

        Dim raw As IEnumerable(Of PeakMs2)
        Dim type As Type = GetFileType(file)

        If type Is Nothing Then
            Return Internal.debug.stop({"the given file is not exists or file format not supported!", "file: " & file}, env)
        ElseIf type Is GetType(indexedmzML) Then
            raw = file.mzMLScanLoader(relativeInto, onlyMs2)
        Else
            raw = file.mzXMLScanLoader(relativeInto, onlyMs2)
        End If

        Return raw _
            .Where(Function(peak) peak.mzInto.Length > 0) _
            .DoCall(Function(scans)
                        Return New pipeline(scans, GetType(PeakMs2))
                    End Function)
    End Function

    ''' <summary>
    ''' get raw scans data from the ``mzXML`` or ``mzMl`` data file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("raw.scans")>
    <RApiReturn(GetType(spectrum), GetType(mzXML.scan))>
    Public Function rawScans(file As String, Optional env As Environment = Nothing) As Object
        Dim type As Type = GetFileType(file)

        If type Is Nothing Then
            Return Internal.debug.stop({"the given file is not exists or file format not supported!", "file: " & file}, env)
        ElseIf type Is GetType(indexedmzML) Then
            Return indexedmzML.LoadScans(file).DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return mzXMLAssembly.XML.LoadScans(file).DoCall(AddressOf pipeline.CreateFromPopulator)
        End If
    End Function

    ''' <summary>
    ''' get polarity data for each ms2 scans
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("polarity")>
    <RApiReturn(GetType(Integer))>
    Public Function ionMode(scans As pipeline, Optional env As Environment = Nothing) As Object
        Dim polar As New List(Of Integer)

        If scans.elementType Like GetType(mzXML.scan) Then
            Dim reader As mzXMLScan = MsDataReader(Of mzXML.scan).ScanProvider()

            For Each scanVal As mzXML.scan In scans.populates(Of mzXML.scan)(env).Where(Function(s) reader.GetMsLevel(s) = 2)
                Call polar.Add(PrecursorType.ParseIonMode(reader.GetPolarity(scanVal)))
            Next
        ElseIf scans.elementType Like GetType(spectrum) Then
            Dim reader As mzMLScan = MsDataReader(Of spectrum).ScanProvider()

            For Each scanVal As spectrum In scans.populates(Of spectrum)(env).Where(Function(s) reader.GetMsLevel(s) = 2)
                Call polar.Add(PrecursorType.ParseIonMode(reader.GetPolarity(scanVal)))
            Next
        Else
            Return Message.InCompatibleType(GetType(mzXML.scan), scans.elementType, env)
        End If

        Return polar.ToArray
    End Function

    <Extension>
    Private Iterator Function mzMLScanLoader(path As String, relativeInto As Boolean, onlyMs2 As Boolean) As IEnumerable(Of PeakMs2)
        Dim basename$ = path.BaseName

        For Each msscan As spectrum In indexedmzML _
            .LoadScans(path) _
            .Where(Function(s)
                       If Not onlyMs2 Then
                           Return True
                       Else
                           Return s.ms_level = "2"
                       End If
                   End Function)

            Dim msLevel As Integer = msscan.ms_level.DoCall(AddressOf ParseInteger)

            Select Case msLevel
                Case 1
                    Yield msscan.ScanData(basename, centroid:=False, raw:=True)
                Case 0
                    ' skip UV data?
                    ' Yield msscan.ScanData(basename, centroid:=False, raw:=True)
                Case Else
                    ' msn
                    Yield msscan.ScanData(basename, centroid:=False, raw:=True)
            End Select
        Next
    End Function

    <Extension>
    Private Iterator Function mzXMLScanLoader(mzXML$, relativeInto As Boolean, onlyMs2 As Boolean) As IEnumerable(Of PeakMs2)
        Dim basename$ = mzXML.FileName

        For Each ms2Scan As mzXML.scan In mzXMLAssembly.XML _
            .LoadScans(mzXML) _
            .Where(Function(s)
                       If Not onlyMs2 Then
                           Return True
                       Else
                           Return s.msLevel = 2
                       End If
                   End Function)

            If ms2Scan.msLevel = 1 Then
                ' ms1的数据总是使用raw intensity值
                Yield ms2Scan.ScanData(basename, raw:=True)
            Else
                Yield ms2Scan.ScanData(basename, raw:=Not relativeInto)
            End If
        Next
    End Function

    ''' <summary>
    ''' get all ms1 raw scans from the raw files
    ''' </summary>
    ''' <param name="raw">
    ''' the file path of the raw data files.
    ''' </param>
    ''' <param name="centroid">
    ''' the tolerance value of m/z for convert to centroid mode
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("ms1.scans")>
    <RApiReturn(GetType(ms1_scan))>
    Public Function getMs1Scans(<RRawVectorArgument>
                                raw As Object,
                                Optional centroid As Object = Nothing,
                                Optional env As Environment = Nothing) As Object

        Dim files As String() = REnv.asVector(Of String)(raw)
        Dim ms1 As New List(Of ms1_scan)
        Dim tolerance As Tolerance = Nothing

        If Not centroid Is Nothing Then
            With getTolerance(centroid, env)
                If .GetUnderlyingType Is GetType(Message) Then
                    Return .TryCast(Of Message)
                Else
                    tolerance = .TryCast(Of Tolerance)
                End If
            End With
        End If

        For Each file As String In files
            Select Case file.ExtensionSuffix.ToLower
                Case "mzxml"
                    ms1 += mzXMLMs1(file, tolerance).IteratesALL
                Case "mzml"
                    ms1 += mzMLMs1(file, tolerance).IteratesALL
                Case Else
                    Throw New NotImplementedException
            End Select
        Next

        Return ms1.ToArray
    End Function

    Private Iterator Function mzXMLMs1(file As String, centroid As Tolerance) As IEnumerable(Of ms1_scan())
        Dim reader As New mzXMLScan
        Dim peakScans As ms2()
        Dim rt_sec As Double

        For Each scan As mzXML.scan In mzXML.XML _
            .LoadScans(file) _
            .Where(Function(s)
                       Return s.msLevel = 1
                   End Function)

            ' ms1的数据总是使用raw intensity值
            peakScans = reader.GetMsMs(scan)
            rt_sec = reader.GetScanTime(scan)

            If Not centroid Is Nothing Then
                peakScans = peakScans.Centroid(centroid, LowAbundanceTrimming.intoCutff).ToArray
            End If

            Yield peakScans _
                .Select(Function(frag)
                            Return New ms1_scan With {
                                .intensity = frag.intensity,
                                .mz = frag.mz,
                                .scan_time = rt_sec
                            }
                        End Function) _
                .ToArray
        Next
    End Function

    Private Iterator Function mzMLMs1(file As String, centroid As Tolerance) As IEnumerable(Of ms1_scan())
        Dim reader As New mzMLScan
        Dim peakScans As ms2()
        Dim rt_sec As Double

        For Each scan As spectrum In indexedmzML _
            .LoadScans(file) _
            .Where(Function(s)
                       Return reader.GetMsLevel(s) = 1
                   End Function)

            peakScans = reader.GetMsMs(scan)
            rt_sec = reader.GetScanTime(scan)

            If Not centroid Is Nothing Then
                peakScans = peakScans.Centroid(centroid, LowAbundanceTrimming.intoCutff).ToArray
            End If

            Yield peakScans _
                .Select(Function(frag)
                            Return New ms1_scan With {
                                .intensity = frag.intensity,
                                .mz = frag.mz,
                                .scan_time = rt_sec
                            }
                        End Function) _
                .ToArray
        Next
    End Function
End Module
