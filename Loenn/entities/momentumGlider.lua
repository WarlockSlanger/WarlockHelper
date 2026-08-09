local mods = require("mods")
local wsUtils = mods.requireFromPlugin("libraries.utils")
local ftype = mods.requireFromPlugin("libraries.fieldTypes")
local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawing = require("utils.drawing")

local momentumGlider = {}
momentumGlider.name = "WarlockHelper/MomentumGlider"

momentumGlider.depth = -5

momentumGlider.fieldOrder = {
	"x","y",
	"throwGliderMomentum","throwGliderSpeed",
	"throwPlayerMomentum","throwPlayerSpeed",
	"dropGliderMomentum","dropGliderSpeed",
	"dropPlayerMomentum","dropPlayerSpeed",
	"pickupGliderMomentum","pickupPlayerMomentum",
	"pickupSpeed",
	"bubble",
	"playerBoost","gliderBoost",
	
}
momentumGlider.fieldInformation = {
	throwGliderMomentum = ftype.vector2,
	throwGliderSpeed = ftype.vector2,
	throwPlayerMomentum = ftype.vector2,
	throwPlayerSpeed = ftype.vector2,
	dropGliderMomentum = ftype.vector2,
	dropGliderSpeed = ftype.vector2,
	dropPlayerMomentum = ftype.vector2,
	dropPlayerSpeed = ftype.vector2,
	pickupGliderMomentum = ftype.vector2,
	pickupPlayerMomentum = ftype.vector2,
	pickupSpeed = ftype.vector2,
}

momentumGlider.placements = {
    {
        name = "normal",
        data = {
            bubble = true,
			throwGliderMomentum = "0.6,0.6",
			throwGliderSpeed = "100,-40",
			throwPlayerMomentum = "1,1",
			throwPlayerSpeed = "-80,0",
			dropGliderMomentum = "0,0",
			dropGliderSpeed = "0,0",
			dropPlayerMomentum = "1,1",
			dropPlayerSpeed = "0,0",
			pickupGliderMomentum = "1,1",
			pickupPlayerMomentum = "1,1",
			pickupSpeed = "0,0",
			playerBoost = true,
			gliderBoost = true,
        }
    }
}

local texture = "objects/WarlockHelper/momentumGlider/idle00"

function momentumGlider.sprite(room, entity)
    local bubble = entity.bubble

    if entity.bubble then
        local x, y = entity.x or 0, entity.y or 0
        local points = drawing.getSimpleCurve({x - 11, y - 1}, {x + 11, y - 1}, {x - 0, y - 6})
        local lineSprites = drawableLine.fromPoints(points):getDrawableSprite()
        local jellySprite = drawableSprite.fromTexture(texture, entity)

        table.insert(lineSprites, 1, jellySprite)

        return lineSprites

    else
        return drawableSprite.fromTexture(texture, entity)
    end
end

function momentumGlider.selection(room, entity) 
	return wsUtils.selectRect(entity,-13,-14,28,17);
end

return momentumGlider