Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Bootstrapping

Namespace GCMS.QuantifyAnalysis

    Public Module StandardCurve

        ''' <summary>
        ''' 选取所传递进来的峰面积作为X轴，tuple第一项目的浓度值作为Y轴结果构建线性回归模型
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <returns></returns>
        <Extension>
        Public Function LinearRegression(raw As (content#, data As ChromatographyPeaktable)()) As IFitted
            Dim line As PointF() = raw _
                .OrderBy(Function(p)
                             ' 从小到大进行排序
                             Return p.content
                         End Function) _
                .Select(Function(p)
                            Return New PointF(p.data.TPACalibration, p.content)
                        End Function) _
                .ToArray
            Dim model As IFitted = FitModel.CreateLinearRegression(line, True)
            Return model
        End Function
    End Module
End Namespace