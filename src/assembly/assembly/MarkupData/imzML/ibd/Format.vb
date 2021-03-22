#Region "Microsoft.VisualBasic::20f9b4cd47456daa830848e30677bcdd, src\assembly\assembly\MarkupData\imzML\ibd\Format.vb"

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

    '     Enum Format
    ' 
    '         Continuous, Processed
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MarkupData.imzML

    ''' <summary>
    ''' In order to insure efficient storage, two different 
    ''' formats of the binary data are defined: continuous 
    ''' and processed. 
    ''' </summary>
    Public Enum Format
        ''' <summary>
        ''' Continuous type means that each spectrum of an image has the 
        ''' same m/z values. As a result the m/z array is only saved once 
        ''' directly behind the UUID of the file and the intensity arrays 
        ''' of the spectra are following. 
        ''' </summary>
        Continuous
        ''' <summary>
        ''' At the processed type every spectrum has its own m/z array. 
        ''' So it is necessary to save both – the m/z array and the 
        ''' corresponding intensity array – per spectrum.
        ''' </summary>
        Processed
    End Enum
End Namespace
