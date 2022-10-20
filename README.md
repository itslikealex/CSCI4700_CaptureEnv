# Capture Environment WPF App
## MTSU • Software Engineering • CSCI-4700 • Dr. Medha Sarkar 

Application that makes it easy to capture multiple video feeds simultaneously. Written in C# and XAML with Microsofts WPF framework. Uses DirectX for live video as well as the Capture Manager NuGet package.


## How to use

Make sure you have .NET desktop development components installed in Visual Studio.

Download VLC to debug and view the .asf

Open the solution file (CSCI4700_CaptureEnv.sln) and if there is a window that pops up asking to upgrade to a newer version of .NET 4.x do not change the selection, just press continue. 

Press the green play button to run the app and start debugging.




## To-Do Items
* [DONE] Separate the video file into multiple files (see .asf 'Advanced Systems Format' file format) because all the video streams are in that one file https://www.codeproject.com/Articles/1017223/CaptureManager-SDK-Capturing-Recording-and-Streami#fourtyfourthdemoprogram

* 1. Set the default fps of any screen capture to 30fps instead of 1fps
* 2. After splitting the files, we will need to get the files into one video by stitching them together.
* 3. Add a time-elapsed overlay on the stitched video.

* 4. Create an error dialog to show instead of using the limited MessageBox.Show()
* 5. Change GUI color and make them all match



* Upgrade GUI make it look better and more customized.
* More to come as we progress.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)