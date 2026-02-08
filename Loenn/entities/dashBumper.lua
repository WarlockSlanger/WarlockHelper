	local dashBumper = {}
	dashBumper.name = "WarlockHelper/DashBumper"
	dashBumper.depth = 0
	dashBumper.nodeLineRenderType = "line"
	dashBumper.texture = "objects/Bumper/Idle22"
	dashBumper.nodeLimits = {0, 1}
	dashBumper.placements = {
		{
			name = "normal",
			data = {
				snapDirection = false,
				red = false,
				wobbling = false,
				dashSpeed = 240.0,
				respawnTime = 0.6,
				moveCycleTime = 1.8181819,
				sineCycleFreq = 0.44,
			}
		},
		{
			name = "red",
			data = {
				snapDirection = false,
				red = true,
				wobbling = false,
				dashSpeed = 240.0,
				respawnTime = 0.6,
				moveCycleTime = 1.8181819,
				sineCycleFreq = 0.44,
			}
		},
	}

	return dashBumper