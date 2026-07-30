local drawableSprite = require("structs.drawable_sprite")
local mods = require("mods")
local wsUtils = mods.requireFromPlugin("libraries.utils")



local momentumTheo = {}
momentumTheo.name = "WarlockHelper/MomentumTheoCrystal"

momentumTheo.depth = 100
momentumTheo.placements = {
    {
        name = "normal",
        data = {
            hasTheo = false,
			momentumDrop = false,
			momentumThrow = true,
			momentumPickup = true
        }
    }
}

local texture1 = "objects/WarlockHelper/momentumTheo/idle00"
local texture2 = "objects/WarlockHelper/momentumTheo/idleTheo00"

function momentumTheo.sprite(room, entity)
	
	local texture = (entity.hasTheo ? texture2 : texture1)
	
    local sprite = drawableSprite.fromTexture(texture, entity)

    sprite.y += -10

    return sprite
end

function momentumTheo.selection(room, entity) 
	return wsUtils.selectRect(entity,-11,-21,21,22);
end

return momentumTheo