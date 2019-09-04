Imports System.Xml.Serialization
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem.PCCompound.Elements

Namespace NCBI.PubChem.PCCompound

    <XmlType("PC-Compound")> Public Class Compound

        Public Property id As PC.id
        Public Property count As PC.count

        Public Shared Function LoadFromXml(xml As String) As Compound
            xml = xml.Replace("PC-Compound_", "")
            xml = xml.Replace("PC-Count_", "")
            xml = xml.Replace("PC-CompoundType_", "")

            Return xml.LoadFromXml(Of Compound)
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