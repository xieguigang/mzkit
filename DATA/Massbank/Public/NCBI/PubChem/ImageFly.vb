#Region "Microsoft.VisualBasic::2909fa3989c7d763f4dab9f11367bf17, Massbank\Public\NCBI\PubChem\ImageFly.vb"

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

    '     Module ImageFly
    ' 
    '         Function: GetImage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.BitmapImage

Namespace NCBI.PubChem

    Public Module ImageFly

        Public Function GetImage(cid$, Optional width% = 300, Optional height% = 300) As Bitmap
            Dim url$ = $"https://pubchem.ncbi.nlm.nih.gov/image/imagefly.cgi?cid={cid}&width={width}&height={height}"
            Dim tmp$ = App.GetAppSysTempFile(".png", sessionID:=App.PID)

            If Not url.DownloadFile(save:=tmp) Then
                Return Nothing
            End If

            Dim white As Color = Color.FromArgb(245, 245, 245)
            Dim bitmap As Bitmap = New Bitmap(tmp) _
                .CorpBlank(margin:=5, blankColor:=white) _
                .ColorReplace(white, Color.Transparent)

            Return bitmap
        End Function
    End Module
End Namespace
