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
        Dim mz As Double()() = agilentGC.readMzMatrix(blockSize)
        Dim into As Double()() = agilentGC.readIntoMatrix(blockSize)

        Return New mzPack With {
            .MS = scan_time.Array.CreateMSScans(totalIons, mz, into).ToArray
        }
    End Function

    <Extension>
    Private Function readMzMatrix(agilentGC As netCDFReader, blockSize As Integer) As Double()()
        Dim mz As shorts
        Dim matrix As Double()()

        Call Console.WriteLine("read m/z matrix...")

        mz = agilentGC.getDataVariable("mass_values")
        matrix = mz.Select(Function(i) CDbl(i)).Split(blockSize)

        Return matrix
    End Function

    <Extension>
    Private Function readIntoMatrix(agilentGC As netCDFReader, blockSize As Integer) As Double()()
        Dim into As integers
        Dim matrix As Double()()

        Call Console.WriteLine("read intensity matrix...")

        into = agilentGC.getDataVariable("intensity_values")
        matrix = into.Select(Function(i) CDbl(i)).Split(blockSize)

        Return matrix
    End Function

    <Extension>
    Private Iterator Function CreateMSScans(scan_time As Double(), totalIons As Double(), mz As Double()(), into As Double()()) As IEnumerable(Of ScanMS1)
        For i As Integer = 0 To scan_time.Length - 1
            Yield New ScanMS1 With {
                .TIC = totalIons(i),
                .BPC = .TIC,
                .rt = scan_time(i),
                .scan_id = i + 1,
                .mz = mz(i),
                .into = into(i)
            }
        Next
    End Function
End Module
