#Region "Microsoft.VisualBasic::474fea4ee4f8344c67f5544057a8c8df, mzkit\src\assembly\assembly\MarkupData\imzML\XML\XML.vb"

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

    '   Total Lines: 88
    '    Code Lines: 56
    ' Comment Lines: 15
    '   Blank Lines: 17
    '     File Size: 3.69 KB


    '     Class PointF3D
    ' 
    '         Properties: X, Y, Z
    ' 
    '     Class XML
    ' 
    '         Properties: version
    ' 
    '         Function: Get3DPositionXYZ, Load3DScanData, LoadScanData, LoadScans, PopulateFileContentDescriptions
    ' 
    '         Sub: Get3DPositionXYZ
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace MarkupData.imzML

    <XmlType("indexedmzML", [Namespace]:=mzML.indexedmzML.xmlns)>
    Public Class XML

        <XmlAttribute>
        Public Property version As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseMetadata(imzml As String) As imzMLMetadata
            Return imzMLMetadata.ReadHeaders(imzml)
        End Function

        ''' <summary>
        ''' just load the scan meta from the imzML file
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadScans(file As String) As IEnumerable(Of ScanData)
            Return mzML.indexedmzML _
                .LoadScans(file) _
                .Select(Function(scan) New ScanData(scan))
        End Function

        Public Shared Sub Get3DPositionXYZ(spectrum As mzML.spectrum, ByRef x As Double, ByRef y As Double, ByRef z As Double)
            Dim scan As mzML.scan = spectrum.scanList.scans(Scan0)
            Dim users As userParam() = scan.userParams

            x = Double.Parse(users.KeyItem("3DPositionX").value)
            y = Double.Parse(users.KeyItem("3DPositionY").value)
            z = Double.Parse(users.KeyItem("3DPositionZ").value)
        End Sub

        Public Shared Function Get3DPositionXYZ(spectrum As mzML.spectrum) As PointF3D
            Dim x, y, z As Double

            Call Get3DPositionXYZ(spectrum, x, y, z)

            Return New PointF3D With {
                .X = x,
                .Y = y,
                .Z = z
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="imzML"></param>
        ''' <returns></returns>
        Public Shared Iterator Function Load3DScanData(imzML As String) As IEnumerable(Of Scan3DReader)
            Dim ibd As ibdReader = ibdReader.Open(imzML.ChangeSuffix("ibd"))
            Dim scans = mzML.indexedmzML.LoadScans(imzML).Select(Function(scan) New ScanData3D(scan))

            For Each scan As ScanData3D In scans
                Yield New Scan3DReader(scan, ibd)
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="imzML"></param>
        ''' <returns></returns>
        Public Shared Iterator Function LoadScanData(imzML As String) As IEnumerable(Of ScanReader)
            Dim ibd As ibdReader = ibdReader.Open(imzML.ChangeSuffix("ibd"))

            For Each scan As ScanData In LoadScans(file:=imzML)
                Yield New ScanReader(scan, ibd)
            Next
        End Function

        Friend Shared Iterator Function PopulateFileContentDescriptions(guid As String, ibd_sha1 As String) As IEnumerable(Of cvParam)
            Yield New cvParam With {.cvRef = "MS", .accession = "MS:1000579", .name = "MS1 spectrum"}
            Yield New cvParam With {.cvRef = "MS", .accession = "MS:1000128", .name = "profile spectrum"}
            Yield New cvParam With {.cvRef = "IMS", .accession = "IMS:1000080", .name = "universally unique identifier", .value = guid}
            Yield New cvParam With {.cvRef = "IMS", .accession = "IMS:1000091", .name = "ibd SHA-1", .value = ibd_sha1}
            Yield New cvParam With {.cvRef = "IMS", .accession = "IMS:1000031", .name = "processed"}
        End Function
    End Class
End Namespace
