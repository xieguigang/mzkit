#Region "Microsoft.VisualBasic::2aaf4a362c848f1a8f55f9159784352e, mzkit\Rscript\Library\mzkit\comprehensive\SingleCells.vb"

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

'   Total Lines: 92
'    Code Lines: 63
' Comment Lines: 18
'   Blank Lines: 11
'     File Size: 3.94 KB


' Module SingleCells
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: cellMatrix, cellStatsTable, singleCellsIons
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.[Object].dataframe
Imports SingleCellMath = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.Math
Imports SingleCellMatrix = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.PeakMatrix

''' <summary>
''' Single cells metabolomics data processor
''' </summary>
''' 
<Package("SingleCells")>
Module SingleCells

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(SingleCellIonStat()), AddressOf cellStatsTable)
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(MzMatrix), AddressOf mzMatrixDf)
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(SpatialMatrixReader), AddressOf mzMatrixDf)
    End Sub

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
    ''' <param name="x"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' implements the ``as.data.frame`` function
    ''' </remarks>
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
    ''' <returns></returns>
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
    ''' export single cell expression matrix from the raw data scans
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("cell_matrix")>
    Public Function cellMatrix(raw As mzPack,
                               Optional mzdiff As Double = 0.005,
                               Optional freq As Double = 0.001,
                               Optional env As Environment = Nothing) As Object

        Dim singleCells As New List(Of DataFrameRow)
        Dim mzSet As Double() = SingleCellMath.GetMzIndex(raw:=raw, mzdiff:=mzdiff, freq:=freq)

        For Each cell_scan As DataFrameRow In SingleCellMatrix.ExportScans(Of DataFrameRow)(raw, mzSet)
            cell_scan.geneID = cell_scan.geneID _
                .Replace("[MS1]", "") _
                .Trim
            singleCells.Add(cell_scan)
        Next

        Return New HTSMatrix With {
            .expression = singleCells.ToArray,
            .sampleID = mzSet _
                .Select(Function(mzi) mzi.ToString("F4")) _
                .ToArray,
            .tag = raw.source
        }
    End Function

    ''' <summary>
    ''' do stats of the single cell metabolomics ions
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="da"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    <ExportAPI("SCM_ionStat")>
    <RApiReturn(GetType(SingleCellIonStat))>
    Public Function singleCellsIons(raw As mzPack,
                                    Optional da As Double = 0.01,
                                    Optional parallel As Boolean = True) As Object

        Return SingleCellIonStat.DoIonStats(raw, da, parallel).ToArray
    End Function

    ''' <summary>
    ''' write the single cell ion feature data matrix
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
    ''' <returns></returns>
    ''' <remarks>
    ''' this function open a lazy reader of the matrix, for load all 
    ''' data into memory at once, use the ``read.mz_matrix`` 
    ''' function.
    ''' </remarks>
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
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("df.mz_matrix")>
    Public Function dfMzMatrix(x As MzMatrix) As Object
        Return New SpatialMatrixReader(x)
    End Function
End Module
