#Region "Microsoft.VisualBasic::5cccc52790eff37c6b0978c4a3c344fd, src\mzmath\LmMath\lm.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module lm
    ' 
    '     Function: CreateLinearModel
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module lm

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="reference">通过实验所建立的标准曲线点</param>
    ''' <param name="contents">标准曲线点所对应的浓度值</param>
    ''' <returns></returns>
    Public Function CreateLinearModel(reference As DataSet, contents As DataSet) As IFitted
        Dim refKeys As String() = contents.Keys.OrderBy(Function(level) level).ToArray
        Dim ref As Vector = refKeys.Select(Function(level) contents(level)).ToArray
        Dim measure As Vector = refKeys.Select(Function(level) reference(level)).ToArray
        Dim weight As Vector = 1 / measure ^ 2
        Dim R2 As Double = -9999
        Dim bestfit As IFitted
        Dim model As IFitted

        model = WeightedLinearRegression.Regress(measure, ref, weight, 1)
        bestfit = model

        If Not model Is Nothing AndAlso model.CorrelationCoefficient > R2 Then
            R2 = model.CorrelationCoefficient
            bestfit = model

            If R2 > 0.999 Then
                Return bestfit
            End If
        End If

        For p As Integer = 0 To 1
            ' 循环删除一个点，取R2最大的
            Dim X, Y As Vector
            Dim RMax As Double = -9999
            Dim modelBest As IFitted = Nothing
            Dim bestX As Vector = Nothing
            Dim bestY As Vector = Nothing

            For i As Integer = 0 To measure.Length - 1
                X = measure.Delete({i})
                Y = ref.Delete({i})
                weight = 1 / X ^ 2
                model = WeightedLinearRegression.Regress(X, Y, weight, 1)

                If Not model Is Nothing AndAlso model.CorrelationCoefficient > RMax Then
                    RMax = model.CorrelationCoefficient
                    modelBest = model
                    bestX = X
                    bestY = Y
                End If
            Next

            If RMax > R2 Then
                R2 = RMax
                bestfit = modelBest
                measure = bestX
                ref = bestY

                If R2 > 0.999 Then
                    Return bestfit
                End If
            End If
        Next

        Return bestfit
    End Function
End Module
