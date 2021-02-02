#Region "Microsoft.VisualBasic::66b2d4e42c1ffd9fdfca09accf794245, Library\mzkit\PubChem.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module PubChemToolKit
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ImageFlyGetImages, pugView, queryPubChem, ReadSIDMap, SIDMapTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Threading
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemistry.NCBI
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("pubchem_kit")>
Module PubChemToolKit

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(SIDMap()), AddressOf SIDMapTable)
    End Sub

    Private Function SIDMapTable(maps As SIDMap(), args As list, env As Environment) As Rdataframe
        Dim data As New Rdataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        data.columns(NameOf(SIDMap.SID)) = maps.Select(Function(m) m.SID).ToArray
        data.columns(NameOf(SIDMap.sourceName)) = maps.Select(Function(m) m.sourceName).ToArray
        data.columns(NameOf(SIDMap.registryIdentifier)) = maps.Select(Function(m) m.registryIdentifier).ToArray
        data.columns(NameOf(SIDMap.CID)) = maps.Select(Function(m) m.CID).ToArray

        Return data
    End Function

    <ExportAPI("image_fly")>
    Public Function ImageFlyGetImages(<RRawVectorArgument>
                                      cid As Object,
                                      <RRawVectorArgument>
                                      Optional size As Object = "500,500",
                                      Optional ignoresInvalidCid As Boolean = False,
                                      Optional env As Environment = Nothing) As Object

        Dim ids As String() = REnv.asVector(Of String)(cid)
        Dim invalids = ids.Where(Function(id) Not id.IsPattern("\d+")).ToArray
        Dim images As New list
        Dim sizeVector As Double()

        If TypeOf size Is String OrElse TypeOf size Is String() Then
            With DirectCast(REnv.asVector(Of String)(size), String()).First.SizeParser
                sizeVector = { .Width, .Height}
            End With
        ElseIf TypeOf size Is Double() Then
            sizeVector = size
        Else
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Double), size.GetType, env), env)
        End If

        Dim img As Image

        For Each id As String In ids
            img = ImageFly.GetImage(id, sizeVector(0), sizeVector(1), doBgTransparent:=False)

            Call Thread.Sleep(1000)
            Call images.slots.Add(id, img)
        Next

        Return images
    End Function

    ''' <summary>
    ''' query of the pubchem database
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="cache$"></param>
    ''' <returns></returns>
    <ExportAPI("query")>
    Public Function queryPubChem(<RRawVectorArgument> id As Object, Optional cache$ = "./", Optional env As Environment = Nothing) As list
        Dim ids As String() = REnv.asVector(Of String)(id)
        Dim cid As String()
        Dim query As New Dictionary(Of String, PugViewRecord)
        Dim result As New list With {
            .slots = New Dictionary(Of String, Object)
        }
        Dim meta As Dictionary(Of String, MetaLib)

        For Each term As String In ids.Distinct.ToArray
            query = PubChem.QueryPugViews(term, cacheFolder:=cache)
            cid = query.Keys.ToArray
            meta = query _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  If a.Value Is Nothing Then
                                      Return Nothing
                                  Else
                                      Return a.Value.GetMetaInfo
                                  End If
                              End Function)

            Call result.slots.Add(term, meta)
        Next

        Return result
    End Function

    <ExportAPI("pugView")>
    Public Function pugView(<RRawVectorArgument> cid As Object,
                            Optional cacheFolder$ = "./pubchem_cache",
                            Optional offline As Boolean = False,
                            Optional env As Environment = Nothing) As Object

        Dim api As WebQuery = $"{cacheFolder}/pugViews/".GetQueryHandler(Of WebQuery)(offline)
        Dim result = env.EvaluateFramework(Of String, PugViewRecord)(
            x:=cid,
            eval:=Function(id)
                      Return api.Query(Of PugViewRecord)(id)
                  End Function)

        Return result
    End Function

    <ExportAPI("SID_map")>
    Public Function ReadSIDMap(sidMapText As String, Optional skipNoCID As Boolean = True, Optional dbfilter$ = Nothing) As SIDMap()
        Dim ls As SIDMap() = SIDMap.GetMaps(handle:=sidMapText, skipNoCID:=skipNoCID).ToArray

        If Not dbfilter.StringEmpty Then
            ls = ls.Where(Function(map) map.sourceName = dbfilter).ToArray
        End If

        Return ls
    End Function
End Module
