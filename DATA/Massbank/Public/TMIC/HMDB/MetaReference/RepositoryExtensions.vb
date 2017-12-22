Imports System.Runtime.CompilerServices
Imports XmlLinq = Microsoft.VisualBasic.Text.Xml.Linq.Data

Namespace TMIC.HMDB

    Public Module RepositoryExtensions

        <Extension>
        Public Function PopulateHMDBMetaData(Xml As String) As IEnumerable(Of MetaReference)
            Return XmlLinq.LoadXmlDataSet(Of MetaReference)(
                XML:=Xml,
                typeName:=NameOf(metabolite),
                xmlns:="http://www.hmdb.ca",
                forceLargeMode:=True
            )
        End Function
    End Module
End Namespace