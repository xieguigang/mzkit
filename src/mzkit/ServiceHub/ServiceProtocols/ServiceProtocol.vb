#Region "Microsoft.VisualBasic::dfb27d933fd3e7d71daa559f71ee73c4, src\mzkit\ServiceHub\ServiceProtocols\ServiceProtocol.vb"

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

    ' Enum ServiceProtocol
    ' 
    '     ExitApp, ExportMzpack, GetBasePeakMzList, GetPixel, GetPixelPolygon
    '     GetPixelRectangle, LoadMSI, LoadMSILayers, LoadSummaryLayer, LoadThermoRawMSI
    '     UnloadMSI
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
    GetBasePeakMzList
    GetPixel
    GetPixelRectangle
    GetPixelPolygon
    LoadSummaryLayer
    ExitApp
End Enum

