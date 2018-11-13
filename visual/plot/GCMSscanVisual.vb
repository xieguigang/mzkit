Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Device
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Model
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
                              Optional size$ = "5000,5000",
                              Optional colors$ = "clusters",
                              Optional axisLabelFontCSS$ = CSSFont.Win7Normal,
                              Optional axisStrokeCss$ = Stroke.AxisStroke,
                              Optional arrowFactor$ = "2,2",
                              Optional sn_threshold# = 5,
                              Optional viewDistance% = 1000,
                              Optional viewAngle$ = "30,60,-56",
                              Optional fov% = 500000) As GraphicsData

        Dim camera As New Camera(viewAngle) With {
            .fov = fov,
            .screen = size.SizeParser,
            .ViewDistance = viewDistance
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

        Dim model As New List(Of Element3D)
        Dim TICArea As New Polygon With {
            .brush = Brushes.Yellow,
            .Path = data _
                .times _
                .Select(Function(time, i)
                            Return New Point3D(time, data.tic(i), 0)
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
                B = New Point3D(rtX, mz.intensity, mz.mz)
            Next

        Next
    End Function
End Module
