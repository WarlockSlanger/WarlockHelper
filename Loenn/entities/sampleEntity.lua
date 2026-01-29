	local sampleEntity = {}
	sampleEntity.name = "WarlockHelper/DashBumper"
	sampleEntity.depth = 0
	sampleEntity.nodeLineRenderType = "line"
	sampleEntity.texture = "objects/Bumper/Idle22"
	sampleEntity.nodeLimits = {0, 1}
	sampleEntity.placements = {
		{
			name = "normal",
			data = {
				snapDirection = false,
				red = false,
			}
		},
		{
			name = "red",
			data = {
				snapDirection = false,
				red = true,
			}
		},
	}

	return sampleEntity