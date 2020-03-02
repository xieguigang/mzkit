#Region "Microsoft.VisualBasic::b391aab534f43f930f2ca71e757cc8be, src\metadna\MetaDNA_visual\test\Module1.vb"

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
'     Sub: dump, Main, visualNet
' 
' /********************************************************************************/

#End Region

Imports MetaDNA.visual
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts

Module Module1

    Sub Main()

        Call visualNet("D:\MassSpectrum-toolkits\MetaDNA\test\lxy-CID30.Xml")

        Pause()

        Call dump("D:\MassSpectrum-toolkits\MetaDNA\test\lxy-CID30.Xml")
        Call dump("D:\MassSpectrum-toolkits\MetaDNA\test\human_blood.Xml")
        Call dump("D:\MassSpectrum-toolkits\MetaDNA\test\urine.Xml")
        Call dump("D:\MassSpectrum-toolkits\MetaDNA\test\human_brain_tissue.Xml")
        Call dump("D:\MassSpectrum-toolkits\MetaDNA\test\human_gastric_mucosa.Xml")

        Pause()
    End Sub

    Private Sub visualNet(file As String)
        Dim model = MetaDNA.visual.XML.LoadDocument(file)
        Dim graph = model.CreateGraph.doForceLayout(iterations:=2000, showProgress:=True)

        Call graph.Draw().Save(file.ChangeSuffix("png"))
    End Sub

    Private Sub dump(file As String)
        Dim model = MetaDNA.visual.XML.LoadDocument(file)

        Call model.TranslateAsTable.Save(file.TrimSuffix)
    End Sub

End Module
