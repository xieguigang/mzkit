#Region "Microsoft.VisualBasic::11d519a9d6dabc32823e6fe044ee5792, assembly\SpectrumTree\Query\Ms2Search.vb"

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

    '   Total Lines: 24
    '    Code Lines: 17 (70.83%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (29.17%)
    '     File Size: 802 B


    '     Class Ms2Search
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Centroid
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace Query

    Public MustInherit Class Ms2Search

        Protected ReadOnly da As Tolerance
        Protected ReadOnly intocutoff As RelativeIntensityCutoff

        Sub New(Optional da As Double = 0.3, Optional intocutoff As Double = 0.05)
            Me.da = Tolerance.DeltaMass(da)
            Me.intocutoff = intocutoff
        End Sub

        Public Function Centroid(matrix As ms2()) As ms2()
            Return matrix.Centroid(da, intocutoff).ToArray
        End Function

        Public MustOverride Function Search(centroid As ms2(), mz1 As Double) As IEnumerable(Of ClusterHit)

    End Class
End Namespace
