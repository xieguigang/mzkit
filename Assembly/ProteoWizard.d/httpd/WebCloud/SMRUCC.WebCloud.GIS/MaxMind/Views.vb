#Region "Microsoft.VisualBasic::2968277412a71632b9eaf65beb17ae40, ..\httpd\WebCloud\SMRUCC.WebCloud.GIS\MaxMind\Views.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Net
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language

Namespace MaxMind.Views

    Public Class Country
        <XmlAttribute> Public Property continent_code As String
        <XmlAttribute> Public Property continent_name As String
        <XmlAttribute> Public Property country_iso_code As String
        <XmlAttribute> Public Property country_name As String
        <XmlElement("city")> Public Property CityLocations As CityLocation()

        Public Overrides Function ToString() As String
            Return String.Format("({0}){1}", country_iso_code, country_name)
        End Function
    End Class

    Public Class CityLocation
        <XmlAttribute> Public Property geoname_id As Long
        <XmlAttribute> Public Property subdivision_1_iso_code As String
        <XmlAttribute> Public Property subdivision_1_name As String
        <XmlAttribute> Public Property subdivision_2_iso_code As String
        <XmlAttribute> Public Property subdivision_2_name As String
        <XmlAttribute> Public Property city_name As String
        <XmlAttribute> Public Property metro_code As String
        <XmlAttribute> Public Property time_zone As String
        <XmlElement("geoLoci")> Public Property GeographicLocations As GeographicLocation()

        Public Function IPLocatedAtCity(IPAddress As Net.IPAddress) As GeographicLocation
            If GeographicLocations.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim LQuery = LinqAPI.DefaultFirst(Of GeographicLocation) <=
 _
                From Location As GeographicLocation
                In Me.GeographicLocations
                Where Not Location Is Nothing AndAlso Location.Locating(IPAddress)
                Select Location

            Return LQuery
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1}/{2}", city_name, subdivision_1_name, subdivision_2_name)
        End Function
    End Class

    Public Class GeographicLocation

        Dim __CIDR As CIDR

        <XmlAttribute("network")> Public Property CIDR As String
            Get
                If __CIDR Is Nothing Then
                    Return ""
                End If
                Return __CIDR.CIDR
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    __CIDR = Nothing
                Else
                    __CIDR = New CIDR(value)
                End If
            End Set
        End Property

        <XmlAttribute> Public Property postal_code As String
        <XmlAttribute> Public Property latitude As Double
        <XmlAttribute> Public Property longitude As Double

        Public Overrides Function ToString() As String
            Return String.Format("[{0}, {1}]  {2}", latitude, longitude, CIDR)
        End Function

        ''' <summary>
        ''' 查看目标IP地址是否落于本地址段之内
        ''' </summary>
        ''' <param name="IP"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Locating(IP As Net.IPAddress) As Boolean
            If Me.__CIDR Is Nothing OrElse __CIDR.Invalid Then
                Return False
            End If
            Return Me.__CIDR.Locating(IP)
        End Function
    End Class

    Public Class CIDR

        Dim StartIP As IPAddress
        Dim EndIP As IPAddress
        Dim __maskBytes As Byte()
        Dim _CIDR_Mask As String

        Public ReadOnly Property CIDR As String
            Get
                Return _CIDR_Mask
            End Get
        End Property

        Shared ReadOnly _InternalZEROIP As IPAddress = IPAddress.Parse("0.0.0.0")
        Shared ReadOnly _Internal255IP As IPAddress = IPAddress.Parse("255.255.255.255")

        Public ReadOnly Property Invalid As Boolean
            Get
                Return StartIP.Equals(_InternalZEROIP) OrElse EndIP.Equals(_Internal255IP)
            End Get
        End Property

        Sub New(CIDR_Mask As String)
            Dim CIDR As String() = CIDR_Mask.Split("/"c)

            If CIDR.Length <> 2 Then
                Throw New Exception($"Target address value ""{CIDR_Mask}"" is not a valid CIDR mask IPAddress value!")
            End If

            'obviously some additional error checking would be delightful
            Dim ip As IPAddress = IPAddress.Parse(CIDR(0))
            Dim bits As Integer = CInt(CIDR(1))
            Dim mask As UInteger = Not (UInteger.MaxValue >> bits)
            Dim ipBytes() As Byte = ip.GetAddressBytes()
            __maskBytes = BitConverter.GetBytes(mask).Reverse().ToArray()
            Dim startIPBytes(ipBytes.Length - 1) As Byte
            Dim endIPBytes(ipBytes.Length - 1) As Byte

            For i As Integer = 0 To ipBytes.Length - 1
                startIPBytes(i) = CByte(ipBytes(i) And __maskBytes(i))
                endIPBytes(i) = CByte(ipBytes(i) Or (Not __maskBytes(i)))
            Next i

            ' You can remove first and last (Network and Broadcast) here if desired

            StartIP = New IPAddress(startIPBytes)
            EndIP = New IPAddress(endIPBytes)
            _CIDR_Mask = CIDR_Mask

            If Me.Invalid Then
                Call (CIDR_Mask & " is not valid!").Warning
                StartIP = ip
            End If
        End Sub

        ''' <summary>
        ''' 查看目标IP地址是否落于本地址段之内
        ''' </summary>
        ''' <param name="IP"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Locating(IP As String) As Boolean
            Return Locating(Net.IPAddress.Parse(IP))
        End Function

        ''' <summary>
        ''' 查看目标IP地址是否落于本地址段之内
        ''' </summary>
        ''' <param name="IP"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Locating(IP As Net.IPAddress) As Boolean
            Dim Tokens = IP.GetAddressBytes
            Dim Start = StartIP.GetAddressBytes
            Dim Ends = EndIP.GetAddressBytes

            For i As Integer = 0 To Tokens.Count - 1
                Dim [byte] As Byte = Tokens(i)
                If Not ([byte] >= Start(i) AndAlso [byte] <= Ends(i)) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Public Overrides Function ToString() As String
            Return StartIP.ToString & " - " & EndIP.ToString & "   [subnet_mask = " &
                __maskBytes(0).ToString & "." & __maskBytes(1).ToString & "." & __maskBytes(2).ToString & "." & __maskBytes(3).ToString & "]"
        End Function
    End Class
End Namespace
