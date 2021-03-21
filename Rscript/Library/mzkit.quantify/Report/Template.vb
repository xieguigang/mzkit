#Region "Microsoft.VisualBasic::a8f73cf5b9e4a8fb38bc680d380689b0, Rscript\Library\mzkit.quantify\Report\Template.vb"

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

    ' Module Template
    ' 
    '     Function: getBlankReport
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder

Module Template

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Friend Function getBlankReport(title As String) As ScriptBuilder
        Return New ScriptBuilder(
            <html lang="zh-CN">
                <head>
                    <meta charset="utf-8"/>
                    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no"/>

                    <title><%= title %> | BioNovoGene</title>

                    <link rel="icon" href="https://cdn.biodeep.cn/favicon.ico" type="image/x-icon"/>

                    <!-- Bootstrap CSS -->
                    <link rel="stylesheet" href="https://cdn.biodeep.cn/styles/bootstrap-4.3.1-dist/css/bootstrap.min.css" crossorigin="anonymous"/>

                    <style type="text/css">
                        .even td{/*必须加td，代表的是一行进行*/
	                      background-color: #f5f5f5;
                        }

                        .critical td {
                          background-color: #ffd1e1;
                        }

                        .warning td {
                          background-color: #fbffd1;
                        }
                    </style>
                </head>
                <body class="container">
                    <h1><%= title %></h1>
                    <p><%= Now.ToString %></p>
                    <hr/>
                    <h2>Table Of Content</h2>
                    <br/>

                    <div>
                        {$TOC}
                    </div>

                    <hr/>
                    <div style="page-break-after: always;"></div>

                    {$linears}
                </body>

                <!-- Optional JavaScript -->
                <!-- jQuery first, then Popper.js, then Bootstrap JS -->
                <script src="https://cdn.biodeep.cn/vendor/jquery-3.2.1.min.js" crossorigin="anonymous"></script>
                <script src="https://cdn.biodeep.cn/vendor/popper.min.js" crossorigin="anonymous"></script>
                <script src="https://cdn.biodeep.cn/styles/bootstrap-4.3.1-dist/js/bootstrap.min.js" crossorigin="anonymous"></script>
            </html>)
    End Function
End Module
