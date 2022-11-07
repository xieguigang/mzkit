Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Public Module GCMSConvertor

    Public Function ConvertGCMS(agilentGC As netCDFReader, Optional println As Action(Of String) = Nothing) As mzPack
        If println Is Nothing Then
            println = AddressOf Console.WriteLine
        End If

        Return New mzPack With {
           .Application = FileApplicationClass.GCMS,
           .MS = LoadMs1Scans(agilentGC, println).ToArray
        }
    End Function

    Public Function LoadMs1Scans(agilentGC As netCDFReader, println As Action(Of String)) As IEnumerable(Of ScanMS1)
        Dim scan_time As doubles = agilentGC.getDataVariable("scan_acquisition_time")
        Dim totalIons As doubles = agilentGC.getDataVariable("total_intensity")
        Dim point_count As integers = agilentGC.getDataVariable("point_count")
        Dim mz As Double()() = agilentGC.readMzMatrix(point_count, println).ToArray
        Dim into As Double()() = agilentGC.readIntoMatrix(point_count, println).ToArray

        Call println("read scan matrix!")

        Return scan_time.Array _
            .CreateMSScans(totalIons, mz, into) _
            .OrderBy(Function(t) t.rt)
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

    Const intensity_values As String = "intensity_values"

    <Extension>
    Private Iterator Function readIntoMatrix(agilentGC As netCDFReader, point_count As integers, println As Action(Of String)) As IEnumerable(Of Double())
        Dim into As ICDFDataVector = Nothing
        Dim offset As Integer = Scan0
        Dim type As CDFDataTypes = agilentGC.getDataVariableEntry(intensity_values).type

        Call println("read intensity matrix...")
        Call agilentGC.getDataVariable("intensity_values", into)

        If type = CDFDataTypes.INT Then
            Dim ints As integers = DirectCast(into, integers)

            For Each width As Integer In point_count
                Yield ints _
                    .Copy(offset, width) _
                    .Select(Function(i) CDbl(i)) _
                    .ToArray

                offset += width
            Next
        ElseIf type = CDFDataTypes.FLOAT Then
            Dim singles As floats = DirectCast(into, floats)

            For Each width As Integer In point_count
                Yield singles _
                    .Copy(offset, width) _
                    .Select(Function(i) CDbl(i)) _
                    .ToArray

                offset += width
            Next
        Else
            Throw New NotImplementedException(type.Description)
        End If
    End Function

    <Extension>
    Private Iterator Function readMzMatrix(agilentGC As netCDFReader, point_count As integers, println As Action(Of String)) As IEnumerable(Of Double())
        Dim offset As Integer = Scan0
        Dim mz As shorts = Nothing

        Call println("read m/z matrix...")
        Call agilentGC.getDataVariable("mass_values", mz)

        For Each width As Integer In point_count
            Yield mz _
                .Copy(offset, width) _
                .Select(Function(i) CDbl(i)) _
                .ToArray

            offset += width
        Next
    End Function
End Module
