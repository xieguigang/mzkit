﻿#Region "Microsoft.VisualBasic::ce1d0a70982331febbcc1b8c708d039d, assembly\SpectrumTree\Pack\SpectrumPack.vb"

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

    '   Total Lines: 133
    '    Code Lines: 81 (60.90%)
    ' Comment Lines: 29 (21.80%)
    '    - Xml Docs: 55.17%
    ' 
    '   Blank Lines: 23 (17.29%)
    '     File Size: 5.32 KB


    '     Class SpectrumPack
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: PathName
    ' 
    '         Sub: (+2 Overloads) Dispose, Push, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Serialization.Bencoding
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace PackLib

    ''' <summary>
    ''' A data pack of the reference spectrum data which 
    ''' is indexed via the formula data
    ''' </summary>
    ''' <remarks>
    ''' A reference library data model in hds file pack format
    ''' </remarks>
    Public Class SpectrumPack : Implements IDisposable, IReferencePack

        ''' <summary>
        ''' Each block is a collection of the metabolite spectrum
        ''' </summary>
        ReadOnly treePack As New InternalFileSystem(0)
        ReadOnly massSet As New Dictionary(Of String, MassIndex)
        ReadOnly file As StreamPack

        Private disposedValue As Boolean

        Sub New(file As Stream)
            Me.file = New StreamPack(file, meta_size:=64 * 1024 * 1024)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function PathName(name As String) As String
            Return Strings.Trim(name).Replace("\", "_").Replace("/", "_").Replace(""""c, "").Replace("?", ".").Replace("*", ".").Trim
        End Function

        ''' <summary>
        ''' the spectrum is reference to the given <paramref name="uuid"/> via the <see cref="PeakMs2.lib_guid"/>
        ''' </summary>
        ''' <param name="uuid"></param>
        ''' <param name="formula"></param>
        ''' <param name="spectrum"></param>
        Public Sub Push(uuid As String, formula As String, spectrum As PeakMs2) Implements IReferencePack.Push
            Dim index As MassIndex

            If Not massSet.ContainsKey(uuid) Then
                Dim f As Formula = FormulaScanner.ScanFormula(formula)

                Call massSet.Add(uuid, New MassIndex With {
                   .exactMass = f.ExactMass,
                   .name = uuid,
                   .formula = formula
                })
            End If

            If spectrum.lib_guid Is Nothing Then
                Throw New NullReferenceException($"no unique reference id(PeakMs2.lib_guid) of your reference spectrum data: {uuid} ({formula})!")
            End If

            index = massSet(uuid)
            index.spectrum.Add(treePack.Append(spectrum, isMember:=False))
        End Sub

        Private Sub Save()
            Dim i As Integer = 0
            Dim map As New Dictionary(Of String, String)

            For Each mass As MassIndex In massSet.Values
                Dim path As String = $"/massSet/{mass.name}.bcode"
                Dim bcode As String = mass.ToBEncodeString

                Call file.WriteText(bcode, path, allocate:=False)

                For Each p As Integer In mass.spectrum
                    ' 20230407
                    ' this may cause the possible data missing
                    ' problem at here
                    If Not map.ContainsKey(treePack(p).Id) Then
                        Call map.Add(treePack(p).Id, mass.name)
                    End If
                Next
            Next

            Call file.WriteText(map.GetJson, "/map.json")

            For Each spectrum As BlockNode In treePack
                Dim path As String = $"/spectrum/{i.ToString.Last}/{i}.dat"
                Dim buf As StreamBuffer = file.OpenBlock(path)

                i += 1

                Using writer As New BinaryDataWriter(buf, Encodings.ASCII) With {
                    .ByteOrder = ByteOrder.LittleEndian
                }
                    Call NodeBuffer.Write(spectrum, file:=writer)
                    Call writer.Flush()
                End Using
            Next
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call Save()
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
End Namespace
