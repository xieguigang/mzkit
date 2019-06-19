Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.MassSpectrum.DATA.MetaLib.Models

Namespace TMIC.HMDB.Repository

    Public Class WebQuery : Inherits WebQuery(Of String)

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            MyBase.New(url:=Function(id) $"http://www.hmdb.ca/metabolites/{id.FormatHMDBId}.xml",
                       contextGuid:=Function(id) id,
                       parser:=AddressOf ParseXml,
                       prefix:=Function(id) Mid(id.Match("\d+").ParseInteger.ToString, 1, 2),
                       cache:=cache,
                       interval:=interval,
                       offline:=offline
            )
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function ParseXml(xml$, null As Type) As metabolite
            Return xml.LoadFromXml(Of metabolite)
        End Function
    End Class
End Namespace