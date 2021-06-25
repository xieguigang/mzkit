
Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports imzML = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML.XML

<Package("MSI")>
Module MSI

    <ExportAPI("open.imzML")>
    Public Function open_imzML(file As String) As Object
        Dim scans As ScanData() = imzML.LoadScans(file:=file).ToArray
        Dim ibd = ibdReader.Open(file.ChangeSuffix("ibd"))

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"scans", scans},
                {"ibd", ibd}
            }
        }
    End Function

    ''' <summary>
    ''' each raw data file is a row scan data
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("row.scans")>
    Public Function rowScans(raw As String, y As Integer, Optional correction As Correction = Nothing, Optional env As Environment = Nothing) As Object
        Using file As FileStream = raw.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim mzpack As mzPack = mzPack.ReadAll(file, ignoreThumbnail:=True)
            Dim pixels As iPixelIntensity() = mzpack.MS _
                .Select(Function(col, i)
                            Return New iPixelIntensity With {
                                .average = col.into.Average,
                                .basePeakIntensity = col.into.Max,
                                .totalIon = col.into.Sum,
                                .x = If(correction Is Nothing, i + 1, correction.GetPixel(col.rt)),
                                .y = y
                            }
                        End Function) _
                .ToArray

            Return pixels
        End Using
    End Function

    <ExportAPI("Correction")>
    Public Function Correction(totalTime As Double, pixels As Integer) As Correction
        Return New Correction(totalTime, pixels)
    End Function

    <ExportAPI("scanMatrix")>
    Public Function MSIScanMatrix(<RRawVectorArgument> rowScans As Object, Optional env As Environment = Nothing) As Object
        Dim data As pipeline = pipeline.TryCreatePipeline(Of iPixelIntensity)(rowScans, env)

        If data.isError Then
            Return data.getError
        End If

        Dim rows = data _
            .populates(Of iPixelIntensity)(env) _
            .GroupBy(Function(p) p.y) _
            .Select(Function(r) r.ToArray) _
            .ToArray
        Dim width As Integer = rows.Select(Function(p) p.Select(Function(pi) pi.x).Max).Max
        Dim height As Integer = rows.Select(Function(p) p.Select(Function(pi) pi.y).Max).Max

        Return New MSISummary With {
            .rowScans = rows,
            .size = New Size(width, height)
        }
    End Function
End Module

Public Class Correction

    Public ReadOnly Property totalTime As Double
    Public ReadOnly Property pixels As Integer
    Public ReadOnly Property pixelsTime As Double

    Sub New(totalTime As Double, pixels As Integer)
        Me.totalTime = totalTime
        Me.pixels = pixels
        Me.pixelsTime = totalTime / pixels
    End Sub

    Public Function GetPixel(rt As Double) As Integer
        Return 1 + CInt(rt / pixelsTime)
    End Function

End Class