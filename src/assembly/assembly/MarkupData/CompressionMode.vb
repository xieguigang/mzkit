Namespace MarkupData

    Public Enum CompressionMode
        zlib
        gzip
        none
    End Enum

    <HideModuleName>
    Module CompressionModeParser

        Friend ReadOnly charToModes As New Dictionary(Of String, CompressionMode) From {
            {"gzip", CompressionMode.gzip}, ' mzXML
            {"zlib", CompressionMode.zlib},
            {"none", CompressionMode.none},
 _
            {"zlib compression", CompressionMode.zlib}, ' mzML/imzML
            {"gzip compression", CompressionMode.gzip},
            {"no compression", CompressionMode.none}
        }

    End Module
End Namespace