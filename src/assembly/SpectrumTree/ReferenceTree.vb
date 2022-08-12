Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Public Class ReferenceTree : Implements IDisposable

    ReadOnly tree As New List(Of BlockNode)
    ReadOnly spectrum As BinaryDataWriter
    ReadOnly da As Tolerance
    ReadOnly intocutoff As RelativeIntensityCutoff

    Private disposedValue As Boolean

    Public Const Magic As String = "BioDeep/ReferenceTree"

    Sub New(file As Stream)
        spectrum = New BinaryDataWriter(file, Encodings.ASCII) With {
            .ByteOrder = ByteOrder.LittleEndian
        }
        spectrum.Write(Magic)
        ' jump point to tree
        spectrum.Write(0&)

        da = Tolerance.DeltaMass(0.3)
        intocutoff = 0.05
    End Sub

    Private Function Append(data As PeakMs2, centroid As ms2(), isMember As Boolean) As Integer
        Dim n As Integer = tree.Count
        Dim childs As Integer()

        If isMember Then
            childs = {}
        Else
            childs = New Integer(9) {}
        End If

        tree.Add(New BlockNode With {
            .Block = WriteSpectrum(data),
            .childs = childs,
            .Id = data.lib_guid,
            .Members = If(isMember, Nothing, New List(Of Integer)),
            .centroid = centroid
        })

        Return n
    End Function

    Public Sub Push(data As PeakMs2)
        Dim centroid As ms2() = data.mzInto _
            .Centroid(da, intocutoff) _
            .ToArray

        If tree.Count = 0 Then
            ' add root node
            Call Append(data, centroid, isMember:=False)
        Else
            Call Push(centroid, node:=tree(Scan0), raw:=data)
        End If
    End Sub

    Private Sub Push(centroid As ms2(), node As BlockNode, raw As PeakMs2)
        Dim score = GlobalAlignment.TwoDirectionSSM(centroid, node.centroid, da)
        Dim min = stdNum.Min(score.forward, score.reverse)
        Dim i As Integer = BlockNode.GetIndex(min)

        If i = -1 Then
            ' add to current cluster members
            node.Members.Add(Append(raw, centroid, isMember:=True))
        ElseIf node.childs(i) > 0 Then
            ' align to next node
            Push(centroid, tree(node.childs(i)), raw)
        Else
            ' create new node
            node.childs(i) = Append(raw, centroid, isMember:=False)
        End If
    End Sub

    Private Function WriteSpectrum(data As PeakMs2) As BufferRegion
        Dim start As Long = spectrum.Position
        Dim scan As ScanMS2 = data.Scan2

        Call Serialization.WriteBuffer(scan, file:=spectrum)

        Return New BufferRegion With {
            .position = start,
            .size = spectrum.Position - start
        }
    End Function

    Private Sub WriteTree()
        Dim jump As Long = spectrum.Position
        Dim nsize = tree.Count

        Call spectrum.Write(nsize)

        For Each node As BlockNode In tree
            Call NodeBuffer.Write(node, file:=spectrum)
        Next

        spectrum.Seek(Magic.Length, SeekOrigin.Begin)
        spectrum.Write(jump)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call WriteTree()
                Call spectrum.Flush()
                Call spectrum.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
