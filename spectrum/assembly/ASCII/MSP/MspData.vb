Imports System.Data.Linq.Mapping

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
        ''' <param name="msp$">``*.msp``代谢组参考库文件路径</param>
        ''' <returns></returns>
        Public Shared Iterator Function Load(msp$) As IEnumerable(Of MspData)
            Dim file$() = msp.ReadAllLines
            Dim libs = file _
                .Split(AddressOf StringEmpty, DelimiterLocation.NotIncludes) _
                .Where(Function(c) c.Length = 0) _
                .ToArray

            For Each metabolite In libs
                Dim parts = metabolite.Split(Function(s) s.MatchPattern("Num Peaks[:]\s*\d+", RegexICSng))
                Dim metadata = parts.First
                Dim peaksdata = parts.Last

            Next
        End Function
    End Class
End Namespace