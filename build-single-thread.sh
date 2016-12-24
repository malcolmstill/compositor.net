#/bin/bash
mcs -debug+ -out:StarfuryST -r:WaylandServer.dll -r:ZxdgShellUnstableV6ServerProtocol.dll -r:OpenTK.dll StarfuryST.cs OutputGlobal.cs CompositorGlobal.cs ShellGlobal.cs SeatGlobal.cs DataDeviceManagerGlobal.cs SubcompositorGlobal.cs ZxdgShellUnstableV6Global.cs Compositor.cs Seat.cs Surface.cs XdgShellV6.cs Mode.cs DesktopMode.cs Pipeline.cs XkbCommon.cs Region.cs
