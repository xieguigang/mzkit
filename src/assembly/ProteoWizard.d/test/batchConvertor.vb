#Region "Microsoft.VisualBasic::ab89490313800117d3ae5a2bef44b4b0, Assembly\ProteoWizard.d\test\batchConvertor.vb"

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

' Module batchConvertor
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports ProteoWizard.d
Imports ProteoWizard.Interop

Module batchConvertor

    Sub Main()
        Dim cli As New ProteoWizardCLI(App.GetVariable("bin"))

        For Each dir As String In App.CommandLine.Name.ListDirectory
            Call dir.__INFO_ECHO

            For Each file As String In dir.EnumerateFiles("*.raw")
                If Not file.ChangeSuffix("mzXML").FileLength > 1024 Then
                    Call cli.Convert2mzML(file, dir, ProteoWizardCLI.OutFileTypes.mzXML)
                End If

                Call file.Warning
            Next
        Next
    End Sub
End Module

