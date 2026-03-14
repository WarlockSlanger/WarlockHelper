	local spriteSizeHalf = 11
	local boosterBumper = {}
	boosterBumper.name = "WarlockHelper/BoosterBumper"
	boosterBumper.depth = 0
	boosterBumper.nodeLineRenderType = "line"
function boosterBumper.texture(room, entity)
    local red = entity.red
    if red then
        return "objects/WarlockHelper/boosterBumper/idleRed30"

    else
        return "objects/WarlockHelper/boosterBumper/idle30"
    end
end
	function boosterBumper.selection(room, entity) 
		local x, y = entity.x or 0, entity.y or 0
		local nodes = entity.nodes or {}

		local nodeRects = {}
		for i, node in ipairs(nodes) do
			nodeRects[i] = utils.rectangle(node.x - spriteSizeHalf, node.y - spriteSizeHalf, spriteSizeHalf*2, spriteSizeHalf*2)
		end

		return utils.rectangle(x - spriteSizeHalf, y - spriteSizeHalf, spriteSizeHalf*2, spriteSizeHalf*2), nodeRects
	end
	boosterBumper.nodeLimits = {0, 1}
	boosterBumper.placements = {
		{
			name = "normal",
			data = {
				snapDirection = false,
				red = false,
				wobbling = false,
				direction = "1,0,0,1",
				respawnTime = 0.6,
				moveCycleTime = 1.8181819,
				wobbleRate = 0.44,
				wobbleStrength = 1.0,
			}
		},
		{
			name = "red",
			data = {
				snapDirection = false,
				red = true,
				wobbling = false,
				direction = "1,0,0,1",
				respawnTime = 0.6,
				moveCycleTime = 1.8181819,
				wobbleRate = 0.44,
				wobbleStrength = 1.0,
			}
		},
	}

	return boosterBumper