Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem


Namespace PoolData

    Public Class TreeFs : Implements IDisposable

        ''' <summary>
        ''' due to the reason of contains in-memory cache system
        ''' inside this module, so we should share this object between
        ''' multiple pool object
        ''' </summary>
        Dim score As MSScoreGenerator

        ''' <summary>
        ''' A sequence spectrum data pool in current level 
        ''' </summary>
        ''' <remarks>
        ''' the stream data should be open in read/write mode
        ''' </remarks>
        Dim spectrumPool As BinaryDataReader
        Dim fs As StreamPack

        Public ReadOnly Property level As Double
        Public ReadOnly Property split As Integer
        Public ReadOnly Property splitDelta As Double

        Private disposedValue As Boolean

        Public ReadOnly Property baseStream As Stream
            Get
                Return spectrumPool.BaseStream
            End Get
        End Property

        Sub New(dir As String)
            Me.fs = New StreamPack(dir & "/cluster.pack", meta_size:=1024 * 1024 * 256)

            spectrumPool = New BinaryDataReader($"{dir}/spectrum.dat".Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
            spectrumPool.ByteOrder = ByteOrder.LittleEndian
            spectrumPool.Encoding = Encoding.ASCII
        End Sub

        Friend Sub SetLevel(level As Double, split As Integer)
            _level = level
            _split = split
            _splitDelta = level / split
        End Sub

        Friend Sub SetScore(da As Double, intocutoff As Double, getSpectral As Func(Of String, PeakMs2))
            score = New MSScoreGenerator(
                align:=AlignmentProvider.Cosine(Tolerance.DeltaMass(da), New RelativeIntensityCutoff(intocutoff)),
                getSpectrum:=getSpectral,
                equals:=0,
                gt:=0
            )
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Add(data As PeakMs2)
            Call score.Add(data)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetScore(x As String, y As String) As Double
            Return score.GetSimilarity(x, y)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadText(path As String) As String
            Return fs.ReadText(path)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WriteText(text As String, path As String)
            Call fs.WriteText(text, path)
        End Sub

        Public Function OpenFolder(path As String) As StreamGroup
            Dim obj As StreamObject = fs.GetObject(path & "/")

            If obj Is Nothing Then
                fs.OpenBlock(path & "/index.txt")
                obj = fs.GetObject(path & "/")
            End If

            Return obj
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadSpectrum(metadata As Metadata) As PeakMs2
            Return InternalFileSystem.ReadSpectrum(spectrumPool, block:=metadata.block)
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call spectrumPool.Dispose()
                    Call fs.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace