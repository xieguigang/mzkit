Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class SpectrumReader : Implements IDisposable

    Dim disposedValue As Boolean
    Dim file As StreamPack
    Dim mzIndex As MzIonSearch

    ''' <summary>
    ''' mapping of <see cref="BlockNode.Id"/> to the mass index <see cref="MassIndex.name"/>
    ''' </summary>
    ReadOnly map As Dictionary(Of String, String)
    ReadOnly spectrum As New Dictionary(Of String, BlockNode)

    Default Public ReadOnly Property GetIdMap(libname As String) As String
        Get
            Return map(libname)
        End Get
    End Property

    Sub New(file As Stream)
        Me.file = New StreamPack(file, [readonly]:=True)
        Me.map = Me.file.ReadText("/map.json").LoadJSON(Of Dictionary(Of String, String))
    End Sub

    Public Iterator Function QueryByMz(mz As Double) As IEnumerable(Of BlockNode)
        Dim ions = mzIndex.QueryByMz(mz).ToArray
        Dim index As IEnumerable(Of String) = From i As IonIndex
                                              In ions
                                              Let i32 As Integer = i.node
                                              Distinct
                                              Let tag As String = i.ToString
                                              Select tag

        For Each key As String In index
            If Not spectrum.ContainsKey(key) Then
                Dim path As String = $"/spectrum/{key.Last}/{key}.dat"
                Dim file As Stream = Me.file.OpenBlock(path)
                Dim spectrumNode = NodeBuffer.Read(New BinaryDataReader(file))

                SyncLock spectrum
                    Call spectrum.Add(key, spectrumNode)
                End SyncLock
            End If

            Yield spectrum(key)
        Next
    End Function

    Private Shared Function evalMz(mass As MassIndex, adducts As MzCalculator()) As IEnumerable(Of IonIndex)
        Return adducts _
            .Select(Function(type)
                        Dim mzi As Double = type.CalcMZ(mass.exactMass)
                        Dim getIndex = mass.spectrum _
                            .Select(Function(i)
                                        Return New IonIndex With {
                                            .mz = mzi,
                                            .node = i
                                        }
                                    End Function)

                        Return getIndex
                    End Function) _
            .IteratesALL
    End Function

    Public Function BuildSearchIndex(adducts As MzCalculator()) As SpectrumReader
        Dim exactMass As MassIndex() = LoadMass(file).ToArray
        Dim mz As IonIndex() = exactMass _
            .Select(Function(mass)
                        Return evalMz(mass, adducts)
                    End Function) _
            .IteratesALL _
            .ToArray

        mzIndex = New MzIonSearch(mz, da:=Tolerance.DeltaMass(0.5))

        Return Me
    End Function

    Private Shared Iterator Function LoadMass(file As StreamPack) As IEnumerable(Of MassIndex)
        Dim files = DirectCast(file.GetObject("/massSet/"), StreamGroup) _
            .ListFiles _
            .Select(Function(f) DirectCast(f, StreamBlock)) _
            .ToArray

        For Each ref As StreamBlock In files
            Dim bcode As String = file.ReadText(ref)
            Dim mass As BDictionary = BencodeDecoder.Decode(bcode).First
            Dim index As New MassIndex

            index.name = mass!name.ToString
            index.exactMass = Val(mass!exactMass.ToString)
            index.spectrum = DirectCast(mass!spectrum, BList) _
                .Select(Function(b) Integer.Parse(b.ToString)) _
                .AsList

            Yield index
        Next
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call file.Dispose()
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
