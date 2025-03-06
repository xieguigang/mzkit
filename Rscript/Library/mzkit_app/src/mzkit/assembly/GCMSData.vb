#Region "Microsoft.VisualBasic::a3ecc5ec71f80d9965286f6163d2d767, Rscript\Library\mzkit_app\src\mzkit\assembly\GCMSData.vb"

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

    '   Total Lines: 36
    '    Code Lines: 23 (63.89%)
    ' Comment Lines: 9 (25.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (11.11%)
    '     File Size: 1.23 KB


    ' Module GCMSData
    ' 
    '     Function: CreateGCMSFeatures
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' GC-MS rawdata handler package module
''' </summary>
<Package("GCMS")>
Module GCMSData

    ''' <summary>
    ''' Create gc-ms features data object
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="peaks"></param>
    ''' <returns></returns>
    <ExportAPI("gcms_features")>
    Public Function CreateGCMSFeatures(raw As mzPack, peaks As PeakSet) As GCMSPeak()
        Dim scans As PeakMs2() = raw.MS _
            .Select(Function(ms1)
                        Return New PeakMs2(ms1.scan_id, ms1.GetMs) With {.rt = ms1.rt}
                    End Function) _
            .ToArray
        Dim features As GCMSPeak() = peaks.peaks _
            .AsParallel _
            .Select(Function(ROI)
                        Return GCMSPeak.CreateFeature(scans, peak:=ROI)
                    End Function) _
            .ToArray

        Return features
    End Function

End Module

