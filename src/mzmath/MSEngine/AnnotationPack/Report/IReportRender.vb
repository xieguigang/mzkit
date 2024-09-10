Public Interface IReportRender

    ReadOnly Property annotation As AnnotationPack

    Function GetIon(xcms_id As String) As AlignmentHit

End Interface
