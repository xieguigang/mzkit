Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Device
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.App.GCMS
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Public Module GCMSscanVisual

    ''' <summary>
    ''' 这个是使用三维图表的方式进行绘制GC_MS的实验数据
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PlotScans(data As GCMSJson,
                              Optional size$ = "8000,5000",
                              Optional padding$ = g.DefaultPadding,
                              Optional bg$ = "white",
                              Optional colors$ = "clusters",
                              Optional massFragmentStrokeCSS$ = "stroke: skyblue; stroke-width: 5px; stroke-dash: solid;",
                              Optional axisLabelFontCSS$ = CSSFont.Win7Normal,
                              Optional axisStrokeCss$ = Stroke.AxisStroke,
                              Optional arrowFactor$ = "2,2",
                              Optional sn_threshold# = 5,
                              Optional viewDistance% = 8000,
                              Optional viewAngle$ = "30,60,-56",
                              Optional fov% = 50000) As GraphicsData

        Dim camera As New Camera(viewAngle) With {
            .fov = fov,
            .screen = size.SizeParser,
            .ViewDistance = viewDistance
        }
        Dim plotRegion As New GraphicsRegion With {
            .Size = camera.screen,
            .Padding = padding
        }
        Dim ROIlist As ROI() = data _
            .ExportROI _
            .Where(Function(region)
                       Return region.snRatio >= sn_threshold
                   End Function) _
            .ToArray
        '
        ' intensity
        '  ^ y   z
        '  |   / m/z
        '  |  /  
        '  | /
        '  |/          retention time
        '  ------------------> X 
        Dim X, Y, Z As Vector

        X = data.times.Range.CreateAxisTicks ' 时间
        Y = data.tic.Range.CreateAxisTicks   ' TIC
        ' Z axis is mass scans
        Z = data.ms _
            .Select(Function(i)
                        Return i.Select(Function(scan) scan.mz)
                    End Function) _
            .IteratesALL _
            .Range _
            .CreateAxisTicks

        ' 因为intensity可能会达到上十万
        ' 与几百秒放在一起会非常不协调
        ' 所以所有的intensity值都会需要经过这个对象进行比例缩放
        Dim intensityScaler As New YScaler(reversed:=False) With {
            .region = plotRegion.PlotRegion,
            .Y = d3js.scale.linear _
                .domain(Y) _
                .range(integers:={0, plotRegion.Height})
        }

        Dim model As New List(Of Element3D)
        Dim TICArea As New Polygon With {
            .brush = Brushes.Yellow,
            .Path = data _
                .times _
                .Select(Function(time, i)
                            Return New Point3D(time, intensityScaler.TranslateY(data.tic(i)), 0)
                        End Function) _
                .ToArray
        }

        ' 添加坐标轴模型
        model += AxisDraw.Axis(
            X, Y, Z, CSSFont.TryParse(axisLabelFontCSS),
            ("retention time(s)", "intensity", "m/z"),
            axisStrokeCss,
            arrowFactor
        )
        ' 添加TIC曲线多边形
        model += TICArea

        Dim axisStroke As Pen = Stroke.TryParse(axisStrokeCss)
        Dim massFragmentStroke As Pen = Stroke.TryParse(massFragmentStrokeCSS)

        ' 添加ms scan信号柱模型
        For Each region As ROI In ROIlist

            ' 每一个ROI的ms scan的X都是相同的
            Dim rtX = region.rt
            Dim msScans = data.GetMsScan(region).GroupByMz()
            Dim A As New Point3D(rtX, 0, 0)
            Dim B As New Point3D(rtX, 0, Z.Max)

            ' 添加绘制基础的线轴的模型
            model += New Line(A, B) With {
                .Stroke = axisStroke
            }

            For Each mz As ms1_scan In msScans
                A = New Point3D(rtX, 0, mz.mz)
                B = New Point3D(rtX, intensityScaler.TranslateY(mz.intensity), mz.mz)

                model += New Line(A, B) With {
                    .Stroke = massFragmentStroke
                }
            Next
        Next

        Dim plotInternal =
            Sub(ByRef g As IGraphics, region As GraphicsRegion)
                ' 要先绘制三维图形，要不然会将图例遮住的
                Call model.RenderAs3DChart(g, camera, region)
            End Sub

        Return plotRegion _
            .Size _
            .GraphicsPlots(padding, bg, plotInternal)
    End Function
End Module
