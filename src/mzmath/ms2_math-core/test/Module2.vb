﻿#Region "Microsoft.VisualBasic::077b9c3cb4ef66876ec602d10c6f516b, mzmath\ms2_math-core\test\Module2.vb"

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

    '   Total Lines: 24
    '    Code Lines: 18 (75.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (25.00%)
    '     File Size: 1.06 KB


    ' Module Module2
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO

Module Module2

    Sub Main()
        Dim identify = EntityObject.LoadDataSet("Y:\干血片\pos\20190719\doMSMSalignment.report1.csv") _
           .Select(Function(d) (ID:=d.ID, name:=d!name)) _
           .ToDictionary(Function(n) n.ID, Function(n) n.name.StringReplace("_\d+", ""))
        Dim data = EntityObject.LoadDataSet("\\192.168.1.239\linux\project\干血片\design2_20190818\46_DEM\stroke.cor.csv").ToArray

        For Each compound In data
            Dim peaks = compound.ID.Trim("+"c).Trim("["c, "]"c).peakGroup(dt:=10).Value
            Dim names = peaks.Objects.Where(Function(id) identify.ContainsKey(id)).Select(Function(id) identify(id)).Distinct.JoinBy(" / ")

            compound("metabolites") = names
        Next

        Call data.SaveDataSet("\\192.168.1.239\linux\project\干血片\design2_20190818\46_DEM\stroke.cor.metabolites.csv")

        Pause()
    End Sub
End Module
