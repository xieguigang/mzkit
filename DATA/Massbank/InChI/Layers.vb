Namespace IUPAC.InChILayers

    Public MustInherit Class Layer

    End Class

    ''' <summary>
    ''' 主层（main layer）：以``1``表示
    ''' </summary>
    Public Class MainLayer : Inherits Layer

    End Class

    ''' <summary>
    ''' 电荷层（charge layer）：以``q``表示
    ''' </summary>
    Public Class ChargeLayer : Inherits Layer

    End Class

    ''' <summary>
    ''' 立体化学层（Stereochemical layer）：以``t``，``m``，``s``表示
    ''' </summary>
    Public Class StereochemicalLayer : Inherits Layer

    End Class

    ''' <summary>
    ''' 异构体层（Isotopic layer）：以``i``表示
    ''' </summary>
    Public Class IsotopicLayer : Inherits Layer

    End Class

    ''' <summary>
    ''' 固定氢原子（Fixed-H layer）：以``f``表示
    ''' </summary>
    Public Class FixedHLayer : Inherits Layer

    End Class

    ''' <summary>
    ''' 再连接层（Reconnected Layer）：以``r``表示
    ''' </summary>
    Public Class ReconnectedLayer : Inherits Layer

    End Class

End Namespace