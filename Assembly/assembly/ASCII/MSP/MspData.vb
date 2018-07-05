Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices

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
        Public Property Precursor_type As String
        Public Property Comments As String
        Public Property Spectrum_type As String
        Public Property Instrument_type As String
        Public Property Instrument As String
        Public Property Ion_mode As String
        Public Property Collision_energy As String

        ''' <summary>
        ''' MoNA里面都主要是讲注释的信息放在<see cref="Comments"/>字段里面的。
        ''' 物质的注释信息主要是放在这个结构体之中，这个属性是对<see cref="Comments"/>
        ''' 属性的解析结果
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MetaDB As MetaData
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Comments.FillData
            End Get
        End Property

        Public Property Peaks As MSMSPeak()

        Public Overrides Function ToString() As String
            Return Name
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load(path$, Optional ms2 As Boolean = True) As IEnumerable(Of MspData)
            Return MspParser.Load(path, ms2)
        End Function
    End Class
End Namespace