#Region "Microsoft.VisualBasic::2aa85d3ef3b34560586d89c88097401a, assembly\assembly\ASCII\MSL\MSLIon.vb"

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

    '   Total Lines: 30
    '    Code Lines: 21 (70.00%)
    ' Comment Lines: 4 (13.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (16.67%)
    '     File Size: 1.22 KB


    '     Class MSLIon
    ' 
    '         Properties: [IS], CASNO, Comment, Contributor, Formula
    '                     MW, Name, Nist, Peaks, RI
    '                     RT, Source
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ASCII.MSL

    Public Class MSLIon

        <Column(Name:="NAME")> Public Property Name As String
        <Column(Name:="CONTRIB")> Public Property Contributor As String
        <Column(Name:="FORM")> Public Property Formula As String
        <Column(Name:="CASNO")> Public Property CASNO As String
        <Column(Name:="NIST")> Public Property Nist As String
        <Column(Name:="RI")> Public Property RI As String
        ''' <summary>
        ''' the <see cref="Name"/> of the internal standard metabolite.
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="IS")> Public Property [IS] As String
        <Column(Name:="COMMENT")> Public Property Comment As String
        <Column(Name:="SOURCE")> Public Property Source As String
        <Column(Name:="MW")> Public Property MW As Double
        <Column(Name:="RT")> Public Property RT As Double?

        Public Property Peaks As ms2()

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace
