Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace ASCII.MSP

    Public Class MspData

        Public Property Name As String
        Public Property Synonym As String

        <Column(Name:="DB#")>
        Public Property DBId As String
        Public Property InChIKey As String
        Public Property MW As Double
        Public Property Formula As String
        Public Property PrecursorMZ As Double
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path">``*.msp``代谢组参考库文件路径</param>
        ''' <returns></returns>
        Public Shared Iterator Function Load(path$) As IEnumerable(Of MspData)
            Dim file$() = path.ReadAllLines
            Dim libs = file _
                .Split(AddressOf StringEmpty, DelimiterLocation.NotIncludes) _
                .Where(Function(c) c.Length > 0) _
                .ToArray

            For Each reference As String() In libs
                Dim parts = reference _
                    .Split(Function(s)
                               Return s.MatchPattern("Num Peaks[:]\s*\d+", RegexICSng)
                           End Function) _
                    .ToArray
                Dim metadata As Dictionary(Of NamedValue(Of String)) =
                    parts _
                    .First _
                    .Select(Function(s) s.GetTagValue(":", trim:=True)) _
                    .ToDictionary
                Dim peaksdata As MSMSPeak() =
                    parts _
                    .Last _
                    .Select(Function(s)
                                With s.Split(" "c)
                                    Return New MSMSPeak(
                                        mz:= .First,
                                        intensity:= .Last)
                                End With
                            End Function) _
                    .ToArray
                Dim getValue = Function(key$)
                                   Return metadata.TryGetValue(key).Value
                               End Function

                Dim msp As New MspData With {
                    .Peaks = peaksdata,
                    .Comments = getValue(NameOf(MspData.Comments)),
                    .DBId = getValue("DB#"),
                    .Formula = getValue(NameOf(MspData.Formula)),
                    .InChIKey = getValue(NameOf(MspData.InChIKey)),
                    .MW = getValue(NameOf(MspData.MW)),
                    .Name = getValue(NameOf(MspData.Name)),
                    .PrecursorMZ = getValue(NameOf(MspData.PrecursorMZ)),
                    .Synonym = getValue(NameOf(MspData.Synonym))
                }

                Yield msp
            Next
        End Function
    End Class
End Namespace