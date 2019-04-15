#Region "Microsoft.VisualBasic::b82f8b03249fea333144a7afbbd51d8e, assembly\MarkupData\mzXML\XML.vb"

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

    '     Class XML
    ' 
    '         Properties: index, indexOffset, msRun, shal
    ' 
    '         Function: ExportPeaktable, LoadScans
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.Math

Namespace MarkupData.mzXML

    ''' <summary>
    ''' ``*.mzXML`` raw data
    ''' </summary>
    ''' 
    <XmlType("mzXML", [Namespace]:="http://sashimi.sourceforge.net/schema_revision/mzXML_3.2")> Public Class XML

        Public Property msRun As MSData
        Public Property index As index
        Public Property indexOffset As Long
        Public Property shal As String

        ''' <summary>
        ''' Load all scan node in the mzXML document.(这个函数使用的是Linq集合的方式进行大型原始数据文件的加载操作的)
        ''' </summary>
        ''' <param name="mzXML"></param>
        ''' <returns></returns>
        Public Shared Function LoadScans(mzXML As String) As IEnumerable(Of scan)
            Return mzXML.LoadXmlDataSet(Of scan)(, xmlns:="http://sashimi.sourceforge.net/schema_revision/mzXML_3.2")
        End Function

        Public Shared Function ExportPeaktable(mzXML As String) As Peaktable()
            Dim ms1 As New List(Of scan)   ' peaktable
            Dim msms As New List(Of scan)  ' ms1 scan为msms scan的母离子
            Dim sample$ = mzXML.BaseName

            For Each scan As scan In LoadScans(mzXML)
                If scan.msLevel = "1" Then
                    ms1 += scan
                ElseIf scan.msLevel = "2" Then
                    msms += scan
                End If
            Next

            Dim ms1Peaktable = ms1 _
                .Select(Function(scan)
                            Dim mzInto = scan.peaks.ExtractMzI
                            Dim rt# = Val(scan.retentionTime.Replace("PT", ""))
                            Dim ms1Data = mzInto _
                                .Select(Function(mz)
                                            Return New Peaktable With {
                                                .scan = scan.num,
                                                .rt = rt,
                                                .sample = sample,
                                                .mz = mz.mz,
                                                .into = mz.intensity,
                                                .energy = scan.collisionEnergy
                                            }
                                        End Function)

                            Return ms1Data
                        End Function) _
                .IteratesALL _
                .ToArray

            ' 需要从ms1 scan里面进行解卷积得到分子的响应度信息
            ' 而后就可以导出peaktable了

            Return ms1Peaktable
        End Function
    End Class
End Namespace
