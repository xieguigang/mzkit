#Region "Microsoft.VisualBasic::76f8e8547fbf931ef1e10b45909dfe2a, G:/mzkit/src/assembly/assembly//MarkupData/mzML/UV/ExtractUVData.vb"

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
    '    Code Lines: 28
    ' Comment Lines: 8
    '   Blank Lines: 8
    '     File Size: 1.71 KB


    '     Module ExtractUVData
    ' 
    '         Function: GetPhotodiodeArrayDetectorInstrumentConfigurationId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace MarkupData.mzML

    ''' <summary>
    ''' helper module for read UV data
    ''' </summary>
    Public Module ExtractUVData

        Public Const UVScanType As String = "electromagnetic radiation spectrum"
        Public Const UVdetector As String = "photodiode array detector"

        ''' <summary>
        ''' get instrument configuration id of ``photodiode array detector``.
        ''' </summary>
        ''' <param name="rawdata"></param>
        ''' <returns></returns>
        Public Function GetPhotodiodeArrayDetectorInstrumentConfigurationId(rawdata As String) As String
            For Each configuration As instrumentConfiguration In rawdata.LoadXmlDataSet(Of instrumentConfiguration)(, xmlns:=indexedmzML.xmlns)
                If configuration Is Nothing OrElse configuration.componentList Is Nothing Then
                    Continue For
                End If
                If configuration.componentList.detector Is Nothing Then
                    Continue For
                End If

                If configuration.componentList.detector _
                    .Any(Function(dev)
                             If dev Is Nothing OrElse dev.cvParams Is Nothing Then
                                 Return False
                             End If

                             Return dev.cvParams.Any(Function(a) a?.name.TextEquals(UVdetector))
                         End Function) Then

                    Return configuration.id
                End If
            Next

            Return Nothing
        End Function
    End Module
End Namespace
