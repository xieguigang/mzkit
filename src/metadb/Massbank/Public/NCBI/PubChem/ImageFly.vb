#Region "Microsoft.VisualBasic::429e7e86df45513898212f26618f7e0a, metadb\Massbank\Public\NCBI\PubChem\ImageFly.vb"

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

'   Total Lines: 89
'    Code Lines: 61 (68.54%)
' Comment Lines: 18 (20.22%)
'    - Xml Docs: 88.89%
' 
'   Blank Lines: 10 (11.24%)
'     File Size: 3.45 KB


'     Module ImageFly
' 
'         Function: (+2 Overloads) GetImage
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Net.WebClient


#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Namespace NCBI.PubChem

    Public Module ImageFly

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="cid"></param>
        ''' <param name="doBgTransparent">
        ''' 是否将得到的图片的背景设置为透明
        ''' </param>
        ''' <returns></returns>
        Public Function GetImage(cid$, Optional size$ = "300,300", Optional doBgTransparent As Boolean = True) As Bitmap
            With size.SizeParser
                Return GetImage(cid, .Width, .Height, doBgTransparent)
            End With
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="cid"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <param name="doBgTransparent">
        ''' 是否将得到的图片的背景设置为透明
        ''' </param>
        ''' <returns></returns>
        Public Function GetImage(cid$,
                                 Optional width% = 300,
                                 Optional height% = 300,
                                 Optional doBgTransparent As Boolean = True) As Bitmap

            Dim url$ = $"https://pubchem.ncbi.nlm.nih.gov/image/imagefly.cgi?cid={cid}&width={width}&height={height}"
            Dim tmp$ = TempFileSystem.GetAppSysTempFile(".png", sessionID:="cid__", prefix:="imageFly___")
            Dim webget As Double = False

            If App.IsConsoleApp Then
                webget = wget.Download(url, save:=tmp)
            Else
                webget = WebServiceUtils.DownloadFile(url, save:=tmp)
            End If

            If Not webget Then
                Return Nothing
            ElseIf Not doBgTransparent Then
                Return tmp.LoadImage
            End If

            Dim white As Color = Color.FromArgb(245, 245, 245)
            Dim bitmap As Bitmap = New Bitmap(tmp).ColorReplace(white, Color.Transparent)

            Return bitmap
        End Function
    End Module
End Namespace
