#Region "Microsoft.VisualBasic::4d204b7390a0f4496a2fefc67e594219, assembly\MarkupData\mzML\XML\MsData\scanList.vb"

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

    '     Class spectrumList
    ' 
    '         Properties: spectrums
    ' 
    '         Function: GetAllMs1
    ' 
    '     Class precursorList
    ' 
    '         Properties: precursor
    ' 
    '     Class scanList
    ' 
    '         Properties: cvParams, scans
    ' 
    '     Class scan
    ' 
    '         Properties: instrumentConfigurationRef
    ' 
    '     Class scanWindowList
    ' 
    '         Properties: scanWindows
    ' 
    '     Class scanWindow
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.IonTargeted

Namespace MarkupData.mzML

    Public Class spectrumList : Inherits DataList

        <XmlElement("spectrum")>
        Public Property spectrums As spectrum()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAllMs1() As IEnumerable(Of spectrum)
            Return spectrums.GetAllMs1
        End Function
    End Class

    Public Class precursorList : Inherits List

        <XmlElement>
        Public Property precursor As precursor()

    End Class

    Public Class scanList : Inherits List

        <XmlElement("cvParam")>
        Public Property cvParams As cvParam()
        <XmlElement("scan")>
        Public Property scans As scan()

    End Class

    Public Class scan : Inherits Params

        <XmlAttribute>
        Public Property instrumentConfigurationRef As String

    End Class

    Public Class scanWindowList : Inherits List

        <XmlElement(NameOf(scanWindow))>
        Public Property scanWindows As scanWindow()

    End Class

    Public Class scanWindow : Inherits Params

    End Class
End Namespace
