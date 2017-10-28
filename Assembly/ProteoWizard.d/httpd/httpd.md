# httpd [version 1.0.0.0]
> SMRUCC.REST.CLI

<!--more-->

**httpd: SMRUCC web cloud application platform**<br/>
_httpd: SMRUCC web cloud application platform_<br/>
Copyright © R&D, SMRUCC 2016. All rights reserved.

**Module AssemblyName**: file:///G:/GCModeller/src/runtime/httpd/HTTP_SERVER/demo/bin/httpd.exe<br/>
**Root namespace**: ``SMRUCC.REST.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/GET](#/GET)|Tools for http get request the content of a specific url.|


##### 1. httpdServerCLI

Server CLI for running this httpd web server.


|Function API|Info|
|------------|----|
|[/run](#/run)|Run start the web server with specific Web App.|
|[/start](#/start)|Run start the httpd web server.|

## CLI API list
--------------------------
<h3 id="/GET"> 1. /GET</h3>

Tools for http get request the content of a specific url.
**Prototype**: ``SMRUCC.REST.CLI::Int32 GET(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
httpd /GET /url [<url>/std_in] [/out <file/std_out>]
```


#### Arguments
##### /url
The resource URL on the web.

###### Example
```bash
/url <file/directory>
# (This argument can accept the std_out from upstream app as input)
```
##### [/out]
The save location of your requested data file.

###### Example
```bash
/out <file/directory>
# (This argument can output data to std_out)
```
##### Accepted Types
###### /url
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /out
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

<h3 id="/run"> 2. /run</h3>

Run start the web server with specific Web App.
**Prototype**: ``SMRUCC.REST.CLI::Int32 RunApp(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
httpd /run /dll <app.dll> [/port <80> /root <wwwroot_DIR>]
```
<h3 id="/start"> 3. /start</h3>

Run start the httpd web server.
**Prototype**: ``SMRUCC.REST.CLI::Int32 Start(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
httpd /start [/port 80 /root <wwwroot_DIR> /threads -1 /cache]
```


#### Arguments
##### [/port]
The server port of this httpd web server to listen.

###### Example
```bash
/port <int32>
```
##### [/root]
The website html root directory path.

###### Example
```bash
/root <file/directory>
# (This argument can accept the std_out from upstream app as input)
```
##### [/threads]
The number of threads of the server thread pool.

###### Example
```bash
/threads <int32>
```
##### [/cache]
Is this server running in file system cache mode? Not recommended for open.

###### Example
```bash
/cache
#(bool flag does not require of argument value)
```
##### Accepted Types
###### /port
**Decalre**:  _System.Int32_
Example: 
```json
0
```

###### /root
**Decalre**:  _System.String_
Example: 
```json
"System.String"
```

###### /threads
**Decalre**:  _System.Int32_
Example: 
```json
0
```

###### /cache
**Decalre**:  _System.Boolean_
Example: 
```json
true
```

