select
	VendorId,
	ModelId,
	Name
from
	devices_names
where
	VendorId = @VendorId
	and
	ModelId = @ModelId