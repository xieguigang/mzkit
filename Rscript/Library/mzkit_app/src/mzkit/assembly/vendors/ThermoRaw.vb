#Region "Microsoft.VisualBasic::941dce3dba42fb5c5eed4cb774266159, Rscript\Library\mzkit_app\src\mzkit\assembly\vendors\ThermoRaw.vb"

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

    '   Total Lines: 80
    '    Code Lines: 61
    ' Comment Lines: 6
    '   Blank Lines: 13
    '     File Size: 2.92 KB


    ' Module ThermoRaw
    ' 
    '     Function: events, logs, MSIPixels, OpenRaw, readAsMSI
    '               readRawScan
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Or NET48 Then
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports rDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
#End If

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("ThermoRaw")>
Module ThermoRaw

#If netcore5 = 0 Or NET48 Then

    ''' <summary>
    ''' open a Thermo raw file
    ''' </summary>
    ''' <param name="rawfile">the file path of the ``*.raw``.</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("open.raw")>
    Public Function OpenRaw(rawfile As String) As MSFileReader
        Return New MSFileReader(rawfile)
    End Function

    <ExportAPI("read.rawscan")>
    Public Function readRawScan(raw As MSFileReader, scanId As Integer) As SingleScanInfo
        Return raw.GetScanInfo(scanId)
    End Function

    <ExportAPI("logs")>
    Public Function logs(scan As SingleScanInfo) As rDataframe
        Dim key As Array = scan.StatusLog.Select(Function(evt) evt.Key).ToArray
        Dim evts As Array = scan.StatusLog.Select(Function(evt) evt.Value).ToArray

        Return New rDataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"log", key},
                {"text", evts}
            }
        }
    End Function

    <ExportAPI("load_MSI")>
    <RApiReturn(GetType(mzPack))>
    Public Function readAsMSI(raw As MSFileReader, <RRawVectorArgument> pixels As Object, Optional env As Environment = Nothing) As Object
        Dim size As Size = InteropArgumentHelper.getSize(pixels, env, "-1,-1").SizeParser

        If size.Width <= 0 OrElse size.Height <= 0 Then
            Return Internal.debug.stop({$"the given pixels size parameter value '{pixels}' is not a valid data!", $"value: {pixels}"}, env)
        End If

        Return raw.LoadFromXMSIRaw(pixels:=size)
    End Function

    <ExportAPI("events")>
    Public Function events(scan As SingleScanInfo) As rDataframe
        Dim key As Array = scan.ScanEvents.Select(Function(evt) evt.Key).ToArray
        Dim evts As Array = scan.ScanEvents.Select(Function(evt) evt.Value).ToArray

        Return New rDataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"event", key},
                {"data", evts}
            }
        }
    End Function
#End If

    <ExportAPI("MSI_pixels")>
    Public Function MSIPixels(mzpack As mzPack) As DataSet()
        Return mzpack.ExactPixelTable
    End Function
End Module
