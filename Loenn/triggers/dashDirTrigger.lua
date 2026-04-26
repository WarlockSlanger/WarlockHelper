local mods = require("mods")
local wsUtils = mods.requireFromPlugin("libraries.utils")
local ftype = mods.requireFromPlugin("libraries.fieldTypes")

--local logging = require("logging")

local dashDirTrigger = {}
dashDirTrigger.name = "WarlockHelper/DashDirTrigger"

function dashDirTrigger.ignoredFields (entity)
	local allFields = {
	"_name", "_id", "originX", "originY",
	"height","width",
	"_mode","persistent",
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
	show("_mode")
	show("persistent")
	
	if entity._mode ~= -1 then
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
	end
	return wsUtils.setDiff(allFields,visible)
end

dashDirTrigger.fieldOrder = {
	"x","y",
	"height","width",
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
	"_mode","persistent"
}
dashDirTrigger.fieldInformation = {
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
	_mode = {
		fieldType=integer,
		options={
			{"Disable",-1},
			{"Matrix",0},
			{"Replace",1},
		},
		editable=false,
	}
}
	dashDirTrigger.placements = {
		{
			name = "normal",
			data = {
				_mode = 0,
				persistent=true,
				direction = ftype.matrix.default,
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
				overrideNeutral=false,
			}
		}
	}

	return dashDirTrigger