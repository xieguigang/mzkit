Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Bootstrapping

Namespace GCMS

    Public Module StandardCurve

        <Extension>
        Public Function LinearRegression(raw As (content#, data As ROITable)()) As WeightedFit
            Dim line As PointF() = raw _
                .OrderBy(Function(p)
                             ' 从小到大进行排序
                             Return p.content
                         End Function) _
                .Select(Function(p)
                            Return New PointF(p.data.integration, p.content)
                        End Function) _
                .ToArray
            Dim model As WeightedFit = FitModel.CreateLinearRegression(line)
            Return model
        End Function
    End Module
End Namespace