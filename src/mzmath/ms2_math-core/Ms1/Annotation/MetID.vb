#Region "Microsoft.VisualBasic::bf907aa587e492b5d6bb10668eb39c4f, mzmath\ms2_math-core\Ms1\Annotation\MetID.vb"

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

    '   Total Lines: 42
    '    Code Lines: 17 (40.48%)
    ' Comment Lines: 19 (45.24%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (14.29%)
    '     File Size: 1.65 KB


    '     Class MetID
    ' 
    '         Properties: intensity, mz, name, precursor_type, rt
    '                     unique_id
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Ms1.Annotations

    ''' <summary>
    ''' the ion annotation model
    ''' </summary>
    Public Class MetID : Implements IReadOnlyId, IMS1Annotation, ICompoundNameProvider

        ''' <summary>
        ''' the unique id of the target query result metabolite
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property unique_id As String Implements IReadOnlyId.Identity, IKeyedEntity(Of String).Key
        ''' <summary>
        ''' the ion adducts of this ion precursor
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property precursor_type As String Implements IMS1Annotation.precursor_type
        <XmlAttribute> Public Property intensity As Double Implements IMs1Scan.intensity

        ''' <summary>
        ''' the source ``m/z`` value of current annotated ion feature.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mz As Double Implements IMs1.mz
        <XmlAttribute> Public Property rt As Double Implements IRetentionTime.rt
        ''' <summary>
        ''' the metabolite name of current ion that annotated.
        ''' </summary>
        ''' <returns></returns>
        <XmlText> Public Property name As String Implements ICompoundNameProvider.CommonName

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class
End Namespace
