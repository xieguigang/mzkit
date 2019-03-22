Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace IUPAC

    ''' <summary>
    ''' The InChI identifier parser
    ''' 
    ''' 国际化合物标识（英语：InChI，英语：International Chemical Identifier）是由国际纯粹与
    ''' 应用化学联合会和美国国家标准技术研究所（National Institute of Standards and Technology，NIST）
    ''' 联合制定的，用以唯一标识化合物IUPAC名称的字符串。
    ''' </summary>
    <ContentType("chemical/x-inchi")> Public Class InChI

        Sub New(inchi As String)

        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(inchi As String) As InChI
            Return New InChI(inchi)
        End Function
    End Class
End Namespace

