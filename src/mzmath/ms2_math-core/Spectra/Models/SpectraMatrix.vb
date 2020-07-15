Imports System.Runtime.CompilerServices
Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Spectra

    ''' <summary>
    ''' The Xml data model of <see cref="Spectra.LibraryMatrix"/>
    ''' </summary>
    Public Class SpectraMatrix : Implements Enumeration(Of ms2)
        Implements INamedValue

        <XmlAttribute>
        Public Property title As String Implements IKeyedEntity(Of String).Key
        Public Property meta As NamedValue()

        <XmlElement>
        Public Property matrix As ms2()

        ''' <summary>
        ''' 这个质谱图内的二级碎片的数量
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        <XmlIgnore>
        Public ReadOnly Property length As Integer
            Get
                Return matrix.Length
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LibraryMatrix() As LibraryMatrix
            Return New LibraryMatrix With {
                .name = title,
                .ms2 = matrix
            }
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of ms2) Implements Enumeration(Of ms2).GenericEnumerator
            For Each fragment As ms2 In matrix.SafeQuery
                Yield fragment
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of ms2).GetEnumerator
            Yield GenericEnumerator()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType([lib] As LibraryMatrix) As SpectraMatrix
            Return New SpectraMatrix With {.title = [lib].name, .matrix = [lib].ms2}
        End Operator
    End Class

End Namespace