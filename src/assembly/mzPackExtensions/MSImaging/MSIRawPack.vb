#Region "Microsoft.VisualBasic::57b7990107997052ca389a13f2627fb1, assembly\mzPackExtensions\MSImaging\MSIRawPack.vb"

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

    '   Total Lines: 262
    '    Code Lines: 191 (72.90%)
    ' Comment Lines: 26 (9.92%)
    '    - Xml Docs: 65.38%
    ' 
    '   Blank Lines: 45 (17.18%)
    '     File Size: 9.88 KB


    ' Module MSIRawPack
    ' 
    '     Function: ExactPixelTable, LoadFromXMSIRaw, (+2 Overloads) LoadMSIFromSCiLSLab, LoadMSISpotsFromSCiLSLab, PixelScaler
    '               PixelScanId, (+2 Overloads) ScalePixels
    ' 
    ' /********************************************************************************/

#End Region

#If NET48 Then
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
#End If

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader.SCiLSLab
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math

''' <summary>
''' read SCiLSLab table export or Xcalibur Raw data file for MS-imaging
''' </summary>
Public Module MSIRawPack

#If NET48 Then

    ''' <summary>
    ''' single raw data file as MSI data
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="pixels">
    ''' 扫描在[X,Y]上的像素点数量
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadFromXMSIRaw(raw As MSFileReader, pixels As Size) As mzPack
        Dim loader As New XRawStream(raw, PixelScanId(pixels, maxrt:=raw.ScanTimeMax))
        Dim pack As mzPack = loader.StreamTo(skipEmptyScan:=False)

        If pixels.Width * pixels.Height <> pack.MS.Length Then
            Call $"Data inconsistent: image pixels number ({pixels.Width * pixels.Height}) is not equals to scan numbers ({pack.MS.Length})!".Warning
        End If

        Return pack
    End Function

    Private Function PixelScanId(pixels As Size, maxrt As Double) As Func(Of SingleScanInfo, Integer, String)
        Dim cal As New ScanTimeCorrection(totalTime:=maxrt, pixels:=pixels.Width)

        Return Function(scan, n)
                   If scan.MSLevel = 1 Then
                       Dim pt As Point = cal.GetPixelPoint(scan.RetentionTime)

                       Return $"[MS1][Scan_{scan.ScanNumber}][{pt.X},{pt.Y}] {scan.FilterText}"
                   Else
                       Return $"[MSn][Scan_{scan.ScanNumber}] {scan.FilterText}"
                   End If
               End Function
    End Function

#End If

    <Extension>
    Public Function ExactPixelTable(mzpack As mzPack) As DataSet()
        Dim mz As New Dictionary(Of String, DataSet)

        For Each scan As ScanMS1 In mzpack.MS
            Dim pixel As String = scan.scan_id.Match("\[\d+,\d+\]")

            For i As Integer = 0 To scan.mz.Length - 1
                Dim mzi As String = scan.mz(i).ToString("F4")

                If Not mz.ContainsKey(mzi) Then
                    mz.Add(mzi, New DataSet With {.ID = mzi})
                End If

                mz(mzi)(pixel) = stdNum.Max(mz(mzi)(pixel), scan.into(i))
            Next
        Next

        Return mz.Values.ToArray
    End Function

    ''' <summary>
    ''' imports one sample file that contains multiple region data inside
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="println"></param>
    ''' <returns></returns>
    Public Function LoadMSIFromSCiLSLab(files As IEnumerable(Of (index$, msdata$)),
                                        Optional println As Action(Of String) = Nothing,
                                        Optional verbose As Boolean = True) As mzPack

        Dim pixels As New List(Of ScanMS1)
        Dim rawfiles As New List(Of String)
        Dim mute As Action(Of String) =
            Sub()
                ' do nothing
            End Sub

        If println Is Nothing Then
            println =
                Sub()
                    ' do nothing
                End Sub
        End If

        For Each tuple As (index$, msdata$) In files
            Dim sampleTag As String = tuple.index.BaseName

            Call println($" --> {sampleTag}")

            Using spots As Stream = tuple.index.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Using msdata As Stream = tuple.msdata.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Call LoadMSISpotsFromSCiLSLab(spots, msdata, 1, 1, sampleTag, If(verbose, println, mute)) _
                        .DoCall(AddressOf pixels.AddRange)
                End Using
            End Using

            Call rawfiles.Add(PackFile.ParseHeader(tuple.index).raw.FileName)
        Next

        If rawfiles.Distinct.Count > 1 Then
            Call println("[warning] the given spots data comes from multiple raw data file!")
        End If

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging,
            .MS = pixels.ToArray.ScalePixels,
            .source = rawfiles.Distinct.JoinBy("; ")
        }
    End Function

    Private Iterator Function LoadMSISpotsFromSCiLSLab(spots As Stream, msdata As Stream,
                                                       minX#,
                                                       minY#,
                                                       sampleTag As String,
                                                       println As Action(Of String)) As IEnumerable(Of ScanMS1)

        Dim spotsXy As SpotPack = SpotPack.ParseFile(spots)
        Dim spotsMs As MsPack = MsPack.ParseFile(msdata, println)
        Dim i As i32 = Scan0
        Dim hasTagdata As Boolean = Not sampleTag.StringEmpty
        Dim refTagdata As String = If(hasTagdata, $"{sampleTag}.", "")

        For Each spot As SpotMs In spotsMs.matrix
            Dim ref As String = (Integer.Parse(spot.spot_id.Match("\d+")) - 1).ToString
            Dim xy As SpotSite = spotsXy.index(ref)
            Dim into As New Vector(spot.intensity)
            Dim mz As New Vector(spotsMs.mz)
            Dim sx = CInt(xy.x - minX) + 1
            Dim sy = CInt(xy.y - minY) + 1

            ' 20220719
            ' x,y should be start from the 1, not ZERO
            ' or pixel drawing will be error!
            Dim ms1 As New ScanMS1 With {
                .BPC = spot.intensity.Max,
                .TIC = spot.intensity.Sum,
                .into = into(into > 0),
                .meta = New Dictionary(Of String, String) From {
                    {"x", sx},
                    {"y", sy},
                    {"spot_id", xy.index}
                },
                .mz = mz(into > 0),
                .rt = ++i,
                .scan_id = $"[MS1][{CInt(xy.x)},{CInt(xy.y)}] {refTagdata}{spot.spot_id} totalIon:{ .TIC.ToString("G5")}"
            }

            If hasTagdata Then
                Call ms1.meta.Add("sample", sampleTag)
                Call println($"[{sampleTag}] {ms1.scan_id}")
            Else
                Call println(ms1.scan_id)
            End If

            Yield ms1
        Next
    End Function

    Public Function LoadMSIFromSCiLSLab(spots As Stream,
                                        msdata As Stream,
                                        Optional println As Action(Of String) = Nothing) As mzPack

        Dim spotsXy As SpotPack = SpotPack.ParseFile(spots)
        Dim spotsList As New List(Of ScanMS1)
        Dim minX As Double = spotsXy.X.Min
        Dim minY As Double = spotsXy.Y.Min
        Dim sampleTag As String = spotsXy.raw.BaseName

        If println Is Nothing Then
            println = Sub()
                          ' do nothing
                      End Sub
        End If

        ' reset the stream position to init0 
        ' due to the reason of SpotPack.ParseFile has
        ' been moved
        spots.Seek(Scan0, SeekOrigin.Begin)
        spotsList.AddRange(LoadMSISpotsFromSCiLSLab(spots, msdata, minX, minY, sampleTag, println))

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging,
            .MS = spotsList.ToArray,
            .source = spotsXy.raw.FileName
        }
    End Function

    <Extension>
    Public Function PixelScaler(raw As ScanMS1()) As SizeF
        Dim vx As Double() = raw _
            .Select(Function(m) Val(m.meta("x"))) _
            .OrderBy(Function(xi) xi) _
            .Distinct _
            .ToArray
        Dim vy As Double() = raw _
            .Select(Function(m) Val(m.meta("y"))) _
            .OrderBy(Function(xi) xi) _
            .Distinct _
            .ToArray
        Dim dx As Double() = NumberGroups.diff(vx)
        Dim dy As Double() = NumberGroups.diff(vy)
        Dim ddx As Double = dx.OrderByDescending(Function(xi) xi).Take(dx.Length * 2 / 3).Average
        Dim ddy As Double = dy.OrderByDescending(Function(yi) yi).Take(dy.Length * 2 / 3).Average

        Return New SizeF(ddx, ddy)
    End Function

    <Extension>
    Public Function ScalePixels(data As ScanMS1(), Optional flip As Boolean = True) As ScanMS1()
        Dim scaler = data.PixelScaler
        Dim dx As Double = scaler.Width
        Dim dy As Double = scaler.Height
        Dim h As Double = data _
            .Select(Function(m) Val(m.meta("y"))) _
            .Max

        h /= dy

        For Each pixelScan As ScanMS1 In data
            Dim x As Integer = Val(pixelScan.meta("x")) / dx
            Dim y As Integer = Val(pixelScan.meta("y")) / dy

            If flip Then
                y = h - y + 1
            End If

            pixelScan.meta("x") = x
            pixelScan.meta("y") = y
            pixelScan.scan_id = $"{pixelScan.scan_id}@[{x},{y}]"
        Next

        Return data
    End Function

    <Extension>
    Public Function ScalePixels(data As mzPack) As mzPack
        data.MS = data.MS.ScalePixels
        Return data
    End Function
End Module
