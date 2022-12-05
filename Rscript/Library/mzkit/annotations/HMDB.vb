
Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemistry
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.Object

''' <summary>
''' toolkit for handling of the hmdb database
''' </summary>
<Package("hmdb_kit")>
Module HMDBTools

    ''' <summary>
    ''' open a reader for read hmdb database
    ''' </summary>
    ''' <param name="xml">
    ''' the file path of the hmdb metabolite database xml file
    ''' </param>
    ''' <returns>
    ''' this function populate a collection of the hmdb metabolites data
    ''' </returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("read.hmdb")>
    Public Function readHMDB(xml As String) As pipeline
        Return TMIC.HMDB _
            .LoadXML(xml) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function
End Module
