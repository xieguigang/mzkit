#Region "Microsoft.VisualBasic::ac6aa85fc177cc71f7dd7920207b06f4, assembly\Comprehensive\GCxGC\D2Chromatogram.vb"

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

    '   Total Lines: 84
    '    Code Lines: 59 (70.24%)
    ' Comment Lines: 17 (20.24%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (9.52%)
    '     File Size: 3.81 KB


    ' Module D2Chromatogram
    ' 
    '     Function: DecodeCDF, EncodeCDF
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.GCxGC
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.DataStorage.netCDF.Data
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' A data model for GCxGC 2d chromatogram
''' </summary>
''' <remarks>
''' is a collection of the <see cref="ChromatogramTick"/> data.
''' </remarks>
Public Module D2Chromatogram

    ''' <summary>
    ''' Export GCxGC data in mzkit cdf format
    ''' </summary>
    ''' <param name="gcxgc"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Function EncodeCDF(gcxgc As IEnumerable(Of Chromatogram2DScan), file As Stream) As Boolean
        Using writer As New CDFWriter(file)
            Dim i As i32 = 1
            Dim size As New Dictionary(Of String, Dimension)
            Dim vector As Double()
            Dim dims As Dimension
            Dim attrs As attribute()

            For Each scan As Chromatogram2DScan In gcxgc
                vector = scan.chromatogram _
                    .Select(Function(d) d.Time) _
                    .JoinIterates(scan.chromatogram.Select(Function(d) d.Intensity)) _
                    .ToArray
                dims = size.ComputeIfAbsent(vector.Length.ToString, Function() New Dimension With {.name = $"sizeof_{vector.Length}", .size = vector.Length})
                attrs = {
                    New attribute With {.name = "scan_time", .type = CDFDataTypes.NC_DOUBLE, .value = scan.scan_time},
                    New attribute With {.name = "intensity", .type = CDFDataTypes.NC_DOUBLE, .value = scan.intensity}
                }
                writer.AddVector($"[{++i}]{scan}", vector, dims, attrs)
            Next

            attrs = {
                New attribute With {.name = "nscans", .value = i - 1, .type = CDFDataTypes.NC_INT},
                New attribute With {.name = "classid", .value = FileApplicationClass.GCxGC.Description, .type = CDFDataTypes.NC_CHAR}
            }
            writer.GlobalAttributes(attrs)
        End Using

        Return True
    End Function

    ''' <summary>
    ''' Processing the cdf file export result which is produced via the <see cref="EncodeCDF"/> function
    ''' </summary>
    ''' <param name="file">the cdf file in mzkit format</param>
    ''' <returns></returns>
    Public Iterator Function DecodeCDF(file As Stream) As IEnumerable(Of Chromatogram2DScan)
        Using reader As New netCDFReader(file)
            Dim nscans As Integer = reader("nscans")
            Dim names As variable() = reader.variables

            For i As Integer = 0 To nscans - 1
                Dim vec As doubles = reader.getDataVariable(names(i))
                Dim time As Double() = vec(0, vec.Length / 2)
                Dim into As Double() = vec(vec.Length / 2, vec.Length)
                Dim ticks As ChromatogramTick() = ChromatogramTick.Zip(time, into).ToArray
                Dim scan_time As Double = names(i).FindAttribute("scan_time").getObjectValue
                Dim intensity As Double = names(i).FindAttribute("intensity").getObjectValue

                Yield New Chromatogram2DScan With {
                    .chromatogram = ticks,
                    .intensity = intensity,
                    .scan_time = scan_time
                }
            Next
        End Using
    End Function
End Module
