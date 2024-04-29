#Region "Microsoft.VisualBasic::f427165eb1a764394544a9662052b54b, E:/mzkit/src/assembly/assembly//MarkupData/imzML/XML/ScanData/iPixelIntensity.vb"

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

    '   Total Lines: 45
    '    Code Lines: 13
    ' Comment Lines: 27
    '   Blank Lines: 5
    '     File Size: 1.44 KB


    '     Class iPixelIntensity
    ' 
    '         Properties: average, basePeakIntensity, basePeakMz, median, min
    '                     numIons
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MarkupData.imzML

    ''' <summary>
    ''' A pixel spot scan summary data in MSI
    ''' </summary>
    Public Class iPixelIntensity : Inherits PixelScanIntensity

        ''' <summary>
        ''' The max intensity in current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property basePeakIntensity As Double
        ''' <summary>
        ''' The average intensity in current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property average As Double
        ''' <summary>
        ''' The ions m/z which it is the max intensity value inside current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property basePeakMz As Double
        ''' <summary>
        ''' the min intensity value of current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property min As Double
        ''' <summary>
        ''' the 50% quantile intensity of current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property median As Double
        ''' <summary>
        ''' the number of the ions in current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property numIons As Integer

        Public Overrides Function ToString() As String
            Return $"[{x}, {y}] {totalIon.ToString("G3")}"
        End Function

    End Class

End Namespace
