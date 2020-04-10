
SharePoint Document library item 모두 삭제
```C#
public class HendleSPListItem
{
	// 아이템 삭제 메서드 진입점
	public static void RemoveListItems(string url)
	{
		SPSecurity.RunWithElevatedPrivileges(delegate ()
		{
			using (SPSite site = new SPSite(url))
			using (SPWeb web = site.RootWeb)
			{
				web.AllowUnsafeUpdates = true;
				SPList list = web.GetList(url);
				using (DisabledEventsScope ds = new DisabledEventsScope())
				{
					RemoveFolderItems(list.RootFolder);
				}
				web.AllowUnsafeUpdates = false;
			}
		});
	}
	
	//삭제 함수(재귀). 해당폴더의 파일을 삭제 후, 하위 폴더가 없으면 폴더 삭제.
	private static void RemoveFolderItems(SPFolder folder)
	{
		for (int i = folder.Files.Count; i > 0; i--)
		{
			if (folder.Files[i - 1].Name == "Web Part View.aspx" || folder.Files[i - 1].Name == "RecentItem.aspx") continue;
			Console.WriteLine(folder.Files[i - 1].ServerRelativeUrl);
			folder.Files[i - 1].Delete();
		}
		//하위폴더가 있으면 하위폴더 삭제 재귀
		if(folder.SubFolders.Count > 0)
		{
			SPFolder sFolder = null;
			for (int i = folder.SubFolders.Count; i > 0; i--)
			{
				sFolder = folder.SubFolders[i - 1];
				if (sFolder.Name == "Forms") continue;
				RemoveFolderItems(sFolder);
			}
		}
                //하위폴더 수 다시체크. 없으면 현재폴더 삭제
                if (folder.SubFolders.Count == 0) { folder.Delete(); }
	}
}
//이벤트 리시버 비활성화 처리
public sealed class DisabledEventsScope : SPItemEventReceiver, IDisposable
{
	bool _originalValue;
	public DisabledEventsScope()
	{
		// Save off the original value of EventFiringEnabled 
		_originalValue = base.EventFiringEnabled;
		// Set EventFiringEnabled to false to disable it 
		base.EventFiringEnabled = false;
	}
	public void Dispose()
	{
		// Set EventFiringEnabled back to its original value 
		base.EventFiringEnabled = _originalValue;
	}
}
 ```

