#Region "Microsoft.VisualBasic::a7d73a99e921345412c4d75a0d2ffa37, G:/mzkit/src/metadb/Lipidomics//Annotation/LipidAnnotationLevelConverter.vb"

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

    '   Total Lines: 41
    '    Code Lines: 34
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.76 KB


    ' Class IdentityDecomposer
    ' 
    '     Properties: Instance
    ' 
    '     Function: Decompose
    ' 
    ' Class LipidAnnotationLevelConverter
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Visit
    ' 
    ' /********************************************************************************/

#End Region

Public NotInheritable Class IdentityDecomposer(Of TResult, TElement)
    Implements IDecomposer(Of TResult, TElement)
    Private Shared _instance As IdentityDecomposer(Of TResult, TElement)

    Public Shared ReadOnly Property Instance As IdentityDecomposer(Of TResult, TElement)
        Get
            Return If(_instance, Function()
                                     _instance = New IdentityDecomposer(Of TResult, TElement)()
                                     Return _instance
                                 End Function())
        End Get
    End Property

    Private Function Decompose(Of T As TElement)(visitor As IAcyclicVisitor, element As T) As TResult Implements IDecomposer(Of TResult, TElement).Decompose
        Dim vis As IVisitor(Of TResult, T) = TryCast(visitor, IVisitor(Of TResult, T))

        If vis IsNot Nothing Then
            Return vis.Visit(element)
        End If
        Return Nothing
    End Function
End Class

Friend NotInheritable Class LipidAnnotationLevelConverter
    Implements IVisitor(Of Lipid, ILipid)
    Private ReadOnly _chainsVisitor As IVisitor(Of ITotalChain, ITotalChain)

    Public Sub New(chainsVisitor As IVisitor(Of ITotalChain, ITotalChain))
        _chainsVisitor = chainsVisitor
    End Sub

    Private Function Visit(item As ILipid) As Lipid Implements IVisitor(Of Lipid, ILipid).Visit
        Dim converted = item.Chains.Accept(_chainsVisitor, IdentityDecomposer(Of ITotalChain, ITotalChain).Instance)
        Dim lipid As Lipid = TryCast(item, Lipid)

        If item.Chains Is converted AndAlso lipid IsNot Nothing Then
            Return lipid
        End If
        Return New Lipid(item.LipidClass, item.Mass, converted)
    End Function
End Class
