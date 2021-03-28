Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

''' <summary>
''' GCxGC assembly data
''' </summary>
Public Module GC2Dimensional

    <Extension>
    Public Function ToMzPack(agilentGC As netCDFReader) As mzPack
        Dim scan_time As doubles = agilentGC.getDataVariable("scan_acquisition_time")
        Dim totalIons As doubles = agilentGC.getDataVariable("total_intensity")
        Dim point_count As integers = agilentGC.getDataVariable("point_count")
        Dim mz As Double()() = agilentGC.readMzMatrix(point_count).ToArray
        Dim into As Double()() = agilentGC.readIntoMatrix(point_count).ToArray

        Return New mzPack With {
            .MS = scan_time.Array.CreateMSScans(totalIons, mz, into).ToArray
        }
    End Function

    <Extension>
    Private Iterator Function readMzMatrix(agilentGC As netCDFReader, point_count As integers) As IEnumerable(Of Double())
        Dim offset As Integer = Scan0
        Dim mz As shorts

        Call Console.WriteLine("read m/z matrix...")

        mz = agilentGC.getDataVariable("mass_values")

        For Each width As Integer In point_count
            Yield mz _
                .Copy(offset, width) _
                .Select(Function(i) CDbl(i)) _
                .ToArray

            offset += width
        Next
    End Function

    <Extension>
    Private Iterator Function readIntoMatrix(agilentGC As netCDFReader, point_count As integers) As IEnumerable(Of Double())
        Dim into As integers
        Dim offset As Integer = Scan0

        Call Console.WriteLine("read intensity matrix...")

        into = agilentGC.getDataVariable("intensity_values")

        For Each width As Integer In point_count
            Yield into _
                .Copy(offset, width) _
                .Select(Function(i) CDbl(i)) _
                .ToArray

            offset += width
        Next
    End Function

    <Extension>
    Private Iterator Function CreateMSScans(scan_time As Double(), totalIons As Double(), mz As Double()(), into As Double()()) As IEnumerable(Of ScanMS1)
        For i As Integer = 0 To scan_time.Length - 1
            Dim mzi As Double() = mz(i)
            Dim inti As Double() = into(i)
            Dim BPC As Double = inti.Max
            ' 20210328
            ' fix bugs fix mzkit_win32: required [MS1] prefix for indicate MS1
            Dim scan_id As String = $"[MS1] {i + 1}.scan_time={stdNum.Round(scan_time(i))}, m/z={mzi(which.Max(inti))}({BPC.ToString("G3")})"

            Yield New ScanMS1 With {
                .TIC = totalIons(i),
                .BPC = BPC,
                .rt = scan_time(i),
                .mz = mzi,
                .into = inti,
                .scan_id = scan_id
            }
        Next
    End Function
End Module
