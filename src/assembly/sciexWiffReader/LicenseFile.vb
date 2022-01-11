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
