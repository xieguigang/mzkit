Imports CompMs.Common.DataStructure

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
            Dim sc As SeparatedChains = Nothing

            If chains.Description.HasFlag(LipidDescription.SnPosition) AndAlso CSharpImpl.__Assign(sc, TryCast(chains, SeparatedChains)) IsNot Nothing Then
                Return New MolecularSpeciesLevelChains(sc.GetDeterminedChains())
            End If
            Return chains
        End Function

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Class
