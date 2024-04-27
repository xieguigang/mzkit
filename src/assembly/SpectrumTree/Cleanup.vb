#Region "Microsoft.VisualBasic::10bb8b5197528aad08ae67d3eb298483, G:/mzkit/src/assembly/SpectrumTree//Cleanup.vb"

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

    '   Total Lines: 111
    '    Code Lines: 95
    ' Comment Lines: 6
    '   Blank Lines: 10
    '     File Size: 4.47 KB


    ' Module Cleanup
    ' 
    '     Function: Compress, CreateSpecRows, Union
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Module Cleanup

    ''' <summary>
    ''' Make union and compress the spectrum which associated a same metabolite.
    ''' </summary>
    ''' <param name="spec"></param>
    ''' <param name="n"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Compress(spec As IEnumerable(Of PeakMs2), n As Integer) As IEnumerable(Of PeakMs2)
        Dim da3 As Tolerance = Tolerance.DeltaMass(0.3)
        Dim rawdata As Dictionary(Of String, PeakMs2) = spec _
            .Select(Function(s, i)
                        s.lib_guid = $"{s.lib_guid}.{i + 1}"
                        Return s
                    End Function) _
            .ToDictionary(Function(s) s.lib_guid)
        Dim allMz As Double() = rawdata.Values _
            .Select(Function(m) m.mzInto) _
            .IteratesALL _
            .ToArray _
            .Centroid(da3, New RelativeIntensityCutoff(0.0)) _
            .Select(Function(a) a.mz) _
            .ToArray
        Dim specData As EntityClusterModel() = rawdata.Values _
            .CreateSpecRows(allMz, da3) _
            .Kmeans(expected:=n)

        For Each group In specData.GroupBy(Function(a) a.Cluster)
            Yield group _
                .ToArray _
                .Union(group.Key, rawdata, da3)
        Next
    End Function

    <Extension>
    Private Function Union(norm_spec As EntityClusterModel(), clusterId As String, rawdata As Dictionary(Of String, PeakMs2), da As Tolerance) As PeakMs2
        If norm_spec.Length = 1 Then
            Return rawdata(norm_spec(Scan0).ID)
        End If

        Dim template = rawdata(norm_spec(Scan0).ID)
        Dim spec As ms2() = norm_spec _
            .Select(Function(r)
                        Return r.Properties _
                            .Where(Function(m) m.Value > 0) _
                            .Select(Function(n)
                                        Return New ms2(n.Key, n.Value)
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray _
            .Centroid(da, New RelativeIntensityCutoff(0)) _
            .ToArray
        Dim rt As Double() = norm_spec.Select(Function(s) rawdata(s.ID).rt).ToArray
        Dim mz1 As NamedCollection(Of Double)() = norm_spec _
            .Select(Function(s) rawdata(s.ID).mz) _
            .GroupBy(da) _
            .OrderByDescending(Function(a) a.Length) _
            .ToArray

        Return New PeakMs2 With {
            .mzInto = spec,
            .file = template.file,
            .lib_guid = template.scan & "#" & clusterId,
            .scan = template.scan,
            .rt = rt.Average,
            .mz = mz1(Scan0).value.Average
        }
    End Function

    <Extension>
    Private Iterator Function CreateSpecRows(rawdata As IEnumerable(Of PeakMs2), allMz As Double(), da As Tolerance) As IEnumerable(Of EntityClusterModel)
        Dim keys As String() = allMz.Select(Function(m) m.ToString("F4")).ToArray
        Dim feature_mz As Double

        For Each spec As PeakMs2 In rawdata
            Dim v As New Dictionary(Of String, Double)
            Dim maxinto As Double = spec.mzInto.Max(Function(m) m.intensity)
            Dim norm As ms2() = spec.mzInto _
                .Select(Function(mzi)
                            Return New ms2 With {
                                .mz = mzi.mz,
                                .intensity = mzi.intensity / maxinto
                            }
                        End Function) _
                .ToArray

            For i As Integer = 0 To keys.Length - 1
                feature_mz = allMz(i)
                v(keys(i)) = Aggregate mzi As ms2
                             In norm
                             Where da(mzi.mz, feature_mz)
                             Into Sum(mzi.intensity)
            Next

            Yield New EntityClusterModel With {
                .ID = spec.lib_guid,
                .Properties = v
            }
        Next
    End Function
End Module
