#Region "Microsoft.VisualBasic::39a511c9108d3f098be2786d37ff00bd, test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: ChromatogramPlotTest, Main, ms1Visual, mzrtPlotTest
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Assembly.MarkupData
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.Ms1
Imports SMRUCC.MassSpectrum.Visualization

Module Module1

    Sub mzrtPlotTest()
        Dim sample1 = "C:\Users\gg.xie\Desktop\peak_tablenew.csv".LoadCsv(Of Ms1Feature)
        Dim sample2 = "C:\Users\gg.xie\Desktop\doMSMSalignment.report1_back.csv".LoadCsv(Of Ms1Feature)
        Dim hits = "C:\Users\gg.xie\Desktop\doMSMSalignment.report1.csv".LoadCsv(Of Ms1Feature)
        Dim samples = {
            New NamedValue(Of IMs1())("peak_tablenew", sample1),
            New NamedValue(Of IMs1())("doMSMSalignment.report1", sample2),
            New NamedValue(Of IMs1())("peaktable match ms2", hits)
        }

        Call MzrtPlot.Plot(samples, size:="8000,3000", sampleColors:="blue,green,red").Save("C:\Users\gg.xie\Desktop\ms1_compares_ms2.png")

        Pause()
    End Sub

    Sub ms1Visual()

        Dim matrix = "D:\Resources\40\40.mzML" _
            .PopulateMS1 _
            .Ms1Chromatogram(New DAmethod With {.Threshold = 0.3}) _
            .Select(Function(mz)
                        Dim table = mz.chromatogram.GroupBy(Function(t) t.Time).Select(Function(c) New ChromatogramTick With {.Time = c.Key, .Intensity = c.Select(Function(s) s.Intensity).Max}).ToArray
                        Return New NamedCollection(Of ChromatogramTick) With {.Name = mz.mz, .Value = table}
                    End Function) _
            .Where(Function(m) m.Value.Length > 50) _
            .ToArray


        ' Call Contour.Plot(matrix, unit:=10, size:="20000,8000").Save("./ms1-contour.png")

        'Dim scans = matrix _
        '    .Select(Function(mzGroup)
        '                Return mzGroup.chromatogram.Select(Function(c) New ms1_scan With {.mz = mzGroup.mz, .intensity = c.Intensity, .scan_time = c.Time})
        '            End Function) _
        '    .IteratesALL _
        '    .ToArray

        ' 生成矩阵进行web可视化
        '
        ' mz
        ' ^
        ' |
        ' |
        ' ----> rt
        '
        'Dim times = matrix.Select(Function(mz) mz.Value.Select(Function(c) c.Time)).IteratesALL.Distinct.OrderBy(Function(s) s).Indexing
        'Dim into_matrix = matrix.Select(Function(mz)
        '                                    Dim row = New Double(times.Count - 1) {}

        '                                    For Each time In mz.Value
        '                                        Dim i = times.IndexOf(time.Time)
        '                                        row(i) = time.Intensity
        '                                    Next

        '                                    Return row
        '                                End Function).ToArray

        'Call into_matrix.MatrixJson.SaveTo("./into.json")

        'Call scans.SaveTo("./test_ms1_scan.csv")
        Call matrix.TICplot("8000,3000", labelLayoutTicks:=5, showLabels:=False, fillCurve:=False).Save("./ms1.plot.png")

        Pause()
    End Sub

    Sub Main()

        Call mzrtPlotTest()

        Call ms1Visual()

        Call ChromatogramPlotTest()


        ' Call Test()

        'Dim ddd = SMRUCC.proteomics.MS_Spectrum.DATA.Statistics.KEGGPathwayCoverages("C:\Users\gg.xie\Desktop\KEGG_ALL.csv".ReadAllLines, "D:\smartnucl_integrative\DATA\KEGG\br08901")
        'Dim ff As New File

        'ff += {"pathway", "cover", "ALL", "coverage%"}

        'For Each row In ddd
        '    ff += New String() {row.Key, row.Value.cover, row.Value.ALL, If(row.Value.ALL = 0, 0, (row.Value.cover / row.Value.ALL) * 100%)}
        'Next

        'Call ff.Save("eeeee.csv", Encodings.ASCII)


        'Pause()

        'Dim unknown As Integer = 0
        'Dim result = SMRUCC.proteomics.MS_Spectrum.DATA.Statistics.HMDBCoverages("C:\Users\xieguigang\Desktop\New folder\metlin.hmdb.txt".ReadAllLines, "C:\Users\xieguigang\Desktop\hmdb_metabolites.xml", unknown)

        'Dim pie = result.Where(Function(item) Not item.Key.StringEmpty).Where(Function(n) n.Value.cover > 0).ToArray
        'Dim csv As New File

        'csv += {"Class", "Percentage"}

        'For Each item In pie
        '    csv += {item.Key, CStr(item.Value.cover / item.Value.ALL)}
        'Next

        'Call csv.Save("x:\dsgfdfgdf.csv")

        'Pause()

        'For Each m In SMRUCC.proteomics.MS_Spectrum.DATA.HMDB.metabolite.Load("C:\Users\xieguigang\Desktop\hmdb_metabolites.xml")

        '    m.name.__DEBUG_ECHO

        'Next

        'Dim rs = SMRUCC.proteomics.MS_Spectrum.DATA.Massbank.RecordIO.ScanLoad("D:\smartnucl_integrative\DATA\OpenData\record\Athens_Univ\")

        'Pause()
    End Sub

    Sub ChromatogramPlotTest()

        Dim c90 = Math.Cos(0.5 * Math.PI)
        Dim c0 = Math.Cos(0)


        Dim ions = "D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\smartnucl.CVD_kb\ion_pair.csv".LoadCsv(Of IonPair)
        Dim ionData = LoadChromatogramList("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\smartnucl.CVD_kb\test\Data20180111-L1.mzML") _
            .MRMSelector(ions) _
            .Where(Function(ion) Not ion.chromatogram Is Nothing) _
            .Select(Function(ion)
                        Return New NamedValue(Of ChromatogramTick()) With {
                            .Name = ion.ion.name,
                            .Description = ion.ion.ToString,
                            .Value = ion.chromatogram.Ticks
                        }
                    End Function) _
            .ToArray



        For Each ion In ionData

            Dim base = ion.Value.Baseline(quantile:=0.65)
            Dim max = ion.Value.Shadows!Intensity.Max

            Call $"{ion.Name}:  {base}/{max} = {(100 * base / max).ToString("F2")}%".__DEBUG_ECHO

        Next


        'For Each ion In ionData
        '    Call ion _
        '        .Value _
        '        .Plot(title:=ion.Name, showMRMRegion:=True, debug:=True) _
        '        .AsGDIImage _
        '        .SaveAs($"./{ion.Name.NormalizePathString.Replace(" ", "-")}_chromatogram.png")
        'Next

        Pause()
    End Sub


    'Sub Test()




    '    For Each ttt In "D:\smartnucl_integrative\20170705_library_mzXML\output\C18-_standards\lib.neg.csv".LoadCsv(Of LibraryMatrix).SpectrumFromMatrix
    '        Call ttt.Plot(title:=ttt.name,
    '                      showPossibleFormula:=True,
    '                      mzAxis:=Nothing,
    '                      showLegend:=False) _
    '        .Save("./test/" & ttt.name.NormalizePathString(True) & ".png")
    '    Next


    '    Dim fsdfsdf = mzXML.XML.LoadScans("G:\30STD_mix 330ppb-2.mz.XML").Select(AddressOf ExtractMzI).ToArray


    '    'Dim fff = BitConverter.GetBytes(3.0!)

    '    'Dim bbb = "eJxz8G3jvr1KodJh0RcGEHCIbfU5Zs5nCefHs+x0KF/0G84vLKyr8l5o4bAJyi9qWBQ0WzUTLl8cMHuD4LbpDnvyIPwSJ79pR0TfweVLkkrDGYU+wvmlQl6G0+dEw/llHyPf/V2zAm5+ZdOSs+oXHsDlax4FBekH74fLNxQXPXFZ+A8u33DQrdBB4y1C/qRbjNqPHgT/omP1uXhhhPqbdg1bxZvg/GbBmijlV+tgfACowGHJ"

    '    'Dim bytes As Byte() = Convert.FromBase64String(bbb)

    '    'Call bytes.Length.__DEBUG_ECHO
    '    'Pause()

    '    'Dim dataaa = bytes.Split(5)
    '    'Dim mzInts = dataaa.Select(Function(b) BitConverter.ToSingle(b, Scan0)).Split(2).Select(Function(pair) (mz:=pair(0), intensity:=pair(1))).ToArray




    '    'Dim mms = bbb.UnzipBase64

    '    'Call mms.ToArray.FlushStream("x:\ddd.xml")

    '    'Dim scans = mzXML.XML.LoadScans("X:\Test.Xml").ToArray


    '    'Call CommonAtoms.SearchByMZAndLimitCharges("-4,6", 100).SaveTo("x:\test.csv")

    '    Dim title = <p>
    '                    H<sub>2</sub>O<sub>5</sub>C<sub>3</sub>Ag
    '                    <br/>
    '                    <span style="color:blue; font-size:20">Test MS/MS spectra plot</span>
    '                </p>

    '    Call MetlinData.Load("../../SpectrumChart/Spectrum.json") _
    '        .Data(Scan0) _
    '        .Plot(title:=title.ToString,
    '              showPossibleFormula:=True) _
    '        .Save("./Plot.png")
    'End Sub
End Module
