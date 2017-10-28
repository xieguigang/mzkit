Imports System.Collections.Specialized
Imports System.Data.Linq.Mapping

Namespace ASCII.MSP

    Public Class MspData

        Public Property Name As String
        Public Property Synonyms As String()

        ''' <summary>
        ''' ``DB#``
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="DB#")>
        Public Property DB_id As String
        Public Property InChIKey As String
        Public Property MW As Double
        Public Property Formula As String
        Public Property PrecursorMZ As String
        Public Property Comments As String

        Public ReadOnly Property MetaDB As MetaData
            Get
                Return Comments.FillData
            End Get
        End Property

        Public Property Peaks As MSMSPeak()

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Private Shared Function incorrects(metadata As NameValueCollection) As Boolean
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
        Public Shared Iterator Function Load(path$, Optional ms2 As Boolean = True) As IEnumerable(Of MspData)
            Dim libs = path _
                .IterateAllLines _
                .Split(AddressOf StringEmpty, includes:=False) _
                .Where(Function(c) c.Length > 0)

            For Each reference As String() In libs
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
                                With s.Split(" "c)
                                    Return New MSMSPeak(
                                        mz:= .First,
                                        intensity:= .Last)
                                End With
                            End Function) _
                    .ToArray
                Dim getValue = Function(key$)
                                   Return metadata(key)
                               End Function

                Dim msp As New MspData With {
                    .Peaks = peaksdata,
                    .Comments = getValue(NameOf(MspData.Comments)),
                    .DB_id = getValue("DB#"),
                    .Formula = getValue(NameOf(MspData.Formula)),
                    .InChIKey = getValue(NameOf(MspData.InChIKey)),
                    .MW = getValue(NameOf(MspData.MW)),
                    .Name = getValue(NameOf(MspData.Name)),
                    .PrecursorMZ = getValue(NameOf(MspData.PrecursorMZ)),
                    .Synonyms = metadata.GetValues("Synonym")
                }

                Yield msp
            Next
        End Function
    End Class
End Namespace