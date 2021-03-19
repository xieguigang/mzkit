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

        Using file.TemporarySeek(indexOffset, SeekOrigin.Begin)
            Call file.Write(file.Position)
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
