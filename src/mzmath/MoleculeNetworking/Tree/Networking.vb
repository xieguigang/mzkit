#Region "Microsoft.VisualBasic::693b8d5f74b398d907f1aa4cb9cbdcd1, mzmath\MoleculeNetworking\Tree\Networking.vb"

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


    ' Code Statistics:

    '   Total Lines: 132
    '    Code Lines: 81
    ' Comment Lines: 42
    '   Blank Lines: 9
    '     File Size: 5.04 KB


    ' Module Networking
    ' 
    '     Function: normPeaki, RepresentativeSpectrum, Tree, unionMetadata
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Molecular networks are visual displays of the chemical space present 
''' in tandem mass spectrometry (MS/MS) experiments. This visualization 
''' approach can detect sets of spectra from related molecules (molecular
''' networks), even when the spectra themselves are not matched to any 
''' known compounds.
'''
''' The visualization Of molecular networks In GNPS represents Each spectrum 
''' As a node, And spectrum-To-spectrum alignments As edges (connections)
''' between nodes. Nodes can be supplemented With metadata, including 
''' dereplication matches Or information that Is provided by the user, e.g.
''' As abundance, origin Of product, biochemical activity, Or hydrophobicity,
''' which can be reflected In a node's size or color. This map of all 
''' related molecules is visualized as a global molecular network.
''' </summary>
Public Module Networking

    ''' <summary>
    ''' implements the molecule networking
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="intocutoff"></param>
    ''' <param name="equals"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Tree(ions As IEnumerable(Of PeakMs2),
                         Optional mzdiff As Double = 0.3,
                         Optional intocutoff As Double = 0.05,
                         Optional equals As Double = 0.85,
                         Optional diff As Double = 0.1) As TreeCluster

        Return New NetworkingTree(mzdiff, intocutoff, equals, interval:=diff).Tree(ions)
    End Function

    Private Iterator Function normPeaki(i As PeakMs2) As IEnumerable(Of ms2)
        Dim maxinto As Double = i.mzInto _
            .Select(Function(mzi) mzi.intensity) _
            .Max

        For Each mzi As ms2 In i.mzInto
            Yield New ms2 With {
                .mz = mzi.mz,
                .intensity = mzi.intensity / maxinto
            }
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cluster"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' <see cref="PeakMs2.collisionEnergy"/> is tagged as the cluster size
    ''' </remarks>
    <Extension>
    Private Function unionMetadata(cluster As IEnumerable(Of PeakMs2)) As Dictionary(Of String, String)
        Return cluster _
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
    End Function

    ''' <summary>
    ''' merge the given collection of the ms2 spectrum data as an union spectrum data
    ''' </summary>
    ''' <param name="cluster"></param>
    ''' <param name="mzdiff">
    ''' the mzdiff tolerance value for grouping the union ms2 peaks based 
    ''' on the centroid function
    ''' </param>
    ''' <param name="zero"></param>
    ''' <param name="key"></param>
    ''' <returns></returns>
    <Extension>
    Public Function RepresentativeSpectrum(cluster As PeakMs2(),
                                           mzdiff As Tolerance,
                                           zero As RelativeIntensityCutoff,
                                           Optional key As String = Nothing) As PeakMs2
        Dim union As ms2() = cluster _
            .Select(AddressOf normPeaki) _
            .IteratesALL _
            .ToArray _
            .Centroid(mzdiff, cutoff:=zero) _
            .ToArray
        Dim rt As Double = cluster _
            .Select(Function(c) c.rt) _
            .TabulateBin _
            .Average
        Dim mz1 As Double
        Dim metadata = cluster.unionMetadata

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
            .collisionEnergy = cluster.Length,
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
