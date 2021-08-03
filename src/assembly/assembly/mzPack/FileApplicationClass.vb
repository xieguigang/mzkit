Imports System.ComponentModel

Namespace mzData.mzWebCache

    ''' <summary>
    ''' 当前的这个mzpack原始数据文件的应用程序类型
    ''' </summary>
    Public Enum FileApplicationClass
        ''' <summary>
        ''' 普通LC-MS非靶向
        ''' </summary>
        <Description("0212F3ED-1FA8-4CB3-B95D-33F43AD60945")> LCMS
        ''' <summary>
        ''' 普通GC-MS靶向/非靶向
        ''' </summary>
        <Description("EE446B9F-204F-4FF7-8302-1D4480FE46AB")> GCMS
        ''' <summary>
        ''' LC-MS/MS靶向
        ''' </summary>
        <Description("60866B4E-708F-4B94-9B22-E4C21D4D97DD")> LCMSMS
        ''' <summary>
        ''' GCxGC原始数据
        ''' </summary>
        <Description("B08A37D3-44BC-4D6C-86C9-D99726368446")> GCxGC
        ''' <summary>
        ''' 质谱成像
        ''' </summary>
        <Description("616DE505-3D78-45FA-BCA7-35ECEF3FE88D")> MSI
    End Enum
End Namespace