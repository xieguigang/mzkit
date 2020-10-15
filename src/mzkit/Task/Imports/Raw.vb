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

Imports Microsoft.VisualBasic.Linq

Public Class Raw

    Public Property source As String
    Public Property ms1_cache As String
    Public Property ms2_cache As String

    ''' <summary>
    ''' 为了提升性能所缓存下来的原始数据散点图
    ''' </summary>
    ''' <returns></returns>
    Public Property scatter As String

    Public Property rtmin As Double
    Public Property rtmax As Double

    Public ReadOnly Property numOfScans As Integer
        Get
            If scans.IsNullOrEmpty Then
                Return 0
            Else
                Return scans.Length
            End If
        End Get
    End Property

    Public Property scans As Ms1ScanEntry()

    Public ReadOnly Property cacheFileExists As Boolean
        Get
            Return ms1_cache.FileExists AndAlso ms2_cache.FileExists
        End Get
    End Property

    Public Function FindMs2Scan(id As String) As ScanEntry
        Return GetMs2Scans.Where(Function(a) a.id = id).FirstOrDefault
    End Function

    Public Function GetCacheFileSize() As Long
        Return ms1_cache.FileLength + ms2_cache.FileLength + scatter.FileLength
    End Function

    Public Function GetMs2Scans() As IEnumerable(Of ScanEntry)
        Return scans.Select(Function(a) a.products).IteratesALL
    End Function

End Class

Public MustInherit Class MsScanEntry

    Public Property id As String
    Public Property rt As Double
    Public Property TIC As Double
    Public Property BPC As Double

End Class

''' <summary>
''' ms1 scan entry data
''' </summary>
Public Class Ms1ScanEntry : Inherits MsScanEntry

    Public Property products As ScanEntry()
End Class

''' <summary>
''' ms2 scan entry data
''' </summary>
Public Class ScanEntry : Inherits MsScanEntry

    Public Property mz As Double
    Public Property polarity As Integer
    Public Property charge As Double
    Public Property XIC As Double

    Public Overrides Function ToString() As String
        Return id
    End Function
End Class
