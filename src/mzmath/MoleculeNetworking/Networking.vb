Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Imaging.Physics.World
Imports Microsoft.VisualBasic.Serialization.JSON
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

Public Module Networking

    <Extension>
    Public Function Tree(ions As IEnumerable(Of PeakMs2),
                         Optional mzdiff As Double = 0.3,
                         Optional intocutoff As Double = 0.05,
                         Optional equals As Double = 0.85) As ClusterTree

        Dim align As New MSScore(New CosAlignment(Tolerance.DeltaMass(mzdiff), New RelativeIntensityCutoff(intocutoff)), ions, equals, equals)
        Dim clustering As New ClusterTree

        For Each ion As PeakMs2 In align.Ions
            Call ClusterTree.Add(clustering, ion.lib_guid, align, threshold:=equals)
        Next

        Return clustering
    End Function

    <Extension>
    Public Function RepresentativeSpectrum(cluster As PeakMs2(),
                                           tolerance As Tolerance,
                                           zero As RelativeIntensityCutoff,
                                           Optional key As String = Nothing) As PeakMs2
        Dim union As ms2() = cluster _
            .Select(Function(i)
                        Dim maxinto As Double = i.mzInto _
                            .Select(Function(mzi) mzi.intensity) _
                            .Max

                        Return i.mzInto _
                            .Select(Function(mzi)
                                        Return New ms2 With {
                                            .mz = mzi.mz,
                                            .intensity = mzi.intensity / maxinto
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray _
            .Centroid(tolerance, cutoff:=zero) _
            .ToArray
        Dim rt As Double = cluster _
            .Select(Function(c) c.rt) _
            .TabulateBin _
            .Average
        Dim mz1 As Double
        Dim metadata = cluster _
            .Select(Function(c) c.meta) _
            .IteratesALL _
            .GroupBy(Function(t) t.Key) _
            .ToDictionary(Function(t) t.Key,
                            Function(t)
                                Return t _
                                    .Select(Function(ti) ti.Value) _
                                    .Distinct _
                                    .JoinBy("; ")
                            End Function)

        If cluster.Length = 1 Then
            mz1 = cluster(Scan0).mz
        Else
            mz1 = 0
            metadata("mz1") = cluster _
                .Select(Function(c) c.mz) _
                .ToArray _
                .GetJson
        End If

        Return New PeakMs2 With {
            .rt = rt,
            .activation = "NA",
            .collisionEnergy = 0,
            .file = key,
            .intensity = cluster.Sum(Function(c) c.intensity),
            .lib_guid = key,
            .mz = mz1,
            .mzInto = union,
            .precursor_type = "NA",
            .scan = "NA",
            .meta = metadata
        }
    End Function
End Module
