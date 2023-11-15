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
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Emit.Delegates
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
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Internal.Object.Converts
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports imzML = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.XML
Imports rDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports SingleCellMath = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.Math
Imports SingleCellMatrix = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.PeakMatrix
Imports std = System.Math
Imports vector = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' MS-Imaging data handler
''' </summary>
<Package("MSI")>
Module MSI

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(IonStat()), AddressOf getStatTable)
    End Sub

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
    ''' <param name="m"></param>
    ''' <param name="factor">the size of this numeric vector should be equals to the 
    ''' ncol of the given dataframe input <paramref name="m"/>.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("scale")>
    Public Function scale(m As rDataframe, <RRawVectorArgument> factor As Object, Optional env As Environment = Nothing) As Object
        Dim f As Double() = CLRVector.asNumeric(factor)
        Dim v As Double()
        Dim cols As String() = m.colnames
        Dim name As String

        If f.Length <> cols.Length Then
            Return Internal.debug.stop($"the dimension of the factor vector({f.Length}) is not matched with the dataframe columns({cols.Length})!", env)
        End If

        m = New rDataframe(m)

        For i As Integer = 0 To cols.Length - 1
            name = cols(i)
            v = CLRVector.asNumeric(m.columns(name))
            v = SIMD.Divide.f64_op_divide_f64_scalar(v, v.Sum)
            v = SIMD.Multiply.f64_scalar_op_multiply_f64(f(i), v)
            m.columns(name) = ReLU.ReLU(v)
        Next

        Return m
    End Function

    ''' <summary>
    ''' get ms-imaging metadata
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
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
    ''' <param name="pixels"></param>
    ''' <param name="context"></param>
    ''' <param name="dims"></param>
    ''' <param name="strict"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.layer")>
    <RApiReturn(GetType(SingleIonLayer))>
    Public Function asMSILayer(pixels As MsImaging.PixelData(),
                               Optional context As String = "MSIlayer",
                               <RRawVectorArgument>
                               Optional dims As Object = Nothing,
                               Optional strict As Boolean = True,
                               Optional env As Environment = Nothing) As Object

        Dim size As String = InteropArgumentHelper.getSize(dims, env, [default]:="0,0")

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
    ''' <param name="raw"></param>
    ''' <param name="partition"></param>
    ''' <returns></returns>
    <ExportAPI("splice")>
    Public Function splice(raw As mzPack, Optional partition As Integer = 5) As mzPack()
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
    ''' <param name="raw"></param>
    ''' <param name="mz"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a character vector of the pixel [x,y] tags.
    ''' </returns>
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
    ''' imML/mzPack
    ''' </param>
    ''' <returns>
    ''' this function will returns the pixels in dimension size(a tuple list data with slot keys w and h) 
    ''' if the count is set to FALSE, by default; otherwise this function will return an integer value for
    ''' indicates the real pixel counts number if the count parameter is set to TRUE.
    ''' </returns>
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
                Return layer.DimensionSize
            End If
        ElseIf TypeOf file Is TissueRegion Then
            Dim region As TissueRegion = file

            If count Then
                Return region.nsize
            Else
                Return region.GetRectangle.Size
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

    <ExportAPI("open.imzML")>
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

    <ExportAPI("write.imzML")>
    Public Function write_imzML(mzpack As mzPack, file As String) As Object
        Return imzXMLWriter.WriteXML(mzpack, output:=file)
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
    ''' calculate the X scale
    ''' </summary>
    ''' <param name="totalTime"></param>
    ''' <param name="pixels"></param>
    ''' <param name="hasMs2"></param>
    ''' <returns></returns>
    <ExportAPI("correction")>
    Public Function Correction(totalTime As Double, pixels As Integer, Optional hasMs2 As Boolean = False) As Correction
        If hasMs2 Then
            Return New ScanMs2Correction(totalTime, pixels)
        Else
            Return New ScanTimeCorrection(totalTime, pixels)
        End If
    End Function

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
    ''' <returns></returns>
    ''' <example>
    ''' let raw = open.mzpack("/path/to/rawdata.mzPack");
    ''' let ionsSet = list(ion1 = 100.0321, ion2 = 563.2254, ion3 = 336.9588);
    ''' 
    ''' MSI::peakMatrix(raw, ionSet = ionsSet, mzError = "da:0.05");
    ''' </example>
    <ExportAPI("peakMatrix")>
    Public Function PeakMatrix(raw As mzPack,
                               Optional topN As Integer = 3,
                               Optional mzError As Object = "da:0.05",
                               <RRawVectorArgument>
                               Optional ionSet As Object = Nothing,
                               Optional env As Environment = Nothing) As Object

        Dim err = Math.getTolerance(mzError, env)

        If err Like GetType(Message) Then
            Return err.TryCast(Of Message)
        End If

        If Not ionSet Is Nothing Then
            Return raw.GetPeakMatrix(ionSet, err.TryCast(Of Tolerance), env)
        Else
            Return raw _
                .TopIonsPeakMatrix(topN, err.TryCast(Of Tolerance)) _
                .ToArray
        End If
    End Function

    <Extension>
    Private Function GetPeakMatrix(raw As mzPack, ionSet As Object, err As Tolerance, env As Environment) As DataSet()
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

        Return raw _
            .SelectivePeakMatrix(ions, err) _
            .ToArray
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
    ''' <param name="raw"></param>
    ''' <returns></returns>
    <ExportAPI("pixelIons")>
    Public Function PixelIons(raw As mzPack) As Integer()
        Return raw.MS.Select(Function(scan) scan.size).ToArray
    End Function

    <ExportAPI("getMatrixIons")>
    Public Function GetMatrixIons(raw As mzPack,
                                  Optional mzdiff As Double = 0.001,
                                  Optional q As Double = 0.001) As Double()

        Return SingleCellMath.GetMzIndex(raw, mzdiff, q)
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
                                Optional env As Environment = Nothing) As Object

        Dim matrix As MzMatrix = SingleCellMatrix.CreateMatrix(raw, mzdiff, freq:=q)
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
                               Optional env As Environment = Nothing) As Object
        Dim scans As ScanMS1()
        Dim msi_dims As Size = InteropArgumentHelper.getSize(dims, env, "0,0").SizeParser
        Dim metadata As Metadata = Nothing

        If TypeOf file Is rDataframe Then
            scans = DirectCast(file, rDataframe).packDf.ToArray
        Else
            Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

            If buf Like GetType(Message) Then
                Return buf.TryCast(Of Message)
            End If

            scans = New StreamReader(buf.TryCast(Of Stream)).packFile.ToArray
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
            .MS = scans.Where(Function(s) Not s Is Nothing).ToArray,
            .source = NameOf(packMatrix),
            .Application = FileApplicationClass.MSImaging,
            .metadata = If(metadata Is Nothing, Nothing, metadata.GetMetadata)
        }
    End Function

    Private Function scan(xy As Integer(), ionsMz As Double(), v As Double(), ByRef ti As Double) As ScanMS1
        Dim ms As ms2() = ionsMz _
            .Select(Function(mzi, i)
                        Return New ms2 With {
                            .mz = mzi,
                            .intensity = v(i)
                        }
                    End Function) _
            .Where(Function(m) m.intensity > 1) _
            .OrderByDescending(Function(m) m.intensity) _
            .ToArray

        If ms.IsNullOrEmpty Then
            Return Nothing
        Else
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
    Private Iterator Function packDf(df As rDataframe) As IEnumerable(Of ScanMS1)
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

            Yield scan(xy, ionsMz, v, ti)
        Next
    End Function

    <Extension>
    Private Iterator Function packFile(read As StreamReader) As IEnumerable(Of ScanMS1)
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

            Yield scan(xy, ionsMz, v, ti)
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

    <ExportAPI("sample_bootstraping")>
    Public Function SampleBootstraping(layer As SingleIonLayer, tissue As TissueRegion(),
                                       Optional n As Integer = 32,
                                       Optional coverage As Double = 0.3) As Object

        Return layer.MSILayer.ExtractSample(tissue, n, coverage)
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