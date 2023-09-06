
Public Interface ILipidomicsVisitorBuilder
        Sub SetChainsState(ByVal state As ChainsIndeterminateState)
        Sub SetAcylDoubleBond(ByVal state As DoubleBondIndeterminateState)
        Sub SetAcylOxidized(ByVal state As OxidizedIndeterminateState)
        Sub SetAlkylDoubleBond(ByVal state As DoubleBondIndeterminateState)
        Sub SetAlkylOxidized(ByVal state As OxidizedIndeterminateState)
        Sub SetSphingoDoubleBond(ByVal state As DoubleBondIndeterminateState)
        Sub SetSphingoOxidized(ByVal state As OxidizedIndeterminateState)
    End Interface

