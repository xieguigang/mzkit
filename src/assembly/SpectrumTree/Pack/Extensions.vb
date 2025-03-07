#Region "Microsoft.VisualBasic::1830af1afa5b0ac84bb6dfd9448bec33, assembly\SpectrumTree\Pack\Extensions.vb"

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

    '   Total Lines: 36
    '    Code Lines: 19 (52.78%)
    ' Comment Lines: 10 (27.78%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 7 (19.44%)
    '     File Size: 1.33 KB


    '     Module Extensions
    ' 
    '         Function: (+2 Overloads) Open
    ' 
    ' 
    ' /********************************************************************************/

#End Region


Imports System.IO
Imports System.Runtime.CompilerServices

Namespace PackLib

    Public Module Extensions

        ''' <summary>
        ''' open reference database file writer
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="file"></param>
        ''' <param name="truncated">
        ''' clear all file content data after open the target file stream?
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Open(Of T As IReferencePack)(file As String, Optional truncated As Boolean = True) As IReferencePack
            Return Open(file, format:=GetType(T), truncated)
        End Function

        Public Function Open(file As String, ByRef format As Type, Optional truncated As Boolean = True) As IReferencePack
            Dim w As Stream = file.Open(FileMode.OpenOrCreate, doClear:=truncated, [readOnly]:=False)

            Select Case format
                Case GetType(MgfPack) : Return New MgfPack(w)
                Case GetType(SpectrumPack) : Return New SpectrumPack(w)
                Case Else
                    Throw New NotImplementedException(format.FullName)
            End Select
        End Function

    End Module
End Namespace
