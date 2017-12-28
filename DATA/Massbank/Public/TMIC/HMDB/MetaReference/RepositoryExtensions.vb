Imports System.Runtime.CompilerServices
Imports XmlLinq = Microsoft.VisualBasic.Text.Xml.Linq.Data

Namespace TMIC.HMDB

    Public Module RepositoryExtensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function PopulateHMDBMetaData(Xml As String) As IEnumerable(Of MetaReference)
            Return XmlLinq.LoadXmlDataSet(Of MetaReference)(
                XML:=Xml,
                typeName:=NameOf(metabolite),
                xmlns:="http://www.hmdb.ca",
                forceLargeMode:=True
            )
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function EnumerateNames(metabolite As MetaReference) As IEnumerable(Of String)
            Return {metabolite.name}.AsList +
                metabolite.synonyms.synonym +
                metabolite.iupac_name
        End Function
    End Module
End Namespace