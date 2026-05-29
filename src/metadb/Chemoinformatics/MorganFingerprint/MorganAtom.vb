#Region "Microsoft.VisualBasic::458ebb6a82dd690c9d449ddcee101308, metadb\Chemoinformatics\MorganFingerprint\MorganAtom.vb"

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

    '   Total Lines: 29
    '    Code Lines: 18 (62.07%)
    ' Comment Lines: 4 (13.79%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (24.14%)
    '     File Size: 920 B


    '     Class MorganAtom
    ' 
    '         Properties: Atom, Code, Index
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.MorganFingerprint

Namespace MorganFingerprint

    Public Class MorganAtom : Inherits Atom
        Implements IMorganAtom

        Public Property Index As Integer Implements IMorganAtom.Index
        Public Property Code As ULong Implements IMorganAtom.Code

        ''' <summary>
        ''' the element atom name
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property Atom As String Implements IMorganAtom.Type

        Sub New(base As Atom)
            Atom = base.Atom
            Coordination = base.Coordination

            If TypeOf base Is MorganAtom Then
                Index = DirectCast(base, MorganAtom).Index
                Code = DirectCast(base, MorganAtom).Code
            End If
        End Sub

    End Class
End Namespace
