Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public Class LayerLoader

    Public Property mz As Double()
    Public Property mzErr As Double
    Public Property method As String
    Public Property densityCut As Double

    Public Function GetTolerance() As Tolerance
        Return Tolerance.ParseScript($"{method}:{mzErr}")
    End Function

    Shared Sub New()
        Call MsgPackSerializer.DefaultContext.RegisterSerializer(New Schema)
    End Sub

    Private Class Schema : Inherits SchemaProvider(Of LayerLoader)

        Protected Overrides Iterator Function GetObjectSchema() As IEnumerable(Of (obj As Type, schema As Dictionary(Of String, NilImplication)))
            Yield (GetType(LayerLoader), New Dictionary(Of String, NilImplication) From {
                {NameOf(mz), NilImplication.MemberDefault},
                {NameOf(mzErr), NilImplication.MemberDefault},
                {NameOf(method), NilImplication.MemberDefault},
                {NameOf(densityCut), NilImplication.MemberDefault}
            })
        End Function
    End Class

End Class