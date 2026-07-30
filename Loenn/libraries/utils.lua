local wsUtils = {}

function wsUtils.strSplit(value,delim)
	local array = {}
	value = value..delim
	local count=0
	local ind=1
	while true do
		local i,j = string.find(value, delim, ind)
		if i == nil then break end
		count = count+1
		array[count] = string.sub(value,ind,i-1)
		ind = j+1
	end
	return array
end
function wsUtils.setDiff(arr1,arr2)
	local set2={}
	local set ={}
	for _,v in ipairs(arr2) do
		set2[v]=true
	end
	for _,v in ipairs(arr1) do
		if set2[v] == nil then
			table.insert(set,v)
		end
	end
	return set
end

function wsUtils.selectRect(entity,offsetX,offsetY,sizeX,sizeY)
	local x, y = entity.x+offsetX or 0, entity.y+offsetY or 0
	local nodes = entity.nodes or {}

	local nodeRects = {}
	for i, node in ipairs(nodes) do
		nodeRects[i] = utils.rectangle(node.x+offsetX, node.y+offsetY, sizeX, sizeY)
	end

	return utils.rectangle(x, y, sizeX, sizeY), nodeRects
end

return wsUtils