Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.proteomics.PNL.OMICS.MwtWinDll.Extensions.IFormulaFinder

Module Program

    Public Function Main() As Integer
        Call Test()
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    Sub Test()

        Dim fsdfsdf = mzXML.XML.LoadScans("G:\30STD_mix 330ppb-2.mz.XML").Select(AddressOf ExtractMzI).ToArray


        'Dim fff = BitConverter.GetBytes(3.0!)

        'Dim bbb = "eJxz8G3jvr1KodJh0RcGEHCIbfU5Zs5nCefHs+x0KF/0G84vLKyr8l5o4bAJyi9qWBQ0WzUTLl8cMHuD4LbpDnvyIPwSJ79pR0TfweVLkkrDGYU+wvmlQl6G0+dEw/llHyPf/V2zAm5+ZdOSs+oXHsDlax4FBekH74fLNxQXPXFZ+A8u33DQrdBB4y1C/qRbjNqPHgT/omP1uXhhhPqbdg1bxZvg/GbBmijlV+tgfACowGHJ"

        'Dim bytes As Byte() = Convert.FromBase64String(bbb)

        'Call bytes.Length.__DEBUG_ECHO
        'Pause()

        'Dim dataaa = bytes.Split(5)
        'Dim mzInts = dataaa.Select(Function(b) BitConverter.ToSingle(b, Scan0)).Split(2).Select(Function(pair) (mz:=pair(0), intensity:=pair(1))).ToArray




        'Dim mms = bbb.UnzipBase64

        'Call mms.ToArray.FlushStream("x:\ddd.xml")

        'Dim scans = mzXML.XML.LoadScans("X:\Test.Xml").ToArray


        'Call CommonAtoms.SearchByMZAndLimitCharges("-4,6", 100).SaveTo("x:\test.csv")


        Call Data.Load("../../SpectrumChart/Spectrum.json") _
            .Data(Scan0) _
            .Plot(title:="H<sub>2</sub>O<sub>5</sub>C<sub>3</sub>Ag</br><span style=""color:blue; font-size:20"">Test MS/MS spectra plot</span>",
                  showPossibleFormula:=True) _
            .Save("./Plot.png")
    End Sub
End Module
