# C# data collection and visualisation projects

These projects were given to me in the university and are only for educational purposes.

## Data collection

Projects with a prefix `CPDT` are the ones that were created for collecting different kinds of data (mostly from a LIDAR) and performing manipulations with it such as segmentation, logging, etc.

- [CPDT_LR1](./CPDT_LR1/) collects data from a 2D LIDAR and performs segnemtation using.
- [CPDT_LR2](./CPDT_LR2/) is the same as [CPDT_LR1](./CPDT_LR1/), but with a 3D LIDAR and more complex.
- [CPDT_LR3](./CPDT_LR3/) collects, parses, visualises and logs messages between CAN modules with specific addresses.
- [CPDT_LR4](./CPDT_LR4/) performs simple UDP connection and transfering data between two forms via it.

## Data visualisation

Projects with a prefix `DVT` were created for data and statistics visualisation, mainly it would be points cloud, histograms, etc.

- [DVT_LR1](./DVT_LR1/) Generates a random 2D cloud of points and performs some statistics visualisation.
- [DVT_LR2](./DVT_LR2/) is the same as [DVT_LR1](./DVT_LR1/), but with a 3D cloud of points which is being drawn to as a Bitmap without any 3D engine.
- [DVT_LR3](./DVT_LR3/) is the same as [DVT_LR1](./DVT_LR1/), but with a 3D cloud of points, more complex and with OpenGL engine.
- [DVT_LR4](./DVT_LR4/) does data visualisation got from three CAN connections.

## Usage

Requires Visual Studio and .NET Framework >=4.8. Works only on Windows, sadly.

Download the VS solution with all of the projects, choose one of them and run it in the Visual Studio.