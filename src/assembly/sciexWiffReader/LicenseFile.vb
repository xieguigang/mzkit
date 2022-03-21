#Region "Microsoft.VisualBasic::cb556097645ef6a3e9c548e81fe0744d, mzkit\src\assembly\sciexWiffReader\LicenseFile.vb"

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

    '   Total Lines: 79
    '    Code Lines: 52
    ' Comment Lines: 13
    '   Blank Lines: 14
    '     File Size: 2.26 KB


    ' Class LicenseFile
    ' 
    '     Properties: company_name, features, key_data, LicenseValid, product_name
    ' 
    '     Function: ApplyLicense, Install
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Clearcore2.Licensing
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml

''' <summary>
''' license file helper for mzkit using clearcore2 system
''' </summary>
''' 
<XmlType("license_key")>
<XmlRoot("license_key")>
Public Class LicenseFile

    Shared ReadOnly filepath As String = App.ProductProgramData & "/Clearcore2_license.xml"

    Public Property company_name As String
    Public Property product_name As String
    Public Property features As String

    ''' <summary>
    ''' base64 string of the sciex license
    ''' </summary>
    ''' <returns></returns>
    Public Property key_data As String

    Shared _valid As Boolean = False

    Public Shared Property LicenseValid As Boolean
        Get
            Return _valid
        End Get
        Private Set(value As Boolean)
            _valid = value
        End Set
    End Property

    ''' <summary>
    ''' install the license key and test for license valid or not
    ''' </summary>
    ''' <param name="key"></param>
    ''' <returns></returns>
    Public Shared Function Install(person As String,
                                   key As String,
                                   Optional product As String = "ProcessingFramework",
                                   Optional features As String = "WiffReaderSDK") As Boolean

        Dim license As New LicenseFile With {
           .company_name = person,
           .features = features,
           .key_data = key,
           .product_name = product
        }

        Call license _
            .GetXml(xmlEncoding:=XmlEncodings.UTF8) _
            .SaveTo(
                path:=LicenseFile.filepath,
                encoding:=Encodings.UTF8.CodePage
            )

        Return ApplyLicense()
    End Function

    Public Shared Function ApplyLicense() As Boolean
        If LicenseFile.filepath.FileLength <= 0 Then
            Return False
        End If

        Try
            LicenseKeys.Keys = {LicenseFile.filepath.ReadAllText}
            LicenseValid = True
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

End Class
