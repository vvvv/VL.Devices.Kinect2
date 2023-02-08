# VL.Devices.Kinect2
Set of nodes to use Kinect2 in VL.

Try it with vvvv, the visual live-programming environment for .NET  
Download: http://visualprogramming.net

## Requirements:
- [Kinect for Windows Runtime 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=44559)

## Using the library
In order to use this library you'll have to install the nuget that is available via nuget.org. For information on how to use nugets with VL, see [Managing Nugets](https://thegraybook.vvvv.org/reference/hde/managing-nugets.html) in the documentation. As described there you go to the commandline and then type:

    nuget install VL.Devices.Kinect2

Once the VL.Devices.Kinect2 nuget is installed and referenced in your VL document you'll see the category "Kinect2" under "Devices" in the nodebrowser. 

Demos are available via the Help Browser!

## Building from source
Beware there is an oddity required for building the included csproj:  
- The csproj references https://www.nuget.org/packages/Microsoft.Kinect 2.0.1410.19000 which includes a reference .dll that can no longer be loaded with .NET6.0
- Therefore we added the actual lib (taken from the GAC) to \lib\net6.0\Microsoft.Kinect.dll for shipping with this NuGet