Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Spectra

    ''' <summary>
    ''' MS2 fragment matrix
    ''' </summary>
    Public Class Library

        ''' <summary>
        ''' Fragment ID in this matrix.
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String
        ''' <summary>
        ''' 前体离子的m/z
        ''' </summary>
        ''' <returns></returns>
        Public Property PrecursorMz As Double
        ''' <summary>
        ''' 碎片的m/z
        ''' </summary>
        ''' <returns></returns>
        Public Property ProductMz As Double
        ''' <summary>
        ''' 当前的这个碎片的信号强度
        ''' </summary>
        ''' <returns></returns>
        Public Property LibraryIntensity As Double
        ''' <summary>
        ''' library name
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        Public Overrides Function ToString() As String
            Return $"[{ProductMz}, {LibraryIntensity}]"
        End Function

    End Class

    ''' <summary>
    ''' A spectra fragment with m/z and into data.
    ''' </summary>
    Public Class ms2

        ''' <summary>
        ''' Molecular fragment m/z
        ''' </summary>
        ''' <returns></returns>
        <DataFrameColumn(NameOf(mz))>
        <XmlAttribute> Public Property mz As Double
        ''' <summary>
        ''' quantity
        ''' </summary>
        ''' <returns></returns>
        <DataFrameColumn(NameOf(quantity))>
        <XmlAttribute> Public Property quantity As Double
        ''' <summary>
        ''' Relative intensity.(percentage) 
        ''' </summary>
        ''' <returns></returns>
        <DataFrameColumn(NameOf(intensity))>
        <XmlAttribute> Public Property intensity As Double

        Public Overrides Function ToString() As String
            If intensity < 1 Then
                Return $"{mz} ({Fix(intensity * 100%)}%)"
            Else
                Return $"{mz} ({Fix(intensity)}%)"
            End If
        End Function
    End Class
End Namespace