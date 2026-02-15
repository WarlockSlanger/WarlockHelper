	local dashDirController = {}
	dashDirController.name = "WarlockHelper/DashDirController"
	dashDirController.depth = -1000000
	dashDirController.texture = "objects/WarlockHelper/dashDirController"
	dashDirController.placements = {
		{
			name = "normal",
			data = {
				a = 1.0,
				b = 0.0,
				c = 0.0,
				d = 1.0,
			}
		},
	}

	return dashDirController