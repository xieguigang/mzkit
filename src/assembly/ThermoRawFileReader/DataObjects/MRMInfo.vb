#Region "Microsoft.VisualBasic::a316cd7f42de24b0ca8a9da52cfff5ee, G:/mzkit/src/assembly/ThermoRawFileReader//DataObjects/MRMInfo.vb"

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
    '    Code Lines: 13
    ' Comment Lines: 11
    '   Blank Lines: 6
    '     File Size: 861 B


    '     Class MRMInfo
    ' 
    '         Sub: DuplicateMRMInfo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices

Namespace DataObjects

    ''' <summary>
    ''' Type for MRM Information
    ''' </summary>
    <CLSCompliant(True)>
    Public Class MRMInfo

        '''         <summary>
        '''      List of mass ranges monitored by the first quadrupole
        '''        </summary>
        Public ReadOnly MRMMassList As New List(Of MRMMassRangeType)


        ''' <summary>
        ''' Duplicate the MRM info
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="target"></param>
        Public Shared Sub DuplicateMRMInfo(source As MRMInfo, <Out> ByRef target As MRMInfo)
            target = New MRMInfo()

            For Each item In source.MRMMassList
                target.MRMMassList.Add(item)
            Next
        End Sub
    End Class
End Namespace
