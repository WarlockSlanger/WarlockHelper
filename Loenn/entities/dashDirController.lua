	local dashDirController = {}
	dashDirController.name = "WarlockHelper/DashDirController"
	dashDirController.depth = -1000000
	dashDirController.texture = "objects/WarlockHelper/dashDirController"
	dashDirController.placements = {
		{
			name = "normal",
			data = {
				direction = "1,0,0,1",
				neutralLeft = "-1,0",
				neutralRight = "1,0",
			}
		},
		{
			name = "map",
			data = {
				right="1,0",
				downRight="0.70710677,0.70710677",
				down="0,1",
				downLeft="-0.70710677,0.70710677",
				left="-1,0",
				upLeft="-0.70710677,-0.70710677",
				up="0,-1",
				upRight="0.70710677,-0.70710677",
				neutralLeft = "-1,0",
				neutralRight = "1,0",
			}
		}
	}

	return dashDirController