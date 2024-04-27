#Region "Microsoft.VisualBasic::9196a0fed673b938f911358cfbd72529, G:/mzkit/src/assembly/BrukerDataReader//XMass/pdata/peaklist.vb"

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

    '   Total Lines: 44
    '    Code Lines: 33
    ' Comment Lines: 3
    '   Blank Lines: 8
    '     File Size: 1.34 KB


    '     Class pklist
    ' 
    '         Properties: [date], creator, pk, shots, spectrumid
    '                     version
    ' 
    '     Class pk
    ' 
    '         Properties: absi, area, chisq, goodn, goodn2
    '                     ind, lind, lmass, mass, massemg
    '                     massgaussian, meth, reso, rind, rmass
    '                     s2n, sigmaemg, tauemg, type
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports System.Xml.Serialization

Namespace XMass

    Public Class pklist
        <XmlAttribute> Public Property version As String
        <XmlAttribute> Public Property creator As String
        <XmlAttribute> Public Property shots As Integer
        <XmlAttribute> Public Property [date] As String
        <XmlAttribute> Public Property spectrumid As String

        <XmlElement>
        Public Property pk As pk()

    End Class

    ''' <summary>
    ''' A peak
    ''' </summary>
    Public Class pk

        Public Property absi As Double
        Public Property area As Double
        Public Property chisq As Double
        Public Property goodn As Double
        Public Property goodn2 As Double
        Public Property ind As Double
        Public Property lind As Double
        Public Property lmass As Double
        Public Property mass As Double
        Public Property massemg As Double
        Public Property massgaussian As Double
        Public Property meth As Double
        Public Property reso As Double
        Public Property rind As Double
        Public Property rmass As Double
        Public Property s2n As Double
        Public Property sigmaemg As Double
        Public Property tauemg As Double
        Public Property type As Double

    End Class
End Namespace
