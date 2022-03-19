#Region "Microsoft.VisualBasic::ed804410692e8e86cd05e83be552600c, mzkit\src\mzkit\Task\GCMSQuantifyIon.vb"

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
    '    Code Lines: 26
    ' Comment Lines: 6
    '   Blank Lines: 4
    '     File Size: 1.11 KB


    ' Module GCMSQuantifyIon
    ' 
    '     Function: GetIon
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS

Public Module GCMSQuantifyIon

    ''' <summary>
    ''' 这个函数不会返回空值
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="id"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetIon(ions As Dictionary(Of String, QuantifyIon), id As String) As QuantifyIon
        Dim ion As QuantifyIon = ions.TryGetValue(id)

        If ion Is Nothing Then
            If id.IsPattern("[0-9.]+[/][0-9.]+") Then
                ion = New QuantifyIon With {
                    .id = id,
                    .name = id,
                    .ms = {},
                    .rt = id.Split("/"c).Select(AddressOf Val).ToArray
                }
            Else
                ion = New QuantifyIon With {
                    .id = id,
                    .ms = {},
                    .name = id,
                    .rt = {-100, -100}
                }
            End If
        End If

        Return ion
    End Function
End Module
