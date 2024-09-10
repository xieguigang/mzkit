Public Interface IReportRender

    ReadOnly Property annotation As AnnotationPack
    Property colorSet As String()

    Function GetIon(xcms_id As String) As AlignmentHit

End Interface
