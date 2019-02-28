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
            Dim identifier = Me("Names and Identifiers")
            Dim formula = Me("Molecular Formula")
            Dim SMILES = Me("Canonical SMILES")
            Dim InChIKey = Me("InChI Key")
            Dim InChI = Me("InChI")
            Dim CAS = identifier.Sections
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