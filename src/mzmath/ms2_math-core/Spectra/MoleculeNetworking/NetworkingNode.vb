Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Class NetworkingNode

    Public Property representation As LibraryMatrix

    Public Property members As PeakMs2()
    Public Property mz As Double

    Public ReadOnly Property referenceId As String
        Get
            Return representation.name
        End Get
    End Property

    Public Function GetXIC() As ChromatogramTick()
        Return members _
            .Select(Function(a)
                        Return New ChromatogramTick With {
                            .Time = a.rt,
                            .Intensity = a.Ms2Intensity
                        }
                    End Function) _
            .OrderBy(Function(a) a.Time) _
            .ToArray
    End Function

    Public Overrides Function ToString() As String
        Return referenceId
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="tolerance">ms2 tolerance</param>
    ''' <returns></returns>
    Public Shared Function Create(parentIon As Double, raw As SpectrumCluster, tolerance As Tolerance) As NetworkingNode
        Dim ions As PeakMs2() = raw.cluster _
            .Select(Function(a)
                        Dim maxInto = a.mzInto.Select(Function(x) x.intensity).Max

                        For i As Integer = 0 To a.mzInto.Length - 1
                            a.mzInto(i).quantity = a.mzInto(i).intensity / maxInto
                        Next

                        Return a
                    End Function) _
            .ToArray
        Dim nodeData As LibraryMatrix = unionRepresentative(ions, tolerance)

        Return New NetworkingNode With {
            .mz = parentIon,
            .members = ions,
            .representation = nodeData
        }
    End Function

    Private Shared Function unionRepresentative(ions As PeakMs2(), tolerance As Tolerance) As LibraryMatrix
        Dim mz As NamedCollection(Of ms2)() = ions _
            .Select(Function(i) i.mzInto) _
            .IteratesALL _
            .GroupBy(Function(a) a.mz, tolerance) _
            .ToArray
        Dim matrix As ms2() = mz _
            .Select(Function(a)
                        Return New ms2 With {
                            .mz = Val(a.name),
                            .intensity = a.Select(Function(x) x.quantity).Max,
                            .quantity = .intensity
                        }
                    End Function) _
            .ToArray
        Dim products As String = matrix _
            .OrderByDescending(Function(a) a.intensity) _
            .Take(3) _
            .Select(Function(a) a.mz.ToString("F3")) _
            .JoinBy(", ")

        Return New LibraryMatrix With {
            .centroid = True,
            .ms2 = matrix,
            .name = $"{ions.Select(Function(a) a.mz).Average.ToString("F3")} [{products}]"
        }
    End Function

End Class