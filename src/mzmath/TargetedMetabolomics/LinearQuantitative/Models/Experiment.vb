#Region "Microsoft.VisualBasic::b23868e415761a1ea547abb7819a1602, mzmath\TargetedMetabolomics\LinearQuantitative\Models\Experiment.vb"

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

    '   Total Lines: 27
    '    Code Lines: 16 (59.26%)
    ' Comment Lines: 4 (14.81%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 7 (25.93%)
    '     File Size: 680 B


    '     Class Experiment
    ' 
    '         Properties: DataFiles, ProjectId
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq

Namespace LinearQuantitative

    ''' <summary>
    ''' a experiment project
    ''' </summary>
    ''' 
    <XmlType("Experiment", [Namespace]:="https://mzkit.org")>
    Public Class Experiment : Inherits XmlDataModel

        <XmlElement("DataFile")> Public Property DataFiles As DataFile()

        <XmlAttribute>
        Public Property ProjectId As String

        Sub New()
        End Sub

        Sub New(files As IEnumerable(Of DataFile))
            DataFiles = files.SafeQuery.ToArray
        End Sub

    End Class
End Namespace
