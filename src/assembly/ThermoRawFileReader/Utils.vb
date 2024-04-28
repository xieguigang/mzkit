#Region "Microsoft.VisualBasic::2ed7edf35e8277c847d8c015c1f06d17, E:/mzkit/src/assembly/ThermoRawFileReader//Utils.vb"

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

    '   Total Lines: 26
    '    Code Lines: 21
    ' Comment Lines: 1
    '   Blank Lines: 4
    '     File Size: 1.17 KB


    ' Module Utils
    ' 
    '     Sub: StoreParallelStrings
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Module Utils

    <Extension>
    Friend Sub StoreParallelStrings(targetList As ICollection(Of KeyValuePair(Of String, String)),
                                    names As IList(Of String),
                                    values As IList(Of String),
                                    Optional skipEmptyNames As Boolean = False,
                                    Optional replaceTabsInValues As Boolean = False)
        Call targetList.Clear()

        For i As Integer = 0 To names.Count - 1
            If skipEmptyNames AndAlso (String.IsNullOrWhiteSpace(names(i)) OrElse Equals(names(i), CStr(ChrW(1)))) Then
                ' Name is empty or null
                Continue For
            End If

            If replaceTabsInValues AndAlso values(CInt(i)).Contains(vbTab) Then
                targetList.Add(New KeyValuePair(Of String, String)(names(i), values(CInt(i)).Replace(CStr(vbTab), CStr(" ")).TrimEnd(" "c)))
            Else
                targetList.Add(New KeyValuePair(Of String, String)(names(i), values(i).TrimEnd(" "c)))
            End If
        Next
    End Sub
End Module
