Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class D2Chromatogram

    Public Property scan_time As Double
    Public Property intensity As Double
    Public Property d2chromatogram As ChromatogramTick()

    Public Overrides Function ToString() As String
        Return $"{intensity.ToString("G3")}@{scan_time.ToString("F2")}"
    End Function

    Public Shared Function EncodeCDF(gcxgc As IEnumerable(Of D2Chromatogram), file As Stream) As Boolean
        Using writer As New CDFWriter(file)
            Dim i As i32 = 1
            Dim size As New Dictionary(Of String, Dimension)
            Dim vector As Double()
            Dim dims As Dimension
            Dim attrs As attribute()

            For Each scan As D2Chromatogram In gcxgc
                vector = scan.d2chromatogram _
                    .Select(Function(d) d.Time) _
                    .JoinIterates(scan.d2chromatogram.Select(Function(d) d.Intensity)) _
                    .ToArray
                dims = size.ComputeIfAbsent(vector.Length.ToString, Function() New Dimension With {.name = $"sizeof_{vector.Length}", .size = vector.Length})
                attrs = {
                    New attribute With {.name = "scan_time", .type = CDFDataTypes.DOUBLE, .value = scan.scan_time},
                    New attribute With {.name = "intensity", .type = CDFDataTypes.DOUBLE, .value = scan.intensity}
                }
                writer.AddVector($"[{++i}]{scan}", vector, dims, attrs)
            Next

            attrs = {New attribute With {.name = "nscans", .value = i - 1, .type = CDFDataTypes.INT}}
            writer.GlobalAttributes(attrs)
        End Using

        Return True
    End Function

    Public Shared Iterator Function DecodeCDF(file As Stream) As IEnumerable(Of D2Chromatogram)
        Using reader As New netCDFReader(file)
            Dim nscans As Integer = reader.getAttribute("nscans")
            Dim names As variable() = reader.variables

            For i As Integer = 0 To nscans - 1
                Dim vec As doubles = reader.getDataVariable(names(i))
                Dim time As Double() = vec(0, vec.Length / 2 - 1)
                Dim into As Double() = vec(vec.Length / 2 - 1, vec.Length - 1)
                Dim ticks As ChromatogramTick() = time _
                    .Select(Function(t, j)
                                Return New ChromatogramTick With {.Time = t, .Intensity = into(j)}
                            End Function) _
                    .ToArray
                Dim scan_time As Double = names(i).FindAttribute("scan_time").getObjectValue
                Dim intensity As Double = names(i).FindAttribute("intensity").getObjectValue

                Yield New D2Chromatogram With {
                    .d2chromatogram = ticks,
                    .intensity = intensity,
                    .scan_time = scan_time
                }
            Next
        End Using
    End Function

End Class
