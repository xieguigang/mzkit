#Region "Microsoft.VisualBasic::702e96d92060071087a73e771098a309, mzmath\ms2_math-core\Spectra\Models\Xml\SSM2MatrixFragment.vb"

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

    '   Total Lines: 75
    '    Code Lines: 45 (60.00%)
    ' Comment Lines: 19 (25.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (14.67%)
    '     File Size: 2.34 KB


    '     Class SSM2MatrixFragment
    ' 
    '         Properties: da, mz, query, ref
    ' 
    '         Function: createFragment, FromXml, GetSampleFragment, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml
Imports System.Xml.Serialization

Namespace Spectra.Xml

    ''' <summary>
    ''' tuple data of [mz, query_intensity, reference_intensity], a MatchedPeak model
    ''' </summary>
    Public Class SSM2MatrixFragment

        ''' <summary>
        ''' The m/z value of the query fragment
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mz As Double

#Region "Fragment intensity"
        ''' <summary>
        ''' intensity in the query spectrum data
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property query As Double
        ''' <summary>
        ''' intensity in the reference spectrum data
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ref As Double
#End Region

        <XmlAttribute> Public Property IsProductIonMatched As Boolean = False
        <XmlAttribute> Public Property IsNeutralLossMatched As Boolean = False

        ''' <summary>
        ''' Mass delta between the query and reference fragment in unit ``da``
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property da As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetSampleFragment() As ms2
            Return New ms2(mz, query)
        End Function

        Public Shared Function FromXml(node As XmlNode, nodeName$) As SSM2MatrixFragment()
            Return (From child As XmlNode
                    In node.ChildNodes
                    Where child.Name = nodeName) _
 _
                .Select(AddressOf createFragment) _
                .ToArray
        End Function

        Private Shared Function createFragment(feature As XmlNode) As SSM2MatrixFragment
            Dim data = feature.Attributes
            Dim mz, query, ref As Double
            Dim da As String

            With data
                mz = !mz.Value
                query = !query.Value.ParseDouble
                ref = !ref.Value.ParseDouble
                da = !da.Value
            End With

            Return New SSM2MatrixFragment With {
                .mz = mz,
                .query = query,
                .ref = ref,
                .da = da
            }
        End Function

        Public Overrides Function ToString() As String
            Return mz
        End Function
    End Class
End Namespace
