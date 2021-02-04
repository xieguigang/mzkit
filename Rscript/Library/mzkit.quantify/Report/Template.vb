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
