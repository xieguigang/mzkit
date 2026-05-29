#Region "Microsoft.VisualBasic::29ffd28c71100695ca5050440c2652fd, mzmath\ms2_math-core\mzPack\IMsAssemblyPack.vb"

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
    '    Code Lines: 6 (25.00%)
    ' Comment Lines: 15 (62.50%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (12.50%)
    '     File Size: 790 B


    ' Interface IMsAssemblyPack
    ' 
    '     Properties: source
    ' 
    '     Function: PickIonScatter
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' an abstract rawdata file model for make data unify representation in mzkit
''' </summary>
Public Interface IMsAssemblyPack

    ''' <summary>
    ''' the source data file reference name
    ''' </summary>
    ''' <returns></returns>
    Property source As String

    ''' <summary>
    ''' get ion scan scatter data from the rawdata pack file
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="rt"></param>
    ''' <param name="mass_da"></param>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    Function PickIonScatter(mz As Double, rt As Double,
                            Optional mass_da As Double = 0.25,
                            Optional dt As Double = 7.5) As IEnumerable(Of ms1_scan)

End Interface
