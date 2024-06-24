Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Parallel

Namespace MatrixMath

    Public Class FeatureTrIQClips : Inherits VectorTask

        ReadOnly raw As MzMatrix
        ReadOnly features As Double()()
        ReadOnly q As Double = 0.6
        ReadOnly levels As Integer = 100

        Public Sub New(m As MzMatrix, Optional q As Double = 0.6, Optional levels As Integer = 100)
            MyBase.New(m.featureSize)

            Me.raw = m
            Me.features = Allocate(Of Double())(all:=True)
            Me.q = q
            Me.levels = levels
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            For i As Integer = start To ends
                Dim offset As Integer = i
                Dim v As Double() = raw.matrix _
                    .Select(Function(x) x(offset)) _
                    .ToArray
                Dim cutoff As Double = TrIQ.FindThreshold(v, q, levels)

                features(offset) = v.ClipUpper(cutoff).ToArray
            Next
        End Sub

        Public Function GetClipMatrix() As MzMatrix
            Return New MzMatrix With {
                .matrixType = raw.matrixType,
                .mz = raw.mz,
                .mzmax = raw.mzmax,
                .mzmin = raw.mzmin,
                .tolerance = raw.tolerance,
                .matrix = raw.matrix _
                    .Select(Function(a, i)
                                Dim offset As Integer = i
                                Dim vi As Double() = features _
                                    .Select(Function(mzi) mzi(offset)) _
                                    .ToArray
                                Dim pi As New PixelData() With {
                                    .label = a.label,
                                    .X = a.X,
                                    .Y = a.Y,
                                    .Z = a.Z,
                                    .intensity = vi
                                }

                                Return pi
                            End Function) _
                    .ToArray
            }
        End Function
    End Class
End Namespace