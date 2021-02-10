#Region "Microsoft.VisualBasic::6a0ce458ca0ec721d9430ec36878bbc4, TargetedMetabolomics\LinearQuantitative\Linear\SampleWorker.vb"

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

    '     Module SampleWorker
    ' 
    '         Function: SampleQuantify, SampleQuantifyScan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default

Namespace LinearQuantitative.Linear

    Public Module SampleWorker

        ''' <summary>
        ''' 默认将``-KB``和``-BLK``结尾的文件都判断为实验空白
        ''' </summary>
        Friend ReadOnly defaultBlankNames As New [Default](Of Func(Of String, Boolean))(
            Function(basename)
                Return InStr(basename, "-KB") > 0 OrElse InStr(basename, "-BLK") > 0
            End Function)

        ''' <summary>
        ''' 对单个原始数据文件做定量计算
        ''' </summary>
        ''' <param name="models"></param>
        ''' <param name="ions"></param>
        ''' <returns></returns>
        <Extension>
        Public Function SampleQuantify(models As StandardCurve(),
                                       ions As TargetPeakPoint(),
                                       integrator As PeakAreaMethods,
                                       Optional names As Dictionary(Of String, String) = Nothing,
                                       Optional baselineQuantile As Double = 0.6,
                                       Optional fileName As String = "NA") As QuantifyScan

            Dim TPA As Dictionary(Of String, IonTPA) = ions _
                .ToDictionary(Function(ion) ion.Name,
                              Function(ion)
                                  Return ion.GetIonTPA(baselineQuantile, integrator)
                              End Function)
            Dim result As New List(Of ContentResult(Of IonPeakTableRow))
            Dim sampleName As String = fileName.BaseName

            If names Is Nothing Then
                names = models _
                    .ToDictionary(Function(line) line.name,
                                  Function(line)
                                      Return line.name
                                  End Function)
            End If

            For Each line As StandardCurve In models
                If TPA.ContainsKey(line.name) Then
                    result += TPA.DoLinearQuantify(line, names, sampleName)
                End If
            Next

            Return result.Where(Function(a) Not a Is Nothing).ToArray.SampleQuantifyScan(fileName)
        End Function

        ''' <summary>
        ''' 对单个原始数据文件做定量计算
        ''' </summary>
        ''' <returns></returns>
        <Extension>
        Public Function SampleQuantifyScan(result As ContentResult(Of IonPeakTableRow)(), fileName As String) As QuantifyScan
            Dim MRMPeakTable As New List(Of IonPeakTableRow)

            For Each metabolite As ContentResult(Of IonPeakTableRow) In result
                MRMPeakTable += metabolite.Peaktable
            Next

            If result.Length = 0 Then
                Call $"[NO_DATA] {fileName.ToFileURL} found nothing!".Warning
                Return Nothing
            End If

            ' 这个是浓度结果数据
            Dim quantify As New DataSet With {
                .ID = fileName.BaseName,
                .Properties = result _
                    .ToDictionary(Function(i) i.Name,
                                    Function(i)
                                        Return i.Content
                                    End Function)
            }

            ' 这个是峰面积比 AIS/At 数据
            Dim X As New DataSet With {
                .ID = fileName.BaseName,
                .Properties = result _
                    .ToDictionary(Function(i) i.Name,
                                    Function(i)
                                        Return i.X
                                    End Function)
            }

            Return New QuantifyScan With {
                .ionPeaks = MRMPeakTable,
                .quantify = quantify,
                .rawX = X
            }
        End Function
    End Module
End Namespace
