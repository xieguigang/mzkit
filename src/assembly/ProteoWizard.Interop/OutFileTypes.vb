#Region "Microsoft.VisualBasic::decb5ff5286f94b7691b3c3fcea84ebc, src\assembly\ProteoWizard.Interop\OutFileTypes.vb"

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

    ' Enum OutFileTypes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports System.ComponentModel

Public Enum OutFileTypes
    ''' <summary>
    ''' write mzXML format
    ''' </summary>
    <Description("--mzXML")> mzXML
    ''' <summary>
    ''' write mzML format [default]
    ''' </summary>
    <Description("--mzML")> mzML
    ''' <summary>
    ''' write mz5 format
    ''' </summary>
    <Description("--mz5")> mz5
    ''' <summary>
    ''' write Mascot generic format
    ''' </summary>
    <Description("--mgf")> mgf
    ''' <summary>
    ''' write ProteoWizard internal text format
    ''' </summary>
    <Description("--text")> text
    ''' <summary>
    ''' write MS1 format
    ''' </summary>
    <Description("--ms1")> ms1
    ''' <summary>
    ''' write CMS1 format
    ''' </summary>
    <Description("--cms1")> cms1
    ''' <summary>
    ''' write MS2 format
    ''' </summary>
    <Description("--ms2")> ms2
    ''' <summary>
    ''' write CMS2 format
    ''' </summary>
    <Description("--cms2")> cms2
End Enum

