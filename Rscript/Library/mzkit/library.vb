
Imports System.IO
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
Imports any = Microsoft.VisualBasic.Scripting
Imports REnv = SMRUCC.Rsharp.Runtime

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

    <ExportAPI("openLibrary")>
    Public Function createLibraryIO(file As Object,
                                    Optional read As Boolean = True,
                                    Optional env As Environment = Nothing) As Object

        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Write, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        If read Then
            Return New Reader(buffer.TryCast(Of Stream))
        Else
            Return New Writer(buffer.TryCast(Of Stream))
        End If
    End Function

    <ExportAPI("queryByMz")>
    <RApiReturn(GetType(Metabolite))>
    Public Function queryByMz([lib] As Reader, mz As Double, Optional tolerance As Object = Nothing, Optional env As Environment = Nothing) As Object
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
            .exact_mass = FormulaScanner.ScanFormula(formula).ExactMass
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

    <ExportAPI("precursorIons")>
    Public Function createPrecursorIons(data As dataframe) As PrecursorData()
        Dim mz As Double() = REnv.asVector(Of Double)(data("mz"))
        Dim rt As Double() = REnv.asVector(Of Double)(data("rt"))
        Dim charge As Integer() = REnv.asVector(Of Integer)(data("charge"))
        Dim ion As String() = REnv.asVector(Of String)(data("ion"))

        Return mz _
            .Select(Function(mzi, i)
                        Return New PrecursorData With {
                            .mz = mzi,
                            .rt = rt(i),
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

            mz = REnv.asVector(Of Double)(df("mz"))
            intensity = REnv.asVector(Of Double)(df("intensity"))
            annotations = REnv.asVector(Of String)(df("annotation"))
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
