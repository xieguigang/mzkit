#Region "Microsoft.VisualBasic::f3cec54cd0b93c99f679b9f2d664cf33, G:/mzkit/src/mzmath/Oligonucleotide_MS//MS_Peak/GroupMass.vb"

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

    '   Total Lines: 35
    '    Code Lines: 25
    ' Comment Lines: 4
    '   Blank Lines: 6
    '     File Size: 1.12 KB


    ' Class GroupMass
    ' 
    '     Properties: [end], mass, name
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: AverageMass, MonoisotopicMass, ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class GroupMass

    ''' <summary>
    ''' 5' or 3'
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property [end] As Integer
    Public ReadOnly Property name As String
    Public ReadOnly Property mass As Double

    Sub New(ends As Integer, name As String, mass As Double)
        Me.[end] = ends
        Me.name = name
        Me.mass = mass
    End Sub

    Public Overrides Function ToString() As String
        Return $"{[end]}'  {name} = {mass}"
    End Function

    Public Shared Iterator Function MonoisotopicMass() As IEnumerable(Of GroupMass)
        Yield New GroupMass(5, "p-", 79.966331)
        Yield New GroupMass(5, "HO-", 17.00274)
        Yield New GroupMass(5, "Cap-", 484.0635)
        Yield New GroupMass(3, "-H", 1.007825)
    End Function

    Public Shared Iterator Function AverageMass() As IEnumerable(Of GroupMass)
        Yield New GroupMass(5, "p-", 79.979902)
        Yield New GroupMass(5, "HO-", 17.00734)
        Yield New GroupMass(5, "Cap-", 484.2764)
        Yield New GroupMass(3, "-H", 1.00794)
    End Function

End Class
