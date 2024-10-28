#Region "Microsoft.VisualBasic::5d910ce7edf4da47c8d6801d3cb5a60c, metadb\Chemoinformatics\InChI\Models\InchiOutput.vb"

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

    '   Total Lines: 43
    '    Code Lines: 20 (46.51%)
    ' Comment Lines: 15 (34.88%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 8 (18.60%)
    '     File Size: 1.62 KB


    '     Class InchiOutput
    ' 
    '         Properties: AuxInfo, Inchi, Log, Message, Status
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region



Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

''' JNA-InChI - Library for calling InChI from Java
''' Copyright © 2018 Daniel Lowe
''' 
''' This library is free software; you can redistribute it and/or
''' modify it under the terms of the GNU Lesser General Public
''' License as published by the Free Software Foundation; either
''' version 2.1 of the License, or (at your option) any later version.
''' 
''' This program is distributed in the hope that it will be useful,
''' but WITHOUT ANY WARRANTY; without even the implied warranty of
''' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
''' GNU Lesser General Public License for more details.
''' 
''' You should have received a copy of the GNU Lesser General Public License
''' along with this program.  If not, see </>.

Namespace IUPAC.InChI
    Public Class InchiOutput

        Public Overridable ReadOnly Property Inchi As String
        Public Overridable ReadOnly Property AuxInfo As String
        Public Overridable ReadOnly Property Message As String
        Public Overridable ReadOnly Property Log As String
        Public Overridable ReadOnly Property Status As InchiStatus

        Sub New(inchi As String, auxInfo As String, message As String, log As String, status As InchiStatus)
            _Inchi = inchi
            _AuxInfo = auxInfo
            _Message = message
            _Log = log
            _Status = status
        End Sub

        Public Overrides Function ToString() As String
            Return Inchi
        End Function
    End Class

End Namespace

