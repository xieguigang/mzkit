#Region "Microsoft.VisualBasic::1411624225756b5b923737144eeb2c06, mzkit\src\mzkit\Task\Properties\PixelProperty.vb"

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

    '   Total Lines: 65
    '    Code Lines: 53
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 2.72 KB


    ' Class PixelProperty
    ' 
    '     Properties: AverageIons, Gini, MaxIntensity, MinIntensity, NumOfIons
    '                 Q1, Q1Count, Q2, Q2Count, Q3
    '                 Q3Count, ScanId, ShannonEntropy, TopIonMz, TotalIon
    '                 X, Y
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Math.Information
Imports Microsoft.VisualBasic.Math.Quantile
Imports stdNum = System.Math

Public Class PixelProperty

    Public ReadOnly Property TopIonMz As Double
    Public ReadOnly Property MaxIntensity As Double
    Public ReadOnly Property MinIntensity As Double
    Public ReadOnly Property NumOfIons As Integer
    <Category("Intensity")> Public ReadOnly Property Q1 As Double
    <Category("Intensity")> Public ReadOnly Property Q2 As Double
    <Category("Intensity")> Public ReadOnly Property Q3 As Double
    <Category("Intensity")> Public ReadOnly Property Q1Count As Integer
    <Category("Intensity")> Public ReadOnly Property Q2Count As Integer
    <Category("Intensity")> Public ReadOnly Property Q3Count As Integer

    Public ReadOnly Property AverageIons As Double
    Public ReadOnly Property TotalIon As Double
    <Category("Pixel")> Public ReadOnly Property X As Integer
    <Category("Pixel")> Public ReadOnly Property Y As Integer
    <Category("Pixel")> Public ReadOnly Property ScanId As String

    Public ReadOnly Property ShannonEntropy As Double
    Public ReadOnly Property Gini As Double

    Sub New(pixel As PixelScan)
        Dim ms As ms2() = pixel.GetMs
        Dim into As Double() = ms.Select(Function(mz) mz.intensity).ToArray

        X = pixel.X
        Y = pixel.Y
        ScanId = pixel.scanId

        If into.Length = 0 Then
        Else
            NumOfIons = ms.Length
            TopIonMz = stdNum.Round(ms.OrderByDescending(Function(i) i.intensity).First.mz, 4)
            MaxIntensity = stdNum.Round(into.Max)
            MinIntensity = stdNum.Round(into.Min)
            TotalIon = stdNum.Round(into.Sum)
            AverageIons = stdNum.Round(into.Average)

            Dim quartile = into.Quartile

            Q1 = stdNum.Round(quartile.Q1)
            Q2 = stdNum.Round(quartile.Q2)
            Q3 = stdNum.Round(quartile.Q3)
            Q1Count = into.Where(Function(i) i <= quartile.Q1).Count
            Q2Count = into.Where(Function(i) i <= quartile.Q2).Count
            Q3Count = into.Where(Function(i) i <= quartile.Q3).Count

            Dim bin = CutBins.FixedWidthBins(into, 10, Function(x) x).ToArray
            Dim probs As Double() = bin.Select(Function(n) n.Count / into.Length).ToArray

            ShannonEntropy = stdNum.Round(probs.ShannonEntropy, 4)
            Gini = stdNum.Round(probs.Gini, 4)
        End If
    End Sub

End Class
