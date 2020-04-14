import { Guid } from '@microsoft/sp-core-library';
import { override } from '@microsoft/decorators';
import { Log } from '@microsoft/sp-core-library';
import {
  BaseListViewCommandSet,
  Command,
  IListViewCommandSetListViewUpdatedParameters,
  IListViewCommandSetExecuteEventParameters
} from '@microsoft/sp-listview-extensibility';
import { Dialog } from '@microsoft/sp-dialog';

//
// jquery 사용을 위해서는 Command 에서 다음의 명령을 실행
// npm install jquery --save
// npm install --save @types/jquery
//
import * as $ from "jquery";
import * as strings from 'SpfxEcbExtensionCommandSetStrings';

/**
 * If your command set uses the ClientSideComponentProperties JSON input,
 * it will be deserialized into the BaseExtension.properties object.
 * You can define an interface to describe it.
 */
export interface ISpfxEcbExtensionCommandSetProperties {
  // This is an example; replace with your own properties
  sampleTextOne: string;  
}

const LOG_SOURCE: string = 'SpfxEcbExtensionCommandSet';

export default class SpfxEcbExtensionCommandSet extends BaseListViewCommandSet<ISpfxEcbExtensionCommandSetProperties> {

  @override
  public onInit(): Promise<void> {
    Log.info(LOG_SOURCE, 'Initialized SpfxEcbExtensionCommandSet');
    return Promise.resolve();
  }

  @override
  public onListViewUpdated(event: IListViewCommandSetListViewUpdatedParameters): void {
    const compareOneCommand: Command = this.tryGetCommand('COMMAND_1');
    if (compareOneCommand) {
      // This command should be hidden unless exactly one row is selected.
      compareOneCommand.visible = event.selectedRows.length === 1;
    }
  }

  @override
  public onExecute(event: IListViewCommandSetExecuteEventParameters): void {
    switch (event.itemId) {
      case 'COMMAND_1':
        //Dialog.alert(`${this.properties.sampleTextOne}`);
        const itemUrl: string = event.selectedRows[0].getValueByName("FileRef");
        const itemId: number = event.selectedRows[0].getValueByName("UniqueId");
        const listId: Guid = this.context.pageContext.list.id;        

        

        //$.ajax({
        //  url: "https://api.wspw.sharepoint.com/api/DownloadFile"
        //  , type: "post"
        //  , data: { "fileUrl": "/sites/SPTeamGroup/Shared Documents/General/202001_월간보고.docx" }
        //  , async: false
        //}).done(data => Dialog.alert(data));
      //test jQuery
      $(document).ready(function (){
        alert(`URL=${itemUrl}&itemId=${itemId}&listId=${listId}`);
      });

        //Dialog.alert(`URL=${itemUrl}&itemId=${itemId}&listId=${listId}`);

        break;
     
      default:
        throw new Error('Unknown command');
    }
  }
}
