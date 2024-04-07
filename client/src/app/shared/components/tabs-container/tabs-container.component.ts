import {AfterContentInit, AfterViewInit, Component, ContentChildren, QueryList} from '@angular/core';
import {TabComponent} from "../tab/tab.component";

@Component({
  selector: 'app-tabs-container',
  templateUrl: './tabs-container.component.html',
  styleUrls: ['./tabs-container.component.scss']
})
export class TabsContainerComponent implements AfterContentInit{
  @ContentChildren(TabComponent) tabs?: QueryList<TabComponent>


  constructor() {
  }

  selectTab(tabTitle: string) {
    this.tabs?.forEach(t => t.active = t.tabTitle === tabTitle)
    return false;
  }

  ngAfterContentInit(): void {
      if(this.tabs) {
        this.tabs.first.active = true;
      }
  }
}
