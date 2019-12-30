#Region "Microsoft.VisualBasic::ae7ee499616b887f4ddd6adea34b5dde, DATA\ms2_math-core\Chromatogram\Deconvolution\Deconvolution.vb"

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

    '     Module Deconvolution
    ' 
    '         Function: GetPeakGroups
    ' 
    '     Class PeakFeature
    ' 
    '         Properties: mz
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Chromatogram

    Public Module Deconvolution

        ''' <summary>
        ''' All of the mz value in <paramref name="mzpoints"/> should be equals
        ''' </summary>
        ''' <param name="mzpoints"></param>
        ''' <returns></returns>
        ''' <remarks>应用于处理复杂的样本数据</remarks>
        <Extension>
        Public Function GetPeakGroups(mzpoints As IEnumerable(Of TICPoint)) As IEnumerable(Of PeakFeature)
            Dim timepoints = mzpoints.OrderBy(Function(p) p.time).ToArray
        End Function


    End Module

    Public Class PeakFeature : Inherits ROI

        Public Property mz As Double

    End Class
End Namespace
