#Region "Microsoft.VisualBasic::167447c006387b0ccfbc1faff222ba9f, assembly\assembly\MarkupData\mzML\XML\Configurations.vb"

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
    '    Code Lines: 26
    ' Comment Lines: 0
    '   Blank Lines: 16
    '     File Size: 1.04 KB


    '     Class instrumentConfigurationList
    ' 
    '         Properties: instrumentConfiguration
    ' 
    '     Class instrumentConfiguration
    ' 
    '         Properties: componentList, id
    ' 
    '         Function: ToString
    ' 
    '     Class componentList
    ' 
    '         Properties: analyzer, detector, source
    ' 
    '     Class component
    ' 
    '         Properties: order
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.ControlVocabulary

Namespace MarkupData.mzML

    Public Class instrumentConfigurationList : Inherits List

        <XmlElement>
        Public Property instrumentConfiguration As instrumentConfiguration()

    End Class

    Public Class instrumentConfiguration : Inherits Params

        <XmlAttribute>
        Public Property id As String

        Public Property componentList As componentList

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class

    Public Class componentList : Inherits List

        Public Property source As component

        <XmlElement>
        Public Property analyzer As component()
        <XmlElement>
        Public Property detector As component()

    End Class

    Public Class component : Inherits Params

        <XmlAttribute> Public Property order As Integer

    End Class
End Namespace
