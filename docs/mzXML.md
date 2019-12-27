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