#Region "Microsoft.VisualBasic::f6d387a85bac04f1ed4136dd14c10589, mzmath\MSEngine\AnnotationPack\Report\IReportRender.vb"

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

    '   Total Lines: 29
    '    Code Lines: 9 (31.03%)
    ' Comment Lines: 14 (48.28%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (20.69%)
    '     File Size: 1011 B


    ' Interface IReportRender
    ' 
    '     Properties: annotation, colorSet, samplefiles
    ' 
    '     Function: GetIon, GetPeak, Tabular
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math

Public Interface IReportRender

    ReadOnly Property annotation As AnnotationPack

    ''' <summary>
    ''' the color schema for the heatmap rendering
    ''' </summary>
    ''' <returns></returns>
    Property colorSet As String()
    ''' <summary>
    ''' ordinal of the sample files or the sample file display selection list
    ''' </summary>
    ''' <returns></returns>
    Property samplefiles As String()

    Function GetIon(xcms_id As String) As AlignmentHit
    Function GetPeak(xcms_id As String) As xcms2

    ''' <summary>
    ''' implements of the html table report
    ''' </summary>
    ''' <param name="refSet"></param>
    ''' <param name="rt_cell"></param>
    ''' <returns>this function generates the html table code for view report</returns>
    Function Tabular(refSet As IEnumerable(Of String), println As Action(Of String), Optional rt_cell As Boolean = False) As IEnumerable(Of String)

End Interface

