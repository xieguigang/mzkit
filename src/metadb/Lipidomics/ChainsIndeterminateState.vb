#Region "Microsoft.VisualBasic::c0418aff9e111d4ab888d3ffd84ca816, metadb\Lipidomics\ChainsIndeterminateState.vb"

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

    '   Total Lines: 81
    '    Code Lines: 65
    ' Comment Lines: 0
    '   Blank Lines: 16
    '     File Size: 3.31 KB


    ' Interface IAcyclicVisitor
    ' 
    ' 
    ' 
    ' Interface IAcyclicDecomposer
    ' 
    ' 
    ' 
    ' Interface IVisitor
    ' 
    '     Function: Visit
    ' 
    ' Interface IDecomposer
    ' 
    '     Function: Decompose
    ' 
    ' Interface IVisitableElement
    ' 
    '     Function: Accept
    ' 
    ' Class ChainsIndeterminateState
    ' 
    '     Properties: MolecularSpeciesLevel, PositionLevel, SpeciesLevel
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Indeterminate, Visit
    '     Enum State
    ' 
    '         MolecularSpeciesLevel, None, PositionLevel, SpeciesLevel
    ' 
    ' 
    ' 
    '     Interface IIndeterminate
    ' 
    '         Function: Indeterminate
    ' 
    '     Class SpeciesLevelIndeterminate
    ' 
    '         Function: Indeterminate
    ' 
    '     Class MolecularSpeciesLevelIndeterminate
    ' 
    '         Function: Indeterminate
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Interface IAcyclicVisitor

End Interface

Public Interface IAcyclicDecomposer(Of Out TResult)

End Interface

Public Interface IVisitor(Of Out TResult, In TElement)
    Inherits IAcyclicVisitor
    Function Visit(item As TElement) As TResult
End Interface

Public Interface IDecomposer(Of Out TResult, In TElement)
    Inherits IAcyclicDecomposer(Of TResult)
    Function Decompose(Of T As TElement)(visitor As IAcyclicVisitor, element As T) As TResult
End Interface

Public Interface IVisitableElement
    Function Accept(Of TResult)(visitor As IAcyclicVisitor, decomposer As IAcyclicDecomposer(Of TResult)) As TResult
End Interface

Public NotInheritable Class ChainsIndeterminateState
    Implements IVisitor(Of ITotalChain, ITotalChain)
    Public Shared ReadOnly Property SpeciesLevel As ChainsIndeterminateState = New ChainsIndeterminateState(State.SpeciesLevel)
    Public Shared ReadOnly Property MolecularSpeciesLevel As ChainsIndeterminateState = New ChainsIndeterminateState(State.MolecularSpeciesLevel)
    Public Shared ReadOnly Property PositionLevel As ChainsIndeterminateState = New ChainsIndeterminateState(State.PositionLevel)

    Private ReadOnly _state As State
    Private ReadOnly _impl As IIndeterminate

    Private Sub New(state As State)
        Select Case state
            Case State.SpeciesLevel
                _impl = New SpeciesLevelIndeterminate()
            Case State.MolecularSpeciesLevel
                _impl = New MolecularSpeciesLevelIndeterminate()
        End Select
    End Sub

    Public Function Indeterminate(chains As ITotalChain) As ITotalChain
        Return If(_impl?.Indeterminate(chains), chains)
    End Function

    Private Function Visit(item As ITotalChain) As ITotalChain Implements IVisitor(Of ITotalChain, ITotalChain).Visit
        Return Indeterminate(item)
    End Function

    Friend Enum State
        None
        SpeciesLevel
        MolecularSpeciesLevel
        PositionLevel
    End Enum

    Friend Interface IIndeterminate
        Function Indeterminate(chains As ITotalChain) As ITotalChain
    End Interface

    Friend NotInheritable Class SpeciesLevelIndeterminate
        Implements IIndeterminate
        Public Function Indeterminate(chains As ITotalChain) As ITotalChain Implements IIndeterminate.Indeterminate
            If (chains.Description And (LipidDescription.Chain Or LipidDescription.SnPosition)) > 0 Then
                Return New TotalChain(chains.CarbonCount, chains.DoubleBondCount, chains.OxidizedCount, chains.AcylChainCount, chains.AlkylChainCount, chains.SphingoChainCount)
            End If
            Return chains
        End Function
    End Class

    Friend NotInheritable Class MolecularSpeciesLevelIndeterminate
        Implements IIndeterminate
        Public Function Indeterminate(chains As ITotalChain) As ITotalChain Implements IIndeterminate.Indeterminate
            Dim sc As SeparatedChains = TryCast(chains, SeparatedChains)

            If chains.Description.HasFlag(LipidDescription.SnPosition) AndAlso sc IsNot Nothing Then
                Return New MolecularSpeciesLevelChains(sc.GetDeterminedChains())
            End If
            Return chains
        End Function
    End Class
End Class
