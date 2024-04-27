#Region "Microsoft.VisualBasic::b3048b852a5b51cf26ba796ab77df74c, G:/mzkit/src/assembly/SpectrumTree//Tree/ReferenceTree.vb"

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


    ' Code Statistics:

    '   Total Lines: 127
    '    Code Lines: 79
    ' Comment Lines: 27
    '   Blank Lines: 21
    '     File Size: 4.74 KB


    '     Class ReferenceTree
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Sub: (+2 Overloads) Dispose, (+2 Overloads) Push, WriteTree
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Text
Imports std = System.Math

Namespace Tree

    ''' <summary>
    ''' the spectrum tree library data structure 
    ''' </summary>
    Public Class ReferenceTree : Implements IDisposable

        ''' <summary>
        ''' the mass tolerance for do parent matched
        ''' </summary>
        Protected ReadOnly da As Tolerance
        Protected ReadOnly tree As InternalFileSystem
        Protected ReadOnly spectrum As BinaryDataWriter
        Protected ReadOnly intocutoff As RelativeIntensityCutoff

        Private disposedValue As Boolean

        Public Const Magic As String = "BioDeep/ReferenceTree"

        Protected Sub New(file As Stream, nbranchs As Integer)
            spectrum = New BinaryDataWriter(file, Encodings.ASCII) With {
                .ByteOrder = ByteOrder.LittleEndian
            }
            spectrum.Write(Magic, BinaryStringFormat.NoPrefixOrTermination)
            ' jump point to tree
            spectrum.Write(0&)

            da = Tolerance.DeltaMass(0.3)
            intocutoff = 0.05
            tree = New InternalFileSystem(nbranch:=nbranchs)
        End Sub

        ''' <summary>
        ''' Create a new reference family tree and which is 
        ''' indexed via the reference spectrum seed
        ''' </summary>
        ''' <param name="file"></param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(file As Stream)
            Call Me.New(file, nbranchs:=10)
        End Sub

        Public Sub Push(data As PeakMs2)
            Dim centroid As ms2() = data.mzInto _
            .Centroid(da, intocutoff) _
            .ToArray

            If tree.size = 0 Then
                ' add root node
                Call tree.Append(data, centroid, isMember:=False, spectrum)
            Else
                Call Push(centroid, node:=tree(Scan0), raw:=data)
            End If
        End Sub

        Protected Overridable Sub Push(centroid As ms2(), node As BlockNode, raw As PeakMs2)
            Dim score = GlobalAlignment.TwoDirectionSSM(centroid, node.centroid, da)
            Dim min As Double = std.Min(score.forward, score.reverse)
            Dim i As Integer = BlockNode.GetIndex(min)

            If i = -1 Then
                ' add to current cluster members
                i = tree.Append(raw, centroid, isMember:=True, spectrum)

                Call node.mz.AddRange(InternalFileSystem.getMz(raw))
                Call node.Members.Add(i)
            ElseIf node.childs(i) > 0 Then
                ' align to next node
                Call Push(centroid, tree(node.childs(i)), raw)
            Else
                ' create new node
                node.childs(i) = tree.Append(raw, centroid, isMember:=False, spectrum)
            End If
        End Sub

        Private Sub WriteTree()
            Dim jump As Long = spectrum.Position
            Dim nsize As Integer = tree.size

            Call spectrum.Write(nsize)

            For Each node As BlockNode In tree
                Call NodeBuffer.Write(node, file:=spectrum)
            Next

            Call spectrum.Seek(Magic.Length, SeekOrigin.Begin)
            Call spectrum.Write(jump)
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
End Namespace
