Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PackLib

    ''' <summary>
    ''' object tools for read the reference spectrum data from the library file
    ''' </summary>
    Public Class SpectrumReader : Implements IDisposable

        Dim disposedValue As Boolean
        Dim file As StreamPack
        Dim mzIndex As MzIonSearch
        Dim metadata As Dictionary(Of String, String)

        ''' <summary>
        ''' mapping of <see cref="BlockNode.Id"/> to the mass index <see cref="MassIndex.name"/>
        ''' </summary>
        ReadOnly map As Dictionary(Of String, String)
        ReadOnly spectrum As New Dictionary(Of String, BlockNode)
        ''' <summary>
        ''' A id subset for filter target ions
        ''' </summary>
        ReadOnly targetSet As Index(Of String)
        ReadOnly libnames As String()
        ReadOnly da As Tolerance = Tolerance.DeltaMass(0.3)
        ReadOnly intocutoff As New RelativeIntensityCutoff(0.05)

        Public ReadOnly Property Libname(i As Integer) As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return libnames.ElementAtOrDefault(i, [default]:=$"#{i}")
            End Get
        End Property

        Default Public ReadOnly Property GetIdMap(libname As String) As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return map(libname)
            End Get
        End Property

        ''' <summary>
        ''' get the number of the spectrum in current reference library
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property nspectrum As Integer
            Get
                Return DirectCast(file.GetObject("/spectrum/"), StreamGroup) _
                    .ListFiles _
                    .Count
            End Get
        End Property

        ''' <summary>
        ''' open the database file in readonly mode
        ''' </summary>
        ''' <param name="target_uuid">
        ''' only a subset of the spectrum will be
        ''' queried if the target idset has been 
        ''' specificed
        ''' </param>
        ''' <param name="file"></param>
        Sub New(file As Stream, Optional target_uuid As String() = Nothing)
            Me.file = New StreamPack(file, [readonly]:=True)
            Me.map = Me.file.ReadText("/map.json").LoadJSON(Of Dictionary(Of String, String))
            Me.metadata = Me.file.ReadText("/metadata.json").LoadJSON(Of Dictionary(Of String, String))
            Me.targetSet = target_uuid.Indexing
            Me.libnames = Me.file.ReadText("/spectrum/libnames.txt").LineTokens

            If metadata Is Nothing Then
                metadata = New Dictionary(Of String, String)
            End If
        End Sub

        ''' <summary>
        ''' test for <see cref="GetIdMap(String)"/>
        ''' </summary>
        ''' <param name="libname"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function HasMapName(libname As String) As Boolean
            Return map.ContainsKey(libname)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAllLibNames() As IEnumerable(Of String)
            Return libnames.AsEnumerable
        End Function

        ''' <summary>
        ''' populate all spectrum which the exact mass+adducts matched 
        ''' the m/z query input.
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <returns></returns>
        Public Iterator Function QueryByMz(mz As Double) As IEnumerable(Of BlockNode)
            Dim ions = mzIndex.QueryByMz(mz).ToArray
            Dim index As IEnumerable(Of String)

            If ions.Length = 0 Then
                Return
            Else
                index = From i As IonIndex
                        In ions
                        Let i32 As Integer = i.node
                        Select i32
                        Distinct
                        Let tag As String = i32.ToString
                        Select tag
            End If

            For Each key As String In index
                Yield GetSpectrum(key)
            Next
        End Function

        Public Iterator Function ListAllSpectrumId() As IEnumerable(Of String)
            Dim ls = DirectCast(file.GetObject("/spectrum/"), StreamGroup) _
              .ListFiles(safe:=True) _
              .Where(Function(f) TypeOf f Is StreamBlock) _
              .Select(Function(f) DirectCast(f, StreamBlock))

            For Each file As StreamBlock In ls
                Yield file.referencePath.FileName.BaseName
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="key">An integer pointer in string type</param>
        ''' <returns></returns>
        Public Function GetSpectrum(key As String) As BlockNode
            If Not spectrum.ContainsKey(key) Then
                Dim path As String = $"/spectrum/{key.Last}/{key}.dat"
                Dim file As Stream = Me.file.OpenBlock(path)
                Dim spectrumNode = NodeBuffer.Read(New BinaryDataReader(file))

                ' the reference spectrum data needs to be centroid
                spectrumNode.centroid = spectrumNode.centroid _
                    .Centroid(da, intocutoff) _
                    .ToArray

                ' add to in-memory cache
                SyncLock spectrum
                    Call spectrum.Add(key, spectrumNode)
                End SyncLock
            End If

            Return spectrum(key)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetSpectrum(pointer As Integer) As BlockNode
            Return GetSpectrum(key:=pointer.ToString)
        End Function

        Public Shared Function GetSpectrum(node As BlockNode, Optional file As String = Nothing) As PeakMs2
            Return New PeakMs2 With {
                .mzInto = node.centroid,
                .lib_guid = node.Id,
                .intensity = node.centroid.Select(Function(m) m.intensity).Sum,
                .mz = node.mz.FirstOrDefault,
                .rt = node.rt,
                .scan = node.Id,
                .file = file
            }
        End Function

        ''' <summary>
        ''' Load all asspciated spectrum data inside target <paramref name="mass"/> index
        ''' </summary>
        ''' <param name="mass"></param>
        ''' <returns></returns>
        Public Iterator Function GetSpectrum(mass As MassIndex) As IEnumerable(Of PeakMs2)
            For Each i As Integer In mass.spectrum
                Dim node As BlockNode = GetSpectrum(key:=i.ToString)
                Dim spectrum As PeakMs2 = GetSpectrum(node, file:=mass.name)

                Yield spectrum
            Next
        End Function

        ''' <summary>
        ''' evaluate the theoretically m/z value based on the 
        ''' exact mass and the given adducts type
        ''' </summary>
        ''' <param name="mass"></param>
        ''' <param name="adducts"></param>
        ''' <returns></returns>
        Private Shared Function evalMz(mass As MassIndex, adducts As MzCalculator()) As IEnumerable(Of IonIndex)
            Return adducts _
                .Select(Function(type)
                            Dim mzi As Double = type.CalcMZ(mass.exactMass)

                            If mzi <= 0 Then
                                Return New IonIndex() {}
                            Else
                                Return mass.spectrum _
                                    .Select(Function(i)
                                                Return New IonIndex With {
                                                    .mz = mzi,
                                                    .node = i
                                                }
                                            End Function)
                            End If
                        End Function) _
                .IteratesALL
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="println">printer function for verbose debug echo, value could be nothing</param>
        ''' <param name="adducts"></param>
        ''' <returns></returns>
        Public Function BuildSearchIndex(println As Action(Of Object), ParamArray adducts As MzCalculator()) As SpectrumReader
            ' the target reference spectrum has already been filter
            ' in the loadMass function.
            Dim exactMass As MassIndex() = LoadMass().ToArray
            Dim mz As IonIndex() = exactMass _
                .Select(Function(mass)
                            Return evalMz(mass, adducts)
                        End Function) _
                .IteratesALL _
                .ToArray

            If println Is Nothing Then
                ' mute
                println = Sub()
                              ' do nothing for mute
                          End Sub
            End If

            If exactMass.IsNullOrEmpty Then
                Dim msg As String = ThrowNoMassIndex()

                Call msg.Warning
                Call println(msg.LineTokens)
            Else
                Call println($"get {mz.Length} ion targets based on the {exactMass.Length} metabolite targets!")
                Call println(mz.Select(Function(i) i.ToString).ToArray)
            End If

            mzIndex = New MzIonSearch(mz, da:=Tolerance.DeltaMass(0.5))

            Return Me
        End Function

        ''' <summary>
        ''' get error message
        ''' </summary>
        ''' <returns></returns>
        Private Function ThrowNoMassIndex() As String
            Dim err_msg As New StringBuilder("There is no ion mass index was loaded from this reference library stream!")
            Dim hasIdTargets As Boolean = targetSet.Count > 0

            If hasIdTargets Then
                Call err_msg.AppendLine("Please check of the target uuid set is correct map to the reference id name?")
                Call err_msg.AppendLine($"Peeks part of the target uuid set: {targetSet.Objects.Take(13).JoinBy(", ")}...")
                Call err_msg.AppendLine($"Peeks part of the ion spectral data reference id in this library: {GetMassFiles.Take(13).Select(Function(f) f.fileName.BaseName).JoinBy(", ")}")
            Else
                Call err_msg.AppendLine("No reference metabolite ion spectral data in this reference library?")
            End If

            Return (err_msg.ToString)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' the file name should be the metabolite unique id, and used as the 
        ''' tag prefix with delimiter ``|``, example as: 
        ''' 
        ''' ```
        ''' HMDB0000001|libname.bcode
        ''' ```
        ''' </remarks>
        Private Function GetMassFiles() As IEnumerable(Of StreamBlock)
            Return DirectCast(file.GetObject("/massSet/"), StreamGroup) _
                .ListFiles(safe:=True) _
                .Where(Function(f) TypeOf f Is StreamBlock) _
                .Select(Function(f) DirectCast(f, StreamBlock))
        End Function

        ''' <summary>
        ''' load all of the metabolite index from the library file 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' data could be filter via a given <see cref="targetSet"/>
        ''' </remarks>
        Public Iterator Function LoadMass() As IEnumerable(Of MassIndex)
            Dim files As StreamBlock() = GetMassFiles().ToArray
            Dim hasIdTargets As Boolean = targetSet.Count > 0

            For Each ref As StreamBlock In files
                If hasIdTargets Then
                    Dim ref_id As String = ref.fileName _
                        .BaseName _
                        .Split("|"c) _
                        .First

                    ' only a subset of the spectrum will be
                    ' queried if the target idset has been 
                    ' specificed
                    If Not ref_id Like targetSet Then
                        Continue For
                    End If
                End If

                Dim bcode As String = file.ReadText(ref)
                Dim mass As BDictionary = BencodeDecoder.Decode(bcode).First
                Dim index As New MassIndex
                Dim formula As BString = Nothing

                index.name = mass!name.ToString
                index.exactMass = Val(mass!exactMass.ToString)
                index.spectrum = DirectCast(mass!spectrum, BList) _
                    .Select(Function(b) Integer.Parse(b.ToString)) _
                    .AsList

                If mass.TryGetValue("formula", formula) Then
                    index.formula = formula.ToString
                End If

                Yield index
            Next
        End Function

        Public Overrides Function ToString() As String
            Dim name As String = metadata.TryGetValue("name", [default]:="Spectrum Reference Library")
            Dim n_mass As Integer = DirectCast(file.GetObject("/massSet/"), StreamGroup).files.Length
            Dim n_spectrum As Integer = nspectrum

            Return $"[{name}] {n_mass} metabolites, {n_spectrum} spectrum"
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

        ''' <summary>
        ''' close the input file
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace