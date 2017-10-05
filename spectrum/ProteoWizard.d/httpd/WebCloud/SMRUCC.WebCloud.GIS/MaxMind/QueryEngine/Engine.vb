#Region "Microsoft.VisualBasic::e5b271eecce3c01b019eca60d7c8965b, ..\httpd\WebCloud\SMRUCC.WebCloud.GIS\MaxMind\QueryEngine\Engine.vb"

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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.WebCloud.GIS.MaxMind.geolite2

Namespace MaxMind

    ''' <summary>
    ''' 地理位置服务查询引擎
    ''' </summary>
    Public Class Engine

        Public ReadOnly Property MySQl As MySQL

        Sub New(uri As ConnectionUri)
            Me.MySQl = uri
        End Sub

        Private ReadOnly Mask As String() = {"255.0.0.0", "255.255.0.0", "255.255.255.0"}

        Public Function LocationQuery(IPAddress As String) As FindResult
            If String.IsNullOrEmpty(IPAddress) OrElse Not Net.IPAddress.TryParse(IPAddress, Nothing) Then
                Return FindResult.Null
            End If

            Dim CIDR As String() = LinqAPI.Exec(Of String) <=
 _
                From mask As String
                In Me.Mask
                Select s = New IPv4(IPAddress, mask).CIDR

            Dim LQuery = LinqAPI.DefaultFirst(Of geolite2_city_blocks_ipv4) <=
 _
                From s_cidr As String
                In CIDR
                Let SQL As String = $"SELECT * FROM geoip_services.geolite2_city_blocks_ipv4 where network = '{s_cidr}' LIMIT 1;"
                Let Query = MySQl.ExecuteScalar(Of geolite2_city_blocks_ipv4)(SQL)
                Where Not Query Is Nothing
                Select Query

            If LQuery Is Nothing Then
                Return FindResult.Null
            End If

            Dim citySQL As String = $"SELECT * FROM geoip_services.geolite2_city_locations where geoname_id = '{LQuery.geoname_id}' LIMIT 1;"
            Dim CityLocation = MySQl.ExecuteScalar(Of geolite2_city_locations)(citySQL)

            If CityLocation Is Nothing Then
                CityLocation = New geolite2_city_locations
            End If

            Return New FindResult With {
                .CIDR = LQuery.network,
                .city_name = CityLocation.city_name,
                .continent_code = CityLocation.continent_code,
                .continent_name = CityLocation.continent_name,
                .country_iso_code = CityLocation.country_iso_code,
                .country_name = CityLocation.country_name,
                .geoname_id = CityLocation.geoname_id,
                .latitude = LQuery.latitude,
                .longitude = LQuery.longitude,
                .metro_code = CityLocation.metro_code,
                .postal_code = LQuery.postal_code,
                .subdivision_1_iso_code = CityLocation.subdivision_1_iso_code,
                .subdivision_1_name = CityLocation.subdivision_1_name,
                .subdivision_2_iso_code = CityLocation.subdivision_2_iso_code,
                .subdivision_2_name = CityLocation.subdivision_2_name,
                .time_zone = CityLocation.time_zone
            }
        End Function
    End Class
End Namespace
