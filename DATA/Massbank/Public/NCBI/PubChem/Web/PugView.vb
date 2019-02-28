Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports MetaInfo = SMRUCC.MassSpectrum.DATA.MetaLib.MetaLib

Namespace NCBI.PubChem

    <XmlRoot("Record", [Namespace]:="http://pubchem.ncbi.nlm.nih.gov/pug_view")>
    Public Class PugViewRecord

        Public Property RecordType As String
        Public Property RecordNumber As String

        <XmlElement("Section")>
        Public Property Sections As Section()
            Get
                Return sectionTable.Values.ToArray
            End Get
            Set(value As Section())
                sectionTable = value.ToDictionary(Function(sec) sec.TOCHeading)
            End Set
        End Property

        <XmlElement(NameOf(Reference))>
        Public Property Reference As Reference()

        Dim sectionTable As Dictionary(Of String, Section)

        Public Function GetMetaInfo() As MetaInfo
            Dim identifier = sectionTable.TryGetValue("Names and Identifiers")
            Dim formula = sectionTable.TryGetValue("Molecular Formula")
            Dim SMILES = sectionTable.TryGetValue("Canonical SMILES")
            Dim InChIKey = sectionTable.TryGetValue("InChI Key")
            Dim InChI = sectionTable.TryGetValue("InChI")
            Dim xref As New MetaLib.xref With {
                .InChI = InChI.GetInformationString("InChI")
            }

            Return New MetaInfo With {
                .formula = formula.GetInformationString("Molecular Formula"),
                .xref = xref,
                .name = identifier _
                    .Sections _
                    .FirstOrDefault(Function(s) s.TOCHeading = "Record Title") _
                    .GetInformationString("Record Title")
            }
        End Function

        Public Overrides Function ToString() As String
            Return RecordNumber
        End Function
    End Class
End Namespace