Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Calculus
Imports Microsoft.VisualBasic.Data.csv

''' <summary>
''' 分子的能量分布模型
''' </summary>
Public Class EnergyModel

    Dim ODE As New ODE With {.y0 = 0, .df = Function(x, y) Bootstraping.ProbabilityDensity(x, 5, 5)}

    Sub New()

        Dim resulkt = ODE.RK4(3000, 0, 100)

        Dim X = {"X"}.JoinIterates(resulkt.X.ToArray.Select(Function(n) CStr(n))).ToArray
        Dim Y = {"Y"}.JoinIterates(resulkt.Y.Vector.Select(Function(n) CStr(n))).ToArray
        Dim csv = {X, Y}.JoinColumns

        Call csv.Save("./test.csv")
    End Sub
End Class
