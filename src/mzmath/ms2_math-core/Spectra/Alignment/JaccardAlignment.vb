#Region "Microsoft.VisualBasic::f8f5242b68044171ba568544a2d8cd2e, mzmath\ms2_math-core\Spectra\Alignment\JaccardAlignment.vb"

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

    '   Total Lines: 57
    '    Code Lines: 46 (80.70%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (19.30%)
    '     File Size: 2.29 KB


    '     Class JaccardAlignment
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetJaccardScore, (+2 Overloads) GetScore
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace Spectra

    Public Class JaccardAlignment : Inherits AlignmentProvider

        ReadOnly topSet As Integer

        Public Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming, Optional topSet As Integer = 5)
            MyBase.New(mzwidth, intocutoff)
            Me.topSet = topSet
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetScore(a As ms2(), b As ms2()) As Double
            Return GlobalAlignment.JaccardIndex(a, b, mzwidth, topSet)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetScore(alignment() As SSM2MatrixFragment) As (forward As Double, reverse As Double)
            Dim j = GetJaccardScore(alignment, topSet)
            Return (j, j)
        End Function

        Public Shared Function GetJaccardScore(alignment() As SSM2MatrixFragment, Optional topSet As Integer = 5) As Double
            Dim q = (From t As SSM2MatrixFragment
                     In alignment
                     Order By t.query Descending
                     Take topSet
                     Where t.query > 0
                     Select t).ToArray
            Dim r = (From t As SSM2MatrixFragment
                     In alignment
                     Order By t.ref Descending
                     Take topSet
                     Where t.ref > 0
                     Select t).ToArray

            Static NA As Index(Of String) = {"NA", "n/a", "NaN"}

            alignment = q.JoinIterates(r).Distinct.ToArray

            Dim intersect As Integer = Aggregate a As SSM2MatrixFragment
                                       In alignment
                                       Where Not a.da Like NA
                                       Into Count
            Dim union As Integer = alignment.Length
            Dim J As Double = intersect / union

            Return J
        End Function
    End Class
End Namespace
