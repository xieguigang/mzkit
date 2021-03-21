#Region "Microsoft.VisualBasic::da1b1c841361d2e579f967a0ee58ec23, src\assembly\ProteoWizard.d\MassWolf.vb"

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

    ' Class MassWolf
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ConvertWatersRaw
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel.Threads
Imports ProteoWizard.Interop
Imports SMRUCC.WebCloud.HTTPInternal
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.Core
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<[Namespace]("MassWolf.d")>
Public Class MassWolf : Inherits WebApp

    ''' <summary>
    ''' MassWolf命令行程序的位置
    ''' </summary>
    ReadOnly BIN$

    Dim taskPool As New ThreadPool

    Public Sub New(main As PlatformEngine)
        MyBase.New(main)

        BIN = App.GetVariable("masswolf")

        Call $"MassWolf={BIN}".__INFO_ECHO

        If Not BIN.FileExists Then
            Call $"MassWolf application is missing, this web app will not working unless you have put MassWolf to the location {BIN}".Warning
        End If
    End Sub

    <ExportAPI("/MassWolf.d/mzXML.vbs")>
    <Usage("/MassWolf.d/mzXML.vbs?path=<path>")>
    <[GET](GetType(String))>
    Public Function ConvertWatersRaw(request As HttpRequest, response As HttpResponse) As Boolean
        Dim path$ = EnsureZipExtract(NormalizeOSSPath(request.URLParameters("path")))
        Dim out$ = NormalizeOSSPath(request.URLParameters("to")) Or $"{path.ParentPath}/msconvert".AsDefault

        For Each part In SplitDirectory(waters:=path)
            Dim args = $"--mzXML {part.In.CLIPath} {(out & "/" & part.Out).CLIPath}"
            Dim shell% = App.Shell(BIN, args, CLR:=False).Run

            Call part.Out.__INFO_ECHO
        Next

        Call response.SuccessMsg("Run MassWolf task complete!")

        Return True
    End Function
End Class
