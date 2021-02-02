
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
Imports REnv = SMRUCC.Rsharp.Runtime
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
End Module
