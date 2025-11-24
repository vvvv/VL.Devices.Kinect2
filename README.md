# VL.Devices.Kinect2
A package for using Kinect2 depth cameras by Microsoft in VL.

For use with vvvv, the visual live-programming environment for .NET: http://vvvv.org

## Requirements:
- [Kinect for Windows Runtime 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=44559)

## Getting started
- Install as [described here](https://thegraybook.vvvv.org/reference/hde/managing-nugets.html) via commandline:

    `nuget install VL.Devices.Kinect2`

- Usage examples and more information are included in the pack and can be found via the [Help Browser](https://thegraybook.vvvv.org/reference/hde/findinghelp.html)

## Contributing
- Report issues on [the vvvv forum](https://forum.vvvv.org/c/vvvv-gamma/28)
- For custom development requests, please [get in touch](mailto:devvvvs@vvvv.org)
- When making a pull-request, please make sure to read the general [guidelines on contributing to vvvv libraries](https://thegraybook.vvvv.org/reference/extending/contributing.html)

## Building from source
Beware there is an oddity required for building the included csproj:  
- The csproj references https://www.nuget.org/packages/Microsoft.Kinect 2.0.1410.19000 which includes a reference .dll that can no longer be loaded with .NET6.0
- Therefore we added the actual lib (taken from the GAC) to \lib\net6.0\Microsoft.Kinect.dll for shipping with this NuGet
