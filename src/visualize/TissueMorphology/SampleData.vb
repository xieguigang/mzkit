Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Math.Distributions

Public Module SampleData

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="layer">
    ''' the layer data of target ion m/z in the ms-imaging raw data file
    ''' </param>
    ''' <param name="regions"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ExtractSample(Of T As Pixel)(layer As T(),
                                                 regions As IEnumerable(Of TissueRegion),
                                                 Optional n As Integer = 32,
                                                 Optional coverage As Double = 0.3) As Dictionary(Of String, NamedValue(Of Double()))

        Dim data As New Dictionary(Of String, NamedValue(Of Double()))
        Dim matrix = Grid(Of T).Create(layer, Function(i) New Point(i.X, i.Y))

        For Each region As TissueRegion In regions
            Dim A As Integer = region.points.Length
            Dim Nsize As Integer = A * coverage
            Dim samples = Bootstraping.Samples(region.points, Nsize, bags:=n).ToArray
            Dim vec = samples _
                .AsParallel _
                .Select(Function(pack)
                            Dim subset As T() = pack.value _
                                .Select(Function(pt)
                                            Return matrix.GetData(pt.X, pt.Y)
                                        End Function) _
                                .Where(Function(p) Not p Is Nothing) _
                                .ToArray
                            Dim d As Double() = subset _
                                .Select(Function(i) i.Scale) _
                                .ToArray

                            If d.Length = 0 Then
                                Return 0.0
                            Else
                                Return d.Sum / A
                            End If
                        End Function) _
                .ToArray

            data(region.label) = New NamedValue(Of Double())(region.color.ToHtmlColor, vec)
        Next

        Return data
    End Function

End Module