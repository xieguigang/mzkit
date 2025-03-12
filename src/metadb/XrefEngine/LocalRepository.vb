#Region "Microsoft.VisualBasic::75855fad37ca5925612501596d653f8b, metadb\XrefEngine\LocalRepository.vb"

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

    '   Total Lines: 179
    '    Code Lines: 124 (69.27%)
    ' Comment Lines: 27 (15.08%)
    '    - Xml Docs: 59.26%
    ' 
    '   Blank Lines: 28 (15.64%)
    '     File Size: 7.32 KB


    ' Class LocalRepository
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: CreateDb_Xrefs, GetAnnotation, GetDbXref, GetMetadata, ReadMetadata
    '               SetIdMapping
    ' 
    '     Sub: (+2 Overloads) Dispose, LoadOffset
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.CrossReference
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.JSON
Imports metadata = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaLib

''' <summary>
''' read only metabolite annotation information data repository
''' </summary>
''' <remarks>
''' implements of <see cref="IMetaDb"/>, use the <see cref="RepositoryWriter"/> for create the annotation database.
''' </remarks>
Public Class LocalRepository : Implements IDisposable, IMetaDb

    ''' <summary>
    ''' the repository file stream data
    ''' </summary>
    ReadOnly s As StreamPack
    ReadOnly offset As New Dictionary(Of String, (id$, BufferRegion))
    ReadOnly cache As New Dictionary(Of String, metadata)
    ReadOnly cacheXrefs As New Dictionary(Of String, Dictionary(Of String, String))
    ReadOnly blocks As New Dictionary(Of String, Stream)

    ''' <summary>
    ''' used for mapping from reference id to metabolite id
    ''' </summary>
    Dim mapping As Dictionary(Of String, String)
    Dim disposedValue As Boolean

    Sub New(file As Stream)
        s = New StreamPack(file, [readonly]:=True)

        Call LoadOffset()
    End Sub

    Private Sub LoadOffset()
        Dim offsetFiles = RepositoryWriter.OffsetFiles(s).ToArray

        For Each indexfile As StreamBlock In offsetFiles
            Dim file As MemoryStream = s.OpenBlock(indexfile, loadMemory:=True)
            Dim index As Dictionary(Of String, BufferRegion) = MsgPackSerializer.Deserialize(GetType(Dictionary(Of String, BufferRegion)), file)
            Dim id As String = indexfile.fileName.BaseName
            Dim path As String = RepositoryWriter.BlockFile(Integer.Parse(id))

            For Each offset As KeyValuePair(Of String, BufferRegion) In index
                Call Me.offset.Add(offset.Key, (id, offset.Value))
            Next

            Call blocks.Add(id, s.OpenFile(path, FileMode.Open, FileAccess.Read))
        Next
    End Sub

    Public Function GetAnnotation(uniqueId As String) As (name As String, formula As String) Implements IMetaDb.GetAnnotation
        Dim metadata As metadata = GetMetadata(uniqueId)
        Dim ref = (metadata.name, metadata.formula)

        Return ref
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uniqueId"></param>
    ''' <returns>the metabolite annotation data model <see cref="metadata"/>.</returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMetadata(uniqueId As String) As Object Implements IMetaDb.GetMetadata
        Return cache.ComputeIfAbsent(uniqueId,
             lazyValue:=Function(id)
                            Return ReadMetadata(id)
                        End Function)
    End Function

    Public Function SetIdMapping(maps As Dictionary(Of String, String))
        mapping = maps
        Return Me
    End Function

    Private Function ReadMetadata(id As String) As metadata
        If Not offset.ContainsKey(id) Then
            If mapping Is Nothing Then
                Return Nothing
            End If
            If mapping.ContainsKey(id) Then
                id = mapping(id)
            End If
            If Not offset.ContainsKey(id) Then
                Return Nothing
            End If
        End If

        Dim ref = offset(id)
        Dim s As Stream = blocks(ref.id)
        Dim buf As Byte() = New Byte(ref.Item2.size - 1) {}

        Call s.Seek(ref.Item2.position, SeekOrigin.Begin)
        Call s.Read(buf, Scan0, buf.Length)

        Dim json_str As String = Encoding.UTF8.GetString(buf)
        Dim metadata As metadata = json_str.LoadJSON(Of metadata)

        Return metadata
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetDbXref(uniqueId As String) As Dictionary(Of String, String) Implements IMetaDb.GetDbXref
        Return cacheXrefs.ComputeIfAbsent(uniqueId,
            lazyValue:=Function(id)
                           Return CreateDb_Xrefs(id)
                       End Function)
    End Function

    Private Function CreateDb_Xrefs(id As String) As Dictionary(Of String, String)
        Dim metadata As metadata = GetMetadata(id)
        Dim db_xrefs As New Dictionary(Of String, String)()
        Dim xrefs As xref = metadata.xref

        If Not xrefs.extras.IsNullOrEmpty Then
            For Each item As KeyValuePair(Of String, String()) In xrefs.extras
                Call db_xrefs.Add(item.Key, item.Value.FirstOrDefault)
            Next
        End If

        Call db_xrefs.Add(NameOf(xrefs.CAS), xrefs.CAS.FirstOrDefault)
        Call db_xrefs.Add(NameOf(xrefs.chebi), xrefs.chebi)
        Call db_xrefs.Add(NameOf(xrefs.ChEMBL), xrefs.ChEMBL)
        Call db_xrefs.Add(NameOf(xrefs.chemspider), xrefs.chemspider)
        Call db_xrefs.Add(NameOf(xrefs.ChemIDplus), xrefs.ChemIDplus)
        Call db_xrefs.Add(NameOf(xrefs.DrugBank), xrefs.DrugBank)
        Call db_xrefs.Add(NameOf(xrefs.foodb), xrefs.foodb)
        Call db_xrefs.Add(NameOf(xrefs.HMDB), xrefs.HMDB)
        Call db_xrefs.Add(NameOf(xrefs.KEGG), xrefs.KEGG)
        Call db_xrefs.Add(NameOf(xrefs.KEGGdrug), xrefs.KEGGdrug)
        Call db_xrefs.Add(NameOf(xrefs.KNApSAcK), xrefs.KNApSAcK)
        Call db_xrefs.Add(NameOf(xrefs.lipidmaps), xrefs.lipidmaps)
        Call db_xrefs.Add(NameOf(xrefs.MeSH), xrefs.MeSH)
        Call db_xrefs.Add(NameOf(xrefs.MetaCyc), xrefs.MetaCyc)
        Call db_xrefs.Add(NameOf(xrefs.metlin), xrefs.metlin)
        Call db_xrefs.Add(NameOf(xrefs.pubchem), xrefs.pubchem)
        Call db_xrefs.Add(NameOf(xrefs.Wikipedia), xrefs.Wikipedia)

        Call db_xrefs.Add(NameOf(xrefs.InChIkey), xrefs.InChIkey)
        Call db_xrefs.Add(NameOf(xrefs.InChI), xrefs.InChI)
        Call db_xrefs.Add(NameOf(xrefs.SMILES), xrefs.SMILES)

        Return db_xrefs
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call s.Dispose()
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
