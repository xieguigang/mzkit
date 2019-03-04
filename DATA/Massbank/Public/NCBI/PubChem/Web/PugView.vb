Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports MetaInfo = SMRUCC.MassSpectrum.DATA.MetaLib.MetaLib

Namespace NCBI.PubChem

    <XmlRoot("Record", [Namespace]:="http://pubchem.ncbi.nlm.nih.gov/pug_view")>
    Public Class PugViewRecord : Inherits InformationSection

        Public Property RecordType As String
        Public Property RecordNumber As String

        <XmlElement(NameOf(Reference))>
        Public Property Reference As Reference()

        Public Function GetMetaInfo() As MetaInfo
            Dim identifier = Me.Section("Names and Identifiers")
            Dim formula = identifier("Molecular Formula")
            Dim SMILES = identifier("Canonical SMILES").GetInformationString("Canonical SMILES")
            Dim InChIKey = identifier("InChI Key").GetInformationString("InChI Key")
            Dim InChI = identifier("InChI")
            Dim CAS = identifier("Other Identifiers")
            Dim computedProperties = Me _
                ("Chemical and Physical Properties") _
                ("Computed Properties").GetInformationTable _
                ("Computed Properties")
            Dim properties = Table.ToDictionary(computedProperties)
            Dim xref As New MetaLib.xref With {
                .InChI = InChI.GetInformationString("InChI"),
                .CAS = CAS.GetInformationString("CAS"),
                .InChIkey = InChIKey,
                .pubchem = RecordNumber
            }

            Return New MetaInfo With {
                .formula = formula.GetInformationString("Molecular Formula"),
                .xref = xref,
                .name = identifier _
                    .Sections _
                    .FirstOrDefault(Function(s) s.TOCHeading = "Record Title") _
                    .GetInformationString("Record Title"),
                .mass = ParseDouble(properties("Exact Mass").Value)
            }
        End Function

        Public Overrides Function ToString() As String
            Return RecordNumber
        End Function
    End Class
End Namespace