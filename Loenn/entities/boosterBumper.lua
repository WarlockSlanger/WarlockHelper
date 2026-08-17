local mods = require("mods")
local wsUtils = mods.requireFromPlugin("libraries.utils")
local ftype = mods.requireFromPlugin("libraries.fieldTypes")

local boosterBumper = {}
boosterBumper.name = "WarlockHelper/BoosterBumper"
function boosterBumper.depth(room, entity)
	return entity.Depth or 0
end

boosterBumper.nodeLineRenderType = "line"

function boosterBumper.texture(room, entity)
	local red = entity.red
	if red then
		return "objects/WarlockHelper/boosterBumper/idleRed30"
	end
	return "objects/WarlockHelper/boosterBumper/idle30"
end

function boosterBumper.selection(room, entity) 
	return wsUtils.selectRect(entity,-11,-11,22,22);
end

boosterBumper.fieldInformation = {
	Depth = ftype.depth,
	direction = ftype.matrix,
}

boosterBumper.nodeLimits = {0, 1}
--[[boosterBumper.placements = {
	{
		name = "normal",
		data = {
			Depth=2000,
			snapDirection = false,
			red = false,
			wobbling = false,
			silentDash = true,
			dashCooldown = true,
			dashInterrupt = false,
			dashSuper = false,
			snapPosition = false,
			direction = ftype.matrix.default,
			respawnTime = 0.6,
			moveCycleTime = 1.8181819,
		}
	},
	{
		name = "red",
		data = {
			Depth=2000,
			snapDirection = false,
			red = true,
			wobbling = false,
			silentDash = true,
			dashCooldown = true,
			dashInterrupt = true,
			dashSuper = false,
			snapPosition = false,
			direction = ftype.matrix.default,
			respawnTime = 0.6,
			moveCycleTime = 1.8181819,
		}
	},
}]]

return boosterBumper