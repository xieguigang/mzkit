# Decode of the raw data

The mzXMl and mzML file both contains a element node that store the bytes raw data in base64 format. And the bytes raw data is also compressed in zip format or gzip format. So you must decompress that raw data bytes by zip or gzip at first, and then you are able to get 32/64bit floats raw data tuples from the raw data.

## VB.NET demo for decode mzXML/mzML

In mzkit library, we have a ``IBase64Container`` data model for unify the decode method of the raw data in mzXML or mzML files:

```vbnet
Public Interface IBase64Container
    Property BinaryArray As String

    Function GetPrecision() As Integer
    Function GetCompressionType() As String
End Interface
```

And then we can decode of the base64 raw data in folowing operation sequence: 

1. get raw bytes from base64; 
2. decompress data by zlib or gzip; 
3. reverse raw byte vector if in networkByteOrder; 
4. split vector in bytes by 4(for 32bit) or 8(for 64 bit); 
5. get floats data values and then converts to mz/into or into/time data tuples.

Here is how:

```vbnet
<Extension> 
Public Function Base64Decode(stream As IBase64Container, Optional networkByteOrder As Boolean = False) As Double()
    ' 1. get raw bytes from base64; 
    Dim bytes As Byte() = Convert.FromBase64String(stream.BinaryArray)
    Dim floats#()
    Dim byteStream As MemoryStream

    ' 2. decompress data by zlib or gzip; 
    Select Case stream.GetCompressionType
        Case "zlib"
            byteStream = bytes.UnZipStream
        Case "gzip"
            byteStream = bytes.UnGzipStream
        Case Else
            Throw New NotImplementedException
    End Select

    Using byteStream
        bytes = byteStream.ToArray
    End Using

    ' 3. reverse raw byte vector if in networkByteOrder; 
    If networkByteOrder AndAlso BitConverter.IsLittleEndian Then
        Call Array.Reverse(bytes)
    End If

    ' 4. split vector in bytes by 4(for 32bit) or 8(for 64 bit);
    Select Case stream.GetPrecision
        Case 64
            floats = bytes _
                .Split(8) _
                .Select(Function(b) BitConverter.ToDouble(b, Scan0)) _
                .ToArray
        Case 32
            floats = bytes _
                .Split(4) _
                .Select(Function(b) BitConverter.ToSingle(b, Scan0)) _
                .Select(Function(s) Val(s)) _
                .ToArray
        Case Else
            Throw New NotImplementedException
    End Select

    ' 5. get floats data values and then converts to mz/into 
    ' or into/time data tuples.
    Return floats
End Function
```

## Perl demo for decode mzXML/mzML

> http://www.mail-archive.com/spctools-discuss@googlegroups.com/msg04769.html

The script below extracts the following correct-looking ``m/z`` intensity values from the ``base64`` string on stock perl v5.10.1 on ScientificLinux 6.3 
64-bit.

```
300.282879284137
61862.0788574219
302.008202550558
88391.3453369141
302.85572635976
29830.3859863281
303.910843219307
348281.520751953
304.847300410523
58940.4178466797
... etc ....
```

```perl
#!/usr/bin/perl

use strict;
use warnings;
use Compress::Zlib qw(uncompress);
use MIME::Base64;

# Data is Base64 encoded and compressed with zlib
my $data = <YOUR DATA HERE>

# Decode and the uncompress
my $base64decoded =  uncompress( decode_base64($data) );

# Data is in 64-bit floats in network order
# Unpack as a 64-bit network order quad int
my @hostOrder = unpack("Q>*", $base64decoded );

foreach my $i (@hostOrder) {

    # Pack into a native quad then unpack into the correct 64-bit float
    my $val = ( unpack("d", pack("Q", $i ) ) );

    print "$val\n";
}
```