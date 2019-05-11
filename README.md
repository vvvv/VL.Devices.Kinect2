# VL.Devices.Kinect2
Set of nodes to use Kinect2 in vl.

## Using the library
At the moment in order to use the library you need to build the project first and then reference VL.Devices.Kinect2.vl from your vl document. See the section titled "Build the C# Project" for information on building.

VL patches can be found here:

    "VL.Devices.Kinect2\help\"

## Contributing to the development
If you want to contribute to this repository, clone it into a directory like:
 
    X:\vl-libs\VL.Devices.Kinect2

### Build the C# Project
Open

    X:\vl-libs\VL.Devices.Kinect2\src\VL.Devices.Kinect2
    
in VisualStudio and build it. This is necessary for a few things that cannot yet be expressed in vl directly, like inheritance.

### Reference VL.Devices.Kinect2.vl

In the vl document where you want to have access to the Kinect2 nodeset, add a dependency to:

	X:\vl-libs\VL.Devices.Kinect2\VL.Devices.Kinect2.vl

The available Kinect2 nodes should appear in the nodebrowser under Devices->Kinect2.

### Get Nuget Dependency
This wrapper is depending on one thirdparty nuget: [Microsoft.Kinect](https://www.nuget.org/packages/Microsoft.Kinect/). If you need to install this dependency manually go to your vvvv's

    \lib\packs (now in Documents\vvvv\gamma-preview)
    
on a commandline and run

    nuget.exe install Microsoft.Kinect
