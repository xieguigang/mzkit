#Region "Microsoft.VisualBasic::f181bb878796360f3683471f36294a07, mzkit\Rscript\Library\mzkit\comprehensive\MSI.vb"

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

'   Total Lines: 661
'    Code Lines: 468
' Comment Lines: 116
'   Blank Lines: 77
'     File Size: 25.23 KB


' Module MSI
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: asMSILayer, basePeakMz, Correction, getimzmlMetadata, GetIonsJointMatrix
'               GetMatrixIons, GetMSIMetadata, getmzpackFileMetadata, getmzPackMetadata, getStatTable
'               IonStats, loadRowSummary, MSI_summary, MSIScanMatrix, open_imzML
'               PeakMatrix, peakSamples, pixelId, PixelIons, PixelMatrix
'               pixels, pixels2D, rowScans, splice, write_imzML
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Landscape.Vendor_3mf.XML
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.ComponentModel.Activations
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Internal.Object.Converts
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting
Imports imzML = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.XML
Imports rDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports SingleCellMath = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.Math
Imports SingleCellMatrix = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.PeakMatrix
Imports std = System.Math
Imports vector = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' MS-Imaging data handler
''' 
''' Mass spectrometry imaging (MSI) is a technique used in mass spectrometry
''' to visualize the spatial distribution of molecules, as biomarkers, 
''' metabolites, peptides or proteins by their molecular masses. 
''' </summary>
<Package("MSI")>
<RTypeExport("msi_layer", GetType(SingleIonLayer))>
<RTypeExport("msi_summary", GetType(MSISummary))>
Module MSI

    Friend Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(IonStat()), AddressOf getStatTable)

        Call generic.add("readBin.msi_layer", GetType(Stream), AddressOf readPeaklayer)
        Call generic.add("readBin.msi_summary", GetType(Stream), AddressOf readSummarylayer)
        Call generic.add("writeBin", GetType(MSISummary), AddressOf writeSummarylayer)
        Call generic.add("writeBin", GetType(SingleIonLayer), AddressOf writePeaklayer)
    End Sub

    Private Function writeSummarylayer(layer As MSISummary, args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call LayerFile.SaveMSISummary(layer, con)
        Call con.Flush()
        Return True
    End Function

    Private Function readSummarylayer(file As Stream, args As list, env As Environment) As Object
        Return LayerFile.LoadSummaryLayer(file)
    End Function

    Private Function writePeaklayer(layer As SingleIonLayer, args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call LayerFile.SaveLayer(layer, con)
        Call con.Flush()
        Return True
    End Function

    Private Function readPeaklayer(file As Stream, args As list, env As Environment) As Object
        Return LayerFile.ParseLayer(file)
    End Function

    Private Function getStatTable(ions As IonStat(), args As list, env As Environment) As rDataframe
        Dim table As New rDataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = ions _
                .Select(Function(i) i.mz.ToString("F4")) _
                .ToArray
        }

        Call table.add(NameOf(IonStat.mz), ions.Select(Function(i) i.mz))
        Call table.add(NameOf(IonStat.mzmin), ions.Select(Function(i) i.mzmin))
        Call table.add(NameOf(IonStat.mzmax), ions.Select(Function(i) i.mzmax))
        Call table.add(NameOf(IonStat.mzwidth), ions.Select(Function(i) i.mzwidth))
        Call table.add(NameOf(IonStat.pixels), ions.Select(Function(i) i.pixels))
        Call table.add(NameOf(IonStat.density), ions.Select(Function(i) i.density))
        Call table.add("basePixel.X", ions.Select(Function(i) i.basePixelX))
        Call table.add("basePixel.Y", ions.Select(Function(i) i.basePixelY))
        Call table.add(NameOf(IonStat.maxIntensity), ions.Select(Function(i) i.maxIntensity))
        Call table.add(NameOf(IonStat.Q1Intensity), ions.Select(Function(i) i.Q1Intensity))
        Call table.add(NameOf(IonStat.Q2Intensity), ions.Select(Function(i) i.Q2Intensity))
        Call table.add(NameOf(IonStat.Q3Intensity), ions.Select(Function(i) i.Q3Intensity))
        Call table.add(NameOf(IonStat.moran), ions.Select(Function(i) i.moran))
        Call table.add(NameOf(IonStat.pvalue), ions.Select(Function(i) i.pvalue))

        Return table
    End Function

    ''' <summary>
    ''' scale the spatial matrix by column
    ''' </summary>
    ''' <param name="m">a dataframe object that contains the spot expression data. 
    ''' should be in format of: spot in column and ion features in rows.</param>
    ''' <param name="factor">the size of this numeric vector should be equals to the 
    ''' ncol of the given dataframe input <paramref name="m"/>.
    ''' </param>
    ''' <param name="bpc">
    ''' scle by bpc or scale by tic?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>A new dataframe data after scaled</returns>
    ''' <example>
    ''' let factor = get_factors();
    ''' let mat = get_matrix();
    ''' 
    ''' if (length(factor) != ncol(mat)) {
    '''     stop("the length of the factor vector should be equals to the column feature size of the given dataframe!");
    ''' } else {
    '''     mat &lt;- MSI::scale(mat, factor);
    ''' }
    ''' </example>
    <ExportAPI("scale")>
    <RApiReturn(GetType(rDataframe))>
    Public Function scale(m As rDataframe, <RRawVectorArgument> factor As Object,
                          Optional bpc As Boolean = False,
                          Optional env As Environment = Nothing) As Object

        Dim f As Double() = CLRVector.asNumeric(factor)
        Dim v As Double()
        Dim cols As String() = m.colnames
        Dim name As String

        If f.Length <> cols.Length Then
            Return Internal.debug.stop($"the dimension of the factor vector({f.Length}) is not matched with the dataframe columns({cols.Length})!", env)
        End If

        m = New rDataframe(m)

        If bpc Then
            For i As Integer = 0 To cols.Length - 1
                name = cols(i)
                ' scale current column field by a speicifc factor f(i)
                v = CLRVector.asNumeric(m.columns(name))
                ' relative max norm
                v = SIMD.Divide.f64_op_divide_f64_scalar(v, v.Max)
                ' then scale to a max factor
                v = SIMD.Multiply.f64_scalar_op_multiply_f64(f(i), v)
                m.columns(name) = ReLU.ReLU(v)
            Next
        Else
            For i As Integer = 0 To cols.Length - 1
                name = cols(i)
                ' scale current column field by a speicifc factor f(i)
                v = CLRVector.asNumeric(m.columns(name))
                ' total sum norm
                v = SIMD.Divide.f64_op_divide_f64_scalar(v, v.Sum)
                ' then scale to a total factor
                v = SIMD.Multiply.f64_scalar_op_multiply_f64(f(i), v)
                m.columns(name) = ReLU.ReLU(v)
            Next
        End If

        Return m
    End Function

    ''' <summary>
    ''' get ms-imaging metadata
    ''' </summary>
    ''' <param name="raw">should be a mzPack rawdata object which is used for the ms-imaging application.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the input raw data object could be also a file path to the ms-imaging mzpack 
    ''' rawdata, but only version 2 mzPack data file will be supported for load 
    ''' metadata.
    ''' </remarks>
    ''' <example>
    ''' let rawdata = open.mzpack(file = "./rawdata.mzPack");
    ''' let msi_data = msi_metadata(rawdata);
    ''' 
    ''' str(as.list(as.object(msi_data)$GetMetadata()));
    ''' </example>
    <ExportAPI("msi_metadata")>
    <RApiReturn(GetType(Metadata))>
    Public Function GetMSIMetadata(<RRawVectorArgument> raw As Object, Optional env As Environment = Nothing) As Object
        If TypeOf raw Is mzPack Then
            Return DirectCast(raw, mzPack).GetMSIMetadata
        End If

        Dim file = SMRUCC.Rsharp.GetFileStream(raw, FileAccess.Read, env)
        Dim metadata As Metadata

        If file Like GetType(Message) Then
            Return file.TryCast(Of Message)
        End If

        If file.TryCast(Of Stream).GetFormatVersion = 1 Then
            ' version 1 format not supports metadata
            Return Internal.debug.stop(New NotSupportedException("version 1 mzPack file can not supports the metadata!"), env)
        Else
            Dim pack As New mzStream(file.TryCast(Of Stream))

            If pack.metadata.IsNullOrEmpty Then
                metadata = mzPack.FromStream(stream:=pack).GetMSIMetadata
            Else
                metadata = New Metadata(pack.metadata)
            End If
        End If

        If TypeOf raw Is String Then
            Call file.TryCast(Of Stream).Dispose()
        End If

        Return metadata
    End Function

    ''' <summary>
    ''' cast the pixel collection to a ion imaging layer data
    ''' </summary>
    ''' <param name="x">Should be a collection of the ms-imaging pixel data 
    ''' object, or a mz matrix object</param>
    ''' <param name="context">
    ''' the ms-imaging layer title, must be a valid mz numeric value if the input x 
    ''' is a mz matrix object
    ''' </param>
    ''' <param name="dims">the dimension size of the ms-imaging layer data,
    ''' this dimension size will be evaluated based on the input pixel collection
    ''' data if this parameter leaves blank(or NULL) by default.</param>
    ''' <param name="strict">
    ''' if the input ``<paramref name="dims"/>`` produce invalid dimension size
    ''' value, example as dimension size is equals to ZERO [0,0], then in strict 
    ''' mode, the dimension value will be evaluated from the input raw data
    ''' automatically for ensure that the dimension size of the generated layer 
    ''' data object is not empty.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' A ms-imaging layer object that could be used for run ms-imaging rendering.
    ''' </returns>
    ''' <example>
    ''' imports "SingleCells" from "mzkit";
    ''' imports "MsImaging" from "mzplot";
    ''' 
    ''' let msi = SingleCells::read.mz_matrix("/path/to/msi_matrix.mat");
    ''' let layer = MSI::as.layer(x, context = 100.0013);
    ''' 
    ''' bitmap(file = "/path/to/save.png") {
    '''     plot(layer);
    ''' }
    ''' </example>
    <ExportAPI("as.layer")>
    <RApiReturn(GetType(SingleIonLayer))>
    Public Function asMSILayer(<RRawVectorArgument> x As Object,
                               Optional context As Object = "MSIlayer",
                               <RRawVectorArgument>
                               Optional dims As Object = Nothing,
                               Optional strict As Boolean = True,
                               Optional env As Environment = Nothing) As Object

        Dim size As String = InteropArgumentHelper.getSize(dims, env, [default]:="0,0")
        Dim pixels As MsImaging.PixelData()

        If TypeOf x Is MzMatrix Then
            Dim mat As New MsImaging.MatrixReader(DirectCast(x, MzMatrix))
            Dim mz As Double() = CLRVector.asNumeric(context)

            If mz.IsNullOrEmpty OrElse mz.All(Function(mzi) mzi <= 0.0) Then
                Return Internal.debug.stop($"invalid given m/z context value: {context}, it should be a positive real number!", env)
            End If

            pixels = mz _
                .Select(Function(mzi) mat.GetSpots(mzi)) _
                .IteratesALL _
                .ToArray
        Else
            pixels = REnv.asVector(Of MsImaging.PixelData)(x)
        End If

        If size = "0,0" Then
            If strict OrElse pixels.Length > 0 Then
                Dim w = Aggregate px In pixels Into Max(px.x)
                Dim h = Aggregate py In pixels Into Max(py.y)

                size = $"{w},{h}"
            End If
        End If

        Return New SingleIonLayer With {
            .DimensionSize = size.SizeParser,
            .IonMz = context,
            .MSILayer = pixels.ToArray
        }
    End Function

    ''' <summary>
    ''' split the raw 2D MSI data into multiple parts with given parts
    ''' </summary>
    ''' <param name="raw">the mzpack rawdata object that used for run ms-imaging data analysis.</param>
    ''' <param name="partition">this parameter indicates that how many blocks that will be splice 
    ''' into parts on width dimension and column dimension. the number of the pixels in each 
    ''' partition block will be evaluated from this parameter.</param>
    ''' <returns>
    ''' A collection of the ms-imaging mzpack object that split from multiple 
    ''' parts based on the input <paramref name="raw"/> data mzpack inputs.
    ''' </returns>
    ''' <example>
    ''' let rawdata = open.mzpack("/path/to/msi_rawdata.mzPack");
    ''' let parts = MSI::splice(rawdata, partition = 6);
    ''' </example>
    <ExportAPI("splice")>
    <RApiReturn(GetType(mzPack))>
    Public Function splice(raw As mzPack, Optional partition As Integer = 5) As Object
        Dim sampler As New PixelsSampler(New ReadRawPack(raw))
        Dim sampling As Size = sampler.MeasureSamplingSize(resolution:=partition)
        Dim samples As NamedCollection(Of PixelScan)() = sampler.SamplingRaw(sampling).ToArray
        Dim packList As mzPack() = samples _
            .Select(Function(blockList)
                        Return New mzPack With {
                            .MS = blockList _
                                .Select(Function(p)
                                            Return DirectCast(p, mzPackPixel).scan
                                        End Function) _
                                .ToArray,
                            .Application = FileApplicationClass.MSImaging,
                            .source = blockList.name
                        }
                    End Function) _
            .ToArray

        Return packList
    End Function

    ''' <summary>
    ''' get pixels [x,y] tags collection for a specific ion
    ''' </summary>
    ''' <param name="raw">a ms-imaging rawdata object in mzpack format.</param>
    ''' <param name="mz">a m/z numeric value</param>
    ''' <param name="tolerance">the mass tolerance error for match the ion in the rawdata.</param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a character vector of the pixel [x,y] tags.
    ''' </returns>
    ''' <example>
    ''' let rawdata = open.mzpack("/path/to/msi_rawdata.mzPack");
    ''' let xy = MSI::pixelId(rawdata, mz = 100.0023, tolerance = "da:0.01");
    ''' 
    ''' print(xy);
    ''' </example>
    <ExportAPI("pixelId")>
    <RApiReturn(GetType(String))>
    Public Function pixelId(raw As mzPack, mz As Double,
                            Optional tolerance As Object = "da:0.1",
                            Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(tolerance, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim mzdiff As Tolerance = mzErr.TryCast(Of Tolerance)
        Dim scans = From scan As ScanMS1
                    In raw.MS.AsParallel
                    Where scan.mz.Any(Function(mzi) (mzdiff(mzi, mz)))
                    Let p As Point = scan.GetMSIPixel
                    Let id As String = $"{p.X},{p.Y}"
                    Select id

        Return scans.ToArray
    End Function

    ''' <summary>
    ''' get pixels size from the raw data file
    ''' </summary>
    ''' <param name="file">
    ''' imML/mzPack, or a single ion layer of the ms-imaging rawdata
    ''' </param>
    ''' <param name="count">
    ''' get the pixel count number instead of get the canvas dimension size of the ms-imaging.
    ''' </param>
    ''' <returns>
    ''' this function will returns the pixels in dimension size(a tuple list data with slot keys w and h) 
    ''' if the count is set to FALSE, by default; otherwise this function will return an integer value for
    ''' indicates the real pixel counts number if the count parameter is set to TRUE.
    ''' </returns>
    ''' <example>
    ''' let layer = MSI::as.layer(x, context = 100.0013);
    ''' let counts = MSI::pixels(layer, count = TRUE);
    ''' let canvas_size = MSI::pixels(layer, count = FALSE);
    ''' 
    ''' print("the data pixels of current ion layer:");
    ''' print(counts);
    ''' print("the canvas dimension size of the ms-imaging:");
    ''' str(canvas_size);
    ''' print("the pixel number of the ms-imaging canvas:");
    ''' print(canvas_size$w * canvas_size$h);
    ''' </example>
    <ExportAPI("pixels")>
    <RApiReturn("w", "h")>
    Public Function pixels(file As Object,
                           Optional count As Boolean = False,
                           Optional env As Environment = Nothing) As Object

        If TypeOf file Is String AndAlso CStr(file).ExtensionSuffix("imzml") Then
            Return getimzmlMetadata(file, env)
        ElseIf TypeOf file Is String AndAlso CStr(file).ExtensionSuffix("mzpack") Then
            Return getmzpackFileMetadata(file, env)
        ElseIf TypeOf file Is mzPack Then
            Return getmzPackMetadata(file, env)
        ElseIf TypeOf file Is SingleIonLayer Then
            Dim layer As SingleIonLayer = DirectCast(file, SingleIonLayer)

            If count Then
                Return layer.MSILayer.Length
            Else
                Return layer.DimensionSize.size_toList
            End If
        ElseIf TypeOf file Is TissueRegion Then
            Dim region As TissueRegion = file

            If count Then
                Return region.nsize
            Else
                Return region.GetRectangle.Size.size_toList
            End If
        Else
            Return Internal.debug.stop("unsupported file!", env)
        End If
    End Function

    Private Function getimzmlMetadata(file As String, env As Environment) As list
        Dim allScans = XML.LoadScans(CStr(file)).ToArray
        Dim width As Integer = Aggregate p In allScans Into Max(p.x)
        Dim height As Integer = Aggregate p In allScans Into Max(p.y)

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"w", width},
                {"h", height}
            }
        }
    End Function

    Private Function getmzpackFileMetadata(file As String, env As Environment) As Object
        Using buf As Stream = CStr(file).Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim ver As Integer = buf.GetFormatVersion
            Dim reader As IMzPackReader

            If ver = 1 Then
                reader = New BinaryStreamReader(buf)
            ElseIf ver = 2 Then
                reader = New mzStream(buf)
            Else
                Return Internal.debug.stop(New NotImplementedException, env)
            End If

            Dim allMeta = reader.EnumerateIndex _
                .Select(AddressOf reader.GetMetadata) _
                .IteratesALL _
                .ToArray
            Dim x As Integer() = allMeta _
                .Where(Function(p) p.Key.TextEquals("x")) _
                .Select(Function(p) p.Value) _
                .Select(AddressOf Integer.Parse) _
                .ToArray
            Dim y As Integer() = allMeta _
                .Where(Function(p) p.Key.TextEquals("y")) _
                .Select(Function(p) p.Value) _
                .Select(AddressOf Integer.Parse) _
                .ToArray

            Return New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"w", x.Max},
                    {"h", y.Max}
                }
            }
        End Using
    End Function

    Private Function getmzPackMetadata(file As mzPack, env As Environment) As list
        Dim meta = DirectCast(file, mzPack).GetMSIMetadata

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"w", meta.scan_x},
                {"h", meta.scan_y}
            }
        }
    End Function

    ''' <summary>
    ''' get or set the dimension size of the ms-imaging mzpack raw data object
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="dims"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' # get dimension size value
    ''' let size = dimension_size(mzpack_rawdata);
    ''' str(size);
    ''' 
    ''' # set new dimension size to the ms-imaging mzpack object
    ''' dimension_size(mzpack_rawdata) = [525, 600];
    ''' 
    ''' let new_size = dimension_size(mzpack_rawdata);
    ''' str(new_size);
    ''' </example>
    <ExportAPI("dimension_size")>
    Public Function dimension_size(raw As mzPack,
                                   <RByRefValueAssign>
                                   <RRawVectorArgument>
                                   Optional dims As Object = Nothing,
                                   Optional env As Environment = Nothing) As Object

        If dims Is Nothing Then
            ' just get dimension size
            Return getmzPackMetadata(raw, env)
        Else
            Dim sizeVal As String = InteropArgumentHelper.getSize(dims, env, "0,0")

            If sizeVal = "0,0" Then
                Return Internal.debug.stop($"invalid dimension size value input: {any.ToString(dims)}", env)
            End If

            Dim dimsVal As Size = sizeVal.SizeParser

            If raw.metadata Is Nothing Then
                raw.metadata = New Dictionary(Of String, String)
            End If

            raw.metadata("width") = dimsVal.Width
            raw.metadata("height") = dimsVal.Height

            Return raw
        End If
    End Function

    ''' <summary>
    ''' open the reader for the imzML ms-imaging file
    ''' </summary>
    ''' <param name="file">the file path to the specific imzML metadata file for load 
    ''' for run ms-imaging analysis.</param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' this function returns a tuple list object that contains 2 slot elements inside:
    ''' 
    ''' 1. scans: is the [x,y] spatial scans data: <see cref="ScanData"/>.
    ''' 2. ibd: is the binary data reader wrapper object for the corresponding 
    '''       ``ibd`` file of the given input imzML file: <see cref="ibdReader"/>.
    ''' </returns>
    ''' <example>
    ''' # the msi_rawdata.ibd file should be in the same folder with the input imzml file.
    ''' let imzml = open.imzML(file = "/path/to/msi_rawdata.imzML");
    ''' </example>
    <ExportAPI("open.imzML")>
    <RApiReturn("scans", "ibd")>
    Public Function open_imzML(file As String, Optional env As Environment = Nothing) As Object
        Dim scans As ScanData() = imzML.LoadScans(file:=file).ToArray
        Dim ibd As ibdReader
        Dim ibdfile As String = file.ChangeSuffix("ibd")

        If Not ibdfile.FileExists Then
            Return Internal.debug.stop({
                $"The intensity binary data file({ibdfile}) is missing!",
                $"ibd file: {ibdfile}"
            }, env)
        Else
            ibd = ibdReader.Open(ibdfile)
        End If

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"scans", scans},
                {"ibd", ibd}
            }
        }
    End Function

    ''' <summary>
    ''' Save and write the given ms-imaging mzpack object as imzML file
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="file"></param>
    ''' <param name="res">
    ''' the spatial resolution value
    ''' </param>
    ''' <param name="ionMode">
    ''' the ion polarity mode value
    ''' </param>
    ''' <param name="dims">
    ''' an integer vector for set the size of the ms-imaging canvas dimension
    ''' </param>
    ''' <returns></returns>
    ''' <example>
    ''' let msi_rawdata = open.mzpack(file = "/path/to/msi_rawdata.mzPack");
    ''' 
    ''' # convert the mzpack object into imzML format
    ''' msi_rawdata
    ''' |> write.imzML(file = "/path/to/msi_rawdata.imzML", dims = [500, 450]);
    ''' </example>
    <ExportAPI("write.imzML")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function write_imzML(mzpack As mzPack, file As String,
                                Optional res As Double = 17,
                                Optional ionMode As IonModes = IonModes.Positive,
                                <RRawVectorArgument>
                                Optional dims As Object = Nothing,
                                Optional env As Environment = Nothing) As Object

        Dim dimSize As String = InteropArgumentHelper.getSize(dims, env, "0,0")
        Dim dimsVal As Size? = Nothing

        If Not dimSize = "0,0" Then
            dimsVal = dimSize.SizeParser
        End If

        Return imzXMLWriter.WriteXML(
            mzpack, output:=file,
            res:=res,
            ionMode:=ionMode,
            dims:=dimsVal
        )
    End Function

    ''' <summary>
    ''' each raw data file is a row scan data
    ''' </summary>
    ''' <param name="y">
    ''' this function will returns the pixel summary data if the ``y`` parameter greater than ZERO.
    ''' </param>
    ''' <param name="correction">
    ''' used for data summary, when the ``y`` parameter is greater than ZERO, 
    ''' this parameter will works.
    ''' </param>
    ''' <param name="raw">
    ''' a file list of mzpack data files
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("row.scans")>
    <RApiReturn(GetType(iPixelIntensity))>
    Public Function rowScans(raw As String(),
                             Optional y As Integer = 0,
                             Optional correction As Correction = Nothing,
                             Optional env As Environment = Nothing) As Object

        If raw.IsNullOrEmpty Then
            Return Internal.debug.stop("the required raw data file list is empty!", env)
        ElseIf raw.Length = 1 Then
            If y > 0 Then
                Using file As FileStream = raw(Scan0).Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Return file.loadRowSummary(y, correction)
                End Using
            Else
                Return Internal.debug.stop("the pixels of column must be specific!", env)
            End If
        Else
            Dim loader = Iterator Function() As IEnumerable(Of mzPack)
                             For Each path As String In raw
                                 Using file As FileStream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                                     Yield mzPack.ReadAll(file, ignoreThumbnail:=True)
                                 End Using
                             Next
                         End Function

            Return pipeline.CreateFromPopulator(loader())
        End If
    End Function

    <Extension>
    Private Function loadRowSummary(file As Stream, y As Integer, correction As Correction) As iPixelIntensity()
        Dim mzpack As mzPack = mzPack.ReadAll(file, ignoreThumbnail:=True)
        Dim pixels As iPixelIntensity() = mzpack.MS _
            .Select(Function(col, i)
                        Dim basePeakMz As Double = col.mz(which.Max(col.into))

                        Return New iPixelIntensity With {
                            .average = col.into.Average,
                            .basePeakIntensity = col.into.Max,
                            .totalIon = col.into.Sum,
                            .x = If(correction Is Nothing, i + 1, correction.GetPixelRowX(col)),
                            .y = y,
                            .basePeakMz = basePeakMz
                        }
                    End Function) _
            .ToArray

        Return pixels
    End Function

    Private Function GetXySpatialFilter(x As Integer(), y As Integer()) As Func(Of Integer, Integer, Boolean)
        If x.IsNullOrEmpty AndAlso y.IsNullOrEmpty Then
            Return Function(xi, yi) True
        ElseIf x.IsNullOrEmpty Then
            Dim yindex As Index(Of Integer) = y.Indexing
            ' filter y
            Return Function(xi, yi) yi Like yindex
        ElseIf y.IsNullOrEmpty Then
            Dim xindex As Index(Of Integer) = x.Indexing
            ' filter x
            Return Function(xi, yi) xi Like xindex
        Else
            ' filter xy
            Dim pixels As Index(Of String) = x _
                .Select(Function(xi, i) $"{xi},{y(i)}") _
                .Indexing

            Return Function(xi, yi) $"{xi},{yi}" Like pixels
        End If
    End Function

    ''' <summary>
    ''' Fetch MSI summary data
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="as_vector">
    ''' returns the raw vector of <see cref="iPixelIntensity"/> if set this
    ''' parameter value to value TRUE, or its wrapper object <see cref="MSISummary"/> 
    ''' if set this parameter value to FALSE by default.
    ''' </param>
    ''' <param name="dims">
    ''' overrides the MSI data its scan dimension value? This parameter value is
    ''' a numeric vector with two integer element that represents the dimension
    ''' of the MSI data(width and height)
    ''' </param>
    ''' <returns></returns>
    ''' <example>
    ''' let rawdata = open.mzpack("/path/to/rawdata.mzPack");
    ''' let spots = read.csv("/path/to/region.csv");
    ''' let into = MSI_summary(rawdata, x = as.integer(spots$x), 
    '''       y = as.integer(spots$y), 
    '''       as.vector = TRUE);
    ''' 
    ''' print(as.data.frame(into));
    ''' print("view of the intensity vector:");
    ''' print([into]::totalIon);
    ''' </example>
    <ExportAPI("MSI_summary")>
    <RApiReturn(GetType(MSISummary), GetType(iPixelIntensity))>
    Public Function MSI_summary(raw As mzPack,
                                Optional x As Integer() = Nothing,
                                Optional y As Integer() = Nothing,
                                Optional as_vector As Boolean = False,
                                <RRawVectorArgument>
                                Optional dims As Object = Nothing,
                                Optional env As Environment = Nothing) As Object

        Dim dimSize = InteropArgumentHelper.getSize(dims, env, [default]:="0,0")
        Dim dimsVal As Size? = Nothing

        If dimSize <> "0,0" Then
            dimsVal = dimSize.SizeParser
        Else
            dimsVal = raw.GetMSIMetadata.GetDimension
        End If

        Dim pixelFilter As IEnumerable(Of iPixelIntensity) = raw.Summary(filter:=GetXySpatialFilter(x, y))

        If Not (x.IsNullOrEmpty OrElse y.IsNullOrEmpty) Then
            Dim pixels As Index(Of String) = x _
                .Select(Function(xi, i) $"{xi},{y(i)}") _
                .Indexing

            If as_vector Then
                ' removes the duplicated pixels
                pixelFilter = pixelFilter _
                    .GroupBy(Function(p) $"{p.x},{p.y}") _
                    .Select(Function(p) p.First)
            End If

            ' pixel and also re-order by xy
            pixelFilter = From p As iPixelIntensity
                          In pixelFilter
                          Order By pixels.IndexOf($"{p.x},{p.y}")
        End If

        If as_vector Then
            Return pixelFilter.ToArray
        Else
            Return MSISummary.FromPixels(
                pixels:=pixelFilter,
                dims:=dimsVal
            )
        End If
    End Function

    ''' <summary>
    ''' calculate the X axis scale
    ''' </summary>
    ''' <param name="totalTime">the max rt of the y scan data</param>
    ''' <param name="pixels">the average pixels of all your y scan data</param>
    ''' <param name="hasMs2">does the ms-imaging raw data contains any ms scan data in ms2 level?</param>
    ''' <returns>
    ''' A x axis correction function wrapper, the clr object type of this 
    ''' function return value is determined based on the flag parameter
    ''' <paramref name="hasMs2"/>:
    ''' 
    ''' 1. for has ms2 data inside your ms-imaging rawdata, a <see cref="ScanMs2Correction"/> object should be used,
    ''' 2. for has no ms2 data, a <see cref="ScanTimeCorrection"/> object is used 
    '''    for run x axis correction based on the average rt diff.
    ''' </returns>
    <ExportAPI("correction")>
    <RApiReturn(GetType(Correction))>
    Public Function Correction(totalTime As Double, pixels As Integer, Optional hasMs2 As Boolean = False) As Object
        If hasMs2 Then
            Return New ScanMs2Correction(totalTime, pixels)
        Else
            Return New ScanTimeCorrection(totalTime, pixels)
        End If
    End Function

    ''' <summary>
    ''' Get the mass spectrum data of the MSI base peak data
    ''' </summary>
    ''' <param name="summary"></param>
    ''' <returns></returns>
    <ExportAPI("basePeakMz")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function basePeakMz(summary As MSISummary) As LibraryMatrix
        Return summary.GetBasePeakMz
    End Function

    ''' <summary>
    ''' Extract the ion features inside a MSI raw data slide sample file
    ''' </summary>
    ''' <param name="raw">
    ''' the raw data object could be a mzpack data object or 
    ''' MS-imaging ion feature layers object
    ''' </param>
    ''' <param name="grid_size">
    ''' the grid cell size for evaluate the pixel density
    ''' </param>
    ''' <param name="da">the mass tolerance value, only works when
    ''' the input raw data object is mzpack object</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' count pixels/density/etc for each ions m/z data
    ''' </remarks>
    <ExportAPI("ionStat")>
    <RApiReturn(GetType(IonStat))>
    Public Function IonStats(<RRawVectorArgument>
                             raw As Object,
                             Optional grid_size As Integer = 5,
                             Optional da As Double = 0.01,
                             Optional parallel As Boolean = True,
                             Optional env As Environment = Nothing) As Object

        If TypeOf raw Is mzPack Then
            Return IonStat.DoStat(
                raw:=DirectCast(raw, mzPack),
                nsize:=grid_size,
                da:=da,
                parallel:=parallel
            ) _
            .OrderBy(Function(s) s.pvalue) _
            .ToArray
        ElseIf TypeOf raw Is MzMatrix Then
            Return IonStat _
                .DoStat(DirectCast(raw, MzMatrix), grid_size, parallel) _
                .OrderBy(Function(s) s.pvalue) _
                .ToArray
        Else
            Dim layers = pipeline.TryCreatePipeline(Of SingleIonLayer)(raw, env)

            If layers.isError Then
                Return layers.getError
            End If

            Return env.EvaluateFramework(Of SingleIonLayer, IonStat)(
                x:=layers.populates(Of SingleIonLayer)(env),
                eval:=Function(layer)
                          Return IonStat.DoStat(layer, nsize:=grid_size)
                      End Function,
                parallel:=parallel
            )
        End If
    End Function

    <ExportAPI("ions_jointmatrix")>
    Public Function GetIonsJointMatrix(raw As list, Optional env As Environment = Nothing) As rDataframe
        Dim allStats = raw.getNames _
            .AsParallel _
            .Select(Function(name)
                        Return New NamedValue(Of IonStat())(name, IonStats(raw.getValue(Of mzPack)(name, env)))
                    End Function) _
            .ToArray
        Dim allMz As Double() = allStats _
            .Select(Function(i) i.Value) _
            .IteratesALL _
            .Select(Function(i) i.mz) _
            .GroupBy(Tolerance.DeltaMass(0.05)) _
            .Select(Function(i) Val(i.name)) _
            .ToArray
        Dim mat As New rDataframe With {
            .rownames = allMz _
                .Select(Function(mzi) mzi.ToString("F4")) _
                .ToArray,
            .columns = New Dictionary(Of String, Array)
        }
        Dim daErr As Tolerance = Tolerance.DeltaMass(0.1)

        For Each file In allStats
            Dim pixels = allMz _
                .Select(Function(mzi)
                            Return file.Value _
                                .Where(Function(i) daErr.Equals(i.mz, mzi)) _
                                .Select(Function(i) i.pixels) _
                                .OrderByDescending(Function(i) i) _
                                .FirstOrDefault
                        End Function) _
                .ToArray

            mat.add(file.Name, pixels)
        Next

        Return mat
    End Function

    ''' <summary>
    ''' combine each row scan raw data files as the pixels 2D matrix
    ''' </summary>
    ''' <param name="rowScans">
    ''' data result comes from the function ``row.scans``.
    ''' </param>
    ''' <param name="yscale">
    ''' apply for mapping smooth MS1 to ms2 scans
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("scans2D")>
    Public Function pixels2D(<RRawVectorArgument>
                             rowScans As Object,
                             Optional correction As Correction = Nothing,
                             Optional intocutoff As Double = 0.05,
                             Optional yscale As Double = 1,
                             Optional env As Environment = Nothing) As Object

        Dim pipeline As pipeline = pipeline.TryCreatePipeline(Of mzPack)(rowScans, env)
        Dim println = env.WriteLineHandler

        If yscale <> 1.0 Then
            Call println($"yscale is {yscale}")
        End If

        If pipeline.isError Then
            Return pipeline.getError
        Else
            Return pipeline _
                .populates(Of mzPack)(env) _
                .MSICombineRowScans(
                    correction:=correction,
                    intocutoff:=intocutoff,
                    yscale:=yscale,
                    progress:=Sub(msg)
                                  Call println(msg)
                              End Sub
                )
        End If
    End Function

    ''' <summary>
    ''' combine each row scan summary vector as the pixels 2D matrix
    ''' </summary>
    ''' <param name="rowScans"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("scanMatrix")>
    <RApiReturn(GetType(MSISummary))>
    Public Function MSIScanMatrix(<RRawVectorArgument> rowScans As Object, Optional env As Environment = Nothing) As Object
        Dim data As pipeline = pipeline.TryCreatePipeline(Of iPixelIntensity)(rowScans, env)

        If data.isError Then
            Return data.getError
        End If

        Return data _
            .populates(Of iPixelIntensity)(env) _
            .DoCall(AddressOf MSISummary.FromPixels)
    End Function

    ''' <summary>
    ''' Extract the ion data matrix
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="topN">
    ''' select top N ion feature in each spot and then union the ion features as 
    ''' the features set, this parameter only works when the <paramref name="ionSet"/> 
    ''' parameter is empty or null.
    ''' </param>
    ''' <param name="ionSet">
    ''' A tuple list of the ion dataset range, the tuple list object should 
    ''' be in data format of [unique_id => mz]. Or this parameter value could also
    ''' be a numeric vector of the target m/z feature values
    ''' </param>
    ''' <param name="mzError">The mass tolerance of the ion m/z</param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' the data format of the two kind of the output data result is keeps the same:
    ''' 
    ''' + for a raw matrix object, the column is the ion features and the rows is the spatial spots.
    ''' + for a dataset collection vector, the column is also the ion features and the 
    '''   rows is the spatial spots.
    ''' </returns>
    ''' <example>
    ''' let raw = open.mzpack("/path/to/rawdata.mzPack");
    ''' let ionsSet = list(ion1 = 100.0321, ion2 = 563.2254, ion3 = 336.9588);
    ''' 
    ''' MSI::peakMatrix(raw, ionSet = ionsSet, mzError = "da:0.05");
    ''' </example>
    <ExportAPI("peakMatrix")>
    <RApiReturn(GetType(MzMatrix), GetType(DataSet))>
    Public Function PeakMatrix(raw As mzPack,
                               Optional topN As Integer = 3,
                               Optional mzError As Object = "da:0.05",
                               <RRawVectorArgument>
                               Optional ionSet As Object = Nothing,
                               Optional raw_matrix As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        Dim err = Math.getTolerance(mzError, env)
        Dim println = env.WriteLineHandler

        If err Like GetType(Message) Then
            Return err.TryCast(Of Message)
        End If

        Call println($"extract ion feature data with mass tolerance: {err.TryCast(Of Tolerance).ToString}")

        If raw_matrix Then
            Call println("the raw ions feature matrix object will be returned!")
        End If

        If Not ionSet Is Nothing Then
            Return raw.GetPeakMatrix(ionSet, err.TryCast(Of Tolerance), raw_matrix, env)
        ElseIf raw_matrix Then
            Dim topIons As Double() = raw.GetMzIndex(mzdiff:=err.TryCast(Of Tolerance).GetErrorDalton, topN:=topN)
            Dim m = Deconvolute.PeakMatrix.CreateMatrix(
                raw:=raw,
                mzdiff:=err.TryCast(Of Tolerance).GetErrorDalton,
                freq:=0,
                mzSet:=topIons
            )

            Return m
        Else
            Return raw _
                .TopIonsPeakMatrix(topN, err.TryCast(Of Tolerance)) _
                .ToArray
        End If
    End Function

    <Extension>
    Private Function GetPeakMatrix(raw As mzPack, ionSet As Object, err As Tolerance,
                                   rawMatrix As Boolean,
                                   env As Environment) As Object

        Dim ions As Dictionary(Of String, Double)

        If TypeOf ionSet Is list Then
            ions = DirectCast(ionSet, list).AsGeneric(Of Double)(env)
        ElseIf ionSet.GetType.ImplementInterface(Of IDictionary) Then
            ions = RConversion.asList(ionSet, New list, env)
        Else
            Dim mz As Double() = CLRVector.asNumeric(ionSet)
            Dim keys As String() = mz _
                .Select(Function(m) m.ToString) _
                .uniqueNames

            ions = keys.Zip(mz) _
                .ToDictionary(Function(m) m.First,
                              Function(m)
                                  Return m.Second
                              End Function)
        End If

        If rawMatrix Then
            Return Deconvolute.PeakMatrix.CreateMatrix(raw, err.GetErrorDalton, 0, mzSet:=ions.Values.ToArray)
        Else
            Return raw _
                .SelectivePeakMatrix(ions, err) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' split the raw MSI 2D data into multiple parts with given resolution parts
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="resolution"></param>
    ''' <param name="mzError"></param>
    ''' <param name="cutoff"></param>
    ''' <param name="env"></param>
    ''' <returns>returns the raw matrix data that contains the peak samples.</returns>
    <ExportAPI("peakSamples")>
    <RApiReturn(GetType(DataSet))>
    Public Function peakSamples(raw As mzPack,
                                Optional resolution As Integer = 100,
                                Optional mzError As Object = "da:0.05",
                                Optional cutoff As Double = 0.05,
                                Optional env As Environment = Nothing) As Object

        Dim err = Math.getTolerance(mzError, env)

        If err Like GetType(Message) Then
            Return err.TryCast(Of Message)
        End If

        Dim sampler As New PixelsSampler(New ReadRawPack(raw))
        Dim sampling As Size = sampler.MeasureSamplingSize(resolution)
        Dim samples = sampler.Sampling(sampling, err.TryCast(Of Tolerance)).ToArray
        Dim matrix As DataSet() = samples _
            .AlignMzPeaks(
                mzErr:=err.TryCast(Of Tolerance),
                cutoff:=cutoff,
                getPeaks:=Function(p) p.GetMs,
                getSampleId:=Function(p) $"{p.X},{p.Y}"
            ) _
            .ToArray

        Return matrix
    End Function

    ''' <summary>
    ''' get number of ions in each pixel scans
    ''' </summary>
    ''' <param name="raw">
    ''' should be a mzpack object that contains multiple spatial spot scans data.
    ''' </param>
    ''' <returns>an integer vector of the number of ions in each spatial spot scans</returns>
    <ExportAPI("pixelIons")>
    <RApiReturn(TypeCodes.integer)>
    Public Function PixelIons(raw As mzPack) As Object
        Return raw.MS _
            .Select(Function(scan) scan.size) _
            .ToArray
    End Function

    ''' <summary>
    ''' get matrix ions feature m/z vector
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="q"></param>
    ''' <param name="fast_bins"></param>
    ''' <returns></returns>
    <ExportAPI("getMatrixIons")>
    <RApiReturn(TypeCodes.double)>
    Public Function GetMatrixIons(<RRawVectorArgument> raw As Object,
                                  Optional mzdiff As Double = 0.001,
                                  Optional q As Double = 0.001,
                                  Optional fast_bins As Boolean = True,
                                  Optional verbose As Boolean = False,
                                  Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of mzPack)(raw, env)
        Dim pool As mzPack()

        If pull.isError Then
            Return pull.getError
        Else
            pool = pull _
                .populates(Of mzPack)(env) _
                .ToArray
        End If

        If pool.Length = 1 Then
            Return SingleCellMath.GetMzIndex(pool(0), mzdiff, q, fast:=fast_bins)
        End If

        ' get all mz ions from the rawdata
        Return pool.AsParallel _
            .Select(Function(rawdata)
                        Return GetMzIndex(
                            raw:=rawdata,
                            mzdiff:=mzdiff, freq:=q,
                            fast:=True,
                            verbose:=verbose
                        )
                    End Function) _
            .AsList() _
            .DoCall(Function(mzBins)
                        Return GetMzIndexFastBin(mzBins, mzdiff, q, verbose:=verbose)
                    End Function)
    End Function

    ''' <summary>
    ''' dumping raw data matrix as text table file. 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="file"></param>
    ''' <param name="mzdiff">
    ''' the mass tolerance width for extract the feature ions
    ''' </param>
    ''' <param name="q">
    ''' the frequence threshold for filter the feature ions, this 
    ''' value range of this parameter should be inside [0,1] which
    ''' means percentage cutoff.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>This function returns a logical value TRUE if the 
    ''' given <paramref name="file"/> stream buffer is not missing,
    ''' otherwise the matrix object itself will be returns from 
    ''' the function.</returns>
    <ExportAPI("pixelMatrix")>
    <RApiReturn(GetType(Boolean), GetType(MzMatrix))>
    Public Function PixelMatrix(raw As mzPack,
                                Optional file As Object = Nothing,
                                Optional mzdiff As Double = 0.001,
                                Optional q As Double = 0.01,
                                Optional fast_bin As Boolean = True,
                                Optional verbose As Boolean = False,
                                Optional env As Environment = Nothing) As Object

        Dim matrix As MzMatrix = SingleCellMatrix.CreateMatrix(
            raw, mzdiff,
            freq:=q,
            fastBin:=fast_bin,
            verbose:=verbose
        )
        Dim println = env.WriteLineHandler

        Call println($"Extract pixel matrix with mzdiff:{mzdiff}, frequency:{q}")
        Call println($"get {matrix.mz.Length} ions with {matrix.matrix.Length} pixel spots")
        Call println("get ion features:")
        Call println(matrix.mz)

        If file Is Nothing Then
            Return matrix
        End If

        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Call matrix.ExportCsvSheet(buf.TryCast(Of Stream))
        Call buf.TryCast(Of Stream).Flush()

        Call println("matrix created!")

        Return True
    End Function

    ''' <summary>
    ''' sum pixels for create pixel spot convolution
    ''' </summary>
    ''' <param name="mat">A matrix liked dataframe object that contains the 
    ''' molecule expression data on each spatial spots, data object should 
    ''' in format of spatial spot in columns and molecule feature in rows.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("levels.convolution")>
    Public Function level_convolution(mat As rDataframe, Optional clusters As Integer = 6, Optional win_size As Integer = 3) As rDataframe
        Dim spatial_vector = mat.columns.AsParallel _
            .Select(Function(a)
                        Return (spot_id:=a.Key, vec:=CLRVector.asNumeric(a.Value))
                    End Function) _
            .OrderByDescending(Function(a) a.vec.Sum) _
            .ToArray
        Dim cluster_groups = spatial_vector.Split(spatial_vector.Length / clusters + 1)
        Dim convolution As New rDataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = mat.getRowNames
        }

        For Each cluster In cluster_groups
            Dim slides = cluster.SlideWindows(winSize:=win_size).ToArray

            For Each cov In slides
                Dim v As Double() = cov.First.vec

                For Each vi In cov.Skip(1)
                    v = SIMD.Add.f64_op_add_f64(v, vi.vec)
                Next

                Call convolution.add(cov.First.spot_id, v)
            Next
        Next

        Return convolution
    End Function

    ''' <summary>
    ''' sum pixels for create pixel spot convolution
    ''' </summary>
    ''' <param name="mat">A matrix liked dataframe object that contains the 
    ''' molecule expression data on each spatial spots, data object should 
    ''' in format of spatial spot in columns and molecule feature in rows.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("spatial.convolution")>
    Public Function spatialConvolution(mat As rDataframe, Optional win_size As Integer = 2, Optional steps As Integer = 1) As rDataframe
        Dim spatial As Grid(Of SpotVector) = Grid(Of SpotVector).Create(SpotVector.LoadDataFrame(mat))
        Dim convolution As New rDataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = mat.getRowNames
        }

        For Each spot As SpotVector In spatial.EnumerateData
            If spot.X Mod steps = 0 AndAlso spot.Y Mod steps = 0 Then
                Dim x = spot.X, y = spot.Y
                Dim vec = spot.expression
                Dim v As Integer = 1

                For xi = x - win_size To x + win_size
                    For yi = y - win_size To y + win_size
                        If xi <> x AndAlso yi <> y Then
                            Dim vi = spatial.GetData(xi, yi)

                            If Not vi Is Nothing Then
                                vec += vi.expression
                                v += 1
                            End If
                        End If
                    Next
                Next

                Call convolution.add($"{x},{y}", vec / v)
            End If
        Next

        Return convolution
    End Function

    ''' <summary>
    ''' pack the matrix file as the MSI mzpack
    ''' </summary>
    ''' <param name="file">
    ''' the file resource reference to the csv table file, and the
    ''' csv file should be in format of ion peaks features in column
    ''' and spatial spot id in rows
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("pack_matrix")>
    Public Function packMatrix(<RRawVectorArgument> file As Object,
                               <RRawVectorArgument>
                               Optional dims As Object = Nothing,
                               Optional res As Double = 17,
                               Optional noise_cutoff As Double = 1,
                               Optional source_tag As String = "pack_matrix",
                               Optional env As Environment = Nothing) As Object
        Dim scans As ScanMS1()
        Dim msi_dims As Size = InteropArgumentHelper.getSize(dims, env, "0,0").SizeParser
        Dim metadata As Metadata = Nothing

        If TypeOf file Is rDataframe Then
            scans = DirectCast(file, rDataframe) _
                .packDf(noise_cutoff) _
                .ToArray
        Else
            Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

            If buf Like GetType(Message) Then
                Return buf.TryCast(Of Message)
            End If

            scans = New StreamReader(buf.TryCast(Of Stream)) _
                .packFile(noise_cutoff) _
                .ToArray
        End If

        If Not msi_dims.IsEmpty Then
            metadata = New Metadata With {
                .[class] = FileApplicationClass.MSImaging.ToString,
                .mass_range = scans _
                    .Where(Function(s) Not s Is Nothing) _
                    .Select(Function(s) s.mz.MinMax) _
                    .IteratesALL _
                    .MinMax,
                .resolution = res,
                .scan_x = msi_dims.Width,
                .scan_y = msi_dims.Height
            }
        End If

        Return New mzPack With {
            .MS = scans _
                .Where(Function(s) Not s Is Nothing) _
                .ToArray,
            .source = source_tag,
            .Application = FileApplicationClass.MSImaging,
            .metadata = If(metadata Is Nothing, Nothing, metadata.GetMetadata)
        }
    End Function

    Private Function scan(xy As Integer(), ionsMz As Double(), v As Double(), ByRef ti As Double, noise_cutoff As Double) As ScanMS1
        Dim ms As ms2() = ionsMz _
            .Select(Function(mzi, i)
                        Return New ms2 With {
                            .mz = mzi,
                            .intensity = v(i)
                        }
                    End Function) _
            .Where(Function(m) m.intensity > noise_cutoff) _
            .OrderByDescending(Function(m) m.intensity) _
            .ToArray

        If ms.IsNullOrEmpty Then
            Return Nothing
            ' zero means do not removes noise
        ElseIf noise_cutoff > 0 Then
            Dim maxinto As Double = ms(0).intensity

            ms = ms _
                .Where(Function(mzi) mzi.intensity / maxinto > 0.01) _
                .ToArray
        End If

        ti += 1.98

        Return New ScanMS1 With {
            .BPC = ms.First.intensity,
            .into = ms.Select(Function(i) i.intensity).ToArray,
            .meta = New Dictionary(Of String, String) From {
                {"x", xy(0)},
                {"y", xy(1)}
            },
            .mz = ms.Select(Function(m) m.mz).ToArray,
            .products = Nothing,
            .rt = ti,
            .TIC = .into.Sum,
            .scan_id = $"[MS1] [{xy(0)},{xy(1)}] {ms.Length}ions: totalIons={ .TIC} basePeak={ .BPC} basepeak_m/z={ms.First.mz}"
        }
    End Function

    <Extension>
    Private Iterator Function packDf(df As rDataframe, noise_cutoff As Double) As IEnumerable(Of ScanMS1)
        Dim ionsMz As Double() = CLRVector.asNumeric(df.colnames)
        Dim ti As Double = 0

        For Each row As NamedCollection(Of Object) In df.forEachRow
            If row.name.IndexOf("_") >= 0 Then
                ' is duplicated spot
                Continue For
            End If

            Dim xy As Integer() = row.name _
                .Split(","c) _
                .Select(Function(si) CInt(Val(si))) _
                .ToArray
            Dim v As Double() = CLRVector.asNumeric(row.value)

            Yield scan(xy, ionsMz, v, ti, noise_cutoff)
        Next
    End Function

    <Extension>
    Private Iterator Function packFile(read As StreamReader, noise_cutoff As Double) As IEnumerable(Of ScanMS1)
        Dim ionsMz As Double() = RowObject.TryParse(read.ReadLine) _
            .Skip(1) _
            .Select(Function(si) Val(si)) _
            .ToArray
        Dim line As Value(Of String) = ""
        Dim ti As Double = 0

        Do While (line = read.ReadLine) IsNot Nothing
            Dim t As String() = Tokenizer.CharsParser(line).ToArray

            If t(0).IndexOf("_") >= 0 Then
                ' is duplicated spot
                Continue Do
            End If

            Dim xy As Integer() = t(0) _
                .Split(","c) _
                .Select(Function(si) CInt(Val(si))) _
                .ToArray
            Dim v As Double() = t _
                .Skip(1) _
                .Select(Function(si) Val(si)) _
                .ToArray

            Yield scan(xy, ionsMz, v, ti, noise_cutoff)
        Loop
    End Function

    ''' <summary>
    ''' evaluate the moran index for each ion layer
    ''' </summary>
    ''' <param name="x">
    ''' A spatial expression data matrix, should be in format of:
    ''' 
    ''' 1. the spatial spot xy in row names, and
    ''' 2. the ions feature m/z label in col names
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("moran_I")>
    Public Function moran_index(x As rDataframe) As Object
        Dim xy As Double()() = x.rownames _
            .Select(Function(si)
                        Return si.Split(","c).Select(Function(si2) Val(si2)).ToArray
                    End Function) _
            .ToArray
        Dim sx As Double() = xy.Select(Function(i) i(0)).ToArray
        Dim sy As Double() = xy.Select(Function(i) i(1)).ToArray
        Dim moran = x.colnames _
            .AsParallel _
            .Select(Function(lbMz)
                        Dim v As Double() = CLRVector.asNumeric(x(lbMz))
                        Dim m As MoranTest = MoranTest.moran_test(v, sx, sy, throwMaxIterError:=False)

                        Return (lbMz, m)
                    End Function) _
            .OrderByDescending(Function(m) m.m.Observed) _
            .ToArray
        Dim df As New rDataframe With {
            .rownames = moran.Select(Function(i) i.lbMz).ToArray,
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add("m/z", moran.Select(Function(i) Val(i.lbMz)))
        Call df.add("moran I", moran.Select(Function(i) i.m.Observed))
        Call df.add("moran i", moran.Select(Function(i) i.m.Expected))
        Call df.add("score", moran.Select(Function(i) i.m.Observed * (-std.Log(i.m.pvalue))))
        Call df.add("sd", moran.Select(Function(i) i.m.SD))
        Call df.add("p-value", moran.Select(Function(i) i.m.pvalue))

        Return df
    End Function

    ''' <summary>
    ''' make expression bootstrapping of current ion layer
    ''' </summary>
    ''' <param name="layer">The target ion layer to run expression bootstraping</param>
    ''' <param name="tissue">A collection of the <see cref="TissueRegion"/> object.</param>
    ''' <param name="n">Get n sample points for each tissue region</param>
    ''' <param name="coverage">The region area coverage for the bootstrapping.</param>
    ''' <returns>
    ''' A tuple list object that contains the expression data for each <see cref="TissueRegion"/>:
    ''' 
    ''' 1. the tuple key is the label of the tissue region data,
    ''' 2. the tuple value is the numeric expression vector that sampling from 
    '''    the corrisponding tissue region, the vector size is equals to the 
    '''    parameter ``n``.
    ''' </returns>
    ''' <remarks>
    ''' Bootstrapping is a statistical procedure that resamples a single dataset to create
    ''' many simulated samples. This process allows you to calculate standard errors, 
    ''' construct confidence intervals, and perform hypothesis testing for numerous types of
    ''' sample statistics. Bootstrap methods are alternative approaches to traditional 
    ''' hypothesis testing and are notable for being easier to understand and valid for more 
    ''' conditions.
    ''' </remarks>
    <ExportAPI("sample_bootstraping")>
    Public Function SampleBootstraping(layer As SingleIonLayer, tissue As TissueRegion(),
                                       Optional n As Integer = 32,
                                       Optional coverage As Double = 0.3) As Object

        Return layer.MSILayer.ExtractSample(tissue, n, coverage)
    End Function

    ''' <summary>
    ''' cast the rawdata matrix as the ms-imaging ion layer
    ''' </summary>
    ''' <param name="x">the matrix object</param>
    ''' <param name="mzdiff">the mass tolerance error in <see cref="DAmethod"/></param>
    ''' <param name="dims">
    ''' the dimension size of the ms-imaging spatial data
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("cast.spatial_layers")>
    <RApiReturn(GetType(SingleIonLayer))>
    Public Function castSpatialLayers(x As MzMatrix,
                                      Optional mzdiff As Double = 0.01,
                                      <RRawVectorArgument>
                                      Optional dims As Object = Nothing,
                                      Optional env As Environment = Nothing) As Object
        Dim mz As Double() = x.mz
        Dim diff1 As Tolerance = New DAmethod(mzdiff)
        Dim diff2 As Tolerance = Tolerance.ParseScript(x.tolerance)
        Dim size As Size = InteropArgumentHelper.getSize(dims, env, [default]:="0,0").SizeParser

        If diff1 > diff2 Then
            ' the mzdiff is greater than the matrix tolerance
            ' needs to centroid the mz features
            mz = mz.GroupBy(offset:=mzdiff) _
                .Select(Function(a) a.Average) _
                .ToArray
        End If

        Return New MsImaging.MatrixReader(x) _
            .ForEachLayer(mz, dims:=size) _
            .ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="features">the ion features m/z vector</param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    <ExportAPI("z_header")>
    Public Function z_header(<RRawVectorArgument> features As Object, Optional mzdiff As Double = 0.001) As MatrixHeader
        Return New MatrixHeader With {
            .matrixType = FileApplicationClass.MSImaging3D,
            .tolerance = mzdiff.ToString,
            .mz = CLRVector.asNumeric(features),
            .numSpots = 0
        }
    End Function

    ''' <summary>
    ''' Create mzpack object for ms-imaging in 3D
    ''' </summary>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("z_assembler")>
    Public Function z_assembler(header As MatrixHeader, file As Object, Optional env As Environment = Nothing) As Object
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If




        Return True
    End Function

    ''' <summary>
    ''' cast the ms-imaging layer data to raster object 
    ''' 
    ''' use this function for cast raster object, for do spatial heatmap rendering in another method.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="layer">
    ''' the layer type for create the raster object, this parameter only works 
    ''' for when the data type of <paramref name="x"/> is <see cref="MSISummary"/>.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("raster")>
    <RApiReturn(GetType(RasterMatrix))>
    Public Function asRaster(x As Object,
                             Optional layer As IntensitySummary = IntensitySummary.Total,
                             Optional env As Environment = Nothing) As Object
        If x Is Nothing Then
            Return x
        End If

        If TypeOf x Is SingleIonLayer Then
            Return DirectCast(x, SingleIonLayer).AsRaster
        ElseIf TypeOf x Is MSISummary Then
            Return DirectCast(x, MSISummary).AsRaster(kind:=layer)
        Else
            Return Message.InCompatibleType(GetType(SingleIonLayer), x.GetType, env)
        End If
    End Function
End Module

Public Class SpotVector : Implements IPoint2D

    Public Property X As Integer Implements IPoint2D.X
    Public Property Y As Integer Implements IPoint2D.Y
    Public Property expression As vector

    Public Overrides Function ToString() As String
        Return $"[{X},{Y}]"
    End Function

    Public Shared Iterator Function LoadDataFrame(mat As rDataframe) As IEnumerable(Of SpotVector)
        For Each col As KeyValuePair(Of String, Array) In mat.columns
            Dim t As String() = col.Key.Split(","c)
            Dim xy As Integer() = t.Select(AddressOf Integer.Parse).ToArray

            Yield New SpotVector With {
                .expression = CLRVector.asNumeric(col.Value).AsVector,
                .X = xy(0),
                .Y = xy(1)
            }
        Next
    End Function
End Class