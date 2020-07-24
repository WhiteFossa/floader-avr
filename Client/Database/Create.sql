-- Vendors names table
create table vendors_names
(
	Id		integer primary key not null  default 0,
	Name	text not null default 'Not set'
);

create unique index idx_vendors_names_id on vendors_names (id);-- Vendors names table

-- Devices names table
create table devices_names
(
	VendorId	integer not null default 0,
	ModelId	integer not null default 0,
	Name	text not null default 'Not set',
	primary key (VendorId, ModelId),
	foreign key(VendorId) references vendor_names(Id)
);

create unique index idx_devices_names_vendor_id_model_id on devices_names (VendorId, ModelId);

-- Devices data
create table devices_data_v1
(
	VendorId				integer not null default 0,
	ModelId				integer not null default 0,
	FlashPagesAll			integer not null default 0, -- How much FLASH pages device have
	FlashPagesWriteable	integer not null default 0, -- How much of them are writeable (i.e. not occupied by bootloader)
	FlashPageSize			integer not null default 0, -- FLASH page size in bytes
	EepromSize			integer not null default 0, -- EEPROM size in bytes
	primary key (VendorId, ModelId),
	foreign key(VendorId) references vendor_names(Id)
);

create unique index idx_devices_data_v1_vendor_id_model_id on devices_data_v1 (VendorId, ModelId);

-- Seeding some initial data
insert into
	vendors_names
	(Id, Name)
values
	(0, 'Example vendor'),
	(1, 'FetLab'),
	(984410, 'White Fossa');
	
insert into
    devices_names
    (VendorId, ModelId, Name)
values
    (0, 1, 'ATMega16 example device'),
    (0, 2, 'ATMega163 example device'),
    (0, 3, 'ATMega168 example device'),
    (1, 1, 'FTX-2 ARDF transmitter'),
    (984410, 1, 'Receiver for Redgerra');

insert into
    devices_data_v1
    (VendorId, ModelId, FlashPagesAll, FlashPagesWriteable, FlashPageSize, EepromSize)
values
    (0, 1, 128, 120, 128, 512),
    (0, 2, 128, 120, 128, 512),
    (0, 3, 128, 120, 128, 512),
    (1, 1, 128, 120, 128, 512),
    (984410, 1, 128, 120, 128, 512);
    
-- Add new vendors and devices below --    
    
