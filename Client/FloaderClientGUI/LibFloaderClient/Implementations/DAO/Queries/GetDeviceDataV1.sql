select
	VendorId,
	ModelId,
	FlashPagesAll,
	FlashPagesWriteable,
	FlashPageSize,
	EepromSize
from
	devices_data_v1
where
	VendorId = @VendorId
	and
	ModelId = @ModelId