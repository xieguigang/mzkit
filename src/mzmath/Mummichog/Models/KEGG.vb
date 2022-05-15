Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

''' <summary>
''' create background network graph model for kegg data
''' </summary>
Public Module KEGG

    <Extension>
    Public Iterator Function CreateBackground(pathways As IEnumerable(Of Map), reactions As Dictionary(Of String, Reaction)) As IEnumerable(Of NamedValue(Of NetworkGraph))

    End Function
End Module
