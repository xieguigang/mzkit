#Region "Microsoft.VisualBasic::42ad9033af1be93e7ab64544e216268e, G:/mzkit/src/metadb/MoNA//Web/WebJSON.vb"

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


    ' Code Statistics:

    '   Total Lines: 64
    '    Code Lines: 52
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 2.47 KB


    ' Class WebJSON
    ' 
    '     Properties: id, spectrum
    ' 
    '     Function: GetJson, ParseMatrix, SearchLCMSByName, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class WebJSON

    Friend Const urlBase As String = "https://mona.fiehnlab.ucdavis.edu/rest/spectra/%s"
    Friend Const query As String = "https://mona.fiehnlab.ucdavis.edu/rest/spectra/search?endpoint=search&query=compound.names=q=%27name=like=%22{q}%22%27%20and%20(tags.text==%22LC-MS%22)%20and%20(metaData=q=%27name==%22ms%20level%22%20and%20value==%22MS2%22%27)&size=30"

    Public Property spectrum As String
    Public Property id As String

    Public Overrides Function ToString() As String
        Return id
    End Function

    Public Function ParseMatrix() As LibraryMatrix
        Dim data As String() = spectrum.Split(" "c)
        Dim mzinto As ms2() = data _
            .Select(Function(str)
                        Dim t As String() = str.Split(":"c)
                        Dim m As New ms2 With {
                            .mz = Val(t(0)),
                            .intensity = Val(t(1))
                        }

                        Return m
                    End Function) _
            .ToArray

        Return New LibraryMatrix With {
            .name = id,
            .ms2 = mzinto
        }
    End Function

    Public Shared Function SearchLCMSByName(name As String, Optional cache As String = "./.mona/") As WebJSON()
        Static cachePool As New Dictionary(Of String, QueryWeb)

        Return cachePool _
            .ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New QueryWeb(cache)
                           End Function) _
            .Query(Of WebJSON())((False, name), cacheType:=".json")
    End Function

    Public Shared Function GetJson(id As String, Optional cache As String = "./.mona/") As WebJSON
        Static cachePool As New Dictionary(Of String, QueryWeb)

        Return cachePool _
            .ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New QueryWeb(cache)
                           End Function) _
            .Query(Of WebJSON)((False, id), cacheType:=".json")
    End Function

End Class
