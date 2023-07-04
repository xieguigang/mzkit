#Region "Microsoft.VisualBasic::011dd6040618020c95af60b8b964f5aa, mzkit\src\visualize\MsImaging\IndexedCache\XICReader.vb"

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

'   Total Lines: 203
'    Code Lines: 144
' Comment Lines: 24
'   Blank Lines: 35
'     File Size: 6.83 KB


'     Class XICReader
' 
'         Properties: dimension
' 
'         Constructor: (+2 Overloads) Sub New
' 
'         Function: GetLayer, GetMz, GetPixel
' 
'         Sub: (+2 Overloads) Dispose
'         Class IonIndex
' 
'             Properties: filename, mz, type
' 
'             Constructor: (+1 Overloads) Sub New
'             Function: ToString
' 
'         Class PixelIndex
' 
'             Properties: filename, x, y
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.GridGraph
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports stdNum = System.Math

Namespace IndexedCache

    ''' <summary>
    ''' the mzImage file reader
    ''' </summary>
    Public Class XICReader : Implements IDisposable

        ReadOnly stream As StreamPack

        Dim disposedValue As Boolean
        Dim dims As Size
        Dim mzquery As BlockSearchFunction(Of IonIndex)
        Dim matrix As Grid(Of PixelIndex)

        Private Class IonIndex

            Public ReadOnly Property mz As Double
            Public Property type As Integer
            Public Property filename As String

            Sub New(mz As Double)
                Me.mz = mz
            End Sub

            Public Overrides Function ToString() As String
                Return $"[{mz}] {filename}"
            End Function

        End Class

        Private Class PixelIndex
            Public Property x As Integer
            Public Property y As Integer
            Public Property filename As String
        End Class

        Public ReadOnly Property dimension As Size
            Get
                Return dims
            End Get
        End Property

        Sub New(file As String)
            Call Me.New(file.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
        End Sub

        Sub New(file As Stream)
            stream = New StreamPack(file,, meta_size:=32 * 1024 * 1024)

            Dim dims As Integer() = stream.GetGlobalAttribute("dims")
            Dim layers As StreamGroup = stream.GetObject("/layers/")
            Dim msdata As StreamGroup = stream.GetObject("/msdata/")
            Dim index As New List(Of IonIndex)
            Dim pixels As New List(Of PixelIndex)
            Dim mzdiff As Double = stream.GetGlobalAttribute("mzdiff")

            Me.dims = New Size(dims(0), dims(1))

            For Each obj As StreamObject In layers.ListFiles
                If TypeOf obj Is StreamGroup Then
                    Continue For
                End If

                Dim mz As Double = obj.GetAttribute("mz")
                Dim type As Integer = obj.GetAttribute("type")

                index.Add(New IonIndex(mz) With {
                    .filename = obj.referencePath.ToString,
                    .type = type
                })
            Next

            For Each obj As StreamObject In msdata.ListFiles
                If TypeOf obj Is StreamGroup Then
                    Continue For
                End If

                Dim x As Integer = obj.GetAttribute("x")
                Dim y As Integer = obj.GetAttribute("y")

                pixels.Add(New PixelIndex With {
                    .filename = obj.referencePath.ToString,
                    .x = x,
                    .y = y
                })
            Next

            Me.mzquery = New BlockSearchFunction(Of IonIndex)(
                data:=index,
                eval:=Function(a) a.mz,
                tolerance:=mzdiff,
                factor:=3
            )
            Me.matrix = Grid(Of PixelIndex).Create(
                data:=pixels,
                getPixel:=Function(i) New Point(i.x, i.y)
            )
        End Sub

        ''' <summary>
        ''' get all mz data inside target data pack
        ''' </summary>
        ''' <returns></returns>
        Public Function GetMz() As Double()
            Return mzquery.Keys
        End Function

        Public Function GetPixel(x As Integer, y As Integer) As PixelScan
            Dim hit As Boolean = False
            Dim index As PixelIndex = matrix.GetData(x, y, hit)

            If Not hit Then
                Return Nothing
            Else
                Using buffer As Stream = stream.OpenBlock(index.filename),
                    bin As New BinaryDataReader(buffer) With {
                        .ByteOrder = ByteOrder.BigEndian
                }
                    Dim nsize As Integer = bin.ReadInt32
                    Dim mz As Double() = bin.ReadDoubles(nsize)
                    Dim intensity As Double() = bin.ReadDoubles(nsize)

                    Return New InMemoryVectorPixel(
                        index.filename.BaseName,
                        x:=x,
                        y:=y,
                        mz:=mz,
                        into:=intensity
                    )
                End Using
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <returns>
        ''' returns nothing if layer not found
        ''' </returns>
        Public Function GetLayer(mz As Double, Optional mzdiff As Tolerance = Nothing) As SingleIonLayer
            Dim index As IonIndex() = mzquery _
                .Search(New IonIndex(mz)) _
                .ToArray

            If index.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim ion As IonIndex = index _
                .OrderBy(Function(i) stdNum.Abs(i.mz - mz)) _
                .First

            If Not mzdiff Is Nothing Then
                If Not mzdiff(mz, ion.mz) Then
                    Return Nothing
                End If
            End If

            Using buffer As Stream = stream.OpenBlock(ion.filename)
                Dim layer = MatrixXIC.Decode(buffer, dims)
                layer.mz = mz
                Return layer.GetLayer(dims)
            End Using
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
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
