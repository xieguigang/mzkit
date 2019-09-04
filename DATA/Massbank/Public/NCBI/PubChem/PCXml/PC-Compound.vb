Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace NCBI.PubChem.PCCompound

    <XmlType("PC-Compound")> Public Class Compound

        Public Property id As PC.id
        Public Property count As PC.count

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadFromXml(xml As String) As Compound
            Return TrimXml(xml).LoadFromXml(Of Compound)
        End Function
    End Class

    Namespace PC

        Public Class count
            <XmlElement("PC-Count")>
            Public Property Count As Elements.Count
        End Class

        Public Class id
            <XmlElement("PC-CompoundType")>
            Public Property Type As Elements.CompoundType
        End Class
    End Namespace

End Namespace