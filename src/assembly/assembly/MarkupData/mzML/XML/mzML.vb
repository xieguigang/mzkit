#Region "Microsoft.VisualBasic::75a4e0207708d985ae588165706e7511, assembly\MarkupData\mzML\XML\mzML.vb"

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

    '     Class mzML
    ' 
    '         Properties: cvList, fileDescription, id, run, version
    ' 
    '         Function: LoadChromatogramList, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.mzML

    <XmlType(NameOf(mzML), [Namespace]:=indexedmzML.xmlns)>
    Public Class mzML

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property version As String

        Public Property cvList As cvList
        Public Property run As run
        Public Property fileDescription As fileDescription

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadChromatogramList(path As String) As IEnumerable(Of chromatogram)
            Return MarkupData.mzML.LoadChromatogramList(path)
        End Function

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class
End Namespace
