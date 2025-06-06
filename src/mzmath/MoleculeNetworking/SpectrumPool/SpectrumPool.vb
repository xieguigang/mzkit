﻿#Region "Microsoft.VisualBasic::8ef7fd80c1a3658ff0c1e844acb1aed2, mzmath\MoleculeNetworking\SpectrumPool\SpectrumPool.vb"

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

    '   Total Lines: 397
    '    Code Lines: 217 (54.66%)
    ' Comment Lines: 123 (30.98%)
    '    - Xml Docs: 68.29%
    ' 
    '   Blank Lines: 57 (14.36%)
    '     File Size: 14.72 KB


    '     Class SpectrumPool
    ' 
    '         Properties: ClusterInfo
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Create, GetFileSystem, GetSpectral, Open, ToString
    ' 
    '         Sub: Add, AddInternal, Commit, (+3 Overloads) DirectPush, (+2 Overloads) Dispose
    '              eval_score
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis

Namespace PoolData

    ''' <summary>
    ''' A pool storage object for the spectrum tree data
    ''' </summary>
    ''' <remarks>
    ''' A multiple tree branch clustering of the ms spectrum object 
    ''' </remarks>
    Public Class SpectrumPool : Implements IDisposable

        Dim classTree As New Dictionary(Of String, SpectrumPool)
        Dim zeroBlock As SpectrumPool

        ''' <summary>
        ''' the representative spectrum of current spectrum tree node
        ''' </summary>
        Dim representative As PeakMs2
        ''' <summary>
        ''' index of the spectrum data in current cluster node
        ''' </summary>
        Dim metadata As MetadataProxy
        ''' <summary>
        ''' the first element in the hash list
        ''' </summary>
        Dim rootId As String

        Dim disposedValue As Boolean

        ReadOnly fs As PoolFs
        ReadOnly handle As String

        Public Const tags As String = "123456789abcdef"

        ''' <summary>
        ''' Get child node by a given key name
        ''' </summary>
        ''' <param name="key">
        ''' + 0, z, zero means z tree
        ''' + other chars in hex <see cref="tags"/> means corresponding child
        ''' </param>
        ''' <returns>
        ''' returns null if not found
        ''' </returns>
        Default Public ReadOnly Property NextChild(key As String) As SpectrumPool
            Get
                If key = "0" OrElse key = "z" OrElse key = "zero" Then
                    Return zeroBlock
                Else
                    If classTree.ContainsKey(key) Then
                        Return classTree(key)
                    Else
                        Return Nothing
                    End If
                End If
            End Get
        End Property

        ''' <summary>
        ''' get the <see cref="Metadata"/> collection of each cluster
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ClusterInfo As IEnumerable(Of Metadata)
            Get
                Return metadata.AllClusterMembers
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="fs">
        ''' the pool filesystem storage
        ''' </param>
        Public Sub New(fs As PoolFs, path As String)
            Me.fs = fs
            Me.handle = path.StringReplace("/{2,}", "/")
            Me.metadata = fs.LoadMetadata(path)
            Me.rootId = fs.FindRootId(path)

            ' the mysql id is started from 1
            ' so zero means nothing at here, set the
            ' rootid string to nothing for avoid spectrum
            ' data loading process in current constructor
            ' function
            If rootId = "0" Then
                rootId = Nothing
            End If

            ' 20230709 lazy laoding, the spectrum pool object could be
            ' created in the add method in lazy mode. no needs for create
            ' all spectrum pool at once in this constructor
            ' 
            'For Each dir As String In fs.GetTreeChilds(path)
            '    If dir.BaseName = "z" Then
            '        zeroBlock = New SpectrumPool(fs, dir)
            '    Else
            '        classTree.Add(dir.BaseName, New SpectrumPool(fs, dir))
            '    End If
            'Next

            If Not rootId.StringEmpty Then
                ' first element always the root element
                ' read as representive spectrum data
                representative = fs.ReadSpectrum(metadata(rootId))
            End If
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetFileSystem() As PoolFs
            Return fs
        End Function

        ''' <summary>
        ''' add spectrum into current molecular networking cluster tree data
        ''' </summary>
        ''' <param name="spectrum"></param>
        Public Sub Add(spectrum As PeakMs2, Optional debug As Boolean = False)
            If Not fs.CheckExists(spectrum) Then
                If debug Then
                    Call AddInternal(spectrum)
                Else
                    Try
                        Call AddInternal(spectrum)
                    Catch ex As Exception
                        Call App.LogException(ex)
                    End Try
                End If
            Else
                Call VBDebugger.EchoLine($"spectrum_exists: {spectrum.ToString}")
            End If
        End Sub

        ''' <summary>
        ''' add the spectrum into current cluster node directly
        ''' </summary>
        ''' <param name="spectrum"></param>
        Public Shared Sub DirectPush(spectrum As PeakMs2, fs As PoolFs, pool As MetadataProxy, root As PeakMs2)
            Dim score As AlignmentOutput = fs.GetScore(spectrum, root)
            Dim PIScore As Double
            Dim pval As Double

            Call eval_score(score, PIScore, pval)
            Call DirectPush(pool, fs, spectrum, PIScore, score, pval)
        End Sub

        ''' <summary>
        ''' add the spectrum into current cluster node directly
        ''' </summary>
        ''' <param name="spectrum"></param>
        ''' <param name="nodeId">
        ''' the target cluster node id
        ''' </param>
        ''' 
        Public Shared Sub DirectPush(spectrum As PeakMs2, fs As PoolFs, nodeId As Integer)
            Dim pool = fs.LoadMetadata(nodeId)
            Dim root = fs.ReadSpectrum(pool(pool.RootId))

            Call DirectPush(spectrum, fs, pool, root)
        End Sub

        ''' <summary>
        ''' add the spectrum into current cluster node directly
        ''' </summary>
        ''' <param name="spectrum"></param>
        ''' <param name="PIScore"></param>
        ''' <param name="score"></param>
        ''' <param name="pval"></param>
        Public Shared Sub DirectPush(pool As MetadataProxy,
                                     fs As PoolFs,
                                     spectrum As PeakMs2,
                                     PIScore As Double,
                                     score As AlignmentOutput,
                                     pval As Double)
            ' in current class node
            pool.Add(spectrum.lib_guid, fs.WriteSpectrum(spectrum))
            pool.Add(spectrum.lib_guid, PIScore, score, pval)
        End Sub

        Public Shared Sub eval_score(score As AlignmentOutput, <Out> ByRef PIScore As Double, <Out> ByRef pval As Double)
            Static zero As Double() = New Double() {.0, .0, .0, .0}

            PIScore = score.forward *
                    score.reverse *
                    score.jaccard *
                    score.entropy

            pval = t.Test(New Double() {
                score.forward, score.reverse, score.jaccard, score.entropy + 0.000000000000001
            }, zero, Hypothesis.TwoSided).Pvalue
        End Sub

        Private Sub AddInternal(spectrum As PeakMs2)
            Dim score As AlignmentOutput
            Dim PIScore As Double
            Dim pval As Double
            Dim is_root As Boolean = False

            If representative Is Nothing Then
                representative = spectrum
                is_root = True
                rootId = spectrum.lib_guid
                score = Nothing
                PIScore = 1
                pval = 0
                VBDebugger.EchoLine($"create_root@{ToString()}: {spectrum.lib_guid}")
            Else
                score = fs.GetScore(spectrum, representative)
                eval_score(score, PIScore, pval)
            End If

            If score Is Nothing OrElse PIScore > fs.level Then
                Call DirectPush(metadata, fs, spectrum, PIScore, score, pval)

                If is_root Then
                    metadata.SetRootId(metadata(spectrum.lib_guid).block.position)
                End If

                VBDebugger.EchoLine($"join_pool@{ToString()}: {spectrum.lib_guid}")
            ElseIf PIScore <= 0 Then
                If zeroBlock Is Nothing Then
                    zeroBlock = New SpectrumPool(fs, handle & $"/z/")
                End If

                Call zeroBlock.AddInternal(spectrum)
            Else
                Dim t As Double = fs.splitDelta
                Dim i As Integer = 0

                Do While t <= fs.level
                    If PIScore <= t Then
                        Dim key As String = tags(i)

                        If Not classTree.ContainsKey(key) Then
                            If metadata.Depth > 2048 Then
                                ' 20230629
                                ' exit of current add method when stack size is greater than 2048
                                ' for avoid the possible stack overflow problem
                                Return
                            End If

                            Call classTree.Add(key, New SpectrumPool(fs, handle & $"/{key}/"))
                        End If

                        Call classTree(key).AddInternal(spectrum)

                        Return
                    Else
                        t += fs.splitDelta
                        i += 1
                    End If
                Loop
            End If
        End Sub

        ''' <summary>
        ''' Find the spectra object from this function if the cache is not hit
        ''' </summary>
        ''' <param name="guid"></param>
        ''' <returns></returns>
        Public Function GetSpectral(guid As String) As PeakMs2
            If representative IsNot Nothing AndAlso representative.lib_guid = guid Then
                Return representative
            End If

            If metadata.HasGuid(guid) Then
                Dim p As Metadata = metadata(guid)
                Dim data As PeakMs2 = fs.ReadSpectrum(p)

                Return data
            Else
                For Each tag As String In classTree.Keys
                    Dim sp As PeakMs2 = classTree(tag).GetSpectral(guid)

                    If Not sp Is Nothing Then
                        Return sp
                    End If
                Next
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' open root folder
        ''' </summary>
        ''' <param name="link">
        ''' ### for local filesystem
        ''' 
        ''' contains multiple files:
        ''' 
        ''' 1. cluster.pack  contains the metadata and structure information of the cluster tree
        ''' 2. spectrum.dat  contains the spectrum data
        ''' 
        ''' ### for web filesystem
        ''' 
        ''' </param>
        ''' <param name="level"></param>
        ''' <param name="split"></param>
        ''' <returns></returns>
        Public Shared Function Create(link As String,
                                      Optional level As Double = 0.85,
                                      Optional split As Integer = 3,
                                      Optional name As String = "no_named",
                                      Optional desc As String = "no_information") As SpectrumPool

            Dim fs As PoolFs = PoolFs.CreateAuto(link, level, split, name, desc)
            Dim pool As New SpectrumPool(fs, "/")

            Call fs.SetLevel(level, split)
            Call fs.SetScore(0.3, 0.05)

            Return pool
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="link"></param>
        ''' <param name="model_id"></param>
        ''' <param name="score">
        ''' WARNING: this optional parameter will overrides the mode score 
        ''' level when this parameter has a positive numeric value in 
        ''' range ``(0,1]``.
        ''' </param>
        ''' <returns></returns>
        Public Shared Function Open(link As String,
                                    Optional model_id As String = Nothing,
                                    Optional score As Double? = Nothing) As SpectrumPool

            Dim fs As PoolFs = PoolFs.OpenAuto(link, model_id)
            Dim pool As New SpectrumPool(fs, "/")

            If score IsNot Nothing AndAlso
                score > 0 AndAlso
                score < 1 Then

                Call fs.SetLevel(score, fs.split)
            Else
                Call fs.SetLevel(fs.level, fs.split)
            End If

            Call fs.SetScore(0.3, 0.05)

            Return pool
        End Function

        Public Overrides Function ToString() As String
            Return $"{handle}..."
        End Function

        Public Sub Commit()
            Call fs.SetRootId(handle, rootId)
            Call fs.CommitMetadata(handle, metadata)

            For Each label As String In classTree.Keys
                Call classTree(label).Commit()
            Next

            If Not zeroBlock Is Nothing Then
                Call zeroBlock.Commit()
            End If
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call Commit()
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
