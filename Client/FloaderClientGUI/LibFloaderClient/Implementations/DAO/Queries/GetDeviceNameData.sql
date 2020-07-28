select
	VendorId,
	ModelId,
	Name
from
	devices_names
where
	VendorId = @vendorId
	and
	ModelId = @modelId