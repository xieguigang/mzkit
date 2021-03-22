#Region "Microsoft.VisualBasic::5b08c828b9f594a2d7e91e33bc0b7e27, src\assembly\mzPack\mzPackWriter.vb"

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

    ' Class mzPackWriter
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Sub: AddOtherScanner, SetThumbnail, writeIndex, writeScannerIndex, writeScanners
    '          writeThumbnail
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http

Public Class mzPackWriter : Inherits BinaryStreamWriter

    ''' <summary>
    ''' ``[readkey => tempfile]`` 
    ''' </summary>
    ReadOnly scanners As New Dictionary(Of String, String)

    ''' <summary>
    ''' temp file path of the thumbnail image
    ''' </summary>
    Dim thumbnail As String
    Dim scannerIndex As New Dictionary(Of String, Long)
    Dim worktemp As String = App.GetAppSysTempFile("_mzpackwriter", App.PID.ToHexString, prefix:="other_scanners")

    Public Sub New(file As String)
        MyBase.New(file)
    End Sub

    Sub New(file As Stream)
        Call MyBase.New(file)
    End Sub

    Public Sub SetThumbnail(img As Image)
        If Not img Is Nothing Then
            thumbnail = $"{worktemp}/thumbnail.png"
            img.SaveAs(thumbnail)
        End If
    End Sub

    Public Sub AddOtherScanner(key As String, data As ChromatogramOverlap)
        Dim file As String = $"{worktemp}/{key.NormalizePathString}.cdf"

        Using buffer As Stream = file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call data.SavePackData(file:=buffer)
        End Using

        scanners(key) = file
    End Sub

    Private Sub writeScanners()
        Dim indexOffset As Long = file.Position

        ' index offset
        Call file.Write(0&)
        Call file.Flush()

        For Each scanner In scanners
            Dim start As Long = file.Position
            Dim bytes As Byte() = scanner.Value.ReadBinary

            Call file.Write(bytes.Length)
            Call file.Write(bytes)
            Call scannerIndex.Add(scanner.Key, start)
            Call file.Flush()
        Next

        Dim scannerIndexOffset As Long = file.Position

        Using file.TemporarySeek(indexOffset, SeekOrigin.Begin)
            Call file.Write(scannerIndexOffset)
            Call file.Flush()
        End Using
    End Sub

    Private Sub writeScannerIndex()
        Call file.Write(scannerIndex.Count)

        For Each item In scannerIndex
            Call file.Write(item.Value)
            Call file.Write(item.Key, BinaryStringFormat.ZeroTerminated)
        Next

        Call file.Flush()
    End Sub

    ''' <summary>
    ''' ``[image_chunk][startOffset]``
    ''' </summary>
    Private Sub writeThumbnail()
        Dim start As Long = file.Position

        If thumbnail.FileExists Then
            Using img As Stream = thumbnail.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Call img _
                    .GZipStream _
                    .ToArray _
                    .DoCall(AddressOf file.Write)
            End Using

            Call file.Write(start)
            Call file.Flush()
        End If
    End Sub

    Protected Overrides Sub writeIndex()
        ' write MS index
        MyBase.writeIndex()

        Call writeScanners()
        Call writeScannerIndex()
        Call writeThumbnail()
    End Sub
End Class

