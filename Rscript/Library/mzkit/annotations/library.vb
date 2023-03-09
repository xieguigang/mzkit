#Region "Microsoft.VisualBasic::f33f9eae48f381a95f137203ec79dd95, mzkit\Rscript\Library\mzkit\annotations\library.vb"

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

'   Total Lines: 235
'    Code Lines: 192
' Comment Lines: 5
'   Blank Lines: 38
'     File Size: 8.72 KB


' Module library
' 
'     Function: AddReference, asSpectrum, createAnnotation, createLibraryIO, createMetabolite
'               createPrecursorIons, PopulateIonData, queryByMz, SaveResult
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.MSEngine.AnnotationLibrary
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting

<Package("annotation")>
Module library

    <ExportAPI("saveLibrary")>
    Public Function SaveResult(<RRawVectorArgument>
                               references As Object,
                               file As Object,
                               Optional env As Environment = Nothing) As Object

        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        Dim data As pipeline = pipeline.TryCreatePipeline(Of Metabolite)(references, env)

        If data.isError Then
            Return data.getError
        End If

        Using package As New Writer(buffer.TryCast(Of Stream))
            For Each met As Metabolite In data.populates(Of Metabolite)(env)
                Call package.AddReference(met)
            Next
        End Using

        Return True
    End Function

    <ExportAPI("populateIonData")>
    <Extension>
    <RApiReturn(GetType(PeakMs2))>
    Public Function PopulateIonData(raw As mzPack, mzdiff As Object, Optional env As Environment = Nothing) As Object
        Dim tolerance = Math.getTolerance(mzdiff, env)

        If tolerance Like GetType(Message) Then
            Return tolerance.TryCast(Of Message)
        End If

        Dim mzErr As Tolerance = tolerance.TryCast(Of Tolerance)
        Dim ions As New List(Of PeakMs2)

        For Each ms1 In raw.MS
            For Each ms2 In ms1.products.SafeQuery
                For Each mzi As Double In ms1.mz
                    If mzErr(mzi, ms2.parentMz) Then
                        Dim ion2 As New PeakMs2 With {
                            .mz = mzi,
                            .rt = ms1.rt,
                            .file = raw.source,
                            .lib_guid = ms2.scan_id,
                            .activation = ms2.activationMethod.Description,
                            .collisionEnergy = ms2.collisionEnergy,
                            .intensity = ms2.intensity,
                            .scan = ms2.scan_id,
                            .mzInto = ms2.GetMs.ToArray
                        }

                        Call ions.Add(ion2)
                    End If
                Next
            Next
        Next

        Return ions.ToArray
    End Function

    <ExportAPI("openLibrary")>
    Public Function createLibraryIO(file As Object,
                                    Optional read As Boolean = True,
                                    Optional massDiff As Double = 0.05,
                                    Optional env As Environment = Nothing) As Object

        Dim mode As IO.FileAccess = If(read, IO.FileAccess.Read, IO.FileAccess.Write)
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, mode, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        If read Then
            Return New Reader(buffer.TryCast(Of Stream), massDiff)
        Else
            Return New Writer(buffer.TryCast(Of Stream))
        End If
    End Function

    <ExportAPI("queryByMz")>
    <RApiReturn(GetType(Metabolite))>
    Public Function queryByMz([lib] As Reader, mz As Double,
                              Optional tolerance As Object = Nothing,
                              Optional env As Environment = Nothing) As Object

        Dim mzdiff = Math.getTolerance(tolerance, env)

        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If

        Return [lib].QueryByMz(mz, mzdiff.TryCast(Of Tolerance)).ToArray
    End Function

    <ExportAPI("addReference")>
    Public Function AddReference(library As Writer, ref As Metabolite) As Writer
        Call library.AddReference(ref)
        Return library
    End Function

    <ExportAPI("annotation")>
    Public Function createAnnotation(id As String,
                                     formula As String,
                                     name As String,
                                     Optional synonym As String() = Nothing,
                                     Optional xref As xref = Nothing) As MetaInfo

        Return New MetaInfo With {
            .xref = If(xref, New xref),
            .formula = formula,
            .ID = id,
            .name = name,
            .synonym = synonym,
            .exact_mass = CDbl(FormulaScanner.ScanFormula(formula))
        }
    End Function

    <ExportAPI("metabolite")>
    Public Function createMetabolite(anno As MetaInfo,
                                     precursors As PrecursorData(),
                                     spectrums As Spectrum()) As Metabolite

        Return New Metabolite With {
            .annotation = anno,
            .precursors = precursors,
            .spectrums = spectrums
        }
    End Function

    ''' <summary>
    ''' create precursor ion library dataset
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <ExportAPI("precursorIons")>
    Public Function createPrecursorIons(data As dataframe) As PrecursorData()
        Dim mz As Double() = CLRVector.asNumeric(data("mz"))
        Dim rt As Double() = CLRVector.asNumeric(data("rt"))
        Dim charge As Integer() = CLRVector.asInteger(data("charge"))
        Dim ion As String() = CLRVector.asCharacter(data("ion"))

        Return mz _
            .Select(Function(mzi, i)
                        Return New PrecursorData With {
                            .mz = mzi,
                            .rt = New Double() {rt(i)},
                            .charge = charge(i),
                            .ion = ion(i)
                        }
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("asSpectrum")>
    Public Function asSpectrum(data As Object,
                               Optional ionMode As Object = Nothing,
                               Optional guid As String = Nothing,
                               Optional env As Environment = Nothing) As Spectrum

        Dim mz As Double()
        Dim intensity As Double()
        Dim annotations As String()

        If TypeOf data Is dataframe Then
            Dim df As dataframe = DirectCast(data, dataframe)

            mz = CLRVector.asNumeric(df("mz"))
            intensity = CLRVector.asNumeric(df("intensity"))
            annotations = CLRVector.asCharacter(df("annotation"))
            ionMode = ParseIonMode(any.ToString(ionMode))
        ElseIf TypeOf data Is LibraryMatrix Then
            Dim mat As LibraryMatrix = DirectCast(data, LibraryMatrix)

            mz = mat.Select(Function(i) i.mz).ToArray
            intensity = mat.Select(Function(i) i.intensity).ToArray
            annotations = mat.Select(Function(i) i.Annotation).ToArray
            guid = If(guid, mat.name)
        ElseIf TypeOf data Is PeakMs2 Then
            Dim peak As PeakMs2 = data

            mz = peak.mzInto.Select(Function(i) i.mz).ToArray
            intensity = peak.mzInto.Select(Function(i) i.intensity).ToArray
            annotations = peak.mzInto.Select(Function(i) i.Annotation).ToArray
            ionMode = ParseIonMode(Parser.ParseMzCalculator(peak.precursor_type).mode)
            guid = If(guid, peak.lib_guid)
        Else
            Throw New NotImplementedException
        End If

        If guid.StringEmpty Then
            guid = mz _
                .JoinIterates(intensity) _
                .Select(Function(d) d.ToString("G6")) _
.DoCall(AddressOf FNV1a.GetHashCode)
        End If

        Return New Spectrum With {
            .guid = guid,
            .annotations = annotations,
            .intensity = intensity,
            .ionMode = ionMode,
            .mz = mz
        }
    End Function
End Module
