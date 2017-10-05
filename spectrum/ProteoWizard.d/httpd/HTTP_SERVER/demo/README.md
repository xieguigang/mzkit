When the project references were configed, switch the profile to ``DEMO``, and then compile the project. The compiled dll and httpd web server application was released to directory: [./bin/](./bin/)

![](./images/DEMO_profile.bmp)

Run the bash script [./start.sh](./start.sh) for start the demo web app, if the demo was running on Windows system, then allow the httpd web server pass through your firewall.

![](./images/uac.png)

At last, you can navigate to localhost, like [http://127.0.0.1/example/test.json](http://127.0.0.1/example/test.json) for test the demo web app.

##### Reference

+ Microsoft.VisualBasic.Architecture.Framework_v3.0_22.0.76.201__8da45dcd8060cc9a.dll
+ SMRUCC.WebCloud.HTTPInternal.dll

and put ``httpd.exe`` program into the same directory of your web app dll files.
