#Region "Microsoft.VisualBasic::25a50560534ef826e73515baf1d77b9d, mzkit\src\mzkit\Task\ParentMatch.vb"

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

    '   Total Lines: 37
    '    Code Lines: 33
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 1.25 KB


    ' Class ParentMatch
    ' 
    '     Properties: adducts, BPC, M, ppm, precursor_type
    '                 rawfile, TIC, XIC
    ' 
    '     Function: ToMs2
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

''' <summary>
''' a precursor matched ms2 spectrum data
''' </summary>
Public Class ParentMatch : Inherits ScanMS2

    Public Property precursor_type As String
    Public Property ppm As Double
    Public Property da As Double
    Public Property adducts As Double
    Public Property M As Integer
    Public Property BPC As Double
    Public Property TIC As Double
    Public Property XIC As Double
    Public Property rawfile As String

    Public Function ToMs2() As PeakMs2
        Return New PeakMs2 With {
            .activation = activationMethod.Description,
            .collisionEnergy = collisionEnergy,
            .file = rawfile,
            .intensity = BPC,
            .mz = parentMz,
            .rt = rt,
            .precursor_type = precursor_type,
            .scan = scan_id,
            .lib_guid = scan_id,
            .mzInto = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = into(i)
                            }
                        End Function) _
                .ToArray
        }
    End Function

End Class
