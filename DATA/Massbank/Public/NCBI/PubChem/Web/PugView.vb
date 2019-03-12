Imports System.Text.RegularExpressions
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

        Public Const HMDB$ = "Human Metabolome Database (HMDB)"

        ''' <summary>
        ''' 从pubchem数据库之中提取注释所需要的必须基本信息
        ''' </summary>
        ''' <returns></returns>
        Public Function GetMetaInfo() As MetaInfo
            Dim identifier = Me("Names and Identifiers")
            Dim formula = identifier("Molecular Formula").GetInformationString("Molecular Formula")
            Dim descriptors = identifier("Computed Descriptors")
            Dim SMILES = descriptors("Canonical SMILES").GetInformationString("Canonical SMILES")
            Dim InChIKey = descriptors("InChI Key").GetInformationString("InChI Key")
            Dim InChI = descriptors("InChI").GetInformationString("InChI")
            Dim otherNames = identifier("Other Identifiers")
            Dim synonyms = identifier _
                ("Synonyms") _
                ("Depositor-Supplied Synonyms").GetInformationStrings _
                ("Depositor-Supplied Synonyms")
            Dim computedProperties = Me _
                ("Chemical and Physical Properties") _
                ("Computed Properties").GetInformationTable _
                ("Computed Properties")
            Dim properties = Table.ToDictionary(computedProperties)
            Dim xref As New MetaLib.xref With {
                .InChI = InChI,
                .CAS = otherNames("CAS")?.GetInformationString("CAS"),
                .InChIkey = InChIKey,
                .pubchem = RecordNumber,
                .chebi = synonyms.FirstOrDefault(Function(id) id.IsPattern("CHEBI[:]\d+")),
                .KEGG = synonyms.FirstOrDefault(Function(id)
                                                    Return id.IsPattern("C\d+", RegexOptions.Singleline)
                                                End Function),
                .HMDB = Reference.GetHMDBId
            }
            Dim commonName$ = identifier _
                .Sections _
                .FirstOrDefault(Function(s) s.TOCHeading = "Record Title") _
                .GetInformationString("Record Title")

            Return New MetaInfo With {
                .formula = formula,
                .xref = xref,
                .name = commonName,
                .mass = ParseDouble(properties("Exact Mass").Value),
                .ID = RecordNumber
            }
        End Function

        Public Overrides Function ToString() As String
            Return RecordNumber
        End Function
    End Class
End Namespace