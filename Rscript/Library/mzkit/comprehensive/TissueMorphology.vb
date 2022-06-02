Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("TissueMorphology")>
Module TissueMorphology

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(TissueRegion()), AddressOf createTissueTable)
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(UMAPPoint()), AddressOf createUMAPTable)
    End Sub

    Private Function createTissueTable(tissues As TissueRegion(), args As list, env As Environment) As dataframe
        Dim labels As String() = tissues.Select(Function(i) i.label).ToArray
        Dim colors As String() = tissues.Select(Function(i) i.color.ToHtmlColor).ToArray
        Dim x As New List(Of Integer)
        Dim y As New List(Of Integer)

        For Each region In tissues
            For Each p As Point In region.points
                x.Add(p.X)
                y.Add(p.Y)
            Next
        Next

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"label", labels},
                {"color", colors},
                {"x", x.ToArray},
                {"y", y.ToArray}
            }
        }
    End Function

    Private Function createUMAPTable(umap As UMAPPoint(), args As list, env As Environment) As dataframe
        Dim px As Integer() = umap.Select(Function(i) i.Pixel.X).ToArray
        Dim py As Integer() = umap.Select(Function(i) i.Pixel.Y).ToArray
        Dim x As Double() = umap.Select(Function(i) i.x).ToArray
        Dim y As Double() = umap.Select(Function(i) i.y).ToArray
        Dim z As Double() = umap.Select(Function(i) i.z).ToArray
        Dim cluster As Integer() = umap.Select(Function(i) i.class).ToArray

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"px", px},
                {"py", py},
                {"x", x},
                {"y", y},
                {"z", z},
                {"cluster", cluster}
            },
            .rownames = px _
                .Select(Function(xi, i) $"{xi},{py(i)}") _
                .ToArray
        }
    End Function

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

    <ExportAPI("loadTissue")>
    Public Function loadTissue(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim readBuf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If readBuf Like GetType(Message) Then
            Return readBuf.TryCast(Of Message)
        End If

        Using buffer As Stream = readBuf.TryCast(Of Stream)
            Return buffer.ReadTissueMorphology.ToArray
        End Using
    End Function

    <ExportAPI("loadUMAP")>
    Public Function loadUMAP(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim readBuf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If readBuf Like GetType(Message) Then
            Return readBuf.TryCast(Of Message)
        End If

        Using buffer As Stream = readBuf.TryCast(Of Stream)
            Return buffer.ReadUMAP
        End Using
    End Function
End Module
