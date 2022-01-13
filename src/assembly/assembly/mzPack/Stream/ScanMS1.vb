#Region "Microsoft.VisualBasic::8e57e5bd762c4ed7a6f4f51a8c88b428, src\assembly\assembly\mzPack\Stream\ScanMS1.vb"

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

'     Class ScanMS1
' 
'         Properties: BPC, meta, products, TIC
' 
'         Function: GetIntensity, GetMs1Scans, GetMSIPixel, hasMetaKeys
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace mzData.mzWebCache

    ''' <summary>
    ''' MS scan
    ''' </summary>
    Public Class ScanMS1 : Inherits MSScan
        Implements ITimeSignal

        Public Overrides Property rt As Double Implements ITimeSignal.time
        Public Property TIC As Double Implements ITimeSignal.intensity
        Public Property BPC As Double
        Public Property products As ScanMS2()

        ''' <summary>
        ''' other meta data about this MS1 scan, likes
        ''' the [x,y] coordinate data of MSI scan data.
        ''' </summary>
        ''' <returns></returns>
        Public Property meta As Dictionary(Of String, String)

        ''' <summary>
        ''' get meta data from <see cref="meta"/>
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Default Public Property value(key As String) As String
            Get
                Return meta.TryGetValue(key)
            End Get
            Set(value As String)
                meta(key) = value
            End Set
        End Property

        ''' <summary>
        ''' get [x, y] pixel point from the scan metadata or scan id data.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetMSIPixel() As Point
            If meta.IsNullOrEmpty Then
                Dim pixel As String = scan_id.Match("\[\d+,\d+\]")

                If pixel.StringEmpty Then
                    Return Nothing
                Else
                    Return Casting.PointParser(pixel.GetStackValue("[", "]"))
                End If
            ElseIf meta.ContainsKey("x") AndAlso meta.ContainsKey("y") Then
                Return New Point(Integer.Parse(meta!x), Integer.Parse(meta!y))
            ElseIf meta.ContainsKey("X") AndAlso meta.ContainsKey("Y") Then
                Return New Point(Integer.Parse(meta!X), Integer.Parse(meta!Y))
            Else
                Return Nothing
            End If
        End Function

        Public Iterator Function GetMs1Scans() As IEnumerable(Of ms1_scan)
            For i As Integer = 0 To mz.Length - 1
                Yield New ms1_scan With {
                    .mz = mz(i),
                    .intensity = into(i),
                    .scan_time = rt
                }
            Next
        End Function

        ''' <summary>
        ''' has all meta keys?
        ''' </summary>
        ''' <param name="keys"></param>
        ''' <returns></returns>
        Public Function hasMetaKeys(ParamArray keys As String()) As Boolean
            If meta.IsNullOrEmpty Then
                Return False
            End If

            For Each key As String In keys
                If Not meta.ContainsKey(key) Then
                    Return False
                End If
            Next

            Return True
        End Function
    End Class
End Namespace
