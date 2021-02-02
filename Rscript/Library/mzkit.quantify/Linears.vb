#Region "Microsoft.VisualBasic::c45967316f2cb5466570addb030eb7e1, Library\mzkit.quantify\Linears.vb"

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

' Module Linears
' 
'     Function: GetLinearPoints, printIS, printLineModel, printStandards, StandardCurveDataSet
'               writeMRMpeaktable, writeStandardCurve
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports REnv = SMRUCC.Rsharp.Runtime
Imports Rlist = SMRUCC.Rsharp.Runtime.Internal.Object.list
Imports stdNum = System.Math

<Package("Linears")>
Module Linears

    Sub Main()
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of StandardCurve)(AddressOf printLineModel)
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of Standards())(AddressOf printStandards)
        REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of [IS]())(AddressOf printIS)
    End Sub

    Private Function printLineModel(line As Object) As String
        If line Is Nothing Then
            Return "NULL"
        Else
            With DirectCast(line, StandardCurve)
                Return $"{ .name}: { .linear.ToString}"
            End With
        End If
    End Function

    Private Function printStandards(obj As Object) As String
        Dim csv = DirectCast(obj, Standards()).ToCsvDoc.ToMatrix.RowIterator.ToArray
        Dim printContent = csv.Print(addBorder:=False)

        Return printContent
    End Function

    Private Function printIS(obj As Object) As String
        Dim csv = DirectCast(obj, [IS]()).ToCsvDoc.ToMatrix.RowIterator.ToArray
        Dim printContent = csv.Print(addBorder:=False)

        Return printContent
    End Function

    <ExportAPI("lines.table")>
    Public Function StandardCurveDataSet(lines As StandardCurve()) As EntityObject()
        Return lines _
            .Select(Function(line)
                        Return New EntityObject With {
                            .ID = line.name,
                            .Properties = New Dictionary(Of String, String) From {
                                {"name", line.points(Scan0).Name},
                                {"equation", "f(x)=" & line.linear.Polynomial.ToString("G5", False)},
                                {"R2", stdNum.Sqrt(line.linear.R2)},
                                {"is.weighted", line.isWeighted},
                                {"IS.calibration", line.requireISCalibration},
                                {"IS", line.IS.name}
                            }
                        }
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("write.points")>
    Public Function writeStandardCurve(points As ReferencePoint(), file$) As Boolean
        Return points.SaveTo(file, silent:=True)
    End Function

    ''' <summary>
    ''' Get reference input points
    ''' </summary>
    ''' <param name="linears"></param>
    ''' <param name="name">The metabolite id</param>
    ''' <returns></returns>
    <ExportAPI("points")>
    Public Function GetLinearPoints(linears As StandardCurve(), name$) As ReferencePoint()
        Dim line As StandardCurve = linears _
            .Where(Function(l)
                       Return l.name = name
                   End Function) _
            .FirstOrDefault

        If line Is Nothing Then
            Return Nothing
        Else
            Return line.points
        End If
    End Function

    ''' <summary>
    ''' Write peak data which is extract from the raw file with 
    ''' the given ion pairs or quantify ion data
    ''' </summary>
    ''' <param name="ionPeaks"></param>
    ''' <param name="file">The output csv file path</param>
    ''' <returns></returns>
    <ExportAPI("write.ionPeaks")>
    Public Function writeMRMpeaktable(ionPeaks As IonPeakTableRow(), file$) As Boolean
        Return ionPeaks.SaveTo(file, silent:=True)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="models"></param>
    ''' <param name="ions"></param>
    ''' <returns></returns>
    <ExportAPI("quantify")>
    Public Function SampleQuantify(models As StandardCurve(),
                                   ions As TargetPeakPoint(),
                                   Optional names As Rlist = Nothing,
                                   Optional baselineQuantile As Double = 0.6,
                                   Optional fileName As String = "NA",
                                   Optional env As Environment = Nothing) As QuantifyScan

        Dim nameIndex As Dictionary(Of String, String)

        If names Is Nothing Then
            nameIndex = models.ToDictionary(Function(ion) ion.name, Function(ion) ion.name)
        Else
            nameIndex = names.AsGeneric(Of String)(env)
        End If

        Return models.SampleQuantify(ions, nameIndex, baselineQuantile, fileName)
    End Function
End Module
