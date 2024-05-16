#Region "Microsoft.VisualBasic::50ae7f080bb34d28612681c6c5d32d7c, assembly\ProteoWizard.Interop\PWConvertProcessor.vb"

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

    '   Total Lines: 26
    '    Code Lines: 20
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 860 B


    ' Class PWConvertProcessor
    ' 
    '     Function: runConvert
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports ProteoWizard.Interop.filters

<Assembly: InternalsVisibleTo("mzkit")>

Friend Class PWConvertProcessor

    Public filetype As OutFileTypes
    Public filters As Filter()
    Public output As String
    Public bin As ProteoWizardCLI

    Public Function runConvert(file As SeqValue(Of String)) As SeqValue(Of Boolean)
        Dim outputfile = $"{output}/{file.value.FileName}"
        Dim stdOut As String = bin.Convert2mzML(file.value, output, filetype, filters)

        Call stdOut.SaveTo(outputfile.ChangeSuffix("log"))

        If outputfile.FileExists(ZERO_Nonexists:=True) Then
            Return New SeqValue(Of Boolean)(file, True)
        Else
            Return New SeqValue(Of Boolean)(file, False)
        End If
    End Function
End Class
