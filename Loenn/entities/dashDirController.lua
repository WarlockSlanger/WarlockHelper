local mods = require("mods")
local wsUtils = mods.requireFromPlugin("libraries.utils")
local ftype = mods.requireFromPlugin("libraries.fieldTypes")

--local logging = require("logging")

local dashDirController = {}
dashDirController.name = "WarlockHelper/DashDirController"
dashDirController.depth = -1000000
dashDirController.texture = "objects/WarlockHelper/dashDirController"

function dashDirController.ignoredFields (entity)
	local allFields = {
	"_name", "_id", "originX", "originY",
	"_mode",
	"right",
	"downRight",
	"down",
	"downLeft",
	"left",
	"upLeft",
	"up",
	"upRight",
	"neutralLeft",
	"neutralRight",
	"direction",
	"overrideNeutral",
	}
	local visible = {}
	local function show(...)
		local arg = {...}
		for _,v in ipairs(arg) do
			table.insert(visible,v)
		end
	end
	show("overrideNeutral")
	if entity.overrideNeutral then
		show("neutralLeft","neutralRight")
	end
	if entity._mode == 0 then
		show("direction")
	end
	if entity._mode == 1 then
		show(
			"right",
			"downRight",
			"down",
			"downLeft",
			"left",
			"upLeft",
			"up",
			"upRight"
		)
	end
	return wsUtils.setDiff(allFields,visible)
end

dashDirController.fieldOrder = {
	"x","y",
	"right",
	"downRight",
	"down",
	"downLeft",
	"left",
	"upLeft",
	"up",
	"upRight",
	"neutralLeft",
	"neutralRight",
	"direction",
	"overrideNeutral",
}
dashDirController.fieldInformation = {
	direction = ftype.matrix,
	right=ftype.vector2,
	downRight=ftype.vector2,
	down=ftype.vector2,
	downLeft=ftype.vector2,
	left=ftype.vector2,
	upLeft=ftype.vector2,
	up=ftype.vector2,
	upRight=ftype.vector2,
	neutralLeft = ftype.vector2,
	neutralRight = ftype.vector2,
}
	dashDirController.placements = {
		{
			name = "normal",
			data = {
				_mode = 0,
				direction = ftype.matrix.default,
				neutralLeft = "-1,0",
				neutralRight = "1,0",
				overrideNeutral=false,
			}
		},
		{
			name = "map",
			data = {
				_mode = 1,
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
				overrideNeutral = false,
			}
		}
	}

	return dashDirController