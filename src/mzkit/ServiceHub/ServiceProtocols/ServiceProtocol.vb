#Region "Microsoft.VisualBasic::677a2360789d9044615d1198922ddb58, mzkit\src\mzkit\ServiceHub\ServiceProtocols\ServiceProtocol.vb"

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

'   Total Lines: 20
'    Code Lines: 14
' Comment Lines: 6
'   Blank Lines: 0
'     File Size: 397.00 B


' Enum ServiceProtocol
' 
'     ExitApp, ExportMzpack, GetBasePeakMzList, GetIonStatList, GetPixel
'     GetPixelPolygon, GetPixelRectangle, LoadMSI, LoadMSILayers, LoadSummaryLayer
'     LoadThermoRawMSI, UnloadMSI
' 
'  
' 
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Parallel

Public Enum ServiceProtocol
    ''' <summary>
    ''' load MSI engine from mzpack
    ''' </summary>
    LoadMSI
    ''' <summary>
    ''' load MSI engine from thermo raw
    ''' </summary>
    LoadThermoRawMSI
    UnloadMSI
    ExportMzpack
    LoadMSILayers
    GetIonStatList
    GetBasePeakMzList
    GetPixel
    GetPixelRectangle
    GetPixelPolygon
    LoadSummaryLayer
    CutBackground
    ExitApp
End Enum

Public Module MSIProtocols

    Public Function LoadPixels(mz As IEnumerable(Of Double), mzErr As Tolerance, handleServiceRequest As Func(Of RequestStream, RequestStream)) As PixelData()
        Dim config As New LayerLoader With {.mz = mz.ToArray, .method = If(TypeOf mzErr Is PPMmethod, "ppm", "da"), .mzErr = mzErr.DeltaTolerance}
        Dim configBytes As Byte() = BSON.GetBuffer(config.GetType.GetJsonElement(config, New JSONSerializerOptions)).ToArray
        Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadMSILayers, configBytes))

        If data Is Nothing Then
            Return {}
        Else
            Dim pixels As PixelData() = PixelData.Parse(data.ChunkBuffer)
            'Dim points = pixels.Select(Function(p) New ClusterEntity With {.uid = $"{p.x},{p.y}", .entityVector = {p.x, p.y}}).ToArray
            'Dim densityList = Density.GetDensity(points, k:=stdNum.Min(points.Length / 10, 150), query:=New KDQuery(points)).ToArray
            Return pixels
        End If
    End Function
End Module