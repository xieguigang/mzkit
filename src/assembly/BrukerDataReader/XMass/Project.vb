#Region "Microsoft.VisualBasic::3566540453b0170b8e8fe718188dcf17, G:/mzkit/src/assembly/BrukerDataReader//XMass/Project.vb"

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

    '   Total Lines: 40
    '    Code Lines: 31
    ' Comment Lines: 3
    '   Blank Lines: 6
    '     File Size: 1.60 KB


    '     Class Project
    ' 
    '         Properties: acqu, acqus, AnalysisParameter, pdata, source
    '                     sptype
    ' 
    '         Function: FromResultFolder
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace XMass

    ''' <summary>
    ''' XMASS, Bruker-Daltonics project reader
    ''' </summary>
    Public Class Project

        Public Property sptype As String
        Public Property pdata As pdata()
        Public Property acqu As NamedValue(Of String())()
        Public Property acqus As NamedValue(Of String())()
        Public Property AnalysisParameter As AnnotationParameter
        Public Property source As String

        Public Shared Function FromResultFolder(dir As String) As Project
            Dim acqu = PropertyFileReader.ReadData($"{dir}/acqu".OpenReader).ToArray
            Dim acqus = PropertyFileReader.ReadData($"{dir}/acqus".OpenReader).ToArray
            Dim method As AnalysisMethod = $"{dir}/Analysis.FAmethod".LoadXml(Of AnalysisMethod)
            Dim parms As AnnotationParameter = ($"{dir}/AnalysisParameter.xml") _
                .ReadAllText _
                .CreateObjectFromXmlFragment(Of AnnotationParameter)
            Dim pdataList As New List(Of pdata)

            For Each pdatadir As String In $"{dir}/pdata".ListDirectory
                Call pdataList.Add(XMass.pdata.LoadFolder(pdatadir))
            Next

            Return New Project With {
                .sptype = $"{dir}/sptype".ReadAllText,
                .acqu = acqu,
                .acqus = acqus,
                .AnalysisParameter = parms,
                .pdata = pdataList.ToArray,
                .source = dir
            }
        End Function
    End Class
End Namespace
