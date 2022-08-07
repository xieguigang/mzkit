Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace IndexedCache

    Public Class XICPackWriter : Implements IDisposable

        ReadOnly stream As StreamPack

        Dim disposedValue As Boolean

        Sub New(file As String)
            Call Me.New(file.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
        End Sub

        Sub New(file As Stream)
            stream = New StreamPack(file,, meta_size:=32 * 1024 * 1024)
        End Sub

        Public Sub SetAttribute(dims As Size)
            Call stream.globalAttributes.Add("dims", {dims.Width, dims.Height})
        End Sub

        Public Sub AddLayer(layer As MatrixXIC)
            Using buffer As Stream = stream.OpenBlock($"/{layer.GetType.Name}/{layer.mz}.ms")
                Call layer.Serialize(buffer)
            End Using
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="file">the target file to write</param>
        Public Shared Sub IndexRawData(raw As PixelReader, file As Stream,
                                       Optional da As Double = 0.01,
                                       Optional spares As Double = 0.2)
            Dim ionList = raw.AllPixels _
                .Select(Function(i)
                            Dim pt As New Point(i.X, i.Y)
                            Dim ions = i.GetMsPipe.Select(Function(ms) (pt, ms))

                            Return ions
                        End Function) _
                .IteratesALL _
                .ToArray
            Dim mzgroups = ionList _
                .GroupBy(Function(mzi)
                             Return mzi.ms.mz
                         End Function, offsets:=da)
            Dim dims As Size = raw.dimension
            Dim total As Integer = dims.Area

            Using pack As New XICPackWriter(file)
                Call pack.SetAttribute(raw.dimension)

                For Each layer In mzgroups
                    Dim pixels = layer.GroupBy(Function(p) $"{p.pt.X},{p.pt.Y}").ToArray
                    Dim data As MatrixXIC

                    If pixels.Length / total > spares Then
                        ' matrix
                        Dim matrix = Grid(Of IGrouping(Of String, (Point, ms2))) _
                            .Create(
                                data:=pixels,
                                getPixel:=Function(p)
                                              Dim t As String() = p.Key.Split(","c)
                                              Dim pt As New Point(t(0), t(1))

                                              Return pt
                                          End Function)
                        Dim intensity As New List(Of Double)
                        Dim hit As Boolean

                        For i As Integer = 1 To dims.Width
                            For j As Integer = 1 To dims.Height
                                Dim p = matrix.GetData(i, j, hit)

                                If hit Then
                                    intensity.Add(p.Average(Function(a) a.Item2.intensity))
                                Else
                                    intensity.Add(0)
                                End If
                            Next
                        Next

                        data = New MatrixXIC With {
                            .mz = Val(layer.name),
                            .intensity = intensity.ToArray
                        }
                    Else
                        ' point
                        data = New PointXIC With {
                            .x = pixels.Select(Function(p) p.First.pt.X).ToArray,
                            .y = pixels.Select(Function(p) p.First.pt.Y).ToArray,
                            .mz = Val(layer.name),
                            .intensity = pixels _
                                .Select(Function(p)
                                            Return p.Average(Function(a) a.ms.intensity)
                                        End Function) _
                                .ToArray
                        }
                    End If

                    Call pack.AddLayer(layer:=data)
                Next
            End Using
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call stream.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace