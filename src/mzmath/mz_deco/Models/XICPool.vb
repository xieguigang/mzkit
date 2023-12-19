Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class XICPool

    ReadOnly samplefiles As New Dictionary(Of String, MzGroup())
    ReadOnly sampleIndex As New Dictionary(Of String, BlockSearchFunction(Of (mz As Double, Integer)))

    Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Add(sample As String, ParamArray ions As MzGroup())
        Call samplefiles.Add(sample, ions)
        Call sampleIndex.Add(sample, ions.Select(Function(i) i.mz).ToArray.CreateMzIndex)
    End Sub

    Public Iterator Function GetXICMatrix(mz As Double, mzdiff As Tolerance) As IEnumerable(Of NamedValue(Of MzGroup))
        For Each file As KeyValuePair(Of String, BlockSearchFunction(Of (mz As Double, Integer))) In sampleIndex
            Dim offsets = file.Value _
                .Search((mz, -1)) _
                .Where(Function(q) mzdiff(mz, q.mz)) _
                .OrderBy(Function(q) mzdiff.MassError(q.mz, mz)) _
                .FirstOrDefault

            If offsets.mz > 0 Then
                Dim XIC = samplefiles(file.Key)(offsets.Item2)
                Dim tuple As New NamedValue(Of MzGroup)(file.Key, XIC)

                Yield tuple
            End If
        Next
    End Function

End Class
