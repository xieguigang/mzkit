#Region "Microsoft.VisualBasic::ee32ecf2839f26aaa335ab1614c466e7, assembly\assembly\ASCII\Raman\Spectroscopy.vb"

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

    '   Total Lines: 32
    '    Code Lines: 28 (87.50%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (12.50%)
    '     File Size: 1.56 KB


    '     Class Spectroscopy
    ' 
    '         Properties: [Date], Comments, DataType, DetailedInformation, FirstX
    '                     FirstY, LastX, Locale, MaxY, MeasurementInformation
    '                     MinY, nPoints, Origin, Owner, Reltax
    '                     Resolution, Spectrometer, Time, Title, Xunits
    '                     xyData, Yunits
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ASCII.Raman

    Public Class Spectroscopy

        <Field("TITLE")> Public Property Title As String
        <Field("DATA TYPE")> Public Property DataType As String
        <Field("ORIGIN")> Public Property Origin As String
        <Field("OWNER")> Public Property Owner As String
        <Field("DATE")> Public Property [Date] As String
        <Field("TIME")> Public Property Time As String
        <Field("SPECTROMETER/DATA SYSTEM")> Public Property Spectrometer As String
        <Field("LOCALE")> Public Property Locale As String
        <Field("RESOLUTION")> Public Property Resolution As String
        <Field("DELTAX")> Public Property Reltax As String
        <Field("XUNITS")> Public Property Xunits As String
        <Field("YUNITS")> Public Property Yunits As String
        <Field("FIRSTX")> Public Property FirstX As String
        <Field("LASTX")> Public Property LastX As String
        <Field("NPOINTS")> Public Property nPoints As Integer
        <Field("FIRSTY")> Public Property FirstY As String
        <Field("MAXY")> Public Property MaxY As String
        <Field("MINY")> Public Property MinY As String
        Public Property xyData As PointF()
        Public Property Comments As Dictionary(Of String, String)
        Public Property DetailedInformation As Dictionary(Of String, String)
        Public Property MeasurementInformation As Dictionary(Of String, String)

    End Class
End Namespace
