#Region "Microsoft.VisualBasic::9e3f7379715f9b5f5faddafd3a95ab19, assembly\assembly\test\Module1.vb"

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

    '   Total Lines: 64
    '    Code Lines: 16 (25.00%)
    ' Comment Lines: 27 (42.19%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 21 (32.81%)
    '     File Size: 2.44 KB


    ' Module Module1
    ' 
    '     Sub: Main1, populateMS2
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML

Module Module1

    Sub populateMS2()
        Dim mzML = "D:\XT-ZWA-1.mzML"

        For Each x In mzML.PopulateMS2



            Pause()
        Next

    End Sub

    Sub Main1()

        Call populateMS2()


        'Dim ions = "D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\smartnucl.CVD_kb\ion_pair.csv".LoadCsv(Of IonPair)
        'Dim ionData = LoadChromatogramList("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\smartnucl.CVD_kb\test\Data20180111-L1.mzML") _
        '    .MRMSelector(ions) _
        '    .Where(Function(ion) Not ion.chromatogram Is Nothing) _
        '    .Select(Function(ion)
        '                Return New NamedValue(Of PointF()) With {
        '                    .Name = ion.ion.name,
        '                    .Description = ion.ion.ToString,
        '                    .Value = ion.chromatogram.Ticks.Select(Function(tick) CType(tick, PointF)).ToArray
        '                }
        '            End Function) _
        '    .ToArray

        'Pause()


        Dim testMsp = MspData.Load("D:\MassSpectrum-toolkits\Assembly\test\HMDB00008.msp").ToArray


        Dim lysoPC = MspData.Load("D:\MassSpectrum-toolkits\Assembly\test\custom-lysoPC+Hpos.msp")

        'Dim meta = lysoPC.First.Comments.LipidBlastParser

        'Call meta.DictionaryTable(where:=Function(x)
        '                                     If x Is Nothing Then
        '                                         Return False
        '                                     ElseIf {GetType(Integer), GetType(Double), GetType(Long)}.IndexOf(x.GetType) > -1 Then
        '                                         If Val(x) = 0R Then
        '                                             Return False
        '                                         Else
        '                                             Return True
        '                                         End If
        '                                     Else
        '                                         Return True
        '                                     End If
        '                                 End Function).GetJson.__DEBUG_ECHO


        Pause()
    End Sub

End Module
