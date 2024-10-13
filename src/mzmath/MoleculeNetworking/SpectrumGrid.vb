Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math

''' <summary>
''' Make sample spectrum aligns to the peaktable based on the pearson correlation method
''' </summary>
Public Class SpectrumGrid

    Dim clusters As SpectrumLine()
    Dim filenames As String()

    Sub New()

    End Sub

    Public Function SetRawDataFiles(files As IEnumerable(Of NamedCollection(Of PeakMs2))) As SpectrumGrid
        clusters = Clustering(files).ToArray
        Return Me
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="rawdata">the spectrum rawdata from multiple rawdata files</param>
    ''' <returns>
    ''' a collection of the ion groups
    ''' </returns>
    Private Iterator Function Clustering(rawdata As IEnumerable(Of NamedCollection(Of PeakMs2))) As IEnumerable(Of SpectrumLine)
        Dim tree As New BinaryClustering()
        Dim ions As New List(Of PeakMs2)
        Dim files As New List(Of String)

        For Each file As NamedCollection(Of PeakMs2) In rawdata
            Call files.Add(file.name)
            Call ions.AddRange(file)
        Next

        tree = tree.Tree(ions)
        filenames = files.ToArray

        For Each cluster As NamedCollection(Of PeakMs2) In tree.GetClusters
            ' split by rt
            Dim rt_groups = cluster _
                .GroupBy(Function(a) a.rt, offsets:=7.5) _
                .ToArray

            For Each group In rt_groups
                Dim fileIndex = group.ToDictionary(Function(a) a.file)
                Dim i2 = filenames _
                    .Select(Function(name)
                                Return If(fileIndex.ContainsKey(name), fileIndex(name).intensity, 0)
                            End Function) _
                    .ToArray

                Yield New SpectrumLine With {
                    .intensity = i2,
                    .cluster = group.ToArray,
                    .rt = Val(group.name)
                }
            Next
        Next
    End Function

    Public Iterator Function AssignPeaks(peaks As IEnumerable(Of xcms2)) As IEnumerable(Of (peak As xcms2, ms2 As PeakMs2(), cor As Double, score As Double))
        For Each peak As xcms2 In peaks
            Dim i1 As Double() = peak(filenames)
        Next
    End Function

End Class

Public Class SpectrumLine

    Public Property cluster As PeakMs2()
    Public Property intensity As Double()
    Public Property rt As Double

End Class
