Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Module ChromatogramPlot

    ''' <summary>
    ''' 从mzML文件之中解析出色谱数据之后，将所有的色谱峰都绘制在一张图之中进行可视化
    ''' </summary>
    ''' <param name="mzML$"></param>
    ''' <param name="size$"></param>
    ''' <param name="margin$"></param>
    ''' <param name="bg$"></param>
    ''' <returns></returns>
    Public Function Plot(mzML$,
                         Optional size$ = "2100,1600",
                         Optional margin$ = g.DefaultPadding,
                         Optional bg$ = "white",
                         Optional colors$ = "",
                         Optional penStyle$ = Stroke.ScatterLineStroke) As GraphicsData

    End Function
End Module
