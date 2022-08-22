DECLARE @SRID VARCHAR(9) = 'SR695508' -- Update SR# here
declare @listOfIDs table (id int)
insert into @listOfIDs(id) Select * from STRING_SPLIT(@SROIDList, ',') -- If values are in a list, enter like (123),(456),(789)...
declare @supplierEligibilityDebitsToRemove table (id int)
Insert Into @supplierEligibilityDebitsToRemove 
	Select SupplierEligibilityDebitId from ManifestSupplierEligibilityDebits 
	where ManifestId in (select * from @listOfIDs);

--------------------------------------View the manifests----------------------------------------
select man.*, stat.[Description] from manifests man
join manifeststatus stat on man.CurrentStatusId = stat.Id
where man.id in (select * from @listOfIDs);

--------------------------------------Delete Applieds-------------------------------------------
Delete from SupplierEligibilityDebitsApplied 
where SupplierEligibilityDebitId in (	select SupplierEligibilityDebitId
										from ManifestSupplierEligibilityDebits
										where manifestid in (select * from @listOfIDs));

---------------------------------------Delete ManifestDebits-------------------------------------
Delete from ManifestSupplierEligibilityDebits where ManifestId in (select * from @listOfIDs);

---------------------------------------Delete Debits (list of deletes from spreadsheet)----------
/*
select * from SupplierEligibilityDebitsApplied 
where SupplierEligibilityDebitId in (	select SupplierEligibilityDebitId
										from ManifestSupplierEligibilityDebits
										where manifestid in (select * from @listOfIDs));
*/

DELETE
FROM SupplierEligibilityDebits WHERE Id IN(select * from @supplierEligibilityDebitsToRemove)

/*
select * from SupplierEligibilityDebitsApplied 
where SupplierEligibilityDebitId in (	select SupplierEligibilityDebitId
										from ManifestSupplierEligibilityDebits
										where manifestid in (select * from @listOfIDs));
*/

----------------------------------------Insert StatusChangeEvents for each Manifest--------------
--https://stackoverflow.com/questions/6606709/iterate-through-rows-in-sql-server-2008
declare @Id int
declare IDs CURSOR LOCAL FOR select ID from @listOfIDs

open IDs
FETCH NEXT FROM IDs into @Id

while @@FETCH_STATUS = 0
begin

	insert into ManifestStatusChangeEvents (ManifestId, ManifestStatusId, UserName, ChangeDate) values  (@Id, 2, @SRID, CURRENT_TIMESTAMP)
	-- select ManifestId , ManifestStatusId , UserName , ChangeDate from ManifestStatusChangeEvents where ManifestID in (@Id) order by ChangeDate desc

    fetch next from IDs into @Id

end

----------------------------------------Update Manifest Statuses-------------------------------------
-- select * from Manifests where Id in (select * from @listOfIDs);

update Manifests set CurrentStatusId = 2 where Id in (select * from @listOfIDs)

-- select * from Manifests where Id in (select * from @listOfIDs);

----------------------------------------Delete eligibility allocations. (No results is OK)-----------
-- select * from SupplierEligibilityAllocated where manifestid in (select * from @listOfIDs);

delete from SupplierEligibilityAllocated where manifestid in (select * from @listOfIDs);

-- select * from SupplierEligibilityAllocated where manifestid in (select * from @listOfIDs);

----------------------------------------Delete expected recovery records-----------------------------
-- select * from ExpectedRecovery where ManifestId in (select * from @listOfIDs);

delete from ExpectedRecovery where ManifestId in (select * from @listOfIDs);

-- select * from ExpectedRecovery where ManifestId in (select * from @listOfIDs);
