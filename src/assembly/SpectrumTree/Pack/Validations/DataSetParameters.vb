#Region "Microsoft.VisualBasic::bf5e6833af783da9a5437caea4728e1b, E:/mzkit/src/assembly/SpectrumTree//Pack/Validations/DataSetParameters.vb"

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

    '   Total Lines: 19
    '    Code Lines: 11
    ' Comment Lines: 4
    '   Blank Lines: 4
    '     File Size: 534 B


    '     Class DataSetParameters
    ' 
    '         Properties: AverageNumberOfSpectrum, IdRange, Ions, RawFiles, rawname
    '                     rtmax, rtmin
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PackLib.Validation

    Public Class DataSetParameters

        Public Property Ions As Integer
        Public Property RawFiles As Integer
        Public Property AverageNumberOfSpectrum As Integer
        Public Property rtmin As Double
        Public Property rtmax As Double

        ''' <summary>
        ''' The raw file base name
        ''' </summary>
        ''' <returns></returns>
        Public Property rawname As String
        Public Property IdRange As String()

    End Class
End Namespace
