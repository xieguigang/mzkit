#Region "Microsoft.VisualBasic::4a110dfe81bee8a773dfc9356e58562f, src\assembly\assembly\MarkupData\mzML\XML\Chromatogram.vb"

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

'     Class chromatogramList
' 
'         Properties: list
' 
'     Class chromatogram
' 
'         Properties: precursor, product
' 
'     Interface IMRMSelector
' 
'         Properties: isolationWindow
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.mzML

    Public Class chromatogramList : Inherits DataList
        <XmlElement(NameOf(chromatogram))>
        Public Property list As chromatogram()
    End Class

    Public Class chromatogram : Inherits BinaryData

        Public Property precursor As precursor
        Public Property product As product

        Public Overrides Function ToString() As String
            If id = "TIC" Then
                Return id
            Else
                Return $"Ion [{precursor.isolationWindow.cvParams.KeyItem("isolation window target m/z").value}/{product.isolationWindow.cvParams.KeyItem("isolation window target m/z").value}]"
            End If
        End Function

    End Class

    Public Interface IMRMSelector
        Property isolationWindow As Params
    End Interface
End Namespace
