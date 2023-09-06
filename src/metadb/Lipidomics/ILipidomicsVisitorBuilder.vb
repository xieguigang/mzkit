
Public Interface ILipidomicsVisitorBuilder
        Sub SetChainsState(state As ChainsIndeterminateState)
        Sub SetAcylDoubleBond(state As DoubleBondIndeterminateState)
        Sub SetAcylOxidized(state As OxidizedIndeterminateState)
        Sub SetAlkylDoubleBond(state As DoubleBondIndeterminateState)
        Sub SetAlkylOxidized(state As OxidizedIndeterminateState)
        Sub SetSphingoDoubleBond(state As DoubleBondIndeterminateState)
        Sub SetSphingoOxidized(state As OxidizedIndeterminateState)
    End Interface

