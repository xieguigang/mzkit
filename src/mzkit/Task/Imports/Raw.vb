#Region "Microsoft.VisualBasic::1570af52a085ba56778e4a1b283e4d89, src\mzkit\Task\Raw.vb"

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

' Class Raw
' 
'     Properties: cache, numOfScans, rtmax, rtmin, scans
'                 source
' 
' Class ScanEntry
' 
'     Properties: BPC, charge, id, mz, polarity
'                 rt, TIC, XIC
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Linq

Public Class Raw

    ''' <summary>
    ''' 原始数据文件位置
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String
    ''' <summary>
    ''' 二进制缓存文件位置
    ''' </summary>
    ''' <returns></returns>
    Public Property cache As String
    Public Property rtmin As Double
    Public Property rtmax As Double
    ''' <summary>
    ''' MS1扫描的数量
    ''' </summary>
    ''' <returns></returns>
    Public Property numOfScan1 As Integer
    ''' <summary>
    ''' MS2扫描的数量
    ''' </summary>
    ''' <returns></returns>
    Public Property numOfScan2 As Integer

    Dim loaded As mzPack

    Public ReadOnly Property cacheFileExists As Boolean
        Get
            Return cache.FileExists
        End Get
    End Property

    Public Function LoadMzpack() As Raw
        Using file As Stream = cache.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            loaded = mzPack.ReadAll(file)
        End Using

        Return Me
    End Function

    Public Function UnloadMzpack() As Raw
        Erase loaded.MS

        loaded.Scanners.Clear()
        loaded.Thumbnail.Dispose()
        loaded.Thumbnail = Nothing
        loaded = Nothing

        Return Me
    End Function

    Public Function FindMs2Scan(id As String) As ScanMS2
        Return GetMs2Scans.Where(Function(a) a.scan_id = id).FirstOrDefault
    End Function

    Public Function GetCacheFileSize() As Long
        Return cache.FileLength
    End Function

    Public Function GetMs1Scans() As IEnumerable(Of ScanMS1)
        Return loaded.MS
    End Function

    Public Function GetMs2Scans() As IEnumerable(Of ScanMS2)
        Return loaded.MS _
            .Select(Function(m1) m1.products) _
            .IteratesALL
    End Function

End Class