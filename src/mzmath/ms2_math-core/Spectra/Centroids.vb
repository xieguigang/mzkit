Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Spectra

    Public Module Centroids

        ''' <summary>
        ''' Convert profile matrix to centroid matrix
        ''' </summary>
        ''' <param name="[lib]"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function CentroidMode([lib] As LibraryMatrix,
                                     tolerance As Tolerance,
                                     Optional cutoff As LowAbundanceTrimming = Nothing,
                                     Optional aggregate As Func(Of IEnumerable(Of Double), Double) = Nothing) As LibraryMatrix

            If aggregate IsNot Nothing Then
                [lib].ms2 = [lib].ms2 _
                    .Centroid(tolerance, cutoff Or LowAbundanceTrimming.Default, aggregate) _
                    .ToArray
            Else
                [lib].ms2 = [lib].ms2 _
                    .Centroid(tolerance, cutoff Or LowAbundanceTrimming.Default) _
                    .ToArray
            End If

            [lib].centroid = True

            Return [lib]
        End Function

        ''' <summary>
        ''' Convert profile matrix to centroid matrix
        ''' 
        ''' the intensity value in a mzbin group is aggregate by the max intensity value 
        ''' </summary>
        ''' <param name="peaks"></param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' order of data processing:
        ''' 
        ''' ``intensity_cutoff -> centroid``
        ''' 
        ''' this function will always keeps the fragment peak annotation data.
        ''' </remarks>
        <Extension>
        Public Function Centroid(peaks As ms2(), tolerance As Tolerance, cutoff As LowAbundanceTrimming) As IEnumerable(Of ms2)
            ' removes low intensity fragment peaks
            ' for save calculation time
            peaks = cutoff.Trim(peaks)

            If peaks.Length = 0 Then
                Return {}
            Else
                ' 20200702 due to the reason of we not calculate the peakarea
                ' so that there is no needs for populate ROI
                ' find the highest fragment directly
                Return peaks _
                    .GroupBy(Function(ms2) ms2.mz, AddressOf tolerance.Equals) _
                    .Select(Function(g)
                                ' 合并在一起的二级碎片的相应强度取最高的为结果
                                Dim fragments As ms2() = g.ToArray
                                Dim maxi As Integer = which.Max(fragments.Select(Function(m) m.intensity))
                                Dim max As ms2 = fragments(maxi)
                                Dim annos As String = fragments _
                                    .Where(Function(f) Not f.Annotation.StringEmpty) _
                                    .Select(Function(f) f.Annotation) _
                                    .Distinct _
                                    .JoinBy(", ")

                                Return New ms2(max.mz, max.intensity) With {
                                    .Annotation = annos
                                }
                            End Function) _
                    .ToArray
            End If
        End Function

        ''' <summary>
        ''' Convert profile matrix to centroid matrix
        ''' 
        ''' the intensity value in a mzbin group is aggregate by the sum intensity value 
        ''' </summary>
        ''' <param name="peaks"></param>
        ''' <param name="tolerance"></param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' this function will keeps the annotation metadata for the peaks.
        ''' </remarks>
        <Extension>
        Public Function Centroid(peaks As ms2(),
                                 tolerance As Tolerance,
                                 cutoff As LowAbundanceTrimming,
                                 aggregate As Func(Of IEnumerable(Of Double), Double)) As IEnumerable(Of ms2)

            ' removes low intensity fragment peaks
            ' for save calculation time
            peaks = cutoff.Trim(peaks)

            If peaks.Length = 0 Then
                Return {}
            Else
                ' 20200702 due to the reason of we not calculate the peakarea
                ' so that there is no needs for populate ROI
                ' find the highest fragment directly
                Return peaks _
                    .GroupBy(Function(ms2) ms2.mz, AddressOf tolerance.Equals) _
                    .Select(Function(g)
                                ' 合并在一起的二级碎片的相应强度取最高的为结果
                                Dim fragments As ms2() = g.ToArray
                                Dim maxi As Integer = which.Max(fragments.Select(Function(m) m.intensity))
                                Dim max As ms2 = fragments(maxi)
                                Dim annos As String = fragments _
                                    .Where(Function(f) Not f.Annotation.StringEmpty) _
                                    .JoinBy(", ")
                                Dim sum As Double = If(g.Length = 1,
                                    max.intensity,
                                    aggregate(g.Select(Function(mzi) mzi.intensity))
                                )

                                Return New ms2(max.mz, sum) With {
                                    .Annotation = annos
                                }
                            End Function) _
                    .ToArray
            End If
        End Function
    End Module
End Namespace