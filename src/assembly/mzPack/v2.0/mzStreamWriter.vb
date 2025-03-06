#Region "Microsoft.VisualBasic::61cff39af76e66cd6300a6cb69ee5ec2, assembly\mzPack\v2.0\mzStreamWriter.vb"

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

    '   Total Lines: 352
    '    Code Lines: 239 (67.90%)
    ' Comment Lines: 59 (16.76%)
    '    - Xml Docs: 64.41%
    ' 
    '   Blank Lines: 54 (15.34%)
    '     File Size: 14.51 KB


    ' Module mzStreamWriter
    ' 
    '     Function: getScan1DirName, (+2 Overloads) readme, (+2 Overloads) WriteStream
    ' 
    '     Sub: writeAnnotations, WriteApplicationClass, WriteMultipleStageProductTree, (+2 Overloads) WriteStream
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

''' <summary>
''' helper module for write <see cref="mzPack"/> 
''' </summary>
Public Module mzStreamWriter

    Public Const metadata_json As String = ".etc/metadata.json"
    Public Const annotations_xml As String = ".etc/annotations.xml"

    ''' <summary>
    ''' Write the fly stream of the mzPack raw data
    ''' </summary>
    ''' <param name="scans">the data scan pipeline.</param>
    ''' <param name="file"></param>
    ''' <param name="meta_size"></param>
    ''' <returns></returns>
    <Extension>
    Public Function WriteStream(scans As IEnumerable(Of ScanMS1), file As Stream,
                                Optional source As String = "unknown",
                                Optional meta_size As Long = 8 * ByteSize.MB,
                                Optional [class] As FileApplicationClass = FileApplicationClass.LCMS,
                                Optional metadata As Dictionary(Of String, String) = Nothing) As Boolean

        If metadata Is Nothing Then
            metadata = New Dictionary(Of String, String)
        Else
            ' make value copy
            metadata = New Dictionary(Of String, String)(metadata)
        End If

        Using pack As New StreamPack(file, meta_size:=meta_size)
            Dim summary As New Dictionary(Of String, Double)
            Dim samples As New List(Of String)
            Dim nscans As Integer = 0

            Call pack.Clear(meta_size)
            Call scans.WriteStream(pack, metadata:=summary, samples:=samples, scanNumbers:=nscans)
            Call pack.WriteText(readme([class], nscans, summary), "readme.txt")
            Call metadata.Add("thumbnail", False)
            Call metadata.Add("ms1", nscans)
            Call metadata.Add("create_time", Now.ToString)
            Call metadata.Add("github", "https://github.com/xieguigang/mzkit")
            Call metadata.Add("application", GetType(mzPack).Assembly.ToString)
            Call metadata.Add("platform", If(App.IsMicrosoftPlatform, "Microsoft Windows", "UNIX/LINUX"))
            Call metadata.Add("source", source)

            Call pack.WriteText([class].ToString, applicationClassFile)
            Call pack.WriteText(summary.GetJson, ".etc/ms_scans.json")
            Call pack.WriteText(samples.Distinct.GetJson, ".etc/sample_tags.json")
            Call pack.WriteText(metadata.GetJson, metadata_json)
        End Using

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="file"></param>
    ''' <param name="meta_size">
    ''' too small buffer size will make the data corrept
    ''' if the tree size is greater than this buffer size 
    ''' parameter value.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function WriteStream(mzpack As mzPack, file As Stream, Optional meta_size As Long = 8 * ByteSize.MB) As Boolean
        Dim metadata As New Dictionary(Of String, String)

        Using pack As New StreamPack(file, meta_size:=meta_size)
            Dim summary As New Dictionary(Of String, Double)

            Call pack.Clear(meta_size)
            Call mzpack.WriteStream(pack, index:=summary)
            Call pack.WriteText(mzpack.readme(summary), "readme.txt")
            Call metadata.Add("thumbnail", mzpack.Thumbnail IsNot Nothing)
            Call metadata.Add("ms1", mzpack.MS.TryCount)
            Call metadata.Add("create_time", Now.ToString)
            Call metadata.Add("github", "https://github.com/xieguigang/mzkit")
            Call metadata.Add("application", GetType(mzPack).Assembly.ToString)
            Call metadata.Add("platform", If(App.IsMicrosoftPlatform, "Microsoft Windows", "UNIX/LINUX"))
            Call metadata.Add("source", mzpack.source)

            For Each data As KeyValuePair(Of String, String) In mzpack.metadata.SafeQuery
                metadata(data.Key) = data.Value
            Next

            Call writeAnnotations(mzpack, pack)
            Call pack.WriteText(metadata.GetJson, metadata_json)
        End Using

        Return True
    End Function

    Private Sub writeAnnotations(mzpack As mzPack, pack As StreamPack)
        If mzpack.Annotations.IsNullOrEmpty Then
            Return
        End If

        Dim annos = mzpack.Annotations.Select(Function(v) New NamedValue(v.Key, v.Value)).ToArray
        Dim xml As String = annos.GetXml

        Call pack.WriteText(xml, annotations_xml)
    End Sub

    <Extension>
    Private Function readme([class] As FileApplicationClass, scanNumbers As Integer, summary As Dictionary(Of String, Double)) As String
        Dim sb As New StringBuilder
        Dim app As FileApplicationClass = [class]

        Call sb.AppendLine($"mzPack data v2.0")
        Call sb.AppendLine($"for MZKit application {app.ToString}({app.Description}) data analysis.")
        Call sb.AppendLine()
        Call sb.AppendLine($"has {scanNumbers} ms scans")
        Call sb.AppendLine("summary:")
        Call sb.AppendLine()

        For Each line In summary
            Call sb.AppendLine($"{line.Key}: {line.Value}")
        Next

        Return sb.ToString
    End Function

    <Extension>
    Private Function readme(mzpack As mzPack, summary As Dictionary(Of String, Double)) As String
        Return readme(mzpack.Application, mzpack.MS.Length, summary)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Friend Function getScan1DirName(scan_id As String) As String
        Return scan_id.Replace("\", "/").Replace("/", "_")
    End Function

    ''' <summary>
    ''' the attribute tag name for indicate the sample source in ms1 scan
    ''' </summary>
    Public Const SampleMetaName As String = "sample"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="pack"></param>
    ''' <param name="metadata"></param>
    ''' <param name="samples"></param>
    ''' <param name="scanNumbers">
    ''' get number of the ms1 scans from the given <paramref name="mzpack"/> data stream
    ''' </param>
    <Extension>
    Public Sub WriteStream(mzpack As IEnumerable(Of ScanMS1),
                           pack As StreamPack,
                           ByRef metadata As Dictionary(Of String, Double),
                           Optional ByRef samples As List(Of String) = Nothing,
                           Optional ByRef scanNumbers As Integer = 0)

        Dim rtmin As Double = 99999
        Dim rtmax As Double = -9999
        Dim mzmin As Double = 99999
        Dim mzmax As Double = -9999

        If samples Is Nothing Then
            samples = New List(Of String)
        End If

        For Each ms1 As ScanMS1 In mzpack
            Dim sampleTag As String

            If ms1.hasMetaKeys(SampleMetaName) Then
                sampleTag = ms1.meta(SampleMetaName)
                samples.Add(sampleTag)
            Else
                sampleTag = ""
            End If

            scanNumbers += 1

            Dim dir As String = $"/MS/{sampleTag}/{ms1.scan_id.getScan1DirName}/"
            Dim dirMetadata As New Dictionary(Of String, Object)
            Dim ms1Bin As String = $"{dir}/Scan1.mz"
            Dim ms1Metadata As New Dictionary(Of String, Object)

            Call dirMetadata.Add("scan_id", ms1.scan_id)
            Call dirMetadata.Add("products", ms1.products.TryCount)
            Call dirMetadata.Add("id", ms1.products.SafeQuery.Select(Function(i) i.scan_id).ToArray)

            Using scan1 As New BinaryDataWriter(pack.OpenBlock(ms1Bin)) With {
                .ByteOrder = ByteOrder.LittleEndian
            }
                Call ms1.WriteScan1(scan1)
            End Using

            For Each tag In ms1.meta.SafeQuery
                Call ms1Metadata.Add(tag.Key, tag.Value)
            Next

            ' save all ms2 level spectrum in current ms1 spectrum node into file
            For Each product As ScanMS2 In ms1.products.SafeQuery
                Call WriteMultipleStageProductTree(pack, product, relpath:=New String() {dir, $"{product.scan_id.MD5}.mz"})

                ' 20241227 mzpack v2.1 upgrade
                ' save ms2 spectrum peak annotation data file
                If Not product.metadata.IsNullOrEmpty Then
                    ' has peak annotation metadata
                    ' save to a file aside the ms2 product file
                    Using blockStream As Stream = pack.OpenBlock($"{dir}/{product.scan_id.MD5}.txt")
                        Using str As New StreamWriter(blockStream)
                            For Each line As String In product.metadata
                                Call str.WriteLine(line)
                            Next

                            Call str.Flush()
                        End Using
                    End Using
                End If

                If product.rt > rtmax Then
                    rtmax = product.rt
                End If
                If product.rt < rtmin Then
                    rtmin = product.rt
                End If
                If product.parentMz > mzmax Then
                    mzmax = product.parentMz
                End If
                If product.parentMz < mzmin Then
                    mzmin = product.parentMz
                End If
            Next

            Call pack.SetAttribute(ms1Bin, ms1Metadata)
            Call pack.SetAttribute(dir, dirMetadata)
        Next

        Call metadata.Add(NameOf(mzmin), mzmin)
        Call metadata.Add(NameOf(mzmax), mzmax)
        Call metadata.Add(NameOf(rtmin), rtmin)
        Call metadata.Add(NameOf(rtmax), rtmax)
    End Sub

    Private Sub WriteMultipleStageProductTree(pack As StreamPack, product As ScanMS2, relpath As String())
        Using blockStream As Stream = pack.OpenBlock(relpath.JoinBy("/"))
            If TypeOf blockStream Is SubStream Then
                ' 20230210
                '
                ' has duplicated scan id will cause the
                ' open block function returns a stream
                ' in readonly!

                Throw New InvalidDataException($"A duplicated scan id({product.scan_id}) was found! you should make the product scan id unique!")
            Else
                Dim scan2 As New BinaryDataWriter(blockStream) With {
                    .ByteOrder = ByteOrder.LittleEndian
                }

                Call product.WriteBuffer(scan2)
                Call scan2.Flush()

                ' do not close the writer at here
                ' or the exception of can not access to
                ' a closed stream will be happended
                '
                ' Call scan2.Close()
                If Not product.product Is Nothing Then
                    Call WriteMultipleStageProductTree(
                        pack:=pack,
                        product:=product.product,
                        relpath:=relpath _
                            .Take(relpath.Length - 1) _
                            .JoinIterates(New String() {"products", $"{product.scan_id.MD5}.mz"}) _
                            .ToArray)
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' write the spectrum and scanner data stream
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="pack"></param>
    ''' <param name="index"></param>
    <Extension>
    Private Sub WriteStream(mzpack As mzPack, pack As StreamPack, ByRef index As Dictionary(Of String, Double))
        Dim samples As New List(Of String)

        Call mzpack.MS.WriteStream(pack, index, samples)

        If Not mzpack.Scanners.IsNullOrEmpty Then
            For Each name As String In mzpack.Scanners.Keys
                Dim scanner As ChromatogramOverlapList = mzpack.Scanners(name)

                Using buffer As Stream = pack.OpenBlock($"/Scanners/{name}.cdf")
                    Call scanner.SavePackData(buffer)
                End Using
            Next
        End If

        If Not mzpack.Chromatogram Is Nothing Then
            ' 20220718  put chromatogram.cdf file into the /MS/ 
            ' data directory will cause the ms1 data reader error!
            ' so move this file to root block
            Using buffer As New BinaryDataWriter(pack.OpenBlock("/chromatogram.cdf")) With {
                .ByteOrder = ByteOrder.LittleEndian
            }
                Call buffer.Write(mzpack.Chromatogram.GetBytes)
            End Using
        End If

        Call index.Add("totalIons", mzpack.totalIons)
        Call index.Add("maxIntensity", mzpack.maxIntensity)

        Call pack.WriteText(mzpack.Application.ToString, applicationClassFile)
        Call pack.WriteText(index.GetJson, ".etc/ms_scans.json")
        Call pack.WriteText(samples.Distinct.GetJson, ".etc/sample_tags.json")

        If Not mzpack.Thumbnail Is Nothing Then
            Using snapshot As Stream = pack.OpenBlock("/thumbnail.png")
#If NET48 Then
                Call mzpack.Thumbnail.Save(snapshot, ImageFormat.Png)
#Else
                Call mzpack.Thumbnail.Save(snapshot, ImageFormats.Png)
#End If
            End Using
        End If
    End Sub

    Private Const applicationClassFile As String = ".etc/app.cls"

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub WriteApplicationClass(app As FileApplicationClass, pack As StreamPack)
        Call pack.WriteText(app.ToString, applicationClassFile)
    End Sub
End Module
