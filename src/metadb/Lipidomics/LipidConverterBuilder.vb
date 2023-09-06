
Friend NotInheritable Class LipidConverterBuilder
        Implements ILipidomicsVisitorBuilder
        Private _acylDoubleBondState, _alkylDoubleBondState, _sphingosineDoubleBondState As DoubleBondIndeterminateState
        Private _acylOxidizedState, _alkylOxidizedState, _sphingosineOxidizedState As OxidizedIndeterminateState
        Private _chainsIndeterminate As ChainsIndeterminateState

        Public Function Create() As LipidAnnotationLevelConverter
            Dim acylVisitor = New AcylChainVisitor(_acylDoubleBondState.AsVisitor(), _acylOxidizedState.AsVisitor())
            Dim alkylVisitor = New AlkylChainVisitor(_alkylDoubleBondState.AsVisitor(), _alkylOxidizedState.AsVisitor())
            Dim sphingosineVisitor = New SphingosineChainVisitor(_sphingosineDoubleBondState.AsVisitor(), _sphingosineOxidizedState.AsVisitor())
            Dim chainVisitor = New ChainVisitor(acylVisitor, alkylVisitor, sphingosineVisitor)
            Dim chainsVisitor = New ChainsVisitor(chainVisitor, _chainsIndeterminate)
            Return New LipidAnnotationLevelConverter(chainsVisitor)
        End Function

        Private Sub SetChainsState(ByVal state As ChainsIndeterminateState) Implements ILipidomicsVisitorBuilder.SetChainsState
            _chainsIndeterminate = state
        End Sub

        Private Sub SetAcylDoubleBond(ByVal state As DoubleBondIndeterminateState) Implements ILipidomicsVisitorBuilder.SetAcylDoubleBond
            _acylDoubleBondState = state
        End Sub

        Private Sub SetAcylOxidized(ByVal state As OxidizedIndeterminateState) Implements ILipidomicsVisitorBuilder.SetAcylOxidized
            _acylOxidizedState = state
        End Sub

        Private Sub SetAlkylDoubleBond(ByVal state As DoubleBondIndeterminateState) Implements ILipidomicsVisitorBuilder.SetAlkylDoubleBond
            _alkylDoubleBondState = state
        End Sub

        Private Sub SetAlkylOxidized(ByVal state As OxidizedIndeterminateState) Implements ILipidomicsVisitorBuilder.SetAlkylOxidized
            _alkylOxidizedState = state
        End Sub

        Private Sub SetSphingoDoubleBond(ByVal state As DoubleBondIndeterminateState) Implements ILipidomicsVisitorBuilder.SetSphingoDoubleBond
            _sphingosineDoubleBondState = state
        End Sub

        Private Sub SetSphingoOxidized(ByVal state As OxidizedIndeterminateState) Implements ILipidomicsVisitorBuilder.SetSphingoOxidized
            _sphingosineOxidizedState = state
        End Sub
    End Class

