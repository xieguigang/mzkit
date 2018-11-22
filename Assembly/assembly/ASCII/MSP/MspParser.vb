Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

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
                Dim metadata As NameValueCollection =
                    parts _
                    .First _
                    .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                    .NameValueCollection

                If incorrects(metadata) Then
                    ' 这个数据是MS3结果，可能需要丢弃掉
                    If ms2 Then
                        Continue For
                    End If
                End If

                Dim peaksdata As MSMSPeak() =
                    parts _
                    .Last _
                    .Skip(1) _
                    .Select(Function(s)
                                With Tokenizer.CharsParser(s:=s, delimiter:=" "c)
                                    Dim mz$ = .First
                                    Dim into$ = .Second
                                    Dim comment$ = .ElementAtOrDefault(2)

                                    Return New MSMSPeak(mz:=mz, intensity:=into, comment:=comment)
                                End With
                            End Function) _
                    .ToArray

                Yield metadata.createObject(peaksdata)
            Next
        End Function

        <Extension>
        Private Function createObject(metadata As NameValueCollection, peaksdata As MSMSPeak()) As MspData
            Dim getValue = Function(key$)
                               If metadata.ContainsKey(key) Then
                                   Dim value = metadata(key)
                                   ' metadata.Remove(key)
                                   Return value
                               Else
                                   Return ""
                               End If
                           End Function

            Dim msp As New MspData With {
                .Peaks = peaksdata,
                .Comments = getValue(NameOf(MspData.Comments)) Or getValue("Comment").AsDefault,
                .DB_id = getValue("DB#"),
                .Formula = getValue(NameOf(MspData.Formula)),
                .InChIKey = getValue(NameOf(MspData.InChIKey)),
                .MW = Val(getValue(NameOf(MspData.MW)) Or getValue("ExactMass").AsDefault),
                .Name = getValue(NameOf(MspData.Name)),
                .PrecursorMZ = getValue(NameOf(MspData.PrecursorMZ)),
                .Synonyms = metadata.GetValues("Synonym") Or {getValue("Synon")}.AsDefault,
                .Precursor_type = getValue("Precursor_type"),
                .Spectrum_type = getValue("Spectrum_type"),
                .Instrument_type = getValue("Instrument_type"),
                .Instrument = getValue("Instrument"),
                .Collision_energy = getValue("Collision_energy"),
                .Ion_mode = getValue("Ion_mode")
            }

            'If metadata.ContainsKey("Synonym") Then
            '    metadata.Remove("Synonym")
            'End If

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