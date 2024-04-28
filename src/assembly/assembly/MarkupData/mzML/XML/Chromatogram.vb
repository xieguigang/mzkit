#Region "Microsoft.VisualBasic::2f5f6b5b34c31a81aff1131c508a64f1, E:/mzkit/src/assembly/assembly//MarkupData/mzML/XML/Chromatogram.vb"

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

    '   Total Lines: 39
    '    Code Lines: 23
    ' Comment Lines: 8
    '   Blank Lines: 8
    '     File Size: 1.08 KB


    '     Class chromatogramList
    ' 
    '         Properties: list
    ' 
    '     Class chromatogram
    ' 
    '         Properties: precursor, product
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.IonTargeted

Namespace MarkupData.mzML

    Public Class chromatogramList : Inherits DataList

        <XmlElement(NameOf(chromatogram))>
        Public Property list As chromatogram()

    End Class

    Public Class chromatogram : Inherits BinaryData

#Region "MRM Ion Pair"
        ''' <summary>
        ''' MRM Q1
        ''' </summary>
        ''' <returns></returns>
        Public Property precursor As precursor
        ''' <summary>
        ''' MRM Q3
        ''' </summary>
        ''' <returns></returns>
        Public Property product As product
#End Region

        Public Overrides Function ToString() As String
            If id = "TIC" Then
                Return id
            Else
                Dim parent As String = Me.precursor.GetMz
                Dim product As String = Me.product.GetMz

                Return $"Ion [{parent}/{product}]"
            End If
        End Function
    End Class
End Namespace
