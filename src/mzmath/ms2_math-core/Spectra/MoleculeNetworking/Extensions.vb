Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports stdNum = System.Math

Namespace Spectra.MoleculeNetworking

    Module Module1

        ''' <summary>
        ''' Split the current spectrum cluster into multiple part of the sub-cluster via a given rt window
        ''' </summary>
        ''' <param name="cluster"></param>
        ''' <param name="rt_win"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function SplitClusterRT(cluster As NetworkingNode, Optional rt_win As Double = 30) As IEnumerable(Of NetworkingNode)
            If cluster.size <= 1 Then
                Yield cluster
                Return
            End If

            Dim rt_groups = cluster.members.GroupBy(Function(m) m.rt, Function(m1, m2) stdNum.Abs(m1 - m2) <= rt_win)
            Dim da As Tolerance = Tolerance.DeltaMass(0.3)
            Dim intocutoff As New RelativeIntensityCutoff(0.05)

            For Each subgroup As NamedCollection(Of PeakMs2) In rt_groups
                Dim mz As Double = subgroup.Select(Function(m) m.mz).Average
                Dim subcluster = NetworkingNode.Create(mz, subgroup.value, da, intocutoff)
                Dim guid As String = $"{cluster.referenceId}-{(subcluster.referenceId & "rt=" & subgroup.name).MD5}"

                ' update the new reference id
                subcluster.representation.name = guid

                Yield subcluster
            Next
        End Function
    End Module
End Namespace