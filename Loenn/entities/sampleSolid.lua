	local fakeTilesHelper = require("helpers.fake_tiles")
	local sampleSolid = {}

	sampleSolid.name = "WarlockHelper/SampleSolid"
	sampleSolid.fieldInformation = {
		dashCollisionResult = {
			options = {"Rebound","NormalCollision","NormalOverride","Bounce","Ignore"},
			editable = false,
		},
	}
	sampleSolid.fieldInformation = fakeTilesHelper.addTileFieldInformation(sampleSolid.fieldInformation,"tiletype")
	sampleSolid.placements = function ()
		return {
			name = "normal",
			data = {
				tiletype = fakeTilesHelper.getPlacementMaterial(),
				width = 8,
				height = 8,
				dashCollisionResult = "Rebound",
			}
		}
	end
	sampleSolid.depth = 0
	sampleSolid.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", true, "tilesFg", {1.0, 1.0, 1.0, 1.0})

	return sampleSolid