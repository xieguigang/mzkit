Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("TissueMorphology")>
Module TissueMorphology

    <ExportAPI("UMAPsample")>
    Public Function createUMAPsample(<RRawVectorArgument>
                                     points As Object,
                                     x As Double(),
                                     y As Double(),
                                     z As Double(),
                                     cluster As Integer(),
                                     Optional env As Environment = Nothing) As UMAPPoint()

        Dim pixels As String() = REnv.asVector(Of String)(points)
        Dim umap As UMAPPoint() = pixels _
            .Select(Function(pi, i)
                        Dim xy As Integer() = pi.Split(","c).Select(AddressOf Integer.Parse).ToArray
                        Dim sample As New UMAPPoint With {
                            .[class] = cluster(i),
                            .Pixel = New Point(xy(0), xy(1)),
                            .x = x(i),
                            .y = y(i),
                            .z = z(i)
                        }

                        Return sample
                    End Function) _
            .ToArray

        Return umap
    End Function

    <ExportAPI("TissueData")>
    Public Function createTissueData(x As Integer(), y As Integer(), labels As String(), Optional colorSet As String = "Paper") As TissueRegion()
        Dim labelClass As String() = labels.Distinct.ToArray
        Dim colors As Color() = Designer.GetColors(colorSet, labelClass.Length)
        Dim regions As New Dictionary(Of String, List(Of Point))

        For Each label As String In labelClass
            Call regions.Add(label, New List(Of Point))
        Next

        For i As Integer = 0 To labels.Length - 1
            Call regions(labels(i)).Add(New Point(x(i), y(i)))
        Next

        Return regions _
            .Select(Function(r, i)
                        Return New TissueRegion With {
                            .color = colors(i),
                            .label = r.Key,
                            .points = r.Value.ToArray
                        }
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("writeCDF")>
    Public Function createCDF(tissueMorphology As TissueRegion(), umap As UMAPPoint(),
                              file As Object,
                              Optional env As Environment = Nothing) As Object

        Dim saveBuf = SMRUCC.Rsharp.GetFileStream(file, IO.FileAccess.Write, env)

        If saveBuf Like GetType(Message) Then
            Return saveBuf.TryCast(Of Message)
        End If

        Using buffer As Stream = saveBuf.TryCast(Of Stream)
            Return tissueMorphology.WriteCDF(umap, file:=buffer)
        End Using
    End Function
End Module
