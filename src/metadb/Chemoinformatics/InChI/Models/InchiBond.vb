#Region "Microsoft.VisualBasic::a6b0fd759dd3fa97c09e6f6ce0034b88, metadb\Chemoinformatics\InChI\Models\InchiBond.vb"

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

    '   Total Lines: 63
    '    Code Lines: 37 (58.73%)
    ' Comment Lines: 15 (23.81%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (17.46%)
    '     File Size: 2.37 KB


    '     Class InchiBond
    ' 
    '         Properties: [End], Start, Stereo, Type
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: getOther, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' JNA-InChI - Library for calling InChI from Java
' Copyright © 2018 Daniel Lowe
' 
' This library is free software; you can redistribute it and/or
' modify it under the terms of the GNU Lesser General Public
' License as published by the Free Software Foundation; either
' version 2.1 of the License, or (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU Lesser General Public License for more details.
' 
' You should have received a copy of the GNU Lesser General Public License
' along with this program.  If not, see </>.

Namespace IUPAC.InChI

    Public Class InchiBond

        Public Overridable ReadOnly Property Start As InchiAtom
        Public Overridable ReadOnly Property [End] As InchiAtom
        Public Overridable ReadOnly Property Type As InchiBondType
        Public Overridable ReadOnly Property Stereo As InchiBondStereo

        Public Sub New(start As InchiAtom, [end] As InchiAtom, type As InchiBondType)
            Me.New(start, [end], type, InchiBondStereo.NONE)
        End Sub

        Public Sub New(start As InchiAtom, [end] As InchiAtom, type As InchiBondType, stereo As InchiBondStereo)
            If start = [end] Then
                Throw New ArgumentException("start and end must be different atoms")
            End If
            If type Is Nothing Then
                Throw New ArgumentException("type must not be null")
            End If
            If stereo Is Nothing Then
                Throw New ArgumentException("stereo must not be null, use InchiBondStereo.NONE")
            End If

            _Start = start
            _End = [end]
            _Type = type
            _Stereo = stereo
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Start.ToString} {Type.ToString} {[End].ToString}"
        End Function

        Public Overridable Function getOther(atom As InchiAtom) As InchiAtom
            If Start Is atom Then
                Return [End]
            ElseIf [End] Is atom Then
                Return Start
            End If

            Return Nothing
        End Function

    End Class

End Namespace
