Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.Medical

Namespace MetaLib

    Public Module KEGGExtensions

        ''' <summary>
        ''' 将KEGG数据库之中的药物编号以及Glyan物质的编号转换为Compound编号
        ''' </summary>
        ''' <param name="input">
        ''' 包含有两个字段的tuple数据：
        ''' 
        ''' + keggDrug KEGG的药物数据库的文件路径
        ''' + KEGGcpd KEGG的代谢物数据库的文件夹路径，在这个文件夹里面应该包含有Glyan物质的数据
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function KEGGDrugGlyan2Compound(input As (keggDrug$, KEGGcpd$)) As Dictionary(Of String, String)
            Dim drugs = DrugParser.ParseStream(input.keggDrug).ToArray
            Dim idMaps As New Dictionary(Of String, String)

            Call "Convert drug id to compound id...".__DEBUG_ECHO

            For Each d As Medical.Drug In drugs
                Dim CId As String() = d.CompoundID

                If Not CId.IsNullOrEmpty Then
                    idMaps(d.Entry) = CId.First
                End If
            Next

            Dim gl = (ls - l - r - "*.Xml" <= input.KEGGcpd) _
                .Where(Function(f) f.BaseName.IsPattern("G\d+")) _
                .Select(AddressOf LoadXml(Of Glycan))

            Call "Scan glycan and convert glucan id to compound id...".__DEBUG_ECHO

            For Each glycan As Glycan In gl
                Dim CId = glycan.CompoundId

                If Not CId.IsNullOrEmpty Then
                    idMaps(glycan.Entry) = CId.First
                End If
            Next

            Return idMaps
        End Function
    End Module
End Namespace