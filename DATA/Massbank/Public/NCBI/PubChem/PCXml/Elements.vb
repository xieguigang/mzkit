Imports System.Xml.Serialization

Namespace NCBI.PubChem.PCCompound.Elements

    Public Class Count
        <XmlElement("heavy-atom")> Public Property heavy_atom As Integer
        <XmlElement("atom-chiral")> Public Property atom_chiral As Integer
        <XmlElement("atom-chiral-def")> Public Property atom_chiral_def As Integer
        <XmlElement("atom-chiral-undef")> Public Property atom_chiral_undef As Integer
        <XmlElement("bond-chiral")> Public Property bond_chiral As Integer
        <XmlElement("bond-chiral-def")> Public Property bond_chiral_def As Integer
        <XmlElement("bond-chiral-undef")> Public Property bond_chiral_undef As Integer
        <XmlElement("isotope-atom")> Public Property isotope_atom As Integer
        <XmlElement("covalent-unit")> Public Property covalent_unit As Integer
        <XmlElement("tautomers")> Public Property tautomers As Integer
    End Class

    Public Class CompoundType
        Public Property id As id
    End Class

    Public Class id
        Public Property cid As String
    End Class
End Namespace