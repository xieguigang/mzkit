#Region "Microsoft.VisualBasic::fc3879dfe369a0115e78b6ec0b8b5480, mzkit\src\metadb\AnnotationLibrary\Data\MassIndex.vb"

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

    '   Total Lines: 23
    '    Code Lines: 8
    ' Comment Lines: 11
    '   Blank Lines: 4
    '     File Size: 637 B


    ' Class MassIndex
    ' 
    '     Properties: mz, referenceIds
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

''' <summary>
''' the database index
''' </summary>
Public Class MassIndex

    ''' <summary>
    ''' mz value for the metabolites
    ''' </summary>
    ''' <returns>round to 4 digits</returns>
    <MessagePackMember(0)> Public Property mz As Double
    ''' <summary>
    ''' a reference id list to read metabolite data
    ''' </summary>
    ''' <returns></returns>
    <MessagePackMember(1)> Public Property referenceIds As String()

    Public Overrides Function ToString() As String
        Return mz.ToString("F4")
    End Function

End Class

