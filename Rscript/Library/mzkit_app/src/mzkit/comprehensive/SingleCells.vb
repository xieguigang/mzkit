#Region "Microsoft.VisualBasic::798383c22c6c8ff5369b402ce3b3387f, Rscript\Library\mzkit_app\src\mzkit\comprehensive\SingleCells.vb"

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

    '   Total Lines: 683
    '    Code Lines: 403 (59.00%)
    ' Comment Lines: 208 (30.45%)
    '    - Xml Docs: 94.23%
    ' 
    '   Blank Lines: 72 (10.54%)
    '     File Size: 26.53 KB


    ' Module SingleCells
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: asHTSExpression, cell_clusters, cell_embedding, cellMatrix, cellStatsTable
    '               dfMzMatrix, embedding_sample, getCellLabels, getSpatialLabels, mzMatrixDf
    '               openMatrix, readMzmatrix, rowApplyScale, singleCellsIons, spatialLabels
    '               spot_vector, writeMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.NLP.Word2Vec
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.ComponentModel.Activations
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.[Interface]
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.[Object].dataframe
Imports SingleCellMath = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.Math
Imports SingleCellMatrix = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.PeakMatrix

''' <summary>
''' Single cells metabolomics data processor
''' 
''' Single-cell analysis is a technique that measures only the target cell itself and can 
''' extract information that would be buried in bulk-cell analysis with high-resolution.
''' </summary>
''' <remarks>
''' Single-cell metabolomics is a powerful tool that can reveal cellular heterogeneity and 
''' can elucidate the mechanisms of biological phenomena in detail. It is a promising 
''' approach in studying plants, especially when cellular heterogeneity has an impact on different 
''' biological processes. In addition, metabolomics, which can be regarded as a detailed 
''' phenotypic analysis, is expected to answer previously unrequited questions which will 
''' lead to expansion of crop production, increased understanding of resistance to diseases,
''' and in other applications as well.
''' </remarks>
<Package("SingleCells")>
Module SingleCells

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(SingleCellIonStat()), AddressOf cellStatsTable)
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(MzMatrix), AddressOf mzMatrixDf)
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(SpatialMatrixReader), AddressOf mzMatrixDf)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function cellStatsTable(ions As SingleCellIonStat(), args As list, env As Environment) As Rdataframe
        Dim table As New Rdataframe With {
           .columns = New Dictionary(Of String, Array),
           .rownames = ions _
               .Select(Function(i) i.mz.ToString("F4")) _
               .ToArray
        }

        Call table.add(NameOf(SingleCellIonStat.mz), ions.Select(Function(i) i.mz))
        Call table.add(NameOf(SingleCellIonStat.cells), ions.Select(Function(i) i.cells))
        Call table.add(NameOf(SingleCellIonStat.maxIntensity), ions.Select(Function(i) i.maxIntensity))
        Call table.add(NameOf(SingleCellIonStat.baseCell), ions.Select(Function(i) i.baseCell))
        Call table.add(NameOf(SingleCellIonStat.Q1Intensity), ions.Select(Function(i) i.Q1Intensity))
        Call table.add(NameOf(SingleCellIonStat.Q2Intensity), ions.Select(Function(i) i.Q2Intensity))
        Call table.add(NameOf(SingleCellIonStat.Q3Intensity), ions.Select(Function(i) i.Q3Intensity))
        Call table.add(NameOf(SingleCellIonStat.RSD), ions.Select(Function(i) i.RSD))

        Return table
    End Function

    ''' <summary>
    ''' cast the matrix object as the dataframe
    ''' </summary>
    ''' <param name="x">should be a rawdata object in general type: <see cref="MzMatrix"/>.</param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' implements the ``as.data.frame`` function
    ''' </remarks>
    ''' <example>
    ''' as.data.frame(x);
    ''' </example>
    <ExportAPI("mz_matrix")>
    <RApiReturn(GetType(Rdataframe))>
    Public Function mzMatrixDf(x As Object,
                               <RListObjectArgument>
                               args As list,
                               Optional env As Environment = Nothing) As Object

        Dim rawdata As MzMatrix

        If x Is Nothing Then
            Return Nothing
        End If

        If TypeOf x Is MzMatrix Then
            rawdata = x
        ElseIf TypeOf x Is SpatialMatrixReader Then
            rawdata = DirectCast(x, SpatialMatrixReader).getMatrix
        Else
            Return Message.InCompatibleType(GetType(MzMatrix), x.GetType, env)
        End If

        Dim singleCell As Boolean = args.getValue("singlecell", env, [default]:=False)
        Dim df As New Rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = If(singleCell, rawdata.getCellLabels, rawdata.getSpatialLabels)
        }
        Dim mz As Double() = rawdata.mz
        Dim offset As Integer
        Dim ionFeatureKey As String

        ' loop each ion mz feature
        For i As Integer = 0 To mz.Length - 1
            offset = i
            ionFeatureKey = mz(i).ToString
            df.add(
                key:=ionFeatureKey,
                value:=rawdata.matrix.Select(Function(r) r.intensity(offset))
            )
        Next

        Return df
    End Function

    ''' <summary>
    ''' Cast the ion feature matrix as the GCModeller expression matrix object
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns>
    ''' the gcmodeller expression matrix object, each <see cref="DataFrameRow"/> element inside the 
    ''' generated matrix object is the expression vector of all metabolite ion features. which means
    ''' the matrix format from this function outputs should be:
    ''' 
    ''' 1. cell labels, or spatial location in rows
    ''' 2. and ion features in columns.
    ''' </returns>
    ''' <example>
    ''' as.expression(x);
    ''' </example>
    <ExportAPI("as.expression")>
    <RApiReturn(GetType(HTSMatrix))>
    Public Function asHTSExpression(x As MzMatrix, Optional single_cell As Boolean = False) As Object
        Return New HTSMatrix With {
            .sampleID = x.mz _
                .Select(Function(mzi) mzi.ToString("F4")) _
                .ToArray,
            .tag = "ions_with_mzdiff:" & x.tolerance,
            .expression = x.matrix _
                .Select(Function(si)
                            Dim label As String

                            If single_cell Then
                                label = si.label
                            Else
                                label = $"{si.X},{si.Y}"
                            End If

                            Return New DataFrameRow With {
                                .geneID = label,
                                .experiments = si.intensity
                            }
                        End Function) _
                .ToArray
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function getCellLabels(x As MzMatrix) As String()
        Return x.matrix.Select(Function(r) r.label).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function getSpatialLabels(x As MzMatrix) As String()
        Return x.matrix.Select(Function(r) $"{r.X},{r.Y}").ToArray
    End Function

    ''' <summary>
    ''' scale matrix for each spot/cell sample
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="scaler">A R# <see cref="RFunction"/> for apply the scale transform.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' # use the internal function
    ''' x &lt;- apply.scale(x, scaler = max);
    ''' # use the lambda function
    ''' # the symbol x of the parameter in f function is a different thing 
    ''' # with the variable x
    ''' let f(x) = x ^ 2;
    ''' x &lt;- apply.scale(x, scaler = f);
    ''' </example>
    <ExportAPI("apply.scale")>
    <RApiReturn(GetType(MzMatrix))>
    Public Function rowApplyScale(x As Object, scaler As RFunction, Optional env As Environment = Nothing) As Object
        Dim lambda As Func(Of Double(), [Variant](Of Message, Double())) =
            Function(xi)
                Dim result As Object = scaler.Invoke(arguments:=New Object() {xi, env}, env)

                If TypeOf result Is Message Then
                    Return DirectCast(result, Message)
                Else
                    Return CLRVector.asNumeric(result)
                End If
            End Function
        Dim scaled As New List(Of PixelData)
        Dim v As [Variant](Of Message, Double())
        Dim into As Double()
        Dim m As MzMatrix

        If x Is Nothing Then
            Return Nothing
        End If
        If TypeOf x Is MzMatrix Then
            m = x
        ElseIf TypeOf x Is SpatialMatrixReader Then
            m = DirectCast(x, SpatialMatrixReader).getMatrix
        Else
            Return Message.InCompatibleType(GetType(MzMatrix), x.GetType, env)
        End If

        Dim t0 = Now
        Dim spots As Integer = m.matrix.Length
        Dim d As Integer = spots / 25
        Dim i As i32 = 0

        For Each spot As PixelData In m.matrix
            v = lambda(spot.intensity)

            If v Like GetType(Message) Then
                Return v.TryCast(Of Message)
            Else
                into = v.TryCast(Of Double())
                into = ReLU.ReLU(into)
            End If

            spot = New PixelData With {
                .label = spot.label,
                .X = spot.X,
                .Y = spot.Y,
                .intensity = into
            }
            scaled.Add(spot)

            If (++i Mod d) = 0 Then
                Call VBDebugger.EchoLine($"[{i}/{spots}] ({spot.ToString}) {(i / spots * 100).ToString("F2")}% ... {StringFormats.ReadableElapsedTime((Now - t0).TotalMilliseconds)}")
            End If
        Next

        Return New MzMatrix With {
            .matrix = scaled.ToArray,
            .mz = m.mz.ToArray,
            .tolerance = m.tolerance
        }
    End Function

    ''' <summary>
    ''' export single cell expression matrix from the raw data scans
    ''' </summary>
    ''' <param name="raw">the raw data for make epxression matrix, could be a mzkit <see cref="mzPack"/> object, 
    ''' or a tuple list of the msdata <see cref="BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.LibraryMatrix"/></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' let rawdata = open.mzpack("/file.mzpack");
    ''' let matrix = cell_matrix(rawdata, mz_matrix = TRUE);
    ''' 
    ''' write.matrix(matrix, file = "/save.dat");
    ''' </example>
    <ExportAPI("cell_matrix")>
    <RApiReturn(GetType(HTSMatrix), GetType(MzMatrix))>
    Public Function cellMatrix(<RRawVectorArgument> raw As Object,
                               Optional mzdiff As Double = 0.005,
                               Optional freq As Double = 0.001,
                               <RRawVectorArgument>
                               Optional ions_mz As Object = Nothing,
                               Optional mz_matrix As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        Dim singleCells As New List(Of DataFrameRow)
        Dim mzSet As Double()
        Dim source As String

        If raw Is Nothing Then
            Return Nothing
        End If

        If TypeOf raw Is mzPack Then
            Dim mzpack As mzPack = DirectCast(raw, mzPack)

            source = mzpack.source
            mzSet = SingleCellMath.GetMzIndex(
                raw:=mzpack,
                mzdiff:=mzdiff,
                freq:=freq
            )

            For Each cell_scan As DataFrameRow In SingleCellMatrix.ExportScans(Of DataFrameRow)(mzpack, mzSet)
                cell_scan.geneID = cell_scan.geneID _
                    .Replace("[MS1]", "") _
                    .Trim
                singleCells.Add(cell_scan)
            Next
        ElseIf TypeOf raw Is list Then
            Dim msdata As Dictionary(Of String, LibraryMatrix) = DirectCast(raw, list).AsGeneric(Of LibraryMatrix)(env)

            source = "msdata"
            mzSet = msdata.Values _
                .IteratesALL _
                .ToArray _
                .Centroid(Tolerance.DeltaMass(mzdiff), New RelativeIntensityCutoff(0)) _
                .Select(Function(mzi) mzi.mz) _
                .ToArray

            For Each cell_scan As DataFrameRow In SingleCellMatrix.ExportScans(Of DataFrameRow)(msdata.Values, mzSet)
                cell_scan.geneID = cell_scan.geneID _
                    .Replace("[MS1]", "") _
                    .Trim
                singleCells.Add(cell_scan)
            Next
        ElseIf TypeOf raw Is dataframe Then
            Dim ions As Double() = CLRVector.asNumeric(ions_mz)
            Dim mat As New MzMatrix With {
                .mz = ions,
                .tolerance = mzdiff
            }
            Dim samples As New List(Of PixelData)

            For Each sample In DirectCast(raw, dataframe).forEachRow
                samples.Add(New PixelData With {
                    .label = sample.name,
                    .intensity = CLRVector.asNumeric(sample.value)
                })
            Next

            mat.matrix = samples.ToArray

            Return mat
        Else
            Return Message.InCompatibleType(GetType(mzPack), raw.GetType, env)
        End If

        If mz_matrix Then
            Return New MzMatrix With {
                .tolerance = $"da:{mzdiff}",
                .matrixType = FileApplicationClass.SingleCellsMetabolomics,
                .mz = mzSet,
                .matrix = singleCells _
                    .Select(Function(v) PixelData.FromSingleCellExpression(v)) _
                    .ToArray
            }
        Else
            Return New HTSMatrix With {
                .expression = singleCells.ToArray,
                .sampleID = mzSet _
                    .Select(Function(mzi) mzi.ToString("F4")) _
                    .ToArray,
                .tag = source
            }
        End If
    End Function

    ''' <summary>
    ''' do statistics of the single cell metabolomics ions features
    ''' </summary>
    ''' <param name="raw">the <see cref="mzPack"/> rawdata object, or a single cells matrix object</param>
    ''' <param name="da"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    <ExportAPI("SCM_ionStat")>
    <RApiReturn(GetType(SingleCellIonStat))>
    Public Function singleCellsIons(raw As Object,
                                    Optional da As Double = 0.01,
                                    Optional parallel As Boolean = True,
                                    Optional env As Environment = Nothing) As Object

        If TypeOf raw Is mzPack Then
            ' needs extract the ion features
            ' and then do cell counts analysis
            Return SingleCellIonStat.DoIonStats(DirectCast(raw, mzPack), da, parallel).ToArray
        ElseIf TypeOf raw Is MzMatrix Then
            Return SingleCellIonStat.DoIonStats(DirectCast(raw, MzMatrix), parallel).ToArray
        Else
            Return Message.InCompatibleType(GetType(MzMatrix), raw.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' write the single cell ion feature data matrix
    ''' </summary>
    ''' <param name="x">the expression <see cref="MzMatrix"/> object.</param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' let rawdata = open.mzpack("/file.mzpack");
    ''' let matrix = cell_matrix(rawdata, mz_matrix = TRUE);
    ''' 
    ''' write.matrix(matrix, file = "/save.dat");
    ''' </example>
    <ExportAPI("write.matrix")>
    <RApiReturn(TypeCodes.boolean)>
    Public Function writeMatrix(x As MzMatrix, file As Object, Optional env As Environment = Nothing) As Object
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        ' save
        Call New MatrixWriter(x).Write(buf.TryCast(Of Stream))
        Call buf.TryCast(Of Stream).Flush()

        If TypeOf file Is String Then
            Call buf.TryCast(Of Stream).Dispose()
        End If

        Return True
    End Function

    ''' <summary>
    ''' open a single cell data matrix reader
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' A tuple list that contains the data elements:
    ''' 
    ''' 1. tolerance: the mass tolerance description for seperates the ion features
    ''' 2. featureSize: the number of the ion features in the raw data file
    ''' 3. ionSet: a numeric vector of the ion features m/z value.
    ''' 4. spots: the number of the spots that read from the rawdata matrix file
    ''' 5. reader: the rawdata <see cref="MatrixReader"/>
    ''' </returns>
    ''' <remarks>
    ''' this function open a lazy reader of the matrix, for load all 
    ''' data into memory at once, use the ``read.mz_matrix`` 
    ''' function.
    ''' </remarks>
    ''' <example>
    ''' let reader = open.matrix("/file.dat");
    ''' let matrix = read.mz_matrix(reader$reader);
    ''' </example>
    <ExportAPI("open.matrix")>
    <RApiReturn(
        NameOf(MatrixReader.tolerance),
        NameOf(MatrixReader.featureSize),
        NameOf(MatrixReader.ionSet),
        NameOf(MatrixReader.spots),
        "reader"
    )>
    Public Function openMatrix(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Dim read As New MatrixReader(buf.TryCast(Of Stream))
        Dim summary As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        Call summary.add(NameOf(MatrixReader.tolerance), read.tolerance)
        Call summary.add(NameOf(MatrixReader.featureSize), read.featureSize)
        Call summary.add(NameOf(MatrixReader.ionSet), read.ionSet)
        Call summary.add(NameOf(MatrixReader.spots), read.spots)
        Call summary.add("reader", read)

        Return summary
    End Function

    ''' <summary>
    ''' load the data matrix into memory at once
    ''' </summary>
    ''' <param name="file">
    ''' a file connection to the matrix file or the matrix lazy 
    ''' reader object which is created via the function 
    ''' ``open.matrix``.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' for create a lazy data reader of the matrix, use the ``open.matrix`` function.
    ''' </remarks>
    ''' <example>
    ''' read.mz_matrix("/file.dat");
    ''' </example>
    <ExportAPI("read.mz_matrix")>
    <RApiReturn(GetType(MzMatrix))>
    Public Function readMzmatrix(<RRawVectorArgument>
                                 file As Object,
                                 Optional env As Environment = Nothing) As Object

        If TypeOf file Is MatrixReader Then
            Return DirectCast(file, MatrixReader).LoadMemory
        Else
            Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

            If buf Like GetType(Message) Then
                Return buf.TryCast(Of Message)
            End If

            Dim read As New MatrixReader(buf.TryCast(Of Stream))
            Dim ms As MzMatrix = read.LoadMemory

            Call read.Dispose()

            Return ms
        End If
    End Function

    ''' <summary>
    ''' cast matrix object to the R liked dataframe object
    ''' </summary>
    ''' <param name="x">the matrix object that going to do the type casting</param>
    ''' <returns></returns>
    ''' <example>
    ''' let matrix = read.mz_matrix("/file.dat");
    ''' # cast object to dataframe liked object
    ''' let df = df.mz_matrix(matrix);
    ''' 
    ''' # get m/z features inside this expression matrix
    ''' print(colnames(df));
    ''' # get single cell labels inside this expression matrix
    ''' print(colnames(df));
    ''' 
    ''' # get expression of specific ion by m/z value
    ''' print(df[, 336.8995]);
    ''' 
    ''' # get expression of a specific singlecell by its label
    ''' print(df["cell_label1", ]);
    ''' </example>
    <ExportAPI("df.mz_matrix")>
    <RApiReturn(GetType(SpatialMatrixReader))>
    Public Function dfMzMatrix(x As MzMatrix) As Object
        Return New SpatialMatrixReader(x)
    End Function

    ''' <summary>
    ''' create a session for create spot cell embedding
    ''' </summary>
    ''' <param name="ndims">the embedding vector size, greater than 30 and less than 100 dimension is recommended.</param>
    ''' <param name="method"></param>
    ''' <param name="freq"></param>
    ''' <param name="diff">
    ''' the score diff for build the tree branch
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("cell_embedding")>
    <RApiReturn(GetType(SpecEmbedding))>
    Public Function cell_embedding(Optional ndims As Integer = 30,
                                   Optional method As TrainMethod = TrainMethod.Skip_Gram,
                                   Optional freq As Integer = 3,
                                   Optional diff As Double = 0.1) As Object

        Return New SpecEmbedding(ndims, method, freq, diff:=diff)
    End Function

    ''' <summary>
    ''' export the cell clustering result
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <returns>a tuple list of the cell clustering result,
    ''' each tuple is a cluster result.</returns>
    <ExportAPI("cell_clusters")>
    Public Function cell_clusters(pool As SpecEmbedding) As list
        Dim tree = pool.GetClusters
        Dim clusters As New list With {
            .slots = tree _
                .ToDictionary(Function(t) t.Key,
                              Function(t)
                                  Return CObj(t.Value)
                              End Function)
        }

        Return clusters
    End Function

    ''' <summary>
    ''' push a sample data into the embedding session
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <param name="sample"></param>
    ''' <param name="tag"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the spectrum data will be re-order via the spectrum total ions desc
    ''' </remarks>
    <ExportAPI("embedding_sample")>
    <RApiReturn(GetType(SpecEmbedding))>
    Public Function embedding_sample(pool As SpecEmbedding, <RRawVectorArgument> sample As Object,
                                     Optional tag As String = Nothing,
                                     Optional vocabulary As SpectrumVocabulary = Nothing,
                                     Optional env As Environment = Nothing) As Object
        Dim pull As PeakMs2()

        If sample Is Nothing Then
            Return pool
        End If

        If TypeOf sample Is mzPack Then
            Dim mzpack As mzPack = sample

            tag = If(tag, mzpack.source)
            pull = mzpack.MS _
                .Select(Function(s)
                            Return New PeakMs2(tag & " - " & s.scan_id, s.GetMs)
                        End Function) _
                .ToArray
        ElseIf TypeOf sample Is MzMatrix Then
            pull = DirectCast(sample, MzMatrix) _
                .GetPeaks(tag) _
                .ToArray
        Else
            Dim pip As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(sample, env)

            If pip.isError Then
                Return pip.getError
            Else
                pull = pip.populates(Of PeakMs2)(env).ToArray
            End If
        End If

        ' re-order the spectrum data via total sum data
        pull = pull _
            .OrderByDescending(Function(s)
                                   Return Aggregate mzi As ms2
                                          In s.mzInto
                                          Into Sum(mzi.intensity)
                               End Function) _
            .ToArray

        If vocabulary Is Nothing Then
            Call pool.AddSample(pull, centroid:=True)
        Else
            Dim terms As String() = pull _
                .Select(Function(spec)
                            Return vocabulary.ToTerm(spec.lib_guid)
                        End Function) _
                .ToArray

            Call pool.AddSample(terms)
        End If

        Return pool
    End Function

    ''' <summary>
    ''' get the cell spot embedding result
    ''' </summary>
    ''' <param name="pool"></param>
    ''' <returns>vector data could be converts the dataframe object via ``as.data.frame``
    ''' </returns>
    ''' <example>
    ''' as.data.frame(spot_vector(x));
    ''' </example>
    <ExportAPI("spot_vector")>
    Public Function spot_vector(pool As SpecEmbedding) As VectorModel
        Return pool.CreateEmbedding
    End Function

    ''' <summary>
    ''' get the labels based on the spatial information of each spot
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    ''' <example>
    ''' let matrix = read.mz_matrix("/file.dat");
    ''' let xyz = spatial_labels(matrix);
    ''' let spatial = strsplit(xyz, ",");
    ''' 
    ''' # x
    ''' print(spatial@{1});
    ''' # y
    ''' print(spatial@{2});
    ''' # z
    ''' print(spatial@{3});
    ''' </example>
    <ExportAPI("spatial_labels")>
    Public Function spatialLabels(x As MzMatrix) As String()
        Return x.matrix.Select(Function(s) $"{s.X},{s.Y},{s.Z}").ToArray
    End Function
End Module
