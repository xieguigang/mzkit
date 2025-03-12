#Region "Microsoft.VisualBasic::1a4ad06c0fdb8e55be8438a0b730621e, mzmath\MSEngine\Library.vb"

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

    '   Total Lines: 117
    '    Code Lines: 83 (70.94%)
    ' Comment Lines: 9 (7.69%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 25 (21.37%)
    '     File Size: 4.07 KB


    ' Class Library
    ' 
    '     Constructor: (+3 Overloads) Sub New
    '     Function: Compares, GetMetadataByID, SearchCandidates
    '     Class HashIndex
    ' 
    '         Function: GetAnnotation, GetDbXref, GetMetadata
    ' 
    '         Sub: Add
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' A reference spectrum and annotation data provider
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class Library(Of T As {INamedValue, GenericCompound})

    Dim search As AVLClusterTree(Of PeakMs2)
    Dim metadata As IMetaDb

    ReadOnly dotcutoff As Double
    ReadOnly right As Double
    ReadOnly cos As CosAlignment

    Private Class HashIndex : Implements IMetaDb

        ReadOnly metadata As New Dictionary(Of String, T)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Add(metadata As T)
            Call Me.metadata.Add(metadata.Identity, metadata)
        End Sub

        Public Function GetAnnotation(uniqueId As String) As (name As String, formula As String) Implements IMetaDb.GetAnnotation
            Dim metadata As T = Me.metadata(uniqueId)
            Dim anno = (metadata.CommonName, metadata.Formula)
            Return anno
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMetadata(uniqueId As String) As Object Implements IMetaDb.GetMetadata
            Return metadata(uniqueId)
        End Function

        Public Function GetDbXref(uniqueId As String) As Dictionary(Of String, String) Implements IMetaDb.GetDbXref
            Throw New NotImplementedException()
        End Function
    End Class

    Sub New(data As IEnumerable(Of (meta As T, spec As PeakMs2)),
            Optional dotcutoff As Double = 0.6,
            Optional right As Double = 0.5)

        Call Me.New(dotcutoff, right)

        Dim metadata = New HashIndex

        For Each ref As (meta As T, spec As PeakMs2) In data
            Call search.Add(ref.spec)
            Call metadata.Add(ref.meta)
        Next

        Me.metadata = metadata
    End Sub

    Sub New(Optional dotcutoff As Double = 0.6,
            Optional right As Double = 0.5)

        Me.dotcutoff = dotcutoff
        Me.right = right
        Me.cos = New CosAlignment(DAmethod.DeltaMass(0.3), New RelativeIntensityCutoff(0.05))
        Me.search = New AVLClusterTree(Of PeakMs2)(AddressOf Compares, Function(a) a.lib_guid)
    End Sub

    Sub New(metadb As IMetaDb, refSpec As IEnumerable(Of PeakMs2),
            Optional dotcutoff As Double = 0.6,
            Optional right As Double = 0.5)

        Me.New(dotcutoff, right)
        Me.metadata = metadb

        For Each ref As PeakMs2 In refSpec
            Call search.Add(ref)
        Next
    End Sub

    Private Function Compares(a As PeakMs2, b As PeakMs2) As Integer
        Dim cosine As Double = cos.GetScore(a.mzInto, b.mzInto)

        If cosine > dotcutoff Then
            Return 0
        ElseIf cosine > right Then
            Return 1
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' get metabolite annotation data by id reference
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMetadataByID(id As String) As T
        Return metadata.GetMetadata(id)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function SearchCandidates(sample As PeakMs2) As IEnumerable(Of AlignmentOutput)
        Dim cluster = search.Search(sample).ToArray

        Return From ref As PeakMs2
               In cluster.AsParallel
               Let alignment As AlignmentOutput = cos.CreateAlignment(sample, ref)
               Select alignment
               Order By alignment.cosine Descending
    End Function

End Class
