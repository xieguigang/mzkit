#Region "Microsoft.VisualBasic::5a2a2f146594d35f7caba2fc0c32ffc4, Rscript\Library\mzkit\Assembly.vb"

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

' Module Assembly
' 
'     Function: ReadMgfIons
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports mzXMLAssembly = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML

''' <summary>
''' The mass spectrum assembly file read/write library module.
''' </summary>
<Package("mzkit.assembly", Category:=APICategories.UtilityTools)>
Module Assembly

    <ExportAPI("read.mgf")>
    Public Function ReadMgfIons(file As String) As Ions()
        Return MgfReader.StreamParser(path:=file).ToArray
    End Function

    <ExportAPI("write.mgf")>
    Public Function writeMgfIons(ions As pipeline, file$, Optional relativeInto As Boolean = False) As Boolean
        Using mgfWriter As StreamWriter = file.OpenWriter(Encodings.ASCII, append:=False)
            For Each ionPeak As PeakMs2 In ions.populates(Of PeakMs2)
                Call ionPeak _
                    .MgfIon _
                    .WriteAsciiMgf(mgfWriter, relativeInto)
            Next
        End Using

        Return True
    End Function

    ''' <summary>
    ''' Convert mzxml file as mgf ions.
    ''' </summary>
    ''' <param name="mzXML"></param>
    ''' <returns></returns>
    <ExportAPI("mzxml.mgf")>
    Public Function mzXML2Mgf(mzXML$, Optional relativeInto As Boolean = False, Optional includesMs1 As Boolean = False) As pipeline
        Dim basename$ = mzXML.FileName
        Dim loader = Iterator Function() As IEnumerable(Of PeakMs2)
                         For Each ms2Scan As scan In mzXMLAssembly.XML _
                            .LoadScans(mzXML) _
                            .Where(Function(s)
                                       If includesMs1 Then
                                           Return True
                                       Else
                                           Return s.msLevel = 2
                                       End If
                                   End Function)

                             If ms2Scan.msLevel = 1 Then
                                 ' ms1的数据总是使用raw intensity值
                                 Yield ms2Scan.ScanData(basename, raw:=True)
                             Else
                                 Yield ms2Scan.ScanData(basename, raw:=Not relativeInto)
                             End If
                         Next
                     End Function

        Return New pipeline(loader(), GetType(PeakMs2))
    End Function
End Module
