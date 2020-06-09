# VL.Devices.Kinect2
Set of nodes to use Kinect2 in vl.


Try it with vvvv, the visual live-programming environment for .NET  
Download: http://visualprogramming.net

## Requirements:
- [Kinect for Windows Runtime 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=44559)

## Using the library
In order to use this library with vl you have to install the nuget that is available via nuget.org. For information on how to use nugets with vl, see [Managing Nugets](https://vvvv.gitbooks.io/the-gray-book/content/en/reference/libraries/dependencies.html#_manage_nugets) in the vl documentation. As described there you go to the commandline and then type:

    nuget install VL.Devices.Kinect2 -prerelease

Once the VL.Devices.Kinect2 nuget is installed and referenced in your vl document you'll see the category "Kinect2" under "Devices" in the nodebrowser.

VL help patches can be found here:

    "VL.Devices.Kinect2\help\"

## Contributing to the development
If you want to contribute to this repository, clone it into a directory like:
 
    X:\vl-libs\VL.Devices.Kinect2

### Build the C# Project
Open

    X:\vl-libs\VL.Devices.Kinect2\src\VL.Devices.Kinect2
    
in VisualStudio and build it. This is necessary for a few things that cannot yet be expressed in vl directly, like inheritance.

#### Troubleshooting

If you are facing an error regarding VL.Core package you must add a nuget package source to Visual Studio which points to this repo: 

* http://teamcity.vvvv.org/guestAuth/app/nuget/v1/FeedService.svc/

### Reference VL.Devices.Kinect2.vl

In the vl document where you want to have access to the Kinect2 nodeset, add a dependency to:

	X:\vl-libs\VL.Devices.Kinect2\VL.Devices.Kinect2.vl

The available Kinect2 nodes should appear in the nodebrowser under Devices->Kinect2.

### Get Nuget Dependency
This wrapper is depending on one thirdparty nuget: [Microsoft.Kinect](https://www.nuget.org/packages/Microsoft.Kinect/). If you need to install this dependency manually go to your vvvv's

    \lib\packs (now in Documents\vvvv\gamma-preview)
    
on a commandline and run

    nuget.exe install Microsoft.Kinect
