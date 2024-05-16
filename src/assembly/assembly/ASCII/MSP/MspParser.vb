#Region "Microsoft.VisualBasic::09914fc06f7804d32b059e75bbf69d10, assembly\assembly\ASCII\MSP\MspParser.vb"

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

    '   Total Lines: 201
    '    Code Lines: 151
    ' Comment Lines: 22
    '   Blank Lines: 28
    '     File Size: 7.75 KB


    '     Module MspParser
    ' 
    '         Function: createObject, fillPrecursorInfo, incorrects, Load, parseMspPeaks
    ' 
    '         Sub: TestMissingFields
    '         Class AliasLambda
    ' 
    '             Function: getValue, getValues
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Namespace ASCII.MSP

    Module MspParser

        Private Function incorrects(metadata As NameValueCollection) As Boolean
            With metadata("PrecursorMZ")
                If .MatchPattern("(\.)?\d+/\d+") OrElse .Count(" "c) >= 1 Then
                    Return True
                Else
                    Return False
                End If
            End With
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">``*.msp``代谢组参考库文件路径</param>
        ''' <returns></returns>
        Public Iterator Function Load(path$, Optional ms2 As Boolean = True) As IEnumerable(Of MspData)
            Dim libs = path _
                .IterateAllLines _
                .Split(AddressOf StringEmpty, includes:=False) _
                .Where(Function(c) c.Length > 0)

            If path.FileLength <= 0 Then
                Call $"Target msp file: [{path}] contains no data or file not found!".Warning
                Return
            End If

            For Each reference As String() In libs.Where(Function(r) Not r.IsNullOrEmpty AndAlso r.Length > 2)
                Dim parts = reference _
                    .Split(Function(s)
                               Return s.MatchPattern("Num Peaks[:]\s*\d+", RegexICSng)
                           End Function) _
                    .ToArray
                Dim metaBlock As String() = parts(Scan0)
                Dim metadata As NameValueCollection = metaBlock _
                    .Select(Function(s)
                                Return s.GetTagValue(":", trim:=True)
                            End Function) _
                    .NameValueCollection

                If incorrects(metadata) Then
                    ' 这个数据是MS3结果，可能需要丢弃掉
                    If ms2 Then
                        Continue For
                    End If
                End If

                Dim peaksdata As ms2() = parts.Last.parseMspPeaks

                Yield metadata.createObject(peaksdata)
            Next
        End Function

        <Extension>
        Private Function parseMspPeaks(str As String()) As ms2()
            Return str _
                .Skip(1) _
                .Select(Function(s)
                            With s.Split(" "c)
                                Dim mz$ = .First
                                Dim into$ = .Second
                                Dim comment$ = .Skip(2).JoinBy(" ")

                                Return New ms2 With {
                                    .mz = mz,
                                    .intensity = into,
                                    .Annotation = comment
                                }
                            End With
                        End Function) _
                .ToArray
        End Function

        Private Class AliasLambda

            Public metadata As NameValueCollection

            Public Function getValue(ParamArray keys As String()) As String
                For Each key As String In keys
                    If metadata.ContainsKey(key) Then
                        Return metadata(key)
                    End If
                Next

                Return ""
            End Function

            Public Function getValues(ParamArray keys As String()) As String()
                Dim list As New List(Of String)

                For Each key As String In keys
                    If metadata.ContainsKey(key) Then
                        Call list.AddRange(metadata.GetValues(key))
                    End If
                Next

                Return list.ToArray
            End Function
        End Class

        <Extension>
        Private Function createObject(metadata As NameValueCollection, peaksdata As ms2()) As MspData
            Dim read As New AliasLambda With {.metadata = metadata}
            Dim metaComment As NameValueCollection = read _
                .getValue(NameOf(MspData.Comments), "Comment") _
                .ToTable
            Dim aliasName As String() = read _
                .getValues("Synonym", "Synon") _
                .Distinct _
                .ToArray

            ' join two collection data
            Call metaComment.Add(metadata)

            Dim msp As New MspData With {
                .Peaks = peaksdata,
                .Comments = metaComment,
                .DB_id = read.getValue("DB#", "ID"),
                .Formula = read.getValue(NameOf(MspData.Formula)),
                .InChIKey = read.getValue(NameOf(MspData.InChIKey)),
                .MW = Val(read.getValue(NameOf(MspData.MW)) Or read.getValue("ExactMass").AsDefault),
                .Name = read.getValue(NameOf(MspData.Name)),
                .PrecursorMZ = read.getValue(NameOf(MspData.PrecursorMZ)),
                .Synonyms = aliasName,
                .Precursor_type = read.getValue("Precursor_type"),
                .Spectrum_type = read.getValue("Spectrum_type"),
                .Instrument_type = read.getValue("Instrument_type"),
                .Instrument = read.getValue("Instrument"),
                .Collision_energy = read.getValue("Collision_energy"),
                .Ion_mode = read.getValue("Ion_mode"),
                .RetentionTime = read.getValue("RETENTIONTIME")
            }

            Return msp.fillPrecursorInfo
        End Function

        ''' <summary>
        ''' solve the missing ms2 precursor ion information
        ''' </summary>
        ''' <param name="msp"></param>
        ''' <returns></returns>
        <Extension>
        Private Function fillPrecursorInfo(msp As MspData) As MspData
            ' the name of current ion is a old xcms_id
            ' in format liked:
            '
            '   M{m/z}T{rt in second}
            '
            If msp.Name.IsPattern("M\d+T\d+(_\d+)?") Then
                ' get mz and rt in integer
                Dim mt As Double() = msp.Name _
                    .Matches("\d+") _
                    .Select(Function(a) Val(a)) _
                    .ToArray

                If msp.PrecursorMZ.StringEmpty Then
                    Dim mz2 = msp.Peaks _
                        .Select(Function(p)
                                    Return (p.mz, d:=stdNum.Abs(p.mz - mt(0)))
                                End Function) _
                        .OrderBy(Function(t) t.d) _
                        .FirstOrDefault

                    If mz2.d < 1 Then
                        msp.PrecursorMZ = mz2.mz
                    Else
                        msp.PrecursorMZ = mt(0)
                    End If
                End If
                If msp.RetentionTime.StringEmpty Then
                    msp.RetentionTime = mt.ElementAtOrDefault(1)
                End If
            End If

            Return msp
        End Function

        ''' <summary>
        ''' 测试是否还存在没有添加的字段值
        ''' </summary>
        ''' <param name="metadata"></param>
        <Extension>
        Public Sub TestMissingFields(metadata As NameValueCollection)

            If metadata.Count > 0 Then
                Throw New NotImplementedException(metadata.ToDictionary.GetJson)
            End If
        End Sub
    End Module
End Namespace
