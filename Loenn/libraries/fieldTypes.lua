local mods = require("mods")
--local logging = require("logging")
local wsUtils = mods.requireFromPlugin("libraries.utils")

local fieldTypes = {}
fieldTypes.matrix = {
	fieldType = "list",
	minimumElements = 6,
	maximumElements = 6,
	elementOptions = {
		fieldType = "number",
	},
	default = "1,0,0,1,0,0",
}

fieldTypes.vector2 = {
	fieldType = "list",
	minimumElements = 2,
	maximumElements = 2,
	elementOptions = {
		fieldType = "number",
	},
	default = "0,0",
}

fieldTypes.depth = {
	fieldType = "integer",
	default = 0,
}

return fieldTypes