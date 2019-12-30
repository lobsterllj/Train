#	This is a mysql script
create database if not exists RBC_TRAIN;
use RBC_TRAIN;
create table if not exists TrainInfo(
	TrainID int primary key not null 
		comment 'primary key is not null and unique',
	L_TRAIN int ,
	NID_ENGINE int not null,
	NID_XUSER int,
	NID_OPERATIONAL varchar(50),
	NC_TRAIN int,
	V_MAXTRAIN int,
	M_LOADINGGAUGE int,
	M_AXLELOAD int,
	M_AIRTIGHT int
)comment='列车配置信息表';
create table if not exists TrainSTMType(
	ID int primary key,
	TrainID int,
	NID_STM int,
	foreign key(TrainID) references TrainInfo(TrainID)
);
create table if not exists TrainRadioNumber(
	ID int primary key,
	TrainID int,
	NID_RADIO varchar(50),
	foreign key(TrainID) references TrainInfo(TrainID)
);
create table if not exists TrainTractionType(
	ID int primary key,
	TrainID int,
	M_TRACTION int,
	foreign key(TrainID) references TrainInfo(TrainID)
);
