#/bin/bash
mcs -debug+ -r:WaylandServer.dll -r:ZxdgShellUnstableV6ServerProtocol.dll -r:OpenTK.dll Starfury.cs OutputGlobal.cs CompositorGlobal.cs ShellGlobal.cs SeatGlobal.cs DataDeviceManagerGlobal.cs SubcompositorGlobal.cs ZxdgShellUnstableV6Global.cs Compositor.cs Seat.cs Surface.cs XdgShellV6.cs
