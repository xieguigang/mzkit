#Region "Microsoft.VisualBasic::2a35635979fd9ce78024ea691138c85a, src\assembly\assembly\MarkupData\mzML\XML\XML.vb"

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

'     Class Xml
' 
'         Properties: fileChecksum, indexList, indexListOffset, mzML
' 
'     Class indexList
' 
'         Properties: index
' 
'     Class index
' 
'         Properties: name, offsets
' 
'     Class offset
' 
'         Properties: idRef, value
' 
'         Function: ToString
' 
'     Class mzML
' 
'         Properties: cvList, run
' 
'     Class cvList
' 
'         Properties: list
' 
'     Class List
' 
'         Properties: count
' 
'     Class DataList
' 
'         Properties: defaultDataProcessingRef
' 
'     Class BinaryData
' 
'         Properties: binaryDataArrayList, defaultArrayLength, id, index
' 
'     Structure cv
' 
'         Properties: fullName, id, URI, version
' 
'     Class precursor
' 
'         Properties: activation, isolationWindow
' 
'     Class product
' 
'         Properties: activation, isolationWindow
' 
'     Class Params
' 
'         Properties: cvParams, userParams
' 
'     Class userParam
' 
'         Properties: name, type, value
' 
'         Function: ToString
' 
'     Class binaryDataArrayList
' 
'         Properties: list
' 
'     Class binaryDataArray
' 
'         Properties: binary, cvParams, encodedLength
' 
'         Function: GetCompressionType, GetPrecision, ToString
' 
'     Class cvParam
' 
'         Properties: accession, cvRef, name, unitAccession, unitCvRef
'                     unitName, value
' 
'         Function: ToString
' 
'     Class run
' 
'         Properties: chromatogramList, defaultInstrumentConfigurationRef, defaultSourceFileRef, id, spectrumList
'                     startTimeStamp
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData.mzML

    <XmlType("indexedmzML", [Namespace]:=Xml.xmlns)>
    Public Class Xml

        Public Property mzML As mzML
        Public Property indexList As indexList
        Public Property indexListOffset As Long
        Public Property fileChecksum As String

        Public Const xmlns As String = "http://psi.hupo.org/ms/mzml"

        Public Shared Iterator Function LoadScans(file As String) As IEnumerable(Of spectrum)
            For Each scan As spectrum In file.LoadXmlDataSet(Of spectrum)(, xmlns:=xmlns)
                Yield scan
            Next
        End Function
    End Class
End Namespace
