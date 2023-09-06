Namespace CompMs.Common.Lipidomics
    Public NotInheritable Class ShorthandNotationDirector
        Private ReadOnly _builder As ILipidomicsVisitorBuilder

        Public Sub New(ByVal builder As ILipidomicsVisitorBuilder)
            _builder = builder
            builder.SetAcylOxidized(OxidizedIndeterminateState.Identity)
            builder.SetAcylDoubleBond(DoubleBondIndeterminateState.Identity)
            builder.SetAlkylOxidized(OxidizedIndeterminateState.Identity)
            builder.SetAlkylDoubleBond(DoubleBondIndeterminateState.Identity)
            builder.SetSphingoOxidized(OxidizedIndeterminateState.Identity)
            builder.SetSphingoDoubleBond(DoubleBondIndeterminateState.Identity)
            builder.SetChainsState(ChainsIndeterminateState.PositionLevel)
        End Sub

        Public Sub Construct()
            ' for acyl
            _builder.SetAcylOxidized(OxidizedIndeterminateState.AllPositions)
            _builder.SetAcylDoubleBond(DoubleBondIndeterminateState.AllPositions)
            ' for alkyl
            _builder.SetAlkylOxidized(OxidizedIndeterminateState.AllPositions)
            _builder.SetAlkylDoubleBond(DoubleBondIndeterminateState.AllPositions.Exclude(1))
            ' for sphingosine
            _builder.SetSphingoOxidized(OxidizedIndeterminateState.AllPositions.Exclude(1))
            _builder.SetSphingoDoubleBond(DoubleBondIndeterminateState.AllPositions)
            ' for chains
            _builder.SetChainsState(ChainsIndeterminateState.SpeciesLevel)
        End Sub

        Public Sub SetSpeciesLevel()
            _builder.SetChainsState(ChainsIndeterminateState.SpeciesLevel)
        End Sub

        Public Sub SetMolecularSpeciesLevel()
            _builder.SetChainsState(ChainsIndeterminateState.MolecularSpeciesLevel)
        End Sub

        Public Sub SetPositionLevel()
            _builder.SetChainsState(ChainsIndeterminateState.PositionLevel)
        End Sub

        Public Sub SetDoubleBondPositionLevel()
            _builder.SetAcylDoubleBond(DoubleBondIndeterminateState.AllCisTransIsomers)
            _builder.SetAlkylDoubleBond(DoubleBondIndeterminateState.AllCisTransIsomers)
            _builder.SetSphingoDoubleBond(DoubleBondIndeterminateState.AllCisTransIsomers)
        End Sub

        Public Sub SetDoubleBondNumberLevel()
            _builder.SetAcylDoubleBond(DoubleBondIndeterminateState.AllPositions)
            _builder.SetAlkylDoubleBond(DoubleBondIndeterminateState.AllPositions.Exclude(1))
            _builder.SetSphingoDoubleBond(DoubleBondIndeterminateState.AllPositions)
        End Sub

        Public Sub SetOxidizedPositionLevel()
            _builder.SetAcylOxidized(OxidizedIndeterminateState.Identity)
            _builder.SetAlkylOxidized(OxidizedIndeterminateState.Identity)
            _builder.SetSphingoOxidized(OxidizedIndeterminateState.Identity)
        End Sub

        Public Sub SetOxidizedNumberLevel()
            _builder.SetAcylOxidized(OxidizedIndeterminateState.AllPositions)
            _builder.SetAlkylOxidized(OxidizedIndeterminateState.AllPositions)
            _builder.SetSphingoOxidized(OxidizedIndeterminateState.AllPositions.Exclude(1))
        End Sub
    End Class
End Namespace
