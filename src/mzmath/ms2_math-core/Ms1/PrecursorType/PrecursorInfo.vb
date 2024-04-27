#Region "Microsoft.VisualBasic::2b9dfbc574ba4f2821f07d4e95ab0622, G:/mzkit/src/mzmath/ms2_math-core//Ms1/PrecursorType/PrecursorInfo.vb"

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

    '   Total Lines: 52
    '    Code Lines: 26
    ' Comment Lines: 19
    '   Blank Lines: 7
    '     File Size: 1.61 KB


    '     Class PrecursorInfo
    ' 
    '         Properties: adduct, charge, ionMode, M, mz
    '                     precursor_type
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' a ion m/z data model, includes the adducts data and <see cref="IonModes"/> polarity data.
    ''' </summary>
    Public Class PrecursorInfo

        ''' <summary>
        ''' the precursor adducts information
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property precursor_type As String
        Public Property charge As Double
        Public Property M As Double
        ''' <summary>
        ''' the exact mass value of the <see cref="precursor_type"/> adducts information.
        ''' </summary>
        ''' <returns></returns>
        Public Property adduct As Double

        ''' <summary>
        ''' mz or exact mass
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="m/z")>
        Public Property mz As String
        ''' <summary>
        ''' the ion polarity data, related to the <see cref="charge"/> value
        ''' </summary>
        ''' <returns></returns>
        Public Property ionMode As IonModes

        Sub New()
        End Sub

        Sub New(mz As MzCalculator)
            precursor_type = mz.ToString
            charge = mz.charge
            M = mz.M
            adduct = mz.adducts
            ionMode = ParseIonMode(mz.mode)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{precursor_type} m/z={mz}"
        End Function
    End Class
End Namespace
