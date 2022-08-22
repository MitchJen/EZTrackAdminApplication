 
--Jeremy Ross

--declare @initials as varchar(20) = 'JER'
--declare @firstName as varchar(20) = 'Jeremy'
--declare @lastName as varchar(20) = 'Ross'
--declare @location as varchar(20) = 'ATLANTA'

--Add user as an Inspector
insert into Inspectors (Initials,FirstName,LastName,Location, IsActive)
values (@initials,@firstName,@lastName, @location, 1)


declare @nextVoiceOperatorId as int = (select top 1 id from VoiceOperators order by id desc) + 1
declare @Countrycode as int = 1
insert into VoiceOperators (Id, InspectorId, LaneId, PrinterId, PhysicalLocationCountryId) values
( @nextVoiceOperatorId, (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location), 10, 1, @Countrycode)
--OperatorId = 44

--Add voice functions for the newly added operator
insert into OperatorVoiceFunctions (OperatorId, VoiceFunctionId) values
( (select Id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location)), 1),
( (select Id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location)), 2),
( (select Id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location)), 3),
( (select Id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location)), 4),
( (select Id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location)), 5)

--Verify changes

/*
select * from OperatorVoiceFunctions 
	where OperatorId = (select id from VoiceOperators where InspectorId = (select id from Inspectors where FirstName = @firstName and LastName = @lastName and Initials = @initials and Location = @location))
*/


