#Region "Microsoft.VisualBasic::ca1e2d8d2e0bc8aaf1fe0a2f3bf22421, src\mzmath\ms2_math-core\Spectra\Alignment\JaccardAlignment.vb"

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

    '     Class JaccardAlignment
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) GetScore
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Spectra

    Public Class JaccardAlignment : Inherits AlignmentProvider

        ReadOnly topSet As Integer

        Public Sub New(mzwidth As Tolerance, intocutoff As LowAbundanceTrimming, Optional topSet As Integer = 5)
            MyBase.New(mzwidth, intocutoff)

            Me.topSet = topSet
        End Sub

        Public Overrides Function GetScore(a As ms2(), b As ms2()) As Double
            Return GlobalAlignment.JaccardIndex(a, b, mzwidth, topSet)
        End Function

        Public Overrides Function GetScore(alignment() As SSM2MatrixFragment) As (forward As Double, reverse As Double)
            Static NA As Index(Of String) = {"NA", "n/a", "NaN"}

            Dim intersect As Integer = Aggregate a As SSM2MatrixFragment
                                       In alignment
                                       Where Not a.da Like NA
                                       Into Sum(1)
            Dim union As Integer = alignment.Length
            Dim J As Double = intersect / union

            Return (J, J)
        End Function
    End Class
End Namespace
