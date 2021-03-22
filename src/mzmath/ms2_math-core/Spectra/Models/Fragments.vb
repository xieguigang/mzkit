#Region "Microsoft.VisualBasic::de3609c37377fc15d90f0aee40d00748, src\mzmath\ms2_math-core\Spectra\Models\Fragments.vb"

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

    '     Class Library
    ' 
    '         Properties: ID, LibraryIntensity, Name, PrecursorMz, ProductMz
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Spectra

    ''' <summary>
    ''' MS2 fragment matrix
    ''' </summary>
    Public Class Library

        ''' <summary>
        ''' Fragment ID in this matrix.
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String
        ''' <summary>
        ''' 前体离子的m/z
        ''' </summary>
        ''' <returns></returns>
        Public Property PrecursorMz As Double
        ''' <summary>
        ''' 碎片的m/z
        ''' </summary>
        ''' <returns></returns>
        Public Property ProductMz As Double
        ''' <summary>
        ''' 当前的这个碎片的信号强度
        ''' </summary>
        ''' <returns></returns>
        Public Property LibraryIntensity As Double
        ''' <summary>
        ''' library name
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        Public Overrides Function ToString() As String
            Return $"[{ProductMz}, {LibraryIntensity}]"
        End Function
    End Class
End Namespace
