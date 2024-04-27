#Region "Microsoft.VisualBasic::50c2860b293bd0a58b12e899870d4176, G:/mzkit/src/mzmath/MSFinder//DatabaseQuery.vb"

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
    '    Code Lines: 22
    ' Comment Lines: 0
    '   Blank Lines: 1
    '     File Size: 767 B


    ' Class DatabaseQuery
    ' 
    '     Properties: Blexp, Bmdb, Chebi, Coconut, Drugbank
    '                 Ecmdb, Foodb, Hmdb, Knapsack, Nanpdb
    '                 Npa, Plantcyc, Pubchem, Smpdb, Stoff
    '                 T3db, Unpd, Ymdb
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Public Class DatabaseQuery
    Public Sub New()
    End Sub

    Public Property Chebi As Boolean
    Public Property Ymdb As Boolean
    Public Property Unpd As Boolean
    Public Property Smpdb As Boolean
    Public Property Pubchem As Boolean
    Public Property Hmdb As Boolean
    Public Property Plantcyc As Boolean
    Public Property Knapsack As Boolean
    Public Property Bmdb As Boolean
    Public Property Drugbank As Boolean
    Public Property Ecmdb As Boolean
    Public Property Foodb As Boolean
    Public Property T3db As Boolean
    Public Property Stoff As Boolean
    Public Property Nanpdb As Boolean
    Public Property Blexp As Boolean
    Public Property Npa As Boolean
    Public Property Coconut As Boolean
End Class

