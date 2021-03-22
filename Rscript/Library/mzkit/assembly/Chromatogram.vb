#Region "Microsoft.VisualBasic::44893f2354315c43ffd6edbff0889a4a, Rscript\Library\mzkit\assembly\Chromatogram.vb"

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

    ' Module ChromatogramTools
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: addOverlaps, overlaps, overlapsSummary, ReadData, scaleScanTime
    '               setLabels, subset
    ' 
    '     Sub: PackData
    ' 
    ' /********************************************************************************/

#End Region


Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("chromatogram")>
<RTypeExport("overlaps", GetType(ChromatogramOverlap))>
Module ChromatogramTools

    Sub New()
        Call ConsolePrinter.AttachConsoleFormatter(Of ChromatogramOverlap)(AddressOf overlapsSummary)
    End Sub

    Private Function overlapsSummary(data As ChromatogramOverlap) As String
        Dim text As New StringBuilder

        Call text.AppendLine($"Chromatogram Overlaps Of {data.length} files:")

        For Each file As String In data.overlaps.Keys
            Call text.AppendLine($"  {file} {data(file).scan_time.Length} scans")
        Next

        Return text.ToString
    End Function

    <ExportAPI("add")>
    Public Function addOverlaps(overlaps As ChromatogramOverlap, name$, data As Chromatogram) As ChromatogramOverlap
        Call overlaps.overlaps.Add(name, data)
        Return overlaps
    End Function

    <ExportAPI("subset")>
    Public Function subset(overlaps As ChromatogramOverlap, names As String()) As ChromatogramOverlap
        Return overlaps(names)
    End Function

    <ExportAPI("labels")>
    Public Function setLabels(overlaps As ChromatogramOverlap, names As String(), Optional env As Environment = Nothing) As ChromatogramOverlap
        overlaps.setNames(names, env)
        Return overlaps
    End Function

    <ExportAPI("scale_time")>
    Public Function scaleScanTime(overlaps As ChromatogramOverlap, Optional unit As String = "minute") As ChromatogramOverlap
        If LCase(unit) = "minute" Then
            Return New ChromatogramOverlap With {
                .overlaps = overlaps.overlaps _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return New Chromatogram With {
                                        .BPC = a.Value.BPC,
                                        .TIC = a.Value.TIC,
                                        .scan_time = a.Value.scan_time.AsVector / 60
                                      }
                                  End Function)
            }
        ElseIf LCase(unit) = "hour" Then
            Return New ChromatogramOverlap With {
                .overlaps = overlaps.overlaps _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return New Chromatogram With {
                                        .BPC = a.Value.BPC,
                                        .TIC = a.Value.TIC,
                                        .scan_time = a.Value.scan_time.AsVector / 60 / 60
                                      }
                                  End Function)
            }
        Else
            Return overlaps
        End If
    End Function

    <ExportAPI("overlaps")>
    <RApiReturn(GetType(ChromatogramOverlap))>
    Public Function overlaps(<RRawVectorArgument> Optional TIC As Object = Nothing, Optional env As Environment = Nothing) As Object
        If TIC Is Nothing Then
            Return New ChromatogramOverlap
        End If

        If TypeOf TIC Is ChromatogramOverlap Then
            Return TIC
        End If

        If TypeOf TIC Is list Then
            Dim result As New ChromatogramOverlap

            For Each item In DirectCast(TIC, list).namedValues
                If Not TypeOf item.Value Is Chromatogram Then
                    Return Message.InCompatibleType(GetType(Chromatogram), item.Value.GetType, env, $"item '{item.Name}' is not a chromatogram value.")
                Else
                    result(item.Name) = item.Value
                End If
            Next

            Return result
        Else
            Dim overlapsData As pipeline = pipeline.TryCreatePipeline(Of Chromatogram)(TIC, env)
            Dim result As New ChromatogramOverlap

            If overlapsData.isError Then
                Return overlapsData.getError
            Else
                For Each item As SeqValue(Of Chromatogram) In overlapsData.populates(Of Chromatogram)(env).SeqIterator
                    result(item.i) = item
                Next
            End If

            Return result
        End If
    End Function

    <ExportAPI("write.pack")>
    Public Sub PackData(overlaps As ChromatogramOverlap, cdf As String)
        Using file As Stream = cdf.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call overlaps.SavePackData(file)
        End Using
    End Sub

    <ExportAPI("read.pack")>
    Public Function ReadData(cdf As String) As ChromatogramOverlap
        Using file As Stream = cdf.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return file.ReadPackData
        End Using
    End Function
End Module

