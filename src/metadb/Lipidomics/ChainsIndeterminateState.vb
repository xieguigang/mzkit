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
