﻿#Region "Microsoft.VisualBasic::59ed6756cb09da450204681ce41930b3, metadb\Lipidomics\ChainVisitorBuilder.vb"

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

    '   Total Lines: 37
    '    Code Lines: 31 (83.78%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (16.22%)
    '     File Size: 2.05 KB


    ' Class ChainVisitorBuilder
    ' 
    '     Function: Create
    ' 
    '     Sub: SetAcylDoubleBond, SetAcylOxidized, SetAlkylDoubleBond, SetAlkylOxidized, SetChainsState
    '          SetSphingoDoubleBond, SetSphingoOxidized
    ' 
    ' /********************************************************************************/

#End Region

Friend NotInheritable Class ChainVisitorBuilder
    Implements ILipidomicsVisitorBuilder
    Private _acylDoubleBondState, _alkylDoubleBondState, _sphingoDoubleBondState As DoubleBondIndeterminateState
    Private _acylOxidizedState, _alkylOxidizedState, _sphingoOxidizedState As OxidizedIndeterminateState

    Public Function Create() As ChainVisitor
        Dim acylVisitor = New AcylChainVisitor(_acylDoubleBondState.AsVisitor(), _acylOxidizedState.AsVisitor())
        Dim alkylVisitor = New AlkylChainVisitor(_alkylDoubleBondState.AsVisitor(), _alkylOxidizedState.AsVisitor())
        Dim sphingoVisitor = New SphingosineChainVisitor(_sphingoDoubleBondState.AsVisitor(), _sphingoOxidizedState.AsVisitor())
        Return New ChainVisitor(acylVisitor, alkylVisitor, sphingoVisitor)
    End Function

    Private Sub SetChainsState(state As ChainsIndeterminateState) Implements ILipidomicsVisitorBuilder.SetChainsState
    End Sub

    Private Sub SetAcylDoubleBond(state As DoubleBondIndeterminateState) Implements ILipidomicsVisitorBuilder.SetAcylDoubleBond
        _acylDoubleBondState = state
    End Sub
    Private Sub SetAcylOxidized(state As OxidizedIndeterminateState) Implements ILipidomicsVisitorBuilder.SetAcylOxidized
        _acylOxidizedState = state
    End Sub

    Private Sub SetAlkylDoubleBond(state As DoubleBondIndeterminateState) Implements ILipidomicsVisitorBuilder.SetAlkylDoubleBond
        _alkylDoubleBondState = state
    End Sub
    Private Sub SetAlkylOxidized(state As OxidizedIndeterminateState) Implements ILipidomicsVisitorBuilder.SetAlkylOxidized
        _alkylOxidizedState = state
    End Sub

    Private Sub SetSphingoDoubleBond(state As DoubleBondIndeterminateState) Implements ILipidomicsVisitorBuilder.SetSphingoDoubleBond
        _sphingoDoubleBondState = state
    End Sub
    Private Sub SetSphingoOxidized(state As OxidizedIndeterminateState) Implements ILipidomicsVisitorBuilder.SetSphingoOxidized
        _sphingoOxidizedState = state
    End Sub

End Class
