#Region "Microsoft.VisualBasic::8e2609076e9c212ea6df1331ab97e58d, assembly\MarkupData\mzML\RawScanParser.vb"

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

    '     Class RawScanParser
    ' 
    '         Function: hasFileContent, IsMRMData, IsSIMData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData.mzML

    Public Class RawScanParser

        Public Const SRMSignature As String = "selected reaction monitoring chromatogram"
        Public Const SIMSignature As String = "selected ion monitoring chromatogram"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function IsSIMData(mzml As String) As Boolean
            Return hasFileContent(mzml, SIMSignature)
        End Function

        Public Shared Function hasFileContent(mzml As String, contentSig As String) As Boolean
            For Each content As fileDescription In mzml.LoadXmlDataSet(Of fileDescription)(, xmlns:=Xmlns)
                If content.fileContent IsNot Nothing AndAlso Not content.fileContent.cvParams.IsNullOrEmpty Then
                    If content.fileContent.cvParams.Any(Function(a) a.name = contentSig) Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            Next

            Return False
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function IsMRMData(mzml As String) As Boolean
            Return hasFileContent(mzml, SRMSignature)
        End Function
    End Class
End Namespace
