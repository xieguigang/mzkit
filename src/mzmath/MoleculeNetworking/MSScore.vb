Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataMining.BinaryTree

Public Class MSScore : Inherits ComparisonProvider

    ReadOnly align As AlignmentProvider
    ReadOnly ms2 As Dictionary(Of String, PeakMs2)

    Public ReadOnly Property Ions As IEnumerable(Of PeakMs2)
        Get
            Return ms2.Values
        End Get
    End Property

    Public Sub New(align As AlignmentProvider, ions As IEnumerable(Of PeakMs2), equals As Double, gt As Double)
        MyBase.New(equals, gt)

        Me.align = align
        Me.ms2 = ions.ToDictionary(Function(i) i.lib_guid)
    End Sub

    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Return align.GetScore(ms2(x).mzInto, ms2(y).mzInto)
    End Function
End Class
