Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components

''' <summary>
''' GCxGC assembly data
''' </summary>
Public Module GC2Dimensional

    <Extension>
    Public Function ToMzPack(agilentGC As netCDFReader) As mzPack
        Dim scan_numbers As Dimension = agilentGC.dimensions.KeyItem("scan_number")
        Dim blockSize As Integer = agilentGC.recordDimension.length / scan_numbers.size
        Dim scan_time As doubles = agilentGC.getDataVariable("scan_acquisition_time")
        Dim totalIons As doubles = agilentGC.getDataVariable("total_intensity")
        Dim mz As shorts = agilentGC.getDataVariable("mass_values")
        Dim into As shorts = agilentGC.getDataVariable("intensity_values")

        Return New mzPack With {
            .MS = blockSize.CreateMSScans(scan_time, totalIons, mz, into).ToArray
        }
    End Function

    <Extension>
    Private Iterator Function CreateMSScans(blockSize As Integer, scan_time As Double(), totalIons As Double(), mz As Short(), into As Short()) As IEnumerable(Of ScanMS1)
        Dim mzMatrix As Double()() = mz.Select(Function(i) CDbl(i)).Split(blockSize).ToArray
        Dim intoMatrix As Double()() = into.Select(Function(i) CDbl(i)).Split(blockSize).ToArray

        For i As Integer = 0 To scan_time.Length - 1
            Yield New ScanMS1 With {
                .TIC = totalIons(i),
                .BPC = .TIC,
                .rt = scan_time(i),
                .scan_id = i + 1,
                .mz = mzMatrix(i),
                .into = intoMatrix(i)
            }
        Next
    End Function
End Module
