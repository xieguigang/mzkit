#Region "Microsoft.VisualBasic::6454335a199e0f59ec1f01518e4e12b9, E:/mzkit/src/mzmath/ms2_math-core//Spectra/MoleculeNetworking/Extensions.vb"

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

    '   Total Lines: 41
    '    Code Lines: 27
    ' Comment Lines: 7
    '   Blank Lines: 7
    '     File Size: 1.70 KB


    '     Module Extensions
    ' 
    '         Function: SplitClusterRT
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports stdNum = System.Math

Namespace Spectra.MoleculeNetworking

    <HideModuleName>
    Public Module Extensions

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
