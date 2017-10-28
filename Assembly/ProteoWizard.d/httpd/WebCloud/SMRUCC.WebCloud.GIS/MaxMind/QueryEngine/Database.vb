#Region "Microsoft.VisualBasic::7b9b3c923c40dc0e259cf9e5fddc1633, ..\httpd\WebCloud\SMRUCC.WebCloud.GIS\MaxMind\QueryEngine\Database.vb"

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
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.GIS.MaxMind.geolite2
Imports SMRUCC.WebCloud.GIS.MaxMind.Views

Namespace MaxMind

    ''' <summary>
    ''' XML数据库查询
    ''' </summary>
    <XmlRoot("geoip")> Public Class Database : Inherits ClassObject

        <XmlElement> Public Property Countries As Views.Country()

        Private Class __cityGeo

            Public city As CityLocation
            Public geo As GeographicLocation

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Class

        Private Shared Function __findCity(country As Country, ip As IPAddress) As __cityGeo
            Return LinqAPI.DefaultFirst(Of __cityGeo) <=
 _
                From city As CityLocation
                In country.CityLocations
                Let Location = city.IPLocatedAtCity(ip)
                Where Not Location Is Nothing
                Select New __cityGeo With {
                    .city = city,
                    .geo = Location
                }
        End Function

        Public Function FindLocation(s_IPAddress As String) As FindResult
            Dim IPAddress As IPAddress = IPAddress.Parse(s_IPAddress)
            Dim LQuery = From Country As Country
                         In Countries.AsParallel
                         Let FindCity = __findCity(Country, IPAddress)
                         Where Not FindCity Is Nothing
                         Select FoundCity = FindCity,
                             Country

            Dim FoundResult = LQuery.FirstOrDefault

            If FoundResult Is Nothing Then
                Return Nothing
            End If

            Return New FindResult With {
                .CIDR = FoundResult.FoundCity.geo.CIDR,
                .city_name = FoundResult.FoundCity.city.city_name,
                .continent_code = FoundResult.Country.continent_code,
                .continent_name = FoundResult.Country.continent_name,
                .country_iso_code = FoundResult.Country.country_iso_code,
                .country_name = FoundResult.Country.country_name,
                .geoname_id = FoundResult.FoundCity.city.geoname_id,
                .latitude = FoundResult.FoundCity.geo.latitude,
                .longitude = FoundResult.FoundCity.geo.longitude,
                .metro_code = FoundResult.FoundCity.city.metro_code,
                .postal_code = FoundResult.FoundCity.geo.postal_code,
                .subdivision_1_iso_code = FoundResult.FoundCity.city.subdivision_1_iso_code,
                .subdivision_1_name = FoundResult.FoundCity.city.subdivision_1_name,
                .subdivision_2_iso_code = FoundResult.FoundCity.city.subdivision_2_iso_code,
                .subdivision_2_name = FoundResult.FoundCity.city.subdivision_2_name,
                .time_zone = FoundResult.FoundCity.city.time_zone
            }
        End Function

        ''' <summary>
        ''' 都按照国家的代码进行分组
        ''' </summary>
        ''' <param name="GeoLite2_City_Blocks">GeoLite2-City-Blocks-IPv4.csv  161MB</param>
        ''' <param name="GeoLite2_City_Locations">GeoLite2-City-Locations-en.csv</param>
        ''' <param name="GeoLite2_Country_Blocks">GeoLite2-Country-Blocks-IPv4.csv</param>
        ''' <param name="GeoLite2_Country_Locations">GeoLite2-Country-Locations-en.csv</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LoadFromCsv(GeoLite2_City_Blocks As String,
                                           GeoLite2_City_Locations As String,
                                           GeoLite2_Country_Blocks As String,
                                           GeoLite2_Country_Locations As String) As Database
            Call Console.WriteLine("Start GeoBlocksCsv task...")
            Dim GeoBlocksCsv = New Task(Of String, geolite2_city_blocks_ipv4())(GeoLite2_City_Blocks,
                                                                   Handle:=Function(Path As String) Path.LoadCsv(Of geolite2_city_blocks_ipv4)(False).ToArray)
            Call Console.WriteLine("Start load CityLocations...")
            Dim CityLocations As geolite2_city_locations() = GeoLite2_City_Locations.LoadCsv(Of geolite2_city_locations)(False).ToArray
            Call Console.WriteLine("Start load GeoBlocks...")
            Dim GeoBlocks = (From Location In GeoBlocksCsv.GetValue.AsParallel Select Location Group By Location.geoname_id Into Group).ToArray.ToDictionary(Function(Location) Location.geoname_id, elementSelector:=Function(data) data.Group.ToArray)
            Call Console.WriteLine("Query country cities...")
            Dim CountryCities = (From City In CityLocations.AsParallel Select City Group By City.country_iso_code Into Group).ToArray
            Call Console.WriteLine("Generate countries data....")
            Dim CountryLQuery = (From Country In CountryCities.AsParallel Select __country(Country.Group.ToArray, GeoBlocks)).ToArray
            Call Console.WriteLine("Xml database compile job done!")
            Return New Database With {.Countries = CountryLQuery}
        End Function

        Private Shared Function __country(Cities As geolite2_city_locations(), GeoBlocks As Dictionary(Of Long, geolite2_city_blocks_ipv4())) As Views.Country
            Dim CountryData As geolite2_city_locations = Cities.First
            Dim Country As New Country With {
                .continent_code = CountryData.continent_code,
                .continent_name = CountryData.continent_name,
                .country_iso_code = CountryData.country_iso_code,
                .country_name = CountryData.country_name,
                .CityLocations = LinqAPI.Exec(Of CityLocation) <=
 _
                    From City As geolite2_city_locations
                    In Cities
                    Let GeoBlocksData = If(
                        GeoBlocks.ContainsKey(City.geoname_id),
                        GeoBlocks(City.geoname_id),
                        New geolite2_city_blocks_ipv4() {})
                    Select New CityLocation With {
                        .GeographicLocations = __geographicLocation(GeoBlocksData),
                        .city_name = City.city_name,
                        .geoname_id = City.geoname_id,
                        .metro_code = City.metro_code,
                        .subdivision_1_iso_code = City.subdivision_1_iso_code,
                        .subdivision_1_name = City.subdivision_1_name,
                        .subdivision_2_iso_code = City.subdivision_2_iso_code,
                        .subdivision_2_name = City.subdivision_2_name,
                        .time_zone = City.time_zone
                    }
               }

            Return Country
        End Function

        Private Shared Function __geographicLocation(data As geolite2_city_blocks_ipv4()) As Views.GeographicLocation()
            Return LinqAPI.Exec(Of GeographicLocation) <=
 _
                From Location As geolite2_city_blocks_ipv4
                In data
                Select New Views.GeographicLocation With {
                    .latitude = Location.latitude,
                    .longitude = Location.longitude,
                    .CIDR = Location.network,
                    .postal_code = Location.postal_code
                }
        End Function
    End Class
End Namespace
