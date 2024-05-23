#Region "Microsoft.VisualBasic::73e6bb086fb8fcba659b578a485bb121, assembly\Comprehensive\GCxGC\D2Chromatogram.vb"

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

    '   Total Lines: 138
    '    Code Lines: 99 (71.74%)
    ' Comment Lines: 21 (15.22%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 18 (13.04%)
    '     File Size: 5.50 KB


    ' Class D2Chromatogram
    ' 
    '     Properties: chromatogram, intensity, scan_id, scan_time, size
    ' 
    '     Constructor: (+3 Overloads) Sub New
    '     Function: DecodeCDF, EncodeCDF, times, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
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
Public Class D2Chromatogram : Implements IReadOnlyId, INamedValue

    Public Property scan_time As Double
    Public Property intensity As Double
    Public Property scan_id As String Implements INamedValue.Key, IReadOnlyId.Identity

    ''' <summary>
    ''' chromatogram data 2d
    ''' </summary>
    ''' <returns></returns>
    Public Property chromatogram As ChromatogramTick()

    Default Public ReadOnly Property getTick(i As DoubleRange) As ChromatogramTick()
        Get
            Return chromatogram.Where(Function(a) i.IsInside(a.Time)).ToArray
        End Get
    End Property

    Default Public ReadOnly Property getTick(i As Integer) As ChromatogramTick
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _chromatogram(i)
        End Get
    End Property

    Public ReadOnly Property size As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return chromatogram.Length
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(t1 As Double)
        scan_time = t1
    End Sub

    Sub New(t1 As Double, id As String)
        scan_id = id
        scan_time = t1
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{intensity.ToString("G3")}@{scan_time.ToString("F2")}"
    End Function

    ''' <summary>
    ''' Export GCxGC data in mzkit cdf format
    ''' </summary>
    ''' <param name="gcxgc"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function EncodeCDF(gcxgc As IEnumerable(Of D2Chromatogram), file As Stream) As Boolean
        Using writer As New CDFWriter(file)
            Dim i As i32 = 1
            Dim size As New Dictionary(Of String, Dimension)
            Dim vector As Double()
            Dim dims As Dimension
            Dim attrs As attribute()

            For Each scan As D2Chromatogram In gcxgc
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
    Public Shared Iterator Function DecodeCDF(file As Stream) As IEnumerable(Of D2Chromatogram)
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

                Yield New D2Chromatogram With {
                    .chromatogram = ticks,
                    .intensity = intensity,
                    .scan_time = scan_time
                }
            Next
        End Using
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function times() As Double()
        Return chromatogram.Select(Function(t) t.Time).ToArray
    End Function
End Class
