Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver

Public Module viz

    ''' <summary>
    ''' 这个函数只负责生成相对应的可视化样式以及调用作图函数,并不会处理网络的布局,所以会需要在
    ''' 调用这个函数之前完成网络的布局生成操作.网络的布局生成可以使用<see cref="doForceLayout"/>
    ''' 函数来完成,也可以从其他的外部文件之中导入
    ''' </summary>
    ''' <param name="metaDNA"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Draw(metaDNA As NetworkGraph, Optional size$ = "8000,7000", Optional defaultColor$ = NameOf(Color.Black)) As GraphicsData
        Return metaDNA.DrawImage(size, defaultColor:=defaultColor.TranslateColor)
    End Function
End Module
