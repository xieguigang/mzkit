#Region "Microsoft.VisualBasic::b2ccbd399a1be2c0904fbe24f5a134f9, src\metadb\MoNA\test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: exportIdList, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.MassSpectrum.DATA.MoNA
Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Sub Main()
        Call exportIdList()
    End Sub

    Sub exportIdList()
        Dim idlist = SDFReader.ParseFile("D:\Database\MoNA\MoNA-export-GNPS-sdf\MoNA-export-GNPS.sdf", skipSpectraInfo:=True).ToDictionary(Function(s) s.ID, Function(s) s.name)

        Call idlist.GetJson.SaveTo("D:\Database\MoNA\GNPS.json")
    End Sub
End Module
