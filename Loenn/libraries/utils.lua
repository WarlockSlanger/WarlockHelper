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

function wsUtils.centeredRect(x,y,w,h)
	return utils.rectangle(x-w/2,y-h/2,w,h)
end

function wsUtils.selectRect(entity,size)
	local x, y = entity.x or 0, entity.y or 0
	local nodes = entity.nodes or {}

	local nodeRects = {}
	for i, node in ipairs(nodes) do
		nodeRects[i] = wsUtils.centeredRect(node.x, node.y, size, size)
	end

	return wsUtils.centeredRect(x, y, size, size), nodeRects
end

return wsUtils