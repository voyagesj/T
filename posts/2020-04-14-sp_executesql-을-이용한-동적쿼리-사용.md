이해가 잘 되지 않아 직접 테스트해본 sp_executesql

동적쿼리를 사용할 경우 EXEC 문 보다 sp_executesql 이 성능상 더 좋다고 한다.

```SQL
/*
sp_executesql 을 이용한 동적쿼리 사용
  
EXECUTE sp_executesql [쿼리문], [파라메터 정의], [파라메터 할당]
*/

DECLARE @SiteId uniqueidentifier = '419506F3-4149-4969-8D68-A77C00E24A60'
DECLARE @sqlString nvarchar(500) = N'select * from tb_Items where SiteId = @SiteIId'
DECLARE @parameterDefinition nvarchar(500) = N'@SiteIId uniqueidentifier'


EXECUTE sp_executesql  @sqlString, @parameterDefinition, @SiteIId=@SiteId
```
