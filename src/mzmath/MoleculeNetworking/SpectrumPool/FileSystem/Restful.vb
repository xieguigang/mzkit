#Region "Microsoft.VisualBasic::2bf0f68dacc8a68b6d91ded83445b541, E:/mzkit/src/mzmath/MoleculeNetworking//SpectrumPool/FileSystem/Restful.vb"

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

    '   Total Lines: 40
    '    Code Lines: 33
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.41 KB


    '     Class Restful
    ' 
    '         Properties: code, debug, info
    ' 
    '         Function: (+2 Overloads) ParseJSON
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.My.JavaScript
Imports Microsoft.VisualBasic.Net.Http

Namespace PoolData

    Public Class Restful

        Public Property code As Integer
        Public Property debug As Object
        Public Property info As Object

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseJSON(json As WebResponseResult) As Restful
            If json Is Nothing Then
                Return New Restful With {.code = -1, .info = Nothing}
            Else
                Return ParseJSON(json.html)
            End If
        End Function

        Public Shared Function ParseJSON(json As String) As Restful
            If json.StringEmpty OrElse json.TextEquals("null") Then
                Return New Restful With {.code = -1, .info = Nothing}
            Else
                Dim obj As JavaScriptObject = DirectCast(JsonParser.Parse(json), JsonObject)
                Dim code As String = obj!code

                Return New Restful With {
                    .code = Integer.Parse(code),
                    .debug = obj!debug,
                    .info = obj!info
                }
            End If
        End Function

    End Class
End Namespace
