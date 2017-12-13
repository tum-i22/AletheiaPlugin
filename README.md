# AletheiaPlugin

## 1. Requirement

Aletheia and OpenCppCoverage must be installed (i.e. they must be available through System PATH variable)

## 2. How to run this solution
a) Clone or download this project (i.e. ```clone https://github.com/tum-i22/AletheiaPlugin.git```)<br />
b) Go to the cloned or downloaded folder<br />
c) Open the ```AletheiaPlugin.sln``` with Visual Studio. <br />
d) Execute in Debug mode to get the experimental Environment<br />
e) Load a <b>Gtest project</b> in Experimental Environment.<br />
f) Select A project in the Visual Studio Solution Explorer <br />
g) Run ```Tool -> Aletheia-GenerateHitSpectra``` <br />
h) The plugin will run in background releasing Visual Studio environment. <br />
i) The HitSpectra will be generated in ```C:\HitSpectras``` folder by default

## 3. Recommendation and Precaution
Plugin is much slower than the app itself. Please give the plugin time to generate hit spectra. If you have a bigger project with a lot of test cases, please use the App instead as the plugin is much slower. <br />
Aletheia and Aletheia Plugin are both computation hungry. If you are using an old computer, these tools might cause your system to hang.

## 4. Contact
In case of question, simply email us at<br >

Mojdeh Golagha (golagha@in.tum.de)<br />
Abu Mohammed Raisuddin (am.raisuddin@tum.de)
