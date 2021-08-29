Namespace PolygonEditor

    Friend Enum MenuOption
        MoveComponent
        AddVertex
        DeleteVertex
        MovePolygon
        RemovePolygon
        HalveEdge
        AddRelation
        RemoveRelation
    End Enum

    Friend Enum Relation
        None
        Equality
        Perpendicular
    End Enum

    Friend Enum Algorithm
        Bresenham
        Library
        Antialiasing
        BresenhamSymmetric
    End Enum
End Namespace