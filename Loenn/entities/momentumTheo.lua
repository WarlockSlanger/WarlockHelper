local drawableSprite = require("structs.drawable_sprite")
local mods = require("mods")
local wsUtils = mods.requireFromPlugin("libraries.utils")
local ftype = mods.requireFromPlugin("libraries.fieldTypes")

local momentumTheo = {}
momentumTheo.name = "WarlockHelper/MomentumTheoCrystal"

momentumTheo.depth = 100

momentumTheo.fieldOrder = {
	"x","y",
	"throwCrystalMomentum","throwCrystalSpeed",
	"throwPlayerMomentum","throwPlayerSpeed",
	"dropCrystalMomentum","dropCrystalSpeed",
	"dropPlayerMomentum","dropPlayerSpeed",
	"pickupCrystalMomentum","pickupPlayerMomentum",
	"pickupSpeed",
	"hasTheo",
	"playerBoost",
	"crystalBoost",
	
}
momentumTheo.fieldInformation = {
	throwCrystalMomentum = ftype.vector2,
	throwCrystalSpeed = ftype.vector2,
	throwPlayerMomentum = ftype.vector2,
	throwPlayerSpeed = ftype.vector2,
	dropCrystalMomentum = ftype.vector2,
	dropCrystalSpeed = ftype.vector2,
	dropPlayerMomentum = ftype.vector2,
	dropPlayerSpeed = ftype.vector2,
	pickupCrystalMomentum = ftype.vector2,
	pickupPlayerMomentum = ftype.vector2,
	pickupSpeed = ftype.vector2,
}

momentumTheo.placements = {
    {
        name = "normal",
        data = {
            hasTheo = false,
			throwCrystalMomentum = "0.6,0.6",
			throwCrystalSpeed = "200,-80",
			throwPlayerMomentum = "1,1",
			throwPlayerSpeed = "-80,0",
			dropCrystalMomentum = "0,0",
			dropCrystalSpeed = "0,0",
			dropPlayerMomentum = "1,1",
			dropPlayerSpeed = "0,0",
			pickupCrystalMomentum = "1,1",
			pickupPlayerMomentum = "1,1",
			pickupSpeed = "0,0",
			playerBoost = true,
			crystalBoost = true,
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